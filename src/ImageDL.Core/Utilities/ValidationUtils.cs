using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ImageDL.Classes.ImageDownloading;
using ImageDL.Core.Enums;
using ImageDL.Interfaces;

namespace ImageDL.Core.Utilities
{
	/// <summary>
	/// Utilities for validating posts.
	/// </summary>
	public static class ValidationUtils
	{
		/// <summary>
		/// Adds a validation rule for the time a post was created.
		/// </summary>
		/// <typeparam name="TGatherer"></typeparam>
		/// <typeparam name="TPost"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="priority"></param>
		/// <param name="newest"></param>
		/// <param name="oldest"></param>
		/// <param name="statusForTimeError"></param>
		public static void ValidateTime<TGatherer, TPost>(
			this PostDictionary<TGatherer, TPost> dictionary,
			int priority,
			DateTime newest,
			DateTime oldest,
			PostStatus statusForTimeError = PostStatus.Stop)
			where TGatherer : PostDownloaderBase
			where TPost : IPost
		{
			dictionary.AddValidationRule(priority, x =>
			{
				//Assume chronological order
				var stop = x.CreatedAt > newest || x.CreatedAt < oldest;
				return Task.FromResult(stop ? statusForTimeError : PostStatus.Nothing);
			});
		}
		/// <summary>
		/// Adds a validation rule for the user who created the post.
		/// </summary>
		/// <typeparam name="TGatherer"></typeparam>
		/// <typeparam name="TPost"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="priority"></param>
		/// <param name="username"></param>
		public static void ValidateUsername<TGatherer, TPost>(
			this PostDictionary<TGatherer, TPost> dictionary,
			int priority,
			string username)
			where TGatherer : PostDownloaderBase
			where TPost : IPost
		{
			dictionary.AddValidationRule(priority, x =>
			{
				var ignorable = username != null && x.Author != username;
				return Task.FromResult(ignorable ? PostStatus.Ignorable : PostStatus.Nothing);
			});
		}
		/// <summary>
		/// Adds a validation rule for the minimum score a post can have.
		/// </summary>
		/// <typeparam name="TGatherer"></typeparam>
		/// <typeparam name="TPost"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="priority"></param>
		/// <param name="score"></param>
		public static void ValidateScore<TGatherer, TPost>(
			this PostDictionary<TGatherer, TPost> dictionary,
			int priority,
			int score)
			where TGatherer : PostDownloaderBase
			where TPost : IPost
		{
			dictionary.AddValidationRule(priority, x =>
			{
				var ignorable = x.Score < score;
				return Task.FromResult(ignorable ? PostStatus.Ignorable : PostStatus.Nothing);
			});
		}
	}
}
