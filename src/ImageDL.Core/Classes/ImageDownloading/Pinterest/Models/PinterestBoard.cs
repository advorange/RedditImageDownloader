using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pinterest.Models
{
	/// <summary>
	/// The board of aPinterest post.
	/// </summary>
	public struct PinterestBoard
	{
		/// <summary>
		/// The category the board is in.
		/// </summary>
		[JsonProperty("category")]
		public string Category { get; private set; }
		/// <summary>
		/// Whether more than one person can post in the board.
		/// </summary>
		[JsonProperty("is_collaborative")]
		public bool IsCollaborative { get; private set; }
		/// <summary>
		/// The layout style.
		/// </summary>
		[JsonProperty("layout")]
		public string Layout { get; private set; }
		/// <summary>
		/// The name of the board.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; private set; }
		/// <summary>
		/// The relative url to the board.
		/// </summary>
		[JsonProperty("url")]
		public string Url { get; private set; }
		/// <summary>
		/// The time the board was created.
		/// </summary>
		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; private set; }
		/// <summary>
		/// Last modified.
		/// </summary>
		[JsonProperty("board_order_modified_at")]
		public DateTime BoardOrderModifiedAt { get; private set; }
		/// <summary>
		/// Whether you have collaborated to the board, will always be false.
		/// </summary>
		[JsonProperty("collaborated_by_me")]
		public bool CollaboratedByMe { get; private set; }
		/// <summary>
		/// Whether you have followed the board, will always be false.
		/// </summary>
		[JsonProperty("followed_by_me")]
		public bool FollowedByMe { get; private set; }
		/// <summary>
		/// The type of object, e.g. board.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
		/// <summary>
		/// The id of the board.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <summary>
		/// The url to the icon for the board.
		/// </summary>
		[JsonProperty("image_thumbnail_url")]
		public Uri ImageThumbnailUrl { get; private set; }

		/// <summary>
		/// Returns the name and id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Name} ({Id})";
	}
}