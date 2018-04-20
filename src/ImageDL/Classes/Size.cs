using ImageDL.Interfaces;

namespace ImageDL.Classes
{
	/// <summary>
	/// Represents the size of something.
	/// </summary>
	public struct Size : ISize
	{
		/// <inheritdoc />
		public int Width { get; }
		/// <inheritdoc />
		public int Height { get; }

		/// <summary>
		/// Creates an instance of <see cref="Size"/>.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public Size(int width, int height)
		{
			Width = width;
			Height = height;
		}
	}
}