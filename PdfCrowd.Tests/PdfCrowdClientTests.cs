using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PdfCrowd.Tests
{
	/// <summary>The tests require a working Internet connection to pdfcrowd.com's web-service.</summary>
	[TestClass]
	[DeploymentItem("AppSettings.config")] // This is needed because the presence of [DeploymentItem] causes the working-directory details to change.
	public class PdfCrowdClientTests
	{
		private static PdfCrowdOptions CreateOptions()
		{
			String userName = ConfigurationManager.AppSettings["pdfCrowdUserName"];
			String apiKey   = ConfigurationManager.AppSettings["pdfCrowdApiKey"];
			return new PdfCrowdOptions( userName, apiKey );
		}

		[TestMethod]
		public void TestGetTokens()
		{
			PdfCrowdOptions options = CreateOptions();

			Int32 tokens = PdfCrowdClient.GetRemainingTokens( options );

			// View the Test Output to see the token count.
			Debug.WriteLine("".PadLeft(80, '-'));
			Debug.WriteLine( tokens + " tokens remain.");
			Debug.WriteLine("".PadLeft(80, '-'));

			Assert.IsTrue( tokens >= 0 );
		}

		[TestMethod]
		public void TestConvertFromUri_Google()
		{
			PdfCrowdOptions options = CreateOptions();
			options.Author = "شيء التهابات";
			options.FooterHtml = "%p - चाय अच्छा है";

			using( PdfCrowdPdfResponse pdf = PdfCrowdClient.ConvertFromUri( options, new Uri( "https://www.google.com" ) ) )
			{
				Assert.IsTrue( pdf.IsPdf );
				Assert.IsTrue( pdf.ContentLength > 0 );
				Assert.IsNotNull( pdf.FileName );
				Assert.IsTrue( pdf.FileName.Length > 0 );

				pdf.SaveAs( "TestConvertFromUri_Google.pdf" ); // This file will end-up in the PdfCrowd.Tests\bin\Debug directory.
			}
		}

		[TestMethod]
		[DeploymentItem(@"TestData\Test.html")]
		public void TestConvertFromFile_HtmlFile()
		{
			PdfCrowdOptions options = CreateOptions();

			using( PdfCrowdPdfResponse pdf = PdfCrowdClient.ConvertFromFile( options, @"Test.html", "text/html" ) )
			{
				Assert.IsTrue( pdf.IsPdf );
				Assert.IsTrue( pdf.ContentLength > 0 );
			}
		}

		[TestMethod]
		[DeploymentItem(@"TestData\Test.html")]
		public void TestConvertFromFile_HtmlFile_NonAsciiFileName_NonAsciiFieldValue()
		{
			PdfCrowdOptions options = CreateOptions();
			options.Author = "شيء التهابات";
			options.FooterHtml = "%p - चाय अच्छा है";

			using( Stream htmlFileStream = File.OpenRead( @"Test.html" ) )
			{
				using( PdfCrowdPdfResponse pdf = PdfCrowdClient.ConvertFromFile( options, fileName: "伯尼·桑德斯2016年", fileContentType: "text/html", stream: htmlFileStream ) )
				{
					Assert.IsTrue( pdf.IsPdf );
					Assert.IsTrue( pdf.ContentLength > 0 );

					// TODO: How can I verify the text is in the PDF? I'll just save it and manually inspect it.

					pdf.SaveAs( "TestConvertFromFile_HtmlFile_NonAsciiFileName_NonAsciiFieldValue_Output.pdf" );  // This file will end-up in the PdfCrowd.Tests\bin\Debug directory.
				}
			}
		}

		[TestMethod]
		[DeploymentItem(@"TestData\Test-UTF8.html")]
		public void TestConvertFromFile_HtmlFile_Utf8()
		{
			PdfCrowdOptions options = CreateOptions();

			using( PdfCrowdPdfResponse pdf = PdfCrowdClient.ConvertFromFile( options, @"Test-UTF8.html", "text/html" ) )
			{
				Assert.IsTrue( pdf.IsPdf );
				Assert.IsTrue( pdf.ContentLength > 0 );
			}
		}

		[TestMethod]
		[DeploymentItem(@"TestData\Test.zip")]
		public void TestConvertFromFile_Zip()
		{
			PdfCrowdOptions options = CreateOptions();

			using( PdfCrowdPdfResponse pdf = PdfCrowdClient.ConvertFromFile( options, @"Test.zip", null ) )
			{
				Assert.IsTrue( pdf.IsPdf );
				Assert.IsTrue( pdf.ContentLength > 0 );
			}
		}

		[TestMethod]
		[DeploymentItem(@"TestData\Test.html")]
		public void TestConvertFromHtml()
		{
			String html = File.ReadAllText(@"Test.html");

			PdfCrowdOptions options = CreateOptions();

			using( PdfCrowdPdfResponse pdf = PdfCrowdClient.ConvertFromHtml( options, html ) )
			{
				Assert.IsTrue( pdf.IsPdf );
				Assert.IsTrue( pdf.ContentLength > 0 );
			}
		}

		[TestMethod]
		[DeploymentItem(@"TestData\Test-UTF8.html")]
		public void TestConvertFromHtml_Utf8()
		{
			String html = File.ReadAllText(@"Test-UTF8.html");

			PdfCrowdOptions options = CreateOptions();

			using( PdfCrowdPdfResponse pdf = PdfCrowdClient.ConvertFromHtml( options, html ) )
			{
				Assert.IsTrue( pdf.IsPdf );
				Assert.IsTrue( pdf.ContentLength > 0 );
			}
		}
		
		private const Int32 fiftyMiB =  50 * 1024 * 1024;

		private static MemoryStream CreateWhitespaceRequestBody(Int32 entitySize)
		{
			Byte[] buffer = new Byte[ entitySize ]; // Technically we can reduce the memory usage by using a generator to dynamically generate the body, but that would mean not knowing Request Content-Length in advance...
			
			for(int i = 0; i < buffer.Length; i++) // I tried this copy with an unsafe/fixed/ptr, and it took 102ms, but this for loop took 92ms *shrugs*.
			{
				buffer[i] = (Byte)' '; // charcode for a space character
			}

			MemoryStream ms = new MemoryStream( buffer ); // the memorystream wraps the buffer, it does not create a copy (fortunately).

			return ms;
		}

		// Getting a "Request entity too large" response is difficult - the service accepts uploads larger than the docs say, and if it's too large it simply closes the TCP connection.
		// I don't want to write code to handle TCP connection issues (which HttpWebRequest does anyway) because the cause for that is legion. It's fine the way it is.

		/// <summary>Sends a 50MB request with HTML located at the 25MB location.</summary>
		[TestMethod]
		[ExpectedPdfCrowdException(PdfCrowdErrorCode.SourceDataTooLarge)]
		[DeploymentItem(@"TestData\Test.html")]
		public void TestConvertFromFile_ExceedSize()
		{
			PdfCrowdOptions options = CreateOptions();

			// The docs say "> 20MB" is enough to trigger the error: "If the size of the uploaded data exceeds 20MB then an HTTP 413 Request entity too large error is returned. "
			// But I sent 20MB+1b and it still worked... // new MemoryStream( ( 20 * 1024 * 1024 ) + 1 )

			MemoryStream ms = CreateWhitespaceRequestBody( fiftyMiB );

			StreamWriter writer = new StreamWriter( ms );

			ms.Seek( fiftyMiB / 2, SeekOrigin.Begin);
			using( Stream fs = File.OpenRead( @"Test.html" ) )
			{
				fs.CopyTo( ms );
			}

			using( PdfCrowdPdfResponse pdf = PdfCrowdClient.ConvertFromFile( options, "TooBig.html", "text/html", ms ) )
			{
				Assert.IsFalse( pdf.IsPdf );
				Assert.IsTrue( pdf.ContentLength > 0 );
			}
		}

		/// <summary>Sends a 50MB request with HTML located at the start of the stream.</summary>
		[TestMethod]
		[ExpectedPdfCrowdException(PdfCrowdErrorCode.UnhandledWebException)]
		[DeploymentItem(@"TestData\Test.html")]
		public void TestConvertFromFile_ExceedSize_TcpConnectionClosed()
		{
			PdfCrowdOptions options = CreateOptions();

			MemoryStream ms = CreateWhitespaceRequestBody( fiftyMiB );

			StreamWriter writer = new StreamWriter( ms );
			using( Stream fs = File.OpenRead( @"Test.html" ) )
			{
				fs.CopyTo( ms );
			}

			using( PdfCrowdPdfResponse pdf = PdfCrowdClient.ConvertFromFile( options, "TooBig.html", "text/html", ms ) )
			{
				Assert.IsFalse( pdf.IsPdf );
			}
		}

		[TestMethod]
		[ExpectedPdfCrowdException(PdfCrowdErrorCode.PdfGenerationFailed)]
		public void TesConvertFromFile_Failed()
		{
			PdfCrowdOptions options = CreateOptions();

			MemoryStream ms = new MemoryStream( 20 * 1024 ); // 20KB of zeroes.
			
			using( PdfCrowdPdfResponse pdf = PdfCrowdClient.ConvertFromFile( options, "Empty.html", "text/html", ms ) )
			{
				Assert.IsFalse( pdf.IsPdf );
			}
		}

		[TestMethod]
		[ExpectedPdfCrowdException(PdfCrowdErrorCode.AuthenticationError)]
		public void TestGetTokens_Unauthenticated()
		{
			PdfCrowdOptions options = new PdfCrowdOptions( "foo", "bar" );
			
			Int32 tokens = PdfCrowdClient.GetRemainingTokens( options );
		}
	}

	public class ExpectedPdfCrowdExceptionAttribute : ExpectedExceptionBaseAttribute
	{
		public PdfCrowdErrorCode ExpectedCode { get; }

		public ExpectedPdfCrowdExceptionAttribute(PdfCrowdErrorCode expectedCode)
		{
			this.ExpectedCode = expectedCode;
		}

		protected override void Verify(Exception exception)
		{
			Assert.IsInstanceOfType( exception, typeof(PdfCrowdException) );

			PdfCrowdException ex = (PdfCrowdException)exception;
			
			Assert.AreEqual( this.ExpectedCode, ex.ErrorCode );
		}
	}
}
