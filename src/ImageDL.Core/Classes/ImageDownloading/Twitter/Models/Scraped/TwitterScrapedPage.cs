﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using ImageDL.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.Scraped
{
	/// <summary>
	/// Json model for the results of a Twitter page.
	/// </summary>
	public struct TwitterScrapedPage
	{
		/// <summary>
		/// Whether more items can be gotten.
		/// </summary>
		[JsonProperty("has_more_items")]
		public bool HasMoreItems { get; private set; }

		/// <summary>
		/// The posts from this iteration.
		/// </summary>
		[JsonProperty("items")]
		public IList<TwitterScrapedPost> Items { get; private set; }

		/// <summary>
		/// The minimum position, used in pagination.
		/// </summary>
		[JsonProperty("min_position")]
		public string MinimumPosition { get; private set; }

		/// <summary>
		/// Creates an instance of <see cref="TwitterScrapedPage"/>.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="text"></param>
		public TwitterScrapedPage(TwitterGatheringMethod method, string text)
		{
			switch (method)
			{
				//Brackets because otherwise jObj is in same space each time
				case TwitterGatheringMethod.Search:
				{
					try
					{
						var jObj = JObject.Parse(text);
						Items = GetPostsFromHtml(UnescapeHtml(jObj["items_html"].ToObject<string>()));
						MinimumPosition = jObj["min_position"].ToObject<string>();
						HasMoreItems = Items.Count > 0; //For some reason this field is always false in the json
					}
					catch (JsonReaderException)
					{
						Items = GetPostsFromHtml(text);
						MinimumPosition = $"TWEET-{Items.Last().Id}-{Items[0].Id}";
						HasMoreItems = true; //Just assume it's true
					}
					return;
				}
				case TwitterGatheringMethod.User:
				{
					var jObj = JObject.Parse(text);
					Items = GetPostsFromHtml(UnescapeHtml(jObj["items_html"].ToObject<string>()));
					MinimumPosition = jObj["min_position"]?.ToObject<string>() ?? Items.Last().Id;
					HasMoreItems = jObj["has_more_items"].ToObject<bool>();
					return;
				}
				default:
					throw new ArgumentException("Invalid method supplied.");
			}
		}

		/// <summary>
		/// Gets the posts from the html.
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		public static List<TwitterScrapedPost> GetPostsFromHtml(string html)
		{
			var doc = new HtmlDocument();
			doc.LoadHtml(html);

			var li = doc.DocumentNode.Descendants("li");
			var tweets = li.Where(x => x.GetAttributeValue("class", "").Contains("js-stream-item") && x.GetAttributeValue("data-item-type", null) == "tweet");
			var list = new List<TwitterScrapedPost>();
			foreach (var tweet in tweets)
			{
				var obj = new TwitterScrapedPost(tweet);
				if (obj.Id != null)
				{
					list.Add(obj);
				}
			}
			return list;
		}

		/// <summary>
		/// Unescapes the html correctly.
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		public static string UnescapeHtml(string html)
		{
			static string Evaluate(Match m)
				=> ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();

			return Regex.Replace(WebUtility.HtmlDecode(html), @"\\u(?<Value>[a-zA-Z0-9]{4})", Evaluate);
		}
	}
}