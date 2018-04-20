using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.Scraped
{
	/// <summary>
	/// Json model for a post from Twitter.
	/// </summary>
	public sealed class TwitterScrapedPost : IPost
	{
		/// <inheritdoc />
		[JsonProperty(nameof(Id))]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://www.twitter.com/{Username}/status/{Id}");
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => FavoriteCount;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(CreatedAtTimestamp)).ToUniversalTime();
		/// <summary>
		/// The urls of the images in the tweet.
		/// </summary>
		[JsonProperty(nameof(ImageUrls))]
		public IList<Uri> ImageUrls { get; private set; }
		/// <summary>
		/// The name of the user who posted this.
		/// </summary>
		[JsonProperty(nameof(Username))]
		public string Username { get; private set; }
		/// <summary>
		/// The unix timestamp in seconds of when this was created.
		/// </summary>
		[JsonProperty(nameof(CreatedAtTimestamp))]
		public ulong CreatedAtTimestamp { get; private set; }
		/// <summary>
		/// The amount of likes the post has.
		/// </summary>
		[JsonProperty(nameof(FavoriteCount))]
		public int FavoriteCount { get; private set; }
		/// <summary>
		/// The amount of retweets the post has.
		/// </summary>
		[JsonProperty(nameof(RetweetCount))]
		public int RetweetCount { get; private set; }
		/// <summary>
		/// The amount of comments the post has.
		/// </summary>
		[JsonProperty(nameof(CommentCount))]
		public int CommentCount { get; private set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			return Task.FromResult(ImageResponse.FromImages(ImageUrls));
		}
		/// <summary>
		/// Returns the id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Id;
		}
	}
}
