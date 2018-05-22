using System;
using System.Linq;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.SpecificUrl.Models
{
	/// <summary>
	/// Indicates this is a post from an unknown source.
	/// </summary>
	public sealed class SpecificUrlPost : IPost
	{
		/// <inheritdoc />
		[JsonIgnore]
		public string Id => _Id;
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => _PostUrl;
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => _Score;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => _CreatedAt;

		[JsonProperty("id")]
		private string _Id;
		[JsonProperty("post_url")]
		private Uri _PostUrl;
		[JsonProperty("score")]
		private int _Score;
		[JsonProperty("created_at")]
		private DateTime _CreatedAt;

		/// <summary>
		/// Creates an instance of <see cref="SpecificUrlPost"/>.
		/// </summary>
		/// <param name="postUrl"></param>
		/// <param name="id"></param>
		/// <param name="score"></param>
		/// <param name="createdAt"></param>
		public SpecificUrlPost(Uri postUrl, string id = null, int score = -1, DateTime createdAt = default)
		{
			_PostUrl = postUrl ?? throw new ArgumentException($"{nameof(postUrl)} cannot be null.");
			_Id = id;
			_Score = score;
			_CreatedAt = createdAt;
		}

		/// <inheritdoc />
		public async Task<ImageResponse> GetImagesAsync(IDownloaderClient client)
		{
			if (PostUrl.ToString().IsImagePath())
			{
				return ImageResponse.FromUrl(PostUrl);
			}
			if (client.Gatherers.SingleOrDefault(x => x.IsFromWebsite(PostUrl)) is IImageGatherer gatherer)
			{
				return await gatherer.FindImagesAsync(client, PostUrl).CAF();
			}
			var result = await client.GetHtmlAsync(() => client.GenerateReq(PostUrl)).CAF();
			if (result.IsSuccess)
			{
				var img = result.Value.DocumentNode.Descendants("img");
				var src = img.Select(x => x.GetAttributeValue("src", null))
					.Select(x =>
					{
						if (Uri.TryCreate(x, UriKind.Absolute, out var uri))
						{
							return uri;
						}
						if (Uri.TryCreate($"https://{PostUrl.Host}{x}", UriKind.Absolute, out uri))
						{
							return uri;
						}
						//How to check if relative to something other than just the host?
						return null;
					})
					.Where(x => x != null);
				if (src.Any())
				{
					return ImageResponse.FromImages(src);
				}
			}
			return ImageResponse.FromNotFound(PostUrl);
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