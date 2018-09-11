using System;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.TheAnimeGallery.Models
{
	/// <summary>
	/// Holds information about a tag on an image.
	/// </summary>
	public struct TheAnimeGalleryTag
	{
		/// <summary>
		/// The name of the tag.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }
		/// <summary>
		/// The url leading to the tag.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }

		/// <summary>
		/// Creates an instance of <see cref="TheAnimeGalleryTag"/>.
		/// </summary>
		/// <param name="node"></param>
		public TheAnimeGalleryTag(HtmlNode node)
		{
			Url = new Uri($"http://www.theanimegallery.com{node.GetAttributeValue("href", null)}");
			Name = node.InnerText;
		}
		/// <summary>
		/// Returns the name.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Name;
	}
}