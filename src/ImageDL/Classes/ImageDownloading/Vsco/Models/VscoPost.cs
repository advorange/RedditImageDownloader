using System;
using System.Collections.Generic;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Json model for a post from Vsco.
	/// </summary>
	public sealed class VscoPost : IPost
	{
		#region Json
		/// <summary>
		/// The name of the gallery.
		/// </summary>
		[JsonProperty("grid_name")]
		public readonly string GridName;
		/// <summary>
		/// The id of the image prefixed by /i/.
		/// </summary>
		[JsonProperty("adaptive_base")]
		public readonly string AdaptiveBase;
		/// <summary>
		/// The user's id.
		/// </summary>
		[JsonProperty("site_id")]
		public readonly int SiteId;
		/// <summary>
		/// The description of the image.
		/// </summary>
		[JsonProperty("description")]
		public readonly string Description;
		/// <summary>
		/// Same as <see cref="Description"/>.
		/// </summary>
		[JsonProperty("description_anchored")]
		public readonly string DescriptionAnchored;
		/// <summary>
		/// The kinds of copyright on this image. Usually is 'restricted'.
		/// </summary>
		[JsonProperty("copyright_classes")]
		public readonly List<string> CopyrightClasses;
		/// <summary>
		/// Where the photo was taken.
		/// </summary>
		[JsonProperty("location_coords")]
		public readonly List<double> LocationCoords;
		/// <summary>
		/// If the image has any <see cref="LocationCoords"/>.
		/// </summary>
		[JsonProperty("has_location")]
		public readonly bool HasLocation;
		/// <summary>
		/// Not sure what this is.
		/// </summary>
		[JsonProperty("feature_link")]
		public readonly string FeatureLink;
		/// <summary>
		/// Not sure what this is.
		/// </summary>
		[JsonProperty("is_featured")]
		public readonly bool IsFeatured;
		/// <summary>
		/// Whether or not the image is a video.
		/// </summary>
		[JsonProperty("is_video")]
		public readonly bool IsVideo;
		/// <summary>
		/// Vsco.co/user's name.
		/// </summary>
		[JsonProperty("perma_domain")]
		public readonly string PermaDomain;
		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("perma_subdomain")]
		public readonly string PermaSubdomain;
		/// <summary>
		/// The link to sharing this post.
		/// </summary>
		[JsonProperty("share_link")]
		public readonly string ShareLink;
		/// <summary>
		/// The link to the image without any http or https.
		/// </summary>
		[JsonProperty("responsive_url")]
		public readonly string ResponsiveUrl;
		/// <summary>
		/// Whether or not this has a location.
		/// </summary>
		[JsonProperty("show_location")]
		public readonly int ShowLocation;
		/// <summary>
		/// The status of the image. Usually holds the same time as <see cref="CreatedAt"/> and is ok.
		/// </summary>
		[JsonProperty("image_status")]
		public readonly VscoImageStatus ImageStatus;
		/// <summary>
		/// Various metadata about the image. Such as the camera used to take the photo.
		/// </summary>
		[JsonProperty("image_meta")]
		public readonly VscoImageMeta ImageMeta;
		/// <summary>
		/// The filter applied to the image.
		/// </summary>
		[JsonProperty("preset")]
		public readonly VscoPreset Preset;
		/// <summary>
		/// The height of the image.
		/// </summary>
		[JsonProperty("height")]
		public readonly int Height;
		/// <summary>
		/// The width of the image
		/// </summary>
		[JsonProperty("width")]
		public readonly int Width;
		/// <summary>
		/// The hashtags in the description.
		/// </summary>
		[JsonProperty("tags")]
		public readonly List<VscoTag> Tags;
		/// <summary>
		/// The unix timestamp in milliseconds of when this picture was taken.
		/// </summary>
		[JsonProperty("capture_date")]
		public readonly long CaptureDateTimestamp;
		/// <summary>
		/// The same as <see cref="CaptureDateTimestamp"/> but suffixed with MS for some reason.
		/// </summary>
		[JsonProperty("capture_date_ms")]
		public readonly long CaptureDateTimestampMS;
		/// <summary>
		/// The unix timestamp in milliseconds of when this picture was uploaded.
		/// </summary>
		[JsonProperty("upload_date")]
		public readonly long UploadDateTimestamp;
		/// <summary>
		/// The unix timestamp in milliseconds of when this picture was last updated.
		/// </summary>
		[JsonProperty("last_updated")]
		public readonly long LastUpdatedTimestamp;
		/// <summary>
		/// The id of the post.
		/// </summary>
		[JsonProperty("_id")]
		private readonly string _Id = null;
		/// <summary>
		/// The link to the post.
		/// </summary>
		[JsonProperty("permalink")]
		private readonly string _Permalink = null;
		#endregion

		/// <inheritdoc />
		public string Id => _Id;
		/// <inheritdoc />
		public Uri PostUrl => new Uri(_Permalink);
		/// <inheritdoc />
		public IEnumerable<Uri> ContentUrls => new[] { new Uri($"https://{ResponsiveUrl}") };
		/// <inheritdoc />
		public int Score => -1;
		/// <inheritdoc />
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(UploadDateTimestamp / 1000)).ToUniversalTime();
		/// <summary>
		/// The time the photo was taken at.
		/// </summary>
		public DateTime CapturedAt => (new DateTime(1970, 1, 1).AddSeconds(CaptureDateTimestamp / 1000)).ToUniversalTime();
		/// <summary>
		/// The time the photo was last updated at.
		/// </summary>
		public DateTime LastUpdatedAt => (new DateTime(1970, 1, 1).AddSeconds(LastUpdatedTimestamp / 1000)).ToUniversalTime();

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Id} ({Width}x{Height})";
		}
	}
}