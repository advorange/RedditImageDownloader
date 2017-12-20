using ImageDL.ImageDownloaders;
using ImageDL.Utilities;
using ImageDL.Utilities.Scraping;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ImageDL
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;
			var downloader = new RedditImageDownloader(args);
			while (!downloader.IsReady)
			{
				downloader.AskForArguments();
				downloader.AddArguments(Console.ReadLine().SplitLikeCommandLine());
			}
			await downloader.StartAsync();
			Console.WriteLine("Press any key to close the program.");
			Console.ReadKey();
			//TODO: image comparison by hashes then similarity?
		}
	}
}