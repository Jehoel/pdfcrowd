using System;
using System.IO;

#if DOTNET20
namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	internal sealed class ExtensionAttribute : Attribute
	{
	}
}

#endif

namespace PdfCrowd
{
#if DOTNET20 || DOTNET35

	internal static class Extensions
	{
		private const Int32 _CopyBufferLength = 81920; // See comment in Reference source, stream.cs. 81920 is deemed the optimal buffer size;

		public static void CopyTo(this Stream stream, Stream destination)
		{
			Byte[] buffer = new Byte[ _CopyBufferLength ];
			Int32 read;
			while( (read = stream.Read( buffer, 0, buffer.Length ) ) != 0 )
			{
				destination.Write( buffer, 0, read );
			}
		}
	}

#endif
}
