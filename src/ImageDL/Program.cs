#define DOWNLOADER
//#define IMAGECOMPARISON

using ImageDL.Classes;
using ImageDL.ImageDownloaders;
using ImageDL.Utilities;
using ImageDL.Utilities.Scraping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
			Console.WriteLine("If you need any help, type 'help:argument name' where argument name is the name of an argument.");
			var downloader = new RedditImageDownloader(args);
			var stillNeedsArgs = true;
			downloader.AllArgumentsSet += () =>
			{
				stillNeedsArgs = false;
				return Task.FromResult(0);
			};

			downloader.AskForArguments();
			while (stillNeedsArgs)
			{
				var input = Console.ReadLine();
				if (input.CaseInsStartsWith("help"))
				{
					downloader.GiveHelp(input.Substring(input.IndexOfAny(new[] { ':', ' ' })).SplitLikeCommandLine());
				}
				else
				{
					downloader.AddArguments(input.SplitLikeCommandLine());
					downloader.AskForArguments();
				}
			}

			await downloader.StartAsync();
			Console.WriteLine("Press any key to close the program.");
			Console.ReadKey();
#endif
#if IMAGECOMPARISON
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < 250; ++i)
			{
				var files = new[]
				{
					@"C:\Users\User\Downloads\New folder (2)\image (1).jpg",
					@"C:\Users\User\Downloads\New folder (2)\image (2).jpg"
				}.Select(x =>
				{
					using (var s = File.Open(x, FileMode.Open))
					using (var bm = new Bitmap(s))
					{
						var md5hash = ImageComparer.CalculateMD5Hash(s);
						return new ImageDetails(new Uri(x), new FileInfo(x), bm);
					}
				}).ToList();
				var sim = ImageComparer.CompareImages(details[0], details[1], 1);
			}
			sw.Stop();
			Console.WriteLine($"MS: {sw.ElapsedMilliseconds}");
			Console.ReadKey();
#endif
		}
	}
}