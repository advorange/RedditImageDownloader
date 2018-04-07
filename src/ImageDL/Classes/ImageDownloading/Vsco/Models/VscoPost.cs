using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageDL.Enums;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Vsco.Models
{
	/// <summary>
	/// Json model for a post from Vsco.
	/// </summary>
	public sealed class VscoPost : IPost
	{
		/// <inheritdoc />
		[JsonProperty("_id")]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonProperty("permalink")]
		public Uri PostUrl { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => -1;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt => (new DateTime(1970, 1, 1).AddSeconds(UploadDateTimestamp / 1000)).ToUniversalTime();
		/// <summary>
		/// The time the photo was taken at.
		/// </summary>
		[JsonIgnore]
		public DateTime CapturedAt => (new DateTime(1970, 1, 1).AddSeconds(CaptureDateTimestamp / 1000)).ToUniversalTime();
		/// <summary>
		/// The time the photo was last updated at.
		/// </summary>
		[JsonIgnore]
		public DateTime LastUpdatedAt => (new DateTime(1970, 1, 1).AddSeconds(LastUpdatedTimestamp / 1000)).ToUniversalTime();
		/// <summary>
		/// The name of the gallery.
		/// </summary>
		[JsonProperty("grid_name")]
		public string GridName { get; private set; }
		/// <summary>
		/// The id of the image prefixed by /i/.
		/// </summary>
		[JsonProperty("adaptive_base")]
		public string AdaptiveBase { get; private set; }
		/// <summary>
		/// The user's id.
		/// </summary>
		[JsonProperty("site_id")]
		public int SiteId { get; private set; }
		/// <summary>
		/// The description of the image.
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; private set; }
		/// <summary>
		/// Same as <see cref="Description"/>.
		/// </summary>
		[JsonProperty("description_anchored")]
		public string DescriptionAnchored { get; private set; }
		/// <summary>
		/// The kinds of copyright on this image. Usually is 'restricted'.
		/// </summary>
		[JsonProperty("copyright_classes")]
		public List<string> CopyrightClasses { get; private set; }
		/// <summary>
		/// Where the photo was taken.
		/// </summary>
		[JsonProperty("location_coords")]
		public List<double> LocationCoords { get; private set; }
		/// <summary>
		/// If the image has any <see cref="LocationCoords"/>.
		/// </summary>
		[JsonProperty("has_location")]
		public bool HasLocation { get; private set; }
		/// <summary>
		/// Not sure what this is.
		/// </summary>
		[JsonProperty("feature_link")]
		public string FeatureLink { get; private set; }
		/// <summary>
		/// Not sure what this is.
		/// </summary>
		[JsonProperty("is_featured")]
		public bool IsFeatured { get; private set; }
		/// <summary>
		/// Whether or not the image is a video.
		/// </summary>
		[JsonProperty("is_video")]
		public bool IsVideo { get; private set; }
		/// <summary>
		/// Vsco.co/user's name.
		/// </summary>
		[JsonProperty("perma_domain")]
		public string PermaDomain { get; private set; }
		/// <summary>
		/// The user's name.
		/// </summary>
		[JsonProperty("perma_subdomain")]
		public string PermaSubdomain { get; private set; }
		/// <summary>
		/// The link to sharing this post.
		/// </summary>
		[JsonProperty("share_link")]
		public Uri ShareLink { get; private set; }
		/// <summary>
		/// The link to the image without any http or https.
		/// </summary>
		[JsonProperty("responsive_url")]
		public string ResponsiveUrl { get; private set; }
		/// <summary>
		/// Whether or not this has a location.
		/// </summary>
		[JsonProperty("show_location")]
		public int ShowLocation { get; private set; }
		/// <summary>
		/// The status of the image. Usually holds the same time as <see cref="CreatedAt"/> and is ok.
		/// </summary>
		[JsonProperty("image_status")]
		public VscoImageStatus ImageStatus { get; private set; }
		/// <summary>
		/// Various metadata about the image. Such as the camera used to take the photo.
		/// </summary>
		[JsonProperty("image_meta")]
		public VscoImageMeta ImageMeta { get; private set; }
		/// <summary>
		/// The filter applied to the image.
		/// </summary>
		[JsonProperty("preset")]
		public VscoPreset Preset { get; private set; }
		/// <summary>
		/// The height of the image.
		/// </summary>
		[JsonProperty("height")]
		public int Height { get; private set; }
		/// <summary>
		/// The width of the image
		/// </summary>
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <summary>
		/// The hashtags in the description.
		/// </summary>
		[JsonProperty("tags")]
		public List<VscoTag> Tags { get; private set; }
		/// <summary>
		/// The unix timestamp in milliseconds of when this picture was taken.
		/// </summary>
		[JsonProperty("capture_date")]
		public long CaptureDateTimestamp { get; private set; }
		/// <summary>
		/// The same as <see cref="CaptureDateTimestamp"/> but suffixed with MS for some reason.
		/// </summary>
		[JsonProperty("capture_date_ms")]
		public long CaptureDateTimestampMS { get; private set; }
		/// <summary>
		/// The unix timestamp in milliseconds of when this picture was uploaded.
		/// </summary>
		[JsonProperty("upload_date")]
		public long UploadDateTimestamp { get; private set; }
		/// <summary>
		/// The unix timestamp in milliseconds of when this picture was last updated.
		/// </summary>
		[JsonProperty("last_updated")]
		public long LastUpdatedTimestamp { get; private set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			var images = new[] { new Uri($"https://{ResponsiveUrl}") };
			return Task.FromResult(new ImageResponse(FailureReason.Success, null, images));
		}
		/// <summary>
		/// Returns the id, width, and height.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Id} ({Width}x{Height})";
		}
	}
}