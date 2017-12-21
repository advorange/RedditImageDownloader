using System.Collections;
using System.IO;

namespace ImageDL.Classes
{
	public struct ImageDetails
	{
		public FileInfo File;
		public BitArray Bits;

		public ImageDetails(FileInfo file, BitArray bits)
		{
			File = file;
			Bits = bits;
		}
	}
}
