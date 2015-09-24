namespace AutoMapper.Mappers
{
    using System.Collections.Generic;

    /// <summary>
    /// Refactored from static MapperRegistry allowing for flexible dependency injection
    /// of mapping concerns.
    /// </summary>
    public interface IObjectMapperCollection : IList<IObjectMapper>
    {
        /// <summary>
        /// 
        /// </summary>
        void Reset();

        /// <summary>
        /// Gets the <see cref="TypeMapMapper"/> instance from among the object mappers.
        /// </summary>
        TypeMapMapper TypeMap { get; }
    }
}
