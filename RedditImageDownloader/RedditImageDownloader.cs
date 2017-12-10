using RedditSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace RedditImageDownloader
{
	public class RedditImageDownloader
	{
		private const string IMGUR = "imgur";
		private const string ALBUM = "/a/";
		private const string GALLERY = "/gallery/";
		private Dictionary<Type, Func<string, object>> _TryParses = new Dictionary<Type, Func<string, object>>
		{
			{ typeof(bool), (value) => bool.TryParse(value, out var result) ? result : false },
			{ typeof(int), (value) => int.TryParse(value, out var result) ? result : default },
			{ typeof(int?), (value) => int.TryParse(value, out var result) ? result as int? : null },
			{ typeof(uint), (value) => uint.TryParse(value, out var result) ? result : default },
			{ typeof(uint?), (value) => uint.TryParse(value, out var result) ? result as uint? : null },
			{ typeof(long), (value) => long.TryParse(value, out var result) ? result : default },
			{ typeof(long?), (value) => long.TryParse(value, out var result) ? result as long? : null },
			{ typeof(ulong), (value) => ulong.TryParse(value, out var result) ? result : default },
			{ typeof(ulong?), (value) => ulong.TryParse(value, out var result) ? result as ulong? : null },
		};

		private FieldInfo[] _Arguments = typeof(RedditImageDownloader)
			.GetFields(BindingFlags.Public | BindingFlags.Instance)
			.Where(x => x.FieldType.IsPrimitive || x.FieldType == typeof(string))
			.ToArray();
		private Dictionary<FieldInfo, bool> _SetArguments = new Dictionary<FieldInfo, bool>();
		private FieldInfo[] _UnsetArguments => this._Arguments.Where(x => !this._SetArguments.TryGetValue(x, out var b) || !b).ToArray();

		private Reddit _Reddit = new Reddit(new WebAgent(), false);
		public string Subreddit;
		public string Folder;
		public int AmountToDownload;
		public int ScoreThreshold;

		public bool IsReady => !this._UnsetArguments.Any();

		public RedditImageDownloader(string[] args)
		{
			SetArguments(args);
		}

		public void SetArguments(string[] args)
		{
			foreach (var argument in args)
			{
				//Split, left side is the arg name, right is value
				var split = argument.Split(new[] { ':' }, 2);
				if (split.Length != 2)
				{
					Console.WriteLine($"Unable to split \"{argument}\" to the correct length.");
					continue;
				}

				//See if any arguments have the supplied name
				var property = this._Arguments.SingleOrDefault(x => x.Name.CaseInsEquals(split[0]));
				if (property == null)
				{
					Console.WriteLine($"{split[0]} is not a valid argument name.");
					continue;
				}

				//If number then use the tryparses, if string just set, if neither then nothing
				if (this._TryParses.TryGetValue(property.FieldType, out var f))
				{
					var value = f(split[1]);
					property.SetValue(this, value);
				}
				else if (property.FieldType == typeof(string))
				{
					property.SetValue(this, split[1]);
				}
				else
				{
					Console.WriteLine($"Unable to set the value for {property.Name}.");
					continue;
				}

				Console.WriteLine($"Successfully set {property.Name} to {property.GetValue(this)}.");
				if (!this._SetArguments.TryGetValue(property, out var b))
				{
					this._SetArguments[property] = true;
				}
			}
		}
		public void AskForArguments()
		{
			var sb = new StringBuilder("The following arguments need to be set:" + Environment.NewLine);
			foreach (var argument in this._UnsetArguments)
			{
				sb.AppendLine($"\t{argument.Name} ({argument.FieldType.Name})");
			}
			Console.WriteLine(sb.ToString().Trim());
		}

		public string[] GetImageUrls(Uri uri)
		{
			var url = uri.ToString();
			switch (url)
			{
				//Imgur
				case string u when url.Contains(IMGUR) && url.Contains(ALBUM):
				{
					return ImgurScraper.Scrape(url);
				}
				case string u when url.Contains(IMGUR) && url.Contains(GALLERY):
				{
					return ImgurScraper.Scrape(url);
				}
				default:
				{
					return new[] { url };
				}
				//TODO: specify for tumblr and reddit image hosting
			}
		}
		public string CorrectUrl(string url)
		{
			switch (url)
			{
				//Don't try downloading videos from youtube
				case string u when false
				|| url.Contains("youtu.be") || url.Contains("youtube")
				|| url.Contains("gfycat")
				|| url.Contains(".gifv") || url.Contains(".gif"):
				{
					return null;
				}
				case string u when url.Contains(IMGUR) && url.Contains("_d") && url.Contains("maxwidth"):
				{
					//I don't know what the purpose of maxwidth is, but when it's in the url it always adds _d to the image id
					//and if that's left in it makes the image really small, so that's why both get removed.
					return u.Substring(0, u.IndexOf("?")).Replace("_d", "");
				}
				case string u when String.IsNullOrWhiteSpace(Path.GetExtension(url)):
				{
					return u + ".png";
				}
				default:
				{
					return url;
				}
			}
		}
		public void DownloadImages()
		{
			var subreddit = this._Reddit.GetSubreddit(this.Subreddit);
			var validPosts = subreddit.Hot.Where(x => !x.IsStickied && !x.IsSelfPost && x.Score >= this.ScoreThreshold);
			var posts = validPosts.Take(this.AmountToDownload);

			//Look through each post
			var element = 0;
			foreach (var post in posts)
			{
				Console.WriteLine($"[#{++element}|\u2191{post.Score}] {post.Url}");
				//Some links might have more than one image
				foreach (var url in GetImageUrls(post.Url))
				{
					var correctUrl = CorrectUrl(url);
					if (correctUrl == null)
					{
						continue;
					}

					DownloadImage(correctUrl);
				}
			}
		}
		public void DownloadImage(string url)
		{
			//Don't bother redownloading files
			var savePath = Path.Combine(this.Folder, url.Split('/').Last());
			if (File.Exists(savePath))
			{
				return;
			}

			try
			{
				var req = (HttpWebRequest)WebRequest.Create(url);
				req.Timeout = 5000;
				req.ReadWriteTimeout = 5000;
				using (var resp = req.GetResponse())
				{
					using (var s = resp.GetResponseStream())
					{
						var bitmap = new Bitmap(s);
						if (bitmap == null)
						{
							Console.WriteLine($"\t{url} is not an image.");
							return;
						}
						else if (bitmap.PhysicalDimension.Width < 200 || bitmap.PhysicalDimension.Height < 200)
						{
							Console.WriteLine($"\t{url} is too small.");
							return;
						}

						bitmap.Save(savePath, ImageFormat.Png);
						Console.WriteLine($"\tSuccessfully downloaded {url}.");
					}
				}
			}
			catch (Exception e)
			{
				HelperActions.WriteException(e);
				var errorFile = new FileInfo(Path.Combine(this.Folder, "FailedDownloads.txt"));
				using (var writer = errorFile.AppendText())
				{
					writer.WriteLine(url);
				}
			}
		}
	}
}
