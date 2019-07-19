using System.Globalization;
using Xunit;
// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;

namespace NString.Tests
{
    public class StringTemplateTests
    {
        [Fact]
        public void Check_Format()
        {
            string name = "FooBar";
            DateTime date = DateTime.Now;

            string format = "Bonjour {0}. Nous sommes le {1:D}, et il est {1:T}.";
            string templateFormat = "Bonjour {Name}. Nous sommes le {Date:D}, et il est {Date:T}.";

            var values1 = new Dictionary<string, object>
            {
                { "Name", name},
                { "Date", date }
            };

            var values2 = new
            {
                Name = name,
                Date = date
            };

            string expected = String.Format(format, name, date);
            string result1 = StringTemplate.Format(templateFormat, values1);
            string result2 = StringTemplate.Format(templateFormat, values2);

            Assert.Equal(expected, result1);
            Assert.Equal(expected, result2);
        }

        [Fact]
        public void Should_Raise_Exception_When_Called_With_Null_Template()
        {
            Assert.Throws<ArgumentNullException>(
                // ReSharper disable once AssignNullToNotNullAttribute
                () => StringTemplate.Format(null, ""));
        }

        [Fact]
        public void Should_Raise_Exception_When_Called_With_Null_Values()
        {
            Assert.Throws<ArgumentNullException>(
                // ReSharper disable once AssignNullToNotNullAttribute
                () => StringTemplate.Format("Bonjour {0}.", null));
        }

        [Fact]
        public void Format_Should_Throw_When_No_Matching_Value_Exists()
        {
            string format = "Bonjour {Name}";

            var values1 = new Dictionary<string, object>
            {
                { "Nam", "numero 6"},
            };

            Assert.Throws<KeyNotFoundException>(() => StringTemplate.Format(format, values1));
            Assert.Equal(StringTemplate.Format(format, values1, false), format);

            var values2 = new
            {
                Nam = "numero 6"
            };

            Assert.Throws<KeyNotFoundException>(() => StringTemplate.Format(format, values2));
            Assert.Equal(StringTemplate.Format(format, values2, false), format);
        }

        [Fact]
        public void ToString_Should_Return_Template()
        {
            StringTemplate testStringTemplate = StringTemplate.Parse("test");
            Assert.Equal("test", testStringTemplate.ToString());
        }

        [Fact]
        public void Implicit_Operator()
        {
            StringTemplate testStringTemplate = "test";
            Assert.Equal("test", testStringTemplate.ToString());
        }

        [Fact]
        public void Test_BracesAreEscaped()
        {
            var values1 = new object[] { "World" };
            var values2 = new { Name = "World" };

            string s = StringTemplate.Format("Hello {{Name}} !", values2);
            Assert.Equal("Hello {Name} !", s);

            var s1 = String.Format("Hello {{{0}}} !", values1);
            var s2 = StringTemplate.Format("Hello {{{Name}}} !", values2);
            Assert.Equal(s1, s2);

            s1 = String.Format("Hello {{{0} !", values1);
            s2 = StringTemplate.Format("Hello {{{Name} !", values2);
            Assert.Equal(s1, s2);

            s1 = String.Format("Hello {0}}} !", values1);
            s2 = StringTemplate.Format("Hello {Name}}} !", values2);
            Assert.Equal(s1, s2);

            Assert.Throws<FormatException>(
                () => s2 = StringTemplate.Format("Hello {{{Name}} !", values2));
        }

        [Fact]
        public void Test_WithFields()
        {
            int x = 42;
            string text = "Hello world";

            string expected = $"{text} {x}";
            string actual = StringTemplate.Format("{text} {x}", new FieldValues { x = x, text = text });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_HiddenMembers()
        {
            DerivedValues values = new DerivedValues { X = "hello", Y = 42 };
            BaseValues bValues = values;
            bValues.X = 42;

            string expected = $"{values.X} {values.Y}";
            string actual = StringTemplate.Format("{X} {Y}", values);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_CachedTemplate()
        {
            var t1 = StringTemplate.Parse("{X}");
            var t2 = StringTemplate.Parse("{X}");

            Assert.Same(t1, t2);
        }

        [Fact]
        public void Test_ClearCached()
        {
            var t1 = StringTemplate.Parse("{X}");
            StringTemplate.ClearCache();
            var t2 = StringTemplate.Parse("{X}");

            Assert.NotSame(t1, t2);
        }

        [Fact]
        public void Test_WithFormatProvider()
        {
            var date = DateTime.Today;

            var cultures = new[] { CultureInfo.InvariantCulture, null, new CultureInfo("fr-FR") };

            foreach (var culture in cultures)
            {
                string expected = date.ToString("D", culture);
                string actual = StringTemplate.Format("{date:D}", new { date }, true, culture);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void Format_Should_Accept_Alignment_In_PlaceHolder()
        {
            var x = new
            {
                Name = "FooBar",
                Value = 42
            };

            string expected = $"{x.Name,-10}: {x.Value,10}";
            string actual = StringTemplate.Format("{Name,-10}: {Value,10}", x);

            Assert.Equal(expected, actual);
        }

        // ReSharper disable NotAccessedField.Local
        #pragma warning disable 414
        class FieldValues
        {
            public int x;
            public string text;
        }
        #pragma warning restore 414
        // ReSharper restore NotAccessedField.Local


        class BaseValues
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int X { get; set; }
            public int Y { get; set; }
        }

        class DerivedValues : BaseValues
        {
            public new string X;
#pragma warning disable 169
            private new string Y;
#pragma warning restore 169
        }
    }
}
