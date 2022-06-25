using System;
using System.Linq;
using System.Reflection;

namespace Extra.Reflection
{
    public static class ReflectionExtensions
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