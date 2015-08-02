using NUnit.Framework;

namespace NString.Tests.StringExtensionsTests
{
    [TestFixture]
    public class IsNullOrEmptyTests
    {
        [Test]
        public void IsNullOrEmpty_Returns_True_If_String_Is_Null()
        {
            const string s = null;
            const bool expected = true;
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            bool actual = s.IsNullOrEmpty();
            Assert.AreEqual(expected, actual);
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
        }

        [Test]
        public void IsNullOrEmpty_Returns_True_If_String_Is_Empty()
        {
            string s = string.Empty;
            const bool expected = true;
            bool actual = s.IsNullOrEmpty();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsNullOrEmpty_Returns_False_If_String_Is_Not_Empty()
        {
            string s = "hello";
            const bool expected = false;
            bool actual = s.IsNullOrEmpty();
            Assert.AreEqual(expected, actual);
        }
    }
}
