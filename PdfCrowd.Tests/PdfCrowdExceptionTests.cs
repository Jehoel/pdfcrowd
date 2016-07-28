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

				Assert.AreEqual( -1, message.IndexOf('<') ); // Ensure there are no XML documentation comments in the resource message text (because the Resources file was populated by copying from the XML docs).
			}

			Assert.IsTrue( true ); // reached the end
		}
	}
}
