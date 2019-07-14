using System.Globalization;
using System.Linq;
using Xunit;
// ReSharper disable InconsistentNaming
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
            var values = new PropertyOrFieldValues { ConcatenatedStringCollectionProperty = StringCollectionValues };

            string actual = StringTemplate.Format("{ConcatenatedStringCollectionProperty}", values);

            Assert.Equal(ExpectedStringCollectionConcatenation, actual);
        }

        [Fact]
        public void StringTemplateValueConverter_Can_Convert_Field()
        {
            var values = new PropertyOrFieldValues { ConcatenatedStringCollectionField = StringCollectionValues };

            string actual = StringTemplate.Format("{ConcatenatedStringCollectionField}", values);

            Assert.Equal(ExpectedStringCollectionConcatenation, actual);
        }

        [Fact]
        public void StringTemplateValueConverter_Can_Convert_Reference_Type()
        {
            var values = new ReferenceOrValueTypeValues { ConcatenatedStringCollection = StringCollectionValues };

            string actual = StringTemplate.Format("{ConcatenatedStringCollection}", values);

            Assert.Equal(ExpectedStringCollectionConcatenation, actual);
        }

        [Fact]
        public void StringTemplateValueConverter_Can_Convert_Value_Type()
        {
            var values = new ReferenceOrValueTypeValues { DateTimeToTicks = new DateTime(ticks: 42) };

            string actual = StringTemplate.Format("{DateTimeToTicks}", values);

            Assert.Equal("42", actual);
        }

        [Fact]
        public void StringTemplateValueConverter_Works_With_FormatProvider()
        {
            var values = new ValuesForFormatProvider { ConvertedWithFortyTwoDaysAdded = DateTime.Today };
            var expectedConvertedDate = values.ConvertedWithFortyTwoDaysAdded + TimeSpan.FromDays(42);

            var cultures = new[] { CultureInfo.InvariantCulture, null, new CultureInfo("fr-FR") };

            foreach (var culture in cultures)
            {
                string expected = expectedConvertedDate.ToString("D", culture);
                string actual = StringTemplate.Format("{ConvertedWithFortyTwoDaysAdded:D}", values, true, culture);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void StringTemplateValueConverter_Works_With_FormatProvider_And_Return_Type_Change()
        {
            var ticks = 0xDEADBEEF;
            var values = new ValuesForFormatProvider { ConvertedToTicks = new DateTime(ticks) };
            var expectedHexTicks = $"{ticks:X}";

            var actual = StringTemplate.Format("{ConvertedToTicks:X}", values, true);
            Assert.Equal(expectedHexTicks, actual);
        }

        [Fact]
        public void StringTemplateValueConverter_Works_With_Alignment_In_PlaceHolder()
        {
            var values = new ValuesForFormatProvider
            {
                ConvertedToTicks = new DateTime(42),
                ConvertedToDayName = DateTime.Today
            };

            string expected = $"{values.ConvertedToDayName.DayOfWeek,-30}: {values.ConvertedToTicks.Ticks,30}";
            string actual = StringTemplate.Format("{ConvertedToDayName,-30}: {ConvertedToTicks,30}", values);

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
        public void StringTemplateValueConverter_Caches_And_Reuse_Converters()
        {
            var values = new MiscConvertersValues { State_Mutator_Converter = 0 };
            var otherValues = new SecondMiscConvertersValues { State_Mutator_Converter2 = 0 };

            string actual1 = StringTemplate.Format("{State_Mutator_Converter}", values);
            string actual2 = StringTemplate.Format("{State_Mutator_Converter}", values);
            string actual3 = StringTemplate.Format("{State_Mutator_Converter}", values);
            string actual4 = StringTemplate.Format("{State_Mutator_Converter2}", otherValues);

            // The converter instance should be reused, so its state is preserved and incremented between calls
            Assert.Equal("1", actual1);
            Assert.Equal("2", actual2);
            Assert.Equal("3", actual3);
            Assert.Equal("4", actual4); // tests caching across types and members
        }

        [Fact]
        public void Should_Throw_When_Attribute_Has_Null_Converter_Type()
        {
            var values = new MiscConvertersValues { Attribute_With_Null_Converter_Type = 0 };

            Assert.Throws<InvalidOperationException>(() => StringTemplate.Format("{Attribute_With_Null_Converter_Type}", values));
        }

        [Fact]
        public void Should_Throw_When_Converter_CanConvert_Is_False()
        {
            var values = new MiscConvertersValues { Converter_CanConvert_Always_False = 0 };

            Assert.Throws<NotSupportedException>(() => StringTemplate.Format("{Converter_CanConvert_Always_False}", values));
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", false)]
        [InlineData("hello", false)]
        public void Generic_Converter_Allows_Null_Values(object input, bool expectedResult)
        {
            var values = new MiscConvertersValues { NullToBool = input };

            string actual = StringTemplate.Format("{NullToBool}", values);

            Assert.Equal(expectedResult, bool.Parse(actual));
        }

        [Fact]
        public void Generic_Converter_CanConvert_Allows_Derived_Type()
        {
            var values = new MiscConvertersValues { String_Array_With_ReadOnlyCollection_Converter = StringCollectionValues };

            string actual = StringTemplate.Format("{String_Array_With_ReadOnlyCollection_Converter}", values);

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
            public override object Convert(object value) => value == null;
        }

        class IncrementStateConverter : IStringTemplateValueConverter
        {
            public int State;
            public bool CanConvert(Type objectType) => true;

            public object Convert(object value) => Interlocked.Increment(ref State);
        }

        class CanNeverConvertConverter : IStringTemplateValueConverter
        {
            public bool CanConvert(Type objectType) => false;

            public object Convert(object value) => throw new NotSupportedException();
        }

        class PropertyOrFieldValues
        {
            [StringTemplateValueConverter(typeof(StringCollectionConverter))]
            public IReadOnlyCollection<string> ConcatenatedStringCollectionProperty { get; set; }

            [StringTemplateValueConverter(typeof(StringCollectionConverter))]
            public IReadOnlyCollection<string> ConcatenatedStringCollectionField;
        }

        class ReferenceOrValueTypeValues
        {
            [StringTemplateValueConverter(typeof(StringCollectionConverter))]
            public IReadOnlyCollection<string> ConcatenatedStringCollection { get; set; }

            [StringTemplateValueConverter(typeof(DateTimeToTicksConverter))]
            public DateTime DateTimeToTicks { get; set; }
        }

        class ValuesForFormatProvider
        {
            [StringTemplateValueConverter(typeof(AddFortyTwoDaysConverter))]
            public DateTime ConvertedWithFortyTwoDaysAdded { get; set; }

            [StringTemplateValueConverter(typeof(DateTimeToTicksConverter))]
            public DateTime ConvertedToTicks { get; set; }

            [StringTemplateValueConverter(typeof(GetDayNameConverter))]
            public DateTime ConvertedToDayName { get; set; }
        }

        class MiscConvertersValues
        {
            [StringTemplateValueConverter(null)]
            public int Attribute_With_Null_Converter_Type { get; set; }

            [StringTemplateValueConverter(typeof(CanNeverConvertConverter))]
            public int Converter_CanConvert_Always_False { get; set; }

            [StringTemplateValueConverter(typeof(IncrementStateConverter))]
            public int State_Mutator_Converter { get; set; }

            [StringTemplateValueConverter(typeof(StringCollectionConverter))]
            public string[] String_Array_With_ReadOnlyCollection_Converter { get; set; }

            [StringTemplateValueConverter(typeof(NullToBoolConverter))]
            public object NullToBool { get; set; }
        }

        class SecondMiscConvertersValues
        {
            [StringTemplateValueConverter(typeof(IncrementStateConverter))]
            public int State_Mutator_Converter2 { get; set; }
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
                public override object Convert(DayOfWeek value)
                {
                    switch (value)
                    {
                        case DayOfWeek.Sunday:
                        case DayOfWeek.Saturday:
                            return "Week-end! :)";
                        default:
                            return $"{value} + :(";
                    }
                }
            }
        }
    }
}
