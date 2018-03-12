using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace ImageDL.Classes
{
	public interface IImageComparer : INotifyPropertyChanged
	{
		int StoredImages { get; }
		int CurrentImagesSearched { get; }
		int ThumbnailSize { get; }

		bool TryStore(Uri uri, FileInfo file, Stream stream, out string error);
		Task CacheSavedFilesAsync(DirectoryInfo directory, int taskGroupLength = 50);
		void DeleteDuplicates(float percentForMatch);
		void CacheFile(FileInfo file);
	}
}
