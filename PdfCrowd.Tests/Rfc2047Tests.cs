using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PdfCrowd.Tests
{
	[TestClass]
	public class Rfc2047Tests
	{
		[TestMethod]
		public void Should_decode_lower_case_q_quoted_text()
		{
			var input = @"=?iso-8859-1?q?=A1Hola,_se=F1or!?=";

			var output = Rfc2047.Decode(input);

			Assert.AreEqual( @"¡Hola, señor!", output );
		}

		[TestMethod]
		public void Should_decode_upper_case_q_quoted_text()
		{
			var input = @"=?iso-8859-1?Q?=A1Hola,_se=F1or!?=";

			var output = Rfc2047.Decode(input);

			Assert.AreEqual( @"¡Hola, señor!", output );
		}

		[TestMethod]
		public void Should_decode_upper_case_b_quoted_text()
		{
			var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes("Some test text"));
			var input = String.Format( CultureInfo.InvariantCulture, @"=?iso-8859-1?B?{0}?=", base64String );

			var output = Rfc2047.Decode(input);

			Assert.AreEqual( @"Some test text", output );
		}

		[TestMethod]
		public void Should_decode_lower_case_b_quoted_text()
		{
			var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes("Some test text"));
			var input = String.Format( CultureInfo.InvariantCulture, @"=?iso-8859-1?b?{0}?=", base64String );

			var output = Rfc2047.Decode(input);

			Assert.AreEqual( @"Some test text", output );
		}

		[TestMethod]
		public void Should_decode_multiple_quoted_blocks()
		{
			var input = @"Normal and multiple =?iso-8859-1?Q?=A1Hola,_se=F1or!?= quoted blocks =?iso-8859-1?B?U29tZSB0ZXN0IHRleHQ=?=";

			var output = Rfc2047.Decode(input);

			Assert.AreEqual( @"Normal and multiple ¡Hola, señor! quoted blocks Some test text", output );
		}

		[TestMethod]
		public void Should_fall_back_to_8859_if_character_set_invalid()
		{
			var input = @"=?wrong?Q?=A1Hola,_se=F1or!?=";

			var output = Rfc2047.Decode(input);

			Assert.AreEqual( @"¡Hola, señor!", output );
		}

		[TestMethod]
		public void Should_just_return_original_text_if_encoding_type_is_not_recognised()
		{
			var input = @"=?iso-8859-1?Z?=A1Hola,_se=F1or!?=";

			var output = Rfc2047.Decode(input);

			Assert.AreEqual( input, output );
		}

		[TestMethod]
		public void Should_ignore_invalid_hex_bytes_in_q_encoded_input()
		{
			var input = @"=?iso-8859-1?Q?=Z4Hola,_se=F1or!?=";

			var output = Rfc2047.Decode(input);

			Assert.AreEqual( @"=Z4Hola, señor!", output );
		}

		[TestMethod]
		public void Should_handle_encoded_words_separated_by_cr_lf_space()
		{
			var input = "=?iso-8859-1?q?=A1Hola,_se=F1or!?=\r\n =?iso-8859-1?q?=A1Hola,_se=F1or!?=";

			var output = Rfc2047.Decode(input);

			Assert.AreEqual( @"¡Hola, señor!¡Hola, señor!", output );
		}

		[TestMethod]
		public void Should_return_empty_string_when_encoding_empty_string()
		{
			var input = @"";

			var output = Rfc2047.Encode("", Rfc2047.ContentEncoding.Base64, Rfc2047.Iso88591);

			Assert.AreEqual( input, output );
		}

		[TestMethod]
		[ExpectedException( typeof( ArgumentException ) )]
		public void Should_throw_when_encoding_with_unknown_content_encoding()
		{
			Rfc2047.Encode( "test", Rfc2047.ContentEncoding.Unknown, Rfc2047.Iso88591 );
		}

		[TestMethod]
		[ExpectedException( typeof( ArgumentException ) )]
		public void Should_throw_when_encoding_with_invalid_character_set()
		{
			Rfc2047.Encode( "test", Rfc2047.ContentEncoding.QEncoding, "fake" );
		}

		[TestMethod]
		public void Should_encode_to_b_encoding()
		{
			var inputText = "Some test text";
			var inputCharacterSet = "iso-8859-1";
			var encodingType = Rfc2047.ContentEncoding.Base64;

			var result = Rfc2047.Encode(inputText, encodingType, inputCharacterSet);

			Assert.AreEqual( "=?iso-8859-1?B?U29tZSB0ZXN0IHRleHQ=?=", result );
		}

		[TestMethod]
		public void Should_encode_to_q_encoding()
		{
			var inputText = "¡Hola, señor!";
			var inputCharacterSet = "iso-8859-1";
			var encodingType = Rfc2047.ContentEncoding.QEncoding;

			var result = Rfc2047.Encode(inputText, encodingType, inputCharacterSet);

			Assert.AreEqual( "=?iso-8859-1?Q?=A1Hola=2C_se=F1or!?=", result );
		}

		[TestMethod]
		public void Should_decode_q_encoded_text_back_to_original_text()
		{
			var inputText = "¡Hola, señor!";
			var inputCharacterSet = "iso-8859-1";
			var encodingType = Rfc2047.ContentEncoding.QEncoding;
			var encoded = Rfc2047.Encode(inputText, encodingType, inputCharacterSet);

			var result = Rfc2047.Decode(encoded);

			Assert.AreEqual( inputText, result );
		}

		[TestMethod]
		public void Should_add_separators_so_lines_do_not_exceed_75_characters()
		{
			var inputText = "This is some very long text. It should be split so no line exceeds 75 characters and should have the separator in between";
			var inputCharacterSet = "iso-8859-1";
			var encodingType = Rfc2047.ContentEncoding.Base64;

			var result = Rfc2047.Encode(inputText, encodingType, inputCharacterSet).Split(new[] { "\r\n " }, StringSplitOptions.RemoveEmptyEntries);

			Assert.IsFalse( result.Where( l => l.Length > 75 ).Any() );
		}

		[TestMethod]
		public void Should_encode_non_iso_8859_1_characters()
		{
			String chineseText = "伯尼·桑德斯2016年";

			{
				String result = Rfc2047.Encode( chineseText, Rfc2047.ContentEncoding.Base64, Rfc2047.Utf8 ); // Have to use Base64 when using multi-byte encoding like UTF-8.
				String decoded = Rfc2047.Decode( result );
				Assert.AreEqual( chineseText, decoded );
			}

			{
				String result2 = Rfc2047.EncodeUtf8Base64( chineseText );
				String decode2 = Rfc2047.Decode( result2 );
				Assert.AreEqual( chineseText, decode2 );
			}
		}
	}
}