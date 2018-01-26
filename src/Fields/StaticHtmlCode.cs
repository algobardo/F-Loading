using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Xml;
using System.Web.UI.HtmlControls;

[assembly: System.Web.UI.WebResource("Fields.images.staticcode_icon.png", "image/png")]

namespace Fields
{
	public class StaticHtmlCode : IRenderable
	{

		public StaticHtmlCode() { }

		#region IRenderable Members

		public Control GetWebControl(System.Web.HttpServerUtility server, XmlNode renderingDocument)
		{
			Label lbl = new Label();
			string val = (renderingDocument.Attributes["src"]).Value;
			// decode value from Xml
			val = server.HtmlDecode( val.Replace("&apos;", "'") );

			//check
			if (StaticHtmlCode.CodeCheck(server, renderingDocument))
				lbl.Text = val + "<br />";
			else
				lbl.Text = "<font color='red'>*** Error: Invalid HTML Code ***</font><br />";

			lbl.Style.Add("z-index", "200");
			return lbl;
		}

		public static bool CodeCheck(System.Web.HttpServerUtility server, XmlNode renderingDocument)
		{
			string val = (renderingDocument.Attributes["src"]).Value;
			// decode value
			val = server.HtmlDecode(val.Replace("&apos;", "'"));

			// not allowed code
			val = val.Replace(" ", "").ToLower();
			if ( val.Contains("<script") || val.Contains("<iframe")
					|| val.Contains("</body") || val.Contains("</html") 
				)
				return false;

			int commStart = val.LastIndexOf("<!--");
			if (commStart >= 0 && val.LastIndexOf("-->") < commStart)
				return false;
			
			return true;
		}

		public static WebControl Icon
		{
			get
			{
				return new CustomImage("Fields.images.staticcode_icon.png", "With this field you can embed html code");
			}
		}

		public string JSON_StyleProperties
		{
			get{	return ""; }
		}

		public string GetValue(Control src, XmlNode renderingDocument)
		{
			return null;
		}

		#endregion
	}
}