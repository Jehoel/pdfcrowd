// From https://github.com/grumpydev/RFC2047-Encoded-Word-Encoder-Decoder/blob/master/EncodedWord/RFC2047.cs
// With minor changes for .NET 2.0 compatibility and FxCop compliance.

//===============================================================================
// RFC2047 (Encoded Word) Decoder
//
// http://tools.ietf.org/html/rfc2047
//===============================================================================
// Copyright © Steven Robbins.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
namespace PdfCrowd
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Text;
	using System.Text.RegularExpressions;

	internal delegate String ContentEncoderDecorder(String value, Encoding encoding);

	/// <summary>
	/// Provides support for decoding RFC2047 (Encoded Word) encoded text
	/// </summary>
	public static class Rfc2047
	{
		/// <summary>
		/// Regex for parsing encoded word sections
		/// From http://tools.ietf.org/html/rfc2047#section-3
		/// encoded-word = "=?" charset "?" encoding "?" encoded-text "?="
		/// </summary>
		private static readonly Regex EncodedWordFormatRegEx = new Regex(@"=\?(?<charset>.*?)\?(?<encoding>[qQbB])\?(?<encodedtext>.*?)\?=", RegexOptions.Singleline | RegexOptions.Compiled);

		/// <summary>
		/// Regex for removing CRLF SPACE separators from between encoded words
		/// </summary>
		private static readonly Regex EncodedWordSeparatorRegEx = new Regex(@"\?=\r\n =\?", RegexOptions.Compiled);

		/// <summary>
		/// Replacement string for removing CRLF SPACE separators
		/// </summary>
		private const String SeparatorReplacement = @"?==?";

		/// <summary>
		/// The maximum line length allowed
		/// </summary>
		private const int MaxLineLength = 75;

		/// <summary>
		/// Regex for "Q-Encoding" hex bytes from http://tools.ietf.org/html/rfc2047#section-4.2
		/// </summary>
		private static readonly Regex QEncodingHexCodeRegEx = new Regex(@"(=(?<hexcode>[0-9a-fA-F][0-9a-fA-F]))", RegexOptions.Compiled);

		/// <summary>
		/// Regex for replacing _ with space as declared in http://tools.ietf.org/html/rfc2047#section-4.2
		/// </summary>
		private static readonly Regex QEncodingSpaceRegEx = new Regex("_", RegexOptions.Compiled);

		/// <summary>
		/// Format for an encoded string
		/// </summary>
		private const String EncodedStringFormat = @"=?{0}?{1}?{2}?=";

		/// <summary>
		/// Special characters, as defined by RFC2047
		/// </summary>
		private static readonly char[] SpecialCharacters = { '(', ')', '<', '>', '@', ',', ';', ':', '<', '>', '/', '[', ']', '?', '.', '=', '\t' };

		/// <summary>
		/// Represents a content encoding type defined in RFC2047
		/// </summary>
		public enum ContentEncoding
		{
			/// <summary>
			/// Unknown / invalid encoding
			/// </summary>
			Unknown,

			/// <summary>
			/// "Q Encoding" (reduced character set) encoding
			/// http://tools.ietf.org/html/rfc2047#section-4.2
			/// </summary>
			QEncoding,

			/// <summary>
			/// Base 64 encoding
			/// http://tools.ietf.org/html/rfc2047#section-4.1
			/// </summary>
			Base64
		}

		private static String _defaultCharacterSet = Iso88591;

		/// <summary>Returns "iso-8859-1".</summary>
		public static String Iso88591 { get; } = "iso-8859-1";

		/// <summary>Returns "utf-8".</summary>
		public static String Utf8     { get; }  = "utf-8";

		/// <summary>The character set to use if none of explicitly specified. Defaults to "iso-8859-1".</summary>
		public static String DefaultCharacterSet
		{
			get { return _defaultCharacterSet; }
			set
			{
				if( !IsSupportedCharacterSet( value  ) ) throw new ArgumentOutOfRangeException( nameof(value) );
				_defaultCharacterSet = value;
			}
		}

		/// <summary>
		/// Encode a string into RFC2047
		/// </summary>
		/// <param name="value">Plain string to encode</param>
		/// <param name="contentEncoding">Content encoding to use</param>
		/// <param name="characterSet">Character set used by <paramref name="value"/>.</param>
		/// <returns>Encoded string</returns>
		public static String Encode(String value, ContentEncoding contentEncoding, String characterSet)
		{
			if( String.IsNullOrEmpty( value ) )
			{
				return String.Empty;
			}

			if( contentEncoding == ContentEncoding.Unknown )
			{
				throw new ArgumentException( "contentEncoding cannot be unknown for encoding.", "contentEncoding" );
			}

			if( !IsSupportedCharacterSet( characterSet ) )
			{
				throw new ArgumentException( "characterSet is not supported", "characterSet" );
			}

			var textEncoding = Encoding.GetEncoding(characterSet);

			var encoder = GetContentEncoder(contentEncoding);

			var encodedContent = encoder.Invoke(value, textEncoding);

			return BuildEncodedString( characterSet, contentEncoding, encodedContent );
		}

		/// <summary>
		/// Decode a string containing RFC2047 encoded sections
		/// </summary>
		/// <param name="encoded">String contaning encoded sections</param>
		/// <returns>Decoded string</returns>
		public static String Decode(String encoded)
		{
			// Remove separators
			var decodedString = EncodedWordSeparatorRegEx.Replace(encoded, SeparatorReplacement);

			return EncodedWordFormatRegEx.Replace(
				decodedString,
				m =>
				{
					var contentEncoding = GetContentEncodingType(m.Groups["encoding"].Value);
					if( contentEncoding == ContentEncoding.Unknown )
					{
						// Regex should never match, but return anyway
						return String.Empty;
					}

					var characterSet = m.Groups["charset"].Value;
					if( !IsSupportedCharacterSet( characterSet ) )
					{
						// Fall back to iso-8859-1 if invalid/unsupported character set found
						characterSet = @"iso-8859-1";
					}

					var textEncoding = Encoding.GetEncoding(characterSet);
					var contentDecoder = GetContentDecoder(contentEncoding);
					var encodedText = m.Groups["encodedtext"].Value;

					return contentDecoder.Invoke( encodedText, textEncoding );
				} );
		}

		/// <summary>
		/// Determines if a character set is supported
		/// </summary>
		/// <param name="characterSet">Character set name</param>
		/// <returns>Bool representing whether the character set is supported</returns>
		private static bool IsSupportedCharacterSet(String characterSet)
		{
			foreach(EncodingInfo e in Encoding.GetEncodings())
			{
				if( String.Equals( e.Name, characterSet, StringComparison.OrdinalIgnoreCase ) ) return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the content encoding type from the encoding character
		/// </summary>
		/// <param name="contentEncodingCharacter">Content contentEncodingCharacter character</param>
		/// <returns>ContentEncoding type</returns>
		private static ContentEncoding GetContentEncodingType(String contentEncodingCharacter)
		{
			switch( contentEncodingCharacter )
			{
				case "Q":
				case "q":
					return ContentEncoding.QEncoding;
				case "B":
				case "b":
					return ContentEncoding.Base64;
				default:
					return ContentEncoding.Unknown;
			}
		}

		/// <summary>
		/// Gets the content decoder delegate for the given content encoding type
		/// </summary>
		/// <param name="contentEncoding">Content encoding type</param>
		/// <returns>Decoding delegate</returns>
		private static ContentEncoderDecorder GetContentDecoder(ContentEncoding contentEncoding)
		{
			switch( contentEncoding )
			{
				case ContentEncoding.Base64:
					return DecodeBase64;
				case ContentEncoding.QEncoding:
					return DecodeQEncoding;
				default:
					// Will never get here, but return a "null" delegate anyway
					return (s, e) => String.Empty;
			}
		}

		/// <summary>
		/// Gets the content encoder delegate for the given content encoding type
		/// </summary>
		/// <param name="contentEncoding">Content encoding type</param>
		/// <returns>Encoding delegate</returns>
		private static ContentEncoderDecorder GetContentEncoder(ContentEncoding contentEncoding)
		{
			switch( contentEncoding )
			{
				case ContentEncoding.Base64:
					return EncodeBase64;
				case ContentEncoding.QEncoding:
					return EncodeQEncoding;
				default:
					// Will never get here, but return a "null" delegate anyway
					return (s, e) => String.Empty;
			}
		}

		/// <summary>
		/// Decodes a base64 encoded string
		/// </summary>
		/// <param name="encodedText">Encoded text</param>
		/// <param name="textEncoder">Encoding instance for the code page required</param>
		/// <returns>Decoded string</returns>
		private static String DecodeBase64(String encodedText, Encoding textEncoder)
		{
			var encodedBytes = Convert.FromBase64String(encodedText);

			return textEncoder.GetString( encodedBytes );
		}

		/// <summary>
		/// Encodes a base64 encoded string
		/// </summary>
		/// <param name="plainText">Plain text</param>
		/// <param name="textEncoder">Encoding instance for the code page required</param>
		/// <returns>Encoded string</returns>
		private static String EncodeBase64(String plainText, Encoding textEncoder)
		{
			Byte[] plainTextBytes = textEncoder.GetBytes(plainText);

			return Convert.ToBase64String( plainTextBytes );
		}

		/// <summary>
		/// Decodes a "Q encoded" string
		/// </summary>
		/// <param name="encodedText">Encoded text</param>
		/// <param name="textEncoder">Encoding instance for the code page required</param>
		/// <returns>Decoded string</returns>
		private static String DecodeQEncoding(String encodedText, Encoding textEncoder)
		{
			var decodedText = QEncodingSpaceRegEx.Replace(encodedText, " ");

			decodedText = QEncodingHexCodeRegEx.Replace(
				decodedText,
				m =>
				{
					var hexString = m.Groups["hexcode"].Value;

					int characterValue;
					if( !int.TryParse( hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out characterValue ) )
					{
						return String.Empty;
					}

					return textEncoder.GetString( new[] { (byte)characterValue } );
				} );

			return decodedText;
		}

		/// <summary>
		/// Encodes a "Q encoded" string
		/// </summary>
		/// <param name="plainText">Plain text</param>
		/// <param name="textEncoder">Encoding instance for the code page required</param>
		/// <returns>Encoded string</returns>
		private static String EncodeQEncoding(String plainText, Encoding textEncoder)
		{
			if( textEncoder.GetByteCount( plainText ) != plainText.Length )
			{
				throw new ArgumentException( "Q encoding only supports single byte encodings", "textEncoder" );
			}

			Byte[] specialBytes = textEncoder.GetBytes( SpecialCharacters );
			Dictionary<Byte,Byte> specialBytesSet = new Dictionary<Byte,Byte>();
			foreach(Byte b in specialBytes) if( !specialBytesSet.ContainsKey( b ) ) specialBytesSet.Add( b, 1 );

			StringBuilder sb = new StringBuilder(plainText.Length);

			Byte[] plainBytes = textEncoder.GetBytes(plainText);

			// Replace "high" values
			for( int i = 0; i < plainBytes.Length; i++ )
			{
				if( plainBytes[i] <= 127 && !specialBytesSet.ContainsKey( plainBytes[i] ) )
				{
					sb.Append( Convert.ToChar( plainBytes[i] ) );
				}
				else
				{
					sb.Append( "=" );
					sb.Append( Convert.ToString( plainBytes[i], 16 ).ToUpper( CultureInfo.InvariantCulture ) );
				}
			}

			return sb.ToString().Replace( " ", "_" );
		}

		/// <summary>
		/// Builds the full encoded string representation
		/// </summary>
		/// <param name="characterSet">Characterset to use</param>
		/// <param name="contentEncoding">Content encoding to use</param>
		/// <param name="encodedContent">Content, encoded to the above parameters</param>
		/// <returns>Valid RFC2047 string</returns>
		private static String BuildEncodedString(String characterSet, ContentEncoding contentEncoding, String encodedContent)
		{
			Char encodingCharacter = '\0';

			switch( contentEncoding )
			{
				case ContentEncoding.Base64:
					encodingCharacter = 'B';
					break;
				case ContentEncoding.QEncoding:
					encodingCharacter = 'Q';
					break;
			}

			Int32 wrapperLength = String.Format( CultureInfo.InvariantCulture, EncodedStringFormat, characterSet, encodingCharacter, String.Empty).Length;
			Int32 chunkLength = MaxLineLength - wrapperLength;

			if( encodedContent.Length <= chunkLength )
			{
				return String.Format( CultureInfo.InvariantCulture, EncodedStringFormat, characterSet, encodingCharacter, encodedContent );
			}

			StringBuilder sb = new StringBuilder();
			foreach( var chunk in SplitStringByLength( encodedContent, chunkLength ) )
			{
				sb.AppendFormat( CultureInfo.InvariantCulture, EncodedStringFormat, characterSet, encodingCharacter, chunk );
				sb.Append( "\r\n " );
			}

			return sb.ToString();
		}

		/// <summary>
		/// Splits a string into chunks
		/// </summary>
		/// <param name="inputString">Input string</param>
		/// <param name="chunkSize">Size of each chunk</param>
		/// <returns>String collection of chunked strings</returns>
		public static IEnumerable<String> SplitStringByLength(this String inputString, int chunkSize)
		{
			for( int index = 0; index < inputString.Length; index += chunkSize )
			{
				yield return inputString.Substring( index, Math.Min( chunkSize, inputString.Length - index ) );
			}
		}

		// Optimized methods, less code:


		/// <summary>RFC2047-encodes the specified value using UTF-8 and Base64.</summary>
		public static String EncodeUtf8Base64(String value)
		{
			if( value == null ) return null;
			if( value.Length == 0 ) return value;

			Byte[] bytes = Encoding.UTF8.GetBytes( value );

			String base64 = Convert.ToBase64String( bytes );

			return BuildEncodedString( "utf-8", ContentEncoding.Base64, base64 );
		}
	}
}