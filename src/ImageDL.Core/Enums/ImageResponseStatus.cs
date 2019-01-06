namespace ImageDL.Core.Enums
{
	/// <summary>
	/// Indicates the status of a url image wise.
	/// </summary>
	public enum ImageResponseStatus
	{
		/// <summary>
		/// The response indicates this is an image.
		/// </summary>
		Image,
		/// <summary>
		/// The response indicates this is not an image.
		/// </summary>
		NotImage,
		/// <summary>
		/// The response indicates this is something animated.
		/// </summary>
		Animated,
		/// <summary>
		/// The response failed.
		/// </summary>
		Fail,
	}
}
