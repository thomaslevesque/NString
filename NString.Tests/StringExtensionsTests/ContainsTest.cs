using System;
using Xunit;

namespace NString.Tests.StringExtensionsTests
{
        public class ContainsTest
    {
        [Fact]
        public void Contains_Throws_If_Input_Is_Null()
        {
            const string input = null;
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => input.Contains("", StringComparison.CurrentCultureIgnoreCase));
        }

        [Fact]
        public void Contains_Throws_If_Substring_Is_Null()
        {
            const string input = "";
            Assert.Throws<ArgumentNullException>(() => input.Contains(null, StringComparison.CurrentCultureIgnoreCase));
        }

        [Fact]
        public void Contains_Throws_If_ComparisonType_Is_Undefined()
        {
            const string input = "";
            Assert.Throws<ArgumentOutOfRangeException>(() => input.Contains("", (StringComparison)42));
        }

        [Theory]
        [InlineData("Hello World", "WORLD", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("Hello World", "world", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("Hello World", "WORLD", StringComparison.CurrentCulture, false)]
        [InlineData("Hello World", "World", StringComparison.CurrentCulture, true)]
        public void Contains_Returns_True_If_Input_Contains_Substring(string input, string subString, StringComparison comparisonType, bool expected)
        {
            Assert.Equal(expected, input.Contains(subString, comparisonType));
        }
    }
}
