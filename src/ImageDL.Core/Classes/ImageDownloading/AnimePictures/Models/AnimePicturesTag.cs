using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.AnimePictures.Models
{
	/// <summary>
	/// A tag applied o an image.
	/// </summary>
	public struct AnimePicturesTag
	{
		/// <summary>
		/// The name of the tag.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }
		/// <summary>
		/// The type of tag.
		/// </summary>
		[JsonProperty("type")]
		public int Type { get; private set; }
		/// <summary>
		/// The tag's value.
		/// </summary>
		[JsonProperty("num")]
		public int Num { get; private set; }

		/// <summary>
		/// Returns the name and value.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Name} ({Num})";
	}
}