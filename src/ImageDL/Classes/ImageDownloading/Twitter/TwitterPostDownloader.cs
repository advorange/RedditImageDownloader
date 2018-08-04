using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesSettingParser;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth;
using ImageDL.Classes.ImageDownloading.Twitter.Models.Scraped;
using ImageDL.Enums;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter
{
	/// <summary>
	/// Downloads images from Twitter.
	/// </summary>
	[DownloaderName("Twitter")]
	public sealed class TwitterPostDownloader : PostDownloader
	{
		//Source file: https://abs.twimg.com/k/en/init.en.9fe6ea43287284f537d7.js
		private static readonly string _Token = "AAAAAAAAAAAAAAAAAAAAAPYXBAAAAAAACLXUNDekMxqa8h%2F40K4moUkGsoc%3DTYfbDKbT3jJPCEVnMYqilB28NHfOPqkca3qaAxGfsyKCs0wRbw";

		/// <summary>
		/// The term to search for.
		/// </summary>
		public string Search
		{
			get => _Search;
			set => _Search = value;
		}
		/// <summary>
		/// Whether or not to include retweets.
		/// </summary>
		public bool IncludeRetweets
		{
			get => _IncludeRetweets;
			set => _IncludeRetweets = value;
		}
		/// <summary>
		/// The method to gather images with.
		/// </summary>
		public TwitterGatheringMethod GatheringMethod
		{
			get => _GatheringMethod;
			set => _GatheringMethod = value;
		}

		private string _Search;
		private bool _IncludeRetweets;
		private TwitterGatheringMethod _GatheringMethod;

		/// <summary>
		/// Creates an instance of <see cref="TwitterPostDownloader"/>.
		/// </summary>
		public TwitterPostDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Search), }, x => Search = x)
			{
				Description = "The term to search for. Can be a username if the method username is used, otherwise will be searched for regualarly.",
			});
			SettingParser.Add(new Setting<bool>(new[] { nameof(IncludeRetweets), "retweets", }, x => IncludeRetweets = x)
			{
				Description = "Whether or not to include retweets when getting tweets from a user. This does nothing if searching is used instead.",
				IsFlag = true,
				IsOptional = true,
			});
			SettingParser.Add(new Setting<TwitterGatheringMethod>(new[] { nameof(GatheringMethod), "method" }, x => GatheringMethod = x, s => (Enum.TryParse(s, true, out TwitterGatheringMethod result), result))
			{
				Description = "How to gather posts. Will either use the search feature or go through the user's posts.",
			});
		}

		/// <inheritdoc />
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var parsed = new TwitterScrapedPage();
			//Iterate to update the pagination start point.
			for (string min = ""; list.Count < AmountOfPostsToGather && (min == "" || parsed.HasMoreItems); min = parsed.MinimumPosition)
			{
				token.ThrowIfCancellationRequested();
				var query = GenerateQuery(GatheringMethod, Search, min, IncludeRetweets);
				var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();
				if (!result.IsSuccess)
				{
					return;
				}

				parsed = new TwitterScrapedPage(GatheringMethod, result.Value);
				foreach (var post in parsed.Items)
				{
					token.ThrowIfCancellationRequested();
					//Could grab each full post (has media sizes and such) but that only has 1000 reqs per 15 mins
					if (post.CreatedAt < OldestAllowed)
					{
						return;
					}
					if (!post.ImageUrls.Any() || (!IncludeRetweets && post.IsRetweet) || post.FavoriteCount < MinScore)
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
		/// <summary>
		/// Generates a url to search with.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="search"></param>
		/// <param name="min"></param>
		/// <param name="includeRetweets"></param>
		/// <returns></returns>
		private static Uri GenerateQuery(TwitterGatheringMethod method, string search, string min, bool includeRetweets)
		{
			switch (method)
			{
				//Why does Twitter's public search API have to be so terrible?
				//Is it maybe to make people not use it, and instead use the actual API?
				case TwitterGatheringMethod.Search:
					if (min == "")
					{
						return new Uri($"https://twitter.com/search" +
							$"?f=tweets" +
							$"&vertical=default" +
							$"&q={WebUtility.UrlEncode(search)}");
					}
					return new Uri("https://twitter.com/i/search/timeline" +
						$"?f=tweets" +
						$"&vertical=default" +
						$"&include_available_features=1" +
						$"&include_entities=1" +
						$"&reset_error_state=false" +
						$"&src=typd" +
						$"&max_position={min}" +
						$"&q={WebUtility.UrlEncode(search)}");
				case TwitterGatheringMethod.User:
					var query = $"https://twitter.com/i/profiles/show/{search}/" +
						$"{(includeRetweets ? "timeline" : "media_timeline")}" +
						$"?include_available_features=1" +
						$"&include_entities=1" +
						$"&reset_error_state=false";
					if (min != "")
					{
						query += $"&max_position={min}";
					}
					return new Uri(query);
				default:
					throw new ArgumentException("Invalid gathering method supplied.");
			}
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<TwitterOAuthPost> GetTwitterPostAsync(IDownloaderClient client, string id)
		{
			var query = new Uri($"https://api.twitter.com/1.1/statuses/show.json?id={id}");
			var result = await client.GetTextAsync(() =>
			{
				var req = client.GenerateReq(query);
				req.Headers.Add("Authorization", $"Bearer {_Token}");
				return req;
			}, TimeSpan.FromMinutes(15)).CAF();
			return result.IsSuccess ? JsonConvert.DeserializeObject<TwitterOAuthPost>(result.Value) : null;
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetTwitterImagesAsync(IDownloaderClient client, Uri url)
		{
			var u = DownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var search = "/status/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
				if (await GetTwitterPostAsync(client, id).CAF() is TwitterOAuthPost post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}
