using System;
using JetBrains.Annotations;

namespace NString.Internal
{
    class Cache<TKey, TValue> where TKey : IComparable<TKey>
    {
        private readonly object _lock = new object();
        private IBinarySearchTree<TKey, TValue> _tree = AVLTree<TKey, TValue>.Empty;

        public TValue GetOrAdd(TKey key, [NotNull] Func<TKey, TValue> valueFactory)
        {
            if (valueFactory == null) throw new ArgumentNullException("valueFactory");
            var tree = _tree;
            var node = tree.Search(key);
            if (!node.IsEmpty)
                return node.Value;

            var value = valueFactory(key);
            lock (_lock)
            {
                // Search again in case the key has been added while we were waiting for the lock
                tree = _tree;
                node = tree.Search(key);
                if (!node.IsEmpty)
                    return node.Value;
                tree = tree.Add(key, value);
                _tree = tree;
            }
            return value;
        }

        public void Clear()
        {
            _tree = AVLTree<TKey, TValue>.Empty;
        }
    }
}
