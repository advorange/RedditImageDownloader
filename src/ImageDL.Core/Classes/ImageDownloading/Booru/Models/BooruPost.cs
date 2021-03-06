﻿using System;
using System.Threading.Tasks;

using ImageDL.Interfaces;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Booru.Models
{
	/// <summary>
	/// Base Json model for a post from a Moebooru site.
	/// </summary>
	public abstract class BooruPost : IPost, ISize
	{
		/// <summary>
		/// The base url of the -booru site. E.G.: https://danbooru.donmai.us or https://www.konachan.com
		/// </summary>
		public abstract Uri BaseUrl { get; }

		/// <inheritdoc />
		public abstract DateTime CreatedAt { get; }

		/// <summary>
		/// The location of the file for this post. May be missing http/https.
		/// </summary>
		[JsonProperty("file_url")]
		public string FileUrl { get; private set; }

		/// <summary>
		/// Whether or not the post has children.
		/// </summary>
		[JsonProperty("has_children")]
		public bool HasChildren { get; private set; }

		/// <inheritdoc />
		public abstract int Height { get; }

		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }

		/// <summary>
		/// The hash of the image.
		/// </summary>
		[JsonProperty("md5")]
		public string Md5 { get; private set; }

		/// <summary>
		/// The id of the parent post to this post if there is one.
		/// </summary>
		[JsonProperty("parent_id")]
		public int? ParentId { get; private set; }

		/// <inheritdoc />
		public abstract Uri PostUrl { get; }

		/// <summary>
		/// The rating of the image. Safe, questionable, or explicit.
		/// </summary>
		[JsonProperty("rating")]
		public char Rating { get; private set; }

		/// <inheritdoc />
		[JsonProperty("score")]
		public int Score { get; private set; } = -1;

		/// <summary>
		/// The source of the image. Can be empty/null if no source is provided.
		/// </summary>
		[JsonProperty("source")]
		public string Source { get; private set; }

		/// <summary>
		/// The tags for this image.
		/// </summary>
		public abstract string Tags { get; }

		/// <inheritdoc />
		public abstract int Width { get; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IDownloaderClient client)
		{
			if (Uri.TryCreate(FileUrl, UriKind.Absolute, out var url)
				|| Uri.TryCreate($"{BaseUrl}{FileUrl}", UriKind.Absolute, out url))
			{
				if (url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps)
				{
					url = new Uri($"https:{url.ToString().Substring(url.ToString().IndexOf("//"))}");
				}

				return Task.FromResult(ImageResponse.FromUrl(url));
			}
			return Task.FromResult(ImageResponse.FromNotFound(PostUrl));
		}

		/// <summary>
		/// Returns the id, width, and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Id} ({Width}x{Height})";
	}
}