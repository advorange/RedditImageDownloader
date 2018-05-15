using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading;
using ImageDL.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageDL.Tests.ImageDownloadingTests
{
	//TODO: implement this for every downloader
	//TODO: also implement unit tests for every image gatherer
	//Not sure exactly how to unit test a downloader considering it relies on posts from the internet which are random
	//Maybe just have the downloader verify the post count and not the cached/deleted/link counts
	[TestClass]
	public class ImageDownloadingTest
	{
		private const int AMOUNT = 5;
		private const int MIN_SCORE = 25;
		private const int MIN_WIDTH = 350;
		private const int MIN_HEIGHT = 350;
		private const int MAX_AGE = int.MaxValue;

		private async Task Downloader_Test<T>(string specificArgs) where T : IImageDownloader, new()
		{
			var client = new ImageDownloaderClient();
			var downloader = new T();

			var genericArgsResult = downloader.SettingParser.Parse(GenerateGenericArgs<T>());
			Assert.AreEqual(0, genericArgsResult.Errors.Length + genericArgsResult.UnusedParts.Length);
			var specificArgsResult = downloader.SettingParser.Parse(specificArgs);
			Assert.AreEqual(0, specificArgsResult.Errors.Length + specificArgsResult.UnusedParts.Length);

			var list = new List<IPost>();
			await downloader.GatherPostsAsync(client, list).CAF();
			Assert.AreEqual(AMOUNT, list.Count);
		}
		private string GetDirectory<T>() where T : IImageDownloader
		{
			return Path.Combine(Directory.GetCurrentDirectory(), typeof(T).Name);
		}
		private string GenerateGenericArgs<T>() where T : IImageDownloader
		{
			return $"-cd" +
				$"-dir \"{GetDirectory<T>()}\"" +
				$"-amt {AMOUNT}" +
				$"-ms {MIN_SCORE}" +
				$"-mh {MIN_HEIGHT}" +
				$"-mw {MIN_WIDTH}" +
				$"-age {MAX_AGE}" +
				$"-sim 990" +
				$"-csi" +
				$"-icpt 4";
		}
	}
}