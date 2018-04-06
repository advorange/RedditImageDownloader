using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Eshuushuu.Models
{
	/// <summary>
	/// Json model for a post from Eshuushuu.
	/// </summary>
	public sealed class EshuushuuPost : IPost
	{
		#region Json
		/// <summary>
		/// Who the post was submitted by.
		/// </summary>
		[JsonProperty("submitted_by")]
		public readonly string SubmittedBy;
		/// <summary>
		/// The name of the image's file.
		/// </summary>
		[JsonProperty("filename")]
		public readonly string Filename;
		/// <summary>
		/// The file name that was originally uploaded.
		/// </summary>
		[JsonProperty("original_filename")]
		public readonly string OriginalFilename;
		/// <summary>
		/// The tags on the image (not source, character, or artist tags).
		/// </summary>
		[JsonProperty("tags")]
		public readonly List<EshuushuuTag> Tags;
		/// <summary>
		/// Tags on where the characters are from.
		/// </summary>
		[JsonProperty("source")]
		public readonly List<EshuushuuTag> Source;
		/// <summary>
		/// Tags on the characters in the image.
		/// </summary>
		[JsonProperty("characters")]
		public readonly List<EshuushuuTag> Characters;
		/// <summary>
		/// Tags on who made the image.
		/// </summary>
		[JsonProperty("artist")]
		public readonly List<EshuushuuTag> Artist;
		/// <summary>
		/// When the image was submitted. This string is not friendly to convert from.
		/// </summary>
		[JsonProperty("submitted_on")]
		public readonly string SubmittedOnString;
		/// <summary>
		/// The size of the file. This string is not friendly to convert from.
		/// </summary>
		[JsonProperty("file_size")]
		public readonly string FileSizeString;
		/// <summary>
		/// The dimensions of the image. This string is not friendly to convert from.
		/// </summary>
		[JsonProperty("dimensions")]
		public readonly string DimensionsString;
		/// <summary>
		/// How many people favorited the image.
		/// </summary>
		[JsonProperty("favorites")]
		public readonly int Favorites;
		/// <summary>
		/// The id of th post.
		/// </summary>
		[JsonProperty(nameof(Id))]
		private readonly string _Id = null;
		/// <summary>
		/// The rating of the image.
		/// </summary>
		[JsonProperty("image_rating")]
		private readonly string _ImageRating = null;
		#endregion

		/// <inheritdoc />
		public string Id => _Id;
		/// <inheritdoc />
		public Uri PostUrl => new Uri($"http://e-shuushuu.net/image/{Id}");
		/// <inheritdoc />
		public IEnumerable<Uri> ContentUrls => new[] { new Uri($"http://e-shuushuu.net/images/{Filename}") };
		/// <inheritdoc />
		public int Score => Favorites;
		/// <inheritdoc />
		public DateTime CreatedAt
		{
			get
			{
				//Try regularly
				if (DateTime.TryParse(SubmittedOnString, out var dt))
				{
					return dt;
				}
				//Try by removing ordinal suffix
				var parts = SubmittedOnString.Split(' ');
				parts[1] = parts[1].Remove(Regex.Match(parts[1], "[a-z]", RegexOptions.IgnoreCase).Index, 2);
				if (DateTime.TryParse(String.Join(" ", parts), out dt))
				{
					return dt;
				}
				throw new ArgumentException("Unable to parse the submitted on datetime.");
			}
		}
		/// <summary>
		/// Not sure since I haven't seen anything that's not N/A, but I assume it's similar to Moebooru's ratings (questionable, etc).
		/// </summary>
		public string ImageRating => new string(_ImageRating.Where(x => !Char.IsWhiteSpace(x)).ToArray());
		/// <summary>
		/// The size of the file in bytes.
		/// </summary>
		public long FileSize
		{
			get
			{
				var parts = FileSizeString.Split(' ');
				var val = Convert.ToDouble(parts[0]);
				switch (parts[1].ToLower()[0])
				{
					case 'k':
						return (long)(val * 1024);
					case 'm':
						return (long)(val * 1024 * 1024);
					case 'g': //Doubt gigabytes are ever going to be relevant.
						return (long)(val * 1024 * 1024 * 1024);
					default: //Bytes by themselves probably not relevant either.
						return (long)val;
				}
			}
		}
		/// <summary>
		/// The width of the image.
		/// </summary>
		public int Width => Convert.ToInt32(DimensionsString.Split('x', ' ')[0]);
		/// <summary>
		/// The height of the image.
		/// </summary>
		public int Height => Convert.ToInt32(DimensionsString.Split('x', ' ')[1]);

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Id} ({Width}x{Height})";
		}
	}
}