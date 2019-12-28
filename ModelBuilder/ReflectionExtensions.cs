namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class ReflectionExtensions
    {
        public static IEnumerable<ConstructorInfo> GetConstructors(this Type type)
        {
            return type.GetTypeInfo().GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        }

        public static bool TypeIsAbstract(this Type type)
        {
            return type.GetTypeInfo().IsAbstract;
        }

        public static bool TypeIsArray(this Type type)
        {
            return type.GetTypeInfo().IsArray;
        }

        public static bool TypeIsClass(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }

        public static bool TypeIsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }

        public static bool TypeIsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        public static bool TypeIsGenericTypeDefinition(this Type type)
        {
            return type.GetTypeInfo().IsGenericTypeDefinition;
        }

        public static bool TypeIsInterface(this Type type)
        {
            return type.GetTypeInfo().IsInterface;
        }

        public static bool TypeIsPublic(this Type type)
        {
            return type.GetTypeInfo().IsPublic;
        }

        public static bool TypeIsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }
    }
}