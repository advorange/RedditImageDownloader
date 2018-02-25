using ImageDL.ImageDownloaders;
using ImageDL.Utilities;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ImageDL
{
	public class Program
	{
		const int bufferSize = 1024;
		const string EXIT = "-1";

		public static async Task Main(string[] args)
		{
			Console.SetIn(new StreamReader(Console.OpenStandardInput(bufferSize), Console.InputEncoding, false, bufferSize));
			Console.OutputEncoding = Encoding.UTF8;

			do
			{
				var downloader = new RedditImageDownloader();
				while (!downloader.AllArgumentsSet)
				{
					downloader.AskForArguments();
					downloader.SetArguments(Console.ReadLine().SplitLikeCommandLine());
				}
				await downloader.StartAsync().ConfigureAwait(false);

				Console.WriteLine($"Type '{EXIT}' to close the program, otherwise type anything else to run it again.");
			} while (Console.ReadLine() != EXIT);
		}
	}
}