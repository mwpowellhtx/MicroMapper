namespace MicroMapper.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using Internal;

    public class ReadOnlyCollectionMapper : IObjectMapper
    {
        public object Map(ResolutionContext context)
        {
            Type genericType = typeof (EnumerableMapper<>);

            var elementType = TypeHelper.GetElementType(context.DestinationType);

            var enumerableMapper = genericType.MakeGenericType(elementType);

            var objectMapper = (IObjectMapper) Activator.CreateInstance(enumerableMapper);

            var nullDestinationValueSoTheReadOnlyCollectionMapperWorks =
                context.PropertyMap != null
                    ? context.CreateMemberContext(context.TypeMap, context.SourceValue, null, context.SourceType,
                        context.PropertyMap)
                    : context;

            return objectMapper.Map(nullDestinationValueSoTheReadOnlyCollectionMapperWorks);
        }

        public bool IsMatch(ResolutionContext context)
        {
            if (!(context.SourceType.IsEnumerableType() && context.DestinationType.IsGenericType()))
                return false;

            var genericType = context.DestinationType.GetGenericTypeDefinition();

            return genericType == typeof (ReadOnlyCollection<>);
        }

        #region Nested type: EnumerableMapper

        private class EnumerableMapper<TElement> : EnumerableMapperBase<IList<TElement>>
        {
            private readonly IList<TElement> _inner = new List<TElement>();

            public override bool IsMatch(ResolutionContext context)
            {
                throw new NotImplementedException();
            }

            protected override void SetElementValue(IList<TElement> elements, object mappedValue, int index)
            {
                _inner.Add((TElement) mappedValue);
            }

            protected override IList<TElement> GetEnumerableFor(object destination)
            {
                return _inner;
            }

            protected override IList<TElement> CreateDestinationObjectBase(Type destElementType, int sourceLength)
            {
                throw new NotImplementedException();
            }

            protected override object CreateDestinationObject(ResolutionContext context, Type destinationElementType,
                int count, IMappingEngineRunner runner)
            {
                return new ReadOnlyCollection<TElement>(_inner);
            }
        }

        #endregion
    }
}