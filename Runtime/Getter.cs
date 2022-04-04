using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Extra.Reflection
{
    public static class Getter
    {
        public static Func<object, TReturn> Build<TReturn>(Type rootType, string propertyPath, BindingFlags bindingFlags = default)
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

            return Build<TReturn>(infos);
        }

        public static Func<object, TReturn> Build<TReturn>(params MemberInfo[] infos)
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
    }
}