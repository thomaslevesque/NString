using Xunit;

namespace NString.Tests.StringExtensionsTests
{
        public class CapitalizeTests
    {
        [Fact]
        public void Capitalize_Returns_Input_With_UpperCase_First_Letter()
        {
            const string s = "hello";
            const string expected = "Hello";
            string actual = s.Capitalize();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Capitalize_Returns_Input_If_Empty()
        {
            const string s = "";
            const string expected = "";
            string actual = s.Capitalize();
            Assert.Equal(expected, actual);
        }
    }
}
