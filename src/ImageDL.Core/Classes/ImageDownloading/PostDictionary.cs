using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Core.Enums;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading
{
	/// <summary>
	/// A collection of posts being gathered.
	/// </summary>
	/// <typeparam name="TGatherer"></typeparam>
	/// <typeparam name="TPost"></typeparam>
	public abstract class PostDictionary<TGatherer, TPost>
			where TGatherer : PostDownloaderBase
			where TPost : IPost
	{
		/// <summary>
		/// All of the current posts.
		/// </summary>
		public IEnumerable<TPost> Values => _InnerDictionary.Values;
		/// <summary>
		/// The amount of currently stored posts.
		/// </summary>
		public int Count => _InnerDictionary.Count;
		/// <summary>
		/// Token used to check whether more comments are still allowed to be downloaded.
		/// </summary>
		public CancellationToken CanGetMorePostsToken => _CanGetMorePostsSource.Token;

		private readonly ConcurrentDictionary<string, TPost> _InnerDictionary = new ConcurrentDictionary<string, TPost>();
		private readonly List<ValidationRule> _ValidationRules = new List<ValidationRule>();
		private readonly CancellationTokenSource _CanGetMorePostsSource = new CancellationTokenSource();
		private readonly TGatherer Parent;
		private readonly CancellationToken GlobalToken;

		/// <summary>
		/// Creates an instance of <see cref="PostDictionary{TGatherer, TPost}"/>.
		/// </summary>
		/// <param name="gatherer"></param>
		/// <param name="globalToken"></param>
		public PostDictionary(TGatherer gatherer, CancellationToken globalToken)
		{
			Parent = gatherer;
			GlobalToken = globalToken;
		}

		/// <summary>
		/// Makes sure the post is not a duplicate and saves it.
		/// </summary>
		/// <param name="post"></param>
		/// <returns>True = can save more posts, False = cannot save more posts.</returns>
		public async Task<bool> SaveAsync(TPost post)
		{
			GlobalToken.ThrowIfCancellationRequested();

			bool CancelTokenIfFalse(bool value)
			{
				//If false, cancel the token because false means stop gatherering
				if (!value)
				{
					_CanGetMorePostsSource.Cancel();
				}
				return value;
			}

			foreach (var rule in _ValidationRules)
			{
				switch (await rule.Func(post).CAF())
				{
					case PostStatus.Nothing:
						break;
					case PostStatus.Ignorable:
						return CancelTokenIfFalse(true);
					case PostStatus.Stop:
						return CancelTokenIfFalse(false);
				}
			}

			//Return true to signify keep collecting, but don't add it because duplicate.
			if (_InnerDictionary.ContainsKey(post.Id))
			{
				return true;
			}
			if (!_InnerDictionary.TryAdd(post.Id, post))
			{
				throw new InvalidOperationException($"Unable to add {post.Id}.");
			}
			//Print out to the console every 50 retrieved
			if (_InnerDictionary.Count % 50 == 0)
			{
				var nameWithSpace = Parent.DownloaderName != null ? $" {Parent.DownloaderName}" : "";
				ConsoleUtils.WriteLine($"{_InnerDictionary.Count}{nameWithSpace} posts found.");
			}

			return CancelTokenIfFalse(_InnerDictionary.Count < Parent.AmountOfPostsToGather);
		}
		/// <summary>
		/// Adds a validation rule for a post being added.
		/// </summary>
		/// <param name="priority"></param>
		/// <param name="rule"></param>
		public void AddValidationRule(int priority, Func<TPost, Task<PostStatus>> rule)
		{
			_ValidationRules.Add(new ValidationRule(priority, rule));
			_ValidationRules.Sort((x, y) => x.Priority.CompareTo(y.Priority));
		}

		private readonly struct ValidationRule
		{
			public readonly int Priority;
			public readonly Func<TPost, Task<PostStatus>> Func;

			public ValidationRule(int priority, Func<TPost, Task<PostStatus>> func)
			{
				Priority = priority;
				Func = func;
			}
		}
	}
}