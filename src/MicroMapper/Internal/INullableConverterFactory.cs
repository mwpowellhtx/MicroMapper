namespace MicroMapper.Internal
{
    using System;

    public interface INullableConverterFactory
    {
        INullableConverter Create(Type nullableType);
    }
}