#pragma warning disable 1591, 649
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Imgur.Models
{
	/// <summary>
	/// Json model for a post from Imgur. Inherits from <see cref="ImgurImage"/> because there is a chance the post isn't an album.
	/// </summary>
	public sealed class ImgurPost : ImgurImage
	{
		[JsonProperty("cover")]
		public readonly string Cover;
		[JsonProperty("cover_width")]
		public readonly int CoverWidth;
		[JsonProperty("cover_height")]
		public readonly int CoverHeight;
		[JsonProperty("privacy")]
		public readonly string Privacy;
		[JsonProperty("layout")]
		public readonly string Layout;
		[JsonProperty("topic")]
		public readonly string Topic;
		[JsonProperty("topic_id")]
		public readonly int TopicId;
		[JsonProperty("is_album")]
		public readonly bool IsAlbum;
		[JsonProperty("is_gallery")]
		public readonly bool IsGallery;
		[JsonProperty("images_count")]
		public readonly int ImagesCount;
		[JsonProperty("images")]
		private List<ImgurImage> _Images;

		[JsonIgnore]
		public List<ImgurImage> Images => _Images ?? (_Images = new List<ImgurImage>() { this, });

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Id} ({Images.Count})";
		}
	}
}