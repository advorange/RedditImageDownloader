using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ImageDL.Enums;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Eshuushuu.Models
{
	/// <summary>
	/// Json model for a post from Eshuushuu.
	/// </summary>
	public sealed class EshuushuuPost : IPost
	{
		/// <inheritdoc />
		[JsonProperty(nameof(Id))]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"http://e-shuushuu.net/image/{Id}");
		/// <inheritdoc />
		[JsonProperty("favorites")]
		public int Score { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
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
		/// The size of the file in bytes.
		/// </summary>
		[JsonIgnore]
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
		[JsonIgnore]
		public int Width => Convert.ToInt32(DimensionsString.Split('x', ' ')[0]);
		/// <summary>
		/// The height of the image.
		/// </summary>
		[JsonIgnore]
		public int Height => Convert.ToInt32(DimensionsString.Split('x', ' ')[1]);
		/// <summary>
		/// Who the post was submitted by.
		/// </summary>
		[JsonProperty("submitted_by")]
		public string SubmittedBy { get; private set; }
		/// <summary>
		/// The name of the image's file.
		/// </summary>
		[JsonProperty("filename")]
		public string Filename { get; private set; }
		/// <summary>
		/// The file name that was originally uploaded.
		/// </summary>
		[JsonProperty("original_filename")]
		public string OriginalFilename { get; private set; }
		/// <summary>
		/// The tags on the image (not source, character, or artist tags).
		/// </summary>
		[JsonProperty("tags")]
		public IList<EshuushuuTag> Tags { get; private set; }
		/// <summary>
		/// Tags on where the characters are from.
		/// </summary>
		[JsonProperty("source")]
		public IList<EshuushuuTag> Source { get; private set; }
		/// <summary>
		/// Tags on the characters in the image.
		/// </summary>
		[JsonProperty("characters")]
		public IList<EshuushuuTag> Characters { get; private set; }
		/// <summary>
		/// Tags on who made the image.
		/// </summary>
		[JsonProperty("artist")]
		public IList<EshuushuuTag> Artist { get; private set; }
		/// <summary>
		/// When the image was submitted. This string is not friendly to convert from.
		/// </summary>
		[JsonProperty("submitted_on")]
		public string SubmittedOnString { get; private set; }
		/// <summary>
		/// The size of the file. This string is not friendly to convert from.
		/// </summary>
		[JsonProperty("file_size")]
		public string FileSizeString { get; private set; }
		/// <summary>
		/// The dimensions of the image. This string is not friendly to convert from.
		/// </summary>
		[JsonProperty("dimensions")]
		public string DimensionsString { get; private set; }
		/// <summary>
		/// The rating of the image.
		/// </summary>
		[JsonProperty("image_rating")]
		public string ImageRating { get; private set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			return Task.FromResult(ImageResponse.FromUrl(new Uri($"http://e-shuushuu.net/images/{Filename}")));
		}
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