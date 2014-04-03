using System;
using NUnit.Framework;

namespace NString.Tests.StringExtensionsTests
{
    [TestFixture]
    public class EllipsisTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ellipsis_Throws_If_Input_Is_Null()
        {
            const string s = null;
            s.Ellipsis(10);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ellipsis_With_Custom_Ellipsis_String_Throws_If_Input_Is_Null()
        {
            const string s = null;
            s.Ellipsis(10, "-----");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ellipsis_Throws_If_Custom_Ellipsis_String_Is_Null()
        {
            const string s = "hello";
            s.Ellipsis(10, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ellipsis_Throws_If_Max_Length_Is_Less_Than_Length_Of_Ellipsis_String()
        {
            const string s = "hello";
            s.Ellipsis(2);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ellipsis_Throws_If_Max_Length_Less_Than_Length_Of_Custom_Ellipsis_String()
        {
            const string s = "hello";
            s.Ellipsis(4, "-----");
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ellipsis_Throws_If_Max_Length_Is_Less_Than_Zero()
        {
            const string s = "hello";
            s.Ellipsis(-1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ellipsis_With_Custom_Ellipsis_String_Throws_If_Max_Length_Is_Less_Than_Zero()
        {
            const string s = "hello";
            s.Ellipsis(-1, "-----");
        }

        [Test]
        public void Ellipsis_Returns_Input_If_Not_Too_Long()
        {
            const string s = "hello";
            const string expected = "hello";
            string actual = s.Ellipsis(8);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Ellipsis_Returns_Input_Truncated_With_Ellipsis_If_Too_Long()
        {
            const string s = "hello world";
            const string expected = "hello...";
            string actual = s.Ellipsis(8);
            Assert.AreEqual(expected, actual);
        }

    }
}
