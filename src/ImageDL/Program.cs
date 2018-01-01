//#define DOWNLOADER
//#define IMAGECOMPARISON
#define URLRESPONSETEST

using ImageDL.Classes;
using ImageDL.ImageDownloaders;
using ImageDL.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

			await downloader.StartAsync().ConfigureAwait(false);
			Console.WriteLine("Press any key to close the program.");
			Console.ReadKey();
#endif
#if IMAGECOMPARISON
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < 250; ++i)
			{
				var details = new[]
				{
					@"C:\Users\User\Downloads\New folder (2)\image (1).jpg",
					@"C:\Users\User\Downloads\New folder (2)\image (2).jpg"
				}.Select(x =>
				{
					using (var s = File.Open(x, FileMode.Open))
					using (var bm = new Bitmap(s))
					{
						var md5hash = s.Hash<MD5>();
						return new ImageDetails(new Uri(x), new FileInfo(x), bm, 64);
					}
				}).ToList();
				var sim = details[0].Equals(details[1], 1);
			}
			sw.Stop();
			Console.WriteLine($"MS: {sw.ElapsedMilliseconds}");
			Console.ReadKey();
#endif
#if URLRESPONSETEST
			var gatherer = await UriImageGatherer.CreateGatherer(new Uri("")).ConfigureAwait(false);
			//var downloader = new RedditImageDownloader();
			//await downloader.DownloadImageAsync(null, new Uri("")).ConfigureAwait(false);
#endif
		}
	}
}