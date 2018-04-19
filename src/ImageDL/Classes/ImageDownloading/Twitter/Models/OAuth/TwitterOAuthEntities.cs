using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Twitter.Models.OAuth
{
	/// <summary>
	/// All of the entities in a post.
	/// </summary>
	public struct TwitterOAuthEntities
	{
		/// <summary>
		/// All the hashtags in the post.
		/// </summary>
		[JsonProperty("hashtags")]
		public IList<TwitterOAuthHashtag> Hashtags { get; private set; }
		/// <summary>
		/// All the urls in the post.
		/// </summary>
		[JsonProperty("urls")]
		public IList<TwitterOAuthUrl> Urls { get; private set; }
		/// <summary>
		/// All the user mentions in the post.
		/// </summary>
		[JsonProperty("user_mentions")]
		public IList<TwitterOAuthUserMention> UserMentions { get; private set; }
		/// <summary>
		/// All the media in the post.
		/// </summary>
		[JsonProperty("media")]
		public IList<TwitterOAuthMedia> Media { get; private set; }
		/// <summary>
		/// All the symbols in the post.
		/// </summary>
		[JsonProperty("symbols")]
		public IList<TwitterOAuthSymbol> Symbols { get; private set; }
		/// <summary>
		/// All the polls in the post.
		/// </summary>
		[JsonProperty("polls")]
		public IList<TwitterOAuthPoll> Polls { get; private set; }
	}
}