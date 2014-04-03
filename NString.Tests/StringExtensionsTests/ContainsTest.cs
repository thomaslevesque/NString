using System;
using NUnit.Framework;

namespace NString.Tests.StringExtensionsTests
{
    [TestFixture]
    public class ContainsTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Contains_Throws_If_Input_Is_Null()
        {
            const string input = null;
            input.Contains("", StringComparison.CurrentCultureIgnoreCase);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Contains_Throws_If_Substring_Is_Null()
        {
            const string input = "";
            input.Contains(null, StringComparison.CurrentCultureIgnoreCase);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Contains_Throws_If_ComparisonType_Is_Undefined()
        {
            const string input = "";
            input.Contains("", (StringComparison)42);
        }

        [Test]
        [TestCase("Hello World", "WORLD", StringComparison.CurrentCultureIgnoreCase, true)]
        [TestCase("Hello World", "world", StringComparison.CurrentCultureIgnoreCase, true)]
        [TestCase("Hello World", "WORLD", StringComparison.CurrentCulture, false)]
        [TestCase("Hello World", "World", StringComparison.CurrentCulture, true)]
        public void Contains_Returns_True_If_Input_Contains_Substring(string input, string subString, StringComparison comparisonType, bool expected)
        {
            Assert.AreEqual(expected, input.Contains(subString, comparisonType));
        }
    }
}
