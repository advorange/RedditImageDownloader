using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth
{
	/// <summary>
	/// Used mainly for cashtags.
	/// </summary>
	public struct TwitterOAuthSymbol
	{
		/// <summary>
		/// Where the symbol starts and ends in the text.
		/// </summary>
		[JsonProperty("indices")]
		public IList<int> Indices { get; private set; }
		/// <summary>
		/// Name of the symbol.
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