using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NString.Tests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void IsNullOrEmpty_Returns_True_If_String_Is_Null()
        {
            const string s = null;
            const bool expected = true;
            bool actual = s.IsNullOrEmpty();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsNullOrEmpty_Returns_True_If_String_Is_Empty()
        {
            string s = string.Empty;
            const bool expected = true;
            bool actual = s.IsNullOrEmpty();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsNullOrEmpty_Returns_False_If_String_Is_Not_Empty()
        {
            string s = "hello";
            const bool expected = false;
            bool actual = s.IsNullOrEmpty();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsNullOrWhiteSpace_Returns_True_If_String_Is_Null()
        {
            const string s = null;
            const bool expected = true;
            bool actual = s.IsNullOrWhiteSpace();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsNullOrWhiteSpace_Returns_True_If_String_Is_Empty()
        {
            string s = string.Empty;
            const bool expected = true;
            bool actual = s.IsNullOrWhiteSpace();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsNullOrWhiteSpace_Returns_True_If_String_Is_WhiteSpace()
        {
            string s = "  \t ";
            const bool expected = true;
            bool actual = s.IsNullOrWhiteSpace();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsNullOrWhiteSpace_Returns_False_If_String_Has_NonWhitespace_Chars()
        {
            string s = "hello";
            const bool expected = false;
            bool actual = s.IsNullOrWhiteSpace();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Join_Throws_If_Values_Is_Null()
        {
            const IEnumerable<string> values = null;
            ExceptionAssert.Throws<ArgumentNullException>(() => values.Join(" "));
        }

        [TestMethod]
        public void Join_Concatenates_Values_If_Separator_Is_Null()
        {
            var values = new[] { "Hello", "World", "!" };
            string expected = String.Concat(values);
            string actual = values.Join();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Join_Concatenates_Values_With_Separator()
        {
            var values = new[] { "Hello", "World", "!" };
            string expected = String.Join(" ", values);
            string actual = values.Join(" ");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetLines_Throws_If_String_Is_Null()
        {
            const string s = null;
            ExceptionAssert.Throws<ArgumentNullException>(() => s.GetLines());
        }

        [TestMethod]
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

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Left_Throws_If_String_Is_Null()
        {
            const string s = null;
            ExceptionAssert.Throws<ArgumentNullException>(() => s.Left(2));
        }

        [TestMethod]
        public void Left_Throws_If_Count_Is_Greater_Than_Length()
        {
            const string s = "hello";
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => s.Left(10));
        }

        [TestMethod]
        public void Left_Returns_Empty_String_If_Count_Is_Zero()
        {
            const string s = "hello";
            string expected = String.Empty;
            string actual = s.Left(0);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Left_Returns_Original_String_If_Count_Equals_Length()
        {
            const string s = "hello";
            const string expected = s;
            string actual = s.Left(s.Length);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Left_Returns_Leftmost_Characters()
        {
            const string s = "hello";
            const string expected = "he";
            string actual = s.Left(2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Right_Throws_If_String_Is_Null()
        {
            const string s = null;
            ExceptionAssert.Throws<ArgumentNullException>(() => s.Right(2));
        }

        [TestMethod]
        public void Right_Throws_If_Count_Is_Greater_Than_Length()
        {
            const string s = "hello";
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => s.Right(10));
        }

        [TestMethod]
        public void Right_Returns_Empty_String_If_Count_Is_Zero()
        {
            const string s = "hello";
            string expected = String.Empty;
            string actual = s.Right(0);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Right_Returns_Original_String_If_Count_Equals_Length()
        {
            const string s = "hello";
            const string expected = s;
            string actual = s.Right(s.Length);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Right_Returns_Rightmost_Characters()
        {
            const string s = "hello";
            const string expected = "lo";
            string actual = s.Right(2);
            Assert.AreEqual(expected, actual);
        }
    }
}
