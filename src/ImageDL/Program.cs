#define DOWNLOADER
//#define IMAGECOMPARISON

using ImageDL.Classes;
using ImageDL.ImageDownloaders;
using ImageDL.Utilities;
using ImageDL.Utilities.Scraping;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ImageDL
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;
#if DOWNLOADER
			var downloader = new RedditImageDownloader(args);
			while (!downloader.IsReady)
			{
				downloader.AskForArguments();
				downloader.AddArguments(Console.ReadLine().SplitLikeCommandLine());
			}
			await downloader.StartAsync();
			Console.WriteLine("Press any key to close the program.");
			Console.ReadKey();
#endif
#if IMAGECOMPARISON
			var files = new[] { @"C:\Users\Nate\Downloads\New folder (2)\imageA.png", @"C:\Users\Nate\Downloads\New folder (2)\imageB.png" };
			var details = new ImageDetails[files.Length];
			for (int i = 0; i < files.Length; ++i)
			{
				var uri = new Uri(files[i]);
				var fileInfo = new FileInfo(files[i]);
				using (var s = File.Open(files[i], FileMode.Open))
				using (var bm = new Bitmap(s))
				{
					details[i] = new ImageDetails(uri, fileInfo, ImageComparer.CalculateBoolHash(bm));
				}
			}
			var sim = ImageComparer.CompareImages(details[0], details[1], 1);
			Console.ReadKey();
#endif
		}
	}
}