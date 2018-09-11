using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesSettingParser;
using ImageDL.Attributes;
using ImageDL.Interfaces;
using Model = ImageDL.Classes.ImageDownloading.SpecificUrl.Models.SpecificUrlPost;

namespace ImageDL.Classes.ImageDownloading.SpecificUrl
{
	/// <summary>
	/// Downloads images from a specific url.
	/// </summary>
	[DownloaderName("SpecificUrl")]
	public sealed class SpecificUrlDownloader : PostDownloader
	{
		/// <summary>
		/// The url to download images from.
		/// </summary>
		public Uri Url { get; set; }

		/// <summary>
		/// Creates an instance of <see cref="SpecificUrlDownloader"/>.
		/// </summary>
		public SpecificUrlDownloader()
		{
			SettingParser.Add(new Setting<Uri>(new[] { nameof(Url), }, x => Url = x,
				parser: s => (Uri.TryCreate(s, UriKind.Absolute, out var result), result))
			{
				Description = "The url to download images from. If this url does not have an associated image gatherer, all images will be downloaded.",
			});
			SettingParser.Remove(SettingParser.GetSetting(nameof(AmountOfPostsToGather), PrefixState.NotPrefixed));
			SettingParser.Remove(SettingParser.GetSetting(nameof(MinScore), PrefixState.NotPrefixed));
			SettingParser.Remove(SettingParser.GetSetting(nameof(MaxDaysOld), PrefixState.NotPrefixed));
		}

		/// <inheritdoc />
		protected override Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			token.ThrowIfCancellationRequested(); //10,000% necessary for this to be here. Not stupid at all.
			list.Add(new Model(Url));
			return Task.FromResult(0);
		}
	}
}