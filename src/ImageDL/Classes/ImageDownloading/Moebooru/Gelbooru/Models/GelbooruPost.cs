using System;
using ImageDL.Classes.ImageDownloading.Moebooru.Models;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Gelbooru.Models
{
	/// <summary>
	/// Json model (gotten through the Xml endpoint though) for a Gelbooru post.
	/// </summary>
	public sealed class GelbooruPost : MoebooruPost
	{
		#region Json
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("change")]
		public readonly int Change;
		/// <summary>
		/// Scaled down version of the file.
		/// </summary>
		[JsonProperty("sample_url")]
		public readonly string SampleUrl;
		/// <summary>
		/// The width of the sample url.
		/// </summary>
		[JsonProperty("sample_width")]
		public readonly int SampleWidth;
		/// <summary>
		/// The height of the sample url.
		/// </summary>
		[JsonProperty("sample_height")]
		public readonly int SampleHeight;
		/// <summary>
		/// Scaled down version of the file to 150px as the biggest side.
		/// </summary>
		[JsonProperty("preview_url")]
		public readonly string PreviewUrl;
		/// <summary>
		/// The width of the preview url.
		/// </summary>
		[JsonProperty("preview_width")]
		public readonly int PreviewWidth;
		/// <summary>
		/// The height of the preview url.
		/// </summary>
		[JsonProperty("preview_height")]
		public readonly int PreviewHeight;
		/// <summary>
		/// Whether the post has any notes.
		/// </summary>
		[JsonProperty("has_notes")]
		public readonly bool HasNotes;
		/// <summary>
		/// Whether the post has any comments.
		/// </summary>
		[JsonProperty("has_comments")]
		public readonly bool HasComments;
		/// <summary>
		/// The status of the image, e.g. active, etc.
		/// </summary>
		[JsonProperty("status")]
		public readonly string Status;
		/// <summary>
		/// The id of the person who submitted it.
		/// </summary>
		[JsonProperty("creator_id")]
		public readonly int CreatorId;
		[JsonProperty("width")]
		private readonly int _Width = -1;
		[JsonProperty("height")]
		private readonly int _Height = -1;
		[JsonProperty("created_at")]
		private readonly DateTime _CreatedAt = new DateTime(1970, 1, 1);
		[JsonProperty("tags")]
		private readonly string _Tags = null;
		#endregion

		/// <inheritdoc />
		public override Uri BaseUrl => new Uri("https://gelbooru.com");
		/// <inheritdoc />
		public override Uri PostUrl => new Uri($"{BaseUrl}/index.php?page=post&s=view&id={Id}");
		/// <inheritdoc />
		public override DateTime CreatedAt => _CreatedAt.ToUniversalTime();
		/// <inheritdoc />
		public override int Width => _Width;
		/// <inheritdoc />
		public override int Height => _Height;
		/// <inheritdoc />
		public override string Tags => _Tags;
	}
}