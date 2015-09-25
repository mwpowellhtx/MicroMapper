﻿namespace MicroMapper.Internal
{
    public interface IDictionaryFactory
    {
        IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>();
    }
}