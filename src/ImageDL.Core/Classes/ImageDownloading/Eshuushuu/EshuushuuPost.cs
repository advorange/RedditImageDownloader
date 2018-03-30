using System;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Eshuushuu
{
	/// <summary>
	/// Model for a post from Eshuushuu.
	/// </summary>
	public sealed class EshuushuuPost
	{
#pragma warning disable 1591
		public readonly string PostUrl;
		public readonly int PostId;
		public readonly string SubmittedBy;
		public readonly DateTime SubmittedOn;
		public readonly string FileName;
		public readonly string OriginalFileName;
		public readonly long FileSize;
		public readonly int Width;
		public readonly int Height;
		public readonly int Favorites;
		public readonly List<Tag> Tags;
		public readonly Tag Source;
		public readonly List<Tag> Characters;
		public readonly Tag Artist;
		public readonly string ImageRating;
#pragma warning restore 1591

		/// <summary>
		/// Holds the value and name of a tag.
		/// </summary>
		public class Tag
		{
			/// <summary>
			/// The value assigned to a tag from Eshuushuu.
			/// </summary>
			public readonly int Value;
			/// <summary>
			/// The name of a tag.
			/// </summary>
			public readonly string Name;
		}
	}
}
