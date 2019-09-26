using System;
using System.IO;

using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageComparing
{
	/// <summary>
	/// Creates a factory to create <see cref="IImageComparer"/>.
	/// </summary>
	public sealed class ImageComparerFactory<T> : IImageComparerFactory where T : IImageComparer
	{
		/// <summary>
		/// Creates a comparer.
		/// </summary>
		/// <param name="databasePath"></param>
		/// <returns></returns>
		public IImageComparer CreateComparer(string databasePath)
		{
			//Make sure the file exists
			if (!File.Exists(databasePath))
			{
				Directory.CreateDirectory(databasePath);
				using var fs = File.Create(databasePath);
			}
			//Make the database hidden so it's not randomly seen
			File.SetAttributes(databasePath, File.GetAttributes(databasePath) | FileAttributes.Hidden);
			return (IImageComparer)Activator.CreateInstance(typeof(T), new[] { databasePath });
		}
	}
}