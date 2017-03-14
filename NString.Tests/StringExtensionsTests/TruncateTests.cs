using Xunit;

namespace NString.Tests.StringExtensionsTests
{
    public class TruncateTests
    {
        [Fact]
        public void Truncate_Returns_Input_If_Not_Too_Long()
        {
            const string s = "hello";
            const string expected = "hello";
            string actual = s.Truncate(8);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Truncate_Returns_Input_Truncated_If_Too_Long()
        {
            const string s = "hello world";
            const string expected = "hello wo";
            string actual = s.Truncate(8);
            Assert.Equal(expected, actual);
        }
    }
}
