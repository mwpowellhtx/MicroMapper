namespace AutoMapper.Internal
{
    /// <summary>
    /// Represents a lazily created instance <see cref="Value"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILazy<out T>
    {
        T Value { get; }
    }
}