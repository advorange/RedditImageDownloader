using System;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Pixiv.Models
{
	/// <summary>
	/// Not sure why, but Pixiv returns empty array when workspace is empty, but object when full.
	/// </summary>
	internal class PixivWorkspaceConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.StartObject)
			{
				return serializer.Deserialize(reader, typeof(PixivWorkspace));
			}
			else
			{
				reader.Skip();
				return new PixivWorkspace();
			}
		}
		public override bool CanConvert(Type objectType) => true;
	}
}