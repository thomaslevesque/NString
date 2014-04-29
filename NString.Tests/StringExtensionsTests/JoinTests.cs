using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NString.Tests.StringExtensionsTests
{
    [TestFixture]
    public class JoinTests
    {
        [Test]
        public void Join_Throws_If_Values_Is_Null()
        {
            const IEnumerable<string> values = null;
            // ReSharper disable once AssignNullToNotNullAttribute
            ExceptionAssert.Throws<ArgumentNullException>(() => values.Join(" "));
        }

        [Test]
        public void Join_Concatenates_Values_If_Separator_Is_Null()
        {
            var values = new[] { "Hello", "World", "!" };
            string expected = String.Concat(values);
            string actual = values.Join();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Join_Concatenates_Values_With_Separator()
        {
            var values = new[] { "Hello", "World", "!" };
            string expected = String.Join(" ", values);
            string actual = values.Join(" ");
            Assert.AreEqual(expected, actual);
        }
    }
}
