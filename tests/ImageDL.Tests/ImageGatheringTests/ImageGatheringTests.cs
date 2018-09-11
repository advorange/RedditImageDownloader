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
using ImageDL.Classes.ImageDownloading.TheAnimeGallery;
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
			=> await Gatherer_Test<AnimePicturesImageGatherer>("https://anime-pictures.net/pictures/view_post/554778").CAF();
		[TestMethod]
		public async Task Artstation_Test()
			=> await Gatherer_Test<ArtstationImageGatherer>("https://www.artstation.com/artwork/4ZV3l").CAF();
		[TestMethod]
		public async Task Bcy_Test()
			=> await Gatherer_Test<BcyImageGatherer>("https://bcy.net/item/detail/6551245175488774404").CAF();
		[TestMethod]
		public async Task Danbooru_Test()
			=> await Gatherer_Test<DanbooruImageGatherer>("https://danbooru.donmai.us/posts/3140015").CAF();
		[TestMethod]
		public async Task Gelbooru_Test()
			=> await Gatherer_Test<GelbooruImageGatherer>("https://gelbooru.com/index.php?page=post&s=view&id=4256163").CAF();
		[TestMethod]
		public async Task Konachan_Test()
			=> await Gatherer_Test<KonachanImageGatherer>("http://konachan.com/post/show/265806/black_hair-blonde_hair-breasts-cleavage-crossover-").CAF();
		[TestMethod]
		public async Task Safebooru_Test()
			=> await Gatherer_Test<SafebooruImageGatherer>("https://safebooru.org/index.php?page=post&s=view&id=1663526").CAF();
		[TestMethod]
		public async Task Yandere_Test()
			=> await Gatherer_Test<YandereImageGatherer>("https://yande.re/post/show/455055").CAF();
		[TestMethod]
		public async Task DeviantArt_Test()
			=> await Gatherer_Test<DeviantArtImageGatherer>("https://disharmonica.deviantart.com/art/Diablo-3-Heroes-of-the-Storm-Li-Ming-cosplay-730551215").CAF();
		[TestMethod]
		public async Task Diyidan_Test()
			=> await Gatherer_Test<DiyidanImageGatherer>("https://www.diyidan.com/main/post/6294360860189844509/detail/1");
		[TestMethod]
		public async Task Eshuushuu_Test()
			=> await Gatherer_Test<EshuushuuImageGatherer>("http://e-shuushuu.net/image/963526/").CAF();
		[TestMethod]
		public async Task Flickr_Test()
			=> await Gatherer_Test<FlickrImageGatherer>("https://www.flickr.com/photos/ukaaa/33480370555/").CAF();
		[TestMethod]
		public async Task Imgur_Test()
			=> await Gatherer_Test<ImgurImageGatherer>("https://imgur.com/LPsxLQE").CAF();
		[TestMethod]
		public async Task Instagram_Test()
			=> await Gatherer_Test<InstagramImageGatherer>("https://www.instagram.com/p/BjGVyd4DBUm/").CAF();
		[TestMethod]
		public async Task Lofter_Test()
			=> await Gatherer_Test<LofterImageGatherer>("http://monsterlei.lofter.com/post/467bec_f7d3a9f").CAF();
		[TestMethod]
		public async Task Pawoo_Test()
			=> await Gatherer_Test<PawooImageGatherer>("https://pawoo.net/@pixiv/99851091445887348").CAF();
		[TestMethod]
		public async Task Pinterest_Test()
			=> await Gatherer_Test<PinterestImageGatherer>("https://www.pinterest.com/pin/108227197274329117").CAF();
		[TestMethod]
		public async Task Pixiv_Test()
			=> await Gatherer_Test<PixivImageGatherer>("https://www.pixiv.net/member_illust.php?mode=medium&illust_id=70165419").CAF();
		[TestMethod]
		public async Task Reddit_Test()
			=> await Gatherer_Test<RedditImageGatherer>("https://www.reddit.com/6z699p").CAF();
		//This test doesn't always work on the first try. Maybe something to do with setting the content filter
		[TestMethod]
		public async Task TheAnimeGallery_Test()
			=> await Gatherer_Test<TheAnimeGalleryImageGatherer>("http://www.theanimegallery.com/gallery/image:198513").CAF();
		[TestMethod]
		public async Task Tumblr_Test()
			=> await Gatherer_Test<TumblrImageGatherer>("https://moxie2d.tumblr.com/post/160928815721/eye-closeups-enm").CAF();
		[TestMethod]
		public async Task Twitter_Test()
			=> await Gatherer_Test<TwitterImageGatherer>("https://twitter.com/hews__/status/1001313971306090496").CAF();
		[TestMethod]
		public async Task Vsco_Test()
			=> await Gatherer_Test<VscoImageGatherer>("https://vsco.co/kusumadjaja/media/5b00b095e034490a3323a6bc").CAF();
		[TestMethod]
		public async Task Weibo_Test()
			=> await Gatherer_Test<WeiboImageGatherer>("https://weibo.com/1632765501/GijfWif2d").CAF();
		[TestMethod]
		public async Task Zerochan_Test()
			=> await Gatherer_Test<ZerochanImageGatherer>("https://www.zerochan.net/138640").CAF();
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