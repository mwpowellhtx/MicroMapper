namespace MicroMapper.Mappers
{
    using System.Reflection;

    public class AssignableMapper : IObjectMapper
    {
        public object Map(ResolutionContext context)
        {
            var runner = context.MapperContext.Runner;
            if (context.SourceValue == null && !runner.ShouldMapSourceValueAsNull(context))
            {
                return runner.CreateObject(context);
            }

            return context.SourceValue;
        }

        public bool IsMatch(ResolutionContext context)
        {
            return context.DestinationType.IsAssignableFrom(context.SourceType);
        }
    }
}