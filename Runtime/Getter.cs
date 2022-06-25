#define EXTRA_REFLECTORS

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Extra.Reflection
{
    public static partial class Getter
    {
        private const bool ExpressionsSupported =
#if ((UNITY_WEBGL || UNITY_IOS || ENABLE_IL2CPP) && !UNITY_EDITOR)
            false;
#endif
            true;
        
        public static Func<TRoot, TReturn> BuildFunc<TRoot, TReturn>(MemberInfo[] infos, bool? useExpression = null)
        {
            var useExpressionCalculated = useExpression ?? ExpressionsSupported;
            return useExpressionCalculated
                ? BuildFuncWithExpression<TRoot, TReturn>(infos)
                : BuildFuncWithReflection<TRoot, TReturn>(infos);
        }
        
        private static Func<TRoot, TReturn> BuildFuncWithExpression<TRoot, TReturn>(MemberInfo[] infos)
        {
            var body = Expression.Parameter(typeof(TRoot)) as Expression;
            body = infos.Aggregate(body, (current, info) => info switch
            {
                FieldInfo or PropertyInfo => Expression.MakeMemberAccess(current, info),
                MethodInfo mi => Expression.Call(current, mi),
                _ => throw new ArgumentOutOfRangeException($"MemberInfo {info.Name} is not FieldInfo, PropertyInfo, or MethodInfo.")
            });
            var returned = Expression.Convert(body, typeof(TReturn));
            var lambda = Expression.Lambda<Func<TRoot, TReturn>>(returned, Expression.Parameter(typeof(TRoot)));

            return lambda.Compile();
        }

        private static Func<TRoot, TReturn> BuildFuncWithReflection<TRoot, TReturn>(MemberInfo[] infos)
        {
            return root => (TReturn)infos.Aggregate<MemberInfo, object>(
                root, (current, item) => item switch
                {
                    FieldInfo fi => fi.GetValue(current),
                    PropertyInfo pi => pi.GetValue(current),
                    MethodInfo mi => mi.Invoke(current, Array.Empty<object>()),
                    _ => null
                }
            );
        }
    }
}