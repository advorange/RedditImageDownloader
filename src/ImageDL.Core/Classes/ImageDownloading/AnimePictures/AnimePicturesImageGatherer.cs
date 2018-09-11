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
		public bool IsFromWebsite(Uri url) => url.Host.CaseInsContains("anime-pictures.net");
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url) => await AnimePicturesPostDownloader.GetAnimePicturesImagesAsync(client, url).CAF();
	}
}