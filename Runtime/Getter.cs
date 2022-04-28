using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace Extra.Reflection
{
    public static class Getter
    {
        public const bool ExpressionsSupported =
#if ((UNITY_WEBGL || UNITY_IOS || ENABLE_IL2CPP) && !UNITY_EDITOR)
                                                 false;
#endif
                                                 true;

        public static Func<object, TReturn> Build<TReturn>(string propertyPath, Type rootType,
            BindingFlags bindingFlags = default, bool? useExpression = null)
        {
            return Build<TReturn>(propertyPath.ToInfos(rootType, bindingFlags), useExpression);
        }

        public static Func<object, TReturn> Build<TReturn>(MemberInfo[] infos, bool? useExpression = null)
        {
            return useExpression ?? ExpressionsSupported ? BuildExpression<TReturn>(infos) : BuildReflection<TReturn>(infos);
        }

        private static Func<object, TReturn> BuildExpression<TReturn>(MemberInfo[] infos)
        {
            var declaringType = infos[0].DeclaringType ?? throw new NullReferenceException($"Declaring type of {infos[0].Name} is null.");

            var obj = Expression.Parameter(typeof(object));
            var param = Expression.Convert(obj, declaringType);

            Expression body = param;
            foreach (var info in infos)
            {
                body = info switch
                {
                    FieldInfo or PropertyInfo => Expression.MakeMemberAccess(body, info),
                    MethodInfo mi => Expression.Call(body, mi),
                    _ => throw new ArgumentOutOfRangeException($"MemberInfo {info.Name} is not FieldInfo, PropertyInfo, or MethodInfo.")
                };
            }

            var convertToReturnType = Expression.Convert(body, typeof(TReturn));
            var lambda = Expression.Lambda<Func<object, TReturn>>(convertToReturnType, obj);

            return lambda.Compile();
        }

        private static Func<object, TReturn> BuildReflection<TReturn>(MemberInfo[] infos)
        {
            return o =>
            {
                var curr = o;
                foreach (var item in infos)
                {
                    curr = item switch
                    {
                        FieldInfo fi => fi.GetValue(curr),
                        PropertyInfo pi => pi.GetValue(curr),
                        MethodInfo mi => mi.Invoke(curr, Array.Empty<object>()),
                        _ => null
                    };
                }
                return (TReturn) curr;
            };
        }

        public static MemberInfo[] ToInfos(this string propertyPath, Type rootType, BindingFlags bindingFlags = default)
        {
            var currentType = rootType;
            var splitName = propertyPath.Split('.');
            var infos = new MemberInfo[splitName.Length];

            for (var i = 0; i < splitName.Length; i++)
            {
                var info = currentType?.GetMember(splitName[i], bindingFlags).FirstOrDefault();
                if (info == null) throw new Exception($"No field of name {splitName[i]} found in Type {currentType}.");
                infos[i] = info;

                if (i == splitName.Length - 1) break;

                currentType = info switch
                {
                    FieldInfo fi => fi.FieldType,
                    PropertyInfo pi => pi.PropertyType,
                    MethodInfo mi => mi.ReturnType,
                    _ => null
                };
            }

            return infos;
        }
    }
}