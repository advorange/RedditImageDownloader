namespace ImageDL.Enums
{
	/// <summary>
	/// The type of tags to get.
	/// </summary>
	public enum TagType
	{
		/// <summary>
		/// Every tag that the image has on it. General, meta, etc.
		/// </summary>
		All,

		/// <summary>
		/// Tags for what the character is doing or looking like.
		/// </summary>
		General,

		/// <summary>
		/// Tags for who is in the image.
		/// </summary>
		Character,

		/// <summary>
		/// Tags for who owns the image.
		/// </summary>
		Copyright,

		/// <summary>
		/// Tags for who made the image.
		/// </summary>
		Artist,

		/// <summary>
		/// Tags about the image file. Resolution, official, etc.
		/// </summary>
		Meta,
	}
}