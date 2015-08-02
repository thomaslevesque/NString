using NUnit.Framework;

namespace NString.Tests.StringExtensionsTests
{
    [TestFixture]
    public class IsNullOrWhitespaceTests
    {
        [Test]
        public void IsNullOrWhiteSpace_Returns_True_If_String_Is_Null()
        {
            const string s = null;
            const bool expected = true;
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            bool actual = s.IsNullOrWhiteSpace();
            Assert.AreEqual(expected, actual);
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
        }

        [Test]
        public void IsNullOrWhiteSpace_Returns_True_If_String_Is_Empty()
        {
            string s = string.Empty;
            const bool expected = true;
            bool actual = s.IsNullOrWhiteSpace();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsNullOrWhiteSpace_Returns_True_If_String_Is_WhiteSpace()
        {
            string s = "  \t ";
            const bool expected = true;
            bool actual = s.IsNullOrWhiteSpace();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsNullOrWhiteSpace_Returns_False_If_String_Has_NonWhitespace_Chars()
        {
            string s = "hello";
            const bool expected = false;
            bool actual = s.IsNullOrWhiteSpace();
            Assert.AreEqual(expected, actual);
        }
    }
}
