using AdvorangesUtils;
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
		#region Json
		/// <summary>
		/// The id of the first image in the post.
		/// </summary>
		[JsonProperty("cover")]
		public readonly string Cover;
		/// <summary>
		/// The width of the cover.
		/// </summary>
		[JsonProperty("cover_width")]
		public readonly int CoverWidth;
		/// <summary>
		/// The height of the cover.
		/// </summary>
		[JsonProperty("cover_height")]
		public readonly int CoverHeight;
		/// <summary>
		/// The privacy setting for this post, e.g. public, etc.
		/// </summary>
		[JsonProperty("privacy")]
		public readonly string Privacy;
		/// <summary>
		/// The layout style for this post.
		/// </summary>
		[JsonProperty("layout")]
		public readonly string Layout;
		/// <summary>
		/// The topic of the post. Usually is 'No Topic'.
		/// </summary>
		[JsonProperty("topic")]
		public readonly string Topic;
		/// <summary>
		/// The id of the topic.
		/// </summary>
		[JsonProperty("topic_id")]
		public readonly int TopicId;
		/// <summary>
		/// Whether the post has more than one image.
		/// </summary>
		[JsonProperty("is_album")]
		public readonly bool IsAlbum;
		/// <summary>
		/// Whether the post is in the public gallery.
		/// </summary>
		[JsonProperty("is_gallery")]
		public readonly bool IsGallery;
		/// <summary>
		/// The amount of images in the post.
		/// </summary>
		[JsonProperty("images_count")]
		public readonly int ImagesCount;
		[JsonProperty("images")]
		private List<ImgurImage> _Images = null;
		#endregion

		/// <inheritdoc />
		public override IEnumerable<Uri> ContentUrls
		{
			get
			{
				var list = new List<Uri> { Mp4Link == null ? PostUrl : new Uri(Mp4Link), };
				if (_Images != null)
				{
					list.AddRange(_Images.SelectMany(x => x.ContentUrls));
				}
				return list;
			}
		}
		/// <summary>
		/// Returns the images in the album.
		/// </summary>
		public List<ImgurImage> Images => _Images ?? (_Images = new List<ImgurImage>() { this, });

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
			Images.AddRange(await ImgurImageDownloader.GetImagesAsync(client, Id).CAF());
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