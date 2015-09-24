namespace AutoMapper.Mappers
{
    using System;
    using System.Reflection;

    public class PrimitiveArrayMapper : IObjectMapper
    {
        public object Map(ResolutionContext context)
        {
            var runner = context.MapperContext.Runner;
            if (context.IsSourceValueNull && runner.ShouldMapSourceCollectionAsNull(context))
            {
                return null;
            }

            var sourceElementType = TypeHelper.GetElementType(context.SourceType);
            var destElementType = TypeHelper.GetElementType(context.DestinationType);

            var sourceArray = (Array) context.SourceValue ?? ObjectCreator.CreateArray(sourceElementType, 0);

            var sourceLength = sourceArray.Length;
            var destArray = ObjectCreator.CreateArray(destElementType, sourceLength);

            Array.Copy(sourceArray, destArray, sourceLength);

            return destArray;
        }

        private bool IsPrimitiveArrayType(Type type)
        {
            if (type.IsArray)
            {
                var elementType = TypeHelper.GetElementType(type);
                return elementType.IsPrimitive() || elementType == typeof (string);
            }

            return false;
        }

        public bool IsMatch(ResolutionContext context)
        {
            var destinationType = context.DestinationType;
            var sourceType = context.SourceType;
            return IsPrimitiveArrayType(destinationType)
                   && IsPrimitiveArrayType(sourceType)
                   && TypeHelper.GetElementType(destinationType) == TypeHelper.GetElementType(sourceType);
        }
    }
}