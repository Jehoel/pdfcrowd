using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace PdfCrowd
{
	/// <summary>Represents expected errors in the PdfCrowd client process.</summary>
	[Serializable]
	public class PdfCrowdException : Exception
	{
		#region Standard Exception and Serialization

		/// <summary>Creates a new PdfCrowdException instance. Class-library consumers do not need to instantiate this class. The constructor is public because of CA1032.</summary>
		public PdfCrowdException() { }

		/// <summary>Creates a new PdfCrowdException instance. Class-library consumers do not need to instantiate this class. The constructor is public because of CA1032.</summary>
		public PdfCrowdException(String message) : base( message ) { }

		/// <summary>Creates a new PdfCrowdException instance. Class-library consumers do not need to instantiate this class. The constructor is public because of CA1032.</summary>
		public PdfCrowdException(String message, Exception inner) : base( message, inner ) { }

		/// <summary>Creates a new PdfCrowdException instance when deserializing. Class-library consumers do not need to instantiate this class.</summary>
		protected PdfCrowdException(SerializationInfo info, StreamingContext context) : base( info, context )
		{
			if( info == null ) throw new ArgumentNullException( nameof( info ) );

			this.ErrorCode = (PdfCrowdErrorCode)info.GetInt32( _errorCodeName );
			this.Details   = info.GetString( _detailsName );
		}

		/// <summary>Populates the SerializationInfo instance when serializing.</summary>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if( info == null ) throw new ArgumentNullException( nameof( info ) );

			info.AddValue("errorCode", (Int32)this.ErrorCode);
			info.AddValue("details", this.Details);

			base.GetObjectData( info, context );
		}

		#endregion

		internal PdfCrowdException(PdfCrowdErrorCode errorCode) : this( errorCode, inner: null, details: null )
		{
		}

		internal PdfCrowdException(PdfCrowdErrorCode errorCode, String details) : this( errorCode, inner: null, details: details )
		{
		}

		internal PdfCrowdException(PdfCrowdErrorCode errorCode, Exception inner) : this( errorCode, inner, null )
		{
		}

		internal PdfCrowdException(PdfCrowdErrorCode errorCode, Exception inner, String details) : base( LocalizeErrorCode( errorCode ), inner )
		{
			this.ErrorCode = errorCode;
			this.Details   = details;
		}

		/// <summary>Gets the unique error code for the given exception. Use this to determine exactly what the issue is.</summary>
		public PdfCrowdErrorCode ErrorCode { get; }

		/// <summary>Contextual information whose meaning and significance varies with <c>ErrorCode</c>.</summary>
		public String Details { get; }

		/// <summary>Returns a localized (human-readable) message for the given error code. The Exception.Message property is set to the string returned from this method automatically. Class library consumers should never need to call this method.</summary>
		/// <exception cref="InvalidEnumArgumentException">When the error code value is not a defined enumeration value.</exception>
		public static String LocalizeErrorCode(PdfCrowdErrorCode code)
		{
			switch(code)
			{
				case PdfCrowdErrorCode.RateLimited:
					return Resources.PdfCrowdErrorCode_RateLimited;

				case PdfCrowdErrorCode.PdfGenerationTimeout:
					return Resources.PdfCrowdErrorCode_PdfGenerationTimeout;

				case PdfCrowdErrorCode.UnhandledWebException:
					return Resources.PdfCrowdErrorCode_UnhandledWebException;

				case PdfCrowdErrorCode.SourceDataTooLarge:
					return Resources.PdfCrowdErrorCode_SourceDataTooLarge;
				
				//

				case PdfCrowdErrorCode.UnhandledBadRequest:
					return Resources.PdfCrowdErrorCode_UnhandledBadRequest;

				case PdfCrowdErrorCode.AuthenticationError:
					return Resources.PdfCrowdErrorCode_AuthenticationError;

				case PdfCrowdErrorCode.PdfGenerationFailed:
					return Resources.PdfCrowdErrorCode_PdfGenerationFailed;

				case PdfCrowdErrorCode.UnhandledServiceError:
					return Resources.PdfCrowdErrorCode_UnhandledServiceError;

				//

				case PdfCrowdErrorCode.ResponseWasNotProcessed:
					return Resources.PdfCrowdErrorCode_ResponseWasNotProcessed;
				case PdfCrowdErrorCode.CouldNotParseGetRemainingTokens:
					return Resources.PdfCrowdErrorCode_CouldNotParseGetRemainingTokens;

				default:
					throw new InvalidEnumArgumentException( nameof(code), (Int32)code, typeof(PdfCrowdErrorCode) );
			}
		}

		private const String _errorCodeName = nameof(ErrorCode);
		private const String _detailsName   = nameof(Details);
	}

	/// <summary>Uniquely identifies the error being raised in a culture-independent way.</summary>
	public enum PdfCrowdErrorCode
	{
		// Documented server responses:

		/// <summary>The PDfCrowed service returned <c>HTTP 503</c> indicating you have made too many requests in a small time period (i.e. you have been rate-limited).</summary>
		RateLimited,
		/// <summary>The PdfCrowd service took more than 40 seconds to generate the PDF or the generated PDF is larger than 100MiB. so the job was aborted.</summary>
		PdfGenerationTimeout,
		/// <summary>The size of the uploaded data exceeded 20MiB and the server returned <c>HTTP 413 Request entity too large</c>.</summary>
		/// <remarks>In practice, the service accepted uploads larger than 30MiB and the service closed the TCP connection when sending 50MiB, I was unable to generate this response during testing.</remarks>
		SourceDataTooLarge,

		// Other expected server responses:

		/// <summary>The service returned HTTP 400 indicating a bad request, but this PdfCrowd client library doesn't know why.</summary>
		UnhandledBadRequest,

		/// <summary>The service rejected the provided username and API key. The server response message is in the <c>Details</c> property.</summary>
		AuthenticationError,

		/// <summary>The PDF generation failed for a reason other than a timeout. See the <c>Details</c> property for the respose content.</summary>
		PdfGenerationFailed,

		/// <summary>The service returned an error code which is not documented. See the <c>Details</c> property for the respose content. View the response by casting the <c>InnerException</c> to <c>WebException</c> and examining the <c>Response</c> property.</summary>
		UnhandledServiceError,

		/// <summary>A <c>WebException</c> was raised by the internal <c>HttpWebRequest</c> which was unexpected and so unhandled.</summary>
		UnhandledWebException,

		// TODO: Add one to handle "out of tokens", but I haven't encountered it yet.

		// Specifics:

		/// <summary>This exception should never happen: the Execute() callback was not executed.</summary>
		ResponseWasNotProcessed,

		/// <summary>The response from GetRemainingTokens could not be parsed as an integer value. Raw string value is in the Exception.Details property.</summary>
		CouldNotParseGetRemainingTokens
	}
}
