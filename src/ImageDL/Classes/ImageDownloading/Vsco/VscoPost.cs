#pragma warning disable 1591, 649, 169
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ImageDL.Classes.ImageDownloading.Vsco
{
	/// <summary>
	/// Json model for the results of a Vsco page.
	/// </summary>
	public class VscoPage
	{
		[JsonProperty("media")]
		public readonly List<VscoPost> Posts;
		[JsonProperty("page")]
		public readonly int PageNumber;
		[JsonProperty("size")]
		public readonly int Size;
		[JsonProperty("total")]
		public readonly int Total;
	}

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
		public override string Link => _Permalink;
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

	/// <summary>
	/// Status of an image. Not sure what that means, I guess success/failure and at what time?
	/// </summary>
	public struct ImageStatus
	{
		[JsonProperty("code")]
		public readonly int Code;
		[JsonProperty("time")]
		private readonly long _Time;

		[JsonIgnore]
		public DateTime Time => (new DateTime(1970, 1, 1).AddSeconds(_Time / 1000)).ToUniversalTime();
	}

	/// <summary>
	/// Filter color to apply to an image.
	/// </summary>
	public struct Preset
	{
		[JsonProperty("color")]
		public readonly string Color;
		[JsonProperty("key")]
		public readonly string Key;
		[JsonProperty("short_name")]
		public readonly string ShortName;
	}

	/// <summary>
	/// Meta information about an image.
	/// </summary>
	public struct ImageMeta
	{
		[JsonProperty("aperture")]
		public readonly double Aperture;
		[JsonProperty("copyright")]
		public readonly string Copyright;
		[JsonProperty("flash_mode")]
		public readonly string FlashMode;
		[JsonProperty("iso")]
		public readonly int Iso;
		[JsonProperty("make")]
		public readonly string Make;
		[JsonProperty("model")]
		public readonly string Model;
		[JsonProperty("shutter_speed")]
		public readonly string ShutterSpeed;
		[JsonProperty("white_balance")]
		public readonly string WhiteBalance;
		[JsonProperty("edit_stack")]
		public readonly EditStack EditStack;
	}

	/// <summary>
	/// The key of the preset.
	/// </summary>
	public struct EditStack
	{
		[JsonProperty("key")]
		public readonly string Key;
	}
}
