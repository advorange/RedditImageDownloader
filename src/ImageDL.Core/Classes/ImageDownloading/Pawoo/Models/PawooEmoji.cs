using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pawoo.Models
{
	/// <summary>
	/// Emoji for Pawoo.
	/// </summary>
	public struct PawooEmoji
	{
		/// <summary>
		/// The name of the emoji.
		/// </summary>
		[JsonProperty("shortcode")]
		public string Shortcode { get; private set; }
		/// <summary>
		/// Link to the emoji.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }
		/// <summary>
		/// Link to the emoji.
		/// </summary>
		[JsonProperty("static_url")]
		public Uri StaticUrl { get; private set; }
		/// <summary>
		/// Whether this is visible in the picker.
		/// </summary>
		[JsonProperty("visible_in_picker")]
		public bool VisibleInPicker { get; private set; }

		/// <summary>
		/// Returns the url.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Url.ToString();
	}
}