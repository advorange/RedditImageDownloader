using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Booru.Danbooru.Models
{
	/// <summary>
	/// Converts keeper data since it is passed unescaped.
	/// </summary>
	public sealed class KeeperDataConverter : JsonConverter<Dictionary<string, int>>
	{
		/// <inheritdoc />
		public override Dictionary<string, int> ReadJson(JsonReader reader, Type objectType, Dictionary<string, int> existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}

			var input = Regex.Unescape(reader.Value.ToString());
			return JsonConvert.DeserializeObject<Dictionary<string, int>>(input);
		}

		/// <inheritdoc />
		public override void WriteJson(JsonWriter writer, Dictionary<string, int> value, JsonSerializer serializer)
			=> serializer.Serialize(writer, value);
	}
}