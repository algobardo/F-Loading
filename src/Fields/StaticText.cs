using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Xml;
using System.Web.UI.HtmlControls;

[assembly: System.Web.UI.WebResource("Fields.images.statictext_icon.png", "image/png")]

namespace Fields
{
	public class StaticText : IRenderable
	{

		public StaticText() { }


		#region IRenderable Members

		public Control GetWebControl(System.Web.HttpServerUtility server, System.Xml.XmlNode renderingDocument)
		{
				Label lbl = new Label();
				lbl.Text = (renderingDocument.Attributes["value"]).Value.FromXmlValue2Render(server);
				lbl.Text = lbl.Text + "<br />";
				lbl.CssClass = "staticLabel";
				return lbl;
		}

		public string JSON_StyleProperties
		{
			get
			{
				return
					 @"[{ 
						
					""group"":""background"",
						""properties"": 
                   [
							{
								""name"":""background-color"",
								""type"":""color"",
								""info"": ""Color of text background. Example: #ff00ff""
							},
							{
								""name"":""fgcolor"",
								""type"":""color"",
								""info"": ""Color of text foreground. Example: #0000aa""
							}
						]  
					}] ";
			}
		}

		public string GetValue(Control src, XmlNode renderingDocument)
		{
			return "";
		}

		public static WebControl Icon
		{
			get
			{
				return new CustomImage("Fields.images.statictext_icon.png", "Shows a static text");
			}
		}

		#endregion

	}
}
