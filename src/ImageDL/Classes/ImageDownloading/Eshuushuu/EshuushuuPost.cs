#pragma warning disable 1591
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImageDL.Classes.ImageDownloading.Eshuushuu
{
	/// <summary>
	/// Json model for a post from Eshuushuu.
	/// </summary>
	public sealed class EshuushuuPost
	{
		[JsonProperty("post_url")]
		public readonly string PostUrl;
		[JsonProperty("post_id")]
		public readonly int PostId;
		[JsonProperty("submitted_by")]
		public readonly string SubmittedBy;
		[JsonProperty("filename")]
		public readonly string Filename;
		[JsonProperty("original_filename")]
		public readonly string OriginalFilename;
		[JsonProperty("favorites")]
		public readonly int Favorites;
		[JsonProperty("tags")]
		public readonly List<Tag> Tags;
		[JsonProperty("source")]
		public readonly List<Tag> Source;
		[JsonProperty("characters")]
		public readonly List<Tag> Characters;
		[JsonProperty("artist")]
		public readonly List<Tag> Artist;
		[JsonProperty("image_rating")]
		private readonly string _ImageRating = null;
		[JsonProperty("submitted_on")]
		private readonly string _SubmittedOn = null;
		[JsonProperty("file_size")]
		private readonly string _FileSize = null;
		[JsonProperty("dimensions")]
		private readonly string _Dimensions = null;

		[JsonIgnore]
		public string ImageRating => new string(_ImageRating.Where(x => !Char.IsWhiteSpace(x)).ToArray());
		[JsonIgnore]
		public DateTime SubmittedOn
		{
			get
			{
				//Try regularly
				if (DateTime.TryParse(_SubmittedOn, out var dt))
				{
					return dt;
				}
				//Try by removing ordinal suffix
				var parts = _SubmittedOn.Split(' ');
				parts[1] = parts[1].Remove(Regex.Match(parts[1], "[a-z]", RegexOptions.IgnoreCase).Index, 2);
				if (DateTime.TryParse(String.Join(" ", parts), out dt))
				{
					return dt;
				}
				throw new ArgumentException("Unable to parse the submitted on datetime.");
			}
		}
		[JsonIgnore]
		public long FileSize
		{
			get
			{
				var parts = _FileSize.Split(' ');
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
		[JsonIgnore]
		public int Width => Convert.ToInt32(_Dimensions.Split('x', ' ')[0]);
		[JsonIgnore]
		public int Height => Convert.ToInt32(_Dimensions.Split('x', ' ')[1]);

		public override string ToString()
		{
			return PostId.ToString();
		}
	}

	/// <summary>
	/// Holds the value and name of a tag.
	/// </summary>
	public struct Tag
	{
		[JsonProperty("value")]
		public readonly int Value;
		[JsonProperty("name")]
		public readonly string Name;
	}
}