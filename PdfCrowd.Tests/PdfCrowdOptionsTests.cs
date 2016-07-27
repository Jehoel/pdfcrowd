using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PdfCrowd.Tests
{
	[TestClass]
	public class PdfCrowdOptionsTests
	{
		[TestMethod]
		public void TestClone()
		{
			PdfCrowdOptions orig = new PdfCrowdOptions( "a", "b" );
			orig.HeaderFooterPageExcludeList.Add( 1 );
			orig.HeaderFooterPageExcludeList.Add( -10 );
			orig.HeaderFooterPageExcludeList.Add( 0 );

			orig.HtmlZoom = true;
			orig.MarginBottom = new Length( 5, LengthUnit.Inches );

			/////////////////////

			PdfCrowdOptions clone = orig.Clone();

			Assert.AreEqual( orig.UserName, clone.UserName );
			Assert.AreEqual( orig.ApiKey, clone.ApiKey );
			Assert.AreEqual( orig.HeaderFooterPageExcludeList.ToString(), clone.HeaderFooterPageExcludeList.ToString() );
			Assert.AreEqual( orig.HtmlZoom, clone.HtmlZoom );
			Assert.AreEqual( orig.MarginBottom, clone.MarginBottom );

		}
	}
}
