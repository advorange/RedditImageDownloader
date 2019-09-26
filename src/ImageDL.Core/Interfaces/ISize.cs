namespace ImageDL.Interfaces
{
	/// <summary>
	/// Indicates the object has a size.
	/// </summary>
	public interface ISize
	{
		/// <summary>
		/// The height of the object.
		/// </summary>
		int Height { get; }

		/// <summary>
		/// The width of the object.
		/// </summary>
		int Width { get; }
	}
}