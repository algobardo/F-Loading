using System.Web.UI.WebControls;
using System.Web.UI;
using System.Xml;

[assembly: System.Web.UI.WebResource("Fields.images.staticimage_icon.png", "image/png")]
namespace Fields
{
	public class StaticImage : IRenderable
	{
		public StaticImage() { }

		#region IRenderable Membri di

		public Control GetWebControl(System.Web.HttpServerUtility server, XmlNode renderingDocument)
		{
			PlaceHolder ph = new PlaceHolder();
			
			string url = (renderingDocument.Attributes["src"]).Value.FromXmlValue2Render(server);
			Image img = new Image();
			img.ImageUrl = url;
			
			XmlAttribute xmlbl = renderingDocument.Attributes["renderedLabel"];
			if (xmlbl != null)
			{
				Label lb = new Label();
				lb.Text = (renderingDocument.Attributes["renderedLabel"]).Value.FromXmlValue2Render(server);
				ph.Controls.Add(lb);
			}

			if (renderingDocument.Attributes["class"] != null)
				img.CssClass = renderingDocument.Attributes["class"].Value.FromXmlValue2Render(server);
			
			if (renderingDocument.Attributes["rel"] != null)
				img.Attributes.Add("rel", renderingDocument.Attributes["rel"].Value.FromXmlValue2Render(server));

			XmlAttribute descr = renderingDocument.Attributes["description"];
			if (descr != null)
			{
				img.ToolTip = descr.Value.FromXmlValue2Render(server);
				img.AlternateText = descr.Value.FromXmlValue2Render(server);
			}

			ph.Controls.Add(img);
			return ph;
		}

		public string GetValue(Control ctrl, XmlNode renderingDocument)
		{
			return "";
		}

		public string JSON_StyleProperties
		{
			get {
				return @"[
						{ 
						""group"": ""border"",
							""properties"": [
							{
								 ""name"":""border-width"",
								 ""type"":""text"",
								 ""validator"":""size"",
								 ""info"": ""Width of Field Border. Example: 2px""
							},
							{
								 ""name"":""border-color"",
								 ""type"":""text"",
								 ""validator"":""color"",
								 ""info"": ""Color of Field Border. Example: #ff00ff""
							},
							{
								 ""name"":""border-style"",
								 ""type"":""text"",
								 ""validator"":""none"",
								 ""info"": ""Type of Field Border. Example: solid""
							}
						]                     
					}
				] "; 
			}
		}

		#endregion

		public static WebControl Icon
		{
			get
			{
				return new CustomImage("Fields.images.staticimage_icon.png", "Shows a static image");
			}
		}

	}
}
