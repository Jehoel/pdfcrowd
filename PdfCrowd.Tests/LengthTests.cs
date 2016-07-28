using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PdfCrowd.Tests
{
	[TestClass]
	public class LengthTests
	{
		[TestMethod]
		public void TestUnspecifiedIsPoints()
		{
			Length points = new Length( 10, LengthUnit.Points );
			Length unspec = new Length( 10, LengthUnit.Unspecified );

			Assert.AreEqual( 0, points.CompareTo( unspec ) );
			Assert.AreEqual( points.ToMillimeters(), unspec.ToMillimeters() );
		}

		[TestMethod]
		public void TestMMNormalization()
		{
			Length mm     = new Length( 100, LengthUnit.Millimeters );

			Length cm     = new Length( 10, LengthUnit.Centimeters );
			Length inches = new Length( 3.93701M, LengthUnit.Inches );
			Length points = new Length( 283.465M, LengthUnit.Points );

			Assert.AreEqual( Math.Round( mm.ToMillimeters(), 3 ), Math.Round(     cm.ToMillimeters(), 3 ) );
			Assert.AreEqual( Math.Round( mm.ToMillimeters(), 3 ), Math.Round( inches.ToMillimeters(), 3 ) );
			Assert.AreEqual( Math.Round( mm.ToMillimeters(), 3 ), Math.Round( points.ToMillimeters(), 3 ) );
		}
	}
}
