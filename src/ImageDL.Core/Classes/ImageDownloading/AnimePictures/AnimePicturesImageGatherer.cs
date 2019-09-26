using System;
using System.Threading.Tasks;

using AdvorangesUtils;

using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.AnimePictures
{
	/// <summary>
	/// Gathers images from a specified AnimePictures link.
	/// </summary>
	public struct AnimePicturesImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
			=> AnimePicturesPostDownloader.GetAnimePicturesImagesAsync(client, url);

		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
			=> url.Host.CaseInsContains("anime-pictures.net");
	}
}