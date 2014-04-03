using NUnit.Framework;

namespace NString.Tests.StringExtensionsTests
{
    [TestFixture]
    public class TruncateTests
    {
        [Test]
        public void Truncate_Returns_Input_If_Not_Too_Long()
        {
            const string s = "hello";
            const string expected = "hello";
            string actual = s.Truncate(8);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Truncate_Returns_Input_Truncated_If_Too_Long()
        {
            const string s = "hello world";
            const string expected = "hello wo";
            string actual = s.Truncate(8);
            Assert.AreEqual(expected, actual);
        }
    }
}
