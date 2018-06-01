using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.ImageDownloading;
using ImageDL.Classes.ImageDownloading.Reddit;
using ImageDL.Classes.SettingParsing;

namespace ImageDL
{
	/// <summary>
	/// Most of the logic for starting a downloader through user input is in this class.
	/// </summary>
	public class ImageDL
	{
		/// <summary>
		/// Various downloaders used to download things.
		/// </summary>
		private static SortedDictionary<string, Type> _ImageDownloaders = new SortedDictionary<string, Type>(typeof(PostDownloader)
			.Assembly.DefinedTypes
			.Where(x => !x.IsAbstract && typeof(PostDownloader).IsAssignableFrom(x))
			.ToDictionary(x => x.GetCustomAttribute<DownloaderNameAttribute>().Name, x => x.AsType()), StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// The key users can press to cancel a downloader.
		/// </summary>
		public ConsoleKey CancelKey { get; set; } = ConsoleKey.Escape;

		/// <summary>
		/// Creates an instance of <see cref="ImageDL"/>.
		/// </summary>
		public ImageDL()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => IOUtils.LogUncaughtException(e.ExceptionObject);
			Console.SetIn(new StreamReader(Console.OpenStandardInput(1024), Console.InputEncoding, false, 1024));
			Console.OutputEncoding = Encoding.UTF8;
			ConsoleUtils.PrintingFlags = 0
				| ConsolePrintingFlags.Print
				| ConsolePrintingFlags.RemoveDuplicateNewLines;
		}

		/// <summary>
		/// Runs a method from the passed in arguments.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="args"></param>
		/// <param name="prefixes">The default prefixes are </param>
		/// <returns></returns>
		public virtual async Task RunFromArguments(IServiceProvider services, string[] args, string[] prefixes = default)
		{
			var parsedArgs = SettingParser.Parse(args, prefixes ?? SettingParser.DefaultPrefixes.ToArray());
			switch (parsedArgs.TryGetValue("method", out var val) ? val : null)
			{
				case "RedditDirectories":
					await RedditDirectories(services).CAF();
					return;
				default:
					await Default(services).CAF();
					return;
			}
		}
		/// <summary>
		/// Lets the user pick what downloader, and then pass in arguments and run it.
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public async Task Default(IServiceProvider services)
		{
			while (true)
			{
				var downloader = (PostDownloader)Activator.CreateInstance(GetDownloaderType());
				while (!downloader.CanStart)
				{
					ConsoleUtils.WriteLine(downloader.SettingParser.GetNeededSettings());
					ConsoleUtils.WriteLine(downloader.SettingParser.Parse(Console.ReadLine()).ToString());
				}

				var source = new CancellationTokenSource();
				await DoMethodWithCancelOption(services, source, async (s, t) =>
				{
					await downloader.DownloadAsync(s, t).CAF();
				}, CancelKey);
			}
		}
		/// <summary>
		/// Updates the various directories using their names as subreddits.
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public async Task RedditDirectories(IServiceProvider services)
		{
			ConsoleUtils.WriteLine("Provide the arguments to use for each directory:");
			var arguments = Console.ReadLine();

			foreach (var dir in GetDirectory().GetDirectories())
			{
				var downloader = new RedditPostDownloader();
				if (arguments != null)
				{
					ConsoleUtils.WriteLine(downloader.SettingParser.Parse(arguments).ToString());
					downloader.Subreddit = dir.Name;
					downloader.SavePath = dir.FullName;
				}
				while (!downloader.CanStart)
				{
					ConsoleUtils.WriteLine(downloader.SettingParser.GetNeededSettings());
					ConsoleUtils.WriteLine(downloader.SettingParser.Parse(Console.ReadLine()).ToString());
				}

				var source = new CancellationTokenSource();
				await DoMethodWithCancelOption(services, source, async (s, t) =>
				{
					await downloader.DownloadAsync(s, t).CAF();
				}, CancelKey);
			}
		}
		/// <summary>
		/// Runs the task, but allows it to be canceled by the user when they press a specified key.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="source"></param>
		/// <param name="f"></param>
		/// <param name="cancelKey"></param>
		/// <param name="catchException"></param>
		/// <returns></returns>
		protected static async Task DoMethodWithCancelOption(
			IServiceProvider services,
			CancellationTokenSource source,
			Func<IServiceProvider, CancellationToken, Task> f,
			ConsoleKey cancelKey = ConsoleKey.Escape,
			bool catchException = true)
		{
			var running = true;
			var c = Task.Run(() => //This thread has to be nonblocking
			{
				while (running)
				{
					//Console.KeyAvailable to not have Console.ReadKey take eat the next key after the downloader is complete
					//True in Console.ReadKey to not have x show up in the console when pressed
					if (Console.KeyAvailable && Console.ReadKey(true).Key == cancelKey)
					{
						source.Cancel();
					}
				}
			});

			if (catchException)
			{
				try
				{
					await f(services, source.Token).CAF();
					running = false;
				}
				catch (OperationCanceledException) //Assume this is something we want to catch and not some random uncaught exception
				{
					ConsoleUtils.WriteLine($"The downloading was canceled.{Environment.NewLine}");
				}
			}
			else
			{
				await f(services, source.Token).CAF();
				running = false;
			}
		}
		/// <summary>
		/// Makes sure the input directory is valid.
		/// </summary>
		/// <returns></returns>
		protected static DirectoryInfo GetDirectory()
		{
			ConsoleUtils.WriteLine("Enter a valid directory:");
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
			return GetDirectory();
		}
		/// <summary>
		/// Gets a valid downloader type.
		/// </summary>
		/// <returns></returns>
		protected static Type GetDownloaderType()
		{
			ConsoleUtils.WriteLine($"Pick from one of the following downloaders: '{String.Join("', '", _ImageDownloaders.Keys)}'");
			if (_ImageDownloaders.TryGetValue(Console.ReadLine(), out var downloaderType))
			{
				return downloaderType;
			}
			ConsoleUtils.WriteLine("Invalid downloader provided.");
			return GetDownloaderType();
		}
	}
}