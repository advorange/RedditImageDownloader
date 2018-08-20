using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pixiv.Models
{
	/// <summary>
	/// Information about this user's workspace.
	/// </summary>
	public sealed class PixivWorkspace
	{
		/// <summary>
		/// Their computer specs.
		/// </summary>
		[JsonProperty("computer")]
		public string Computer { get; private set; }
		/// <summary>
		/// Their monitor.
		/// </summary>
		[JsonProperty("monitor")]
		public string Monitor { get; private set; }
		/// <summary>
		/// The software they use to draw.
		/// </summary>
		[JsonProperty("software")]
		public string Software { get; private set; }
		/// <summary>
		/// Their scanner.
		/// </summary>
		[JsonProperty("scanner")]
		public string Scanner { get; private set; }
		/// <summary>
		/// Their tablet.
		/// </summary>
		[JsonProperty("tablet")]
		public string Tablet { get; private set; }
		/// <summary>
		/// Their mouse.
		/// </summary>
		[JsonProperty("mouse")]
		public string Mouse { get; private set; }
		/// <summary>
		/// Their printer.
		/// </summary>
		[JsonProperty("printer")]
		public string Printer { get; private set; }
		/// <summary>
		/// What's on their table (TMI much?).
		/// </summary>
		[JsonProperty("on_table")]
		public string OnTable { get; private set; }
		/// <summary>
		/// The music they listen to.
		/// </summary>
		[JsonProperty("music")]
		public string Music { get; private set; }
		/// <summary>
		/// Their table.
		/// </summary>
		[JsonProperty("table")]
		public string Table { get; private set; }
		/// <summary>
		/// Their chair.
		/// </summary>
		[JsonProperty("chair")]
		public string Chair { get; private set; }
		/// <summary>
		/// Other info about them.
		/// </summary>
		[JsonProperty("other")]
		public string Other { get; private set; }
		/// <summary>
		/// Image url.
		/// </summary>
		[JsonProperty("image_url")]
		public string ImageUrl { get; private set; }
		/// <summary>
		/// Different sizes of the image url.
		/// </summary>
		[JsonProperty("image_urls")]
		public IDictionary<string, string> ImageUrls { get; private set; }
	}
}