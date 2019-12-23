using System;
using Xunit;

namespace NString.Tests.StringExtensionsTests
{
        public class ReplaceAtTests
    {
        [Fact]
        public void ReplaceAt_Throws_If_Input_Is_Null()
        {
            const string? input = null;
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentNullException>(() => input!.ReplaceAt(0, ' '));
        }

        [Theory]
        [InlineData("foo", -1)]
        [InlineData("foo", 3)]
        public void ReplaceAt_Throws_If_Index_Out_Of_Range(string input, int index)
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentOutOfRangeException>(() => input.ReplaceAt(index, ' '));
        }


        [Fact]
        public void ReplaceAt_Replaces_Specified_Character()
        {
            const string s = "hello world";
            const string expected = "hello_world";
            string actual = s.ReplaceAt(5, '_');
            Assert.Equal(expected, actual);
        }

    }
}
