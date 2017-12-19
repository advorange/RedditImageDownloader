using System.Threading.Tasks;

namespace ImageDL.ImageDownloaders
{
	public interface IImageDownloader
	{
		bool IsReady { get; }

		Task StartAsync();
		void SetArguments(string[] args);
		void AskForArguments();
	}
}
