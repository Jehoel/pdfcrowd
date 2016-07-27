using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PdfCrowd.Tests
{
	/// <summary>The tests require a working Internet connection to pdfcrowd.com's web-service.</summary>
	[TestClass]
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
	}
}
