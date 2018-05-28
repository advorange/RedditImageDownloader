namespace ImageDL.Enums
{
	/// <summary>
	/// The method of how to gather Twitter posts.
	/// </summary>
	public enum TwitterGatheringMethod
	{
		/// <summary>
		/// Look through a user's posts.
		/// </summary>
		User,
		/// <summary>
		/// Use the Twitter search function to find matching posts.
		/// </summary>
		Search,
	}
}