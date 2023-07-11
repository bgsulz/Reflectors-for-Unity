using System;
using Extra.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extra.Editor.Properties
{
    [Flags]
    public enum PropertyReferenceType
    {
        Getter = 1,
        Setter = 2
    }
    
    [Serializable]
    public abstract class PropertyReference
    {
        [SerializeField] protected Object root;
        [SerializeField] protected string propertyPath;

        public abstract void Initialize(PropertyReferenceType type = PropertyReferenceType.Getter, bool? useExpression = null);
    }

    [Serializable]
    public class PropertyReference<T> : PropertyReference
    {
        protected PropertyReferenceType Type;
        protected Getter<T> PropertyGetter;
        protected Setter<T> PropertySetter;

        public override void Initialize(PropertyReferenceType type = PropertyReferenceType.Getter, bool? useExpression = null)
        {
            Type = type;

            PropertyGetter = null;
            PropertySetter = null;
            
            if (type.HasFlag(PropertyReferenceType.Getter))
                PropertyGetter = Getter.Build<T>(root.GetType(), propertyPath, useExpression: useExpression);

            if (type.HasFlag(PropertyReferenceType.Setter))
                PropertySetter = Setter.Build<T>(root.GetType(), propertyPath, useExpression: useExpression);
        }

        public T Value
        {
            get
            {
                if (PropertyGetter == null) 
                    throw new Exception("Attempting to get value; PropertyReference not initialized as Getter.");
                return PropertyGetter.GetValue(root);
            }
            set
            {
                if (PropertySetter == null) 
                    throw new Exception("Attempting to set value; PropertyReference not initialized as Setter.");
                PropertySetter.SetValue(root, value);
            }
        }
    }
}