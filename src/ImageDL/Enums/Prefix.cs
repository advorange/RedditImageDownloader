namespace ImageDL.Enums
{
	/// <summary>
	/// Whether or not a prefix is required for setting parsing.
	/// </summary>
	public enum PrefixState
	{
		/// <summary>
		/// Indicates the prefix is required.
		/// </summary>
		Required,
		/// <summary>
		/// Indicates the prefix is optional.
		/// </summary>
		Optional,
		/// <summary>
		/// Indicates the prefix must not be there.
		/// </summary>
		NotPrefixed,
	}
}