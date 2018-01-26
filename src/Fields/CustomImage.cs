using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Fields
{
	public class CustomImage : System.Web.UI.WebControls.Image
	{
		private string ImageAddr;
		private Type FieldType;

		public CustomImage(string imageAddr, Type fieldType, string tooltip)
			: base()
		{
			if (String.IsNullOrEmpty(imageAddr))
				throw new ArgumentNullException("Empty resource address");

			if (fieldType.Assembly.GetManifestResourceInfo(imageAddr) == null)
				throw new ArgumentException("Invalid resource address");

			this.ImageAddr = imageAddr;
			this.FieldType = fieldType;
			this.ToolTip = tooltip;
		}

		public CustomImage(string imageAddr, Type fieldType)
			: this(imageAddr, fieldType, "")
		{ }

		public CustomImage(string imageAddr, string tooltip)
			: this(imageAddr, typeof(CustomImage), tooltip)
		{ }

		public CustomImage(string imageAddr)
			: this(imageAddr, typeof(CustomImage), "")
		{ }

		protected override void Render(HtmlTextWriter writer)
		{
			this.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(this.FieldType, this.ImageAddr);
			base.Render(writer);
		}
	}

}
