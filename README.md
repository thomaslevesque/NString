# NString

A collection of utilities for working with strings in .NET.

This library is a Portable Class Library targeting .NET 4.5, Windows Phone 8 and Windows Store apps (8.0).

## StringTemplate

This class allows you to format values in a way similar to `String.Format`, but with named placeholders instead of numbered placeholders. The values to format are passed as a simple object (anonymous types are allowed of course). Here's an example:

    static void Main()
    {
        var joe = new Person { Name = "Joe", DateOfBirth = new DateTime(1980, 6, 22) };
        string text = StringTemplate.Format("{Name} was born on {DateOfBirth:D}", joe);
        Console.WriteLine(text); // Prints "Joe was born on Sunday, 22 June 1980"
    }

## Extension methods

### IsNullOrEmpty, IsNullOrWhiteSpace, Join

Same as the methods of the same name in the `String` class, but as extension methods, which makes them more convenient to use.

### GetLines

Enumerates all lines in a string.

### Left, Right

Returns a string containing a specified number of characters from the left or right side of a string. These methods are shortcuts to common use cases of `Substring`, and behave in the same way (so they will throw if you try to get more characters than the length of the string).

### Truncate

Returns a string truncated to the specified number of characters. Similar to `Left`, but doesn't throw if the string is shorter than the specified length.

### Capitalize

Capitalizes a string by making its first character uppercase.

### MatchesWildcard

Checks if a string matches the specified wildcard pattern.

### Ellipsis

Truncates a string to the specified length, replacing the last characters with an ellipsis (three dots) or with the specified ellipsis string.

### IsValidEmail

Checks if the specified string is a valid email address.

### Contains

Checks if the specified string contains the specified substring, using the specified comparison type. Unlike the `String.StartsWith` and `String.EndsWith` methods, the `String.Contains` method doesn't have an overload to specify the comparison type; this extension method fills that gap.

### ReplaceAt

Replaces a single character at the specified position with the specified replacement character.
