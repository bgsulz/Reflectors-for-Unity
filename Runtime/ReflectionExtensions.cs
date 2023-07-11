using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

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

        public static object GetValue(this MemberInfo info, object root)
        {
            return info switch
            {
                FieldInfo fi => fi.GetValue(root),
                PropertyInfo pi => pi.GetValue(root),
                MethodInfo mi => mi.Invoke(root, Array.Empty<object>()),
                _ => null
            };
        }
        
        public static void SetValue(this MemberInfo info, object root, object value)
        {
            switch (info)
            {
                case FieldInfo fi:
                    fi.SetValue(root, value); break;
                case PropertyInfo pi:
                    pi.SetValue(root, value); break;
            }
        }

        private static readonly Dictionary<string, string> TypeAliases = new()
        {
            ["Void"] = "void",
            ["Int32"] = "int",
            ["Single"] = "float",
            ["Double"] = "double",
            ["Boolean"] = "bool",
            ["System.String"] = "string",
        };

        public static string ToKeyword(this string name) 
            => TypeAliases.TryGetValue(name, out var res) ? res : name;

        public static string ReplaceKeywords(this string input)
        {
            var patternString = string.Join("|", TypeAliases.Keys.Select(x => $"({x})"));
            var pattern = new Regex(patternString);
            return pattern.Replace(input, match => match.Value.ToKeyword());
        }

    }
}