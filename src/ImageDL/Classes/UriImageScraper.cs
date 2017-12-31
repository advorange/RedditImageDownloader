using HtmlAgilityPack;
using ImageDL.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ImageDL.Classes
{
	public class UriImageGatherer
	{
		private static string[] _AnimatedSites = new[]
		{
			"youtu.be", "youtube",
			"gfycat",
			"streamable",
			"v.redd.it",
		};

		public Uri OriginalUri { get; private set; }
		public ImmutableList<Uri> ImageUris { get; private set; }
		public string Error { get; private set; }

		private UriImageGatherer(Uri uri)
		{
			OriginalUri = uri;
		}

		public static async Task<UriImageGatherer> CreateGatherer(Uri uri)
		{
			var gatherer = new UriImageGatherer(uri);
			if (GetRequiresScraping(gatherer.OriginalUri.ToString()))
			{
				var urls = await gatherer.ScrapeImages().ConfigureAwait(false);
				var notNull = urls.Where(x => !String.IsNullOrWhiteSpace(x));
				gatherer.ImageUris = notNull.Select(x => GetFixedUri(x)).Where(x => x != null).ToImmutableList();
			}
			else
			{
				gatherer.ImageUris = new[] { GetFixedUri(gatherer.OriginalUri.ToString()) }.ToImmutableList();
			}
			return gatherer;
		}

		//TODO: specify for tumblr and reddit image hosting
		private static Uri GetFixedUri(string uri)
		{
			var hasExtension = !String.IsNullOrWhiteSpace(Path.GetExtension(uri));

			//Remove all optional arguments
			if (uri.Contains('?'))
			{
				uri = uri.Substring(0, uri.IndexOf('?'));
			}
			//Imgur
			if (uri.CaseInsContains("imgur.com"))
			{
				uri = uri.Replace("_d", ""); //Some thumbnail thing
				if (!hasExtension)
				{
					uri = "https://i.imgur.com/" + uri.Substring(uri.LastIndexOf('/') + 1) + ".png";
				}
			}
			//Http/Https
			if (!(uri.CaseInsStartsWith("http://") || uri.CaseInsStartsWith("https://")))
			{
				if (uri.Contains("//"))
				{
					uri = uri.Substring(uri.IndexOf("//") + 2);
				}
				uri = "https://" + uri;
			}

			return Uri.TryCreate(uri, UriKind.Absolute, out var result) ? result : null;
		}
		private static bool GetRequiresScraping(string uri) => false
			|| (uri.Contains("imgur") && (uri.Contains("/a/") || uri.Contains("/gallery/")))
			|| (uri.Contains("tumblr") && uri.Contains("/post/"))
			|| (uri.Contains("deviantart") && uri.Contains("/art/"))
			|| (uri.Contains("instagram") && uri.Contains("/p/"));

		private async Task<IEnumerable<string>> ScrapeImages()
		{
			try
			{
				var req = Utils.CreateWebRequest(OriginalUri);
				//DeviantArt 18+ filter
				req.CookieContainer.Add(new Cookie("agegate_state", "1", "/", ".deviantart.com"));

				using (var resp = await req.GetResponseAsync())
				using (var s = resp.GetResponseStream())
				{
					var doc = new HtmlDocument();
					doc.Load(s);

					var uri = OriginalUri.ToString();
					if (uri.Contains("imgur"))
					{
						return ScrapeImgurImages(doc);
					}
					else if (uri.Contains("tumblr"))
					{
						return ScrapeTumblrImages(doc);
					}
					else if (uri.Contains("deviantart"))
					{
						return ScrapeDeviantArtImages(doc);
					}
					else if (uri.Contains("instagram"))
					{
						return ScrapeInstagramImages(doc);
					}
					else
					{
						throw new ArgumentException($"The supplied uri {uri} is invalid for this method.");
					}
				}
			}
			catch (Exception e)
			{
				e.WriteException();
				Error = e.Message;
				return new string[0];
			}
		}
		private IEnumerable<string> ScrapeImgurImages(HtmlDocument doc)
		{
			//Not sure if this is a foolproof way to only get the wanted images
			var images = doc.DocumentNode.Descendants("img");
			var itemprop = images.Where(x => x.GetAttributeValue("itemprop", null) != null);
			return itemprop.Select(x => x.GetAttributeValue("src", null)).Where(x => !String.IsNullOrWhiteSpace(x));
		}
		private IEnumerable<string> ScrapeTumblrImages(HtmlDocument doc)
		{
			//18+ filter
			if (doc.DocumentNode.Descendants().Any(x => x.GetAttributeValue("id", null) == "safemode_actions_display"))
			{
				Error = "this tumblr post is locked behind safemode";
				return new string[0];
			}

			var images = doc.DocumentNode.Descendants("img");
			var media = images.Where(x => x.HasClass("media"));
			return media.Select(x => x.GetAttributeValue("src", null)).Where(x => !String.IsNullOrWhiteSpace(x));
		}
		private IEnumerable<string> ScrapeDeviantArtImages(HtmlDocument doc)
		{
			//18+ filter (shouldn't be reached since the cookies are set)
			if (doc.DocumentNode.Descendants("div").Any(x => x.HasClass("dev-content-mature")))
			{
				var img = doc.DocumentNode.Descendants("img").Where(x =>
				{
					var attrs = x.Attributes;
					return attrs["width"] != null && attrs["height"] != null && attrs["alt"] != null && attrs["src"] != null && attrs["srcset"] != null && attrs["sizes"] != null;
				})
				.Select(x => x.GetAttributeValue("srcset", null))
				.Select(x =>
				{
					var i = x.LastIndexOf("w,");
					return x.Substring(i + 2, x.LastIndexOf(' ') - i - 2);
				});

				if (img.Any())
				{

				}
				Error = "this deviantart post is locked behind mature content";
				return new string[0];
			}

			var images = doc.DocumentNode.Descendants("img");
			var deviations = images.Where(x => x.GetAttributeValue("data-embed-type", null) == "deviation");
			return deviations.Select(x => x.GetAttributeValue("src", null)).Where(x => !String.IsNullOrWhiteSpace(x));
		}
		private IEnumerable<string> ScrapeInstagramImages(HtmlDocument doc)
		{
			var meta = doc.DocumentNode.Descendants("meta");
			var images = meta.Where(x => x.GetAttributeValue("property", null) == "og:image");
			return images.Select(x => x.GetAttributeValue("content", null)).Where(x => !String.IsNullOrWhiteSpace(x));
		}
	}
}
