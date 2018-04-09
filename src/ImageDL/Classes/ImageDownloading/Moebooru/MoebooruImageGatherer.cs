﻿using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading.Moebooru.Models;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Moebooru
{
	/// <summary>
	/// Gathers images from a specified Moebooru based website link.
	/// </summary>
	public abstract class MoebooruImageGatherer : IImageGatherer
	{
		private readonly string _Name;
		private readonly string _Search;
		private readonly Func<IImageDownloaderClient, string, Task<MoebooruPost>> _Func;

		/// <summary>
		/// Creates an instance of <see cref="MoebooruImageGatherer"/>.
		/// </summary>
		/// <param name="name">The name of the website..</param>
		/// <param name="search">The term to search for directly before an id.</param>
		/// <param name="func">The function to get posts with.</param>
		public MoebooruImageGatherer(string name, string search, Func<IImageDownloaderClient, string, Task<MoebooruPost>> func)
		{
			_Name = name;
			_Search = search;
			_Func = func;
		}

		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains(_Name);
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			var u = ImageDownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			if (u.CaseInsIndexOf(_Search, out var index))
			{
				var id = u.Substring(index + _Search.Length).Split('/')[0];
				if (await _Func(client, id).CAF() is MoebooruPost post)
				{
					return await post.GetImagesAsync(client).CAF();
				}
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}