namespace AutoMapper.Internal
{
    using System;
    using System.Threading;

    //TODO: may make this one non-static, and give it an interface as well, ILazyFactory
    public static class LazyFactory
    {
        /// <summary>
        /// Returns a lazily created instance via <see cref="ILazy{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static ILazy<T> Create<T>(Func<T> valueFactory)
            where T : class
        {
            return new LazyImpl<T>(valueFactory);
        }

        private sealed class LazyImpl<T> : ILazy<T>
            where T : class
        {
            private readonly object _syncObj = new object();
            private readonly Func<T> _valueFactory;
            private bool _isDelegateInvoked;

            private T _value;

            public LazyImpl(Func<T> valueFactory)
            {
                _valueFactory = valueFactory;
            }

            public T Value
            {
                get
                {
                    if (!_isDelegateInvoked)
                    {
                        var temp = _valueFactory();
                        Interlocked.CompareExchange(ref _value, temp, null);

                        var locked = false;

                        try
                        {
                            Monitor.Enter(_syncObj);
                            locked = true;

                            _isDelegateInvoked = true;
                        }
                        finally
                        {
                            if (locked)
                            {
                                Monitor.Exit(_syncObj);
                            }
                        }
                    }

                    return _value;
                }
            }
        }
    }
}