using AdvorangesUtils;
using ImageDL.Classes.ImageScraping;
using ImageDL.Classes.SettingParsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloading.Eshuushuu
{
	/// <summary>
	/// Downloads images from Eshuushuu.
	/// </summary>
	public sealed class EshuushuuImageDownloader : ImageDownloader<EshuushuuPost>
	{
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

		private string _Tags;

		/// <summary>
		/// Creates an instance of <see cref="EshuushuuImageDownloader"/>.
		/// </summary>
		public EshuushuuImageDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Tags), }, x => Tags = x)
			{
				Description = "The tags to search for. Must be numbers joined with +. Use http://e-shuushuu.net/search/ for help.",
			});
		}

		/// <inheritdoc />
		protected override void WritePostToConsole(EshuushuuPost post, int count)
		{
			Console.WriteLine($"[#{count}|\u2191{post.Favorites}] {post.PostUrl}");
		}
		/// <inheritdoc />
		protected override async Task<List<EshuushuuPost>> GatherPostsAsync()
		{
			throw new NotImplementedException();
		}
		/// <inheritdoc />
		protected override FileInfo GenerateFileInfo(EshuushuuPost post, Uri uri)
		{
			var extension = Path.GetExtension(uri.LocalPath);
			var name = $"{post.PostId}_" +
				$"{post.Artist.Name}_" +
				$"{post.Source.Name}_" +
				$"{String.Join("_", post.Characters.Select(x => x.Name))}".Replace(' ', '_');
			return GenerateFileInfo(Directory, name, extension);
		}
		/// <inheritdoc />
		protected override async Task<ScrapeResult> GatherImagesAsync(EshuushuuPost post)
		{
			return await Client.ScrapeImagesAsync(new Uri($"http://e-shuushuu.net/images/{post.FileName}")).CAF();
		}
		/// <inheritdoc />
		protected override ContentLink CreateContentLink(EshuushuuPost post, Uri uri, string reason)
		{
			return new ContentLink(uri, post.Favorites, reason);
		}
	}
}
