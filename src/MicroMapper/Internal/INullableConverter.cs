namespace MicroMapper.Internal
{
    using System;

    public interface INullableConverter
    {
        object ConvertFrom(object value);
        Type UnderlyingType { get; }
    }
}