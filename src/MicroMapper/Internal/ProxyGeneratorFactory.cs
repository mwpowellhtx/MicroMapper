#if MONODROID || NET4
namespace MicroMapper.Internal
{
    public class ProxyGeneratorFactoryOverride : IProxyGeneratorFactory
    {
        public IProxyGenerator Create()
        {
            return new ProxyGenerator();
        }
    }
}

#endif