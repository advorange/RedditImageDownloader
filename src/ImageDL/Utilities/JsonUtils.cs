using System.IO;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace ImageDL.Utilities
{
	/// <summary>
	/// Utilities for Json.
	/// </summary>
	public static class JsonUtils
	{
		/// <summary>
		/// Returns a json string, with no @'s in front of names.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static string ConvertXmlToJson(string xml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			var builder = new StringBuilder();
			using (var writer = new StringWriter(builder))
			{
				JsonSerializer.Create().Serialize(new CustomJsonWriter(writer), doc);
				return builder.ToString();
			}
		}
	}

	//Source: https://stackoverflow.com/a/43485727
	internal class CustomJsonWriter : JsonTextWriter
	{
		public CustomJsonWriter(TextWriter writer) : base(writer) { }

		public override void WritePropertyName(string name)
		{
			if (name.StartsWith("@") || name.StartsWith("#"))
			{
				base.WritePropertyName(name.Substring(1));
			}
			else
			{
				base.WritePropertyName(name);
			}
		}
	}
}