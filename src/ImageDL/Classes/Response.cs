using ImageDL.Enums;

namespace ImageDL.Classes
{
	/// <summary>
	/// Response to when gathering or downloading images.
	/// </summary>
	public class Response
	{
		/// <summary>
		/// The broad reason why this failed.
		/// </summary>
		public readonly FailureReason Reason;
		/// <summary>
		/// The more specific reason why this failed.
		/// </summary>
		public readonly string Text;
		/// <summary>
		/// Whether or not the response indicates success.
		/// </summary>
		public bool IsSuccess => Reason == FailureReason.Success;

		/// <summary>
		/// Creates an instance of <see cref="Response"/>.
		/// </summary>
		/// <param name="reason"></param>
		/// <param name="text"></param>
		public Response(FailureReason reason, string text)
		{
			Reason = reason;
			Text = text;
		}
	}
}