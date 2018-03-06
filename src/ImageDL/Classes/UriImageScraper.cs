using HtmlAgilityPack;
using ImageDL.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDL.Classes
{
	/// <summary>
	/// Gathers images from the passed in <see cref="Uri"/>. Attempts to scrape images if the link is not a direct image link.
	/// </summary>
	public class UriImageGatherer
	{
		protected static List<string> _AnimatedSites = new List<string>
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
		protected static List<Test> _Info = new List<Test>
		{
			new Test(x => true, ScrapeImgurImages, ),
		};

		/// <summary>
		/// The original <see cref="Uri"/> that was passed into the constructor.
		/// </summary>
		public Uri OriginalUri { get; }
		/// <summary>
		/// Similar to <see cref="OriginalUri"/> except removes optional arguments and fixes some weird things sites do with their urls.
		/// </summary>
		public Uri EditedUri { get; private set; }
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

		private Test _Test;

		private UriImageGatherer(Uri uri)
		{
			throw new NotImplementedException("chofl");
			OriginalUri = uri;
			IsVideo = _AnimatedSites.Any(x => uri.ToString().CaseInsContains(x));
			EditedUri = EditUri();
		}

		/// <summary>
		/// Gathers images from <paramref name="uri"/>.
		/// </summary>
		/// <param name="uri">The location to get images from.</param>
		/// <returns>A <see cref="UriImageGatherer"/> which contains any images gathered and any errors which occurred.</returns>
		public static async Task<UriImageGatherer> CreateGatherer(Uri uri)
		{
			var g = new UriImageGatherer(uri);
			if (g.RequiresScraping())
			{
				var response = await g.ScrapeImages().ConfigureAwait(false);
				g.ImageUris = response.Uris.Select(x => g.EditUri(x)).Where(x => x != null).ToImmutableList();
				g.Error = response.Error;
			}
			else
			{
				g.ImageUris = new[] { g.EditedUri }.ToImmutableList();
			}
			return g;
		}

		private Uri EditUri()
		{
			var uri = OriginalUri.ToString();
			//Remove all optional arguments
			if (uri.Contains('?'))
			{
				uri = uri.Substring(0, uri.IndexOf('?'));
			}
			//Http/Https
			if (!uri.CaseInsStartsWith("http://") && !uri.CaseInsStartsWith("https://"))
			{
				uri = uri.Contains("//") ? uri.Substring(uri.IndexOf("//") + 2) : uri;
				uri = "https://" + uri;
			}
			return Uri.TryCreate(_Test.UriEditer(uri), UriKind.Absolute, out var result) ? result : null;
		}
		private static string EditImgurUri(string uri)
		{
			var hasExtension = !String.IsNullOrWhiteSpace(Path.GetExtension(uri));
			uri = uri.Replace("_d", ""); //Some thumbnail thing
			uri = hasExtension ? uri : "https://i.imgur.com/" + uri.Substring(uri.LastIndexOf('/') + 1) + ".png";
			return uri;
		}
		private static string EditTumblrUri(string uri)
		{
			//Only want to download images so replace with this. If blog post will throw exception but gets caught when downloading
			uri = uri.Replace("/post/", "/image/");
			return uri;
		}

		private async Task<ScrapeResponse> ScrapeImages()
		{
			WebResponse resp = null;
			Stream s = null;
			try
			{
				var req = OriginalUri.CreateWebRequest();
				req.CookieContainer.Add(new Cookie("agegate_state", "1", "/", ".deviantart.com")); //DeviantArt 18+ filter

				var doc = new HtmlDocument();
				doc.Load(s = (resp = await req.GetResponseAsync().ConfigureAwait(false)).GetResponseStream());

				switch (_Site)
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
			catch (WebException e)
			{
				e.Write();
				return new ScrapeResponse(new string[0], e.Message);
			}
			finally
			{
				resp?.Dispose();
				s?.Dispose();
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
					return w < 0 || s < 0 || w > s ? null : x.Substring(w, s - w);
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

		private bool RequiresScraping()
		{
			if (IsVideo)
			{
				return false;
			}

			var u = OriginalUri.ToString();
			return (_Site == Site.Imgur && (u.Contains("/a/") || u.Contains("/gallery/")))
				|| (_Site == Site.Tumblr && !u.Contains("media.tumblr"))
				|| (_Site == Site.DeviantArt && u.Contains("/art/"))
				|| (_Site == Site.Instagram && u.Contains("/p/"));
		}
		private static bool ImgurRequiresScraping()
		{

		}
	}

	public class Test
	{
		public readonly Func<string, bool> WebsiteMatches;
		public readonly Func<string, bool> RequiresScraping;
		public readonly Func<string, string> UriEditer;
		public readonly Func<HtmlDocument, ScrapeResponse> Scraper;

		public Test(
			Func<string, bool> websiteMatches,
			Func<string, bool> requiresScraping,
			Func<string, string> uriEditer,
			Func<HtmlDocument, ScrapeResponse> scraper)
		{
			WebsiteMatches = websiteMatches;
			RequiresScraping = requiresScraping;
			UriEditer = uriEditer;
			Scraper = scraper;
		}
	}

	public struct ScrapeResponse
	{
		public readonly IEnumerable<string> Uris;
		public readonly string Error;

		public ScrapeResponse(IEnumerable<string> uris, string error = null)
		{
			Uris = uris.Where(x => !String.IsNullOrWhiteSpace(x));
			Error = error;
		}
	}

}
