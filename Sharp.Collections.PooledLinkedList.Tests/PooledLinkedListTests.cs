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
using FluentAssertions;
using NUnit.Framework;
using static Sharp.Collections.NodeIdHelpers;

namespace Sharp.Collections
{
    [TestFixture]
    public class PooledLinkedListTests
    {
        [Test]
        public void Initial()
        {
            var list = new PooledLinkedList<string>();

            list.Should().HaveCount(0);
            list.Should().BeEmpty();

            list.Pool.Items.Should().BeEmpty();
            list.Pool.Nexts.Should().BeEmpty();
        }

        [Test]
        public void AddFirst_Once()
        {
            var list = new PooledLinkedList<string>();

            list.AddFirst("a");

            list.Should().HaveCount(1);
            list.Should().Equal("a");

            list.Pool.Items.Should().Equal("a");
            list.Pool.Nexts.Should().Equal(None);
        }

        [Test]
        public void AddFirst_Several()
        {
            var list = new PooledLinkedList<string>();

            list.AddFirst("a");
            list.AddFirst("b");
            list.AddFirst("c");

            list.Should().HaveCount(3);
            list.Should().Equal("c", "b", "a");

            list.Pool.Items.Should().Equal("a", "b", "c");
            list.Pool.Nexts.Should().Equal(None, 0, 1);
        }

        [Test]
        public void AddLast_Once()
        {
            var list = new PooledLinkedList<string>();

            list.AddLast("a");

            list.Should().HaveCount(1);
            list.Should().Equal("a");

            list.Pool.Items.Should().Equal("a");
            list.Pool.Nexts.Should().Equal(None);
        }

        [Test]
        public void AddLast_Several()
        {
            var list = new PooledLinkedList<string>();

            list.AddLast("a");
            list.AddLast("b");
            list.AddLast("c");

            list.Should().HaveCount(3);
            list.Should().Equal("a", "b", "c");

            list.Pool.Items.Should().Equal("a", "b", "c");
            list.Pool.Nexts.Should().Equal(1, 2, None);
        }

        [Test]
        public void TryRemoveFirst_NotEmpty()
        {
            var list = new PooledLinkedList<string>();

            list.AddLast("a");
            list.AddLast("b");
            list.AddLast("c");

            var removed = list.TryRemoveFirst(out var item);

            removed.Should().BeTrue();

            item.Should().Be("a");

            list.Should().HaveCount(2);
            list.Should().Equal("b", "c");

            list.Pool.Items.Should().Equal(null, "b", "c");
            list.Pool.Nexts.Should().Equal(None, 2, None);
        }

        [Test]
        public void IsReadOnly()
        {
            var list = new PooledLinkedList<string>();

            list.As<ICollection<string>>().IsReadOnly.Should().BeFalse();
        }
    }
}
