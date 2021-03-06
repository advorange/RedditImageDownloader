﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using AdvorangesSettingParser.Implementation.Instance;

using AdvorangesUtils;

using ImageDL.Attributes;
using ImageDL.Classes.ImageDownloading.Eshuushuu.Models;
using ImageDL.Interfaces;

using Newtonsoft.Json.Linq;

using Model = ImageDL.Classes.ImageDownloading.Eshuushuu.Models.EshuushuuPost;

namespace ImageDL.Classes.ImageDownloading.Eshuushuu
{
	/// <summary>
	/// Downloads images from Eshuushuu.
	/// </summary>
	[DownloaderName("E-shuushuu")]
	public sealed class EshuushuuPostDownloader : PostDownloader
	{
		private string _Tags;

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

		/// <summary>
		/// Creates an instance of <see cref="EshuushuuPostDownloader"/>.
		/// </summary>
		public EshuushuuPostDownloader()
		{
			SettingParser.Add(new Setting<string>(() => Tags)
			{
				Description = "The tags to search for. Must be numbers joined with +. Use http://e-shuushuu.net/search/ for help.",
			});
		}

		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetEshuushuuImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			const string search = "/image/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
				if (await GetEshuushuuPostAsync(client, id).CAF() is Model post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}

		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetEshuushuuPostAsync(IDownloaderClient client, string id)
		{
			var query = new Uri($"http://e-shuushuu.net/httpreq.php?mode=show_all_meta&image_id={id}");
			var result = await client.GetHtmlAsync(() => client.GenerateReq(query)).CAF();
			if (!result.IsSuccess)
			{
				return null;
			}

			var jObj = new JObject
			{
				{ nameof(Model.Id).ToLower(), id }
			};

			//dt is the name (datatype?) dd is the value (datadata?)
			//They should always have the same count of each
			var dt = result.Value.DocumentNode.Descendants("dt").ToArray();
			var dd = result.Value.DocumentNode.Descendants("dd").ToArray();
			for (var i = 0; i < dt.Length; ++i)
			{
				var t = dt[i];
				var d = dd[i];

				//Create the name from the title. Remove the colon at th end, replace spaces with underscores, etc
				var name = new string(t.InnerText.Replace(":", "").Replace(" ", "_").ToLower().Where(x => !char.IsWhiteSpace(x)).ToArray());
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
							{ nameof(EshuushuuTag.Value).ToLower(), x.Descendants("a").Single().GetAttributeValue("href", "").Replace("/tags/", "") },
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

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var parsed = new List<Model>();
			//Iterate to get the next page of results
			for (var i = 0; list.Count < AmountOfPostsToGather && (i == 0 || parsed.Count >= 15); ++i)
			{
				token.ThrowIfCancellationRequested();
				var query = new Uri("http://e-shuushuu.net/search/results/" +
					"?thumbs=1" +
					"&hide_disabled=1" +
					$"&tags={WebUtility.UrlEncode(Tags)}" +
					$"&page={i}");
				var result = await client.GetHtmlAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				var div = result.Value.DocumentNode.Descendants("div");
				var thumbnails = div.Where(x => x.GetAttributeValue("class", null) == "display");
				var ids = thumbnails.Select(x => x.Id.TrimStart('i')).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct();
				var tasks = ids.GroupInto(4).Select(async x =>
				{
					var tmp = new List<Model>();
					foreach (var id in x)
					{
						tmp.Add(await GetEshuushuuPostAsync(client, id).CAF());
					}
					return tmp;
				});
				parsed = (await Task.WhenAll(tasks).CAF()).SelectMany(x => x).ToList();
				foreach (var post in parsed)
				{
					token.ThrowIfCancellationRequested();
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (!HasValidSize(post, out _) || post.Score < MinScore)
					{
						continue;
					}
					if (!Add(list, post))
					{
						return;
					}
				}
			}
		}
	}
}