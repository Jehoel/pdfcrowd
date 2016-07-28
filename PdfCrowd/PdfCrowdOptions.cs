using System;

namespace PdfCrowd
{
	internal delegate void OnProperty(String keyName, String stringValue, Boolean shouldUriEncode);

	/// <summary>The set of options (both required and optional) for PDfCrowd's service.</summary>
	public partial class PdfCrowdOptions
	{
		/// <summary>Creates a new PdfCrowdOptions object and sets the minimum required properties. Uses HTTP endpoint.</summary>
		public PdfCrowdOptions(String userName, String apiKey) : this( userName, apiKey, useHttps: true )
		{
		}

		/// <summary>Creates a new PdfCrowdOptions object and sets the minimum required properties. Specify if HTTPS should be used or HTTP.</summary>
		public PdfCrowdOptions(String userName, String apiKey, Boolean useHttps)
		{
			this.UserName = userName;
			this.ApiKey   = apiKey;

			this.BaseUri  = useHttps ? PdfCrowdClient.HttpsBaseUri : PdfCrowdClient.HttpBaseUri;
		}

		/// <summary>Indicates if all required properties are present and valid.</summary>
		public Boolean Validate()
		{
#if DOTNET45
			if( String.IsNullOrEmpty( this.UserName ) ) return false;
			if( String.IsNullOrEmpty( this.ApiKey   ) ) return false;
#else
			if( String.IsNullOrEmpty( this.UserName ) ) return false;
			if( String.IsNullOrEmpty( this.ApiKey   ) ) return false;
#endif
			return true;
		}

#region Awkward Options
		
		/// <summary>Base address of the PdfCrowd.com web service to use.</summary>
		public Uri BaseUri { get; set; }

		private const String _HeaderFooterPageExcludeListKey = "header_footer_page_exclude_list";

		/// <summary>The physical page numbers on which the header a footer are not printed. Negative numbers count backwards from the last page: -1 is the last page, -2 is the last but one page, and so on.</summary>
		public PageNumberSet HeaderFooterPageExcludeList { get; } = new PageNumberSet();

#endregion

		internal void ForEachValue(OnProperty callback)
		{
			this.ForEachValueImpl( callback );

			// Awkward properties:

			String headerFooterPageExcludeList = this.HeaderFooterPageExcludeList.ToString();
			if( headerFooterPageExcludeList != null )
			{
				callback( _HeaderFooterPageExcludeListKey, headerFooterPageExcludeList, false );
			}
		}

		/// <summary>Creates a deep copy of this PdfCrowdOptions instance.</summary>
		public PdfCrowdOptions Clone()
		{
			PdfCrowdOptions dolly = new PdfCrowdOptions( this.UserName, this.ApiKey );
			this.CopyPropertiesToImpl( dolly );

			// Awkward properties:

			dolly.BaseUri = this.BaseUri;

			foreach(Int32 pageNumber in this.HeaderFooterPageExcludeList)
			{
				dolly.HeaderFooterPageExcludeList.Add( pageNumber );
			}

			//

			return dolly;
		}
	}
}
