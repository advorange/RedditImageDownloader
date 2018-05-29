using System.IO;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading;
using ImageDL.Classes.ImageDownloading.AnimePictures;
using ImageDL.Classes.ImageDownloading.Artstation;
using ImageDL.Classes.ImageDownloading.Bcy;
using ImageDL.Classes.ImageDownloading.Booru.Danbooru;
using ImageDL.Classes.ImageDownloading.Booru.Gelbooru;
using ImageDL.Classes.ImageDownloading.Booru.Konachan;
using ImageDL.Classes.ImageDownloading.Booru.Safebooru;
using ImageDL.Classes.ImageDownloading.Booru.Yandere;
using ImageDL.Classes.ImageDownloading.DeviantArt;
using ImageDL.Classes.ImageDownloading.Diyidan;
using ImageDL.Classes.ImageDownloading.Eshuushuu;
using ImageDL.Classes.ImageDownloading.Flickr;
using ImageDL.Classes.ImageDownloading.FourChan;
using ImageDL.Classes.ImageDownloading.Imgur;
using ImageDL.Classes.ImageDownloading.Instagram;
using ImageDL.Classes.ImageDownloading.Pawoo;
using ImageDL.Classes.ImageDownloading.Pinterest;
using ImageDL.Classes.ImageDownloading.Pixiv;
using ImageDL.Classes.ImageDownloading.Reddit;
using ImageDL.Classes.ImageDownloading.Tumblr;
using ImageDL.Classes.ImageDownloading.Twitter;
using ImageDL.Classes.ImageDownloading.Vsco;
using ImageDL.Classes.ImageDownloading.Weibo;
using ImageDL.Enums;
using ImageDL.Interfaces;
using ImageDL.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageDL.Tests.ImageDownloadingTests
{
	//TODO: also implement unit tests for every image gatherer
	//Not sure exactly how to unit test a downloader considering it relies on posts from the internet which are random
	//Maybe just have the downloader verify the post count and not the cached/deleted/link counts
	[TestClass]
	public class ImageGatheringTests
	{
		private const int AMOUNT = 5;
		private const int MIN_SCORE = 0;
		private const int MIN_WIDTH = 100;
		private const int MIN_HEIGHT = 100;
		private const int MAX_AGE = 10000;

		[TestMethod]
		public async Task FourChan_Test()
		{
			await Downloader_Test<FourChanPostDownloader>($"-{nameof(FourChanPostDownloader.Board)} a").CAF();
		}
		[TestMethod]
		public async Task AnimePictures_Test()
		{
			await Downloader_Test<AnimePicturesPostDownloader>($"-{nameof(AnimePicturesPostDownloader.Tags)} blonde").CAF();
		}
		[TestMethod]
		public async Task Artstation_Test()
		{
			await Downloader_Test<ArtstationPostDownloader>($"-{nameof(ArtstationPostDownloader.Username)} jakubrozalski").CAF();
		}
		[TestMethod]
		public async Task Bcy_Test()
		{
			await Downloader_Test<BcyPostDownloader>($"-{nameof(BcyPostDownloader.Username)} 319378").CAF();
		}
		[TestMethod]
		public async Task Danbooru_Test()
		{
			await Downloader_Test<DanbooruPostDownloader>($"-{nameof(DanbooruPostDownloader.Tags)} blonde").CAF();
		}
		[TestMethod]
		public async Task Gelbooru_Test()
		{
			await Downloader_Test<GelbooruPostDownloader>($"-{nameof(GelbooruPostDownloader.Tags)} blonde").CAF();
		}
		[TestMethod]
		public async Task Konachan_Test()
		{
			await Downloader_Test<KonachanPostDownloader>($"-{nameof(KonachanPostDownloader.Tags)} dress").CAF();
		}
		[TestMethod]
		public async Task Safebooru_Test()
		{
			await Downloader_Test<SafebooruPostDownloader>($"-{nameof(SafebooruPostDownloader.Tags)} blonde").CAF();
		}
		[TestMethod]
		public async Task Yandere_Test()
		{
			await Downloader_Test<YanderePostDownloader>($"-{nameof(YanderePostDownloader.Tags)} dress").CAF();
		}
		[TestMethod]
		public async Task DeviantArt_Test()
		{
			await Downloader_Test<DeviantArtPostDownloader>($"-{nameof(DeviantArtPostDownloader.Tags)} by:disharmonica " +
				$"-{nameof(DeviantArtPostDownloader.GatheringMethod)} {DeviantArtGatheringMethod.Scraping}").CAF();
		}
		[TestMethod]
		public async Task Diyidan_Test()
		{
			await Downloader_Test<DiyidanPostDownloader>($"-{nameof(DiyidanPostDownloader.Username)} 6294196636885527271");
		}
		[TestMethod]
		public async Task Eshuushuu_Test()
		{
			await Downloader_Test<EshuushuuPostDownloader>($"-{nameof(EshuushuuPostDownloader.Tags)} 169").CAF();
		}
		[TestMethod]
		public async Task Flickr_Test()
		{
			await Downloader_Test<FlickrPostDownloader>($"-{nameof(FlickrPostDownloader.Search)} portrait " +
				$"-{nameof(FlickrPostDownloader.GatheringMethod)} {FlickrGatheringMethod.Tags}").CAF();
		}
		[TestMethod]
		public async Task Imgur_Test()
		{
			await Downloader_Test<ImgurPostDownloader>($"-{nameof(ImgurPostDownloader.Tags)} dogs").CAF();
		}
		[TestMethod]
		public async Task Instagram_Test()
		{
			await Downloader_Test<InstagramPostDownloader>($"-{nameof(InstagramPostDownloader.Username)} instagram").CAF();
		}
		[TestMethod]
		public async Task Pawoo_Test()
		{
			await Downloader_Test<PawooPostDownloader>($"-{nameof(PawooPostDownloader.Username)} @pixiv " +
				$"-{nameof(PawooPostDownloader.LoginUsername)} h2821117@nwytg.com " +
				$"-{nameof(PawooPostDownloader.LoginPassword)} password").CAF();
		}
		[TestMethod]
		public async Task Pinterest_Test()
		{
			await Downloader_Test<PinterestPostDownloader>($"-{nameof(PinterestPostDownloader.Search)} dogs " +
				$"-{nameof(PinterestPostDownloader.GatheringMethod)} {PinterestGatheringMethod.Tags}").CAF();
		}
		[TestMethod]
		public async Task Pixiv_Test()
		{
			await Downloader_Test<PixivPostDownloader>($"-{nameof(PixivPostDownloader.UserId)} 4338012 " +
				$"-{nameof(PixivPostDownloader.LoginUsername)} h2821117@nwytg.com " +
				$"-{nameof(PixivPostDownloader.LoginPassword)} password").CAF();
		}
		[TestMethod]
		public async Task Reddit_Test()
		{
			await Downloader_Test<RedditPostDownloader>($"-{nameof(RedditPostDownloader.Subreddit)} pics").CAF();
		}
		[TestMethod]
		public async Task Tumblr_Test()
		{
			await Downloader_Test<TumblrPostDownloader>($"-{nameof(TumblrPostDownloader.Username)} moxie2d").CAF();
		}
		[TestMethod]
		public async Task Twitter_Test()
		{
			await Downloader_Test<TwitterPostDownloader>($"-{nameof(TwitterPostDownloader.Search)} hews__" +
				$"-{nameof(TwitterPostDownloader.GatheringMethod)} {TwitterGatheringMethod.User}").CAF();
			await Downloader_Test<TwitterPostDownloader>($"-{nameof(TwitterPostDownloader.Search)} #dogs" +
				$"-{nameof(TwitterPostDownloader.GatheringMethod)} {TwitterGatheringMethod.Search}").CAF();
		}
		[TestMethod]
		public async Task Vsco_Test()
		{
			await Downloader_Test<VscoPostDownloader>($"-{nameof(VscoPostDownloader.Username)} kusumadjaja").CAF();
		}
		[TestMethod]
		public async Task Weibo_Test()
		{
			await Downloader_Test<WeiboPostDownloader>($"-{nameof(WeiboPostDownloader.Username)} 1632765501").CAF();
		}
		private async Task Downloader_Test<T>(string specificArgs) where T : IPostGatherer, IHasSettings, new()
		{
			var services = new DefaultServiceProviderFactory().CreateServiceProvider(new ServiceCollection()
				.AddSingleton<IDownloaderClient, DownloaderClient>()
				.AddTransient<IImageComparer, WindowsImageComparer>());
			var downloader = new T();

			var genericArgsResult = downloader.SettingParser.Parse(GenerateGenericArgs<T>());
			Assert.AreEqual(0, genericArgsResult.Errors.Length + genericArgsResult.UnusedParts.Length, $"Generic args failed in {typeof(T).Name}");
			var specificArgsResult = downloader.SettingParser.Parse(specificArgs);
			Assert.AreEqual(0, specificArgsResult.Errors.Length + specificArgsResult.UnusedParts.Length, $"Specific args failed in {typeof(T).Name}");
			Assert.IsTrue(downloader.CanStart, $"Not all arguments set in {typeof(T).Name}");

			var list = await downloader.GatherAsync(services).CAF();
			Assert.AreEqual(AMOUNT, list.Count, $"Not enough posts gotten in {typeof(T).Name}");
		}
		private string GetDirectory<T>()
		{
			return Path.Combine(Directory.GetCurrentDirectory(), typeof(T).Name);
		}
		private string GenerateGenericArgs<T>()
		{
			return $"-{nameof(PostDownloader.CreateDirectory)} " +
				$"-{nameof(PostDownloader.SavePath)} \"{GetDirectory<T>()}\" " +
				$"-{nameof(PostDownloader.AmountOfPostsToGather)} {AMOUNT} " +
				$"-{nameof(PostDownloader.MinScore)} {MIN_SCORE} " +
				$"-{nameof(PostDownloader.MinHeight)} {MIN_HEIGHT} " +
				$"-{nameof(PostDownloader.MinWidth)} {MIN_WIDTH} " +
				$"-{nameof(PostDownloader.MaxDaysOld)} {MAX_AGE} " +
				$"-{nameof(PostDownloader.MaxImageSimilarity)} 990 " +
				$"-{nameof(PostDownloader.ImagesCachedPerThread)} 4 " +
				$"-{nameof(PostDownloader.Start)}";
		}
	}
}