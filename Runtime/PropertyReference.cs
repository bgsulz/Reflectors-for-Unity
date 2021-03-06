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
        [SerializeField] protected Object root;
        [SerializeField] protected string propertyPath;

        public abstract void Initialize(bool? useExpression = null);
    }

    [Serializable]
    public class PropertyReference<T> : PropertyReference
    {
        protected PropertyGetter<T> PropertyGetter;

        public override void Initialize(bool? useExpression = null)
        {
            PropertyGetter = Getter.BuildPropertyGetter<T>(root, propertyPath, Getter.AccessFlags, useExpression);
        }

        public T Value => PropertyGetter.Value;
    }
}