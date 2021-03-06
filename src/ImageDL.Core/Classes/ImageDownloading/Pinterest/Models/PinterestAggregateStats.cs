﻿using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pinterest.Models
{
	/// <summary>
	/// Stats for a Pinterest post.
	/// </summary>
	public struct PinterestAggregateStats
	{
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("done")]
		public int Done { get; private set; }

		/// <summary>
		/// How many saves this post has had.
		/// </summary>
		[JsonProperty("saves")]
		public int Saves { get; private set; }

		/// <summary>
		/// Returns the saves.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Saves.ToString();
	}
}