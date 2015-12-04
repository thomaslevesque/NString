using System;
using NUnit.Framework;

namespace NString.Tests.StringExtensionsTests
{
    [TestFixture]
    public class ReplaceAtTests
    {
        [Test]
        public void ReplaceAt_Throws_If_Input_Is_Null()
        {
            const string input = null;
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            ExceptionAssert.Throws<ArgumentNullException>(() => input.ReplaceAt(0, ' '));
        }

        [Test]
        [TestCase("foo", -1)]
        [TestCase("foo", 3)]
        public void ReplaceAt_Throws_If_Index_Out_Of_Range(string input, int index)
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => input.ReplaceAt(index, ' '));
        }


        [Test]
        public void ReplaceAt_Replaces_Specified_Character()
        {
            const string s = "hello world";
            const string expected = "hello_world";
            string actual = s.ReplaceAt(5, '_');
            Assert.AreEqual(expected, actual);
        }

    }
}
