namespace ImageDL.ImageDownloaders.RedditDownloader
{
	/// <summary>
	/// Provides the information for downloading images from reddit.
	/// </summary>
	public class RedditImageDownloaderArguments : ImageDownloaderArguments
	{
		private string _Subreddit;
		private int _ScoreThreshold;

		public string Subreddit
		{
			get => _Subreddit;
			set
			{
				_Subreddit = value;
				AddArgumentToSetArguments();
			}
		}
		public int ScoreThreshold
		{
			get => _ScoreThreshold;
			set
			{
				_ScoreThreshold = value;
				AddArgumentToSetArguments();
			}
		}

		public RedditImageDownloaderArguments(string[] args) : base(args, typeof(RedditImageDownloaderArguments)) { }
	}
}
