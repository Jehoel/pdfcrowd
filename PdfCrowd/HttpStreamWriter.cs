using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace PdfCrowd
{
	/// <summary>Wraps (and owns) a Stream. Provides <c>x-www-form-urlencoded</c> and <c>multipart/form-data</c> encoding services.</summary>
	internal sealed class HttpStreamWriter : IDisposable
	{
		private static readonly ASCIIEncoding _ascii        = CreateAsciiEncoding();
		private static readonly Encoder       _asciiEncoder = _ascii.GetEncoder();

		const Int32 _CharBufferLength = 1024; // 1024 is the buffer size in StreamWriter.

		private          Int32  charI      = 0;
		private readonly Char[] charBuffer = new Char[ _CharBufferLength ];
		private readonly Byte[] byteBuffer = new Byte[ _ascii.GetMaxByteCount( _CharBufferLength ) ]; // reusable buffer to temporarily hold the ASCII encoded characters.

		public static String MultipartBoundary { get; } = CreateMultipartBoundary();

		/// <summary>Creates an ASCIIEncoding object that throws <c>EncoderFallbackException</c> when it encounters a non-ASCII character.</summary>
		private static ASCIIEncoding CreateAsciiEncoding()
		{
			return (ASCIIEncoding)Encoding.GetEncoding( "us-ascii", new EncoderExceptionFallback(), new DecoderExceptionFallback() );
		}

		private static String CreateMultipartBoundary()
		{
			// If we're feeling paranoid: search through the request body for the boundary and create a new one first.
			// You don't need to buffer the entire request body - simply searching each flushed buffer copy data would be sufficient (a cross-buffer IndexOf operation, of course).

			// We're sending a GUID because the odds of those bytes appearing by chance in any given upload are insanely low.
			return Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
		}

		private readonly Stream stream;

		public HttpStreamWriter(Stream stream)
		{
			this.stream = stream;
		}

		public void Dispose()
		{
			this.Flush();
			this.stream.Dispose();
		}

		////

		public void Write(Char value)
		{
			if( this.charI == _CharBufferLength )
			{
				this.Flush( false );
			}

			this.charBuffer[ this.charI++ ] = value;
		}

		public void Write(String value)
		{
			if( value == null ) return;

			Int32 lengthRemaining = value.Length;
			Int32 valueIdx = 0;
			while( lengthRemaining > 0 )
			{
				if( this.charI == _CharBufferLength )
				{
					this.Flush( false );
				}

				Int32 count = _CharBufferLength - this.charI;
				if( count > lengthRemaining ) count = lengthRemaining;

				value.CopyTo( valueIdx, this.charBuffer, this.charI, count );
				this.charI += count;
				valueIdx += count;
				lengthRemaining -= count;
			}
		}

		public void WriteLine()
		{
			this.Write( "\r\n" ); // Write "\r\n" instead of Environment.NewLine because HTTP uses \r\n as a line-separator.
		}

		public void WriteLine(String value)
		{
			this.Write( value );
			this.WriteLine();
		}

		public void Flush()
		{
			this.Flush( true );
		}

		private void Flush(Boolean flushEncoder)
		{
			if( this.charI == 0 ) return;

			Int32 bytesCount = _asciiEncoder.GetBytes( this.charBuffer, 0, this.charI, this.byteBuffer, 0, flushEncoder );
			this.charI = 0;

			if( bytesCount > 0 ) this.stream.Write( this.byteBuffer, 0, bytesCount );
		}

		////

		/// <summary>Has greater-than-7-bit characters.</summary>
		private static Boolean HasGT7BitChars(String s)
		{
			foreach(Char c in s)
			{
				if( c > 127 ) return true;
			}
			return false;
		}

		private static String WrapUriEscape(String value, Boolean skipCheck)
		{
			if( skipCheck ) return value;

			if( HasGT7BitChars( value ) ) return Uri.EscapeDataString( value );
			return value;
		}

		public void WriteXWwwFormUrlEncoded(Boolean withAmpersand, String key, String value, Boolean shouldUriEncodeValue)
		{
			if( withAmpersand ) this.Write( '&' );
			this.Write( key ); // All of the keys in this project are 7-bit safe, hence no need to escape them.
			this.Write( '=' );
			this.Write( WrapUriEscape( value, !shouldUriEncodeValue ) );
		}

		private static String WrapRfc2047(String value)
		{
			if( HasGT7BitChars( value ) ) return Rfc2047.Encode( value, Rfc2047.ContentEncoding.Base64, "utf-8" );
			return value;
		}

		private void WriteMultipartMessageHeader(String disposition, String name, String fileName, String contentType)
		{
			this.Write("--");
			this.WriteLine( MultipartBoundary );

			this.Write("Content-Disposition: ");
			this.Write( disposition ); // All of the disposition in this project are 7-bit safe, hence no need to escape them.

			if( name != null )
			{
				this.Write("; name=\"");
				this.Write( name ); // All of the keys in this project are 7-bit safe, hence no need to escape them.
				this.Write('"');
			}

			if( fileName != null )
			{
				this.Write("; filename=\"");
				this.Write( WrapRfc2047( fileName ) );
				this.Write('"');
			}

			this.WriteLine();

			if( contentType != null )
			{
				this.Write("Content-Type: ");
				this.WriteLine( contentType ); // Content-Type values will be 7-bit safe.
			}
		}

		private void WriteMultipartMessageFooterAfterFile()
		{
			this.WriteLine();
			this.Write("--");
			this.Write( MultipartBoundary );
			this.WriteLine("--");
		}

		/// <summary>Writes the specified key and value as a <c>form-data</c> multipart entry. The values are written as raw.</summary>
		public void WriteMultipartFormData(String key, String value)
		{
			this.WriteMultipartMessageHeader( "form-data", key, null, null );
			this.WriteLine();

			this.WriteLine( WrapRfc2047( value ) );
		}

		public void WriteMultipartFormData(String key, String fileName, String fileContentType, Stream fileStream)
		{
			this.WriteMultipartMessageHeader( "form-data", key, fileName, fileContentType ?? "application/octet-stream" );
			this.WriteLine();
			this.Flush();

			fileStream.CopyTo( this.stream );

			this.WriteMultipartMessageFooterAfterFile();
		}

#if DISABLED_CODE

		/// <summary>Uploads multiple files under a single name using multipart/mixed rules. This is different to calling <c>WriteMultipartFormData</c> multiple times.</summary>
		public void WriteMultipartFormData(String key, IEnumerable<FileUpload> fileUploads)
		{
			String innerBoundary = CreateMultipartBoundary();

			this.WriteMultipartMessageHeader( "form-data", key, null, null );
			this.Write("Content-Type: multipart/mixed; boundary=");
			this.Write( innerBoundary );

			foreach(FileUpload file in fileUploads)
			{
				this.WriteLine();
				this.Write("--");
				this.Write( innerBoundary );
				this.WriteMultipartMessageHeader( "file",  null, file.FileName, file.ContentType );
				this.WriteLine("Content-Transfer-Encoding: binary"); // adding this every time means we don't need to sniff file contents first to determine if they're 7-bit safe or not.
				this.WriteLine();

				this.Flush();

				file.FileStream.CopyTo( this.stream );
			}

			this.WriteLine();
			this.Write("--");
			this.Write( innerBoundary );
			this.Write("--"); // don't call WriteLine() because we want only 1 line-break after the "--", which will be added by `WriteMultipartMessageFooterAfterFile()`

			this.WriteMultipartMessageFooterAfterFile();

			this.WriteLine();
			this.Flush();
		}
#endif
	}

#if DISABLED_CODE
	/// <summary>The minimum set of data required for each multi-file upload field.</summary>
	public class FileUpload
	{
		public String ContentType { get; }
		public String FileName { get; }
		public Stream FileStream { get; }
	}
#endif
}


