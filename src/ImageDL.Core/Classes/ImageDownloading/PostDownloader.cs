using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesSettingParser.Interfaces;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Core.Enums;
using ImageDL.Interfaces;
using ImageDL.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace ImageDL.Classes.ImageDownloading
{
	/// <summary>
	/// Downloads images from a site.
	/// </summary>
	[DownloaderName("Unknown")]
	public abstract class PostDownloader : PostDownloaderBase, IPostDownloader, IPostGatherer, IParsable
	{
		private static readonly string NL = Environment.NewLine;
		private static readonly string NLT = NL + "\t";

		/// <inheritdoc />
		public async Task<DownloaderResponse> DownloadAsync(IEnumerable<IPost> posts, IServiceProvider services, CancellationToken token = default)
		{
			if (!posts.Any())
			{
				return DownloaderResponse.FromNoPostsFound();
			}

			//This cannot be null, a client is necessary for downloading
			var client = services.GetRequiredService<IDownloaderClient>();
			//This can be null, image comparing is not critical
			var comparer = services.GetService<IImageComparerFactory>()?.CreateComparer(Path.Combine(Directory.FullName, ".ImageDL.db"));

			try
			{
				var downloadedCount = 0;
				var postCount = 0;
				var links = new List<ContentLink>();
				foreach (var post in posts)
				{
					token.ThrowIfCancellationRequested();
					ConsoleUtils.WriteLine(post.Format(++postCount));

					var images = await post.GetImagesAsync(client).CAF();
					//If wasn't success, log it
					if (images.IsSuccess == false)
					{
						links.AddRange(images.ImageUrls.Select(x => post.CreateContentLink(x, images)));
						ConsoleUtils.WriteLine($"\t{images.Text.Replace(NL, NLT)}", ConsoleColor.Yellow);
						continue;
					}

					var count = 0;
					foreach (var group in images.ImageUrls.GroupInto(5))
					{
						//Download every image in that group of 5 at the same time to speed up downloading
						await Task.WhenAll(group.Select(async x =>
						{
							Response result;
							try
							{
								result = await DownloadImageAsync(client, comparer, post, x).CAF();
							}
							//Catch all so they can be written and logged as a failed download
							catch (Exception e)
							{
								e.Write();
								links.Add(post.CreateContentLink(x, new Response(ImageResponse.EXCEPTION, e.Message, false)));
								return;
							}

							var text = $"\t[#{Interlocked.Increment(ref count)}] {result.Text.Replace(NL, NLT)}";
							switch (result.IsSuccess)
							{
								case true:
									ConsoleUtils.WriteLine(text);
									Interlocked.Increment(ref downloadedCount);
									break;
								case false:
									ConsoleUtils.WriteLine(text, ConsoleColor.Yellow);
									links.Add(post.CreateContentLink(x, result));
									break;
								default:
									ConsoleUtils.WriteLine(text, ConsoleColor.Cyan);
									break;
							}
						})).CAF();
					}
				}
				ConsoleUtils.WriteLine("");

				var cachedCount = 0;
				var deletedCount = 0;
				if (comparer != null)
				{
					cachedCount = await comparer.CacheSavedFilesAsync(Directory, ImagesCachedPerThread, token).CAF();
					PostUtils.WriteIfNotZero(cachedCount, () => $"{cachedCount} image(s) successfully cached from file.{NL}");
					deletedCount = comparer.HandleDuplicates(Directory, MaxImageSimilarity);
					PostUtils.WriteIfNotZero(deletedCount, () => $"{deletedCount} match(es) found and deleted.{NL}");
				}
				var linkCount = SaveStoredContentLinks(Directory, links);
				PostUtils.WriteIfNotZero(linkCount, () => $"{linkCount} link(s) added to file.{NL}");
				return DownloaderResponse.FromFinished(postCount, downloadedCount, cachedCount, deletedCount, linkCount);
			}
			finally
			{
				(comparer as IDisposable)?.Dispose();
			}
		}
		/// <inheritdoc />
		public async Task<IReadOnlyCollection<IPost>> GatherAsync(IServiceProvider services, CancellationToken token = default)
		{
			var client = services.GetRequiredService<IDownloaderClient>(); //Cannot be null, 100% necessary for gathering
			var posts = await GatherAsync(client, token).CAF();
			var sorted = posts.GroupBy(x => x.Id).Select(x => x.First()).OrderByDescending(x => x.Score).ToArray();
			ConsoleUtils.WriteLine($"Found {sorted.Length} posts.{NL}");
			return sorted;
		}
		/// <summary>
		/// Gathers and downloads posts in one.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task<DownloaderResponse> DownloadAsync(IServiceProvider services, CancellationToken token = default)
			=> await DownloadAsync(await GatherAsync(services, token).CAF(), services, token).CAF();
		/// <summary>
		/// Actual implementation of <see cref="GatherAsync(IServiceProvider, CancellationToken)"/>.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		protected abstract Task<IEnumerable<IPost>> GatherAsync(IDownloaderClient client, CancellationToken token = default);
		/// <summary>
		/// Checks min width, min height, and the min/max aspect ratios.
		/// </summary>
		/// <param name="size"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		protected bool HasValidSize(ISize size, out string error)
			=> HasValidSize(size.Width, size.Height, out error);
		/// <summary>
		/// Checks min width, min height, and the min/max aspect ratios.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		protected bool HasValidSize(int width, int height, out string error)
		{
			if (width < MinWidth || height < MinHeight)
			{
				error = $"is too small ({width}x{height}).";
				return false;
			}
			var aspectRatio = width / (double)height;
			if (aspectRatio < MinAspectRatio.Value || aspectRatio > MaxAspectRatio.Value)
			{
				error = $"does not fit in the aspect ratio restrictions ({width}x{height}).";
				return false;
			}
			error = null;
			return true;
		}
		/// <summary>
		/// Downloads an image from <paramref name="url"/> and saves it. Returns a text response.
		/// </summary>
		/// <param name="client">The client to download with.</param>
		/// <param name="comparer">The comparer to use.</param>
		/// <param name="post">The post to save from.</param>
		/// <param name="url">The location to the file to save.</param>
		/// <returns>A text response indicating what happened to the uri.</returns>
		private async Task<Response> DownloadImageAsync(IDownloaderClient client, IImageComparer comparer, IPost post, Uri url)
		{
			var file = post.GenerateFileInfo(Directory, url);
			if (File.Exists(file.FullName))
			{
				return new Response("Already Saved", $"{url} is already saved as {file}.", false);
			}

			HttpResponseMessage resp = null;
			Stream rs = null;
			MemoryStream ms = null;
			FileStream fs = null;
			try
			{
				resp = await client.SendAsync(client.GenerateReq(url)).CAF();
				switch (resp.GetImageResponseType())
				{
					case ImageResponseStatus.Fail:
						return new Response(ImageResponse.EXCEPTION, $"{url} had the following exception:{NL}{resp}", false);
					case ImageResponseStatus.Animated:
						return new Response(ImageResponse.ANIMATED, $"{url} is either a video or a gif.", false);
					case ImageResponseStatus.NotImage:
						return new Response("Not An Image", $"{url} did not lead to an image.", false);
				}

				//Need to use a memory stream and copy to it
				//Otherwise doing either the md5 hash or creating an image ends up getting to the end of the response stream
				//And with this reponse stream seeks cannot be used on it.
				await (rs = await resp.Content.ReadAsStreamAsync().CAF()).CopyToAsync(ms = new MemoryStream());

				//If image is too small, don't bother saving
				var (width, height) = post.GetSize(ms);
				if (!HasValidSize(width, height, out var sizeError))
				{
					return new Response("Does Not Meet Size Reqs", $"{url} {sizeError}", false);
				}
				//If the image comparer returns any errors when trying to store, then return that error
				if (comparer != null && !comparer.TryStore(file, ms, width, height, out var cachingError))
				{
					return new Response("Unable To Cache", $"{url} {cachingError}", false);
				}
				//Save the file
				ms.Seek(0, SeekOrigin.Begin);
				await ms.CopyToAsync(fs = file.Create()).CAF();
				return new Response(null, $"Successfully saved {url} to {file}.", true);
			}
			finally
			{
				resp?.Dispose();
				rs?.Dispose();
				ms?.Dispose();
				fs?.Dispose();
			}
		}
		/// <summary>
		/// Saves the stored content links to file.
		/// Returns the amount of links put into the file.
		/// <paramref name="diretory"></paramref>
		/// <paramref name="links"></paramref>
		/// </summary>
		private int SaveStoredContentLinks(DirectoryInfo diretory, IEnumerable<ContentLink> links)
		{
			var filteredLinks = links.GroupBy(x => x.Url).Select(x => x.First()).ToList();
			if (!filteredLinks.Any())
			{
				return 0;
			}

			//Only read once, make sure no duplicate uris will be added
			var file = new FileInfo(Path.Combine(diretory.FullName, "Links.txt"));
			if (file.Exists)
			{
				using (var reader = new StreamReader(file.OpenRead()))
				{
					var text = reader.ReadToEnd();
					for (var i = filteredLinks.Count - 1; i >= 0; --i)
					{
						if (text.Contains(filteredLinks[i].Url.ToString()))
						{
							filteredLinks.RemoveAt(i);
						}
					}
				}
			}
			//Put all of the links with the same reasons together, ordered by score
			var groups = filteredLinks.GroupBy(x => x.Reason).Select(group =>
			{
				var len = group.Max(x => x.AssociatedNumber).ToString().Length;
				var formatted = group.OrderByDescending(x => x.AssociatedNumber)
					.Select(x => $"{x.AssociatedNumber.ToString().PadLeft(len, '0')} {x.Url}");
				return $"{group.Key.ToString().FormatTitle()} - {FormattingUtils.ToSaving()}{NL}{string.Join(NL, formatted)}{NL}";
			});
			using (var writer = file.AppendText())
			{
				foreach (var line in groups)
				{
					writer.WriteLine(line);
				}
			}
			return filteredLinks.Count;
		}
	}
}