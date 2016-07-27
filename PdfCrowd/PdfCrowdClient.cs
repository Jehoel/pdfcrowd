using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;

namespace PdfCrowd
{
	/// <summary>A client for PdfCrowd.com</summary>
	public static class PdfCrowdClient
	{
		/// <summary>The public HTTP endpoint base address of PdfCrowd.com</summary>
		public static Uri HttpBaseUri  { get; } = new Uri("http://pdfcrowd.com/api/");

		/// <summary>The public HTTPS endpoint base address of PdfCrowd.com</summary>
		public static Uri HttpsBaseUri { get; } = new Uri("https://pdfcrowd.com/api/");

		/// <summary>The resource located at <paramref name="uri"/> is converted into a PDF.</summary>
		public static PdfCrowdPdfResponse ConvertFromUri(PdfCrowdOptions options, Uri uri)
		{
			if( options == null ) throw new ArgumentNullException( nameof(options) );
			if( uri     == null ) throw new ArgumentNullException( nameof(uri) );

			HttpWebResponse response = Execute( "pdf/convert/uri/", RequestEncoding.XFormWwwEncoding, options, 
				delegate(HttpStreamWriter writer, Boolean isFirst)
				{
					writer.WriteXWwwFormUrlEncoded( !isFirst, "src", uri.ToString(), shouldUriEncodeValue: true );
				}
			);

			return new PdfCrowdPdfResponse( response );
		}

		/// <summary>The raw HTML in <paramref name="rawHtml"/> is rendered and converted into a PDF.</summary>
		public static PdfCrowdPdfResponse ConvertFromHtml(PdfCrowdOptions options, String rawHtml)
		{
			if( options == null ) throw new ArgumentNullException( nameof(options) );
			if( rawHtml == null ) throw new ArgumentNullException( nameof(rawHtml) );

			HttpWebResponse response = Execute( "pdf/convert/html/", RequestEncoding.XFormWwwEncoding, options, 
				delegate(HttpStreamWriter writer, Boolean isFirst)
				{
					writer.WriteXWwwFormUrlEncoded( !isFirst, "src", rawHtml, shouldUriEncodeValue: true );
				}
			);

			return new PdfCrowdPdfResponse( response );
		}

		/// <summary>The file at <paramref name="fileName"/> is uploaded to PdfCrowd where it is rendered and converted to a PDF. The file can be a HTML file, or it can be a compressed archive file containing HTML and other files.</summary>
		/// <param name="options"></param>
		/// <param name="fileName">Path of the file to upload. An absolute path is not necessary (it is passed into the <c>FileStream</c> consructor as-is, so if the file is in the current working directory then a relative file-name is sufficient).</param>
		/// <param name="fileContentType">The MIME content-type of the file to upload. This can be null. If null, then the MIME content-type is derived from the file's file name extension. If there is no extension or the extension is not recognised then an <c>ArgumentException</c> is raised.</param>
		public static PdfCrowdPdfResponse ConvertFromFile(PdfCrowdOptions options, String fileName, String fileContentType)
		{
			if( options == null ) throw new ArgumentNullException( nameof(options) );
			if( fileName == null ) throw new ArgumentNullException( nameof(fileName) );

			using( FileStream fs = new FileStream( fileName, FileMode.Open, FileAccess.Read, FileShare.Read ) )
			{
				return ConvertFromFile( options, fileName, fileContentType, fs );
			}
		}

		private static readonly Char[] _fileNameInvalidChars = GetFileNameInvalidChars();

		private static Char[] GetFileNameInvalidChars()
		{
			List<Char> chars = new List<Char>();
			chars.Add( Path.PathSeparator );
			chars.Add( Path.VolumeSeparatorChar );
			chars.Add( Path.DirectorySeparatorChar );
			chars.Add( Path.AltDirectorySeparatorChar );
			chars.AddRange( Path.GetInvalidFileNameChars() );
			return chars.ToArray();
		}

		/// <summary>Uploads a single HTML file, or an archive (.zip, .tar.gz, or .tar.bz2) file.</summary>
		/// <param name="options"></param>
		/// <param name="fileName">The short file name of the file to upload (no paths permitted, they will cause an <c>ArgumentException</c> to be raised). This is used as the file-name field in MIME uploads.</param>
		/// <param name="fileContentType">The MIME content-type of the file to upload. This can be null. If null, then the MIME content-type is derived from the file's file name extension. If there is no extension or the extension is not recognised then an <c>ArgumentException</c> is raised.</param>
		/// <param name="stream">The file contents to upload.</param>
		/// <exception cref="ArgumentException">The <paramref name="fileName"/> value was null, empty or invalid (e.g. not a short file-name). Or the MIME content-type could not be derived (and was not explicitly specified).</exception>
		public static PdfCrowdPdfResponse ConvertFromFile(PdfCrowdOptions options, String fileName, String fileContentType, Stream stream)
		{
			if( options == null ) throw new ArgumentNullException( nameof(options) );
			if( fileName == null ) throw new ArgumentNullException( nameof(fileName) );
			if( stream == null ) throw new ArgumentNullException( nameof(stream) );

			if( fileName.Length == 0 || Path.IsPathRooted( fileName ) || fileName.IndexOfAny( _fileNameInvalidChars ) > -1 )
			{
				throw new ArgumentException( Resources.PdfCrowdClient_InvalidFileNameArgument, nameof(fileName) );
			}

			if( fileContentType == null )
			{
				fileContentType = GetContentType( Path.GetExtension( fileName ) );
				if( fileContentType == null )
				{
					throw new ArgumentException( Resources.PdfCrowdClient_InvalidFileContentTypeArgument, nameof(fileContentType) );
				}
			}

			// NOTE: It isn't documented, but can we upload using content-type: application/octet-stream for maximum efficiency?
			// Update: No, it looks for the "src" name for the data to use.

			HttpWebResponse response = Execute( "pdf/convert/html/", RequestEncoding.MultipartFormData, options, 
				delegate(HttpStreamWriter writer, Boolean isFirst)
				{
					writer.WriteMultipartFormData( "src", fileName, fileContentType, stream );
				}
			);

			return new PdfCrowdPdfResponse( response );
		}

