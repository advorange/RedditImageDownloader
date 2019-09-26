using System.Collections.Generic;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pixiv.Models
{
	/// <summary>
	/// Json model for misc info about a Pixiv user.
	/// </summary>
	public sealed class PixivUserProfile
	{
		/// <summary>
		/// Their birthday.
		/// </summary>
		[JsonProperty("birth_date")]
		public string BirthDate { get; private set; }

		/// <summary>
		/// Their bloodtype.
		/// </summary>
		[JsonProperty("blood_type")]
		public string BloodType { get; private set; }

		/// <summary>
		/// Various ways to contact this person.
		/// </summary>
		[JsonProperty("contacts")]
		public IDictionary<string, string> Contacts { get; private set; }

		/// <summary>
		/// Their gender.
		/// </summary>
		[JsonProperty("gender")]
		public string Gender { get; private set; }

		/// <summary>
		/// Their homepage.
		/// </summary>
		[JsonProperty("homepage")]
		public string Homepage { get; private set; }

		/// <summary>
		/// Introduction specified by them.
		/// </summary>
		[JsonProperty("introduction")]
		public string Introduction { get; private set; }

		/// <summary>
		/// Their job.
		/// </summary>
		[JsonProperty("job")]
		public string Job { get; private set; }

		/// <summary>
		/// Their location.
		/// </summary>
		[JsonProperty("location")]
		public string Location { get; private set; }

		/// <summary>
		/// Their tags.
		/// </summary>
		[JsonProperty("tags")]
		public string Tags { get; private set; }

		/// <summary>
		/// Information about how they draw.
		/// </summary>
		[JsonProperty("workspace")]
		[JsonConverter(typeof(PixivWorkspaceConverter))]
		public PixivWorkspace Workspace { get; private set; }
	}
}