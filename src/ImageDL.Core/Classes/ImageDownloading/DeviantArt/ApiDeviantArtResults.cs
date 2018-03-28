using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.DeviantArt
{
	/// <summary>
	/// Json model for searching for DeviantArt posts through the API.
	/// </summary>
	public class ApiDeviantArtResults
	{
#pragma warning disable 1591
		[JsonProperty("has_more")]
		public readonly bool HasMore;
		[JsonProperty("next_offset")]
		public readonly int? NextOffset;
		[JsonProperty("estimated_total")]
		public readonly int EstimatedTotal;
		[JsonProperty("results")]
		public readonly List<ApiDeviantArtPost> Results;

		/// <summary>
		/// Json model for a DeviantArt post gotten via the API.
		/// </summary>
		public class ApiDeviantArtPost
		{
			[JsonProperty("is_favourited")]
			public readonly bool IsFavorited;
			[JsonProperty("is_deleted")]
			public readonly bool IsDeleted;
			[JsonProperty("is_mature")]
			public readonly bool IsMature;
			[JsonProperty("is_downloadable")]
			public readonly bool IsDownloadable;
			[JsonProperty("allows_comments")]
			public readonly bool AllowsComments;
			[JsonProperty("published_time")]
			public readonly ulong PublishedTime;
			[JsonProperty("download_filesize")]
			public readonly long DownloadFileSize;
			[JsonProperty("deviationid")]
			public readonly string DeviationUUID;
			[JsonProperty("printid")]
			public readonly string PrintUUID;
			[JsonProperty("url")]
			public readonly string Url;
			[JsonProperty("title")]
			public readonly string Title;
			[JsonProperty("category")]
			public readonly string Category;
			[JsonProperty("category_path")]
			public readonly string CategoryPath;
			[JsonProperty("author")]
			public readonly ApiAuthorInfo Author;
			[JsonProperty("stats")]
			public readonly ApiStats Stats;
			[JsonProperty("preview")]
			public readonly ApiThumbnail Preview;
			[JsonProperty("content")]
			public readonly ApiThumbnail Content;
			[JsonProperty("thumbs")]
			public readonly List<ApiThumbnail> Thumbnails;

			/// <summary>
			/// Holds information about the author of the image.
			/// </summary>
			public class ApiAuthorInfo
			{
				[JsonProperty("userid")]
				public readonly string UUID;
				[JsonProperty("username")]
				public readonly string Username;
				[JsonProperty("usericon")]
				public readonly string UserIcon;
				[JsonProperty("type")]
				public readonly string Type;
			}

			/// <summary>
			/// Holds information about comments/favorites
			/// </summary>
			public class ApiStats
			{
				[JsonProperty("comments")]
				public readonly int Comments;
				[JsonProperty("favourites")]
				public readonly int Favorites;
			}

			/// <summary>
			/// Holds information about a resized version of the image.
			/// </summary>
			public class ApiThumbnail
			{
				[JsonProperty("transparency")]
				public readonly bool IsTransparent;
				[JsonProperty("width")]
				public readonly int Width;
				[JsonProperty("height")]
				public readonly int Height;
				[JsonProperty("filesize")]
				public readonly long FileSize;
				[JsonProperty("src")]
				public readonly string Source;
			}
		}
#pragma warning restore 1591
	}
}