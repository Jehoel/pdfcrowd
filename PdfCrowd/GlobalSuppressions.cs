using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage( "Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "PdfCrowd.PdfCrowdClient.#GetRemainingTokens(PdfCrowd.PdfCrowdOptions)", Justification = "Best-practices is to suppress this warning: http://stackoverflow.com/questions/3831676/ca2202-how-to-solve-this-case" )]
[assembly: SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "api", Scope = "member", Target = "PdfCrowd.PdfCrowdOptions.#.ctor(System.String,System.String)" )]
[assembly: SuppressMessage( "Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Mi", Scope = "resource", Target = "PdfCrowd.Resources.resources", Justification = "Using the base-2 prefix \"Mi\" instead of the base-10 prefix \"M\"." )]
[assembly: SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api", Scope = "member", Target = "PdfCrowd.PdfCrowdOptions.#ApiKey" )]
[assembly: SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "PdfCrowd.PageNumberSet", Justification = "This is not a Collection." )]
[assembly: SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "api", Scope = "member", Target = "PdfCrowd.PdfCrowdOptions.#.ctor(System.String,System.String,System.Boolean)" )]
[assembly: SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rfc", Scope = "type", Target = "PdfCrowd.Rfc2047" )]
[assembly: SuppressMessage( "Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "PdfCrowd.Rfc2047.#SplitStringByLength(System.String,System.Int32)" )]
[assembly: SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Iso", Scope = "member", Target = "PdfCrowd.Rfc2047.#Iso88591" )]

