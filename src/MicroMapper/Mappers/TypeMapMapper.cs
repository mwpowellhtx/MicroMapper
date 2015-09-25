namespace MicroMapper.Mappers
{
    using System.Collections.Generic;
    using System.Linq;

    public class TypeMapMapper : IObjectMapper
	{
        /// <summary>
        /// Gets the Mappers as a point of extensibility.
        /// </summary>
        public IList<ITypeMapObjectMapper> Mappers { get; }

        public TypeMapMapper()
        {
            /* Instead specify the mappers in this way.
            This offers the same extensibility in a non-static manner. */
            Mappers = new List<ITypeMapObjectMapper>(TypeMapObjectMapperRegistry.Mappers);
        }

	    public object Map(ResolutionContext context)
		{
	        context.TypeMap.Seal();

	        var mapperToUse = Mappers.First(_ => _.IsMatch(context));

	        // check whether the context passes conditions before attempting to map the value (depth check)
            var mappedObject = !context.TypeMap.ShouldAssignValue(context) ? null : mapperToUse.Map(context);

	        return mappedObject;
		}

		public bool IsMatch(ResolutionContext context)
		{
			return context.TypeMap != null;
		}
	}
}