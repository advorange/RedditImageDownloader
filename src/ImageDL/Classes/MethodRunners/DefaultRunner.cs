using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.ImageDownloading;
using ImageDL.Interfaces;

namespace ImageDL.Classes.MethodRunners
{
	/// <summary>
	/// Downloads images from a specified website.
	/// </summary>
	public sealed class DefaultRunner : IMethodRunner
	{
		private SortedDictionary<string, Type> _ImageDownloaders = GetDownloaders();

		/// <inheritdoc />
		public async Task RunAsync(IServiceProvider services)
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
				var running = true;
				var f = Task.Run(() =>
				{
					while (running)
					{
						if (Console.ReadKey().Key == ConsoleKey.X)
						{
							source.Cancel();
						}
					}
				});

				try
				{
					await downloader.DownloadAsync(services, source.Token).CAF();
					running = false;
				}
				catch (OperationCanceledException)
				{
					ConsoleUtils.WriteLine($"The downloading was canceled.{Environment.NewLine}");
				}
			}
		}
		private Type GetDownloaderType()
		{
			ConsoleUtils.WriteLine($"Pick from one of the following downloaders: '{String.Join("', '", _ImageDownloaders.Keys)}'");
			while (true)
			{
				if (_ImageDownloaders.TryGetValue(Console.ReadLine().Trim(), out var downloaderType))
				{
					return downloaderType;
				}
				ConsoleUtils.WriteLine($"Invalid downloader; pick from one of the following downloaders: '{String.Join("', '", _ImageDownloaders.Keys)}'");
			}
		}
		private static SortedDictionary<string, Type> GetDownloaders()
		{
			var types = typeof(PostDownloader).Assembly.DefinedTypes;
			var dls = types.Where(x => !x.IsAbstract && typeof(PostDownloader).IsAssignableFrom(x));
			var dict = dls.ToDictionary(x => x.GetCustomAttribute<DownloaderNameAttribute>().Name, x => x.AsType());
			return new SortedDictionary<string, Type>(dict, StringComparer.OrdinalIgnoreCase);
		}
	}
}