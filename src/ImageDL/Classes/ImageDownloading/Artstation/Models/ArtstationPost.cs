using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Artstation.Models
{
	/// <summary>
	/// Json model for a post from Artstation.
	/// </summary>
	public sealed class ArtstationPost : IPost
	{
		/// <inheritdoc />
		[JsonProperty("hash_id")]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonProperty("permalink")]
		public Uri PostUrl { get; private set; }
		/// <inheritdoc />
		[JsonProperty("likes_count")]
		public int Score { get; private set; }
		/// <inheritdoc />
		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; private set; }
		/// <summary>
		/// Whether you have liked this post. Will always be falsed because we're browsing anonymously.
		/// </summary>
		[JsonProperty("liked")]
		public bool Liked { get; private set; }
		/// <summary>
		/// Names of some tags associated with the post.
		/// </summary>
		[JsonProperty("tags")]
		public IList<string> Tags { get; private set; }
		/// <summary>
		/// Whether the post is adult.
		/// </summary>
		[JsonProperty("hide_as_adult")]
		public bool HideAsAdult { get; private set; }
		/// <summary>
		/// Whether the post is public.
		/// </summary>
		[JsonProperty("visible_on_artstation")]
		public bool VisibleOnArtstation { get; private set; }
		/// <summary>
		/// The images in the post.
		/// </summary>
		[JsonProperty("assets")]
		public IList<ArtstationAsset> Assets { get; private set; }
		/// <summary>
		/// Not sure, couldn't find a populated example.
		/// </summary>
		[JsonProperty("collections")]
		public IList<object> Collections { get; private set; }
		/// <summary>
		/// The user who posted this post.
		/// </summary>
		[JsonProperty("user")]
		public ArtstationUser User { get; private set; }
		/// <summary>
		/// How the images were created.
		/// </summary>
		[JsonProperty("medium")]
		public ArtstationMedium Medium { get; private set; }
		/// <summary>
		/// Categories this post belongs to. Similar to tags on other websites.
		/// </summary>
		[JsonProperty("categories")]
		public IList<ArtstationCategory> Categories { get; private set; }
		/// <summary>
		/// Not sure, couldn't find a populated example.
		/// </summary>
		[JsonProperty("software_items")]
		public IList<object> SoftwareItems { get; private set; }
		/// <summary>
		/// The id of the user who posted this.
		/// </summary>
		[JsonProperty("user_id")]
		public int UserId { get; private set; }
		/// <summary>
		/// The title of the post.
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; private set; }
		/// <summary>
		/// The description of the post. This is in HTML.
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; private set; }
		/// <summary>
		/// When the post was last updated.
		/// </summary>
		[JsonProperty("updated_at")]
		public DateTime UpdatedAt { get; private set; }
		/// <summary>
		/// How many views a post has.
		/// </summary>
		[JsonProperty("views_count")]
		public int ViewsCount { get; private set; }
		/// <summary>
		/// How many comments a post has.
		/// </summary>
		[JsonProperty("comments_count")]
		public int CommentsCount { get; private set; }
		/// <summary>
		/// The first image in the post.
		/// </summary>
		[JsonProperty("cover_url")]
		public Uri CoverUrl { get; private set; }
		/// <summary>
		/// When the post was published.
		/// </summary>
		[JsonProperty("published_at")]
		public DateTime PublishedAt { get; private set; }
		/// <summary>
		/// If the post has been picked by the editors.
		/// </summary>
		[JsonProperty("editor_pick")]
		public bool EditorPick { get; private set; }
		/// <summary>
		/// If the post has adult content.
		/// </summary>
		[JsonProperty("adult_content")]
		public bool AdultContent { get; private set; }
		/// <summary>
		/// Not sure, worse adult content?
		/// </summary>
		[JsonProperty("admin_adult_content")]
		public bool AdminAdultContent { get; private set; }
		/// <summary>
		/// Additional part of a url to access this post.
		/// </summary>
		[JsonProperty("slug")]
		public string Slug { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("suppressed")]
		public bool Suppressed { get; private set; }
		/// <summary>
		/// Whether the post is public.
		/// </summary>
		[JsonProperty("visible")]
		public bool Visible { get; private set; }
		/// <summary>
		/// The number this post was posted in on the website.
		/// </summary>
		[JsonProperty("id")]
		public int NumberId { get; private set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IDownloaderClient client)
		{
			var images = Assets.Where(x => x.AssetType == "image").Select(x => x.ImageUrl);
			return Task.FromResult(ImageResponse.FromImages(images));
		}
		/// <summary>
		/// Returns the id.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Id;
		}
	}
}