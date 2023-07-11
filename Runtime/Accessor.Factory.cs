#define EXTRA_REFLECTORS

using System;
using System.Reflection;
using Codice.CM.SEIDInfo;
using UnityEngine;

namespace Extra.Reflection
{
    public static class Accessor
    {
        public const BindingFlags AccessFlags = (BindingFlags) 52;

        public const bool ExpressionsSupported =
#if ((UNITY_WEBGL || UNITY_IOS || ENABLE_IL2CPP) && !UNITY_EDITOR)
        false;
#endif
            true;

        public static TAccessor Build<TAccessor, TReturn>(Type genericTypeDefinition, Type rootType, string propertyPath, BindingFlags bindingFlags = AccessFlags, bool? useExpression = default) 
            where TAccessor : class, IAccessor
        {
            if (!TryBuild<TAccessor, TReturn>(genericTypeDefinition, rootType, propertyPath, out var res, bindingFlags, useExpression)) 
                throw new Exception($"Failed to build setter for property path {propertyPath}.");
            return res;
        }

        public static TAccessor Build<TAccessor>(string propertyPath, BindingFlags bindingFlags = AccessFlags, bool? useExpression = default)
            where TAccessor : class, IAccessor, new()
        {
            if (!TryBuild<TAccessor>(propertyPath, out var res, bindingFlags, useExpression)) 
                throw new Exception($"Failed to build setter for property path {propertyPath}.");
            return res;
        }

        public static bool TryBuild<TAccessor, TReturn>(Type genericTypeDefinition, Type rootType, string propertyPath, out TAccessor accessor, BindingFlags bindingFlags = AccessFlags, bool? useExpression = default)
            where TAccessor : class, IAccessor
        {
            var type = genericTypeDefinition.MakeGenericType(rootType, typeof(TReturn));
            accessor = Activator.CreateInstance(type) as TAccessor;
            return accessor != null && accessor.Initialize(propertyPath, bindingFlags, useExpression);
        }
        
        public static bool TryBuild<TAccessor>(string propertyPath, out TAccessor accessor, 
            BindingFlags bindingFlags = AccessFlags, bool? useExpression = default)
            where TAccessor : class, IAccessor, new()
        {
            accessor = new TAccessor();
            return accessor.Initialize(propertyPath, bindingFlags, useExpression);
        }
    }
}