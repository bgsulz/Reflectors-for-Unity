using System;
using UnityEngine;

namespace Extra.Editor.Properties
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public abstract class ConditionalAccessAttribute : PropertyAttribute
    {
        public string[] ConditionNames { get; }
        public bool HideWhenDisabled { get; }
        public bool Indent { get; }
        public bool Inverted { get; }
        public bool AllRequired { get; }

        protected ConditionalAccessAttribute(bool inverted, bool hideWhenDisabled, bool indent, bool allRequired, params string[] conditionNames)
        {
            Inverted = inverted;
            HideWhenDisabled = hideWhenDisabled;
            Indent = indent;
            AllRequired = allRequired;
            ConditionNames = conditionNames;
        }
    }

    public class EnableIfAttribute : ConditionalAccessAttribute
    {
        public EnableIfAttribute(params string[] conditionNames) : base(false, false, false, false, conditionNames) { }
        public EnableIfAttribute(bool shouldIndent = false, params string[] conditionNames) : base(false, false, shouldIndent, false, conditionNames) { }
    }

    public class DisableIfAttribute : ConditionalAccessAttribute
    {
        public DisableIfAttribute(params string[] conditionNames) : base(true, false, false, false, conditionNames) { }
        public DisableIfAttribute(bool shouldIndent = false, params string[] conditionNames) : base(true, false, shouldIndent, false, conditionNames) { }
    }

    public class ShowIfAttribute : ConditionalAccessAttribute
    {
        public ShowIfAttribute(params string[] conditionNames) : base(false, true, false, false, conditionNames) { }
        public ShowIfAttribute(bool shouldIndent = false, params string[] conditionNames) : base(false, true, shouldIndent, false, conditionNames) { }
    }

    public class HideIfAttribute : ConditionalAccessAttribute
    {
        public HideIfAttribute(params string[] conditionNames) : base(true, true, false, false, conditionNames) { }
        public HideIfAttribute(bool shouldIndent = false, params string[] conditionNames) : base(true, true, shouldIndent, false, conditionNames) { }
    }

    public class EnableIfAllAttribute : ConditionalAccessAttribute
    {
        public EnableIfAllAttribute(params string[] conditionNames) : base(false, false, false, true, conditionNames) { }
        public EnableIfAllAttribute(bool shouldIndent = false, params string[] conditionNames) : base(false, false, shouldIndent, true, conditionNames) { }
    }

    public class DisableIfAllAttribute : ConditionalAccessAttribute
    {
        public DisableIfAllAttribute(params string[] conditionNames) : base(true, false, false, true, conditionNames) { }
        public DisableIfAllAttribute(bool shouldIndent = false, params string[] conditionNames) : base(true, false, shouldIndent, true, conditionNames) { }
    }

    public class ShowIfAllAttribute : ConditionalAccessAttribute
    {
        public ShowIfAllAttribute(params string[] conditionNames) : base(false, true, false, true, conditionNames) { }
        public ShowIfAllAttribute(bool shouldIndent = false, params string[] conditionNames) : base(false, true, shouldIndent, true, conditionNames) { }
    }

    public class HideIfAllAttribute : ConditionalAccessAttribute
    {
        public HideIfAllAttribute(params string[] conditionNames) : base(true, true, false, true, conditionNames) { }
        public HideIfAllAttribute(bool shouldIndent = false, params string[] conditionNames) : base(true, true, shouldIndent, true, conditionNames) { }
    }

    public class IfOnTabAttribute : ShowIfAttribute
    {
        // TODO: Add validation
        public IfOnTabAttribute(string tabsName, int tabNumber) : base($"{tabsName}.OnTab{tabNumber}") { }
    }

    public class IfFoldoutAttribute : ShowIfAttribute
    {
        // TODO: Add validation
        public IfFoldoutAttribute(string foldoutName) : base(shouldIndent: false, $"{foldoutName}.IsExpanded") { }
    }
}