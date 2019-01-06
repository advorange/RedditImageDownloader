using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesSettingParser.Implementation;
using AdvorangesSettingParser.Implementation.Instance;
using AdvorangesSettingParser.Utils;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.ImageComparing;
using ImageDL.Classes.ImageDownloading;
using ImageDL.Classes.ImageDownloading.Reddit;
using ImageDL.Interfaces;
using Microsoft.Extensions.DependencyInjection;

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
		public static ImmutableDictionary<string, Type> ImageDownloaders { get; set; } = new Dictionary<string, Type>(
			typeof(PostDownloader).Assembly.DefinedTypes
			.Where(x => !x.IsAbstract && typeof(PostDownloader).IsAssignableFrom(x))
			.ToDictionary(x => x.GetCustomAttribute<DownloaderNameAttribute>().Name, x => x.AsType()))
			.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);
		/// <summary>
		/// The key users can press to cancel a downloader.
		/// </summary>
		public ConsoleKey CancelKey { get; set; } = ConsoleKey.Escape;

		/// <summary>
		/// Creates an instance of <see cref="ImageDL"/>.
		/// </summary>
		public ImageDL()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				IOUtils.LogUncaughtException(e.ExceptionObject);
				ConsoleUtils.WriteLine(((Exception)e.ExceptionObject).Message);
				Console.ReadLine();
			};
			Console.SetIn(new StreamReader(Console.OpenStandardInput(1024), Console.InputEncoding, false, 1024));
			Console.OutputEncoding = Encoding.UTF8;
			Console.Title = "ImageDL";
			ConsoleUtils.PrintingFlags = 0
				| ConsolePrintingFlags.Print
				| ConsolePrintingFlags.RemoveDuplicateNewLines;
		}

		/// <summary>
		/// Creates a service provider using the specified image comparer.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IServiceProvider CreateServices<T>() where T : IImageComparer
		{
			//Services used when downloading. Client should be constant, but comparer should be discarded after each use.
			return new ServiceCollection()
				.AddSingleton<IDownloaderClient, DownloaderClient>()
				.AddSingleton<IImageComparerFactory, ImageComparerFactory<T>>()
				.BuildServiceProvider();
		}
		/// <summary>
		/// Runs a method from the passed in arguments.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="args"></param>
		/// <param name="prefixes">The default prefixes are </param>
		/// <returns></returns>
		public virtual Task RunFromArguments(IServiceProvider services, string[] args, IEnumerable<string> prefixes = default)
		{
			var parsedArgs = ArgumentMappingUtils.Parse(new ParseArgs(args, new[] { '"' }, new[] { '"' }), prefixes ?? SettingParser.DefaultPrefixes);
			var dictionary = parsedArgs.ToDictionary(x => x.Setting, x => x.Args, StringComparer.OrdinalIgnoreCase);
			switch (dictionary.TryGetValue("method", out var val) ? val : null)
			{
				case "RedditDirectories":
					return RedditDirectories(services);
				default:
					return Default(services);
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
					ConsoleUtils.WriteLine(downloader.SettingParser.GetNeededSettings().FormatNeededSettings());
					ConsoleUtils.WriteLine(downloader.SettingParser.Parse(Console.ReadLine()).ToString());
				}
				await DoMethodWithCancelOption(t => downloader.DownloadAsync(services, t), CancelKey);
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
					ConsoleUtils.WriteLine(downloader.SettingParser.GetNeededSettings().FormatNeededSettings());
					ConsoleUtils.WriteLine(downloader.SettingParser.Parse(Console.ReadLine()).ToString());
				}
				await DoMethodWithCancelOption(t => downloader.DownloadAsync(services, t), CancelKey);
			}
		}
		/// <summary>
		/// Runs the task, but allows it to be canceled by the user when they press a specified key.
		/// </summary>
		/// <param name="f"></param>
		/// <param name="cancelKey"></param>
		/// <param name="catchException"></param>
		/// <returns></returns>
		protected static async Task DoMethodWithCancelOption(
			Func<CancellationToken, Task> f,
			ConsoleKey cancelKey = ConsoleKey.Escape,
			bool catchException = true)
		{
			var running = true;
			var source = new CancellationTokenSource();
			_ = Task.Run(() => //This thread has to be nonblocking
			{
				while (running)
				{
					//Console.KeyAvailable to not have Console.ReadKey eat the next key after the downloader is complete
					//True in Console.ReadKey to not have the key show up in the console when pressed
					if (Console.KeyAvailable && Console.ReadKey(true).Key == cancelKey)
					{
						source.Cancel();
					}
				}
			});

			try
			{
				ConsoleUtils.WriteLine("Press esc to cancel and stop downloading posts.");
				await f(source.Token).CAF();
				running = false;
			}
			catch (OperationCanceledException) when (catchException)
			{
				ConsoleUtils.WriteLine($"The downloading was canceled.{Environment.NewLine}");
			}
		}
		/// <summary>
		/// Makes sure the input directory is valid.
		/// </summary>
		/// <returns></returns>
		protected static DirectoryInfo GetDirectory()
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
		/// <summary>
		/// Gets a valid downloader type.
		/// </summary>
		/// <returns></returns>
		protected static Type GetDownloaderType()
		{
			ConsoleUtils.WriteLine($"Pick from one of the following downloaders: '{string.Join("', '", ImageDownloaders.Keys)}'");
			while (true)
			{
				if (ImageDownloaders.TryGetValue(Console.ReadLine(), out var downloaderType))
				{
					return downloaderType;
				}
				ConsoleUtils.WriteLine("Invalid downloader provided.");
			}
		}
	}
}