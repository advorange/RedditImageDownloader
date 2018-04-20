using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
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
		/// The lowest value id of the items.
		/// </summary>
		[JsonProperty("min_position")]
		public string MinimumPosition
		{
			get => _MinimumPosition ?? ItemIds.Last();
			private set => _MinimumPosition = value;
		}
		/// <summary>
		/// Whether more items can be gotten.
		/// </summary>
		[JsonProperty("has_more_items")]
		public bool HasMoreItems { get; private set; }
		/// <summary>
		/// The html of the items.
		/// </summary>
		[JsonProperty("items_html")]
		public string ItemsHtml
		{
			get => _ItemsHtml;
			private set
			{
				_ItemsHtml = Regex.Replace(Regex.Unescape(WebUtility.HtmlDecode(value)), @"\\u(?<Value>[a-zA-Z0-9]{4})", m =>
				{
					return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
				});

				//TODO: parse manually instead of loading inside html doc?
				var doc = new HtmlDocument();
				doc.LoadHtml(_ItemsHtml);

				var li = doc.DocumentNode.Descendants("li");
				var items = li.Where(x => x.GetAttributeValue("class", "").Contains("js-stream-item"));
				var itemIds = items.Select(t =>
				{
					var div = t.Descendants("div");
					var tweetInfo = div.Single(x => x.GetAttributeValue("class", "").Contains("js-stream-tweet"));
					return tweetInfo.GetAttributeValue("data-tweet-id", null);
				});
				ItemIds = itemIds.ToList();
			}
		}
		/// <summary>
		/// The html converted to objects.
		/// </summary>
		[JsonIgnore]
		public IList<string> ItemIds { get; private set; }
		/// <summary>
		/// The amount of posts gotten.
		/// </summary>
		[JsonProperty("new_latent_count")]
		public int NewLatentCount { get; private set; }
		[JsonIgnore]
		private string _ItemsHtml;
		[JsonIgnore]
		private string _MinimumPosition;
	}
}