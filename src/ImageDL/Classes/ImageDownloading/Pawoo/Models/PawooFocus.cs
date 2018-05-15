using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pawoo.Models
{
	/// <summary>
	/// The focus of an image.
	/// </summary>
	public struct PawooFocus
	{
		/// <summary>
		/// The x coordinate of the focus.
		/// </summary>
		[JsonProperty("x")]
		public double X { get; private set; }
		/// <summary>
		/// The y coordinate of the focus.
		/// </summary>
		[JsonProperty("y")]
		public double Y { get; private set; }
	}
}