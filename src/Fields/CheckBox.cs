using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: System.Web.UI.WebResource("Fields.images.checkbox_icon.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.checkbox_preview.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.checkbox_preview_tooltip.png", "image/png")]
namespace Fields
{
	public class CheckBoxField : IBaseType
	{
		private XmlSchema baseSchema;
		private bool text = false;

		#region constructors

		public CheckBoxField()
		{
			baseSchema = new XmlSchema();

			//<xs:complexType name="CheckBoxN">
			XmlSchemaComplexType newType = new XmlSchemaComplexType();
			// the name must be filled later
			newType.Name = "";
			baseSchema.Items.Add(newType);

			//<xs:complexContent>
			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			newType.ContentModel = complexContent;

			//<xs:extension base="CheckBox">
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
			complexContent.Content = complexContentExtension;
			complexContentExtension.BaseTypeName = new XmlQualifiedName("CheckBoxField");

			//<xs:sequence>
			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexContentExtension.Particle = seq;

			//<xs:element name="Value" type="xs:boolean" defeult="false" />
			XmlSchemaElement elem = new XmlSchemaElement();
			seq.Items.Add(elem);
			elem.Name = "Value";
			elem.SchemaTypeName = new XmlQualifiedName("boolean", "http://www.w3.org/2001/XMLSchema");
			elem.DefaultValue = "false";
		}

		public CheckBoxField(XmlSchema schema, string name)
		{
			// in this constructor it's useful to set the ID, cause GUI is calling it
			this.Name = name;

			//why don't use the former constructor? <---
			baseSchema = new XmlSchema();

			//<xs:complexType name="CheckBoxN">
			XmlSchemaComplexType newType = new XmlSchemaComplexType();
			newType.Name = ((XmlSchemaComplexType)schema.Items[0]).Name;
			baseSchema.Items.Add(newType);

			//<xs:complexContent>
			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			newType.ContentModel = complexContent;

			//<xs:extension base="CheckBox">
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
			complexContent.Content = complexContentExtension;
			complexContentExtension.BaseTypeName = new XmlQualifiedName("CheckBox");

			//<xs:sequence>
			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexContentExtension.Particle = seq;

			//<xs:element name="Value" type="xs:boolean" default="false"/>
			XmlSchemaElement elem = new XmlSchemaElement();
			seq.Items.Add(elem);
			elem.Name = "Value";
			elem.SchemaTypeName = new XmlQualifiedName("boolean", "http://www.w3.org/2001/XMLSchema");
			elem.DefaultValue = "false";
		}

		#endregion

		#region IField Members

		public string TypeName
		{
			set { ((XmlSchemaComplexType)baseSchema.Items[0]).Name = value; }
			get { return ((XmlSchemaComplexType)baseSchema.Items[0]).Name; }
		}

		public System.Xml.Schema.XmlSchemaComplexType TypeSchema
		{
			get { return (XmlSchemaComplexType)baseSchema.Items[0]; }
		}

		public System.Web.UI.Control GetWebControl(System.Web.HttpServerUtility server, System.Xml.XmlNode renderingDocument)
		{
			PlaceHolder ph = new PlaceHolder();

			Label lbl = new Label();
			if (renderingDocument.Attributes["renderedLabel"] != null)
				lbl.Text = renderingDocument.Attributes["renderedLabel"].Value.FromXmlValue2Render(server);
			else
				lbl.Text = this.Name.FromXmlName2Render(server);
			lbl.CssClass = "label";
			ph.Controls.Add(lbl);

			CheckBoxControl cbox = new CheckBoxControl(this);
			cbox.CausesValidation = false;

			if (renderingDocument.Attributes["class"] != null)
				cbox.CssClass = renderingDocument.Attributes["class"].Value.FromXmlValue2Render(server);
			//if (renderingDocument.Attributes["rel"] != null)
			//	cbox.Attributes.Add("rel", renderingDocument.Attributes["rel"].Value.FromXmlValue2Render(server));
			if (renderingDocument.Attributes["description"] != null)
				cbox.ToolTip = renderingDocument.Attributes["description"].Value.FromXmlValue2Render(server);

			ph.Controls.Add(cbox);

			//no validators, yay!!

			return ph;
		}


		public string GetValue(System.Web.UI.Control ctrl1, XmlNode renderingDocument)
		{
			Control ctrl = ctrl1.Controls[1];
			XmlDocument doc = new XmlDocument();
			string root = String.IsNullOrEmpty(this.Name) ? "null" : this.Name;
			XmlNode nodeValue = doc.CreateElement(root);
			doc.AppendChild(nodeValue);
			nodeValue.AppendChild(doc.CreateElement("Value"));
			nodeValue.FirstChild.InnerText = ((CheckBoxControl)ctrl).Checked ? "true" : "false";
			return doc.OuterXml;
		}

		public void SetValue(List<XmlNode> nds)
		{
			string temp = nds[0].FirstChild.InnerText.Trim();
			text = temp.ToLower().Equals("true");
		}

		public Control GetUnrelatedControl()
		{
			return new CheckBoxControl(this);
		}

		public string Name { get; set; }

		public void setExampleValue() { text = false; }

		public static WebControl Icon
		{
			get
			{
				return new CustomImage("Fields.images.checkbox_icon.png", "Insert a checkBox with label");
			}
		}

		public static WebControl Preview
		{
			get
			{
				return new CustomImage("Fields.images.checkbox_preview.png");
			}
		}

		public static WebControl PreviewTooltip
		{
			get
			{
				return new CustomImage("Fields.images.checkbox_preview_tooltip.png");
			}
		}

		public string JSON_StyleProperties
		{
			get
			{
				return
					 @"[
					{ 
					""group"":""MainColor"",
						""properties"": [
						{
							""name"":""color"",
							""type"":""text"",
							""validator"":""color"",
							""info"": ""foreground color of field. Example: #0000ff""
						},
						{
							""name"":""background-color"",
							""type"":""text"",
							""validator"":""color"",
							""info"": ""Color of Field background. Example: #ff00ff""
						}
						]  
					}
				] ";
			}
		}

		#endregion

		#region predicates

		[Predicate("Is Checked", Description = "Return if is checked")]
		public static bool IsChecked(CheckBoxField c)
		{
			return c.text;
		}

		#endregion

		#region innerClass

		public class CheckBoxControl : CheckBox
		{
			CheckBoxField father;

			public CheckBoxControl(CheckBoxField field)
				: base()
			{
				father = field;
				this.ID = field.Name;
				if (father.text)
					this.Checked = true;
			}

		}
		#endregion

	}
}
