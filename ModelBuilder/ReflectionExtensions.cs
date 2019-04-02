namespace ModelBuilder
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
#if NETSTANDARD1_5
    using System.Collections.Generic;

#endif

    internal static class ReflectionExtensions
    {
        public static bool TypeIsAbstract(this Type type)
        {
#if NET45
            return type.IsAbstract;
#else
            return type.GetTypeInfo().IsAbstract;
#endif
        }

        public static bool TypeIsArray(this Type type)
        {
#if NET45
            return type.IsArray;
#else
            return type.GetTypeInfo().IsArray;
#endif
        }

        public static bool TypeIsClass(this Type type)
        {
#if NET45
            return type.IsClass;
#else
            return type.GetTypeInfo().IsClass;
#endif
        }

        public static bool TypeIsEnum(this Type type)
        {
#if NET45
            return type.IsEnum;
#else
            return type.GetTypeInfo().IsEnum;
#endif
        }

        public static bool TypeIsGenericType(this Type type)
        {
#if NET45
            return type.IsGenericType;
#else
            return type.GetTypeInfo().IsGenericType;
#endif
        }

        public static bool TypeIsGenericTypeDefinition(this Type type)
        {
#if NET45
            return type.IsGenericTypeDefinition;
#else
            return type.GetTypeInfo().IsGenericTypeDefinition;
#endif
        }

        public static bool TypeIsInterface(this Type type)
        {
#if NET45
            return type.IsInterface;
#else
            return type.GetTypeInfo().IsInterface;
#endif
        }

        public static bool TypeIsPublic(this Type type)
        {
#if NET45
            return type.IsPublic;
#else
            return type.GetTypeInfo().IsPublic;
#endif
        }

        public static bool TypeIsValueType(this Type type)
        {
#if NET45
            return type.IsValueType;
#else
            return type.GetTypeInfo().IsValueType;
#endif
        }

#if NETSTANDARD1_5

        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().IsGenericTypeDefinition
                ? type.GetTypeInfo().GenericTypeParameters
                : type.GetTypeInfo().GenericTypeArguments;
        }

        public static bool IsAssignableFrom(this Type type, Type otherType)
        {
            return type.GetTypeInfo().IsAssignableFrom(otherType.GetTypeInfo());
        }

        public static IEnumerable<Attribute> GetCustomAttributes(this Type type, Type attributeType, bool inherit)
        {
            return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit);
        }

        public static bool IsInstanceOfType(this Type type, object target)
        {
            Debug.Assert(target != null, "A target must be specified");

            var targetType = target.GetType().GetTypeInfo();

            return type.GetTypeInfo().IsAssignableFrom(targetType);
        }

        public static ConstructorInfo GetConstructor(this Type type, Type[] types)
        {
            return type.GetTypeInfo().GetConstructor(types);
        }

        public static MethodInfo GetMethod(this Type type, string name)
        {
            return type.GetTypeInfo().GetMethod(name);
        }

        public static IEnumerable<PropertyInfo> GetProperties(
            this Type type,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
        {
            return type.GetTypeInfo().GetProperties(bindingFlags);
        }

        public static IEnumerable<Type> GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().GetInterfaces();
        }

        public static IEnumerable<ConstructorInfo> GetConstructors(this Type type)
        {
            return type.GetTypeInfo().GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        }

#endif
    }
}