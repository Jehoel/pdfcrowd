using System;
using System.Globalization;
using System.CodeDom.Compiler;

namespace PdfCrowd
{
	public partial class PdfCrowdOptions
	{
	#region Main
		
		/// <summary>Your username at Pdfcrowd.</summary>
		public String UserName { get; set; }
		
		/// <summary>API key. Can be found on your account page. Note that the key is regenerated when you change your password.</summary>
		public String ApiKey { get; set; }

	#endregion

	#region Page Setup
		
		/// <summary>PDF page width</summary>
		public Length Width { get; set; }
		
		/// <summary>PDF page height</summary>
		public Length Height { get; set; }
		
		/// <summary>Top PDF page margin</summary>
		public Length MarginTop { get; set; }
		
		/// <summary>Right PDF page margin</summary>
		public Length MarginRight { get; set; }
		
		/// <summary>Bottom PDF page margin</summary>
		public Length MarginBottom { get; set; }
		
		/// <summary>Left PDF page margin</summary>
		public Length MarginLeft { get; set; }

	#endregion

	#region Header and Footer
		
		/// <summary>Places the specified HTML code inside the page footer. The following variables are expanded: <c>%u</c> - URL to convert. <c>%p</c> - The current page number. <c>%n</c> - Total number of pages.</summary>
		public String FooterHtml { get; set; }
		
		/// <summary>Loads HTML code from the specified URL and places it inside the page footer. The following variables are expanded: <c>%u</c> - URL to convert. <c>%p</c> - The current page number. <c>%n</c> - Total number of pages.</summary>
		public Uri FooterUri { get; set; }
		
		/// <summary>Places the specified HTML code inside the page header. The following variables are expanded: <c>%u</c> - URL to convert. <c>%p</c> - The current page number. <c>%n</c> - Total number of pages.</summary>
		public String HeaderHtml { get; set; }
		
		/// <summary>Loads HTML code from the specified URL and places it inside the page header.  The following variables are expanded: <c>%u</c> - URL to convert. <c>%p</c> - The current page number. <c>%n</c> - Total number of pages.</summary>
		public Uri HeaderUri { get; set; }
		
		/// <summary>An offset between physical and logical page numbers. The default value is 0.</summary>
		public Int32 PageNumberingOffset { get; set; }

	#endregion

	#region HTML Options
		
		/// <summary>Do not print images.</summary>
		public Boolean DisableImages { get; set; }
		
		/// <summary>Do not print backgrounds.</summary>
		public Boolean DisableBackgrounds { get; set; }
		
		/// <summary>HTML zoom in percents. It determines the precision used for rendering of the HTML content. Despite its name, it does not zoom the HTML content. Higher values can improve glyph positioning and can lead to overall better visual appearance of the generated PDF .The default value is 100.</summary>
		public Boolean HtmlZoom { get; set; }
		
		/// <summary>Do not run JavaScript in web pages.</summary>
		public Boolean DisableJavaScript { get; set; }
		
		/// <summary>Do not create hyperlinks in the PDF.</summary>
		public Boolean DisableHyperlinks { get; set; }
		
		/// <summary>The text encoding to use when none is specified in the web page. The default value is utf-8.</summary>
		public Boolean TextEncoding { get; set; }
		
		/// <summary>Use the print CSS media type (if available).</summary>
		public Boolean UseCssPrintMediaType { get; set; }

	#endregion

	#region PDF Options
		
		/// <summary>Encrypts the PDF. This prevents search engines from indexing the document.</summary>
		public Boolean Encrypt { get; set; }
		
		/// <summary>Sets the PDF author field.</summary>
		public String Author { get; set; }
		
		/// <summary>Protects the PDF with an user password. When a PDF has an user password, it must be supplied in order to view the document and to perform operations allowed by the access permissions. At most 32 characters.</summary>
		public String UserPassword { get; set; }
		
		/// <summary>Protects the PDF with an owner password. Supplying an owner password grants unlimited access to the PDF including changing the passwords and access permissions. At most 32 characters.</summary>
		public String OwnerPassword { get; set; }
		
