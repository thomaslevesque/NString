# NString

[![NuGet version](https://img.shields.io/nuget/v/NString.svg)](https://www.nuget.org/packages/NString)
[![AppVeyor build](https://img.shields.io/appveyor/ci/thomaslevesque/nstring.svg)](https://ci.appveyor.com/project/thomaslevesque/nstring)
[![AppVeyor tests](https://img.shields.io/appveyor/tests/thomaslevesque/nstring.svg)](https://ci.appveyor.com/project/thomaslevesque/nstring/build/tests)

A collection of utilities for working with strings in .NET.

This library is a Portable Class Library targeting .NET 4.5, Windows Phone 8 and Windows Store apps (8.0).

Available as a [NuGet package](https://www.nuget.org/packages/NString/)

## StringTemplate

This class allows you to format values in a way similar to `String.Format`, but with named placeholders instead of numbered placeholders. The values to format are passed as a simple object (anonymous types are allowed of course). Format specifiers can be used in the same way as with `String.Format`. Here's an example:

```csharp
static void Main()
{
    var joe = new Person { Name = "Joe", DateOfBirth = new DateTime(1980, 6, 22) };
    string text = StringTemplate.Format("{Name} was born on {DateOfBirth:D}", joe);
    Console.WriteLine(text); // Prints "Joe was born on Sunday, 22 June 1980"
}
```

## Extension methods

### IsNullOrEmpty, IsNullOrWhiteSpace, Join

Same as the methods of the same name in the `String` class, but as extension methods, which makes them more convenient to use.

```csharp
"".IsNullOrEmpty() // true
" ".IsNullOrWhiteSpace() // true
string[] words = { "hello", "world" };
words.Join(" ") // "hello world"
```

### GetLines

Lazily enumerates all lines in a string, making it easy to apply transformations to each line using Linq, for instance.

```csharp
public string IndentAllLines(string text, string indent = "    ")
{
    indentedLines = text.GetLines().Select(line => indent + line);
    return string.Join(Environment.NewLine, indentedLines);
}
```

### Left, Right

Returns a string containing a specified number of characters from the left or right side of a string. These methods are shortcuts for common use cases of `Substring`, and behave in the same way (so they will throw if you try to get more characters than the length of the string).

```csharp
"hello".Left(2) // "he"
"hello".Right(3) // "llo"
```

### Truncate

Returns a string truncated to the specified number of characters. Similar to `Left`, but doesn't throw if the string is shorter than the specified length.

```csharp
"hello".Truncate(2) // "he"
"hello".Truncate(8) // "hello"
```

### Capitalize

Capitalizes a string by making its first character uppercase.

```csharp
"hello".Capitalize() // "Hello"
```

### MatchesWildcard

Checks if a string matches the specified wildcard pattern (supports `*` for any number of characters, and `?` for exactly one character).

```csharp
if (fileName.MatchesWildCard("foobar.*.log"))
{
    ...
}
```

### Ellipsis

Truncates a string to the specified length, replacing the extra characters with an ellipsis (three dots) or with the specified ellipsis string.
The specified length includes the length of the ellipsis.

```csharp
"hello world".Ellipsis(8) // "hello..."
"hello world".Ellipsis(20) // "hello world"
```

### Contains

Checks if the specified string contains the specified substring, using the specified comparison type (`Ordinal`, `CurrentCultureIgnoreCase`, etc).  Unlike the `String.StartsWith` and `String.EndsWith` methods, the `String.Contains` method doesn't have an overload to specify the comparison type; this extension method fills that gap.

```csharp
"Hello World".Contains("world", StringComparison.CurrentCultureIgnoreCase) // true
```

### ReplaceAt

Replaces a single character at the specified position with the specified replacement character.

```csharp
"hello world".ReplaceAt(0, 'j') // "jello world"
```
