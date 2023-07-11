using System;
using System.Reflection;

namespace Extra.Reflection
{
    public static partial class Getter
    {
        public static Getter<TReturn> Build<TReturn>(Type rootType, string propertyPath,
            BindingFlags bindingFlags = Accessor.AccessFlags, bool? useExpression = default)
            => Accessor.Build<Getter<TReturn>, TReturn>(typeof(Getter<,>), rootType, propertyPath, bindingFlags, useExpression);

        public static Getter<TRoot, TReturn> Build<TRoot, TReturn>(string propertyPath,
            BindingFlags bindingFlags = Accessor.AccessFlags, bool? useExpression = default)
            => Accessor.Build<Getter<TRoot, TReturn>>(propertyPath, bindingFlags, useExpression);

        public static bool TryBuild<TReturn>(Type rootType, string propertyPath, out Getter<TReturn> getter,
            BindingFlags bindingFlags = Accessor.AccessFlags, bool? useExpression = default)
            => Accessor.TryBuild<Getter<TReturn>, TReturn>(typeof(Getter<,>), rootType, propertyPath, out getter, bindingFlags, useExpression);

        public static bool TryBuild<TRoot, TReturn>(string propertyPath, out Getter<TRoot, TReturn> getter,
            BindingFlags bindingFlags = Accessor.AccessFlags, bool? useExpression = default)
            => Accessor.TryBuild<Getter<TRoot, TReturn>>(propertyPath, out getter, bindingFlags, useExpression);
    }
}