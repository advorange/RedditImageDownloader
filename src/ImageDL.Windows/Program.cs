using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading;
using ImageDL.Classes.ImageDownloading.Reddit;
using ImageDL.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ImageDL.Windows
{
	public class Program
	{
		public const int BUFFER_SIZE = 1024;
		public const string EXIT = "-1";

		public static Dictionary<string, Type> ImageDownloaders = typeof(IImageDownloader).Assembly.DefinedTypes
			.Where(x => !x.IsAbstract && typeof(IImageDownloader).IsAssignableFrom(x))
			.ToDictionary(x => ((IImageDownloader)Activator.CreateInstance(x)).Name, x => x.AsType(), StringComparer.OrdinalIgnoreCase);
		public static List<Func<IServiceProvider, Task>> Methods = new List<Func<IServiceProvider, Task>>
		{
			Single, UpdateRedditDirectory,
		};

		public static async Task Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => IOUtils.LogUncaughtException(e.ExceptionObject);
			ConsoleUtils.LogTimeAndCaller = false;
			ConsoleUtils.RemoveMarkdown = false;
			ConsoleUtils.RemoveDuplicateNewLines = true;
			Console.SetIn(new StreamReader(Console.OpenStandardInput(BUFFER_SIZE), Console.InputEncoding, false, BUFFER_SIZE));
			Console.OutputEncoding = Encoding.UTF8;

			//Services used when downloading. Client should be constant, but comparer should be discarded after each use.
			var services = new DefaultServiceProviderFactory().CreateServiceProvider(new ServiceCollection()
				.AddSingleton<IImageDownloaderClient, ImageDownloaderClient>()
				.AddTransient<IImageComparer, WindowsImageComparer>());
			ConsoleUtils.WriteLine($"Pick from one of the following methods: '{String.Join("', '", Methods.Select(x => x.Method.Name))}'");
			while (true)
			{
				var line = Console.ReadLine().Trim();
				if (line == EXIT)
				{
					break;
				}
				else if (Methods.SingleOrDefault(x => x.Method.Name.CaseInsEquals(line)) is Func<IServiceProvider, Task> t)
				{
					await t(services).CAF();
					ConsoleUtils.WriteLine($"Method finished. Type '{EXIT}' to exit the program, otherwise type anything else to run it again.");
					continue;
				}
				ConsoleUtils.WriteLine($"Invalid method. Pick from one of the following methods: '{String.Join("', '", Methods.Select(x => x.Method.Name))}'");
			}
		}

		private static async Task Single(IServiceProvider services)
		{
			var downloader = (IImageDownloader)Activator.CreateInstance(GetDownloaderType());
			while (!downloader.CanStart)
			{
				ConsoleUtils.WriteLine(downloader.SettingParser.GetNeededSettings());
				ConsoleUtils.WriteLine(downloader.SettingParser.Parse(Console.ReadLine()).ToString());
			}
			await downloader.StartAsync(services).CAF();
		}
		private static async Task UpdateRedditDirectory(IServiceProvider services)
		{
			ConsoleUtils.WriteLine("Provide the arguments to use for each directory:");
			var arguments = Console.ReadLine();

			foreach (var dir in GetDirectory().GetDirectories())
			{
				var downloader = new RedditImageDownloader();
				if (arguments != null)
				{
					ConsoleUtils.WriteLine(downloader.SettingParser.Parse(arguments).ToString());
					downloader.Subreddit = dir.Name;
					downloader.Directory = dir.FullName;
				}
				while (!downloader.CanStart)
				{
					ConsoleUtils.WriteLine(downloader.SettingParser.GetNeededSettings());
					ConsoleUtils.WriteLine(downloader.SettingParser.Parse(Console.ReadLine()).ToString());
				}
				await downloader.StartAsync(services).CAF();
			}
		}

		private static Type GetDownloaderType()
		{
			ConsoleUtils.WriteLine($"Pick from one of the following downloaders: '{String.Join("', '", ImageDownloaders.Keys)}'");
			while (true)
			{
				if (ImageDownloaders.TryGetValue(Console.ReadLine().Trim(), out var downloaderType))
				{
					return downloaderType;
				}
				ConsoleUtils.WriteLine($"Invalid downloader; pick from one of the following downloaders: '{String.Join("', '", ImageDownloaders.Keys)}'");
			}
		}
		private static DirectoryInfo GetDirectory()
		{
			ConsoleUtils.WriteLine("Enter a valid directory:");
			while (true)
			{
				try
				{
					var directory = new DirectoryInfo(Console.ReadLine());
					if (directory.Exists)
					{
						return directory;
					}
				}
				catch { }
				ConsoleUtils.WriteLine("Invalid directory provided.");
			}
		}
	}
}