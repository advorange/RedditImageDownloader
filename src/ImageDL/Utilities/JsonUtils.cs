using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

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
		/// <summary>
		/// Returns a json string created from the supplied node.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static JToken ConvertHtmlNodeToJToken(HtmlNode node)
		{
			var jObj = new JObject();
			foreach (var attribute in node.Attributes)
			{
				jObj.Add(attribute.Name, System.Net.WebUtility.HtmlDecode(attribute.Value));
			}
			var text = node.InnerText.Trim();
			if (!String.IsNullOrWhiteSpace(text))
			{
				jObj.Add("inner_text", System.Net.WebUtility.HtmlDecode(text));
			}
			if (node.ChildNodes.Any())
			{
				var children = new JArray();
				foreach (var child in node.ChildNodes)
				{
					var childToken = ConvertHtmlNodeToJToken(child);
					if (!childToken.HasValues) //If no values were gotten, this token is unecessary
					{
						continue;
					}
					children.Add(childToken);
				}
				jObj.Add("children", children);
			}
			return jObj;
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