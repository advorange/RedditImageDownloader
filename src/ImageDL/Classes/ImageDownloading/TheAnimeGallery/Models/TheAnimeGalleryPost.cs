using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.TheAnimeGallery.Models
{
	/// <summary>
	/// Model for a post from TheAnimeGallery.
	/// </summary>
	public sealed class TheAnimeGalleryPost : IPost, ISize
	{
		/// <inheritdoc />
		[JsonProperty("post_id")]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"http://www.theanimegallery.com/gallery/image:{Id}");
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => VoteCount;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt
		{
			get
			{
				if (DateTime.TryParse(CreatedAtString, out var dt))
				{
					return dt;
				}
				//Try by removing ordinal suffix
				var parts = CreatedAtString.Split(' ');
				parts[1] = parts[1].Remove(Regex.Match(parts[1], "[a-z]", RegexOptions.IgnoreCase).Index, 2);
				if (DateTime.TryParse(String.Join(" ", parts), out dt))
				{
					return dt;
				}
				throw new ArgumentException($"Unable to convert {CreatedAtString} to a datetime.");
			}
		}
		/// <inheritdoc />
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <inheritdoc />
		[JsonProperty("height")]
		public int Height { get; private set; }
		/// <summary>
		/// How many times this image has been downloaded.
		/// </summary>
		[JsonProperty("download_count")]
		public int DownloadCount { get; private set; }
		/// <summary>
		/// How many views this image has.
		/// </summary>
		[JsonProperty("view_count")]
		public int ViewCount { get; private set; }
		/// <summary>
		/// How many votes this image has.
		/// </summary>
		[JsonProperty("vote_count")]
		public int VoteCount { get; private set; }
		/// <summary>
		/// How many favorites this image has.
		/// </summary>
		[JsonProperty("favorite_count")]
		public int FavoriteCount { get; private set; }
		/// <summary>
		/// The string representing when this was uploaded.
		/// </summary>
		[JsonProperty("created_at_string")]
		public string CreatedAtString { get; private set; }
		/// <summary>
		/// The name of the user who uploaded this.
		/// </summary>
		[JsonProperty("username")]
		public string Username { get; private set; }
		/// <summary>
		/// The id of the user who uploaded this.
		/// </summary>
		[JsonProperty("user_id")]
		public string UserId { get; private set; }
		/// <summary>
		/// The category the image is in.
		/// </summary>
		[JsonProperty("category")]
		public string Category { get; private set; }
		/// <summary>
		/// The title of the image.
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; private set; }
		/// <summary>
		/// The series this image is from.
		/// </summary>
		[JsonProperty("series")]
		public string Series { get; private set; }
		/// <summary>
		/// The rating of the image. Safe, erotic, or adult.
		/// </summary>
		[JsonProperty("rating")]
		public string Rating { get; private set; }
		/// <summary>
		/// The tags of the image.
		/// </summary>
		[JsonProperty("tags")]
		public IList<TheAnimeGalleryTag> Tags { get; private set; }
		/// <summary>
		/// The url leading to the image.
		/// </summary>
		[JsonIgnore]
		public Uri ImageUrl { get; private set; }
		/// <summary>
		/// The url leading to the thumbnail.
		/// </summary>
		[JsonProperty("thumbnail_url")]
		public Uri ThumbnailUrl { get; private set; }

		/// <summary>
		/// Creates an instance of <see cref="TheAnimeGalleryPost"/>.
		/// </summary>
		/// <param name="node"></param>
		public TheAnimeGalleryPost(HtmlNode node)
		{
			var div = node.Descendants("div");

			DownloadCount = GetNumber(div.Single(x => x.GetAttributeValue("class", null) == "downloads").InnerText);
			ViewCount = GetNumber(div.Single(x => x.GetAttributeValue("class", null) == "views").InnerText);
			VoteCount = GetNumber(div.Single(x => x.GetAttributeValue("class", null) == "score user").InnerText);
			FavoriteCount = GetNumber(div.Single(x => x.GetAttributeValue("class", null) == "favs user").InnerText);

			var date = div.Single(x => x.GetAttributeValue("class", null) == "date");
			CreatedAtString = date.InnerText.Split('.')[0];

			var userInfo = date.Descendants("a").Single();
			Username = userInfo.InnerText;
			UserId = userInfo.GetAttributeValue("href", null).Split(':').Last();

			var series = div.Single(x => x.GetAttributeValue("class", null) == "seriesGroup");
			var title = series.Descendants("div").Single(x => x.GetAttributeValue("class", null) == "title");
			Title = title.Descendants("h1").Single().InnerText;
			Series = title.Descendants("h2").Single().InnerText;
			Category = title.Descendants("h3").Single().InnerText;
			Rating = title.Descendants("span").Single().GetAttributeValue("class", null).Split(' ').Last();

			var tagList = div.Single(x => x.GetAttributeValue("id", null) == "tagbox");
			Tags = tagList.Descendants("li").Select(x => new TheAnimeGalleryTag(x.Descendants("a").Single())).ToList();

			var download = div.Single(x => x.GetAttributeValue("class", null) == "download");
			var a = download.Descendants("a").First();
			ImageUrl = new Uri($"https://www.theanimegallery.com{a.GetAttributeValue("href", null)}");
			var parts = a.InnerText.Split(new[] { "&times;" }, StringSplitOptions.RemoveEmptyEntries);
			var nums = parts.Select(x => GetNumber(x)).ToArray();
			Width = nums[0];
			Height = nums[1];

			var thumbnail = div.Single(x => x.GetAttributeValue("class", null) == "image");
			var image = thumbnail.Descendants("img").Single(x => x.GetAttributeValue("class", null) == "block");
			ThumbnailUrl = new Uri($"http://www.theanimegallery.com{image.GetAttributeValue("src", null)}");
			Id = Path.GetFileNameWithoutExtension(ThumbnailUrl.ToString()).Split('_').Last();
		}

		private int GetNumber(string input)
		{
			if (!int.TryParse(input.Trim(), NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var num))
			{
				throw new InvalidOperationException($"Unable to parsed a number from the input {input}.");
			}
			return num;
		}
		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IDownloaderClient client)
		{
			return Task.FromResult(ImageResponse.FromUrl(ImageUrl));
		}
		/// <summary>
		/// Returns the post id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Id;
		}
	}
}