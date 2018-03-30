using Newtonsoft.Json;
using System;

namespace ImageDL.Classes.ImageDownloading.Booru.Gelbooru
{
	/// <summary>
	/// Json model (gotten through the Xml endpoint though) for a Gelbooru post.
	/// </summary>
	public sealed class GelbooruPost : BooruPost
	{
#pragma warning disable 1591, 649 //Disabled since most of these are self explanatory and this is a glorified Json model
		[JsonProperty("change")]
		public readonly int Change;
		[JsonProperty("sample_url")]
		public readonly string SampleUrl;
		[JsonProperty("sample_width")]
		public readonly int SampleWidth;
		[JsonProperty("sample_height")]
		public readonly int SampleHeight;
		[JsonProperty("preview_url")]
		public readonly string PreviewUrl;
		[JsonProperty("preview_width")]
		public readonly int PreviewWidth;
		[JsonProperty("preview_height")]
		public readonly int PreviewHeight;
		[JsonProperty("has_notes")]
		public readonly bool HasNotes;
		[JsonProperty("has_comments")]
		public readonly bool HasComments;
		[JsonProperty("status")]
		public readonly string Status;
		[JsonProperty("creator_id")]
		public readonly int CreatorId;

		[JsonProperty("width")]
		private readonly int _Width;
		[JsonProperty("height")]
		private readonly int _Height;
		[JsonProperty("created_at")]
		private readonly DateTime _CreatedAt;
		[JsonProperty("tags")]
		private readonly string _Tags;

		[JsonIgnore]
		public override string BaseUrl => "https://gelbooru.com";
		[JsonIgnore]
		public override string PostUrl => $"{BaseUrl}/index.php?page=post&s=view&id={Id}";
		[JsonIgnore]
		public override int Width => _Width;
		[JsonIgnore]
		public override int Height => _Height;
		[JsonIgnore]
		public override DateTime CreatedAt => _CreatedAt.ToUniversalTime();
		[JsonIgnore]
		public override string Tags => _Tags;
#pragma warning restore 1591, 649
	}
}
