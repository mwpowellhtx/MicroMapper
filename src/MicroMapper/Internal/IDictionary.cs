﻿namespace MicroMapper.Internal
{
    using System;
    using System.Collections.Generic;
#if NETFX_CORE
    // For purposes of Universal Windows (Window 10 SDK)
    using System.Reflection;
#endif

    /// <summary>
    /// Represents the hybrid between the concurrent dictionary and read-only dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IDictionary<TKey, TValue>
    {
        TValue AddOrUpdate(TKey key, TValue addValue,
            Func<TKey, TValue, TValue> updateValueFactory);

        bool TryGetValue(TKey key, out TValue value);

        TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory);

        TValue this[TKey key] { get; set; }

        bool TryRemove(TKey key, out TValue value);

        void Clear();

        ICollection<TValue> Values { get; }

        ICollection<TKey> Keys { get; }

        bool ContainsKey(TKey key);
    }

    //TODO: I'm not sure what this is or why it doesn't make sense as part of "mapper configuration"
    public static class FeatureDetector
    {
        public static Func<Type, bool> IsIDataRecordType = t => false;

        private static bool? _isEnumGetNamesSupported;

        public static bool IsEnumGetNamesSupported
        {
            get
            {
                if (_isEnumGetNamesSupported == null)
                    _isEnumGetNamesSupported = ResolveIsEnumGetNamesSupported();

                return _isEnumGetNamesSupported.Value;
            }
        }

        private static bool ResolveIsEnumGetNamesSupported()
        {
            return typeof (Enum).GetMethod("GetNames") != null;
        }
    }
}