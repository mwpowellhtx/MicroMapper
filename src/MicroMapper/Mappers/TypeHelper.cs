namespace MicroMapper.Mappers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Internal;

    /// <summary>
    /// Type helper extension methods.
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Returns the <see cref="GetElementType(Type,IEnumerable)"/> corresponding to the
        /// <paramref name="enumerableType"/>.
        /// </summary>
        /// <param name="enumerableType"></param>
        /// <returns></returns>
        /// <remarks>Avoid confusion with the <see cref="Type.GetElementType"/> function.</remarks>
        public static Type GetNullEnumerableElementType(this Type enumerableType)
        {
            return GetElementType(enumerableType, null);
        }

        public static Type GetElementType(Type enumerableType)
        {
            return GetElementType(enumerableType, null);
        }

        public static Type GetElementType(Type enumerableType, IEnumerable enumerable)
        {
            if (enumerableType.HasElementType)
            {
                return enumerableType.GetElementType();
            }

            if (enumerableType.IsGenericType() &&
                enumerableType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
            {
                return enumerableType.GetGenericArguments()[0];
            }

            Type ienumerableType = GetIEnumerableType(enumerableType);
            if (ienumerableType != null)
            {
                return ienumerableType.GetGenericArguments()[0];
            }

            if (typeof (IEnumerable).IsAssignableFrom(enumerableType))
            {
                var first = enumerable?.Cast<object>().FirstOrDefault();

                return first?.GetType() ?? typeof (object);
            }

            throw new ArgumentException($"Unable to find the element type for type '{enumerableType}'.", nameof(enumerableType));
        }

        /// <summary>
        /// Returns whether the <paramref name="type"/> HasAttribute <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static bool HasAttribute<TAttribute>(Type type, bool inherit = false)
            where TAttribute : Attribute
        {
            return type.GetCustomAttributes(typeof (TAttribute), inherit).Any();
        }

        public static Type GetEnumerationType(Type enumType)
        {
            if (enumType.IsNullableType())
            {
                enumType = enumType.GetGenericArguments()[0];
            }

            if (!enumType.IsEnum())
                return null;

            return enumType;
        }

        private static Type GetIEnumerableType(Type enumerableType)
        {
            try
            {
                return enumerableType.GetInterfaces().FirstOrDefault(t => t.Name == "IEnumerable`1");
            }
            catch (AmbiguousMatchException)
            {
                if (enumerableType.BaseType() != typeof (object))
                    return GetIEnumerableType(enumerableType.BaseType());

                return null;
            }
        }
    }
}