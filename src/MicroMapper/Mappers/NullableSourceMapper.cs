namespace MicroMapper.Mappers
{
    using Internal;

    public class NullableSourceMapper : IObjectMapper
    {
        public object Map(ResolutionContext context)
        {
            return context.SourceValue
                   ?? context.MapperContext.Runner.CreateObject(context);
        }

        public bool IsMatch(ResolutionContext context)
        {
            return context.SourceType.IsNullableType()
                   && !context.DestinationType.IsNullableType();
        }
    }
}