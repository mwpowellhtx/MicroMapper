namespace AutoMapper.Mappers
{
    /* TODO: how is this different from IObjectMapper ? the interfaces are the 'same' but for different type name ?
    I suppose they did not used to be virtually identical, but with the consolidation of concerns into MapperContext, that became the case */
    public interface ITypeMapObjectMapper
    {
        object Map(ResolutionContext context);

        bool IsMatch(ResolutionContext context);
    }
}