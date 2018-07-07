using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Attributes;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Newtonsoft.Json.Linq;
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
		protected override async Task GatherAsync(IDownloaderClient client, List<IPost> list, CancellationToken token)
		{
			var userId = await GetUserIdAsync(client, Username).CAF();
			var parsed = new List<Model>();
			for (long ts = 0; list.Count < AmountOfPostsToGather && (ts == 0 || parsed.Count >= 50); ts = parsed.Last().CreatedAt.Millisecond)
			{
				//Info: https://www.litreily.top/2018/03/17/lofter/
				ts = ts == 0 ? ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds() : ts;

				var query = new Uri($"http://{Username}.lofter.com/dwr/call/plaincall/ArchiveBean.getArchivePostByTime.dwr");
				var result = await client.GetTextAsync(() =>
				{
					var req = client.GenerateReq(query, HttpMethod.Post);
					req.Content = new FormUrlEncodedContent(new Dictionary<string, string>
					{
						{ "callCount", "1" },
						{ "scriptSessionId", "${scriptSessionId}187" },
						{ "httpSessionId", "" },
						{ "c0-scriptName", "ArchiveBean" },
						{ "c0-methodName", "getArchivePostByTime" },
						{ "c0-id", "0" },
						{ "c0-param0", $"number:{userId}" }, //User id
						{ "c0-param1", $"number:{ts}" }, //Timestamp
						{ "c0-param2", $"number:50" }, //Posts per req
						{ "c0-param3", $"boolean:false" },
						{ "batchId", "1" },
					});
					return req;
				}).CAF();
			}
		}
		/// <summary>
		/// Gets the id of the Lofter user.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="username"></param>
		/// <returns></returns>
		private static async Task<string> GetUserIdAsync(IDownloaderClient client, string username)
		{
			var query = new Uri($"http://{username}.lofter.com");
			var result = await client.GetTextAsync(() => client.GenerateReq(query)).CAF();

			var search = "blogId=";
			var cut = result.Value.Substring(result.Value.IndexOf(search) + search.Length);
			return cut.Substring(0, cut.IndexOf('"'));
		}
		/// <summary>
		/// Converts 
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		private static JObject ConvertJsToJson(string html)
		{
			//The request gives back Javascript object creation instead of JSON (no clue why, so we need to do this)
			var parts = html.SplitLikeCommandLine(new[] { ';' }).Select(x => x.Trim());
			var jObj = new JObject();
			for (int i = 0; i < 50; ++i)
			{
				var info = parts.Where(x => x.StartsWith($"s{i}.") || x.StartsWith($"s{i + 50}."));
			}
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