		private static String GetContentType(String extension)
		{
			switch(extension)
			{
				case ".html":
				case ".htm":
					return "text/html";
				case ".tar.gz":
					return "application/gzip"; // http://superuser.com/questions/901962/what-is-the-correct-mime-type-for-a-tar-gz-file
				case ".tar.bz2":
					return "application/x-bzip2"; // https://en.wikipedia.org/wiki/List_of_archive_formats
				case ".zip":
					return "application/zip";
				default:
					return null;
			}
		}

		/// <summary>Gets the token balance of the PdfCrowd.com account specified by the <c>UserName</c> property of <paramref name="options" />.</summary>
		public static Int32 GetRemainingTokens(PdfCrowdOptions options)
		{
			if( options == null ) throw new ArgumentNullException( nameof(options) );

			Int32 tokens = Int32.MinValue;
			Boolean gotResponse = false;

			String path = String.Concat( "user/", options.UserName, "/tokens/" );
			using( HttpWebResponse response = Execute( path, RequestEncoding.XFormWwwEncoding, options, null ) )
			using( Stream responseStream = response.GetResponseStream() )
			using( StreamReader rdr = new StreamReader( responseStream ) ) // TODO: Response encoding?
			{
				String responseText = rdr.ReadToEnd();

				if( !Int32.TryParse( responseText, NumberStyles.Integer, CultureInfo.InvariantCulture, out tokens ) )
				{
					throw new PdfCrowdException( PdfCrowdErrorCode.CouldNotParseGetRemainingTokens, responseText );
				}

				gotResponse = true;
			}

			if( !gotResponse )
			{
				throw new PdfCrowdException( PdfCrowdErrorCode.ResponseWasNotProcessed );
			}

			return tokens;
		}

		private enum RequestEncoding
		{
			XFormWwwEncoding,
			MultipartFormData
		}

		private static readonly String _UserAgent = "PdfCrowdClient/" + typeof(PdfCrowdClient).Assembly.GetName().Version.ToString() + " (OS: " + Environment.OSVersion.ToString() + "; .NET: " + Environment.Version.ToString() + ")";

		private delegate void OnWriteRequest(HttpStreamWriter writer, Boolean isFirst);

		// All requests are POST, the Username and API key are submitted as named values in the body.
		private static HttpWebResponse Execute(String path, RequestEncoding encoding, PdfCrowdOptions options, OnWriteRequest onWriteRequest)
		{
			if( !options.Validate() ) throw new ArgumentException( Resources.PdfCrowdClient_InvalidPdfCrowdOptions, nameof(options) );

			Uri uri = new Uri( options.BaseUri, path );

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create( uri );
			request.Method = "POST";
			request.UserAgent = _UserAgent;

			using( Stream requestStream = request.GetRequestStream() )
			using( HttpStreamWriter writer = new HttpStreamWriter( requestStream ) )
			{
				if( encoding == RequestEncoding.XFormWwwEncoding )
				{
					request.ContentType = "application/x-www-form-urlencoded";

					Boolean first = true;
					options.ForEachValue(
						delegate(String keyName, String stringValue, Boolean shouldUriEncode) {

							writer.WriteXWwwFormUrlEncoded( withAmpersand: !first, key: keyName, value: stringValue, shouldUriEncodeValue: shouldUriEncode );

							first = false;
						}
					);

					onWriteRequest?.Invoke( writer, first );
				}
				else if( encoding == RequestEncoding.MultipartFormData )
				{
					request.ContentType = "multipart/form-data; boundary=" + HttpStreamWriter.MultipartBoundary;

					Boolean first = true;
					options.ForEachValue(
						delegate(String keyName, String stringValue, Boolean shouldUriEncode) {

							writer.WriteMultipartFormData( keyName, stringValue );

							first = false;
						}
					);

					onWriteRequest?.Invoke( writer, first );
				}

				writer.Flush();
			}

			return GetResponse( request );
		}

		private static HttpWebResponse GetResponse(HttpWebRequest request)
		{
			try
			{
				return (HttpWebResponse)request.GetResponse();
			}
			catch(WebException wex)
			{
				if( wex.Response != null )
				{
					HttpWebResponse response = (HttpWebResponse)wex.Response;

					String responseContent;
					using( StreamReader rdr = new StreamReader( response.GetResponseStream() ) )
					{
						responseContent = rdr.ReadToEnd();
					}

					Int32 statusCode = (Int32)response.StatusCode;
					switch( statusCode )
					{
						case 503:
							throw new PdfCrowdException( PdfCrowdErrorCode.RateLimited, wex, responseContent );
						case 510:
						case 502:
							throw new PdfCrowdException( PdfCrowdErrorCode.PdfGenerationTimeout, wex, responseContent );
						case 413:
							throw new PdfCrowdException( PdfCrowdErrorCode.SourceDataTooLarge, wex, responseContent );
					}
				}

				throw new PdfCrowdException( PdfCrowdErrorCode.UnhandledWebException, wex );
			}
		}
	}
}
