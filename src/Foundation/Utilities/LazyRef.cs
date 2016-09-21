using System;
using System.Diagnostics;

namespace Foundation.Utilities
{
    [DebuggerStepThrough]
    public sealed class LazyRef<T>
    {
        private Func<T> _initializer;
        private T _value;

        public LazyRef(Func<T> initializer)
        {
            _initializer = initializer;
        }

        public LazyRef(T value)
        {
            _value = value;
        }

        public T Value
        {
            get
            {
                if (_initializer != null)
                {
                    _value = _initializer();
                    _initializer = null;
                }

                return _value;
            }
            set
            {
                _value = value;
                _initializer = null;
            }
        }

        public bool HasValue => _initializer == null;

        public void Reset(Func<T> initializer)
        {
            _initializer = initializer;
            _value = default(T);
        }
    }
}
