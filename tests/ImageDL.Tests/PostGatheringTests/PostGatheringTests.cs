using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdvorangesSettingParser.Interfaces;
using AdvorangesSettingParser.Utils;
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
using ImageDL.Classes.ImageDownloading.Lofter;
using ImageDL.Classes.ImageDownloading.Pawoo;
using ImageDL.Classes.ImageDownloading.Pinterest;
using ImageDL.Classes.ImageDownloading.Pixiv;
using ImageDL.Classes.ImageDownloading.Reddit;
using ImageDL.Classes.ImageDownloading.Tumblr;
using ImageDL.Classes.ImageDownloading.Twitter;
using ImageDL.Classes.ImageDownloading.Vsco;
using ImageDL.Classes.ImageDownloading.Weibo;
using ImageDL.Classes.ImageDownloading.Zerochan;
using ImageDL.Enums;
using ImageDL.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageDL.Tests.PostGatheringTests
{
	//Not sure exactly how to unit test a downloader considering it relies on posts from the internet which are random
	//Maybe just have the downloader verify the post count and not the cached/deleted/link counts
	[TestClass]
	public class PostGatheringTests
	{
		private const int AMOUNT = 5;
		private const int MIN_SCORE = 0;
		private const int MIN_WIDTH = 100;
		private const int MIN_HEIGHT = 100;
		private const int MAX_AGE = 10000;

		[TestMethod]
		public async Task FourChan_Test()
			=> await Gatherer_Test<FourChanPostDownloader>(
				$"-{nameof(FourChanPostDownloader.Board)} a");
		[TestMethod]
		public async Task AnimePictures_Test()
			=> await Gatherer_Test<AnimePicturesPostDownloader>(
				$"-{nameof(AnimePicturesPostDownloader.Tags)} blonde");
		[TestMethod]
		public async Task Artstation_Test()
			=> await Gatherer_Test<ArtstationPostDownloader>(
				$"-{nameof(ArtstationPostDownloader.Username)} jakubrozalski");
		[TestMethod]
		public async Task Bcy_Test()
			=> await Gatherer_Test<BcyPostDownloader>(
				$"-{nameof(BcyPostDownloader.Username)} 319378");
		[TestMethod]
		public async Task Danbooru_Test()
			=> await Gatherer_Test<DanbooruPostDownloader>(
				$"-{nameof(DanbooruPostDownloader.Tags)} blonde");
		[TestMethod]
		public async Task Gelbooru_Test()
			=> await Gatherer_Test<GelbooruPostDownloader>(
				$"-{nameof(GelbooruPostDownloader.Tags)} blonde");
		[TestMethod]
		public async Task Konachan_Test()
			=> await Gatherer_Test<KonachanPostDownloader>(
				$"-{nameof(KonachanPostDownloader.Tags)} dress");
		[TestMethod]
		public async Task Safebooru_Test()
			=> await Gatherer_Test<SafebooruPostDownloader>(
				$"-{nameof(SafebooruPostDownloader.Tags)} blonde");
		[TestMethod]
		public async Task Yandere_Test()
			=> await Gatherer_Test<YanderePostDownloader>(
				$"-{nameof(YanderePostDownloader.Tags)} dress");
		[TestMethod]
		public async Task DeviantArt_Test()
			=> await Gatherer_Test<DeviantArtPostDownloader>(
				$"-{nameof(DeviantArtPostDownloader.Tags)} by:disharmonica " +
				$"-{nameof(DeviantArtPostDownloader.GatheringMethod)} {DeviantArtGatheringMethod.Scraping}");
		[TestMethod]
		public async Task Diyidan_Test()
			=> await Gatherer_Test<DiyidanPostDownloader>(
				$"-{nameof(DiyidanPostDownloader.Username)} 6293615542255832232");
		[TestMethod]
		public async Task Eshuushuu_Test()
			=> await Gatherer_Test<EshuushuuPostDownloader>(
				$"-{nameof(EshuushuuPostDownloader.Tags)} 169");
		[TestMethod]
		public async Task Flickr_Test()
			=> await Gatherer_Test<FlickrPostDownloader>(
				$"-{nameof(FlickrPostDownloader.Search)} portrait " +
				$"-{nameof(FlickrPostDownloader.GatheringMethod)} {FlickrGatheringMethod.Tags}");
		[TestMethod]
		public async Task Imgur_Test()
			=> await Gatherer_Test<ImgurPostDownloader>(
				$"-{nameof(ImgurPostDownloader.Tags)} dogs");
		[TestMethod]
		public async Task Instagram_Test()
			=> await Gatherer_Test<InstagramPostDownloader>(
				$"-{nameof(InstagramPostDownloader.Username)} instagram");
		[TestMethod]
		public async Task Lofter_Test()
			=> await Gatherer_Test<LofterPostDownloader>(
				$"-{nameof(LofterPostDownloader.Username)} monsterlei");
		[TestMethod]
		public async Task Pawoo_Test()
			=> await Gatherer_Test<PawooPostDownloader>(
				$"-{nameof(PawooPostDownloader.Username)} @pixiv " +
				$"-{nameof(PawooPostDownloader.LoginUsername)} h2821117@nwytg.com " +
				$"-{nameof(PawooPostDownloader.LoginPassword)} password");
		[TestMethod]
		public async Task Pinterest_Test()
			=> await Gatherer_Test<PinterestPostDownloader>(
				$"-{nameof(PinterestPostDownloader.Search)} dogs " +
				$"-{nameof(PinterestPostDownloader.GatheringMethod)} {PinterestGatheringMethod.Tags}");
		[TestMethod]
		public async Task Pixiv_Test()
			=> await Gatherer_Test<PixivPostDownloader>(
				$"-{nameof(PixivPostDownloader.UserId)} 4338012 " +
				$"-{nameof(PixivPostDownloader.LoginUsername)} h2821117@nwytg.com " +
				$"-{nameof(PixivPostDownloader.LoginPassword)} password");
		[TestMethod]
		public async Task Reddit_Test()
			=> await Gatherer_Test<RedditPostDownloader>(
				$"-{nameof(RedditPostDownloader.Subreddit)} pics");
		[TestMethod]
		public async Task Tumblr_Test()
			=> await Gatherer_Test<TumblrPostDownloader>(
				$"-{nameof(TumblrPostDownloader.Username)} moxie2d");
		[TestMethod]
		public async Task Twitter_Test()
		{
			await Gatherer_Test<TwitterPostDownloader>(
				$"-{nameof(TwitterPostDownloader.Search)} ShitpostBot5000 " +
				$"-{nameof(TwitterPostDownloader.GatheringMethod)} {TwitterGatheringMethod.User}").CAF();
			await Gatherer_Test<TwitterPostDownloader>(
				$"-{nameof(TwitterPostDownloader.Search)} #dogs " +
				$"-{nameof(TwitterPostDownloader.GatheringMethod)} {TwitterGatheringMethod.Search}").CAF();
		}
		[TestMethod]
		public async Task Vsco_Test()
			=> await Gatherer_Test<VscoPostDownloader>(
				$"-{nameof(VscoPostDownloader.Username)} kusumadjaja");
		[TestMethod]
		public async Task Weibo_Test()
			=> await Gatherer_Test<WeiboPostDownloader>(
				$"-{nameof(WeiboPostDownloader.Username)} 1632765501");
		[TestMethod]
		public async Task Zerochan_Test()
			=> await Gatherer_Test<ZerochanPostDownloader>(
				$"-{nameof(ZerochanPostDownloader.Tags)} Dress");
		private async Task Gatherer_Test<T>(string specificArgs) where T : IPostGatherer, IParsable, new()
		{
			var services = ImageDL.CreateServices<NetFrameworkImageComparer>();
			var gatherer = new T();

			var genericArgsResult = gatherer.SettingParser.Parse(GenerateGenericArgs<T>());
			Assert.AreEqual(0, genericArgsResult.Errors.Count() + genericArgsResult.UnusedParts.Count(), $"Generic args failed in {typeof(T).Name}");
			var specificArgsResult = gatherer.SettingParser.Parse(specificArgs);
			Assert.AreEqual(0, specificArgsResult.Errors.Count() + specificArgsResult.UnusedParts.Count(), $"Specific args failed in {typeof(T).Name}");
			Assert.IsTrue(gatherer.SettingParser.AreAllSet(), $"Not all arguments set in {typeof(T).Name}");

			var list = await gatherer.GatherAsync(services).CAF();
			Assert.AreEqual(AMOUNT, list.Count, $"Not enough posts gotten in {typeof(T).Name}");
		}
		private string GetDirectory<T>()
			=> Path.Combine(Directory.GetCurrentDirectory(), typeof(T).Name);
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