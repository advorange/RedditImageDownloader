using System;
using System.Linq;
using ImageDL.Classes.ImageDownloading.Moebooru.Models;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Gelbooru.Models
{
	/// <summary>
	/// Json model (gotten through the Xml endpoint though) for a Gelbooru post.
	/// </summary>
	public class GelbooruPost : MoebooruPost
	{
		/// <inheritdoc />
		[JsonIgnore]
		public override Uri BaseUrl => new Uri("https://gelbooru.com");
		/// <inheritdoc />
		[JsonIgnore]
		public override Uri PostUrl => new Uri($"{BaseUrl}index.php?page=post&s=view&id={Id}");
		/// <inheritdoc />
		[JsonIgnore]
		public override DateTime CreatedAt
		{
			get
			{
				if (DateTime.TryParse(CreatedAtString, out var dt))
				{
					return dt;
				}
				var parts = CreatedAtString.Split(' ').ToList();
				parts.Insert(3, parts.Last());
				if (DateTime.TryParse(String.Join(" ", parts.Take(parts.Count - 1)), out dt))
				{
					return dt;
				}
				throw new ArgumentException($"Unable to convert {CreatedAtString} to a datetime.");
			}
		}
		/// <inheritdoc />
		[JsonIgnore]
		public override int Width => _Width;
		/// <inheritdoc />
		[JsonIgnore]
		public override int Height => _Height;
		/// <inheritdoc />
		[JsonIgnore]
		public override string Tags => _Tags;
		/// <summary>
		/// No clue.
		/// </summary>
		[JsonProperty("change")]
		public int Change { get; private set; }
		/// <summary>
		/// Scaled down version of the file.
		/// </summary>
		[JsonProperty("sample_url")]
		public string SampleUrl { get; private set; }
		/// <summary>
		/// The width of the sample url.
		/// </summary>
		[JsonProperty("sample_width")]
		public int SampleWidth { get; private set; }
		/// <summary>
		/// The height of the sample url.
		/// </summary>
		[JsonProperty("sample_height")]
		public int SampleHeight { get; private set; }
		/// <summary>
		/// Scaled down version of the file to 150px as the biggest side.
		/// </summary>
		[JsonProperty("preview_url")]
		public string PreviewUrl { get; private set; }
		/// <summary>
		/// The width of the preview url.
		/// </summary>
		[JsonProperty("preview_width")]
		public int PreviewWidth { get; private set; }
		/// <summary>
		/// The height of the preview url.
		/// </summary>
		[JsonProperty("preview_height")]
		public int PreviewHeight { get; private set; }
		/// <summary>
		/// Whether the post has any notes.
		/// </summary>
		[JsonProperty("has_notes")]
		public bool HasNotes { get; private set; }
		/// <summary>
		/// Whether the post has any comments.
		/// </summary>
		[JsonProperty("has_comments")]
		public bool HasComments { get; private set; }
		/// <summary>
		/// The status of the image, e.g. active, etc.
		/// </summary>
		[JsonProperty("status")]
		public string Status { get; private set; }
		/// <summary>
		/// The id of the person who submitted it.
		/// </summary>
		[JsonProperty("creator_id")]
		public int CreatorId { get; private set; }
		/// <summary>
		/// String representation of when the post was created at.
		/// </summary>
		[JsonProperty("created_at")]
		public string CreatedAtString;

		[JsonProperty("width")]
		private int _Width = -1;
		[JsonProperty("height")]
		private int _Height = -1;
		[JsonProperty("tags")]
		private string _Tags = null;
	}
}