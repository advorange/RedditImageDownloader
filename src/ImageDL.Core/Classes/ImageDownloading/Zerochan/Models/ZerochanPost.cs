using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using AdvorangesUtils;

using ImageDL.Interfaces;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Zerochan.Models
{
	/// <summary>
	/// Json model for a Zerochan post.
	/// </summary>
	public sealed class ZerochanPost : IPost, ISize
	{
		/// <summary>
		/// The name of whoever posted this.
		/// </summary>
		[JsonProperty("author")]
		public string Author { get; private set; }

		/// <summary>
		/// The size of the file in bytes.
		/// </summary>
		[JsonIgnore]
		public long ContentSize
		{
			get
			{
				//TODO: better way
				var val = Convert.ToDouble(new string(ContentSizeString.Where(x => char.IsDigit(x)).ToArray()));
				return (char.ToLower(ContentSizeString.First(x => !char.IsDigit(x)))) switch
				{
					'k' => (long)(val * 1024),
					'm' => (long)(val * 1024 * 1024),
					'g' => (long)(val * 1024 * 1024 * 1024),
					_ => (long)val,
				};
			}
		}

		/// <summary>
		/// The file size.
		/// </summary>
		[JsonProperty("contentSize")]
		public string ContentSizeString { get; private set; }

		/// <summary>
		/// Url to the image.
		/// </summary>
		[JsonProperty("contentUrl")]
		public Uri ContentUrl { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt
		{
			get
			{
				const string FORMAT = "ddd MMM d HH:mm:ss yyyy";
				var createdAt = CreatedAtString.Replace("  ", " "); //Not sure why there's a double space in it
				if (DateTime.TryParse(createdAt, out var dt))
				{
					return dt;
				}
				if (DateTime.TryParseExact(createdAt, FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
				{
					return dt;
				}
				throw new ArgumentException($"Unable to convert {CreatedAtString} to a datetime.");
			}
		}

		/// <summary>
		/// A string representing when this was created.
		/// </summary>
		[JsonProperty("datePublished")]
		public string CreatedAtString { get; private set; }

		/// <summary>
		/// The description of the image.
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; private set; }

		/// <summary>
		/// The image format, e.g. jpg, png, etc.
		/// </summary>
		[JsonProperty("encodingFormat")]
		public string EncodingFormat { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public int Height => Convert.ToInt32(HeightString.Split(' ')[0]);

		/// <summary>
		/// The height with px added at the end.
		/// </summary>
		[JsonProperty("height")]
		public string HeightString { get; private set; }

		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }

		/// <summary>
		/// The name of the image.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://www.zerochan.net/{Id}");

		/// <inheritdoc />
		[JsonIgnore]
		public int Score
		{
			get
			{
				//Usually in the format of something like this:
				//View and download this 990x700 DURARARA!! image with 2192 favorites, or browse the gallery.
				const string search = "favorites";
				if (Description.CaseInsIndexOf(search, out var index))
				{
					var parts = Description.Substring(0, index).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					return Convert.ToInt32(parts.Last());
				}
				return -1;
			}
		}

		/// <summary>
		/// The tags of the image.
		/// </summary>
		[JsonProperty("tags")]
		public IList<string> Tags { get; private set; }

		/// <summary>
		/// Url to the image's thumbnail.
		/// </summary>
		[JsonProperty("thumbnail")]
		public Uri Thumbnail { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public int Width => Convert.ToInt32(WidthString.Split(' ')[0]);

		/// <summary>
		/// The width with px added at the end.
		/// </summary>
		[JsonProperty("width")]
		public string WidthString { get; private set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IDownloaderClient client) => Task.FromResult(ImageResponse.FromUrl(ContentUrl));

		/// <summary>
		/// Returns the id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Id;
	}
}