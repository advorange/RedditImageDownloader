using System;
using System.IO;
using System.Reflection;
using AdvorangesSettingParser.Implementation.Instance;
using AdvorangesSettingParser.Utils;
using ImageDL.Attributes;

namespace ImageDL.Classes.ImageDownloading
{
	/// <summary>
	/// Contains the arguments used for post downloading.
	/// </summary>
	[DownloaderName("Unknown")]
	public abstract class PostDownloaderBase
	{
		/// <summary>
		/// The directory to save images to.
		/// </summary>
		public DirectoryInfo Directory { get; set; }
		/// <summary>
		/// The amount of posts to look through.
		/// </summary>
		public int AmountOfPostsToGather { get; set; }
		/// <summary>
		/// The minimum width an image can have before it won't be downloaded.
		/// </summary>
		public int MinWidth { get; set; }
		/// <summary>
		/// The minimum height an image can have before it won't be downloaded.
		/// </summary>
		public int MinHeight { get; set; }
		/// <summary>
		/// The maximum allowed image similarity before an image is considered a duplicate.
		/// </summary>
		public Percentage MaxImageSimilarity { get; set; } = new Percentage(1);
		/// <summary>
		/// How many images to cache per thread. Lower = faster, but more CPU.
		/// </summary>
		public int ImagesCachedPerThread { get; set; } = 50;
		/// <summary>
		/// The minimum score an image can have before it won't be downloaded. Not every site uses this.
		/// </summary>
		public int MinScore { get; set; }
		/// <summary>
		/// The minimum aspect ratio an image can have.
		/// </summary>
		public AspectRatio MinAspectRatio { get; set; } = new AspectRatio(0, 1);
		/// <summary>
		/// The maximum aspect ratio an image can have.
		/// </summary>
		public AspectRatio MaxAspectRatio { get; set; } = new AspectRatio(1, 0);
		/// <summary>
		/// Indicates whether or not to create the directory if it does not exist.
		/// </summary>
		public bool CreateDirectory { get; set; }
		/// <summary>
		/// Indicates the user wants the downloader to start.
		/// </summary>
		public bool Start { get; set; }
		/// <summary>
		/// The oldest allowed posts. This is in UTC.
		/// </summary>
		public DateTime OldestAllowed { get; set; } = DateTime.MinValue;
		/// <summary>
		/// The newest allowed posts. This is in UTC.
		/// </summary>
		public DateTime NewestAllowed { get; set; } = DateTime.MaxValue;


		/// <inheritdoc />
		public SettingParser SettingParser { get; }
		/// <inheritdoc />
		public bool CanStart => Start && SettingParser.AreAllSet();
		/// <summary>
		/// The name of this downloader.
		/// </summary>
		public string DownloaderName => _DownloaderName ?? (_DownloaderName = GetType().GetCustomAttribute<DownloaderNameAttribute>()?.Name);

		private string _DownloaderName;

		/// <summary>
		/// Creates an instance of <see cref="PostDownloader"/>.
		/// </summary>
		public PostDownloaderBase()
		{
			SettingParser = new SettingParser
			{
				new Setting<DirectoryInfo>(() => Directory, new[] { "savepath", "path", "dir" }, parser: TryParseImageDLDirectoryInfo)
				{
					Description = "The directory to save to.",
				},
				new Setting<int>(() => AmountOfPostsToGather, new[] { "amt" })
				{
					Description = "The amount of images to download.",
				},
				new Setting<int>(() => MinWidth, new[] { "minw", "mw" })
				{
					Description = "The minimum width to save an image with.",
				},
				new Setting<int>(() => MinHeight, new[] { "minh", "mh" })
				{
					Description = "The minimum height to save an image with.",
				},
				new Setting<DateTime>(() => OldestAllowed, new[] { "oldest" }, parser: TryParseUtils.TryParseUTCDateTime)
				{
					Description = "The oldest an image can be before it won't be saved.",
				},
				new Setting<DateTime>(() => NewestAllowed, new[] { "newest" }, parser: TryParseUtils.TryParseUTCDateTime)
				{
					Description = "The newest an image can be before it won't be saved.",
				},
				new Setting<Percentage>(() => MaxImageSimilarity, new[] { "sim" }, parser: Percentage.TryParse)
				{
					Description = "The percentage similarity before an image should be deleted (1 = .1%, 1000 = 100%).",
					IsOptional = true,
				},
				new Setting<int>(() => ImagesCachedPerThread, new[] { "icpt" })
				{
					Description = "How many images to cache on each thread (lower = faster but more CPU).",
					IsOptional = true,
				},
				new Setting<int>(() => MinScore, new[] { "mins", "ms" })
				{
					Description = "The minimum score for an image to have before being ignored.",
					IsOptional = true,
				},
				new Setting<AspectRatio>(() => MinAspectRatio, new[] { "minar" }, parser: AspectRatio.TryParse)
				{
					Description = "The minimum aspect ratio for an image to have before being ignored.",
					IsOptional = true,
				},
				new Setting<AspectRatio>(() => MaxAspectRatio, new[] { "maxar" }, parser: AspectRatio.TryParse)
				{
					Description = "The maximum aspect ratio for an image to have before being ignored.",
					IsOptional = true,
				},
				new Setting<bool>(() => CreateDirectory, new[] { "create", "cd" })
				{
					Description = "Whether or not to create the directory if it does not exist.",
					IsFlag = true,
					IsOptional = true,
				},
				new Setting<bool>(() => Start, new[] { "s" })
				{
					Description = "Whether or not to start the downloader. Will not start until all other arguments are provided.",
					IsFlag = true,
				},
			};
		}

		private bool TryParseImageDLDirectoryInfo(string s, out DirectoryInfo result)
		{
			try
			{
				result = new DirectoryInfo(s);
				if (CreateDirectory)
				{
					result.Create();
				}
				//Return CreateDirectory because DirectoryInfo is cached meaning it will say it does not exist
				return CreateDirectory || result.Exists;
			}
			catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentException || ex is PathTooLongException)
			{
				result = null;
				return false;
			}
		}
	}
}