using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Meta information about an image.
	/// </summary>
	public struct VscoImageMeta
	{
		/// <summary>
		/// Iso for the camera.
		/// </summary>
		[JsonProperty("iso")]
		public int Iso;

		/// <summary>
		/// The camera's aperture radius.
		/// </summary>
		[JsonProperty("aperture")]
		public double Aperture { get; private set; }

		/// <summary>
		/// Who owns the image.
		/// </summary>
		[JsonProperty("copyright")]
		public string Copyright { get; private set; }

		/// <summary>
		/// The preset used for the image.
		/// </summary>
		[JsonProperty("edit_stack")]
		public VscoEditStack EditStack { get; private set; }

		/// <summary>
		/// Flash setting on the camera.
		/// </summary>
		[JsonProperty("flash_mode")]
		public string FlashMode { get; private set; }

		/// <summary>
		/// Camera manufacturer.
		/// </summary>
		[JsonProperty("make")]
		public string Make { get; private set; }

		/// <summary>
		/// Camera type.
		/// </summary>
		[JsonProperty("model")]
		public string Model { get; private set; }

		/// <summary>
		/// Shutter speed for the camera.
		/// </summary>
		[JsonProperty("shutter_speed")]
		public string ShutterSpeed { get; private set; }

		/// <summary>
		/// White balance for the camera.
		/// </summary>
		[JsonProperty("white_balance")]
		public string WhiteBalance { get; private set; }
	}
}