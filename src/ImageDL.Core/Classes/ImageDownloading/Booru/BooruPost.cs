using Newtonsoft.Json;
using System;

namespace ImageDL.Classes.ImageDownloading.Booru
{
	/// <summary>
	/// Base Json model for a post from a -booru site.
	/// </summary>
	public abstract class BooruPost
	{
#pragma warning disable 1591
		[JsonProperty("id")]
		public readonly int Id;
		[JsonProperty("created_at")]
		public readonly DateTime CreatedAt;
		[JsonProperty("source")]
		public readonly string Source;
		[JsonProperty("score")]
		public readonly int Score;
		[JsonProperty("md5")]
		public readonly string MD5;
		[JsonProperty("file_size")]
		public readonly long FileSize;
		[JsonProperty("file_url")]
		public readonly string FileUrl;
		[JsonProperty("rating")]
		public readonly char Rating;
		[JsonProperty("has_children")]
		public readonly bool HasChildren;
		[JsonProperty("parent_id")]
		public readonly int? ParentId;
		public abstract int Width { get; }
		public abstract int Height { get; }
#pragma warning restore 1591
	}
}
