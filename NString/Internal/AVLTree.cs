using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NString.Internal
{
    interface IStack<T> : IEnumerable<T>
    {
        IStack<T> Pop();
        IStack<T> Push(T value);
        T Peek();
        bool IsEmpty { get; }
    }

    sealed class ImmutableStack<T> : IStack<T>
    {
        private sealed class EmptyStack : IStack<T>
        {
            public bool IsEmpty { get { return true; } }
            public T Peek() { throw new Exception("Empty stack"); }
            public IStack<T> Pop() { throw new Exception("Empty stack"); }
            public IStack<T> Push(T value) { return new ImmutableStack<T>(value, this); }
            public IEnumerator<T> GetEnumerator() { yield break; }
            IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
        }
        private static readonly EmptyStack empty = new EmptyStack();
        public static IStack<T> Empty { get { return empty; } }
        private readonly T _head;
        private readonly IStack<T> _tail;
        private ImmutableStack(T head, IStack<T> tail)
        {
            this._head = head;
            this._tail = tail;
        }
        public bool IsEmpty { get { return false; } }
        public T Peek() { return _head; }
        public IStack<T> Pop() { return _tail; }
        public IStack<T> Push(T value) { return new ImmutableStack<T>(value, this); }
        public static IStack<T> Push(T head, IStack<T> tail) { return new ImmutableStack<T>(head, tail); }
        public IEnumerator<T> GetEnumerator()
        {
            for (IStack<T> stack = this; !stack.IsEmpty; stack = stack.Pop())
                yield return stack.Peek();
        }
        IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
    }

    interface IBinaryTree<V>
    {
        bool IsEmpty { get; }
        V Value { get; }
        IBinaryTree<V> Left { get; }
        IBinaryTree<V> Right { get; }
    }

    interface IMap<TKey, TValue> where TKey : IComparable<TKey>
    {
        bool Contains(TKey key);
        TValue Lookup(TKey key);
        IMap<TKey, TValue> Add(TKey key, TValue value);
        IMap<TKey, TValue> Remove(TKey key);
        IEnumerable<TKey> Keys { get; }
        IEnumerable<TValue> Values { get; }
        IEnumerable<KeyValuePair<TKey, TValue>> Pairs { get; }
    }

    interface IBinarySearchTree<TKey, TValue> :
        IBinaryTree<TValue>,
        IMap<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        TKey Key { get; }
        new IBinarySearchTree<TKey, TValue> Left { get; }
        new IBinarySearchTree<TKey, TValue> Right { get; }
        new IBinarySearchTree<TKey, TValue> Add(TKey key, TValue value);
        new IBinarySearchTree<TKey, TValue> Remove(TKey key);
        IBinarySearchTree<TKey, TValue> Search(TKey key);
    }

    sealed class AVLTree<TKey, TValue> : IBinarySearchTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private sealed class EmptyAVLTree : IBinarySearchTree<TKey, TValue>
        {
            // IBinaryTree
            public bool IsEmpty { get { return true; } }
            public TValue Value { get { throw new Exception("empty tree"); } }
            IBinaryTree<TValue> IBinaryTree<TValue>.Left { get { throw new Exception("empty tree"); } }
            IBinaryTree<TValue> IBinaryTree<TValue>.Right { get { throw new Exception("empty tree"); } }
            // IBinarySearchTree
            public IBinarySearchTree<TKey, TValue> Left { get { throw new Exception("empty tree"); } }
            public IBinarySearchTree<TKey, TValue> Right { get { throw new Exception("empty tree"); } }
            public IBinarySearchTree<TKey, TValue> Search(TKey key) { return this; }
            public TKey Key { get { throw new Exception("empty tree"); } }
            public IBinarySearchTree<TKey, TValue> Add(TKey key, TValue value) { return new AVLTree<TKey, TValue>(key, value, this, this); }
            public IBinarySearchTree<TKey, TValue> Remove(TKey key) { throw new Exception("Cannot remove item that is not in tree."); }
            // IMap
            public bool Contains(TKey key) { return false; }
            public TValue Lookup(TKey key) { throw new Exception("not found"); }
            IMap<TKey, TValue> IMap<TKey, TValue>.Add(TKey key, TValue value) { return this.Add(key, value); }
            IMap<TKey, TValue> IMap<TKey, TValue>.Remove(TKey key) { return this.Remove(key); }
            public IEnumerable<TKey> Keys { get { yield break; } }
            public IEnumerable<TValue> Values { get { yield break; } }
            public IEnumerable<KeyValuePair<TKey, TValue>> Pairs { get { yield break; } }
        }
        private static readonly EmptyAVLTree empty = new EmptyAVLTree();
        public static IBinarySearchTree<TKey, TValue> Empty { get { return empty; } }
        private readonly TKey _key;
        private readonly TValue _value;
        private readonly IBinarySearchTree<TKey, TValue> _left;
        private readonly IBinarySearchTree<TKey, TValue> _right;
        private readonly int _height;
        private AVLTree(TKey key, TValue value, IBinarySearchTree<TKey, TValue> left, IBinarySearchTree<TKey, TValue> right)
        {
            this._key = key;
            this._value = value;
            this._left = left;
            this._right = right;
            this._height = 1 + Math.Max(Height(left), Height(right));
        }
        // IBinaryTree
        public bool IsEmpty { get { return false; } }
        public TValue Value { get { return _value; } }
        IBinaryTree<TValue> IBinaryTree<TValue>.Left { get { return _left; } }
        IBinaryTree<TValue> IBinaryTree<TValue>.Right { get { return _right; } }
        // IBinarySearchTree
        public IBinarySearchTree<TKey, TValue> Left { get { return _left; } }
        public IBinarySearchTree<TKey, TValue> Right { get { return _right; } }
        public IBinarySearchTree<TKey, TValue> Search(TKey key)
        {
            int compare = key.CompareTo(Key);
            if (compare == 0)
                return this;
            if (compare > 0)
                return Right.Search(key);
            return Left.Search(key);
        }
        public TKey Key { get { return _key; } }
        public IBinarySearchTree<TKey, TValue> Add(TKey key, TValue value)
        {
            AVLTree<TKey, TValue> result;
            if (key.CompareTo(Key) > 0)
                result = new AVLTree<TKey, TValue>(Key, Value, Left, Right.Add(key, value));
            else
                result = new AVLTree<TKey, TValue>(Key, Value, Left.Add(key, value), Right);
            return MakeBalanced(result);
        }
        public IBinarySearchTree<TKey, TValue> Remove(TKey key)
        {
            IBinarySearchTree<TKey, TValue> result;
            int compare = key.CompareTo(Key);
            if (compare == 0)
            {
                // We have a match. If this is a leaf, just remove it
                // by returning Empty.  If we have only one child,
                // replace the node with the child.
                if (Right.IsEmpty && Left.IsEmpty)
                    result = Empty;
                else if (Right.IsEmpty)
                    result = Left;
                else if (Left.IsEmpty)
                    result = Right;
                else
                {
                    // We have two children. Remove the next-highest node and replace
                    // this node with it.
                    IBinarySearchTree<TKey, TValue> successor = Right;
                    while (!successor.Left.IsEmpty)
                        successor = successor.Left;
                    result = new AVLTree<TKey, TValue>(successor.Key, successor.Value, Left, Right.Remove(successor.Key));
                }
            }
            else if (compare < 0)
                result = new AVLTree<TKey, TValue>(Key, Value, Left.Remove(key), Right);
            else
                result = new AVLTree<TKey, TValue>(Key, Value, Left, Right.Remove(key));
            return MakeBalanced(result);
        }
        // IMap
        public bool Contains(TKey key) { return !Search(key).IsEmpty; }
        IMap<TKey, TValue> IMap<TKey, TValue>.Add(TKey key, TValue value) { return this.Add(key, value); }
        IMap<TKey, TValue> IMap<TKey, TValue>.Remove(TKey key) { return this.Remove(key); }
        public TValue Lookup(TKey key)
        {
            IBinarySearchTree<TKey, TValue> tree = Search(key);
            if (tree.IsEmpty)
                throw new Exception("not found");
            return tree.Value;
        }
        public IEnumerable<TKey> Keys { get { return from t in Enumerate() select t.Key; } }
        public IEnumerable<TValue> Values
        {
            get { return from t in Enumerate() select t.Value; }
        }
        public IEnumerable<KeyValuePair<TKey, TValue>> Pairs
        {
            get
            {
                return from t in Enumerate() select new KeyValuePair<TKey, TValue>(t.Key, t.Value);
            }
        }
        private IEnumerable<IBinarySearchTree<TKey, TValue>> Enumerate()
        {
            var stack = ImmutableStack<IBinarySearchTree<TKey, TValue>>.Empty;
            for (IBinarySearchTree<TKey, TValue> current = this; !current.IsEmpty || !stack.IsEmpty; current = current.Right)
            {
                while (!current.IsEmpty)
                {
                    stack = stack.Push(current);
                    current = current.Left;
                }
                current = stack.Peek();
                stack = stack.Pop();
                yield return current;
            }
        }
        // Static helpers for tree balancing
        private static int Height(IBinarySearchTree<TKey, TValue> tree)
        {
            if (tree.IsEmpty)
                return 0;
            var avlTree = (AVLTree<TKey, TValue>) tree;
            return avlTree._height;
        }
        private static IBinarySearchTree<TKey, TValue> RotateLeft(IBinarySearchTree<TKey, TValue> tree)
        {
            if (tree.Right.IsEmpty)
                return tree;
            return new AVLTree<TKey, TValue>(tree.Right.Key, tree.Right.Value,
                new AVLTree<TKey, TValue>(tree.Key, tree.Value, tree.Left, tree.Right.Left),
                tree.Right.Right);
        }
        private static IBinarySearchTree<TKey, TValue> RotateRight(IBinarySearchTree<TKey, TValue> tree)
        {
            if (tree.Left.IsEmpty)
                return tree;
            return new AVLTree<TKey, TValue>(tree.Left.Key, tree.Left.Value, tree.Left.Left,
                new AVLTree<TKey, TValue>(tree.Key, tree.Value, tree.Left.Right, tree.Right));
        }
        private static IBinarySearchTree<TKey, TValue> DoubleLeft(IBinarySearchTree<TKey, TValue> tree)
        {
            if (tree.Right.IsEmpty)
                return tree;
            AVLTree<TKey, TValue> rotatedRightChild = new AVLTree<TKey, TValue>(tree.Key, tree.Value, tree.Left, RotateRight(tree.Right));
            return RotateLeft(rotatedRightChild);
        }
        private static IBinarySearchTree<TKey, TValue> DoubleRight(IBinarySearchTree<TKey, TValue> tree)
        {
            if (tree.Left.IsEmpty)
                return tree;
            AVLTree<TKey, TValue> rotatedLeftChild = new AVLTree<TKey, TValue>(tree.Key, tree.Value, RotateLeft(tree.Left), tree.Right);
            return RotateRight(rotatedLeftChild);
        }
        private static int Balance(IBinarySearchTree<TKey, TValue> tree)
        {
            if (tree.IsEmpty)
                return 0;
            return Height(tree.Right) - Height(tree.Left);
        }
        private static bool IsRightHeavy(IBinarySearchTree<TKey, TValue> tree) { return Balance(tree) >= 2; }
        private static bool IsLeftHeavy(IBinarySearchTree<TKey, TValue> tree) { return Balance(tree) <= -2; }
        private static IBinarySearchTree<TKey, TValue> MakeBalanced(IBinarySearchTree<TKey, TValue> tree)
        {
            IBinarySearchTree<TKey, TValue> result;
            if (IsRightHeavy(tree))
            {
                if (IsLeftHeavy(tree.Right))
                    result = DoubleLeft(tree);
                else
                    result = RotateLeft(tree);
            }
            else if (IsLeftHeavy(tree))
            {
                if (IsRightHeavy(tree.Left))
                    result = DoubleRight(tree);
                else
                    result = RotateRight(tree);
            }
            else
                result = tree;
            return result;
        }
    }
 
}
