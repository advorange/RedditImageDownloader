using System;
using System.Threading.Tasks;
using FChan.Library;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.FourChan.Models
{
	/// <summary>
	/// Holds the gotten post for 4Chan.
	/// </summary>
	public sealed class FourChanPost : IPost
	{
		/// <summary>
		/// The post holding all of the information.
		/// </summary>
		[JsonProperty("post")]
		public Post Post { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public string Id => Post.PostNumber.ToString();
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"http://boards.4chan.org/{Post.Board}/thread/{ThreadId}#p{Id}");
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => int.MinValue;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(Post.UnixTimestamp)).ToUniversalTime();
		/// <summary>
		/// The id of the thread this was posted in.
		/// </summary>
		[JsonProperty("thread_id")]
		public string ThreadId { get; private set; }

		/// <summary>
		/// Creates an instance of <see cref="FourChanPost"/>.
		/// </summary>
		/// <param name="post"></param>
		/// <param name="threadId"></param>
		public FourChanPost(Post post, int threadId)
		{
			Post = post;
			ThreadId = threadId.ToString();
		}

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			return Task.FromResult(Post.HasImage
				? ImageResponse.FromUrl(new Uri($"http://i.4cdn.org/{Post.Board}/{Post.FileName}{Post.FileExtension}"))
				: ImageResponse.FromNotFound(PostUrl));
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
