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

namespace Sharp.Collections
{
    partial class PooledLinkedList<T>
    {
        /// <summary>
        ///   Enumerates the items of a <see cref="PooledLinkedList{T}"/>.
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            private readonly NodePool _pool;
            private          NodeId   _next;
            private          NodeId   _id;

            /// <summary>
            ///   Initializes a new <see cref="Enumerator"/> instance for the
            ///   specified <see cref="PooledLinkedList{T}"/>.
            /// </summary>
            /// <param name="list">
            ///   The list to enumerate.
            /// </param>
            internal Enumerator(PooledLinkedList<T> list)
            {
                _pool = list._pool;
                _next = list._head;
                _id   = None;
            }

            /// <inheritdoc/>
            public T Current => _pool.GetItem(_id);

            /// <inheritdoc/>
            object? IEnumerator.Current => Current;

            /// <inheritdoc/>
            public bool MoveNext()
            {
                if (IsNone(_id = _next))
                    return false;

                _next = _pool.GetNext(_next);
                return true;
            }

            /// <inheritdoc/>
            void IEnumerator.Reset() => throw new NotSupportedException();

            /// <inheritdoc/>
            void IDisposable.Dispose() { }
        }
    }
}
