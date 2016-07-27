# pdfcrowd
An independently developed .NET PdfCrowd.com client

## Background

PdfCrowd.com has their own client ( https://github.com/pdfcrowd/pdfcrowd-dotnet ) however I'm not happy with the client because it doesn't conform with the Microsoft C# and .NET coding conventions, as well as not being stylistically idiomatic C#.

## Details

For maximum compatibility, this client is written against the .NET Framework 2.0 (rendering it fully compatible with .NET 2.0, 3.5, 4.0 and 4.5). I have not attempted to port it to DNX (".NET Core").

It might be an idea to add `async`/`await` support, submit a PR and I'll merge it - with one request: please wrap .NET 4.5-specific things like `#if DOTNET45` so it will build against .NET 2.0 and 3.5, thanks!

## Objectives

* Thread-safe
* Culture-safe
* Idiomatic C#
* FxCop compliant
* Minimal memory usage (avoid intermediate strings, prefer copying directly between streams)

## Design and Usage

* All client configuration and state (e.g. PDF options, etc) is stored in the `PdfCrowdOptions` class.
* The methods to execute a request are static and thread-safe, just pass in the `PdfCrowdOptions` object for configuration.
* Tip: you can `.Clone()` the `PdfCrowdOptions` object if you want to change a single value for a single request without having to set everything up again.

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
