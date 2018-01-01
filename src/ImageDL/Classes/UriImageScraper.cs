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
		public Uri EditedUri { get; private set; }
		public ImmutableList<Uri> ImageUris { get; private set; }
		public string Error { get; private set; }
		private Site _Site;
		private bool _RequiresScraping;

		private UriImageGatherer(Uri uri)
		{
			var u = uri.ToString();
			OriginalUri = uri;
			_Site = Enum.GetNames(typeof(Site))
				.Where(x => u.CaseInsContains(x))
				.Select(x => (Site)Enum.Parse(typeof(Site), x))
				.SingleOrDefault();
			EditedUri = EditUri(_Site, u);
			_RequiresScraping = false
				|| (_Site == Site.Imgur && (u.Contains("/a/") || u.Contains("/gallery/")))
				|| (_Site == Site.Tumblr && !u.Contains("media.tumblr"))
				|| (_Site == Site.DeviantArt && u.Contains("/art/"))
				|| (_Site == Site.Instagram && u.Contains("/p/"));
		}

		public static async Task<UriImageGatherer> CreateGatherer(Uri uri)
		{
			var g = new UriImageGatherer(uri);
			if (g._RequiresScraping)
			{
				var response = await ScrapeImages(g._Site, g.EditedUri).ConfigureAwait(false);
				g.ImageUris = response.Uris.Select(x => EditUri(g._Site, x)).Where(x => x != null).ToImmutableList();
				g.Error = response.Error;
			}
			else
			{
				g.ImageUris = new[] { g.EditedUri }.ToImmutableList();
			}
			return g;
		}

		//TODO: specify reddit image hosting
		private static Uri EditUri(Site s, string uri)
		{
			var hasExtension = !String.IsNullOrWhiteSpace(Path.GetExtension(uri));

			//Remove all optional arguments
			if (uri.Contains('?'))
			{
				uri = uri.Substring(0, uri.IndexOf('?'));
			}
			//Imgur
			if (s == Site.Imgur)
			{
				uri = uri.Replace("_d", ""); //Some thumbnail thing
				uri = hasExtension ? uri : "https://i.imgur.com/" + uri.Substring(uri.LastIndexOf('/') + 1) + ".png";
			}
			//Tumblr
			if (s == Site.Tumblr)
			{
				//Only want to download images so replace with this. If blog post will throw exception but whatever
				uri = uri.Replace("/post/", "/image/");
			}
			//Http/Https
			if (!uri.CaseInsStartsWith("http://") && !uri.CaseInsStartsWith("https://"))
			{
				uri = uri.Contains("//") ? uri.Substring(uri.IndexOf("//") + 2) : uri;
				uri = "https://" + uri;
			}

			return Uri.TryCreate(uri, UriKind.Absolute, out var result) ? result : null;
		}

		private static async Task<ScrapeResponse> ScrapeImages(Site site, Uri uri)
		{
			try
			{
				var req = Utils.CreateWebRequest(uri);
				//DeviantArt 18+ filter
				req.CookieContainer.Add(new Cookie("agegate_state", "1", "/", ".deviantart.com"));

				using (var resp = await req.GetResponseAsync())
				using (var s = resp.GetResponseStream())
				{
					var doc = new HtmlDocument();
					doc.Load(s);

					switch (site)
					{
						case Site.Imgur:
						{
							return ScrapeImgurImages(doc);
						}
						case Site.Tumblr:
						{
							return ScrapeTumblrImages(doc);
						}
						case Site.DeviantArt:
						{
							return ScrapeDeviantArtImages(doc);
						}
						case Site.Instagram:
						{
							return ScrapeInstagramImages(doc);
						}
						default:
						{
							throw new ArgumentException($"The supplied uri {uri} is invalid for this method.");
						}
					}
				}
			}
			catch (WebException e)
			{
				e.WriteException();
				return new ScrapeResponse(new string[0], e.Message);
			}
		}
		private static ScrapeResponse ScrapeImgurImages(HtmlDocument doc)
		{
			//Not sure if this is a foolproof way to only get the wanted images
			var images = doc.DocumentNode.Descendants("img");
			var itemprop = images.Where(x => x.GetAttributeValue("itemprop", null) != null);
			return new ScrapeResponse(itemprop.Select(x => x.GetAttributeValue("src", null)));
		}
		private static ScrapeResponse ScrapeTumblrImages(HtmlDocument doc)
		{
			//18+ filter
			if (doc.DocumentNode.Descendants().Any(x => x.GetAttributeValue("id", null) == "safemode_actions_display"))
			{
				return new ScrapeResponse(new string[0], "this tumblr post is locked behind safemode");
			}

			var meta = doc.DocumentNode.Descendants("meta");
			var images = meta.Where(x => x.GetAttributeValue("property", null) == "og:image");
			return new ScrapeResponse(images.Select(x => x.GetAttributeValue("content", null)));
		}
		private static ScrapeResponse ScrapeDeviantArtImages(HtmlDocument doc)
		{
			//18+ filter (shouldn't be reached since the cookies are set)
			if (doc.DocumentNode.Descendants("div").Any(x => x.HasClass("dev-content-mature")))
			{
				var img = doc.DocumentNode.Descendants("img")
				.Where(x =>
				{
					var attrs = x.Attributes;
					return attrs["width"] != null && attrs["height"] != null && attrs["alt"] != null && attrs["src"] != null && attrs["srcset"] != null && attrs["sizes"] != null;
				})
				.Select(x => x.GetAttributeValue("srcset", null))
				.Select(x =>
				{
					var w = x.LastIndexOf("w,") + 2; //W for w,
					var s = x.LastIndexOf(' '); //S for space
					return w > -1 && s > -1 ? null : x.Substring(w, s - w);
				});

				return img.Any()
					? new ScrapeResponse(img)
					: new ScrapeResponse(new string[0], "this deviantart post is locked behind mature content");
			}

			var images = doc.DocumentNode.Descendants("img");
			var deviations = images.Where(x => x.GetAttributeValue("data-embed-type", null) == "deviation");
			return new ScrapeResponse(deviations.Select(x => x.GetAttributeValue("src", null)));
		}
		private static ScrapeResponse ScrapeInstagramImages(HtmlDocument doc)
		{
			var meta = doc.DocumentNode.Descendants("meta");
			var images = meta.Where(x => x.GetAttributeValue("property", null) == "og:image");
			return new ScrapeResponse(images.Select(x => x.GetAttributeValue("content", null)));
		}

		private struct ScrapeResponse
		{
			public readonly IEnumerable<string> Uris;
			public readonly string Error;

			public ScrapeResponse(IEnumerable<string> uris, string error = null)
			{
				Uris = uris.Where(x => !String.IsNullOrWhiteSpace(x));
				Error = error;
			}
		}
		private enum Site
		{
			Imgur,
			Tumblr,
			DeviantArt,
			Instagram,
		}
	}
}
