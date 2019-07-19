using System;
using System.Linq;
using Xunit;

namespace NString.Tests.StringExtensionsTests
{
        public class GetLinesTests
    {
        [Fact]
        public void GetLines_Throws_If_String_Is_Null()
        {
            const string s = null;
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => s.GetLines());
        }

        [Fact]
        public void GetLines_Returns_All_Lines_In_String()
        {
            const string input = @"Yesterday
All my troubles seemed so far away
Now it looks as though they're here to stay
Oh I believe in yesterday";

            var expected = new[]
            {
                "Yesterday",
                "All my troubles seemed so far away",
                "Now it looks as though they're here to stay",
                "Oh I believe in yesterday"
            };

            var actual = input.GetLines().ToArray();

            Assert.Equal(expected, actual);
        }
    }
}