		/// <summary>Do not allow to print the generated PDF.</summary>
		public Boolean DisallowPrinting { get; set; }
		
		/// <summary>Do not allow to modify the PDF.</summary>
		public Boolean DisallowModifying { get; set; }
		
		/// <summary>Do not allow to extract text and graphics from the PDF.</summary>
		public Boolean DisallowCopyingContents { get; set; }
		
		/// <summary>Specifies the initial page layout when the PDF is opened in a viewer.</summary>
		public PdfPageLayout PageLayout { get; set; }
		
		/// <summary>Specifies the initial page zoom type when the PDF is opened in a viewer.</summary>
		public PdfZoomType InitialPdfZoomType { get; set; }
		
		/// <summary>Specifies the initial page zoom when the PDF is opened in a viewer. Defaults to 100%.</summary>
		public Decimal InitialPdfZoom { get; set; }
		
		/// <summary>Specifies the appearance of the PDF when opened.</summary>
		public PdfPageMode PageMode { get; set; }
		
		/// <summary>Prints at most the specified number of pages.</summary>
		public Int32 MaxPages { get; set; }
		
		/// <summary>The file name of the created PDF (max 180 chars). If not specified then the name is auto-generated.</summary>
		public String PdfFileName { get; set; }
		
		/// <summary>The scaling factor used to convert between HTML and PDF. The default value is 1.333 (4/3) which makes the PDF content up to 1/3 larger than HTML.</summary>
		public Decimal PdfScalingFactor { get; set; }
		
		/// <summary>The page background color in RRGGBB hexadecimal format.</summary>
		public String PageBackgroundColor { get; set; }
		
		/// <summary>Do not print the body background.</summary>
		public String TransparentBackground { get; set; }

	#endregion

	#region Watermark
		
		/// <summary>A public absolute URL of the watermark image (must start either with http:// or https://). The supported formats are PNG and JPEG.</summary>
		public Uri WatermarkUri { get; set; }
		
		/// <summary>The horizontal watermark offset. The default value is 0.</summary>
		public Length WatermarkOffsetX { get; set; }
		
		/// <summary>The vertical watermark offset. The default value is 0.</summary>
		public Length WatermarkOffsetY { get; set; }
		
		/// <summary>The watermark rotation in degrees.</summary>
		public Decimal WatermarkRotation { get; set; }
		
		/// <summary>When set then the watermark is be placed in the background. By default, the watermark is placed in the foreground.</summary>
		public Boolean WatermarkInBackground { get; set; }

	#endregion

	#region Miscellaneous
		
		/// <summary>The conversion request will fail if the converted URL returns 4xx or 5xx HTTP status code.</summary>
		public Boolean FailOnNon200 { get; set; }
		
		/// <summary>The value of the Content-Disposition HTTP header sent in the response. Allowed values: <c>inline</c>: The browser will open the PDF in the browser window. <c>attachment</c>: Forces the browser to pop up a Save As dialog. This is the default value.</summary>
		public String ContentDisposition { get; set; }
		
		/// <summary>Insert the Pdfcrowd logo to the footer.</summary>
		public Boolean PdfCrowdLogo { get; set; }

