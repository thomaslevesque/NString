using System;
using NUnit.Framework;

namespace NString.Tests.StringExtensionsTests
{
    [TestFixture]
    public class RightTests
    {
        [Test]
        public void Right_Throws_If_String_Is_Null()
        {
            const string s = null;
            ExceptionAssert.Throws<ArgumentNullException>(() => s.Right(2));
        }

        [Test]
        public void Right_Throws_If_Count_Is_Greater_Than_Length()
        {
            const string s = "hello";
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => s.Right(10));
        }

        [Test]
        public void Right_Returns_Empty_String_If_Count_Is_Zero()
        {
            const string s = "hello";
            string expected = String.Empty;
            string actual = s.Right(0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Right_Returns_Original_String_If_Count_Equals_Length()
        {
            const string s = "hello";
            const string expected = s;
            string actual = s.Right(s.Length);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Right_Returns_Rightmost_Characters()
        {
            const string s = "hello";
            const string expected = "lo";
            string actual = s.Right(2);
            Assert.AreEqual(expected, actual);
        }


    }
}
