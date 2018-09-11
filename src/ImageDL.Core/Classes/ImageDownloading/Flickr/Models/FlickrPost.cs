using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageDL.Classes.ImageDownloading.Flickr.Models
{
	/// <summary>
	/// Json model for a post from Flickr.
	/// </summary>
    public sealed class FlickrPost : IPost, ISize
    {
		/// <inheritdoc />
        [JsonProperty("id")]
        public string Id { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://www.flickr.com/photos/{OwnerId}/{Id}");
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => CountFaves;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(CreatedAtTimestamp)).ToUniversalTime();
		/// <inheritdoc />
		[JsonIgnore]
		public int Width => _OWidth ?? _MWidth ?? -1;
		/// <inheritdoc />
		[JsonIgnore]
		public int Height => _OHeight ?? _MHeight ?? -1;
		/// <summary>
		/// The id of the owner.
		/// </summary>
		[JsonProperty("owner")]
        public string OwnerId { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
        [JsonProperty("secret")]
        public string Secret { get; private set; }
		/// <summary>
		/// The server this was uploaded to.
		/// </summary>
        [JsonProperty("server")]
        public string Server { get; private set; }
		/// <summary>
		/// The farm this was uploaded to.
		/// </summary>
        [JsonProperty("farm")]
        public int Farm { get; private set; }
		/// <summary>
		/// The title of the post.
		/// </summary>
        [JsonProperty("title")]
        public string Title { get; private set; }
		/// <summary>
		/// Whether the post is public.
		/// </summary>
        [JsonProperty("ispublic")]
        public int Ispublic { get; private set; }
		/// <summary>
		/// Whether the post is from a friend.
		/// </summary>
        [JsonProperty("isfriend")]
        public int Isfriend { get; private set; }
		/// <summary>
		/// Whether the post is from family.
		/// </summary>
        [JsonProperty("isfamily")]
        public int Isfamily { get; private set; }
		/// <summary>
		/// Whether the post is safe.
		/// </summary>
        [JsonProperty("safe")]
        public int Safe { get; private set; }
		/// <summary>
		/// When the post was uploaded.
		/// </summary>
        [JsonProperty("dateupload")]
        public long CreatedAtTimestamp { get; private set; }
		/// <summary>
		/// The username of the owner.
		/// </summary>
        [JsonProperty("ownername")]
        public string Ownername { get; private set; }
		/// <summary>
		/// How many favorites this has.
		/// </summary>
        [JsonProperty("count_faves")]
        public int CountFaves { get; private set; }
		/// <summary>
		/// How many comments this has.
		/// </summary>
        [JsonProperty("count_comments")]
        public int CountComments { get; private set; }
		/// <summary>
		/// The type of media this is, e.g. photo, etc.
		/// </summary>
        [JsonProperty("media")]
        public string Media { get; private set; }
		/// <summary>
		/// The media status, e.g. ready, etc.
		/// </summary>
        [JsonProperty("media_status")]
        public string MediaStatus { get; private set; }
		/// <summary>
		/// The url to the original image.
		/// </summary>
		[JsonIgnore]
		public Uri ImageUrl => _OImageUrl ?? _MImageUrl;
		/// <summary>
		/// The url of the original image.
		/// </summary>
		[JsonProperty("url_o")]
		private Uri _OImageUrl { get; set; }
		/// <summary>
		/// The width of the original image.
		/// </summary>
		[JsonProperty("width_o")]
		private int? _OWidth { get; set; }
		/// <summary>
		/// The height of the original image.
		/// </summary>
		[JsonProperty("height_o")]
		private int? _OHeight { get; set; }
		/// <summary>
		/// The url of a smaller image, will only be used if o is not found.
		/// </summary>
		[JsonProperty("url_m")]
		private Uri _MImageUrl { get; set; }
		/// <summary>
		/// The width of a smaller image, will only be used if o is not found.
		/// </summary>
		[JsonProperty("width_m")]
		private int? _MWidth { get; set; }
		/// <summary>
		/// The height of a smaller image, will only be used if o is not found.
		/// </summary>
		[JsonProperty("height_m")]
		private int? _MHeight { get; set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IDownloaderClient client) => Task.FromResult(ImageResponse.FromUrl(ImageUrl));
		/// <summary>
		/// Returns the id, width, and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Id} ({Width}x{Height})";
	}
}