using System;
using Xunit;

namespace NString.Tests.StringExtensionsTests
{
    public class JoinTests
    {
        [Fact]
        public void Join_Throws_If_Argument_Is_Null()
        {
            TestHelper.AssertThrowsWhenArgumentNull(() => new[] { "a", "b", "c" }.Join(" "));
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
