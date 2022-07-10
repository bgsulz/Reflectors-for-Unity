using System;
using System.Reflection;

namespace Extra.Reflection
{
    public abstract class Getter<T>
    {
        public abstract bool Initialize(string propertyPath, BindingFlags bindingFlags = Getter.AccessFlags, bool? useExpression = null);
        public abstract T GetValue(object root);
    }
    
    public class Getter<TRoot, T> : Getter<T>
    {
        private Func<TRoot, T> _getterFunc;
        
        public T GetValue(TRoot root) => _getterFunc.Invoke(root);
        public override T GetValue(object root) => _getterFunc.Invoke((TRoot) root);

        public override bool Initialize(string propertyPath, BindingFlags bindingFlags = Getter.AccessFlags, bool? useExpression = null)
        {
            _getterFunc = Getter.BuildFunc<TRoot, T>(propertyPath.ToInfos(typeof(TRoot), bindingFlags), useExpression);
            return _getterFunc != null;
        }
    }
    
    public abstract class PropertyGetter<T>
    {
        public abstract T Value { get; }
        public abstract bool Initialize(object root, string propertyPath, BindingFlags bindingFlags = Getter.AccessFlags, bool? useExpression = null);
    }
    
    public class PropertyGetter<TRoot, T> : PropertyGetter<T> where TRoot : class
    {
        private TRoot _root;
        private Getter<TRoot, T> _getter;
        
        public override T Value => _getter.GetValue(_root);

        public override bool Initialize(object root, string propertyPath, BindingFlags bindingFlags = Getter.AccessFlags, bool? useExpression = null)
        {
            _root = root as TRoot;
            return Getter.TryBuild(propertyPath, out _getter, bindingFlags, useExpression);
        }
    }
}