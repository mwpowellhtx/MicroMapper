namespace MicroMapper.Mappers
{
    using System;
    using System.Reflection;

    public class AssignableArrayMapper : IObjectMapper
    {
        public object Map(ResolutionContext context)
        {
            var runner = context.MapperContext.Runner;
            if (context.SourceValue == null && !runner.ShouldMapSourceCollectionAsNull(context))
            {
                return runner.CreateObject(context);
            }

            return context.SourceValue;
        }

        public bool IsMatch(ResolutionContext context)
        {
            var sourceType = context.SourceType;
            var destinationType = context.DestinationType;
            return context.DestinationType.IsAssignableFrom(sourceType)
                   && destinationType.IsArray && sourceType.IsArray
                   && !AreElementsExplicitlyMapped(context.MapperContext.Engine,
                       sourceType, destinationType);
        }

        private static bool AreElementsExplicitlyMapped(IMappingEngine engine,
            Type sourceType, Type destinationType)
        {
            var sourceElementType = sourceType.GetElementType();
            var destinationElementType = destinationType.GetElementType();
            return engine.ConfigurationProvider.FindTypeMapFor(
                sourceElementType, destinationElementType) != null;
        }
    }
}