namespace PdfCrowd
{
#pragma warning disable 1591 // Disable warnings about missing XML comments.

	public enum PdfPageLayout
	{
		/// <summary>(The PdfCrowd.com documentation does not say what the default page layout is)</summary>
		Unspecified = 0,
		SinglePage = 1,
		Continuous = 2,
		ContinuousFacing = 3
	}

	public enum PdfZoomType
	{
		/// <summary>(The PdfCrowd.com documentation does not say what the default Zoom Type is)</summary>
		Unspecified = 0,
		FitWidth = 1,
		FitHeight = 2,
		FitPage = 3,
		/// <summary>The zoom value itself is specified by the InitialPdfZoom options value.</summary>
		Zoom = 4
	}

	public enum PdfPageMode
	{
		Unspecified = 0,
		/// <summary>Neither document outline nor thumbnail images visible.</summary>
		ContentOnly = 1,
		/// <summary>Thumbnail images visible.</summary>
		WithThumbnails = 2,
		/// <summary>Full-screen mode.</summary>
		FullScreen = 3
	}

	public enum LengthUnit
	{
		/// <summary><c>Unspecified</c> Lengths are assumed to be <c>Points</c> by PdfCrowd.com.</summary>
		Unspecified = 0,
		Points,
		Inches,
		Millimeters,
		Centimeters
	}

#pragma warning restore 1591
}