	#endregion

		
		[GeneratedCode("PdfCrowdOptions.tt", "1.0")]
		private void ForEachValueImpl(OnProperty callback)
		{
			if( this.UserName != default(String) )
			{
				callback( "username", this.UserName, true );
			}

			if( this.ApiKey != default(String) )
			{
				callback( "key", this.ApiKey, true );
			}

			if( this.Width != default(Length) )
			{
				callback( "width", this.Width.ToString(), true );
			}

			if( this.Height != default(Length) )
			{
				callback( "height", this.Height.ToString(), true );
			}

			if( this.MarginTop != default(Length) )
			{
				callback( "margin_top", this.MarginTop.ToString(), true );
			}

			if( this.MarginRight != default(Length) )
			{
				callback( "margin_right", this.MarginRight.ToString(), true );
			}

			if( this.MarginBottom != default(Length) )
			{
				callback( "margin_bottom", this.MarginBottom.ToString(), true );
			}

			if( this.MarginLeft != default(Length) )
			{
				callback( "margin_left", this.MarginLeft.ToString(), true );
			}

			if( this.FooterHtml != default(String) )
			{
				callback( "footer_html", this.FooterHtml, true );
			}

			if( this.FooterUri != default(Uri) )
			{
				callback( "footer_url", this.FooterUri.ToString(), true );
			}

			if( this.HeaderHtml != default(String) )
			{
				callback( "header_html", this.HeaderHtml, true );
			}

			if( this.HeaderUri != default(Uri) )
			{
				callback( "header_url", this.HeaderUri.ToString(), true );
			}

			if( this.PageNumberingOffset != default(Int32) )
			{
				callback( "page_numbering_offset", this.PageNumberingOffset.ToString( CultureInfo.InvariantCulture ), false );
			}

			if( this.DisableImages != default(Boolean) )
			{
				callback( "no_images", "true", false );
			}

			if( this.DisableBackgrounds != default(Boolean) )
			{
				callback( "no_backgrounds", "true", false );
			}

			if( this.HtmlZoom != default(Boolean) )
			{
				callback( "html_zoom", "true", false );
			}

			if( this.DisableJavaScript != default(Boolean) )
			{
				callback( "no_javascript", "true", false );
			}

			if( this.DisableHyperlinks != default(Boolean) )
			{
				callback( "no_hyperlinks", "true", false );
			}

			if( this.TextEncoding != default(Boolean) )
			{
				callback( "text_encoding", "true", false );
			}

			if( this.UseCssPrintMediaType != default(Boolean) )
			{
				callback( "use_print_media", "true", false );
			}

			if( this.Encrypt != default(Boolean) )
			{
				callback( "encrypted", "true", false );
			}

			if( this.Author != default(String) )
			{
				callback( "author", this.Author, true );
			}

			if( this.UserPassword != default(String) )
			{
				callback( "user_pwd", this.UserPassword, true );
			}

			if( this.OwnerPassword != default(String) )
			{
				callback( "owner_pwd", this.OwnerPassword, true );
			}

			if( this.DisallowPrinting != default(Boolean) )
			{
				callback( "no_print", "true", false );
			}

			if( this.DisallowModifying != default(Boolean) )
			{
				callback( "no_modify", "true", false );
			}

			if( this.DisallowCopyingContents != default(Boolean) )
			{
				callback( "no_copy", "true", false );
			}

			if( this.PageLayout != default(PdfPageLayout) )
			{
				callback( "page_layout", ((Int32)this.PageLayout).ToString( CultureInfo.InvariantCulture ), false );
			}

			if( this.InitialPdfZoomType != default(PdfZoomType) )
			{
				callback( "initial_pdf_zoom_type", ((Int32)this.InitialPdfZoomType).ToString( CultureInfo.InvariantCulture ), false );
			}

			if( this.InitialPdfZoom != default(Decimal) )
			{
				callback( "initial_pdf_zoom", this.InitialPdfZoom.ToString( CultureInfo.InvariantCulture ), false );
			}

			if( this.PageMode != default(PdfPageMode) )
			{
				callback( "page_mode", ((Int32)this.PageMode).ToString( CultureInfo.InvariantCulture ), false );
			}

			if( this.MaxPages != default(Int32) )
			{
				callback( "max_pages", this.MaxPages.ToString( CultureInfo.InvariantCulture ), false );
			}

			if( this.PdfFileName != default(String) )
			{
				callback( "pdf_name", this.PdfFileName, true );
			}

			if( this.PdfScalingFactor != default(Decimal) )
			{
				callback( "pdf_scaling_factor", this.PdfScalingFactor.ToString( CultureInfo.InvariantCulture ), false );
			}

			if( this.PageBackgroundColor != default(String) )
			{
				callback( "page_background_color", this.PageBackgroundColor, true );
			}

			if( this.TransparentBackground != default(String) )
			{
				callback( "transparent_background", this.TransparentBackground, true );
			}

			if( this.WatermarkUri != default(Uri) )
			{
				callback( "watermark_url", this.WatermarkUri.ToString(), true );
			}

			if( this.WatermarkOffsetX != default(Length) )
			{
				callback( "watermark_offset_x", this.WatermarkOffsetX.ToString(), true );
			}

			if( this.WatermarkOffsetY != default(Length) )
			{
				callback( "watermark_offset_y", this.WatermarkOffsetY.ToString(), true );
			}

			if( this.WatermarkRotation != default(Decimal) )
			{
				callback( "watermark_rotation", this.WatermarkRotation.ToString( CultureInfo.InvariantCulture ), false );
			}

			if( this.WatermarkInBackground != default(Boolean) )
			{
				callback( "watermark_in_background", "true", false );
			}

			if( this.FailOnNon200 != default(Boolean) )
			{
				callback( "fail_on_non200", "true", false );
			}

			if( this.ContentDisposition != default(String) )
			{
				callback( "content_disposition", this.ContentDisposition, true );
			}

			if( this.PdfCrowdLogo != default(Boolean) )
			{
				callback( "pdfcrowd_logo", "true", false );
			}

		}

