#pragma warning disable 1591, 649, 169
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Meta information about an image.
	/// </summary>
	public struct VscoImageMeta
	{
		[JsonProperty("aperture")]
		public readonly double Aperture;
		[JsonProperty("copyright")]
		public readonly string Copyright;
		[JsonProperty("flash_mode")]
		public readonly string FlashMode;
		[JsonProperty("iso")]
		public readonly int Iso;
		[JsonProperty("make")]
		public readonly string Make;
		[JsonProperty("model")]
		public readonly string Model;
		[JsonProperty("shutter_speed")]
		public readonly string ShutterSpeed;
		[JsonProperty("white_balance")]
		public readonly string WhiteBalance;
		[JsonProperty("edit_stack")]
		public readonly VscoEditStack EditStack;
	}
}
