using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.AnimePictures.Models
{
	/// <summary>
	/// Json model for 
	/// </summary>
	public sealed class AnimePicturesPost : IPost, ISize
	{
		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://anime-pictures.net/pictures/view_post/{Id}");
		/// <inheritdoc />
		[JsonProperty("score_number")]
		public int Score { get; private set; }
		/// <inheritdoc />
		[JsonProperty("pubtime")]
		public DateTime CreatedAt { get; private set; }
		/// <inheritdoc />
		[JsonProperty("width")]
		public int Width { get; private set; }
		/// <inheritdoc />
		[JsonProperty("height")]
		public int Height { get; private set; }
		/// <summary>
		/// The hash of the file.
		/// </summary>
		[JsonProperty("md5")]
		public string Md5 { get; private set; }
		/// <summary>
		/// The hash of the pixels.
		/// </summary>
		[JsonProperty("md5_pixels")]
		public string Md5Pixels { get; private set; }
		/// <summary>
		/// 150px biggest side.
		/// </summary>
		[JsonProperty("small_preview")]
		public string SmallPreview { get; private set; }
		/// <summary>
		/// 300px biggest side.
		/// </summary>
		[JsonProperty("medium_preview")]
		public string MediumPreview { get; private set; }
		/// <summary>
		/// 600px biggest side.
		/// </summary>
		[JsonProperty("big_preview")]
		public string BigPreview { get; private set; }
		/// <summary>
		/// Seems like <see cref="Score"/> and this got reversed, because this one is useless but has the Json property name 'score'.
		/// </summary>
		[JsonProperty("score")]
		public int UselessScore { get; private set; }
		/// <summary>
		/// The size of the file.
		/// </summary>
		[JsonProperty("size")]
		public long FileSize { get; private set; }
		/// <summary>
		/// The amount of people who have downloaded this image.
		/// </summary>
		[JsonProperty("download_count")]
		public int DownloadCount { get; private set; }
		/// <summary>
		/// Whether the post is NSFW or not.
		/// </summary>
		[JsonProperty("erotics")]
		public int Erotics { get; private set; }
		/// <summary>
		/// The color associated with this image.
		/// </summary>
		[JsonProperty("color")]
		public IList<int> Color { get; private set; }
		/// <summary>
		/// The file extension.
		/// </summary>
		[JsonProperty("ext")]
		public string Ext { get; private set; }
		/// <summary>
		/// Not sure, assume 1 is success though.
		/// </summary>
		[JsonProperty("status")]
		public int Status { get; private set; }
		/// <summary>
		/// The name of whoever uploaded the image.
		/// </summary>
		[JsonProperty("user_name")]
		public string UserName { get; private set; }
		/// <summary>
		/// The id of whoever uploaded the image.
		/// </summary>
		[JsonProperty("user_id")]
		public int UserId { get; private set; }
		/// <summary>
		/// The profile picture of whoever uploaded the image.
		/// </summary>
		[JsonProperty("user_avatar")]
		public Uri UserAvatar { get; private set; }
		/// <summary>
		/// The names of the tags applied to this image.
		/// </summary>
		[JsonProperty("tags")]
		public IList<string> Tags { get; private set; }
		/// <summary>
		/// The names of the tags with their values applied to this image.
		/// </summary>
		[JsonProperty("tags_full")]
		public IList<AnimePicturesTag> TagsFull { get; private set; }
		/// <summary>
		/// Whether you have starred the image. This will always be false because we're browsing anonymously.
		/// </summary>
		[JsonProperty("star_it")]
		public bool StarIt { get; private set; }
		/// <summary>
		/// Whether you have favorited the image. This will always be false because we're browsing anonymously.
		/// </summary>
		[JsonProperty("is_favorites")]
		public bool IsFavorites { get; private set; }
		/// <summary>
		/// The folder you put this into. This will always be false because we're browsing anonymously.
		/// </summary>
		[JsonProperty("favorite_folder")]
		public string FavoriteFolder { get; private set; }
		/// <summary>
		/// Not sure, maybe lists all the users who have favorited this if you had the permissions?
		/// </summary>
		[JsonProperty("user_favorite_folders")]
		public object UserFavoriteFolders { get; private set; }
		/// <summary>
		/// The direct link to the image.
		/// </summary>
		[JsonIgnore]
		public Uri FileUrl => new Uri(_FileUrl.Replace("?", "").Replace(" ", "+"));

		[JsonProperty("file_url")]
		private string _FileUrl = null;

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IImageDownloaderClient client)
		{
			return Task.FromResult(ImageResponse.FromUrl(FileUrl));
		}
	}
}