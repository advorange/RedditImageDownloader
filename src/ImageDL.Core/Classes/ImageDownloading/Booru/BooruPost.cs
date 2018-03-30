using Newtonsoft.Json;
using System;

namespace ImageDL.Classes.ImageDownloading.Booru
{
	/// <summary>
	/// Base Json model for a post from a -booru site.
	/// </summary>
	public abstract class BooruPost
	{
		/// <summary>
		/// The id of the post.
		/// </summary>
		[JsonProperty("id")]
		public readonly int Id;
		/// <summary>
		/// The source of the image. Can be empty/null if no source is provided.
		/// </summary>
		[JsonProperty("source")]
		public readonly string Source;
		/// <summary>
		/// The score of the post.
		/// </summary>
		[JsonProperty("score")]
		public readonly int Score;
		/// <summary>
		/// The rating of the image. Safe, questionable, or explicit.
		/// </summary>
		[JsonProperty("rating")]
		public readonly char Rating;
		/// <summary>
		/// The id of the parent post to this post if there is one.
		/// </summary>
		[JsonProperty("parent_id")]
		public readonly int? ParentId;
		/// <summary>
		/// The hash of the image.
		/// </summary>
		[JsonProperty("md5")]
		public readonly string MD5;
		/// <summary>
		/// Whether or not the post has children.
		/// </summary>
		[JsonProperty("has_children")]
		public readonly bool HasChildren;
		/// <summary>
		/// Where the image is located.
		/// </summary>
		[JsonIgnore]
		public string FileUrl
		{
			get
			{
				if (Uri.TryCreate(_FileUrl, UriKind.Absolute, out _))
				{
					return _FileUrl;
				}
				else if (Uri.TryCreate($"{BaseUrl}{_FileUrl}", UriKind.Absolute, out _))
				{
					return $"{BaseUrl}{_FileUrl}";
				}
				else
				{
					throw new ArgumentException($"Unable to generate an absolute uri with {_FileUrl}.");
				}
			}
		}
		/// <summary>
		/// The base url of the -booru site. E.G.: https://www.danbooru.donmai.us or https://www.konachan.com
		/// </summary>
		[JsonIgnore]
		public abstract string BaseUrl { get; }
		/// <summary>
		/// The url leading to this post.
		/// </summary>
		[JsonIgnore]
		public abstract string PostUrl { get; }
		/// <summary>
		/// The width of the image.
		/// </summary>
		[JsonIgnore]
		public abstract int Width { get; }
		/// <summary>
		/// The height of the image.
		/// </summary>
		[JsonIgnore]
		public abstract int Height { get; }
		/// <summary>
		/// The time the post was created at.
		/// </summary>
		[JsonIgnore]
		public abstract DateTime CreatedAt { get; }
		/// <summary>
		/// The tags for this image.
		/// </summary>
		[JsonIgnore]
		public abstract string Tags { get; }

		[JsonProperty("file_url")]
		private readonly string _FileUrl = null;
	}
}
