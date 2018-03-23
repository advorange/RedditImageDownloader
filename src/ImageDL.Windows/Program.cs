﻿using ImageDL.Classes.ImageDownloaders;
using ImageDL.Core.Utilities;
using ImageDL.Interfaces;
using ImageDL.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDL.Windows
{
	public class Program
	{
		public const int BUFFER_SIZE = 1024;
		public const string EXIT = "-1";

		public static Dictionary<string, Type> ImageDownloaders = typeof(IImageDownloader).Assembly.DefinedTypes
			.Where(x => !x.IsAbstract && typeof(IImageDownloader).IsAssignableFrom(x))
			.Select(x => x.AsType())
			.ToDictionary(x => x.Name.FormatTitle().Split(' ')[0], x => x, StringComparer.OrdinalIgnoreCase);
		public static Dictionary<string, Func<Task>> Methods = new Dictionary<string, Func<Task>>(StringComparer.OrdinalIgnoreCase)
		{
			{ nameof(Single), Single },
			{ nameof(UpdateRedditDirectory), UpdateRedditDirectory },
		};

		public static async Task Main(string[] args)
		{
			Console.SetIn(new StreamReader(Console.OpenStandardInput(BUFFER_SIZE), Console.InputEncoding, false, BUFFER_SIZE));
			Console.OutputEncoding = Encoding.UTF8;

			var test = new DanbooruImageDownloader
			{
				TagString = "highres",
				AmountToDownload = 1000,
				MaxDaysOld = 100,
			};
			await test.StartAsync().ConfigureAwait(false);

			Console.WriteLine($"Pick from one of the following methods: '{String.Join("', '", Methods.Keys)}'");
			do
			{
				if (Methods.TryGetValue(Console.ReadLine(), out var t))
				{
					await t().ConfigureAwait(false);
					Console.WriteLine($"Method finished. Type '{EXIT}' to exit the program, otherwise type anything else to run it again.");
				}
				else
				{
					Console.WriteLine($"Invalid method. Pick from one of the following methods: '{String.Join("', '", Methods.Keys)}'");
				}
			} while (Console.ReadLine() != EXIT);
		}

		private static async Task Single()
		{
			var downloader = CreateDownloader(GetDownloaderType());
			while (!downloader.AllArgumentsSet)
			{
				downloader.AskForArguments();
				downloader.SetArguments(Console.ReadLine().SplitLikeCommandLine());
			}
			await downloader.StartAsync().ConfigureAwait(false);
		}
		private static async Task UpdateRedditDirectory()
		{
			Console.WriteLine("Provide the arguments to use for each directory:");
			var arguments = Console.ReadLine();

			foreach (var dir in GetDirectory().GetDirectories())
			{
				var downloader = new RedditImageDownloader
				{
					ImageComparer = new WindowsImageComparer(),
				};
				if (arguments != null)
				{
					downloader.SetArguments(arguments.SplitLikeCommandLine());
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
		}

		private static Type GetDownloaderType()
		{
			Type downloaderType;
			Console.WriteLine($"Pick from one of the following downloaders: '{String.Join("', '", ImageDownloaders.Keys)}'");
			while (!ImageDownloaders.TryGetValue(Console.ReadLine(), out downloaderType))
			{
				Console.WriteLine($"Invalid downloader; pick from one of the following downloaders: '{String.Join("', '", ImageDownloaders.Keys)}'");
				continue;
			}
			return downloaderType;
		}
		private static IImageDownloader CreateDownloader(Type t)
		{
			var downloader = (IImageDownloader)Activator.CreateInstance(t);
			downloader.ImageComparer = new WindowsImageComparer();
			return downloader;
		}
		private static DirectoryInfo GetDirectory()
		{
			Console.WriteLine("Enter a valid directory:");
			DirectoryInfo directory = null;
			while (!Directory.Exists(directory?.FullName))
			{
				try
				{
					directory = new DirectoryInfo(Console.ReadLine());
				}
				catch
				{
					Console.WriteLine("Invalid directory provided.");
				}
			}
			return directory;
		}
	}
}