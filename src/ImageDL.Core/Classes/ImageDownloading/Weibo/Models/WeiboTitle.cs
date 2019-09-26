using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Weibo.Models
{
	/// <summary>
	/// Contains information about the title of a Weibo post.
	/// </summary>
	public struct WeiboTitle
	{
		/// <summary>
		/// The color of the title.
		/// </summary>
		[JsonProperty("base_color")]
		public int BaseColor { get; private set; }

		/// <summary>
		/// The text of the title.
		/// </summary>
		[JsonProperty("text")]
		public string Text { get; private set; }

		/// <summary>
		/// Returns the text.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Text;
	}
}