		private void CopyPropertiesToImpl(PdfCrowdOptions other)
		{
			other.UserName = this.UserName;
			other.ApiKey = this.ApiKey;
			other.Width = ( this.Width != null ) ? new Length( this.Width ) : null;
			other.Height = ( this.Height != null ) ? new Length( this.Height ) : null;
			other.MarginTop = ( this.MarginTop != null ) ? new Length( this.MarginTop ) : null;
			other.MarginRight = ( this.MarginRight != null ) ? new Length( this.MarginRight ) : null;
			other.MarginBottom = ( this.MarginBottom != null ) ? new Length( this.MarginBottom ) : null;
			other.MarginLeft = ( this.MarginLeft != null ) ? new Length( this.MarginLeft ) : null;
			other.FooterHtml = this.FooterHtml;
			other.FooterUri = this.FooterUri;
			other.HeaderHtml = this.HeaderHtml;
			other.HeaderUri = this.HeaderUri;
			other.PageNumberingOffset = this.PageNumberingOffset;
			other.DisableImages = this.DisableImages;
			other.DisableBackgrounds = this.DisableBackgrounds;
			other.HtmlZoom = this.HtmlZoom;
			other.DisableJavaScript = this.DisableJavaScript;
			other.DisableHyperlinks = this.DisableHyperlinks;
			other.TextEncoding = this.TextEncoding;
			other.UseCssPrintMediaType = this.UseCssPrintMediaType;
			other.Encrypt = this.Encrypt;
			other.Author = this.Author;
			other.UserPassword = this.UserPassword;
			other.OwnerPassword = this.OwnerPassword;
			other.DisallowPrinting = this.DisallowPrinting;
			other.DisallowModifying = this.DisallowModifying;
			other.DisallowCopyingContents = this.DisallowCopyingContents;
			other.PageLayout = this.PageLayout;
			other.InitialPdfZoomType = this.InitialPdfZoomType;
			other.InitialPdfZoom = this.InitialPdfZoom;
			other.PageMode = this.PageMode;
			other.MaxPages = this.MaxPages;
			other.PdfFileName = this.PdfFileName;
			other.PdfScalingFactor = this.PdfScalingFactor;
			other.PageBackgroundColor = this.PageBackgroundColor;
			other.TransparentBackground = this.TransparentBackground;
			other.WatermarkUri = this.WatermarkUri;
			other.WatermarkOffsetX = ( this.WatermarkOffsetX != null ) ? new Length( this.WatermarkOffsetX ) : null;
			other.WatermarkOffsetY = ( this.WatermarkOffsetY != null ) ? new Length( this.WatermarkOffsetY ) : null;
			other.WatermarkRotation = this.WatermarkRotation;
			other.WatermarkInBackground = this.WatermarkInBackground;
			other.FailOnNon200 = this.FailOnNon200;
			other.ContentDisposition = this.ContentDisposition;
			other.PdfCrowdLogo = this.PdfCrowdLogo;
		}
	}
}

