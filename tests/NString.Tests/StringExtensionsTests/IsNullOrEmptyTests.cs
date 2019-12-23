using Xunit;

namespace NString.Tests.StringExtensionsTests
{
    public class IsNullOrEmptyTests
    {
        [Fact]
        public void IsNullOrEmpty_Returns_True_If_String_Is_Null()
        {
            const string? s = null;
            const bool expected = true;
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            bool actual = s.IsNullOrEmpty();
            Assert.Equal(expected, actual);
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
        }

        [Fact]
        public void IsNullOrEmpty_Returns_True_If_String_Is_Empty()
        {
            string s = string.Empty;
            const bool expected = true;
            bool actual = s.IsNullOrEmpty();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsNullOrEmpty_Returns_False_If_String_Is_Not_Empty()
        {
            string s = "hello";
            const bool expected = false;
            bool actual = s.IsNullOrEmpty();
            Assert.Equal(expected, actual);
        }
    }
}
