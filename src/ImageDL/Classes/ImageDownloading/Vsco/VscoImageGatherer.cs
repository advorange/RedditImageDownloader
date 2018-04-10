using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading.Vsco.Models;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Vsco
{
	/// <summary>
	/// Gathers images from a specified Vsco link.
	/// </summary>
	public sealed class VscoImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("vsco.co");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return await VscoImageDownloader.GetVscoImagesAsync(client, url).CAF();
		}
	}
}