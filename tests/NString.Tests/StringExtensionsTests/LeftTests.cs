using System;
using Xunit;

namespace NString.Tests.StringExtensionsTests
{
    public class LeftTests
    {
        [Fact]
        public void Left_Throws_If_Argument_Is_Null()
        {
            TestHelper.AssertThrowsWhenArgumentNull(() => "abcde".Left(2));
        }

        [Fact]
        public void Left_Throws_If_Count_Is_Greater_Than_Length()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => "hello".Left(10));
        }

        [Fact]
        public void Left_Returns_Empty_String_If_Count_Is_Zero()
        {
            const string s = "hello";
            string expected = String.Empty;
            string actual = s.Left(0);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Left_Returns_Original_String_If_Count_Equals_Length()
        {
            const string s = "hello";
            const string expected = s;
            string actual = s.Left(s.Length);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Left_Returns_Leftmost_Characters()
        {
            const string s = "hello";
            const string expected = "he";
            string actual = s.Left(2);
            Assert.Equal(expected, actual);
        }
    }
}
