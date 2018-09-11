using System;

namespace ImageDL.Classes
{
	/// <summary>
	/// Holds information about an api key.
	/// </summary>
	public struct ApiKey
	{
		/// <summary>
		/// The key to use.
		/// </summary>
		public readonly string Key;
		/// <summary>
		/// When the key was created at.
		/// </summary>
		public readonly DateTime CreatedAt;
		/// <summary>
		/// How long the key is valid for.
		/// </summary>
		public readonly TimeSpan ValidFor;

		/// <summary>
		/// Creates an instance of <see cref="ApiKey"/>.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="validFor"></param>
		public ApiKey(string key, TimeSpan? validFor = null)
		{
			Key = key ?? throw new ArgumentException("The api key cannot be null.", nameof(key));
			CreatedAt = DateTime.UtcNow;
			ValidFor = validFor ?? TimeSpan.FromSeconds(-1);
		}

		/// <inheritdoc />
		public override string ToString() => Key;
	}
}