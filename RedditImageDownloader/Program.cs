using System;

namespace RedditImageDownloader
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var downloader = new RedditImageDownloader(args);
			while (!downloader.IsReady)
			{
				downloader.AskForArguments();
				downloader.SetArguments(Console.ReadLine().SplitLikeCommandLine());
			}
			downloader.DownloadImages();
			Console.WriteLine("Press any key to close the program.");
			Console.ReadKey();
		}
	}
}