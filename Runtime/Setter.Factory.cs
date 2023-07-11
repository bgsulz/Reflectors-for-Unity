using System;
using System.Reflection;

namespace Extra.Reflection
{
    public static partial class Setter
    {
        public static Setter<TReturn> Build<TReturn>(Type rootType, string propertyPath, BindingFlags bindingFlags = Accessor.AccessFlags, bool? useExpression = default) 
            => Accessor.Build<Setter<TReturn>, TReturn>(typeof(Setter<,>), rootType, propertyPath, bindingFlags, useExpression);

        public static Setter<TReturn> Build<TRoot, TReturn>(string propertyPath, BindingFlags bindingFlags = Accessor.AccessFlags, bool? useExpression = default) 
            => Accessor.Build<Setter<TRoot, TReturn>>(propertyPath, bindingFlags, useExpression);
    
        public static bool TryBuild<TReturn>(Type rootType, string propertyPath, out Setter<TReturn> setter, BindingFlags bindingFlags = Accessor.AccessFlags, bool? useExpression = default) 
            => Accessor.TryBuild<Setter<TReturn>, TReturn>(typeof(Setter<,>), rootType, propertyPath, out setter, bindingFlags, useExpression);
    
        public static bool TryBuild<TRoot, TReturn>(string propertyPath, out Setter<TRoot, TReturn> setter, BindingFlags bindingFlags = Accessor.AccessFlags, bool? useExpression = default) 
            => Accessor.TryBuild(propertyPath, out setter, bindingFlags, useExpression);
    }
}