using System.Globalization;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading;

namespace NString.Tests
{
    public class StringTemplateConverterTests
    {
        private static readonly string[] StringCollectionValues = { "1", "2", "3" };
        private const string ExpectedStringCollectionConcatenation = "1|2|3";

        [Fact]
        public void StringTemplateValueConverter_Can_Convert_Property()
        {
            var values = new BasicTestValues { StringCollectionProperty = StringCollectionValues };

            string actual = StringTemplate.Format("{StringCollectionProperty}", values);

            Assert.Equal(ExpectedStringCollectionConcatenation, actual);
        }

        [Fact]
        public void StringTemplateValueConverter_Can_Convert_Field()
        {
            var values = new BasicTestValues { StringCollectionField = StringCollectionValues };

            string actual = StringTemplate.Format("{StringCollectionField}", values);

            Assert.Equal(ExpectedStringCollectionConcatenation, actual);
        }

        [Fact]
        public void StringTemplateValueConverter_Can_Convert_Value_Type()
        {
            var values = new BasicTestValues { ValueTypeProperty = new DateTime(ticks: 42) };

            string actual = StringTemplate.Format("{ValueTypeProperty}", values);

            Assert.Equal("42", actual);
        }

        [Fact]
        public void StringTemplateValueConverter_Works_With_FormatProvider()
        {
            var values = new ValuesForFormatProvider { DateTimeConvertedWithFortyTwoDaysAdded = DateTime.Today };
            var expectedConvertedDate = values.DateTimeConvertedWithFortyTwoDaysAdded + TimeSpan.FromDays(42);

            var cultures = new[] { CultureInfo.InvariantCulture, null, new CultureInfo("fr-FR") };

            foreach (var culture in cultures)
            {
                string expected = expectedConvertedDate.ToString("D", culture);
                string actual = StringTemplate.Format("{DateTimeConvertedWithFortyTwoDaysAdded:D}", values, true, culture);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void StringTemplateValueConverter_Works_With_FormatProvider_And_Return_Type_Change()
        {
            var ticks = 1337;
            var values = new ValuesForFormatProvider { DateTimeConvertedToTicks = new DateTime(ticks) };
            var formatProvider = new NumberFormatInfo { CurrencySymbol = "piasses", CurrencyPositivePattern = 3 /* symbol at the end, with a space */ };
            var expectedFormattedTicks = string.Format(formatProvider, "{0:C0}", ticks);

            var actual = StringTemplate.Format("{DateTimeConvertedToTicks:C0}", values, true, formatProvider);
            Assert.Equal(expectedFormattedTicks, actual);
        }

        [Fact]
        public void StringTemplateValueConverter_Works_With_Alignment_In_PlaceHolder()
        {
            var values = new ValuesForFormatProvider
            {
                DateTimeConvertedToTicks = new DateTime(42),
                DateTimeConvertedToDayName = DateTime.Today
            };

            string expected = $"{values.DateTimeConvertedToDayName.DayOfWeek,-30}: {values.DateTimeConvertedToTicks.Ticks,30}";
            string actual = StringTemplate.Format("{DateTimeConvertedToDayName,-30}: {DateTimeConvertedToTicks,30}", values);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringTemplateValueConverter_Works_With_Enum()
        {
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                var values = new EnumValues { Day = day };

                string expected = EnumValues.GetCustomDayString(day);
                string actual = StringTemplate.Format("{Day}", values);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void StringTemplateValueConverter_Caches_And_Reuses_Converters()
        {
            StringTemplate.ClearCache(); // Make sure we don't use instances from other tests...

            var values = new ValuesWithMiscConverters { AttributedWithStateMutatorConverter = 0 };
            var otherValues = new OtherValuesWithMiscConverters { AttributedWithStateMutatorConverter2 = 0 };

            string actual1 = StringTemplate.Format("{AttributedWithStateMutatorConverter}", values);
            string actual2 = StringTemplate.Format("{AttributedWithStateMutatorConverter}", values);
            string actual3 = StringTemplate.Format("{AttributedWithStateMutatorConverter}", values);
            string actual4 = StringTemplate.Format("{AttributedWithStateMutatorConverter2}", otherValues);

            // The converter instance should be reused, so its state is preserved and incremented between calls
            Assert.Equal("1", actual1);
            Assert.Equal("2", actual2);
            Assert.Equal("3", actual3);
            Assert.Equal("4", actual4); // tests caching across types and members
        }

        [Fact]
        public void StringTemplateValueConverter_Caches_And_Reuses_Converters_And_Behaves_Properly_When_Caches_Are_Cleared()
        {
            StringTemplate.ClearCache(); // Make sure we don't use instances from other tests...

            var values = new ValuesWithMiscConverters { AttributedWithStateMutatorConverter = 0 };
            var otherValues = new OtherValuesWithMiscConverters { AttributedWithStateMutatorConverter2 = 0 };

            string actual1 = StringTemplate.Format("{AttributedWithStateMutatorConverter}", values);
            string actual2 = StringTemplate.Format("{AttributedWithStateMutatorConverter}", values);

            // Clear the cached getters and converters.
            // This test will fail if the getters cache is cleared, but not the converters cache.
            StringTemplate.ClearCache();

            string actual3 = StringTemplate.Format("{AttributedWithStateMutatorConverter}", values);
            string actual4 = StringTemplate.Format("{AttributedWithStateMutatorConverter2}", otherValues);

            // The converter instance should be reused, so its state is preserved and incremented between calls
            Assert.Equal("1", actual1);
            Assert.Equal("2", actual2);
            Assert.Equal("1", actual3);
            Assert.Equal("2", actual4); // tests caching across types and members
        }

        [Fact]
        public void Should_Throw_When_Attribute_Has_Null_Converter_Type()
        {
            var values = new ValuesWithMiscConverters { AttributedWithNullConverterType = 0 };

            Assert.Throws<InvalidOperationException>(() => StringTemplate.Format("{AttributedWithNullConverterType}", values));
        }

        [Fact]
        public void Should_Throw_When_Converter_CanConvert_Is_False()
        {
            var values = new ValuesWithMiscConverters { AttributedWithConverterHavingCanConvertAlwaysFalse = 0 };

            Assert.Throws<NotSupportedException>(() => StringTemplate.Format("{AttributedWithConverterHavingCanConvertAlwaysFalse}", values));
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", false)]
        [InlineData("hello", false)]
        public void Generic_Converter_Allows_Null_Values(object input, bool expectedResult)
        {
            var values = new ValuesWithMiscConverters { AttributedWithNullToBoolConverter = input };

            string actual = StringTemplate.Format("{AttributedWithNullToBoolConverter}", values);

            Assert.Equal(expectedResult, bool.Parse(actual));
        }

        [Fact]
        public void Generic_Converter_CanConvert_Allows_Derived_Type()
        {
            var values = new ValuesWithMiscConverters { StringArrayWithReadOnlyCollectionConverter = StringCollectionValues };

            string actual = StringTemplate.Format("{StringArrayWithReadOnlyCollectionConverter}", values);

            Assert.Equal(ExpectedStringCollectionConcatenation, actual);
        }

        [Fact]
        public void Generic_Converter_Throws_When_Given_Incompatible_Value()
        {
            IStringTemplateValueConverter converter = new StringCollectionConverter();

            var shouldWork = converter.Convert(StringCollectionValues);

            Assert.Equal(ExpectedStringCollectionConcatenation, shouldWork); // sanity check

            Assert.Throws<NotSupportedException>(() => converter.Convert("whatever"));
        }

        class StringCollectionConverter : StringTemplateValueConverter<IReadOnlyCollection<string>>
        {
            public override object Convert(IReadOnlyCollection<string> value) => string.Join("|", value);
        }

        class DateTimeToTicksConverter : StringTemplateValueConverter<DateTime>
        {
            public override object Convert(DateTime value) => value.Ticks;
        }

        class AddFortyTwoDaysConverter : StringTemplateValueConverter<DateTime>
        {
            public override object Convert(DateTime value) => value + TimeSpan.FromDays(42);
        }

        class GetDayNameConverter : StringTemplateValueConverter<DateTime>
        {
            public override object Convert(DateTime value) => value.DayOfWeek.ToString();
        }

        class NullToBoolConverter : StringTemplateValueConverter<object>
        {
            public override object Convert(object value) => value is null;
        }

        class IncrementStateConverter : IStringTemplateValueConverter
        {
            public int State;
            public bool CanConvert(Type objectType) => true;

            public object Convert(object? value) => Interlocked.Increment(ref State);
        }

        class CanNeverConvertConverter : IStringTemplateValueConverter
        {
            public bool CanConvert(Type objectType) => false;

            public object Convert(object? value) => throw new NotSupportedException();
        }

        class BasicTestValues
        {
            [StringTemplateValueConverter(typeof(StringCollectionConverter))]
            public IReadOnlyCollection<string>? StringCollectionProperty { get; set; }

            [StringTemplateValueConverter(typeof(StringCollectionConverter))]
            public IReadOnlyCollection<string>? StringCollectionField;

            [StringTemplateValueConverter(typeof(DateTimeToTicksConverter))]
            public DateTime ValueTypeProperty { get; set; }
        }

        class ValuesForFormatProvider
        {
            [StringTemplateValueConverter(typeof(AddFortyTwoDaysConverter))]
            public DateTime DateTimeConvertedWithFortyTwoDaysAdded { get; set; }

            [StringTemplateValueConverter(typeof(DateTimeToTicksConverter))]
            public DateTime DateTimeConvertedToTicks { get; set; }

            [StringTemplateValueConverter(typeof(GetDayNameConverter))]
            public DateTime DateTimeConvertedToDayName { get; set; }
        }

        class ValuesWithMiscConverters
        {
            [StringTemplateValueConverter(null!)]
            public int AttributedWithNullConverterType { get; set; }

            [StringTemplateValueConverter(typeof(CanNeverConvertConverter))]
            public int AttributedWithConverterHavingCanConvertAlwaysFalse { get; set; }

            [StringTemplateValueConverter(typeof(IncrementStateConverter))]
            public int AttributedWithStateMutatorConverter { get; set; }

            [StringTemplateValueConverter(typeof(StringCollectionConverter))]
            public string[]? StringArrayWithReadOnlyCollectionConverter { get; set; }

            [StringTemplateValueConverter(typeof(NullToBoolConverter))]
            public object? AttributedWithNullToBoolConverter { get; set; }
        }

        class OtherValuesWithMiscConverters
        {
            [StringTemplateValueConverter(typeof(IncrementStateConverter))]
            public int AttributedWithStateMutatorConverter2 { get; set; }
        }

        class EnumValues
        {
            [StringTemplateValueConverter(typeof(EnumValuesConverter))]
            public DayOfWeek Day { get; set; }

            public static string GetCustomDayString(DayOfWeek day)
            {
                switch (day)
                {
                    case DayOfWeek.Sunday:
                    case DayOfWeek.Saturday:
                        return "Week-end! :)";
                    default:
                        return $"{day} + :(";
                }
            }

            class EnumValuesConverter : StringTemplateValueConverter<DayOfWeek>
            {
                public override object Convert(DayOfWeek value) => EnumValues.GetCustomDayString(value);
            }
        }
    }
}
