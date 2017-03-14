using System;
using Xunit;

namespace NString.Tests.StringExtensionsTests
{
        public class LeftTests
    {
        [Fact]
        public void Left_Throws_If_String_Is_Null()
        {
            const string s = null;
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => s.Left(2));
        }

        [Fact]
        public void Left_Throws_If_Count_Is_Greater_Than_Length()
        {
            const string s = "hello";
            Assert.Throws<ArgumentOutOfRangeException>(() => s.Left(10));
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
