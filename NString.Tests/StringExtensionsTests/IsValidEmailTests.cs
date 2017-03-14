using System;
using Xunit;

namespace NString.Tests.StringExtensionsTests
{
        public class IsValidEmailTests
    {
        [Fact]
        public void IsValidEmail_Throws_If_Input_Is_Null()
        {
            const string input = null;
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentNullException>(() => input.IsValidEmail());
        }

        [Theory]
        [InlineData("test@test.com", true)]
        [InlineData("test.test@test.com", true)]
        [InlineData("test", false)]
        [InlineData("test@test", false)]
        [InlineData("test@test@test.com", false)]
        public void IsValidEmail_Returns_True_If_Email_Is_Valid(string input, bool expected)
        {
            Assert.Equal(expected, input.IsValidEmail());
        }
    }
}
