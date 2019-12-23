using System;
using System.Collections.Generic;
using Xunit;

namespace NString.Tests.StringExtensionsTests
{
        public class JoinTests
    {
        [Fact]
        public void Join_Throws_If_Values_Is_Null()
        {
            const IEnumerable<string>? values = null;
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => values!.Join(" "));
        }

        [Fact]
        public void Join_Concatenates_Values_If_Separator_Is_Null()
        {
            var values = new[] { "Hello", "World", "!" };
            string expected = String.Concat(values);
            string actual = values.Join();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Join_Concatenates_Values_With_Separator()
        {
            var values = new[] { "Hello", "World", "!" };
            string expected = String.Join(" ", values);
            string actual = values.Join(" ");
            Assert.Equal(expected, actual);
        }
    }
}
