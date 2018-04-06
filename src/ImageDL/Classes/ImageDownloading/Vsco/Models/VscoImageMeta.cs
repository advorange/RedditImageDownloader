using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Meta information about an image.
	/// </summary>
	public struct VscoImageMeta
	{
		/// <summary>
		/// The camera's aperture radius.
		/// </summary>
		[JsonProperty("aperture")]
		public readonly double Aperture;
		/// <summary>
		/// Who owns the image.
		/// </summary>
		[JsonProperty("copyright")]
		public readonly string Copyright;
		/// <summary>
		/// Flash setting on the camera.
		/// </summary>
		[JsonProperty("flash_mode")]
		public readonly string FlashMode;
		/// <summary>
		/// Iso for the camera.
		/// </summary>
		[JsonProperty("iso")]
		public readonly int Iso;
		/// <summary>
		/// Camera manufacturer.
		/// </summary>
		[JsonProperty("make")]
		public readonly string Make;
		/// <summary>
		/// Camera type.
		/// </summary>
		[JsonProperty("model")]
		public readonly string Model;
		/// <summary>
		/// Shutter speed for the camera.
		/// </summary>
		[JsonProperty("shutter_speed")]
		public readonly string ShutterSpeed;
		/// <summary>
		/// White balance for the camera.
		/// </summary>
		[JsonProperty("white_balance")]
		public readonly string WhiteBalance;
		/// <summary>
		/// The preset used for the image.
		/// </summary>
		[JsonProperty("edit_stack")]
		public readonly VscoEditStack EditStack;
	}
}