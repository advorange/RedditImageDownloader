using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models.Graphql
{
	/// <summary>
	/// Not sure why there needs to be a stupid extra root object.
	/// </summary>
	public sealed class InstagramGraphqlResult
	{
		/// <summary>
		/// Holds the information.
		/// </summary>
		[JsonProperty("graphql")]
		public readonly InstagramGraphql Graphql;
	}
}