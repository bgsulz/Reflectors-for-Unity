// Original by dninosnores and wappenull.

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Extra.Extensions
{
    public static class DrawerFinder
    {
        private const BindingFlags NonPublicFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags AllInstanceFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        private struct TypeAndFieldInfo
        {
            public Type Type;
            public FieldInfo Fi;
        }

        private static readonly Dictionary<int, TypeAndFieldInfo> PathHashToTypeDict = new();
        private static readonly Dictionary<Type, PropertyDrawer> TypeToDrawerDict = new();

        public static void SmartPropertyField(this SerializedProperty property, out float height, Rect position, GUIContent label,
            string assemblyFilter = default, string drawerFilter = default)
        {
            // This is needed because of internal Unity optimizations that merge GUIContent with the same name.
            var clone = new GUIContent(label);

            var drawer = property.FindDrawerForProperty(out height, assemblyFilter, drawerFilter, clone);

            if (drawer == null)
            {
                EditorGUI.PropertyField(position, property, clone, true);
            }
            else
            {
                drawer.OnGUI(position, property, clone);
            }
        }

        public static void SmartPropertyField(this SerializedProperty property, Rect position, GUIContent label,
            string assemblyFilter = default, string drawerFilter = default)
        {
            property.SmartPropertyField(out _, position, label, assemblyFilter, drawerFilter);
        }

        private static PropertyDrawer FindDrawerForProperty(this SerializedProperty property, out float height,
            string assemblyFilter = default, string drawerFilter = default, GUIContent label = default)
        {
            var labelToDisplay = label ?? GUIContent.none;

            // Set height to default. May be overwritten later.
            height = EditorGUI.GetPropertyHeight(property, labelToDisplay, true);

            var pathHash = GetUniquePropertyPathHash(property);

            if (!PathHashToTypeDict.TryGetValue(pathHash, out var typeAndFi))
            {
                typeAndFi = GetPropertyData(property);
                PathHashToTypeDict[pathHash] = typeAndFi;
            }

            if (typeAndFi.Type == null)
            {
                return null;
            }

            var alreadyLocatedDrawer = TypeToDrawerDict.TryGetValue(typeAndFi.Type, out var drawer);

            if (!alreadyLocatedDrawer)
            {
                drawer = typeAndFi.Type.FindDrawerForType(assemblyFilter, drawerFilter);
                TypeToDrawerDict.Add(typeAndFi.Type, drawer);
            }

            if (drawer == null)
            {
                return null;
            }

            var fieldInfoBacking = typeof(PropertyDrawer).GetField("m_FieldInfo", NonPublicFlags);
            fieldInfoBacking?.SetValue(drawer, typeAndFi.Fi);

            height = drawer.GetPropertyHeight(property, labelToDisplay ?? GUIContent.none);

            return drawer;
        }

        private static PropertyDrawer FindDrawerForType(this Type propertyType,
            string assemblyFilter = default, string drawerFilter = default)
        {
            var typeDrawerIsFor = typeof(CustomPropertyDrawer).GetField("m_Type", NonPublicFlags);
            Type TypeDrawerIsForValue(CustomPropertyDrawer drawerAttribute) => (Type)typeDrawerIsFor!.GetValue(drawerAttribute);

            var drawerUseForChildren = typeof(CustomPropertyDrawer).GetField("m_UseForChildren", NonPublicFlags);
            bool UseForChildrenValue(CustomPropertyDrawer drawerAttribute) => (bool)drawerUseForChildren!.GetValue(drawerAttribute);

            var useAssemblyFilter = !string.IsNullOrWhiteSpace(assemblyFilter);
            var useDrawerFilter = !string.IsNullOrWhiteSpace(drawerFilter);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (useAssemblyFilter && !assembly.FullName.Contains(assemblyFilter)) continue;

                foreach (var candidate in assembly.GetTypes())
                {
                    if (useDrawerFilter && !candidate.Name.Contains(drawerFilter)) continue;

                    // Does Type "candidate" have [CustomPropertyDrawer(typeof(T))]?
                    foreach (var attribute in candidate.GetCustomAttributes(typeof(CustomPropertyDrawer)))
                    {
                        // If this attribute isn't [CustomPropertyDrawer], move on.
                        if (attribute is not CustomPropertyDrawer drawerAttribute) continue;

                        // If this CustomPropertyDrawer doesn't apply to this specific type, move on.
                        var drawerType = TypeDrawerIsForValue(drawerAttribute);
                        if (!propertyType.AbleToUseDrawer(drawerType, UseForChildrenValue(drawerAttribute))) continue;

                        // If Type "candidate" is not a PropertyDrawer, move on.
                        if (!candidate.IsSubclassOf(typeof(PropertyDrawer))) continue;

                        // We've found it!
                        return (PropertyDrawer)Activator.CreateInstance(candidate);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// For caching.
        /// </summary>
        private static int GetUniquePropertyPathHash(SerializedProperty property)
        {
            var type = property.serializedObject.targetObject.GetType();
            var path = property.propertyPath;
            return type.GetHashCode() + path.GetHashCode();
        }

        private static TypeAndFieldInfo GetPropertyData(SerializedProperty property)
        {
            var fullPath = property.propertyPath.Split('.');

            var parentType = property.serializedObject.targetObject.GetType();

            // To start, assign to "fi" the FieldInfo of the parent-most field
            // e.g. type Board has field Score being drawn by SmartPropertyDrawer.
            // First, get FieldInfo of Board object.
            var fi = parentType.GetField(fullPath[0], AllInstanceFlags);

            // Then, get type of parent-most field.
            // e.g. typeof(Board)
            var resolvedType = fi?.FieldType;

            for (var i = 1; i < fullPath.Length; i++)
            {
                // PropertyDrawers apply to elements of array types.
                // Drill down immediately into element type of array.
                if (fullPath.IsPropertyPathOfArray(i))
                {
                    // For arrays, e.g. int[]
                    if (fi!.FieldType.IsArray)
                    {
                        resolvedType = fi.FieldType.GetElementType();
                    }
                    // For lists, e.g. List<int>
                    else if (IsListType(fi.FieldType, out var wrappedType))
                    {
                        resolvedType = wrappedType;
                    }

                    i++;
                }
                else
                {
                    fi = resolvedType?.GetField(fullPath[i]);
                    resolvedType = fi?.FieldType;
                }
            }

            return new TypeAndFieldInfo
            {
                Type = resolvedType,
                Fi = fi
            };
        }

        private static bool IsPropertyPathOfArray(this string[] fullPath, int rootIndex)
        {
            return fullPath[rootIndex] == "Array" &&
                   rootIndex + 1 < fullPath.Length &&
                   fullPath[rootIndex + 1].StartsWith("data");
        }

        private static bool IsListType(this Type type, out Type wrappedType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                wrappedType = type.GetGenericArguments()[0];
                return true;
            }

            wrappedType = null;
            return false;
        }

        /// <summary>
        /// Returns true if an object of type propertyType is able to use a [CustomPropertyDrawer(drawerType)].
        /// </summary>
        private static bool AbleToUseDrawer(this Type propertyType, Type drawerType, bool useForChildren)
        {
            // This equality check does not work on generic types.
            if (drawerType == propertyType) return true;

            // Generic types may still apply even if useForChildren is false.
            // For instance, a drawer for Foo<,> would apply to "derived type" Foo<int, string>.
            if (!drawerType.IsGenericType && !useForChildren) return false;

            return drawerType.IsAssignableFrom(propertyType) || drawerType.IsGenericImplementationOf(propertyType);
        }

        /// <summary>
        /// Returns true if the parent type is generic and the child type implements it.
        /// For instance, typeof(List&#60;int&#62;).IsGenericImplementationOf(typeof(List&#60;T&#62;)) returns true.
        /// </summary>
        private static bool IsGenericImplementationOf(this Type child, Type parent)
        {
            if (!parent.IsGenericType) return false;

            var currentType = child;
            var isAccessor = false;
            while (currentType != null)
            {
                if (currentType.IsGenericType &&
                    currentType.GetGenericTypeDefinition() == parent.GetGenericTypeDefinition())
                {
                    isAccessor = true;
                    break;
                }

                currentType = currentType.BaseType;
            }

            return isAccessor;
        }
    }
}