using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using Model = ImageDL.Classes.ImageDownloading.Booru.Gelbooru.GelbooruPost;

namespace ImageDL.Classes.ImageDownloading.Booru.Gelbooru
{
	/// <summary>
	/// Downloads images from Danbooru.
	/// </summary>
	public sealed class GelbooruImageDownloader : BooruImageDownloader<Model>
	{
		/// <summary>
		/// Creates an instance of <see cref="GelbooruImageDownloader"/>.
		/// </summary>
		/// <param name="client">The client to download images with.</param>
		public GelbooruImageDownloader(ImageDownloaderClient client) : base(client, new Uri("https://gelbooru.com"), int.MaxValue, false) { }

		/// <inheritdoc />
		protected override Uri GenerateQuery(int page)
		{
			return new Uri($"https://gelbooru.com/index.php" +
				$"?page=dapi" +
				$"&s=post" +
				$"&q=index" +
				$"&json=0" +
				$"&limit=100" +
				$"&tags={WebUtility.UrlEncode(Tags)}" +
				$"&pid={page}");
		}
		/// <inheritdoc />
		protected override List<Model> Parse(string text)
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
	}
}