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

using System.Collections.Generic;
using static Sharp.Collections.NodeIdHelpers;
using NodeId = System.Int32;

namespace Sharp.Collections
{
    partial class PooledLinkedList<T>
    {
        internal class NodePool
        {
            private readonly List<T>      _items;
            private readonly List<NodeId> _nexts;

            // List of unused nodes
            private NodeId _head;
            private NodeId _tail;

            internal NodePool()
            {
                _items = new List<T>();
                _nexts = new List<NodeId>();
                _head  = None;
                _tail  = None;
            }

            // Test accessors
            internal IReadOnlyList<T>      Items => _items;
            internal IReadOnlyList<NodeId> Nexts => _nexts;

            public T GetItem(NodeId id)
            {
                return _items[id];
            }

            public void SetItem(NodeId id, T item)
            {
                _items[id] = item;
            }

            public NodeId GetNext(NodeId id)
            {
                return _nexts[id];
            }

            public void SetNext(NodeId id, NodeId next)
            {
                _nexts[id] = next;
            }

            public (T Item, NodeId Next) GetNode(NodeId id)
            {
                return (_items[id], _nexts[id]);
            }

            public void SetNode(NodeId id, T item, NodeId next)
            {
                _items[id] = item;
                _nexts[id] = next;
            }

            public NodeId Store(T item)
            {
                var head = _head;

                return IsNone(head)
                    ? StoreNew (      item)
                    : StoreAt  (head, item);
            }

            private NodeId StoreNew(T item)
            {
                var id = _items.Count;

                _items.Add(item);
                _nexts.Add(None);

                return id;
            }

            private NodeId StoreAt(NodeId id, T item)
            {
                // assume: id == _head

                var head = GetNext(id);
                   _head = head;

                if (IsNone(head))
                    _tail = None;

                SetNode(id, item, None);
                return id;
            }

            public void Free(NodeId id)
            {
                var tail = _tail;
                   _tail = id;

                if (IsNone(tail))
                    _head = id;
                else
                    SetNext(tail, id);

                do
                {
                    SetItem(id, default!);
                    id = GetNext(id);
                }
                while (IsSome(id));
            }
        }
    }
}
