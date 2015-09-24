namespace AutoMapper.Mappers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Internal;

    public class EnumerableToDictionaryMapper : IObjectMapper
    {
        private Type KvpType { get; } = typeof(KeyValuePair<,>);

        public bool IsMatch(ResolutionContext context)
        {
            var destinationType = context.DestinationType;
            var sourceType = context.SourceType;
            return destinationType.IsDictionaryType()
                   && sourceType.IsEnumerableType()
                   && !sourceType.IsDictionaryType();
        }

        public object Map(ResolutionContext context)
        {
            var runner = context.MapperContext.Runner;
            var sourceEnumerableValue = (IEnumerable)context.SourceValue ?? new object[0];
            var enumerableValue = sourceEnumerableValue.Cast<object>();

            var sourceElementType = TypeHelper.GetElementType(context.SourceType, sourceEnumerableValue);
            var genericDestDictType = context.DestinationType.GetDictionaryType();
            var  destKeyType = genericDestDictType.GetGenericArguments()[0];
            var destValueType = genericDestDictType.GetGenericArguments()[1];
            var destKvpType = KvpType.MakeGenericType(destKeyType, destValueType);

            var destDictionary = ObjectCreator.CreateDictionary(context.DestinationType, destKeyType, destValueType);
            var count = 0;

            foreach (var item in enumerableValue)
            {
                var typeMap = runner.ConfigurationProvider.ResolveTypeMap(item, null, sourceElementType, destKvpType);

                var targetSourceType = typeMap != null ? typeMap.SourceType : sourceElementType;
                var targetDestinationType = typeMap != null ? typeMap.DestinationType : destKvpType;

                var newContext = context.CreateElementContext(typeMap, item, targetSourceType, targetDestinationType,
                    count);

                var mappedValue = runner.Map(newContext);
                var keyProperty = mappedValue.GetType().GetProperty("Key");
                var destKey = keyProperty.GetValue(mappedValue, null);

                var valueProperty = mappedValue.GetType().GetProperty("Value");
                var destValue = valueProperty.GetValue(mappedValue, null);

                genericDestDictType.GetMethod("Add").Invoke(destDictionary, new[] {destKey, destValue});

                count++;
            }

            return destDictionary;
        }
    }
}