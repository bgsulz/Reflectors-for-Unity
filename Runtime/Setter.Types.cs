using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace Extra.Reflection
{
    public interface IAccessor
    {
        bool Initialize(string propertyPath, BindingFlags bindingFlags = Accessor.AccessFlags,
            bool? useExpression = null);
    }

    public abstract class Setter<T> : IAccessor
    {
        protected const BindingFlags AccessFlags = (BindingFlags)52;

        public abstract bool Initialize(string propertyPath, BindingFlags bindingFlags = AccessFlags,
            bool? useExpression = null);

        public abstract void SetValue(object root, T value);
    }

    public class Setter<TRoot, T> : Setter<T>, IAccessor
    {
        private Action<TRoot, T> _getterFunc;

        public override void SetValue(object root, T value) => _getterFunc.Invoke((TRoot) root, value);
        public void SetValue(TRoot root, T value) => _getterFunc.Invoke(root, value);

        public override bool Initialize(string propertyPath, BindingFlags bindingFlags = AccessFlags,
            bool? useExpression = null)
        {
            _getterFunc =
                Setter.BuildAction<TRoot, T>(propertyPath.ToInfos(typeof(TRoot), bindingFlags), useExpression);
            return _getterFunc != null;
        }
    }
}