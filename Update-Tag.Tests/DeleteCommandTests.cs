using System;
using System.Collections.Generic;
using Xunit;

namespace Update_Tag.Tests
{
    public class DeleteCommandTests
    {
        [Fact]
        public void ParseSelection_parses_comma_separated_indexes()
        {
            var actual = Update_Tag.DeleteCommand.ParseSelection("1,3,7", 10);

            Assert.Equal(
                new HashSet<int> { 1, 3, 7 },
                actual);
        }

        [Fact]
        public void ParseSelection_parses_single_range_inclusive()
        {
            var actual = Update_Tag.DeleteCommand.ParseSelection("1-7", 10);

            Assert.Equal(
                new HashSet<int> { 1, 2, 3, 4, 5, 6, 7 },
                actual);
        }

        [Fact]
        public void ParseSelection_parses_mixed_ranges_and_indexes()
        {
            var actual = Update_Tag.DeleteCommand.ParseSelection("2-4,1", 10);

            Assert.Equal(
                new HashSet<int> { 1, 2, 3, 4 },
                actual);
        }

        [Fact]
        public void ParseSelection_deduplicates_overlapping_ranges()
        {
            var actual = Update_Tag.DeleteCommand.ParseSelection("1-3,2", 10);

            Assert.Equal(
                new HashSet<int> { 1, 2, 3 },
                actual);
        }

        [Fact]
        public void ParseSelection_rejects_out_of_bounds_index()
        {
            Assert.Throws<ArgumentException>(() =>
                Update_Tag.DeleteCommand.ParseSelection("0", 10));
        }

        [Fact]
        public void ParseSelection_rejects_out_of_bounds_range()
        {
            Assert.Throws<ArgumentException>(() =>
                Update_Tag.DeleteCommand.ParseSelection("1-11", 10));
        }

        [Fact]
        public void ParseSelection_rejects_invalid_range_order()
        {
            Assert.Throws<ArgumentException>(() =>
                Update_Tag.DeleteCommand.ParseSelection("5-1", 10));
        }

        [Fact]
        public void ParseSelection_rejects_non_numeric_tokens()
        {
            Assert.Throws<ArgumentException>(() =>
                Update_Tag.DeleteCommand.ParseSelection("a,1", 10));
        }
    }
}

