using System;
using Xunit;

namespace NString.Tests.StringExtensionsTests
{
    public class RightTests
    {
        [Fact]
        public void Right_Throws_If_String_Is_Null()
        {
            const string s = null;
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => s.Right(2));
        }

        [Fact]
        public void Right_Throws_If_Count_Is_Greater_Than_Length()
        {
            const string s = "hello";
            Assert.Throws<ArgumentOutOfRangeException>(() => s.Right(10));
        }

        [Fact]
        public void Right_Returns_Empty_String_If_Count_Is_Zero()
        {
            const string s = "hello";
            string expected = String.Empty;
            string actual = s.Right(0);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Right_Returns_Original_String_If_Count_Equals_Length()
        {
            const string s = "hello";
            const string expected = s;
            string actual = s.Right(s.Length);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Right_Returns_Rightmost_Characters()
        {
            const string s = "hello";
            const string expected = "lo";
            string actual = s.Right(2);
            Assert.Equal(expected, actual);
        }


    }
}
