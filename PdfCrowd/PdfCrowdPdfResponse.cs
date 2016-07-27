using System;
using System.IO;
using System.Net;

namespace PdfCrowd
{
	/// <summary>Encapsulates a generated PDF resposne from the service. Ensure consuming code wraps this in a <c>using</c> block or is otherwise sure to call <c>Dispose</c>.</summary>
	public sealed class PdfCrowdPdfResponse : IDisposable
	{
		private readonly HttpWebResponse response;
		private readonly Stream          stream;

		internal PdfCrowdPdfResponse(HttpWebResponse response)
		{
			this.response = response;
			this.stream   = response.GetResponseStream();
		}

		/// <summary>Disposes of the HttpWebResponse and Stream instances wrapped by this object.</summary>
		public void Dispose()
		{
			this.stream.Dispose();
			((IDisposable)this.response).Dispose();
		}

		/// <summary>Gets the HttpWebResponse received from the service. Do not call <c>GetResponseStream()</c>, instead use the <c>Stream</c> property on this object.</summary>
		public HttpWebResponse Response
		{
			get { return this.response; }
		}

		/// <summary>Gets the response stream received from the service, this will contain a PDF document file if the operation was successful.</summary>
		public Stream Stream
		{
			get { return this.stream; }
		}

		/// <summary>The file name from the Response Content-Disposition header.</summary>
		public String FileName
		{
			get { return this.response.Headers["Content-Disposition"]; }
		}

		/// <summary>Indicates if the response MIME content-type is the PDF content-type.</summary>
		public Boolean IsPdf
		{
			get { return this.response.ContentType == PdfContentType; }
		}

		/// <summary>Returns the length of the response (<c>HttpWebResponse.ContentLength</c>).</summary>
		public Int64 ContentLength
		{
			get { return this.response.ContentLength; }
		}

		/// <summary>Returns the string "application/pdf".</summary>
		public static String PdfContentType { get; } = "application/pdf";

		/// <summary>Saves the response data to the specified fileName. This wraps a call to <c>FileStream</c>'s constructor, so ensure that no file will be overwritten.</summary>
		public void SaveAs(String fileName)
		{
			using( FileStream fs = new FileStream( fileName, FileMode.CreateNew, FileAccess.Write, FileShare.None ) )
			{
				this.Stream.CopyTo( fs );
			}
		}

		/// <summary>Saves the response data to the specified stream. This wraps a call to <c>Stream.CopyTo</c>.</summary>
		public void SaveAs(Stream destination)
		{
			this.Stream.CopyTo( destination );
		}
	}
}
