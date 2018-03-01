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
	/// <summary>
	/// Gathers images from the passed in <see cref="Uri"/>. Attempts to scrape images if the link is not a direct image link.
	/// </summary>
	public sealed class UriImageGatherer
	{
		private static string[] _AnimatedSites = new[]
		{
			"youtu.be", "youtube",
			"gfycat",
			"streamable",
			"v.redd.it",
			"vimeo",
			"dailymotion",
			"twitch",
			"liveleak",
		};

		/// <summary>
		/// The original <see cref="Uri"/> that was passed into the constructor.
		/// </summary>
		public readonly Uri OriginalUri;
		/// <summary>
		/// Similar to <see cref="OriginalUri"/> except removes optional arguments and fixes some weird things sites do with their urls.
		/// </summary>
		public readonly Uri EditedUri;
		/// <summary>
		/// The images to download.
		/// </summary>
		public ImmutableList<Uri> ImageUris { get; private set; }
		/// <summary>
		/// Any errors which have occurred during getting <see cref="ImageUris"/>.
		/// </summary>
		public string Error { get; private set; }
		/// <summary>
		/// Indicates whether or not the link leads to a video site.
		/// </summary>
		public bool IsVideo { get; private set; }
		private readonly Site _Site;
		private readonly bool _RequiresScraping;

		private UriImageGatherer(Uri uri)
		{
			var u = uri.ToString();
			OriginalUri = uri;
			IsVideo = _AnimatedSites.Any(x => u.CaseInsContains(x));
			//TODO: more comprehensive way of getting the site
			_Site = Enum.GetValues(typeof(Site)).Cast<Site>().SingleOrDefault(x => u.CaseInsContains(x.ToString()));
			_RequiresScraping = !IsVideo && (false
				|| (_Site == Site.Imgur && (u.Contains("/a/") || u.Contains("/gallery/")))
				|| (_Site == Site.Tumblr && !u.Contains("media.tumblr"))
				|| (_Site == Site.DeviantArt && u.Contains("/art/"))
				|| (_Site == Site.Instagram && u.Contains("/p/")));
			EditedUri = EditUri(_Site, u);
		}

		/// <summary>
		/// Gathers images from <paramref name="uri"/>.
		/// </summary>
		/// <param name="uri">The location to get images from.</param>
		/// <returns>A <see cref="UriImageGatherer"/> which contains any images gathered and any errors which occurred.</returns>
		public static async Task<UriImageGatherer> CreateGatherer(Uri uri)
		{
			var g = new UriImageGatherer(uri);
			if (g._RequiresScraping)
			{
				var response = await ScrapeImages(g._Site, g.OriginalUri).ConfigureAwait(false);
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
				var req = uri.CreateWebRequest();
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
				e.Write();
				return new ScrapeResponse(new string[0], e.Message);
			}
		}
		private static ScrapeResponse ScrapeImgurImages(HtmlDocument doc)
		{
			//Only works on albums with less than 10 images
			//Otherwise not all the images load in as images, but their ids will still be there.
#if false
			var images = doc.DocumentNode.Descendants("img");
			var itemprop = images.Where(x => x.GetAttributeValue("itemprop", null) != null);
			return new ScrapeResponse(itemprop.Select(x => x.GetAttributeValue("src", null)));
#endif
			var div = doc.DocumentNode.Descendants("div");
			var images = div.Where(x => x.GetAttributeValue("itemtype", null) == "http://schema.org/ImageObject")
				.Select(x => x.GetAttributeValue("id", null))
				.Where(x => x != null);
			return new ScrapeResponse(images.Select(x => $"https://i.imgur.com/{x}.png"));
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
				//Scuffed way to get the images even when the 18+ filter is on
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
			None = 0,
			Imgur,
			Tumblr,
			DeviantArt,
			Instagram,
		}
	}
}
