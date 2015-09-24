namespace AutoMapper.Mappers
{
    using System;

    public class FlagsEnumMapper : IObjectMapper
    {
        public object Map(ResolutionContext context)
        {
            var runner = context.MapperContext.Runner;
            var enumDestType = TypeHelper.GetEnumerationType(context.DestinationType);

            if (context.SourceValue == null)
            {
                return runner.CreateObject(context);
            }

            return Enum.Parse(enumDestType, context.SourceValue.ToString(), true);
        }

        public bool IsMatch(ResolutionContext context)
        {
            var sourceEnumType = TypeHelper.GetEnumerationType(context.SourceType);
            var destEnumType = TypeHelper.GetEnumerationType(context.DestinationType);

            return !(sourceEnumType == null || destEnumType == null)
                   && TypeHelper.HasAttribute<FlagsAttribute>(sourceEnumType)
                   && TypeHelper.HasAttribute<FlagsAttribute>(destEnumType);
        }
    }
}