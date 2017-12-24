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
		public readonly int Width;
		public readonly int Height;

		public ImageDetails(Uri uri, FileInfo file, IEnumerable<bool> boolHash, int width, int height)
		{
			Uri = uri;
			File = file;
			BoolHash = boolHash.ToList().AsReadOnly();
			Width = width;
			Height = height;
		}
	}
}
