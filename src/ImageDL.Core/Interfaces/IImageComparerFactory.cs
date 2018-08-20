namespace ImageDL.Interfaces
{
	/// <summary>
	/// Interface for having a comparer dedicated for each folder.
	/// </summary>
	public interface IImageComparerFactory
	{
		/// <summary>
		/// Creates a comparer from the supplied file.
		/// </summary>
		/// <param name="databasePath"></param>
		/// <returns></returns>
		IImageComparer CreateComparer(string databasePath);
	}
}