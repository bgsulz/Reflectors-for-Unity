#if UNITY_EDITOR

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Extra.Extensions
{
    public static class PropertyDrawerExtensions
    {
        public static T Target<T>(this SerializedProperty property) where T : Object
            => property.serializedObject.targetObject as T;

        public static T Subject<T>(this SerializedProperty property, FieldInfo fieldInfo) where T : class
            => fieldInfo.GetValue(property.serializedObject.targetObject) as T;

        public static void RepaintInspector(this SerializedObject self, ref UnityEditor.Editor editor)
        {
            if (editor != null && editor.serializedObject == self)
            {
                editor.Repaint();
                return;
            }

            foreach (var item in ActiveEditorTracker.sharedTracker.activeEditors)
            {
                if (item.serializedObject != self) continue;

                editor = item;
                editor.Repaint();
                return;
            }
        }
    }
}

#endif