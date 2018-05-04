using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Bcy.Models
{
	/// <summary>
	/// Tag for a post on Bcy.
	/// </summary>
	public struct BcyPostTag
	{
		/// <summary>
		/// The id of the tag.
		/// </summary>
		[JsonProperty("tag_id")]
		public int TagId { get; private set; }
		/// <summary>
		/// The text value of the tag.
		/// </summary>
		[JsonProperty("tag_name")]
		public string TagName { get; private set; }

		/// <summary>
		/// Returns the tag name and id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{TagName} ({TagId})";
		}
	}
}