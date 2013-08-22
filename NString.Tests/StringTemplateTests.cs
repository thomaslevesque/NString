using System.Globalization;
// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NString.Tests
{
    [TestClass]
    public class StringTemplateTests
    {
        [TestMethod]
        public void Check_Format()
        {
            string name = "Zobi la mouche";
            DateTime date = DateTime.Now;

            string format = "Bonjour {0}. Nous sommes le {1:D}, et il est {1:T}.";
            string templateFormat = "Bonjour {Name}. Nous sommes le {Date:D}, et il est {Date:T}.";

            var values1 = new Dictionary<string, object>
            {
                { "Name", "Zobi la mouche"},
                { "Date", date }
            };

            var values2 = new
            {
                Name = name,
                Date = date
            };

            string result0 = String.Format(format, name, date);
            string result1 = StringTemplate.Format(templateFormat, values1);
            string result2 = StringTemplate.Format(templateFormat, values2);

            Assert.AreEqual(result0, result1);
            Assert.AreEqual(result0, result2);
        }

        [TestMethod]
        public void Should_Raise_Exception_When_Called_With_Null_Template()
        {
            ExceptionAssert.Throws<ArgumentNullException>(
                () => StringTemplate.Format(null, ""));
        }

        [TestMethod]
        public void Should_Raise_Exception_When_Called_With_Null_Values()
        {
            ExceptionAssert.Throws<ArgumentNullException>(
                () => StringTemplate.Format("Bonjour {0}.", null));
        }

        [TestMethod]
        public void Format_Should_Throw_When_No_Matching_Value_Exists()
        {
            string format = "Bonjour {Name}";

            var values1 = new Dictionary<string, object>
            {
                { "Nam", "numero 6"},
            };

            ExceptionAssert.Throws<KeyNotFoundException>(() => StringTemplate.Format(format, values1));
            Assert.AreEqual(StringTemplate.Format(format, values1, false), format);

            var values2 = new
            {
                Nam = "numero 6"
            };

            ExceptionAssert.Throws<KeyNotFoundException>(() => StringTemplate.Format(format, values2));
            Assert.AreEqual(StringTemplate.Format(format, values2, false), format);
        }

        [TestMethod]
        public void ToString_Should_Return_Template()
        {
            StringTemplate testStringTemplate = StringTemplate.Parse("test");
            Assert.AreEqual("test", testStringTemplate.ToString());
        }

        [TestMethod]
        public void Implicit_Operator()
        {
            StringTemplate testStringTemplate = "test";
            Assert.AreEqual("test", testStringTemplate.ToString());
        }

        [TestMethod]
        public void Test_BracesAreEscaped()
        {
            var values1 = new object[] { "World" };
            var values2 = new { Name = "World" };

            string s = StringTemplate.Format("Hello {{Name}} !", values2);
            Assert.AreEqual("Hello {Name} !", s);

            string s1, s2;
            s1 = String.Format("Hello {{{0}}} !", values1);
            s2 = StringTemplate.Format("Hello {{{Name}}} !", values2);
            Assert.AreEqual(s1, s2);

            s1 = String.Format("Hello {{{0} !", values1);
            s2 = StringTemplate.Format("Hello {{{Name} !", values2);
            Assert.AreEqual(s1, s2);

            s1 = String.Format("Hello {0}}} !", values1);
            s2 = StringTemplate.Format("Hello {Name}}} !", values2);
            Assert.AreEqual(s1, s2);

            ExceptionAssert.Throws<FormatException>(
                () => s2 = StringTemplate.Format("Hello {{{Name}} !", values2));
        }

        [TestMethod]
        public void Test_WithFields()
        {
            int x = 42;
            string text = "Hello world";

            string expected = string.Format("{0} {1}", text, x);
            string actual = StringTemplate.Format("{text} {x}", new FieldValues { x = x, text = text });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test_HiddenMembers()
        {
            DerivedValues values = new DerivedValues { X = "hello", Y = 42 };
            BaseValues bValues = values;
            bValues.X = 42;

            string expected = string.Format("{0} {1}", values.X, values.Y);
            string actual = StringTemplate.Format("{X} {Y}", values);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test_CachedTemplate()
        {
            var t1 = StringTemplate.Parse("{X}");
            var t2 = StringTemplate.Parse("{X}");

            Assert.AreSame(t1, t2);
        }

        [TestMethod]
        public void Test_WithFormatProvider()
        {
            var date = DateTime.Today;

            var cultures = new[] { CultureInfo.InvariantCulture, null, CultureInfo.GetCultureInfo("fr-FR") };

            foreach (var culture in cultures)
            {
                string expected = date.ToString("D", culture);
                string actual = StringTemplate.Format("{date:D}", new { date }, true, culture);
                Assert.AreEqual(expected, actual);
            }
        }

        // ReSharper disable NotAccessedField.Local
        class FieldValues
        {
            public int x;
            public string text;
        }
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
