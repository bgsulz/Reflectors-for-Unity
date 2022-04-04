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

        public abstract void Initialize();
    }

    [Serializable]
    public class FieldReference<T> : FieldReference
    {
        private Func<object, T> _getter;

        public override void Initialize() => _getter = Getter.Build<T>(root.GetType(), name, AccessFlags);

        public T Value
        {
            get
            {
                if (_getter == null) throw new NullReferenceException($"Getter for field {name} is null.");
                return _getter.Invoke(root);
            }
        }
    }
}