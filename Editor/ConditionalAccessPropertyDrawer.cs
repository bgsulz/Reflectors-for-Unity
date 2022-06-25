#if UNITY_EDITOR

using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Extra.Extensions;
using Extra.Reflection;

namespace Extra.Editor.Properties
{
    [CustomPropertyDrawer(typeof(ConditionalAccessAttribute), true)]
    public class ConditionalAccessPropertyDrawer : PropertyDrawer
    {
        private const BindingFlags AccessFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private const string ArrayErrorMsg = "This attribute does not support lists; use DataArray<T> or DataList<T>.";

        private PropertyGetter<bool>[] _getters;

        private bool? _isArrayItem;
        private float _propertyHeight;
        private UnityEditor.Editor _myEditor;

        private ConditionalAccessAttribute _attribute;
        private ConditionalAccessAttribute Attribute => _attribute ??= (ConditionalAccessAttribute)attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _isArrayItem ??= property.serializedObject.FindProperty(fieldInfo.Name).isArray;

            if (_isArrayItem!.Value)
            {
                ArrayNotSupportedLabelField(position, label);
                return;
            }

            var result = AreConditionsSatisfied(property.serializedObject.targetObject, Attribute.ConditionNames, Attribute.AllRequired);

            ConditionalPropertyField(position, property, label, result);

            property.serializedObject.RepaintInspector(ref _myEditor);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => _propertyHeight;

        private void ArrayNotSupportedLabelField(Rect position, GUIContent label)
        {
            _propertyHeight = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(position, label.ToString(), ArrayErrorMsg);
        }

        private void ConditionalPropertyField(Rect position, SerializedProperty property, GUIContent label, bool? value)
        {
            var isAccessible = value!.Value != Attribute.Inverted;
            var isDrawn = isAccessible || !Attribute.HideWhenDisabled;
            if (!isDrawn)
            {
                _propertyHeight = 0;
                return;
            }

            var wasEnabled = GUI.enabled;
            var wasIndentLevel = EditorGUI.indentLevel;

            GUI.enabled = isAccessible;
            EditorGUI.indentLevel += Attribute.Indent ? 1 : 0;

            property.SmartPropertyField(out _propertyHeight, position, label);

            GUI.enabled = wasEnabled;
            EditorGUI.indentLevel = wasIndentLevel;
        }

        private bool AreConditionsSatisfied(object context, string[] conditionNames, bool allRequired)
        {
            if (_getters == null || _getters.Length != conditionNames.Length)
            {
                InitializeGettersAndContainer(context, conditionNames);
            }

            foreach (var getter in _getters)
            {
                var res = getter.Value;

                switch (res)
                {
                    // If !allRequired, one true is good enough!
                    case true when !allRequired: return true;

                    // If allRequired, one false means the whole thing is false.
                    case false when allRequired: return false;
                }
            }

            // If allRequired, we got through it all without disqualifying! Return true.
            // If !allRequired, we went through it all without a single true. Return false.
            return allRequired;
        }

        private void InitializeGettersAndContainer(object context, string[] conditionNames)
        {
            _getters = new PropertyGetter<bool>[conditionNames.Length];

            for (var i = 0; i < conditionNames.Length; i++)
            {
                _getters[i] = Getter.BuildPropertyGetter<bool>(context, conditionNames[i], AccessFlags);
            }
        }
    }
}

#endif