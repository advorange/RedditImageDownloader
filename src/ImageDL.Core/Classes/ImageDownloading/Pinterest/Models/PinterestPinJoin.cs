using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pinterest.Models
{
	/// <summary>
	/// Pinterest post information about localization and tags.
	/// </summary>
	public struct PinterestPinJoin
	{
		/// <summary>
		/// The ids of the pin in various localizations.
		/// </summary>
		[JsonProperty("country_canonical_pins")]
		public IDictionary<string, string> CountryCanonicalPins { get; private set; }
		/// <summary>
		/// Search engine optimized description.
		/// </summary>
		[JsonProperty("seo_description")]
		public string SeoDescription { get; private set; }
		/// <summary>
		/// Tags on the post.
		/// </summary>
		[JsonProperty("visual_annotation")]
		public IList<string> VisualAnnotation { get; private set; }
		/// <summary>
		/// Descriptions on the post.
		/// </summary>
		[JsonProperty("visual_descriptions")]
		public IList<string> VisualDescriptions { get; private set; }
		/// <summary>
		/// The pin id being used in the current localization.
		/// </summary>
		[JsonProperty("canonical_pin")]
		public PinterestCanonicalPin CanonicalPin { get; private set; }

		/// <summary>
		/// Returns <see cref="CanonicalPin"/> as a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return CanonicalPin.ToString();
		}
	}
}