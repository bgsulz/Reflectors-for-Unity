using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Extra.Reflection
{
    public static partial class Setter
    {
        public static Action<TRoot, TReturn> BuildAction<TRoot, TReturn>(MemberInfo[] infos, bool? useExpression = null)
        {
            var useExpressionCalculated = useExpression ?? Accessor.ExpressionsSupported;
            return useExpressionCalculated
                ? BuildActionWithExpression<TRoot, TReturn>(infos)
                : BuildActionWithReflection<TRoot, TReturn>(infos);
        }

        private static Action<TRoot, TReturn> BuildActionWithExpression<TRoot, TReturn>(MemberInfo[] infos)
        {
            var input = Expression.Parameter(typeof(TRoot));
            var result = Expression.Parameter(typeof(TReturn));

            var access = infos.Aggregate(input as Expression, (current, info) => info switch
            {
                FieldInfo or PropertyInfo => Expression.MakeMemberAccess(current, info),
                _ => throw new ArgumentOutOfRangeException($"MemberInfo {info.Name} is not FieldInfo or PropertyInfo.")
            });

            var assigner = Expression.Assign(access, result);
            var lambda = Expression.Lambda<Action<TRoot, TReturn>>(assigner, input, result);

            return lambda.Compile();
        }

        private static Action<TRoot, TReturn> BuildActionWithReflection<TRoot, TReturn>(MemberInfo[] infos)
        {
            return (root, value) =>
            {
                object obj = root;
                foreach (var info in infos[..^1]) obj = info.GetValue(obj);
                infos[^1].SetValue(obj, value);
            };
        }
    }
}