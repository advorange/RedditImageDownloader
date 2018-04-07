using AdvorangesUtils;
using ImageDL.Enums;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloading.Imgur.Models
{
	/// <summary>
	/// Json model for a post from Imgur. Inherits from <see cref="ImgurImage"/> because there is a chance the post isn't an album.
	/// </summary>
	public sealed class ImgurPost : ImgurImage
	{
		/// <summary>
		/// Returns the images in the album.
		/// </summary>
		[JsonIgnore]
		public List<ImgurImage> Images => _Images ?? new List<ImgurImage>() { this, };
		/// <summary>
		/// The id of the first image in the post.
		/// </summary>
		[JsonProperty("cover")]
		public string Cover { get; private set; }
		/// <summary>
		/// The width of the cover.
		/// </summary>
		[JsonProperty("cover_width")]
		public int CoverWidth { get; private set; }
		/// <summary>
		/// The height of the cover.
		/// </summary>
		[JsonProperty("cover_height")]
		public int CoverHeight { get; private set; }
		/// <summary>
		/// The privacy setting for this post, e.g. public, etc.
		/// </summary>
		[JsonProperty("privacy")]
		public string Privacy { get; private set; }
		/// <summary>
		/// The layout style for this post.
		/// </summary>
		[JsonProperty("layout")]
		public string Layout { get; private set; }
		/// <summary>
		/// The topic of the post. Usually is 'No Topic'.
		/// </summary>
		[JsonProperty("topic")]
		public string Topic { get; private set; }
		/// <summary>
		/// The id of the topic.
		/// </summary>
		[JsonProperty("topic_id")]
		public int TopicId { get; private set; }
		/// <summary>
		/// Whether the post has more than one image.
		/// </summary>
		[JsonProperty("is_album")]
		public bool IsAlbum { get; private set; }
		/// <summary>
		/// Whether the post is in the public gallery.
		/// </summary>
		[JsonProperty("is_gallery")]
		public bool IsGallery { get; private set; }
		/// <summary>
		/// The amount of images in the post.
		/// </summary>
		[JsonProperty("images_count")]
		public int ImagesCount { get; private set; }
		/// <summary>
		/// Backing field for Images.
		/// </summary>
		[JsonProperty("images")]
		private List<ImgurImage> _Images = null;

		/// <summary>
		/// Makes sure this post has all the images.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		public async Task SetAllImages(IImageDownloaderClient client)
		{
			if (!IsAlbum || ImagesCount == Images.Count)
			{
				return;
			}
			Images.Clear();
			Images.AddRange(await ImgurImageDownloader.GetImagesFromApi(client, Id).CAF());
		}
		/// <inheritdoc />
		public override async Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			Uri[] urls;
			if (IsAlbum)
			{
				var tasks = Images.Select(async x => await x.GetImagesAsync(client).CAF());
				urls = (await Task.WhenAll(tasks).CAF()).SelectMany(x => x.ImageUrls).ToArray();
			}
			else
			{
				urls = new[] { Mp4Link ?? PostUrl };
			}
			return new ImageResponse(FailureReason.Success, null, urls);
		}
		/// <summary>
		/// Returns the id and image count.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Id} ({Images.Count})";
		}
	}
}