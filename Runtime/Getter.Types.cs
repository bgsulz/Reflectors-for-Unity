using System;
using System.Reflection;

namespace Extra.Reflection
{
    public abstract class Getter<T> : IAccessor
    {
        public abstract bool Initialize(string propertyPath, BindingFlags bindingFlags = Accessor.AccessFlags, bool? useExpression = null);
        public abstract T GetValue(object root);
    }
    
    public class Getter<TRoot, T> : Getter<T>, IAccessor
    {
        private Func<TRoot, T> _getterFunc;
        
        public T GetValue(TRoot root) => _getterFunc.Invoke(root);
        public override T GetValue(object root) => _getterFunc.Invoke((TRoot) root);

        public override bool Initialize(string propertyPath, BindingFlags bindingFlags = Accessor.AccessFlags, bool? useExpression = null)
        {
            _getterFunc = Getter.BuildFunc<TRoot, T>(propertyPath.ToInfos(typeof(TRoot), bindingFlags), useExpression);
            return _getterFunc != null;
        }
    }
}