namespace MicroMapper
{
    using System;

    public interface IMemberResolver : IValueResolver
    {
        Type MemberType { get; }
    }
}