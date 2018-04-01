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
using System.Net.Http;
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
		public EshuushuuImageDownloader() : base("Eshuushuu")
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Tags), }, x => Tags = x)
			{
				Description = "The tags to search for. Must be numbers joined with +. Use http://e-shuushuu.net/search/ for help.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(List<EshuushuuPost> list)
		{
			var parsed = new List<EshuushuuPost>();
			var keepGoing = true;
			//Iterate to get the next page of results
			for (int i = 0; keepGoing && list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 15); ++i)
			{
				var result = await Client.GetMainTextAndRetryIfRateLimitedAsync(GenerateQuery(i)).CAF();
				if (!result.IsSuccess)
				{
					break;
				}

				var doc = new HtmlDocument();
				doc.LoadHtml(result.Text);

				parsed = (await Task.WhenAll(doc.DocumentNode.Descendants("div")
					.Where(x => x.GetAttributeValue("class", null) == "display")
					.Select(x => x.Id.TrimStart('i'))
					.Where(x => !String.IsNullOrWhiteSpace(x))
					.Distinct()
					.Select(async x => await Parse(Convert.ToInt32(x)).CAF())).CAF()).ToList();
				foreach (var post in parsed)
				{
					if (!(keepGoing = post.SubmittedOn >= OldestAllowed))
					{
						break;
					}
					else if (!FitsSizeRequirements(null, post.Width, post.Height, out _) || post.Favorites < MinScore)
					{
						continue;
					}
					else if (!(keepGoing = Add(list, post)))
					{
						break;
					}
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

		private Uri GenerateQuery(int page)
		{
			return new Uri("http://e-shuushuu.net/search/results/" +
				"?thumbs=1" +
				"&hide_disabled=1" +
				$"&tags={WebUtility.UrlEncode(Tags)}" +
				$"&page={page}");
		}
		private async Task<EshuushuuPost> Parse(int id)
		{
			var query = $"http://e-shuushuu.net/httpreq.php?mode=show_all_meta&image_id={id}";
			var result = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri(query)).CAF();
			if (!result.IsSuccess)
			{
				throw new HttpRequestException($"Unable to parse the Eshuushuu post {id}");
			}

			var doc = new HtmlDocument();
			doc.LoadHtml(result.Text);

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
						return new JObject
						{
							{ nameof(Tag.Value).ToLower(), x.Descendants("a").Single().GetAttributeValue("href", null).Replace("/tags/", "") },
							{ nameof(Tag.Name).ToLower(), x.InnerText.Replace("\"", "") },
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
	}
}