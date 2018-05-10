using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageDL.Attributes;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Model = ImageDL.Classes.ImageDownloading.SpecificUrl.Models.SpecificUrlPost;

namespace ImageDL.Classes.ImageDownloading.SpecificUrl
{
	/// <summary>
	/// Downloads images from a specific url.
	/// </summary>
	[DownloaderName("SpecificUrl")]
	public sealed class SpecificUrlImageDownloader : ImageDownloader
	{
		/// <summary>
		/// The url to download images from.
		/// </summary>
		public Uri Url
		{
			get => _Url;
			set => _Url = value;
		}

		private Uri _Url;

		/// <summary>
		/// Creates an instance of <see cref="SpecificUrlImageDownloader"/>.
		/// </summary>
		public SpecificUrlImageDownloader()
		{
			SettingParser.Add(new Setting<Uri>(new[] { nameof(Url), }, x => Url = x, s => (Uri.TryCreate(s, UriKind.Absolute, out var result), result))
			{
				Description = "The url to download images from. If this url does not have an associated image gatherer, all images will be downloaded.",
			});
			SettingParser.Remove(SettingParser.GetSetting(nameof(AmountOfPostsToGather)));
			SettingParser.Remove(SettingParser.GetSetting(nameof(MinScore)));
			SettingParser.Remove(SettingParser.GetSetting(nameof(MaxDaysOld)));
		}

		/// <inheritdoc />
		public override Task GatherPostsAsync(IImageDownloaderClient client, List<IPost> list)
		{
			list.Add(new Model(Url));
			return Task.FromResult(0);
		}
	}
}