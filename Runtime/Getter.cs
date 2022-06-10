using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Extra.Reflection
{
    public abstract class Getter<T>
    {
        public abstract bool Initialize(object root, string propertyPath, BindingFlags flags, bool? useExpression);
        public abstract T Value { get; }
    }

    public class Getter<TRoot, T> : Getter<T> where TRoot : class
    {
        private TRoot _root;
        private Func<TRoot, T> _getterFunc;
        
        public override T Value => _getterFunc.Invoke(_root);

        public override bool Initialize(object root, string propertyPath, BindingFlags flags, bool? useExpression)
        {
            _root = root as TRoot;
            _getterFunc = Getter.BuildFunc<TRoot, T>(propertyPath.ToInfos(typeof(TRoot), flags), useExpression);
            return _getterFunc != null;
        }
    }
    
    public static class Getter
    {
        private const bool ExpressionsSupported =
#if ((UNITY_WEBGL || UNITY_IOS || ENABLE_IL2CPP) && !UNITY_EDITOR)
            false;
#endif
            true;

        public static Getter<TReturn> Build<TReturn>(object root, string propertyPath,
            BindingFlags bindingFlags = default, bool? useExpression = default)
        {
            if (!TryBuild<TReturn>(root, propertyPath, out var res, bindingFlags, useExpression)) 
                throw new Exception($"Failed to build getter for property path {propertyPath} on type {typeof(TReturn).Name}.");
            return res;
        }
        
        public static bool TryBuild<TReturn>(object root, string propertyPath, out Getter<TReturn> getter, 
            BindingFlags bindingFlags = default, bool? useExpression = default)
        {
            var getterType = typeof(Getter<,>).MakeGenericType(root.GetType(), typeof(TReturn));
            getter = Activator.CreateInstance(getterType) as Getter<TReturn>;

            return getter != null && getter.Initialize(root, propertyPath, bindingFlags, useExpression);
        }
        
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

        private static Func<TRoot, TReturn> BuildFuncWithReflection<TRoot, TReturn>(MemberInfo[] infos) =>
            root => (TReturn) infos.Aggregate<MemberInfo, object>(root, (current, item) => item switch
            {
                FieldInfo fi => fi.GetValue(current),
                PropertyInfo pi => pi.GetValue(current),
                MethodInfo mi => mi.Invoke(current, Array.Empty<object>()),
                _ => null
            });
    }

    public static class GetterExtensions
    {
        public static MemberInfo[] ToInfos(this string propertyPath, Type rootType, BindingFlags bindingFlags = default)
        {
            var currentType = rootType;
            var splitName = propertyPath.Split('.');
            var infos = new MemberInfo[splitName.Length];

            for (var (i, len) = (0, splitName.Length); i < len; i++)
            {
                var info = currentType?.GetMember(splitName[i], bindingFlags).FirstOrDefault();
                infos[i] = info ?? throw new Exception($"No field of name {splitName[i]} found in Type {currentType}.");

                if (i == len - 1) break;

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