using System;
using System.IO;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading.Reddit;
using ImageDL.Interfaces;

namespace ImageDL.Classes.MethodRunners
{
	/// <summary>
	/// Treats every subdirectory as the name of a subreddit, and downloads the images from those.
	/// </summary>
	public sealed class RedditDirectoryRunner : IMethodRunner
	{
		/// <inheritdoc />
		public async Task RunAsync(IServiceProvider services)
		{
			ConsoleUtils.WriteLine("Provide the arguments to use for each directory:");
			var arguments = Console.ReadLine();

			foreach (var dir in GetDirectory().GetDirectories())
			{
				var downloader = new RedditPostDownloader();
				if (arguments != null)
				{
					ConsoleUtils.WriteLine(downloader.SettingParser.Parse(arguments).ToString());
					downloader.Subreddit = dir.Name;
					downloader.SavePath = dir.FullName;
				}
				while (!downloader.CanStart)
				{
					ConsoleUtils.WriteLine(downloader.SettingParser.GetNeededSettings());
					ConsoleUtils.WriteLine(downloader.SettingParser.Parse(Console.ReadLine()).ToString());
				}
				await downloader.DownloadAsync(services).CAF();
			}
		}
		private DirectoryInfo GetDirectory()
		{
			ConsoleUtils.WriteLine("Enter a valid directory:");
			while (true)
			{
				try
				{
					var directory = new DirectoryInfo(Console.ReadLine());
					if (directory.Exists)
					{
						return directory;
					}
				}
				catch { }
				ConsoleUtils.WriteLine("Invalid directory provided.");
			}
		}
	}
}