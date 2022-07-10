using System;
using System.Reflection;

namespace Extra.Reflection
{
    public static partial class Getter
    {
        public static Getter<TReturn> Build<TReturn>(Type rootType, string propertyPath,
            BindingFlags bindingFlags = AccessFlags, bool? useExpression = default)
        {
            if (!TryBuild<TReturn>(rootType, propertyPath, out var res, bindingFlags, useExpression)) 
                throw new Exception($"Failed to build getter for property path {propertyPath} of type {typeof(TReturn).Name}.");
            return res;
        }
        
        public static Getter<TRoot, TReturn> Build<TRoot, TReturn>(string propertyPath,
            BindingFlags bindingFlags = AccessFlags, bool? useExpression = default)
        {
            if (!TryBuild<TRoot, TReturn>(propertyPath, out var res, bindingFlags, useExpression)) 
                throw new Exception($"Failed to build getter for property path {propertyPath} of type {typeof(TReturn).Name}.");
            return res;
        }
        
        public static PropertyGetter<TReturn> BuildPropertyGetter<TReturn>(object root, string propertyPath,
            BindingFlags bindingFlags = AccessFlags, bool? useExpression = default)
        {
            if (!TryBuildPropertyGetter<TReturn>(root, propertyPath, out var res, bindingFlags, useExpression)) 
                throw new Exception($"Failed to build getter with root for property path {propertyPath} of type {typeof(TReturn).Name}.");
            return res;
        }

        public static bool TryBuild<TReturn>(Type rootType, string propertyPath, out Getter<TReturn> getter, 
            BindingFlags bindingFlags = AccessFlags, bool? useExpression = default)
        {
            var getterType = typeof(Getter<,>).MakeGenericType(rootType, typeof(TReturn));
            getter = Activator.CreateInstance(getterType) as Getter<TReturn>;
            return getter != null && getter.Initialize(propertyPath, bindingFlags, useExpression);
        }
        
        public static bool TryBuild<TRoot, TReturn>(string propertyPath, out Getter<TRoot, TReturn> getter, 
            BindingFlags bindingFlags = AccessFlags, bool? useExpression = default)
        {
            getter = new Getter<TRoot, TReturn>();
            return getter.Initialize(propertyPath, bindingFlags, useExpression);
        }

        public static bool TryBuildPropertyGetter<TReturn>(object root, string propertyPath, out PropertyGetter<TReturn> getter,
            BindingFlags bindingFlags = AccessFlags, bool? useExpression = default)
        {
            var getterType = typeof(PropertyGetter<,>).MakeGenericType(root.GetType(), typeof(TReturn));
            getter = Activator.CreateInstance(getterType) as PropertyGetter<TReturn>;
            return getter != null && getter.Initialize(root, propertyPath, bindingFlags, useExpression);
        }
    }
}