using System;
using NUnit.Framework;

namespace NString.Tests.StringExtensionsTests
{
    [TestFixture]
    public class IsValidEmailTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsValidEmail_Throws_If_Input_Is_Null()
        {
            const string input = null;
            // ReSharper disable once AssignNullToNotNullAttribute
            input.IsValidEmail();
        }

        [Test]
        [TestCase("test@test.com", true)]
        [TestCase("test.test@test.com", true)]
        [TestCase("test", false)]
        [TestCase("test@test", false)]
        [TestCase("test@test@test.com", false)]
        public void IsValidEmail_Returns_True_If_Email_Is_Valid(string input, bool expected)
        {
            Assert.AreEqual(expected, input.IsValidEmail());
        }
    }
}
