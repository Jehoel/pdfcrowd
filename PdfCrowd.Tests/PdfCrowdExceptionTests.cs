using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PdfCrowd.Tests
{
	[TestClass]
	public class PdfCrowdExceptionTests
	{
		[TestMethod]
		public void LocaliseErrorCode()
		{
			PdfCrowdErrorCode[] allCodes = (PdfCrowdErrorCode[]) Enum.GetValues( typeof(PdfCrowdErrorCode) );

			foreach(PdfCrowdErrorCode code in allCodes)
			{
				String message = PdfCrowdException.LocalizeErrorCode( code );
				Assert.IsNotNull( message );
				Assert.IsTrue( message.Length > 0 );
			}

			Assert.IsTrue( true ); // reached the end
		}
	}
}
