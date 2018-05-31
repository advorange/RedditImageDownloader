using System;
using System.Diagnostics;
using System.IO;
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
		//TODO: rework this main method since most can be put into where .Net Core and Framework will intersect
		//TODO: appropriately name the recycling bin method since it causes exception in .net core
		public static async Task Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => IOUtils.LogUncaughtException(e.ExceptionObject);
			Console.SetIn(new StreamReader(Console.OpenStandardInput(1024), Console.InputEncoding, false, 1024));
			Console.OutputEncoding = Encoding.UTF8;
			ConsoleUtils.PrintingFlags = 0
				| ConsolePrintingFlags.Print
				| ConsolePrintingFlags.RemoveDuplicateNewLines;

			//Services used when downloading. Client should be constant, but comparer should be discarded after each use.
			var services = new DefaultServiceProviderFactory().CreateServiceProvider(new ServiceCollection()
				.AddSingleton<IDownloaderClient, DownloaderClient>()
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