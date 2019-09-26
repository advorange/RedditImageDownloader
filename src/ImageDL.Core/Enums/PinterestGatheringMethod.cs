namespace ImageDL.Enums
{
	/// <summary>
	/// How to use the string to search Pinterest.
	/// </summary>
	public enum PinterestGatheringMethod
	{
		/// <summary>
		/// Indicates to search for everything a user has on their page.
		/// </summary>
		User,

		/// <summary>
		/// Indicates to search for everything on a board.
		/// </summary>
		Board,

		/// <summary>
		/// Indicates to search for everything with the tags.
		/// </summary>
		Tags,
	}
}