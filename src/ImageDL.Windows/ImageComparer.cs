using ImageDL.Classes.ImageComparers;
using Microsoft.VisualBasic.FileIO;

namespace ImageDL.Classes
{
	/// <summary>
	/// Attempts to get rid of duplicate images.
	/// </summary>
	public sealed class WindowsImageComparer : ImageComparer<WindowsImageDetails>
	{
		protected override void Delete(ImageDetails details)
		{
			FileSystem.DeleteFile(details.File.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
		}
	}
}
