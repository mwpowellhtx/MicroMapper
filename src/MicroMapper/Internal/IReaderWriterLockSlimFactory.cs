namespace MicroMapper.Internal
{
    public interface IReaderWriterLockSlimFactory
    {
        IReaderWriterLockSlim Create();
    }
}