using System;
using NUnit.Framework;

namespace NString.Tests.StringExtensionsTests
{
    [TestFixture]
    public class LeftTests
    {
        [Test]
        public void Left_Throws_If_String_Is_Null()
        {
            const string s = null;
            ExceptionAssert.Throws<ArgumentNullException>(() => s.Left(2));
        }

        [Test]
        public void Left_Throws_If_Count_Is_Greater_Than_Length()
        {
            const string s = "hello";
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => s.Left(10));
        }

        [Test]
        public void Left_Returns_Empty_String_If_Count_Is_Zero()
        {
            const string s = "hello";
            string expected = String.Empty;
            string actual = s.Left(0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Left_Returns_Original_String_If_Count_Equals_Length()
        {
            const string s = "hello";
            const string expected = s;
            string actual = s.Left(s.Length);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Left_Returns_Leftmost_Characters()
        {
            const string s = "hello";
            const string expected = "he";
            string actual = s.Left(2);
            Assert.AreEqual(expected, actual);
        }
    }
}
