using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageDL.Interfaces;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Weibo.Models
{
	/// <summary>
	/// Json model for a post from Weibo.
	/// </summary>
	public sealed class WeiboPost : IPost
	{
		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; private set; }
		/// <inheritdoc />
		[JsonIgnore]
		public Uri PostUrl => new Uri($"https://weibo.com/{User.Id}/{BId}");
		/// <inheritdoc />
		[JsonIgnore]
		public int Score => AttitudesCount;
		/// <inheritdoc />
		[JsonIgnore]
		public DateTime CreatedAt
		{
			get
			{
				var parts = CreatedAtDate.Split('-').Select(x =>
				{
					var valid = new string(x.TakeWhile(c => Char.IsNumber(c)).ToArray());
					return Convert.ToInt32(valid);
				}).ToArray();
				switch (parts.Length)
				{
					case 1: //E.G. "14chinese" means 14 hours ago
						return DateTime.UtcNow - TimeSpan.FromHours(parts[0]);
					case 2: //E.G. "5-20" means May 20th
						return new DateTime(DateTime.UtcNow.Year, parts[0], parts[1]);
					case 3: //E.G. "17-5-20" means May 20th, 2017
						return new DateTime(parts[0], parts[1], parts[2]);
					default:
						throw new InvalidOperationException($"Invalid {nameof(CreatedAtDate)} received.");
				}
			}
		}
		/// <summary>
		/// Will be mm-dd if it was posted in the current year, otherwise will be yyyy-mm-dd
		/// </summary>
		[JsonProperty("created_at")]
		public string CreatedAtDate { get; private set; }
		/// <summary>
		/// The id again.
		/// </summary>
		[JsonProperty("idstr")]
		public string IdString { get; private set; }
		/// <summary>
		/// The id again again.
		/// </summary>
		[JsonProperty("mid")]
		public string MId { get; private set; }
		/// <summary>
		/// Whether you can edit the post.
		/// </summary>
		[JsonProperty("can_edit")]
		public bool CanEdit { get; private set; }
		/// <summary>
		/// The text of the post. Will be in Html.
		/// </summary>
		[JsonProperty("text")]
		public string Text { get; private set; }
		/// <summary>
		/// The length of the text.
		/// </summary>
		[JsonProperty("textLength")]
		public int TextLength { get; private set; }
		/// <summary>
		/// How the post was created.
		/// </summary>
		[JsonProperty("source")]
		public string Source { get; private set; }
		/// <summary>
		/// Whether you have favorited the post.
		/// </summary>
		[JsonProperty("favorited")]
		public bool Favorited { get; private set; }
		/// <summary>
		/// The url to the thumbnail.
		/// </summary>
		[JsonProperty("thumbnail_pic")]
		public Uri ThumbnailUrl { get; private set; }
		/// <summary>
		/// The url to the middle sized picture.
		/// </summary>
		[JsonProperty("bmiddle_pic")]
		public Uri MiddlePictureUrl { get; private set; }
		/// <summary>
		/// The url to the original picture.
		/// </summary>
		[JsonProperty("original_pic")]
		public Uri OriginalPictureUrl { get; private set; }
		/// <summary>
		/// Whether the post is paid promotion.
		/// </summary>
		[JsonProperty("is_paid")]
		public bool IsPaid { get; private set; }
		/// <summary>
		/// Whether the post is from a VIP.
		/// </summary>
		[JsonProperty("mblog_vip_type")]
		public int BlogVipType { get; private set; }
		/// <summary>
		/// The user who posted this.
		/// </summary>
		[JsonProperty("user")]
		public WeiboUser User { get; private set; }
		/// <summary>
		/// How many reposts this has.
		/// </summary>
		[JsonProperty("reposts_count")]
		public int RepostsCount { get; private set; }
		/// <summary>
		/// How many comments this has.
		/// </summary>
		[JsonProperty("comments_count")]
		public int CommentsCount { get; private set; }
		/// <summary>
		/// How many likes this has.
		/// </summary>
		[JsonProperty("attitudes_count")]
		public int AttitudesCount { get; private set; }
		/// <summary>
		/// Whether this is pending approval.
		/// </summary>
		[JsonProperty("pending_approval_count")]
		public int PendingApprovalCount { get; private set; }
		/// <summary>
		/// Whether the text needs to be truncated.
		/// </summary>
		[JsonProperty("isLongText")]
		public bool IsLongText { get; private set; }
		/// <summary>
		/// The visibility of the post.
		/// </summary>
		[JsonProperty("visible")]
		public WeiboVisible Visible { get; private set; }
		/// <summary>
		/// Whether this has more information.
		/// </summary>
		[JsonProperty("more_info_type")]
		public int MoreInfoType { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("cardid")]
		public string CardId { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("content_auth")]
		public int ContentAuth { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("mblogtype")]
		public int BlogType { get; private set; }
		/// <summary>
		/// Whether this is currently the user's most recent/top post.
		/// </summary>
		[JsonProperty("isTop")]
		public int IsTop { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("weibo_position")]
		public int WeiboPosition { get; private set; }
		/// <summary>
		/// Information about the page this was posted to.
		/// </summary>
		[JsonProperty("page_info")]
		public WeiboPageInfo PageInfo { get; private set; }
		/// <summary>
		/// Another unique id for the post.
		/// </summary>
		[JsonProperty("bid")]
		public string BId { get; private set; }
		/// <summary>
		/// The picture of the post.
		/// </summary>
		[JsonProperty("pics")]
		public IList<WeiboPicture> Pictures { get; private set; }
		/// <summary>
		/// The title of the post.
		/// </summary>
		[JsonProperty("title")]
		public WeiboTitle Title { get; private set; }
		/// <summary>
		/// How many views this has. This will be in the form of number then Chinese letters. The Chinese translates to 'views'
		/// </summary>
		[JsonProperty("obj_ext")]
		public string ObjExt { get; private set; }
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("picStatus")]
		public string PicStatus { get; private set; }

		/// <inheritdoc />
		public Task<ImageResponse> GetImagesAsync(IDownloaderClient client) => Task.FromResult(ImageResponse.FromImages(Pictures.Select(x => x.Large.Url)));
		/// <summary>
		/// Returns the id.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => Id;
	}
}