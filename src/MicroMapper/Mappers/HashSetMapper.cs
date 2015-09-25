namespace MicroMapper.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Internal;
    using System.Reflection;

    public class HashSetMapper : IObjectMapper
    {
        public object Map(ResolutionContext context)
        {
            var genericType = typeof (EnumerableMapper<,>);

            var collectionType = context.DestinationType;
            var elementType = TypeHelper.GetElementType(context.DestinationType);

            var enumerableMapper = genericType.MakeGenericType(collectionType, elementType);

            var objectMapper = (IObjectMapper) Activator.CreateInstance(enumerableMapper);

            return objectMapper.Map(context);
        }

        public bool IsMatch(ResolutionContext context)
        {
            var isMatch = context.SourceType.IsEnumerableType() && IsSetType(context.DestinationType);

            return isMatch;
        }

#if !NETFX_CORE
        private static bool IsSetType(Type type)
        {
            if (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (ISet<>))
            {
                return true;
            }

            var genericInterfaces = type.GetInterfaces().Where(t => t.IsGenericType());
            var baseDefinitions = genericInterfaces.Select(t => t.GetGenericTypeDefinition());

            var isCollectionType = baseDefinitions.Any(t => t == typeof (ISet<>));

            return isCollectionType;
        }


        private class EnumerableMapper<TCollection, TElement> : EnumerableMapperBase<TCollection>
            where TCollection : ISet<TElement>
        {
            public override bool IsMatch(ResolutionContext context)
            {
                throw new NotImplementedException();
            }

            protected override void SetElementValue(TCollection destination, object mappedValue, int index)
            {
                destination.Add((TElement) mappedValue);
            }

            protected override void ClearEnumerable(TCollection enumerable)
            {
                enumerable.Clear();
            }

            protected override TCollection CreateDestinationObjectBase(Type destElementType, int sourceLength)
            {
                var collection = typeof (TCollection).IsInterface()
                    ? new HashSet<TElement>()
                    : ObjectCreator.CreateDefaultValue(typeof (TCollection));

                return (TCollection) collection;
            }
        }

#else

        //TODO: check these build... and run...

        private static bool IsSetType(Type type)
        {
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(ISet<>))
            {
                return true;
            }

            var genericInterfaces = type.GetTypeInfo().ImplementedInterfaces.Where(t => t.GetTypeInfo().IsGenericType);
            var baseDefinitions = genericInterfaces.Select(t => t.GetGenericTypeDefinition());

            var isCollectionType = baseDefinitions.Any(t => t == typeof(ISet<>));

            return isCollectionType;
        }


        private class EnumerableMapper<TCollection, TElement> : EnumerableMapperBase<TCollection>
            where TCollection : ISet<TElement>
        {
            public override bool IsMatch(ResolutionContext context)
            {
                throw new NotImplementedException();
            }

            protected override void SetElementValue(TCollection destination, object mappedValue, int index)
            {
                destination.Add((TElement)mappedValue);
            }

            protected override void ClearEnumerable(TCollection enumerable)
            {
                enumerable.Clear();
            }

            protected override TCollection CreateDestinationObjectBase(Type destElementType, int sourceLength)
            {
                var collection = typeof(TCollection).GetTypeInfo().IsInterface
                    ? new HashSet<TElement>()
                    : ObjectCreator.CreateDefaultValue(typeof(TCollection));

                return (TCollection)collection;
            }
        }

#endif
    }
}