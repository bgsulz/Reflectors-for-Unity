using System;
using System.Reflection;
using Extra.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extra.Editor.Properties
{
    [Serializable]
    public abstract class FieldReference
    {
        public const BindingFlags AccessFlags =
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        [SerializeField] protected Object root;
        [SerializeField] protected string name;

        public abstract void Initialize(bool? useExpression = null);
    }

    [Serializable]
    public class FieldReference<T> : FieldReference
    {
        private Func<object, T> _getter;

        public override void Initialize(bool? useExpression = null)
        {
            _getter = Getter.Build<T>(name, root.GetType(), AccessFlags, useExpression);
            if (_getter == null)
                throw new ArgumentException($"Unable to build getter for object {root.name} and field {name}.");
        }

        public T Value => _getter.Invoke(root);
    }
}