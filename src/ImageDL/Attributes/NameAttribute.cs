using System;

namespace ImageDL.Attributes
{
	/// <summary>
	/// Holds the name of this downloader.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DownloaderNameAttribute : Attribute
	{
		/// <summary>
		/// The name of the downloader.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Creates an instance of <see cref="DownloaderNameAttribute"/>.
		/// </summary>
		/// <param name="name"></param>
		public DownloaderNameAttribute(string name)
		{
			Name = name;
		}
	}
}