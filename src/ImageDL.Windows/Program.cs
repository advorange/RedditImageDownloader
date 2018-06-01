using System.Threading.Tasks;
using ImageDL.Classes.ImageDownloading;
using ImageDL.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ImageDL.Windows
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			//Services used when downloading. Client should be constant, but comparer should be discarded after each use.
			var services = new DefaultServiceProviderFactory().CreateServiceProvider(new ServiceCollection()
				.AddSingleton<IDownloaderClient, DownloaderClient>()
				.AddTransient<IImageComparer, WindowsImageComparer>());
			await new ImageDL().RunFromArguments(services, args);
		}
	}
}