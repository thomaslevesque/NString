# Release notes

## 1.3.0

- added support for .NET Standard 2.0
- added support for [SourceLink](https://github.com/dotnet/sourcelink)

## 1.2.0

- added support for .NET Standard 1.0

## 1.1.8

- added `Reverse` method
- split `Capitalize` method into two overload
- added `[Pure]` ReSharper annotations to extension methods

## 1.1.7

- fixed wrong dependency version in NuGet package

## 1.1.6

- fixed up NuGet package for compatibility with NuGet v3 and project.json (note: requires NuGet 2.8.6 or later)

## 1.1.5

- the `ClearCache` method now also clears the getter cache

## 1.1.4

- fixed regression
- fixed package dependencies

## 1.1.3

- fixed NuGet package (missing files)
- included documentation

## 1.1.2

- fixed lack of support for alignment in placeholders for `StringTemplate` (#2)
- updated to target Windows Phone 8.1 as well

## 1.1.0

- added support for case-insensitive matching in `MatchesWildcard` (#1)

## 1.0.2

- added ReSharper annotations

## 1.0.1

- switched unit tests to NUnit

## 1.0.0

- first release