using System;
using Xunit;

namespace NString.Tests.StringExtensionsTests
{
    public class EllipsisTests
    {
        [Fact]
        public void Ellipsis_Throws_If_Argument_Is_Null()
        {
            TestHelper.AssertThrowsWhenArgumentNull(() => "abc".Ellipsis(10));
        }

        [Fact]
        public void Ellipsis_With_Custom_Ellipsis_String_Throws_If_Argument_Is_Null()
        {
            TestHelper.AssertThrowsWhenArgumentNull(() => "abc".Ellipsis(10, "----"));
        }

        [Fact]
        public void Ellipsis_Throws_If_Max_Length_Is_Less_Than_Length_Of_Ellipsis_String()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => "hello".Ellipsis(2));
        }

        [Fact]
        public void Ellipsis_Throws_If_Max_Length_Less_Than_Length_Of_Custom_Ellipsis_String()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => "hello".Ellipsis(4, "-----"));
        }

        [Fact]
        public void Ellipsis_Throws_If_Max_Length_Is_Less_Than_Zero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => "hello".Ellipsis(-1));
        }

        [Fact]
        public void Ellipsis_With_Custom_Ellipsis_String_Throws_If_Max_Length_Is_Less_Than_Zero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => "hello".Ellipsis(-1, "-----"));
        }

        [Fact]
        public void Ellipsis_Returns_Input_If_Not_Too_Long()
        {
            const string s = "hello";
            const string expected = "hello";
            string actual = s.Ellipsis(8);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Ellipsis_Returns_Input_Truncated_With_Ellipsis_If_Too_Long()
        {
            const string s = "hello world";
            const string expected = "hello...";
            string actual = s.Ellipsis(8);
            Assert.Equal(expected, actual);
        }

    }
}
