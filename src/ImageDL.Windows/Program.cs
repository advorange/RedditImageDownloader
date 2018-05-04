using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading;
using ImageDL.Classes.MethodRunners;
using ImageDL.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ImageDL.Windows
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => IOUtils.LogUncaughtException(e.ExceptionObject);
			ConsoleUtils.LogTimeAndCaller = false;
			ConsoleUtils.RemoveMarkdown = false;
			ConsoleUtils.RemoveDuplicateNewLines = true;
			Console.SetIn(new StreamReader(Console.OpenStandardInput(1024), Console.InputEncoding, false, 1024));
			Console.OutputEncoding = Encoding.UTF8;

			//Services used when downloading. Client should be constant, but comparer should be discarded after each use.
			var services = new DefaultServiceProviderFactory().CreateServiceProvider(new ServiceCollection()
				.AddSingleton<IImageDownloaderClient, ImageDownloaderClient>()
				.AddTransient<IImageComparer, WindowsImageComparer>());

			//If there are any args, try to find a matching method to run
			switch (args != null && args.Length > 0 ? args[0] : null)
			{
				case "RedditDirUpdate":
					await new RedditDirectoryRunner().RunAsync(services).CAF();
					return;
				default:
					await new DefaultRunner().RunAsync(services).CAF();
					return;
			}
		}
	}
}