using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using AdvorangesUtils;
using ImageDL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Classes.ImageDownloading.Moebooru.Gelbooru.Models.GelbooruPost;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Gelbooru
{
	/// <summary>
	/// Downloads images from Danbooru.
	/// </summary>
	public sealed class GelbooruImageDownloader : MoebooruImageDownloader<Model>
	{
		/// <summary>
		/// Creates an instance of <see cref="GelbooruImageDownloader"/>.
		/// </summary>
		public GelbooruImageDownloader() : base("Gelbooru", int.MaxValue, false) { }

		/// <inheritdoc />
		protected override Uri GenerateQuery(string tags, int page)
		{
			return GenerateGelbooruQuery(tags, page);
		}
		/// <inheritdoc />
		protected override List<Model> Parse(string text)
		{
			return ParseGelbooruPosts(text);
		}

		/// <summary>
		/// Generates a search uri.
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		public static Uri GenerateGelbooruQuery(string tags, int page)
		{
			return new Uri($"https://gelbooru.com/index.php" +
				$"?page=dapi" +
				$"&s=post" +
				$"&q=index" +
				$"&json=0" +
				$"&limit=100" +
				$"&tags={WebUtility.UrlEncode(tags)}" +
				$"&pid={page}");
		}
		/// <summary>
		/// Parses Gelbooru posts from the supplied text.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static List<Model> ParseGelbooruPosts(string text)
		{
			//Parses through Xml instead of Json since the Json doesn't include some information idk why
			var doc = new XmlDocument();
			doc.LoadXml(text);

			var posts = JObject.Parse(JsonConvert.SerializeXmlNode(doc))["posts"]["post"];
			foreach (var post in posts)
			{
				foreach (var prop in post.OfType<JProperty>().ToList())
				{
					//Properties get prefixed with @ from SerializeXmlNode
					if (!prop.Name.StartsWith("@"))
					{
						continue;
					}
					else if (prop.Name == "@created_at")
					{
						//These get returned as this format: "Thu Mar 29 22:00:33 -0500 2018"
						//100% useless, won't parse. Correct format is: "Thu Mar 29 2018 22:00:33 -0500"
						var parts = prop.Value.ToString().Split(' ').ToList();
						parts.Insert(3, parts.Last());
						prop.Value = String.Join(" ", parts.Take(parts.Count - 1));
					}

					prop.Remove();
					post[prop.Name.Substring(1)] = prop.Value;
				}
			}
			return posts.ToObject<List<Model>>();
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetGelbooruPostAsync(IImageDownloaderClient client, string id)
		{
			var query = GenerateGelbooruQuery($"id:{id}", 0);
			var result = await client.GetText(client.GetReq(query)).CAF();
			return result.IsSuccess ? ParseGelbooruPosts(result.Value)[0] : null;
		}
	}
}