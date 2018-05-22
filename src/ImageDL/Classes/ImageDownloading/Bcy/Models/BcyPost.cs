using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Bcy.Models
{
	/// <summary>
	/// Json model for a post from Bcy.
	/// </summary>
	public sealed class BcyPost : IPost
	{
		/// <inheritdoc />
		[JsonIgnore]
		public string Id => Details.PostId.ToString();
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl
		{
			get
			{
				switch (OType)
				{
					case "drawer":
						return new Uri($"https://bcy.net/illust/detail/{Details.ChannelId}/{Details.PostId}");
					case "coser":
						return new Uri($"https://bcy.net/coser/detail/{Details.ChannelId}/{Details.PostId}");
					case "group":
						return new Uri($"https://bcy.net/group/list/{Details.ChannelId}");
				}
				switch (OTypeData)
				{
					case "ask":
						return new Uri($"https://bcy.net/u/{UserId}/ask");
					case "daily":
						return new Uri($"https://bcy.net/daily/detail/{Details.PostId}");
				}
				throw new InvalidOperationException("Unable to generate a url for this post.");
			}
		}
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => Details.FavoriteCount;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(CreatedAtTimestamp)).ToUniversalTime();
		/// <summary>
		/// Not sure, not post id or submitter id though.
		/// </summary>
		[JsonProperty("tl_id")]
		public int TlId { get; private set; }
		/// <summary>
		/// The unix timestamp in seconds of when this was created.
		/// </summary>
		[JsonProperty("ctime")]
		public long CreatedAtTimestamp { get; private set; }
		/// <summary>
		/// The id of the submitter.
		/// </summary>
		[JsonProperty("uid")]
		public int UserId { get; private set; }
		/// <summary>
		/// Same as <see cref="UserId"/>, so not sure why this field exists.
		/// </summary>
		[JsonProperty("ouid")]
		public int OUserId { get; private set; }
		/// <summary>
		/// Not sure, usually is "drawer."
		/// </summary>
		[JsonProperty("otype")]
		public string OType { get; private set; }
		/// <summary>
		/// Not sure, usually is "post."
		/// </summary>
		[JsonProperty("otype_data")]
		public string OTypeData { get; private set; }
		/// <summary>
		/// Not sure, usually is 0.
		/// </summary>
		[JsonProperty("trans_id")]
		public int TransId { get; private set; }
		/// <summary>
		/// Details about the post, such as images.
		/// </summary>
		[JsonProperty("detail")]
		public BcyDetails Details { get; private set; }
		/// <summary>
		/// The submitter's avatar url.
		/// </summary>
		[JsonProperty("avatar")]
		public string UserAvatar { get; private set; }
		/// <summary>
		/// The submitter's name.
		/// </summary>
		[JsonProperty("uname")]
		public string Username { get; private set; }
		/// <summary>
		/// Same as <see cref="UserAvatar"/>, so not sure why this field exists.
		/// </summary>
		[JsonProperty("oavatar")]
		public string OUserAvatar { get; private set; }
		/// <summary>
		/// Same as <see cref="Username"/>, so not sure why this field exists.
		/// </summary>
		[JsonProperty("ouname")]
		public string OUsername { get; private set; }

		/// <inheritdoc />
		public async Task<ImageResponse> GetImagesAsync(IDownloaderClient client)
		{
			return await BcyPostDownloader.GetBcyImagesAsync(client, PostUrl).CAF();
		}
		/// <summary>
		/// Returns the post url.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return PostUrl.ToString();
		}
	}
}