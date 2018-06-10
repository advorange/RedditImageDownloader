using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Model = ImageDL.Interfaces.IPost;

namespace ImageDL.Classes.ImageDownloading.Lofter
{
	/// <summary>
	/// Downloads images from Lofter.
	/// </summary>
	[DownloaderName("Lofter")]
	public sealed class LofterPostDownloader : PostDownloader
	{
		/// <summary>
		/// The username to search for.
		/// </summary>
		public string Username
		{
			get => _Username;
			set => _Username = value;
		}

		private string _Username;

		/// <summary>
		/// Creates an instance of <see cref="LofterPostDownloader"/>.
		/// </summary>
		public LofterPostDownloader()
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Username), "user", }, x => Username = x)
			{
				Description = "The user to download images from."
			});
		}

		/// <inheritdoc />
		protected override Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetLofterPostAsync(IDownloaderClient client, string username, string id)
		{
			var query = new Uri($"http://{username}.lofter.com/post/{id}");
			var result = await client.GetHtmlAsync(() => client.GenerateReq(query)).CAF();
			//return result.IsSuccess ? new Model(result.Value.DocumentNode) : null;
			throw new NotImplementedException();
		}
		/// <summary>
		/// Gets the images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetLofterImagesAsync(IDownloaderClient client, Uri url)
		{
			throw new NotImplementedException();
		}
	}
}