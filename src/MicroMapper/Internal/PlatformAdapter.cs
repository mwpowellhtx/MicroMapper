using System;

namespace AutoMapper.Internal
{
    using IAdapterDictionary = System.Collections.Generic.IDictionary<Type, Func<object>>;
    using AdapterDictionary = System.Collections.Generic.Dictionary<Type, Func<object>>;

    //TODO: this seems like a likely candidate for dependency injection exposure: could start with a couple of default factories, allowing extension
    public static class PlatformAdapter
    {
        private static readonly IAdapterDictionary _factories = new AdapterDictionary
        {
            {typeof (IMapperContext), () => new MapperContext()},
            {typeof (IMapperContextFactory), () => new MapperContextFactory()},

#if NET4 || NETFX_CORE || MONODROID || MONOTOUCH || __IOS__ || DNXCORE50
            {typeof (IDictionaryFactory), () => new DictionaryFactoryOverride()},
#else
            {typeof(IDictionaryFactory), () => new DictionaryFactory()},
#endif

#if NET4 || NETFX_CORE || MONODROID || MONOTOUCH || __IOS__ || DNXCORE50
            {typeof (IEnumNameValueMapperFactory), () => new EnumNameValueMapperFactoryOverride()},
#else
            {typeof(IEnumNameValueMapperFactory), () => new EnumNameValueMapperFactory() },
#endif

#if MONODROID || MONOTOUCH || __IOS__ || NET4
            {typeof (INullableConverterFactory), () => new NullableConverterFactoryOverride()},
#else
            {typeof(INullableConverterFactory), () => new NullableConverterFactory()},
#endif

#if MONODROID || NET4
            {typeof(IProxyGeneratorFactory), () => new ProxyGeneratorFactoryOverride()},
#else
            {typeof (IProxyGeneratorFactory), () => new ProxyGeneratorFactory()},
#endif

#if MONODROID || MONOTOUCH || __IOS__ || NETFX_CORE || NET4
            {typeof (IReaderWriterLockSlimFactory), () => new ReaderWriterLockSlimFactoryOverride()},
#else
            {typeof(IReaderWriterLockSlimFactory), () => new ReaderWriterLockSlimFactory()},
#endif

        };

        public static T Resolve<T>()
        {
            var value = (T) _factories[typeof (T)]();

            return value;
        }
    }
}
