namespace ImageDL.Classes
{
	/// <summary>
	/// Response to when gathering or downloading images.
	/// </summary>
	public class Response
	{
		/// <summary>
		/// The broad reason why this failed.
		/// If this is null, this indicates success.
		/// </summary>
		public readonly string ReasonType;
		/// <summary>
		/// The actual reason.
		/// </summary>
		public readonly string Text;
		/// <summary>
		/// Whether or not the response indicates success.
		/// Can be null in the case of an unknown status.
		/// </summary>
		public readonly bool? IsSuccess;

		/// <summary>
		/// Creates an instance of <see cref="Response"/>.
		/// </summary>
		/// <param name="reasonType"></param>
		/// <param name="text"></param>
		/// <param name="isSuccess"></param>
		public Response(string reasonType, string text, bool? isSuccess)
		{
			ReasonType = reasonType;
			Text = text;
			IsSuccess = isSuccess;
		}
	}
}