namespace AutoMapper.Mappers
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;

    public abstract class EnumerableMapperBase<TEnumerable> : IObjectMapper
        where TEnumerable : IEnumerable
    {
        public object Map(ResolutionContext context)
        {
            var runner = context.MapperContext.Runner;
            if (context.IsSourceValueNull && runner.ShouldMapSourceCollectionAsNull(context))
            {
                return null;
            }

            var enumerableValue = ((IEnumerable) context.SourceValue ?? new object[0]).Cast<object>().ToList();

            var sourceElementType = TypeHelper.GetElementType(context.SourceType, enumerableValue);
            var destElementType = TypeHelper.GetElementType(context.DestinationType);

            var sourceLength = enumerableValue.Count;
            var destination = GetOrCreateDestinationObject(context, destElementType, sourceLength);
            var enumerable = GetEnumerableFor(destination);

            ClearEnumerable(enumerable);

            var i = 0;

            foreach (var item in enumerableValue)
            {
                var newContext = context.CreateElementContext(null, item, sourceElementType, destElementType, i);
                var elementResolutionResult = new ResolutionResult(newContext);

                var typeMap = runner.ConfigurationProvider.ResolveTypeMap(elementResolutionResult, destElementType);

                var targetSourceType = typeMap != null ? typeMap.SourceType : sourceElementType;
                var targetDestinationType = typeMap != null ? typeMap.DestinationType : destElementType;

                newContext = context.CreateElementContext(typeMap, item, targetSourceType, targetDestinationType, i);

                var mappedValue = runner.Map(newContext);

                SetElementValue(enumerable, mappedValue, i);

                i++;
            }

            var valueToAssign = destination;
            return valueToAssign;
        }

        protected virtual object GetOrCreateDestinationObject(ResolutionContext context,
            Type destElementType, int sourceLength)
        {
            var runner = context.MapperContext.Runner;

            if (context.DestinationValue != null)
            {
                // If the source is not an array, assume we can add to it...
                if (!(context.DestinationValue is Array))
                    return context.DestinationValue;

                // If the source is an array, ensure that we have enough room...
                var array = (Array) context.DestinationValue;

                if (array.Length >= sourceLength)
                    return context.DestinationValue;
            }

            return CreateDestinationObject(context, destElementType, sourceLength, runner);
        }

        protected virtual TEnumerable GetEnumerableFor(object destination)
        {
            return (TEnumerable) destination;
        }

        protected virtual void ClearEnumerable(TEnumerable enumerable)
        {
        }

        protected virtual object CreateDestinationObject(ResolutionContext context, Type destinationElementType,
            int count, IMappingEngineRunner runner)
        {
            var destinationType = context.DestinationType;

            if (!destinationType.IsInterface() && !destinationType.IsArray)
            {
                return runner.CreateObject(context);
            }

            return CreateDestinationObjectBase(destinationElementType, count);
        }

        public abstract bool IsMatch(ResolutionContext context);

        protected abstract void SetElementValue(TEnumerable destination, object mappedValue, int index);

        protected abstract TEnumerable CreateDestinationObjectBase(Type destElementType, int sourceLength);
    }
}