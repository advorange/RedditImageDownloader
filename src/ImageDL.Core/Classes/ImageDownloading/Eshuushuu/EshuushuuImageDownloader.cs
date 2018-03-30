using AdvorangesUtils;
using HtmlAgilityPack;
using ImageDL.Classes.ImageScraping;
using ImageDL.Classes.SettingParsing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloading.Eshuushuu
{
	/// <summary>
	/// Downloads images from Eshuushuu.
	/// </summary>
	public sealed class EshuushuuImageDownloader : ImageDownloader<EshuushuuPost>
	{
		/// <summary>
		/// The terms to search for. Joined by +, must be numbers.
		/// </summary>
		public string Tags
		{
			get => _Tags;
			set
			{
				if (value.Split('+').Any(x => !int.TryParse(x, out _)))
				{
					throw new ArgumentException("Invalid tags supplied. Must only be numbers with + in between them.");
				}
				_Tags = value;
			}
		}

		private string _Tags;

		/// <summary>
		/// Creates an instance of <see cref="EshuushuuImageDownloader"/>.
		/// </summary>
		public EshuushuuImageDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Tags), }, x => Tags = x)
			{
				Description = "The tags to search for. Must be numbers joined with +. Use http://e-shuushuu.net/search/ for help.",
			});
		}

		private async Task<EshuushuuPost> Parse(int id)
		{
			var query = $"http://e-shuushuu.net/httpreq.php?mode=show_all_meta&image_id={id}";
			var html = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri(query), TimeSpan.FromSeconds(2)).CAF();

			var doc = new HtmlDocument();
			doc.LoadHtml(html);

			var jObj = new JObject
			{
				{ "post_url", $"http://e-shuushuu.net/image/{id}/" },
				{ "post_id", id }
			};

			//dt is the name (datatype?) dd is the value (datadata?)
			//They should always have the same count of each
			var dt = doc.DocumentNode.Descendants("dt").ToArray();
			var dd = doc.DocumentNode.Descendants("dd").ToArray();
			for (int i = 0; i < dt.Count(); ++i)
			{
				var t = dt[i];
				var d = dd[i];

				//Create the name from the title. Remove the colon at th end, replace spaces with underscores, etc
				var name = new string(t.InnerText.Replace(":", "").Replace(" ", "_").ToLower().Where(x => !Char.IsWhiteSpace(x)).ToArray());
				var span = d.Descendants("span");
				//If no span children then it's just pure text
				if (!span.Any())
				{
					jObj.Add(name, d.InnerText);
					continue;
				}

				var tags = span.Where(x => x.GetAttributeValue("class", null) == "tag");
				if (tags.Any()) //Any tags means to just add them as a list
				{
					jObj.Add(name, new JArray(tags.Select(x =>
					{
						var a = x.Descendants("a").Single();
						return new JObject
						{
							{ nameof(EshuushuuPost.Tag.Value).ToLower(), a.GetAttributeValue("href", null).Replace("/tags/", "") },
							{ nameof(EshuushuuPost.Tag.Name).ToLower(), x.InnerText.Replace("\"", "") },
						};
					}).ToArray()));
				}
				else //If no tags then it's most likely whoever submitted it
				{
					jObj.Add(name, span.First().InnerText);
				}
			}
			return jObj.ToObject<EshuushuuPost>();
		}
		/// <inheritdoc />
		protected override async Task GatherPostsAsync(List<EshuushuuPost> posts)
		{
			//Uses for instead of while to save 2 lines.
			for (int i = 0; posts.Count < AmountToDownload; ++i)
			{
				var finished = false;
				//Cap of 15 per page, keep incrementing to get more
				var html = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri(GenerateQuery(i)), TimeSpan.FromSeconds(2)).CAF();
				//Parse each post from the html
				var doc = new HtmlDocument();
				doc.LoadHtml(html);

				var results = doc.DocumentNode.Descendants("div").Where(x => x.GetAttributeValue("class", null) == "display");
				var ids = results.Select(x => x.Id.TrimStart('i'))
					.Where(x => !String.IsNullOrWhiteSpace(x))
					.Select(x => Convert.ToInt32(x))
					.Distinct();
				foreach (var id in ids)
				{
					var post = await Parse(id).CAF();
					if (post.SubmittedOn < OldestAllowed)
					{
						finished = true;
						break;
					}
					else if (!FitsSizeRequirements(null, post.Width, post.Height, out _) || post.Favorites < MinScore)
					{
						continue;
					}

					posts.Add(post);
					if (posts.Count == AmountToDownload)
					{
						finished = true;
						break;
					}
					else if (posts.Count % 25 == 0)
					{
						Console.WriteLine($"{posts.Count} Eshuushuu posts found.");
					}
				}

				//Anything less than a full page means everything's been searched
				if (finished || ids.Count() < 15)
				{
					break;
				}
			}
		}
		/// <inheritdoc />
		protected override List<EshuushuuPost> OrderAndRemoveDuplicates(List<EshuushuuPost> list)
		{
			return list.OrderByDescending(x => x.Favorites).ToList();
		}
		/// <inheritdoc />
		protected override void WritePostToConsole(EshuushuuPost post, int count)
		{
			Console.WriteLine($"[#{count}|\u2191{post.Favorites}] {post.PostUrl}");
		}
		/// <inheritdoc />
		protected override FileInfo GenerateFileInfo(EshuushuuPost post, Uri uri)
		{
			var extension = Path.GetExtension(uri.LocalPath);
			var name = $"{post.PostId}_" +
				$"{String.Join("_", post.Artist.Select(x => x.Name))}_" +
				$"{String.Join("_", post.Characters.Select(x => x.Name))}".Replace(' ', '_');
			return GenerateFileInfo(Directory, name, extension);
		}
		/// <inheritdoc />
		protected override async Task<ScrapeResult> GatherImagesAsync(EshuushuuPost post)
		{
			return await Client.ScrapeImagesAsync(new Uri($"http://e-shuushuu.net/images/{post.Filename}")).CAF();
		}
		/// <inheritdoc />
		protected override ContentLink CreateContentLink(EshuushuuPost post, Uri uri, string reason)
		{
			return new ContentLink(uri, post.Favorites, reason);
		}

		private string GenerateQuery(int page)
		{
			return "http://e-shuushuu.net/search/results/" +
				"?thumbs=1" +
				"&hide_disabled=1" +
				$"&tags={WebUtility.UrlEncode(Tags)}" +
				$"&page={page}";
		}
	}
}
