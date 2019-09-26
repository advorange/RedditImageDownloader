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
		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; private set; }

		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }

		/// <inheritdoc />
		[JsonProperty("post_url")]
		public Uri PostUrl { get; private set; }

		/// <inheritdoc />
		[JsonProperty("score")]
		public int Score { get; private set; }

		/// <summary>
		/// Creates an instance of <see cref="SpecificUrlPost"/>.
		/// </summary>
		/// <param name="postUrl"></param>
		/// <param name="id"></param>
		/// <param name="score"></param>
		/// <param name="createdAt"></param>
		public SpecificUrlPost(Uri postUrl, string id = null, int score = -1, DateTime createdAt = default)
		{
			PostUrl = postUrl ?? throw new ArgumentException($"{nameof(postUrl)} cannot be null.");
			Id = id;
			Score = score;
			CreatedAt = createdAt;
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
				var src = img
					.Select(x => x.GetAttributeValue("src", null))
					.Select(x =>
					{
						if (Uri.TryCreate(x, UriKind.Absolute, out var uri)
							|| Uri.TryCreate($"https://{PostUrl.Host}{x}", UriKind.Absolute, out uri))
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
		public override string ToString() => PostUrl.ToString();
	}
}