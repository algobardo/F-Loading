using System.Collections.Generic;
using System.Xml;
using System.Web;

namespace Fields
{
	public static class Utility
	{
		public static List<XmlNode> ToList(this XmlNodeList lst)
		{
			List<XmlNode> nds = new List<XmlNode>();
			foreach (XmlNode nd in lst)
			{
				nds.Add(nd);
			}
			return nds;
		}

		public static string FromXmlValue2Render(this string s, HttpServerUtility sr)
		{
         s = s.Replace("&apos;", "'");
			// incredible but this code is ok...
			return sr.HtmlEncode(sr.HtmlDecode(s));
		}

		public static string FromNotEncodedRender2XmlValue(this string s)
		{
			return xmlEncode(s);
		}

		public static string FromXmlName2Render(this string s, HttpServerUtility sr)
		{
			return sr.HtmlEncode(XmlConvert.DecodeName(s));
		}

		public static string FromNotEncodedRender2XmlName(this string s)
		{
			return XmlConvert.EncodeLocalName(s);
		}

		private static string xmlEncode(string s)
		{
			return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&apos;").Replace("\"", "&quot;");
		}


	}
}
