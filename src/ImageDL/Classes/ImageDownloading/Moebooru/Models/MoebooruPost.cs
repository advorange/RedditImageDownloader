using System;
using System.Collections.Generic;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Models
{
	/// <summary>
	/// Base Json model for a post from a Moebooru site.
	/// </summary>
	public abstract class MoebooruPost : IPost
	{
		#region Json
		/// <summary>
		/// The source of the image. Can be empty/null if no source is provided.
		/// </summary>
		[JsonProperty("source")]
		public readonly string Source;
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
		/// The location of the file for this post. May be missing http/https.
		/// </summary>
		[JsonProperty("file_url")]
		public readonly string FileUrl;
		/// <summary>
		/// The id of the post.
		/// </summary>
		[JsonProperty("id")]
		private readonly string _Id = null;
		/// <summary>
		/// The score of the post.
		/// </summary>
		[JsonProperty("score")]
		private readonly int _Score = -1;
		#endregion

		/// <inheritdoc />
		public string Id => _Id.ToString();
		/// <inhertitdoc />
		public abstract Uri PostUrl { get; }
		/// <inheritdoc />
		public IEnumerable<Uri> ContentUrls
		{
			get
			{
				if (Uri.TryCreate(FileUrl, UriKind.Absolute, out _))
				{
					return new[] { new Uri(FileUrl) };
				}
				else if (Uri.TryCreate($"{BaseUrl}{FileUrl}", UriKind.Absolute, out _))
				{
					return new[] { new Uri($"{BaseUrl}{FileUrl}") };
				}
				else
				{
					throw new ArgumentException($"Unable to generate an absolute url with {FileUrl}.");
				}
			}
		}
		/// <inheritdoc />
		public int Score => _Score;
		/// <inheritdoc />
		public abstract DateTime CreatedAt { get; }
		/// <summary>
		/// The base url of the -booru site. E.G.: https://danbooru.donmai.us or https://www.konachan.com
		/// </summary>
		public abstract Uri BaseUrl { get; }
		/// <summary>
		/// The width of the image.
		/// </summary>
		public abstract int Width { get; }
		/// <summary>
		/// The height of the image.
		/// </summary>
		public abstract int Height { get; }
		/// <summary>
		/// The tags for this image.
		/// </summary>
		public abstract string Tags { get; }

		/// <summary>
		/// Returns the id, width, and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Id} ({Width}x{Height})";
		}
	}
}