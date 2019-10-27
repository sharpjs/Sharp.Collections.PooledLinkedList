/*
    Copyright (C) 2019 Jeffrey Sharp

    Permission to use, copy, modify, and distribute this software for any
    purpose with or without fee is hereby granted, provided that the above
    copyright notice and this permission notice appear in all copies.

    THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
    WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
    MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
    ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
    WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
    ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
    OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using static Sharp.Collections.NodeIdHelpers;
using NodeId = System.Int32;

#if NULLABLE
using System.Diagnostics.CodeAnalysis;
#endif

namespace Sharp.Collections
{
    /// <summary>
    ///   A singly-linked list, implemented using a node pool for improved
    ///   cache performance relative to <see cref="LinkedList{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    ///   The type of items in the list.
    /// </typeparam>
    public partial class PooledLinkedList<T> : ICollection<T>
    {
        private readonly NodePool _pool;

        private NodeId _head;
        private NodeId _tail;
        private int    _count;

        /// <summary>
        ///   Initializes a new <see cref="PooledLinkedList{T}"/> instance.
        /// </summary>
        public PooledLinkedList()
        {
            _pool = new NodePool();
            _head = None;
            _tail = None;
        }

        // Test accessor
        internal NodePool Pool => _pool;

        /// <inheritdoc/>
        public int Count => _count;

        /// <inheritdoc/>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        ///   Adds the specified item at the start of the list.  This method
        ///   provides the 'push' operation when the list functions as a stack.
        /// </summary>
        /// <param name="item">
        ///   The item to add.
        /// </param>
        public void AddFirst(T item)
        {
            var id   = _pool.Store(item);
            var next = _head;

            _head = id;

            if (IsNone(next))
                _tail = id;
            else
                _pool.SetNext(id, next);

            _count++;
        }

        /// <summary>
        ///   Adds the specified item at the end of the list.  This method
        ///   provides the 'enqueue' operation when the list functions as a
        ///   queue.
        /// </summary>
        /// <param name="item">
        ///   The item to add.
        /// </param>
        public void AddLast(T item)
        {
            var id   = _pool.Store(item);
            var prev = _tail;

           _tail = id;

            if (IsNone(prev))
                _head = id;
            else
                _pool.SetNext(prev, id);

            _count++;
        }

        /// <inheritdoc/>
        void ICollection<T>.Add(T item) => AddLast(item);

        /// <summary>
        ///   Removes the first item if the list is not empty.  This method
        ///   provides the 'pop' operation when the list functions as a stack
        ///   and the 'dequeue' operation when the list functions as a queue.
        /// </summary>
        /// <param name="item">
        ///   Set on return.  If an item was removed, contains the item;
        ///   otherwise, contains the default value of type
        ///   <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        ///   <c>true</c> if an item was removed; <c>false</c> otherwise.
        /// </returns>
#if NULLABLE
        public bool TryRemoveFirst([MaybeNullWhen(false)] out T item)
#else
        public bool TryRemoveFirst(out T item)
#endif
        {
            var head = _head;

            if (IsNone(head))
            {
                item = default!;
                return false;
            }

            item = _pool.GetItem(head);

            Remove(None, head);
            return true;
        }

        /// <summary>
        ///   Removes the first item that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">
        ///   foo
        /// </param>
        /// <param name="item">
        ///   Set on return.  If an item was removed, contains the item;
        ///   otherwise, contains the default value of type
        ///   <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        ///   <c>true</c> if an item was removed; <c>false</c> otherwise.
        /// </returns>
        public bool TryRemoveFirst(
            Func<T, bool> predicate,
#if NULLABLE
            [MaybeNullWhen(false)]
#endif
            out T item)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var (prev, id) = (None, _head);

            if (!Search(predicate, ref prev, ref id, out item))
                return false;

            Remove(prev, id);
            return true;
        }

        /// <summary>
        ///   Removes the first item that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">
        ///   foo
        /// </param>
        /// <param name="max">
        /// </param>
        /// <returns>
        ///   <c>true</c> if an item was removed; <c>false</c> otherwise.
        /// </returns>
        public bool TryRemoveAll(Func<T, bool> predicate, int max = 0)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var (prev, id, n) = (None, _head, 0);

            for (n = 0; n < max; n++)
            {
                if (!Search(predicate, ref prev, ref id, out _))
                    break;

                Link(prev, id = _pool.GetNext(id));
                _count--;
            }

            return true;
        }

        private bool Search(
            Func<T, bool> predicate,
            ref NodeId    prev,
            ref NodeId    node,
#if NULLABLE
            [MaybeNullWhen(false)]
#endif
            out T item)
        {
            while (IsSome(node))
            {
                item = _pool.GetItem(node);

                if (predicate(item))
                    return true;

                prev = node;
                node = _pool.GetNext(node);
            }

            item = default!;
            return false;
        }

        private void Remove(NodeId prev, NodeId node)
        {
            Link(prev, _pool.GetNext(node));
            _count--;
        }

        private void Link(NodeId node, NodeId next)
        {
            if (IsNone(node))
                _head = next;
            else
                _pool.SetNext(node, next);

            if (IsNone(next))
                _tail = node;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _pool.Free(_head);

            _head  = None;
            _tail  = None;

            _count = 0;
        }

        /// <inheritdoc/>
        bool ICollection<T>.Remove(T item)
        {
            var comparer = EqualityComparer<T>.Default;

            return TryRemoveFirst(x => comparer.Equals(x, item), out _);
        }

        /// <inheritdoc/>
        bool ICollection<T>.Contains(T item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            var i = arrayIndex;

            foreach (var item in this)
                array[i++] = item;
        }

        /// <inheritdoc/>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
