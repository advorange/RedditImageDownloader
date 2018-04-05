namespace ImageDL.Enums
{
	/// <summary>
	/// The reason something to do with downloading images failed.
	/// </summary>
	public enum FailureReason
	{
		/// <summary>
		/// Not a failure.
		/// </summary>
		Success,
		/// <summary>
		/// The content is animated, which isn't an image.
		/// </summary>
		AnimatedContent,
		/// <summary>
		/// Unable to find the required thing.
		/// </summary>
		NotFound,
		/// <summary>
		/// An exception occurred.
		/// </summary>
		Exception,
		/// <summary>
		/// Miscellaneous error.
		/// </summary>
		Misc,
		/// <summary>
		/// The file is already downloaded.
		/// </summary>
		AlreadyDownloaded,
		/// <summary>
		/// The file does not fit size requirements. Too small or too big.
		/// </summary>
		DoesNotFitSizeRequirements,
	}
}
