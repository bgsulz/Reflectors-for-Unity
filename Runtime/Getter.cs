using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Extra.Reflection
{
    public static partial class Getter
    {
        public static Func<TRoot, TReturn> BuildFunc<TRoot, TReturn>(MemberInfo[] infos, bool? useExpression = null)
        {
            var useExpressionCalculated = useExpression ?? Accessor.ExpressionsSupported;
            return useExpressionCalculated
                ? BuildFuncWithExpression<TRoot, TReturn>(infos)
                : BuildFuncWithReflection<TRoot, TReturn>(infos);
        }
        
        private static Func<TRoot, TReturn> BuildFuncWithExpression<TRoot, TReturn>(MemberInfo[] infos)
        {
            var input = Expression.Parameter(typeof(TRoot));
            
            var access = infos.Aggregate(input as Expression, (current, info) => info switch
            {
                FieldInfo or PropertyInfo => Expression.MakeMemberAccess(current, info),
                MethodInfo mi => Expression.Call(current, mi),
                _ => throw new ArgumentOutOfRangeException($"MemberInfo {info.Name} is not FieldInfo, PropertyInfo, or MethodInfo.")
            });
            
            var converter = Expression.Convert(access, typeof(TReturn));
            var lambda = Expression.Lambda<Func<TRoot, TReturn>>(converter, input);

            return lambda.Compile();
        }

        private static Func<TRoot, TReturn> BuildFuncWithReflection<TRoot, TReturn>(MemberInfo[] infos)
        {
            return root =>
            {
                object obj = root;
                foreach (var info in infos) obj = info.GetValue(obj);
                return (TReturn)obj;
            };
        }
    }
}