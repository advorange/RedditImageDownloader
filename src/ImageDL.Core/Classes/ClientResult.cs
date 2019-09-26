using System.Net;

namespace ImageDL.Classes
{
	/// <summary>
	/// Result of getting the text from a webpage.
	/// </summary>
	public struct ClientResult<T>
	{
		/// <summary>
		/// Whether or not the request was successful.
		/// </summary>
		public readonly bool IsSuccess;

		/// <summary>
		/// The http status code for the request.
		/// </summary>
		public readonly HttpStatusCode StatusCode;

		/// <summary>
		/// The value of the request.
		/// </summary>
		public readonly T Value;

		/// <summary>
		/// Creates an instance of <see cref="ClientResult{T}"/>.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="statusCode"></param>
		/// <param name="isSuccess"></param>
		public ClientResult(T value, HttpStatusCode statusCode, bool isSuccess)
		{
			Value = value;
			StatusCode = statusCode;
			IsSuccess = isSuccess;
		}
	}
}