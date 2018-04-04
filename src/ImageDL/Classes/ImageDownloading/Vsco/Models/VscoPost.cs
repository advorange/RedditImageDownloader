#pragma warning disable 1591, 649, 169
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Json model for a post from Vsco.
	/// </summary>
	public sealed class VscoPost : Post
	{
		[JsonProperty("grid_name")]
		public readonly string GridName;
		[JsonProperty("adaptive_base")]
		public readonly string AdaptiveBase;
		[JsonProperty("site_id")]
		public readonly int SiteId;
		[JsonProperty("description")]
		public readonly string Description;
		[JsonProperty("description_anchored")]
		public readonly string DescriptionAnchored;
		[JsonProperty("copyright_classes")]
		public readonly List<string> CopyrightClasses;
		[JsonProperty("location_coords")]
		public readonly List<double> LocationCoords;
		[JsonProperty("has_location")]
		public readonly bool HasLocation;
		[JsonProperty("feature_link")]
		public readonly string FeatureLink;
		[JsonProperty("is_featured")]
		public readonly bool IsFeatured;
		[JsonProperty("is_video")]
		public readonly bool IsVideo;
		[JsonProperty("perma_domain")]
		public readonly string PermaDomain;
		[JsonProperty("perma_subdomain")]
		public readonly string PermaSubdomain;
		[JsonProperty("share_link")]
		public readonly string ShareLink;
		[JsonProperty("responsive_url")]
		public readonly string ResponsiveUrl;
		[JsonProperty("show_location")]
		public readonly int ShowLocation;
		[JsonProperty("image_status")]
		public readonly ImageStatus ImageStatus;
		[JsonProperty("image_meta")]
		public readonly ImageMeta ImageMeta;
		[JsonProperty("preset")]
		public readonly Preset Preset;
		[JsonProperty("height")]
		public readonly int Height;
		[JsonProperty("width")]
		public readonly int Width;

		[JsonProperty("capture_date")]
		private readonly long _CaptureDate;
		[JsonProperty("capture_date_ms")]
		private readonly long _CaptureDateMs;
		[JsonProperty("upload_date")]
		private readonly long _UploadDate;
		[JsonProperty("last_updated")]
		private readonly long _LastUpdated;
		[JsonProperty("_id")]
		private readonly string _Id;
		[JsonProperty("permalink")]
		private readonly string _Permalink;

		/// <inheritdoc />
		[JsonIgnore]
		public override string PostUrl => _Permalink;
		/// <inheritdoc />
		[JsonIgnore]
		public override string ContentUrl => $"https://{ResponsiveUrl}";
		/// <inheritdoc />
		[JsonIgnore]
		public override string Id => _Id;
		/// <inheritdoc />
		[JsonIgnore]
		public override int Score => -1;
		[JsonIgnore]
		public DateTime CapturedAt => (new DateTime(1970, 1, 1).AddSeconds(_CaptureDate / 1000)).ToUniversalTime();
		[JsonIgnore]
		public DateTime UploadedAt => (new DateTime(1970, 1, 1).AddSeconds(_UploadDate / 1000)).ToUniversalTime();
		[JsonIgnore]
		public DateTime LastUpdatedAt => (new DateTime(1970, 1, 1).AddSeconds(_LastUpdated / 1000)).ToUniversalTime();

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Id} ({Width}x{Height})";
		}
	}
}
