using System;
using System.Text;
using NUnit.Framework;

namespace NString.Tests.StringExtensionsTests
{
    [TestFixture]
    public class ReverseTests
    {
        [Test]
        public void Reverse_Throws_If_String_Is_Null()
        {
            const string s = null;
            // ReSharper disable once AssignNullToNotNullAttribute
            ExceptionAssert.Throws<ArgumentNullException>(() => s.Reverse());
        }

        [Test]
        public void Reverse_Empty_String_Returns_Empty_String()
        {
            string s = string.Empty;
            string expected = string.Empty;
            string actual = s.Reverse();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Reverse_String_With_ASCII_Chars_Returns_Reversed_String()
        {
            const string s = "hello world";
            const string expected = "dlrow olleh";
            string actual = s.Reverse();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Reverse_FormC_String_With_Accented_Char_Returns_Reversed_String()
        {
            string s = "Les Misérables".Normalize(NormalizationForm.FormC);
            string expected = "selbarésiM seL".Normalize(NormalizationForm.FormC);
            string actual = s.Reverse();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Reverse_FormD_String_With_Accented_Char_Returns_Reversed_String()
        {
            string s = "Les Misérables".Normalize(NormalizationForm.FormD);
            string expected = "selbarésiM seL".Normalize(NormalizationForm.FormD);
            string actual = s.Reverse();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Reverse_String_With_Emoji_Returns_Reversed_String()
        {
            const string s = "Hi there 😎 !";
            const string expected = "! 😎 ereht iH";
            string actual = s.Reverse();
            Assert.AreEqual(expected, actual);
        }
    }
}
