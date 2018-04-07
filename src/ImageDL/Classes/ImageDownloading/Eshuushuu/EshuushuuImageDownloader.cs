using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading.Eshuushuu.Models;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Classes.ImageDownloading.Eshuushuu.Models.EshuushuuPost;

namespace ImageDL.Classes.ImageDownloading.Eshuushuu
{
	/// <summary>
	/// Downloads images from Eshuushuu.
	/// </summary>
	public sealed class EshuushuuImageDownloader : ImageDownloader<Model>
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
		public EshuushuuImageDownloader() : base("E-shuushuu")
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Tags), }, x => Tags = x)
			{
				Description = "The tags to search for. Must be numbers joined with +. Use http://e-shuushuu.net/search/ for help.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(IImageDownloaderClient client, List<Model> list)
		{
			var parsed = new List<Model>();
			var keepGoing = true;
			//Iterate to get the next page of results
			for (int i = 0; keepGoing && list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 15); ++i)
			{
				var query = new Uri("http://e-shuushuu.net/search/results/" +
					$"?thumbs=1" +
					$"&hide_disabled=1" +
					$"&tags={WebUtility.UrlEncode(Tags)}" +
					$"&page={i}");
				var result = await client.GetHtml(client.GetReq(query)).CAF();
				if (!result.IsSuccess)
				{
					break;
				}

				parsed = (await Task.WhenAll(result.Value.DocumentNode.Descendants("div")
					.Where(x => x.GetAttributeValue("class", null) == "display")
					.Select(x => x.Id.TrimStart('i'))
					.Where(x => !String.IsNullOrWhiteSpace(x))
					.Distinct()
					.Select(async x => await GetEshuushuuPostAsync(client, x).CAF())).CAF()).ToList();
				foreach (var post in parsed)
				{
					if (!(keepGoing = post.CreatedAt >= OldestAllowed))
					{
						break;
					}
					else if (!HasValidSize(null, post.Width, post.Height, out _) || post.Score < MinScore)
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

		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetEshuushuuPostAsync(IImageDownloaderClient client, string id)
		{
			var query = new Uri($"http://e-shuushuu.net/httpreq.php?mode=show_all_meta&image_id={id}");
			var result = await client.GetHtml(client.GetReq(query)).CAF();
			if (!result.IsSuccess)
			{
				return null;
			}

			var jObj = new JObject
			{
				{ nameof(Model.Id), id }
			};

			//dt is the name (datatype?) dd is the value (datadata?)
			//They should always have the same count of each
			var dt = result.Value.DocumentNode.Descendants("dt").ToArray();
			var dd = result.Value.DocumentNode.Descendants("dd").ToArray();
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
					jObj.Add(name, d.InnerText.Trim());
					continue;
				}

				var tags = span.Where(x => x.GetAttributeValue("class", null) == "tag");
				if (tags.Any()) //Any tags means to just add them as a list
				{
					jObj.Add(name, new JArray(tags.Select(x =>
					{
						return new JObject
						{
							{ nameof(EshuushuuTag.Value).ToLower(), x.Descendants("a").Single().GetAttributeValue("href", null).Replace("/tags/", "") },
							{ nameof(EshuushuuTag.Name).ToLower(), x.InnerText.Replace("\"", "") },
						};
					}).ToArray()));
				}
				else //If no tags then it's most likely whoever submitted it
				{
					jObj.Add(name, span.First().InnerText.Trim());
				}
			}
			return jObj.ToObject<Model>();
		}
	}
}