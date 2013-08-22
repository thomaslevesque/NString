using System;
using System.Collections.Immutable;
using System.Threading;
using JetBrains.Annotations;

namespace NString.Internal
{
    class Cache<TKey, TValue>
    {
        private IImmutableDictionary<TKey, TValue> _cache = ImmutableDictionary.Create<TKey, TValue>();

        public TValue GetOrAdd(TKey key, [NotNull] Func<TKey, TValue> valueFactory)
        {
            valueFactory.CheckArgumentNull("valueFactory");

            TValue newValue = default(TValue);
            bool newValueCreated = false;
            while (true)
            {
                var oldCache = _cache;
                TValue value;
                if (oldCache.TryGetValue(key, out value))
                    return value;

                // Value not found; create it if necessary
                if (!newValueCreated)
                {
                    newValue = valueFactory(key);
                    newValueCreated = true;
                }

                // Add the new value to the cache
                var newCache = oldCache.Add(key, newValue);
                if (Interlocked.CompareExchange(ref _cache, newCache, oldCache) == oldCache)
                {
                    // Cache successfully written
                    return newValue;
                }

                // Failed to write the new cache because another thread
                // already changed it; try again
            }
        }

        public void Clear()
        {
            _cache = _cache.Clear();
        }
    }
}
