using System;
using Xunit;

namespace NString.Tests.StringExtensionsTests
{
    public class MatchesWildcardTests
    {
        [Fact]
        public void MatchesWildcard_Throws_If_Argument_Is_Null()
        {
            TestHelper.AssertThrowsWhenArgumentNull(() => "abcde".MatchesWildcard("*", true));
        }

        [Theory]
        [InlineData("hello world", "*", true)]
        [InlineData("hello world", "hello*", true)]
        [InlineData("hello world", "*world", true)]
        [InlineData("hello world", "hel*rld", true)]
        [InlineData("hello world", "*hello*", true)]
        [InlineData("hello world", "*hello world", true)]
        [InlineData("hello world", "hello world*", true)]
        [InlineData("hello world", "hello", false)]
        [InlineData("hello world", "*hello", false)]
        [InlineData("hello world", "world*", false)]
        public void MatchesWildcard_Returns_True_If_Input_Matches_Pattern_With_Stars(string input, string pattern, bool expected)
        {
            Assert.Equal(expected, input.MatchesWildcard(pattern));
        }

        [Theory]
        [InlineData("hello world", "???????????", true)]
        [InlineData("hello world", "hello ?????", true)]
        [InlineData("hello world", "????? world", true)]
        [InlineData("hello world", "hel?????rld", true)]
        [InlineData("hello world", "???lo wo???", true)]
        [InlineData("hello world", "?", false)]
        [InlineData("hello world", "?hello world", false)]
        [InlineData("hello world", "hello world?", false)]
        [InlineData("hello world", "?hello world?", false)]
        public void MatchesWildcard_Returns_True_If_Input_Matches_Pattern_With_QuestionMarks(string input, string pattern, bool expected)
        {
            Assert.Equal(expected, input.MatchesWildcard(pattern));
        }

        [Theory]
        [InlineData("hello world", "HELLO WORLD", true)]
        [InlineData("hello world", "HELLO *", true)]
        [InlineData("hello world", "* WORLD", true)]
        [InlineData("hello world", "HELLO ?????", true)]
        public void MatchesWildcard_Ignores_Case(string input, string pattern, bool expected)
        {
            Assert.Equal(expected, input.MatchesWildcard(pattern, false));
        }

        [Theory]
        [InlineData("hello world", "HELLO WORLD", false)]
        [InlineData("hello world", "HELLO *", false)]
        [InlineData("hello world", "* WORLD", false)]
        [InlineData("hello world", "HELLO ?????", false)]
        public void MatchesWildcard_Matches_Case(string input, string pattern, bool expected)
        {
            Assert.Equal(expected, input.MatchesWildcard(pattern));
        }
    }
}
