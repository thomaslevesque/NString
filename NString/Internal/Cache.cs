using System;
using System.Collections.Immutable;
using System.Threading;
using JetBrains.Annotations;

namespace NString.Internal
{
    class Cache<TKey, TValue>
    {
        private IImmutableDictionary<TKey, TValue> _cache = ImmutableDictionary.Create<TKey, TValue>();

        public TValue GetOrAdd(TKey key, [NotNull] Func<TValue> valueFactory)
        {
            valueFactory.CheckArgumentNull("valueFactory");

            Lazy<TValue> newValue = new Lazy<TValue>(valueFactory);

            while (true)
            {
                var oldCache = _cache;
                TValue value;
                if (oldCache.TryGetValue(key, out value))
                    return value;

                // Value not found ; create it and add it to the cache
                value = newValue.Value;
                var newCache = oldCache.Add(key, value);
                if (Interlocked.CompareExchange(ref _cache, newCache, oldCache) == oldCache)
                {
                    // Cache successfully written
                    return value;
                }

                // Failed to write the new cache because another thread
                // already changed it; try again
            }
        }

        public void Clear()
        {
            while (true)
            {
                var oldCache = _cache;
                var newCache = _cache.Clear();
                if (Interlocked.CompareExchange(ref _cache, newCache, oldCache) == oldCache)
                {
                    // Cache successfully written
                    return;
                }
                // Failed to write the new cache because another thread
                // already changed it; try again
            }
        }
    }
}
