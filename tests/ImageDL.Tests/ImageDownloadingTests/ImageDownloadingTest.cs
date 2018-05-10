using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading;
using ImageDL.Interfaces;
using ImageDL.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageDL.Tests.ImageDownloadingTests
{
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

		public async Task Downloader_Test<T>(string specificArgs) where T : IImageDownloader, new()
		{
			var services = new DefaultServiceProviderFactory().CreateServiceProvider(new ServiceCollection()
				.AddSingleton<IImageDownloaderClient, ImageDownloaderClient>()
				.AddTransient<IImageComparer, WindowsImageComparer>());
			var downloader = new T();

			var genericArgsResult = downloader.SettingParser.Parse(GenerateGenericArgs<T>());
			Assert.AreEqual(0, genericArgsResult.Errors.Length + genericArgsResult.UnusedParts.Length);
			var specificArgsResult = downloader.SettingParser.Parse(specificArgs);
			Assert.AreEqual(0, specificArgsResult.Errors.Length + specificArgsResult.UnusedParts.Length);

			var downloaderResults = await downloader.StartAsync(services).CAF();
			Assert.AreEqual(AMOUNT, downloaderResults.GatheredPostCount);
		}
		public string GetDirectory<T>() where T : IImageDownloader
		{
			return Path.Combine(Directory.GetCurrentDirectory(), typeof(T).Name);
		}
		public string GenerateGenericArgs<T>() where T : IImageDownloader
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