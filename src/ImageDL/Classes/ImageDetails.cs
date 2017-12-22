using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageDL.Classes
{
	public struct ImageDetails
	{
		public readonly Uri Uri;
		public readonly FileInfo File;
		public readonly IReadOnlyList<bool> BoolHash;

		public ImageDetails(Uri uri, FileInfo file, IEnumerable<bool> boolHash)
		{
			Uri = uri;
			File = file;
			BoolHash = boolHash.ToList().AsReadOnly();
		}
	}
}
