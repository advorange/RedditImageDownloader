using System;
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
using ImageDL.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageDL.Tests.ImageGatheringTests
{
	[TestClass]
	public class ImageGatheringTests
	{
		/*
		[TestMethod]
		public async Task FourChan_Test()
		{
			//Not sure there is an easy way to do this one.
			//Even if there is, not much of a reason because the links die after several hours
		}
		*/

		[TestMethod]
		public async Task AnimePictures_Test()
		{
			const string URL = "https://anime-pictures.net/pictures/view_post/554778";
			await Gatherer_Test<AnimePicturesImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Artstation_Test()
		{
			const string URL = "https://www.artstation.com/artwork/4ZV3l";
			await Gatherer_Test<ArtstationImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Bcy_Test()
		{
			const string URL = "https://bcy.net/item/detail/6551245175488774404";
			await Gatherer_Test<BcyImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Danbooru_Test()
		{
			const string URL = "https://danbooru.donmai.us/posts/3140015";
			await Gatherer_Test<DanbooruImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task DeviantArt_Test()
		{
			const string URL = "https://disharmonica.deviantart.com/art/Diablo-3-Heroes-of-the-Storm-Li-Ming-cosplay-730551215";
			await Gatherer_Test<DeviantArtImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Diyidan_Test()
		{
			const string URL = "https://www.diyidan.com/main/post/6294360860189844509/detail/1";
			await Gatherer_Test<DiyidanImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Eshuushuu_Test()
		{
			const string URL = "http://e-shuushuu.net/image/963526/";
			await Gatherer_Test<EshuushuuImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Flickr_Test()
		{
			const string URL = "https://www.flickr.com/photos/ukaaa/33480370555/";
			await Gatherer_Test<FlickrImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Gelbooru_Test()
		{
			const string URL = "https://gelbooru.com/index.php?page=post&s=view&id=4256163";
			await Gatherer_Test<GelbooruImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Imgur_Test()
		{
			const string URL = "https://imgur.com/LPsxLQE";
			await Gatherer_Test<ImgurImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Instagram_Test()
		{
			const string URL = "https://www.instagram.com/p/BjGVyd4DBUm/";
			await Gatherer_Test<InstagramImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Konachan_Test()
		{
			const string URL = "http://konachan.com/post/show/265806/black_hair-blonde_hair-breasts-cleavage-crossover-";
			await Gatherer_Test<KonachanImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Lofter_Test()
		{
			const string URL = "http://monsterlei.lofter.com/post/467bec_f7d3a9f";
			await Gatherer_Test<LofterImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Pawoo_Test()
		{
			const string URL = "https://pawoo.net/@pixiv/99851091445887348";
			await Gatherer_Test<PawooImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Pinterest_Test()
		{
			const string URL = "https://www.pinterest.com/pin/108227197274329117";
			await Gatherer_Test<PinterestImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Pixiv_Test()
		{
			const string URL = "https://www.pixiv.net/member_illust.php?mode=medium&illust_id=70165419";
			await Gatherer_Test<PixivImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Reddit_Test()
		{
			const string URL = "https://www.reddit.com/6z699p";
			await Gatherer_Test<RedditImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Safebooru_Test()
		{
			const string URL = "https://safebooru.org/index.php?page=post&s=view&id=1663526";
			await Gatherer_Test<SafebooruImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Tumblr_Test()
		{
			const string URL = "https://moxie2d.tumblr.com/post/160928815721/eye-closeups-enm";
			await Gatherer_Test<TumblrImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Twitter_Test()
		{
			const string URL = "https://twitter.com/hews__/status/1001313971306090496";
			await Gatherer_Test<TwitterImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Vsco_Test()
		{
			const string URL = "https://vsco.co/kusumadjaja/media/5b00b095e034490a3323a6bc";
			await Gatherer_Test<VscoImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Weibo_Test()
		{
			const string URL = "https://weibo.com/1632765501/GijfWif2d";
			await Gatherer_Test<WeiboImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Yandere_Test()
		{
			const string URL = "https://yande.re/post/show/455055";
			await Gatherer_Test<YandereImageGatherer>(URL).CAF();
		}

		[TestMethod]
		public async Task Zerochan_Test()
		{
			const string URL = "https://www.zerochan.net/138640";
			await Gatherer_Test<ZerochanImageGatherer>(URL).CAF();
		}

		private async Task Gatherer_Test<T>(string input) where T : IImageGatherer, new()
		{
			var client = new DownloaderClient();
			var gatherer = new T();

			var url = new Uri(input);
			Assert.IsTrue(gatherer.IsFromWebsite(url), $"Unable to verify that {input} is from {typeof(T).Name}.");
			var results = await gatherer.FindImagesAsync(client, url).CAF();
			Assert.AreNotEqual(0, results.ImageUrls.Length, $"Unable to find any images for {typeof(T).Name}");
		}
	}
}