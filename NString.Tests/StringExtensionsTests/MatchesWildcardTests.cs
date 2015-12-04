using System;
using NUnit.Framework;

namespace NString.Tests.StringExtensionsTests
{
    [TestFixture]
    public class MatchesWildcardTests
    {
        [Test]
        public void MatchesWildcard_Throws_If_Input_Is_Null()
        {
            const string s = null;
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            ExceptionAssert.Throws<ArgumentNullException>(() => s.MatchesWildcard("*"));
        }

        [Test]
        public void MatchesWildcard_Throws_If_Pattern_Is_Null()
        {
            const string s = "hello world";
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            ExceptionAssert.Throws<ArgumentNullException>(() => s.MatchesWildcard(null));
        }

        [Test]
        [TestCase("hello world", "*", true)]
        [TestCase("hello world", "hello*", true)]
        [TestCase("hello world", "*world", true)]
        [TestCase("hello world", "hel*rld", true)]
        [TestCase("hello world", "*hello*", true)]
        [TestCase("hello world", "*hello world", true)]
        [TestCase("hello world", "hello world*", true)]
        [TestCase("hello world", "hello", false)]
        [TestCase("hello world", "*hello", false)]
        [TestCase("hello world", "world*", false)]
        public void MatchesWildcard_Returns_True_If_Input_Matches_Pattern_With_Stars(string input, string pattern, bool expected)
        {
            Assert.AreEqual(expected, input.MatchesWildcard(pattern));
        }

        [Test]
        [TestCase("hello world", "???????????", true)]
        [TestCase("hello world", "hello ?????", true)]
        [TestCase("hello world", "????? world", true)]
        [TestCase("hello world", "hel?????rld", true)]
        [TestCase("hello world", "???lo wo???", true)]
        [TestCase("hello world", "?", false)]
        [TestCase("hello world", "?hello world", false)]
        [TestCase("hello world", "hello world?", false)]
        [TestCase("hello world", "?hello world?", false)]
        public void MatchesWildcard_Returns_True_If_Input_Matches_Pattern_With_QuestionMarks(string input, string pattern, bool expected)
        {
            Assert.AreEqual(expected, input.MatchesWildcard(pattern));
        }

        [Test]
        [TestCase("hello world", "HELLO WORLD", true)]
        [TestCase("hello world", "HELLO *", true)]
        [TestCase("hello world", "* WORLD", true)]
        [TestCase("hello world", "HELLO ?????", true)]
        public void MatchesWildcard_Ignores_Case(string input, string pattern, bool expected)
        {
            Assert.AreEqual(expected, input.MatchesWildcard(pattern, false));
        }

        [Test]
        [TestCase("hello world", "HELLO WORLD", false)]
        [TestCase("hello world", "HELLO *", false)]
        [TestCase("hello world", "* WORLD", false)]
        [TestCase("hello world", "HELLO ?????", false)]
        public void MatchesWildcard_Matches_Case(string input, string pattern, bool expected)
        {
            Assert.AreEqual(expected, input.MatchesWildcard(pattern));
        }
    }
}
