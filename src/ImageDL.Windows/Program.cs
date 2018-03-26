using AdvorangesUtils;
using ImageDL.Classes.ImageDownloaders.DeviantArt;
using ImageDL.Classes.ImageDownloaders.Reddit;
using ImageDL.Interfaces;
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
			.ToDictionary(x => x.Name.FormatTitle().Split(' ')[0], x => x.AsType(), StringComparer.OrdinalIgnoreCase);
		public static List<Func<Task>> Methods = new List<Func<Task>>
		{
			Single, UpdateRedditDirectory,
		};

		public static async Task Main(string[] args)
		{
			Console.SetIn(new StreamReader(Console.OpenStandardInput(BUFFER_SIZE), Console.InputEncoding, false, BUFFER_SIZE));
			Console.OutputEncoding = Encoding.UTF8;
			Console.WriteLine($"Pick from one of the following methods: '{String.Join("', '", Methods.Select(x => x.Method.Name))}'");
			do
			{
				var line = Console.ReadLine();
				if (Methods.SingleOrDefault(x => x.Method.Name.CaseInsEquals(line)) is Func<Task> t)
				{
					await t().CAF();
					Console.WriteLine($"Method finished. Type '{EXIT}' to exit the program, otherwise type anything else to run it again.");
				}
				else
				{
					Console.WriteLine($"Invalid method. Pick from one of the following methods: '{String.Join("', '", Methods.Select(x => x.Method.Name))}'");
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
			await downloader.StartAsync().CAF();
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
				await downloader.StartAsync().CAF();
			}
		}

		private static Type GetDownloaderType()
		{
			Console.WriteLine($"Pick from one of the following downloaders: '{String.Join("', '", ImageDownloaders.Keys)}'");
			while (true)
			{
				if (!ImageDownloaders.TryGetValue(Console.ReadLine(), out var downloaderType))
				{
					Console.WriteLine($"Invalid downloader; pick from one of the following downloaders: '{String.Join("', '", ImageDownloaders.Keys)}'");
					continue;
				}
				return downloaderType;
			}
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