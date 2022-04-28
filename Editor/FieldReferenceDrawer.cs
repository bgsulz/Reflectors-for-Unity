using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Extra.Extensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extra.Editor.Properties
{
    [CustomPropertyDrawer(typeof(FieldReference<>), true)]
    public class FieldReferenceDrawer : PropertyDrawer
    {
        private readonly Dictionary<Type, MemberInfo[]> _cachedMemberInfosMap = new();

        private FieldReference _subject;
        private StringSearchProvider _searchProvider;

        private string[] _membersList;

        private SearchDepthAttribute Attribute => attribute as SearchDepthAttribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var positionNoLabel = EditorGUI.PrefixLabel(position, label);

            // Retrieve props
            var objProp = property.FindPropertyRelative("root");
            var pathProp = property.FindPropertyRelative("name");

            // Set up rects
            var left = positionNoLabel;
            left.width *= 0.5f;
            var right = left;
            right.x += left.width;

            // Populate obj from field
            EditorGUI.BeginChangeCheck();
            objProp.objectReferenceValue = EditorGUI.ObjectField(left, objProp.objectReferenceValue, typeof(Object), true);
            var changed = EditorGUI.EndChangeCheck();

            if (changed && objProp.objectReferenceValue == null)
            {
                EditorGUI.LabelField(right, "No object in field.");
                return;
            }

            if (changed || _membersList == null)
            {
                // Find suitable members
                var foundFields = TryGetMembersOfTargetType(objProp.objectReferenceValue, right, out _membersList);
                if (!foundFields) return;
            }

            if (changed)
            {
                pathProp.stringValue = _membersList[0];
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

        private bool TryGetMembersOfTargetType(Object obj, Rect position, out string[] membersList)
        {
            var targetType = fieldInfo.FieldType.GenericTypeArguments[0];
            membersList = RecursiveGetMembersOfTargetType(targetType, obj.GetType(), maxDepth: Attribute?.SearchDepth ?? 1).ToArray();

            if (membersList.Length > 0) return true;

            EditorGUI.LabelField(position, $"No fields of type {targetType}.");
            return false;
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

            res = type.GetMembers(FieldReference.AccessFlags);
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