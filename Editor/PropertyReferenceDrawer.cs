using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Extra.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extra.Editor.Properties
{
    [CustomPropertyDrawer(typeof(PropertyReference<>), true)]
    public class PropertyReferenceDrawer : PropertyDrawer
    {
        private readonly Dictionary<Type, MemberInfo[]> _cachedMemberInfosMap = new();

        private PropertyReference _subject;
        private StringSearchProvider _searchProvider;

        private string[] _membersList;

        private SearchDepthAttribute Attribute => attribute as SearchDepthAttribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var positionNoLabel = EditorGUI.PrefixLabel(position, label);

            // Retrieve props
            var objProp = property.FindPropertyRelative("root");
            var pathProp = property.FindPropertyRelative("propertyPath");

            // Set up rects
            var left = positionNoLabel;
            left.width *= 0.5f;
            var right = left;
            right.x += left.width;

            // Populate obj from field
            EditorGUI.BeginChangeCheck();
            objProp.objectReferenceValue = EditorGUI.ObjectField(left, objProp.objectReferenceValue, typeof(Object), true);
            var changed = EditorGUI.EndChangeCheck();

            if (objProp.objectReferenceValue == null)
            {
                EditorGUI.LabelField(right, "No object in field.");
                return;
            }

            if (changed || _membersList == null)
            {
                // Find suitable members
                var targetType = fieldInfo.FieldType.GenericTypeArguments[0];
                var foundFields = TryGetMembersOfTargetType(targetType, objProp.objectReferenceValue.GetType(), out _membersList);
                if (!foundFields)
                {
                    pathProp.stringValue = string.Empty;
                    return;
                }
            }

            if (changed)
            {
                pathProp.stringValue = _membersList[0];
            }

            if (_membersList.Length <= 0)
            {
                EditorGUI.LabelField(right, "No properties found.");
                return;
            }

            // Check if dropdown clicked
            var buttonClicked = EditorGUI.DropdownButton(right, new GUIContent(pathProp.stringValue), FocusType.Keyboard);
            if (!buttonClicked) return;

            // Set up SearchWindow and provider
            if (!_searchProvider) _searchProvider = ScriptableObject.CreateInstance<StringSearchProvider>();
            _searchProvider.Construct(_membersList, x =>
            {
                pathProp.stringValue = x;
                pathProp.serializedObject.ApplyModifiedProperties();
            });
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), _searchProvider);
        }

        private bool TryGetMembersOfTargetType(Type targetType, Type rootType, out string[] membersList)
        {
            membersList = RecursiveGetMembersOfTargetType(targetType, rootType, maxDepth: Attribute?.SearchDepth ?? 1).ToArray();
            return membersList.Length > 0;
        }

        private IEnumerable<string> RecursiveGetMembersOfTargetType(Type targetType, Type rootType, string prefix = "", int depth = 0, int maxDepth = 1)
        {
            if (depth > maxDepth) yield break;

            var members = SmartGetMembers(rootType);
            foreach (var item in members.Where(IsGettable))
            {
                var itemType = item switch
                {
                    FieldInfo fi => fi.FieldType,
                    PropertyInfo pi => pi.PropertyType,
                    MethodInfo mi => mi.ReturnType,
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (itemType == targetType) yield return $"{prefix}{item.Name}";
                else if (itemType.IsPrimitive) continue;

                foreach (var subitem in RecursiveGetMembersOfTargetType(targetType, itemType, $"{prefix}{item.Name}.", depth + 1, maxDepth))
                {
                    yield return subitem;
                }
            }
        }

        private MemberInfo[] SmartGetMembers(Type type)
        {
            if (_cachedMemberInfosMap.TryGetValue(type, out var res))
                return res;

            res = type.GetMembers(Getter.AccessFlags);
            _cachedMemberInfosMap.Add(type, res);
            return res;
        }

        private static bool IsGettable(MemberInfo info)
        {
            return info switch
            {
                FieldInfo => true,
                PropertyInfo pi => pi.GetIndexParameters().Length <= 0,
                MethodInfo mi => mi.GetParameters().Length <= 0 && !mi.IsSpecialName,
                _ => false
            };
        }
    }
}