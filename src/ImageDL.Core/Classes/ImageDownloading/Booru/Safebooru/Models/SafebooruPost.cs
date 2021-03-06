﻿using System;

using ImageDL.Classes.ImageDownloading.Booru.Gelbooru.Models;

namespace ImageDL.Classes.ImageDownloading.Booru.Safebooru.Models
{
	/// <summary>
	/// Json model (gotten through the Xml endpoint though) for a Safebooru post.
	/// </summary>
	public sealed class SafebooruPost : GelbooruPost
	{
		/// <inheritdoc />
		public override Uri BaseUrl => new Uri("https://safebooru.org");
	}
}