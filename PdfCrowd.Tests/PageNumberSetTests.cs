using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PdfCrowd.Tests
{
	[TestClass]
	public class PageNumberSetTests
	{
		[TestMethod]
		public void TestPageNumbers()
		{
			Int32[] numbers = new Int32[] { 0, 1, 2, 3, 4, -5, -4, -3, -2, -1 };

			Random rng = new Random( 100 ); // const seeds for deterministic tests.

			for(int i = 0; i < 100; i++)
			{
				Int32[] scrambled = numbers.OrderBy( value => rng.Next() ).ToArray();

				PageNumberSet pns = new PageNumberSet();

				foreach(Int32 v in scrambled) pns.Add( v );

				Assert.AreEqual( "0,1,2,3,4,-5,-4,-3,-2,-1", pns.ToString() );
			}

		}
	}
}
