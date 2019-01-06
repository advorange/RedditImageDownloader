namespace ImageDL.Core.Enums
{
	/// <summary>
	/// The status of a post.
	/// </summary>
	public enum PostStatus
	{
		/// <summary>
		/// Don't do anything special.
		/// </summary>
		Nothing,
		/// <summary>
		/// Don't add the item to the dictionary.
		/// </summary>
		Ignorable,
		/// <summary>
		/// Stop all downloading now.
		/// </summary>
		Stop,
	}
}
