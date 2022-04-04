using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            if (!TryGetObjectFromField(objProp, (left, right), out var obj)) return;

            // Find suitable members
            if (!TryGetMembersOfTargetTypeAll(obj, right, out var membersList)) return;

            // Check if dropdown clicked
            var buttonClicked = EditorGUI.DropdownButton(right, new GUIContent(pathProp.stringValue), FocusType.Keyboard);
            if (!buttonClicked) return;

            // Set up SearchWindow and provider
            if (!_searchProvider) _searchProvider = ScriptableObject.CreateInstance<StringSearchProvider>();
            _searchProvider.Construct(membersList, x =>
            {
                pathProp.stringValue = x;
                pathProp.serializedObject.ApplyModifiedProperties();
            });
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), _searchProvider);
        }

        private static bool TryGetObjectFromField(SerializedProperty objProp, (Rect left, Rect right) positions, out Object obj)
        {
            obj = objProp.objectReferenceValue = EditorGUI.ObjectField(positions.left, objProp.objectReferenceValue, typeof(Object), true);
            if (obj) return true;

            EditorGUI.LabelField(positions.right, "No object in field.");
            return false;
        }

        private bool TryGetMembersOfTargetTypeAll(Object obj, Rect position, out string[] membersList)
        {
            var targetType = fieldInfo.FieldType.GenericTypeArguments[0];
            membersList = RecursiveGetMembersOfTargetTypeAll(targetType, obj.GetType(), maxDepth: Attribute?.SearchDepth ?? 1).ToArray();

            if (membersList.Length > 0) return true;

            EditorGUI.LabelField(position, $"No fields of type {targetType}.");
            return false;
        }

        private IEnumerable<string> RecursiveGetMembersOfTargetTypeAll(Type targetType, Type rootType, string prefix = "", int depth = 0, int maxDepth = 1)
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

                foreach (var subitem in RecursiveGetMembersOfTargetTypeAll(targetType, itemType, $"{prefix}{item.Name}.", depth + 1, maxDepth))
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