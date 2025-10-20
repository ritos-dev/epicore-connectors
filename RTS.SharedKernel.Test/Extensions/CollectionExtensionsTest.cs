using FluentAssertions;
using RTS.SharedKernel.Extensions;

namespace RTS.SharedKernel.Test.Extensions
{
    public class CollectionExtensionsTest
    {
        [Fact]
        public void UnorderedEquals_BothNull_ReturnsTrue()
        {
            IReadOnlyCollection<int>? first = null;
            IReadOnlyCollection<int>? second = null;

            first.UnorderedEquals(second).Should().BeTrue();
        }

        [Fact]
        public void UnorderedEquals_OneNull_ReturnsFalse()
        {
            IReadOnlyCollection<int>? first = null;
            IReadOnlyCollection<int>? second = [1];

            first.UnorderedEquals(second).Should().BeFalse();
            second.UnorderedEquals(first).Should().BeFalse();
        }

        [Fact]
        public void UnorderedEquals_SameReference_ReturnsTrue()
        {
            var list = new List<int> { 1, 2, 3 };

            list.UnorderedEquals(list).Should().BeTrue();
        }

        [Fact]
        public void UnorderedEquals_DifferentCounts_ReturnsFalse()
        {
            var first = new List<int> { 1, 2, 3 };
            var second = new List<int> { 1, 2 };

            first.UnorderedEquals(second).Should().BeFalse();
        }

        [Fact]
        public void UnorderedEquals_SameElementsDifferentOrder_ReturnsTrue()
        {
            var first = new List<int> { 1, 2, 3 };
            var second = new List<int> { 3, 2, 1 };

            first.UnorderedEquals(second).Should().BeTrue();
        }

        [Fact]
        public void UnorderedEquals_DifferentElements_ReturnsFalse()
        {
            var first = new List<int> { 1, 2, 3 };
            var second = new List<int> { 4, 5, 6 };

            first.UnorderedEquals(second).Should().BeFalse();
        }

        [Fact]
        public void UnorderedEquals_DuplicateElements_ReturnsTrue()
        {
            var first = new List<int> { 1, 2, 2, 3 };
            var second = new List<int> { 2, 1, 3, 2 };

            first.UnorderedEquals(second).Should().BeTrue();
        }

        [Fact]
        public void UnorderedEquals_DuplicateElementsDifferentCounts_ReturnsFalse()
        {
            var first = new List<int> { 1, 2, 2, 3 };
            var second = new List<int> { 2, 1, 3, 3 };

            first.UnorderedEquals(second).Should().BeFalse();
        }
    }
}
