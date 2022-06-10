using System;
using System.Reflection;
using Extra.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extra.Editor.Properties
{
    [Serializable]
    public abstract class PropertyReference
    {
        public const BindingFlags AccessFlags = (BindingFlags) 52;

        [SerializeField] protected Object root;
        [SerializeField] protected string propertyPath;

        public abstract void Initialize(bool? useExpression = null);
    }

    [Serializable]
    public class PropertyReference<T> : PropertyReference
    {
        protected Getter<T> PropertyGetter;

        public override void Initialize(bool? useExpression = null)
        {
            PropertyGetter = Getter.Build<T>(root, propertyPath, AccessFlags, useExpression);
        }

        public T Value => PropertyGetter.Value;
    }
}