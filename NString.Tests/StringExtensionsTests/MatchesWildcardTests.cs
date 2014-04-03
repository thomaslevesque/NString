using System;
using NUnit.Framework;

namespace NString.Tests.StringExtensionsTests
{
    [TestFixture]
    public class MatchesWildcardTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MatchesWildcard_Throws_If_Input_Is_Null()
        {
            const string s = null;
            s.MatchesWildcard("*");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MatchesWildcard_Throws_If_Pattern_Is_Null()
        {
            const string s = "hello world";
            s.MatchesWildcard(null);
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
    }
}
