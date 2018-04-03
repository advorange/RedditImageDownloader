using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading;
using ImageDL.Classes.ImageScraping;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageDL.Classes
{
	/// <summary>
	/// Abstract class for a post.
	/// </summary>
	public abstract class Post
	{
		/// <summary>
		/// The link to the post.
		/// </summary>
		public abstract string Link { get; }
		/// <summary>
		/// The id of the post.
		/// </summary>
		public abstract string Id { get; }
		/// <summary>
		/// The score of the post.
		/// </summary>
		public abstract int Score { get; }

		/// <summary>
		/// Generates the file to use for this uri.
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="uri"></param>
		/// <returns></returns>
		public virtual FileInfo GenerateFileInfo(DirectoryInfo dir, Uri uri)
		{
			var directory = dir.ToString();
			var name = $"{Id}_{Path.GetFileNameWithoutExtension(uri.LocalPath)}";
			var extension = Path.GetExtension(uri.LocalPath);

			//Make sure the extension has a period
			extension = extension.StartsWith(".") ? extension : "." + extension;
			//Remove any invalid file name path characters
			name = new string(name.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());
			//Max file name length has to be under 260 for windows, but 256 for some other things, so just go with 255.
			var nameLen = 255 - directory.Length - 1 - extension.Length; //Subtract extra 1 for / between dir and file
			return new FileInfo(Path.Combine(directory, name.Substring(0, Math.Min(name.Length, nameLen)) + extension));
		}
		/// <summary>
		/// Creates a content link of the object.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="reason"></param>
		/// <returns></returns>
		public virtual ContentLink CreateContentLink(Uri uri, string reason)
		{
			return new ContentLink(uri, Score, reason);
		}
		/// <summary>
		/// Gathers the images from the post.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		public virtual async Task<ScrapeResult> GatherImagesAsync(ImageDownloaderClient client)
		{
			var uri = new Uri(Link);
			var scraper = client.Scrapers.SingleOrDefault(x => x.IsFromWebsite(uri));
			var isAnimated = client.AnimatedContentDomains.Any(x => x.IsMatch(uri.ToString()));
			var editedUri = scraper == null ? uri : scraper.EditUri(uri);

			//If the link goes directly to an image, just use that
			if (editedUri.ToString().IsImagePath())
			{
				return new ScrapeResult(uri, isAnimated, null, new[] { editedUri }, null);
			}
			//If the link is animated, return nothing and give an error
			else if (isAnimated)
			{
				return new ScrapeResult(uri, isAnimated, null, new[] { editedUri }, $"{editedUri} is animated content (gif/video).");
			}
			//If the scraper isn't null and the uri requires scraping, scrape it
			else if (scraper != null)
			{
				return await scraper.ScrapeAsync(this, uri).CAF();
			}
			//Otherwise, just return the uri and hope for the best.
			else
			{
				return new ScrapeResult(uri, isAnimated, scraper, new[] { editedUri }, null);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public string ToString(int count)
		{
			return $"[#{count}|\u2191{(Score >= 0 ? $"|\u2191{Score}" : "")}] {Link}";
		}
		/// <inheritdoc />
		public override string ToString()
		{
			return Link;
		}
	}
}
