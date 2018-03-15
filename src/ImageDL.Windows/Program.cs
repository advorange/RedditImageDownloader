using ImageDL.Classes.ImageComparers;
using ImageDL.Classes.ImageDownloaders;
using ImageDL.Utilities;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ImageDL.Windows
{
	public class Program
	{
		const int bufferSize = 1024;
		const string EXIT = "-1";

		public static async Task Main(string[] args)
		{
			Console.SetIn(new StreamReader(Console.OpenStandardInput(bufferSize), Console.InputEncoding, false, bufferSize));
			Console.OutputEncoding = Encoding.UTF8;

			//TODO: work on this
			switch (Console.ReadLine())
			{
				case "1":
				{
					string line = null;
					do
					{
						var downloader = new RedditImageDownloader(new ImageComparer<WindowsImageDetails>());
						if (line != null)
						{
							downloader.SetArguments(line.SplitLikeCommandLine());
						}
						while (!downloader.AllArgumentsSet)
						{
							downloader.AskForArguments();
							downloader.SetArguments(Console.ReadLine().SplitLikeCommandLine());
						}
						await downloader.StartAsync().ConfigureAwait(false);

						Console.WriteLine($"Type '{EXIT}' to close the program, otherwise type anything else to run it again.");
					} while ((line = Console.ReadLine()) != EXIT);
					return;
				}
				case "2":
				{
					DirectoryInfo directory = null;
					while (!Directory.Exists(directory?.FullName))
					{
						Console.WriteLine("Enter a valid directory:");
						try
						{
							directory = new DirectoryInfo(Console.ReadLine());
						}
						catch
						{
							Console.WriteLine("Invalid directory provided.");
						}
					}

					Console.WriteLine("Provide the arguments to use for each directory:");
					var line = Console.ReadLine();

					foreach (var dir in directory.GetDirectories())
					{
						var downloader = new RedditImageDownloader(new ImageComparer<WindowsImageDetails>());
						if (line != null)
						{
							downloader.SetArguments(line.SplitLikeCommandLine());
							downloader.Subreddit = dir.Name;
							downloader.Directory = dir.FullName;
						}
						while (!downloader.AllArgumentsSet)
						{
							downloader.AskForArguments();
							downloader.SetArguments(Console.ReadLine().SplitLikeCommandLine());
						}
						await downloader.StartAsync().ConfigureAwait(false);
					}

					Console.WriteLine("Finished updating the directories.");
					Console.ReadLine();
					return;
				}
			}
		}
	}
}