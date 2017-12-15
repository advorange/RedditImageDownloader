using ImageDL.ImageDownloaders.RedditDownloader;
using ImageDL.Utilities;
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
			var arguments = new RedditImageDownloaderArguments(args);
			while (!arguments.IsReady)
			{
				arguments.AskForArguments();
				arguments.SetArguments(Console.ReadLine().SplitLikeCommandLine());
			}
			await new RedditImageDownloader().StartAsync(arguments);
			Console.WriteLine("Press any key to close the program.");
			Console.ReadKey();
			//TODO: image comparison by hashes then similarity?
		}
	}
}