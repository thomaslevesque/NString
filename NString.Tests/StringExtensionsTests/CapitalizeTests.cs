using NUnit.Framework;

namespace NString.Tests.StringExtensionsTests
{
    [TestFixture]
    public class CapitalizeTests
    {
        [Test]
        public void Capitalize_Returns_Input_With_UpperCase_First_Letter()
        {
            const string s = "hello";
            const string expected = "Hello";
            string actual = s.Capitalize();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Capitalize_Returns_Input_If_Empty()
        {
            const string s = "";
            const string expected = "";
            string actual = s.Capitalize();
            Assert.AreEqual(expected, actual);
        }
    }
}
