# pdfcrowd
An independently developed .NET PdfCrowd.com client

## Background

PdfCrowd.com has their own client ( https://github.com/pdfcrowd/pdfcrowd-dotnet ) however I'm not happy with the client because it doesn't conform with the Microsoft C# and .NET coding conventions, as well as not being stylistically idiomatic C#.

## Objectives

* Thread-safe
* Culture-safe
* Idiomatic C#
* FxCop compliant
* Minimal memory usage (avoid intermediate strings, prefer copying directly between streams)
* Builds for .NET 2.0, 3.5 and 4.5

## Design and Usage

* All client configuration and state (e.g. PDF options, etc) is stored in the `PdfCrowdOptions` class.
* The methods to execute a request are static and thread-safe, just pass in the `PdfCrowdOptions` object for configuration.
* Tip: you can `.Clone()` the `PdfCrowdOptions` object if you want to change a single value for a single request without having to set everything up again.

## Future Ideas

* .NET 4.0 support? (It already has 4.5 support, I don't know if this is worthwhile)
* .NET Core (DNX) support
* C# `async` and `await` support?
  * (If you choose to add this, please wrap the code in `#ifdef DOTNET45` statements so the project builds on older .NET versions.

## Coding Style

Naming and casing style is Microsoft's .NET naming conventions, amended with the following rules:

* Casing:
  * `TitleCase` types and all type members (except fields).
  * `camelCase` for private fields, locals and parameters (always use `this.` for accessing all instance members, such as fields).
  * `_underscoreCamelCase` for static fields (including `static readonly`).
  * `_UnderscoreTitleCase` for const fields.
* Braces and newlines
  * Use the "braces in same column on their own lines" style.
  * Braces-lines generally should not be followed by an empty line. There should be no more than 2 empty lines (to visually separate related code).
* Indentation
  * "Semantic indentation" - use Tabs `\t` for each indent level, use individual spaces for vertical alignment after an indent-level.
    * http://lea.verou.me/2012/01/why-tabs-are-clearly-superior/
* C#
  * Use the actual type names instead of language alias, e.g. `Int32` instead of `int`, `Single` instead of `float`, etc. This applies to declarations and accessing static members.
    * With the exception of performing casts where `Int64 foo = (long)123;` is acceptable.
  * Only use `var` when dealing with anonymous types or nested generics longer than ~20 chars in length.
    * When using a complicated concrete generic, e.g. `Dictionary<String,List<Tuple<String,Int32>>>` prefer using a `using` type name alias instead.
