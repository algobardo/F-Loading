using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using System.Web.UI;

[assembly: System.Web.UI.WebResource("Fields.Resources.ColorPicker.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Fields.Resources.colorpicker_crosshairs.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.Resources.colorpicker_h.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.Resources.colorpicker_position.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.Resources.colorpicker_sv.png", "image/png")]

[assembly: System.Web.UI.WebResource("Fields.images.colorpicker_icon.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.colorpicker_preview.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.colorpicker_preview_tooltip.png", "image/png")]
namespace Fields
{
	public class ColorPicker : IBaseType
	{

		private XmlSchema baseSchema;
		private string text;

		/// <summary>
		/// Initializes a new instance of the <see cref="ColorPicker"/> class.
		/// </summary>
		public ColorPicker()
		{
			baseSchema = new XmlSchema();

			//<xs:complexType name="ColorPickerN">
			XmlSchemaComplexType newType = new XmlSchemaComplexType();
			// the name must be filled later
			newType.Name = "";
			baseSchema.Items.Add(newType);

			//<xs:complexContent>
			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			newType.ContentModel = complexContent;

			//<xs:extension base="ColorPicker">
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
			complexContent.Content = complexContentExtension;
			complexContentExtension.BaseTypeName = new XmlQualifiedName("ColorPicker");

			//<xs:sequence>
			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexContentExtension.Particle = seq;

			//<xs:element name="Value" type="xs:string"/>
			XmlSchemaElement elem = new XmlSchemaElement();
			seq.Items.Add(elem);
			elem.Name = "Value";
			elem.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ColorPicker"/> class from a XMLSchema
		/// </summary>
		/// <param name="schema">The schema xml that describe the ColorPicker state.</param>
		public ColorPicker(XmlSchema schema, string name)
		{
			// in this constructor it's useful to set the ID, cause GUI is calling it
			this.Name = name;

			baseSchema = new XmlSchema();

			//<xs:complexType name="ColorPickerN">
			XmlSchemaComplexType newType = new XmlSchemaComplexType();
			newType.Name = ((XmlSchemaComplexType)schema.Items[0]).Name;
			baseSchema.Items.Add(newType);

			//<xs:complexContent>
			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			newType.ContentModel = complexContent;

			//<xs:extension base="ColorPicker">
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
			complexContent.Content = complexContentExtension;
			complexContentExtension.BaseTypeName = new XmlQualifiedName("ColorPicker");

			//<xs:sequence>
			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexContentExtension.Particle = seq;

			//<xs:element name="Value" type="xs:string"/>
			XmlSchemaElement elem = new XmlSchemaElement();
			seq.Items.Add(elem);
			elem.Name = "Value";
			elem.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

			if (Common.getElementFromSchema(schema).IsNillable)
				elem.IsNillable = true;

			if (((XmlSchemaSimpleType)Common.getElementFromSchema(schema).SchemaType) == null)
				return;

			XmlSchemaObjectCollection constrColl =
					((XmlSchemaSimpleTypeRestriction)
							((XmlSchemaSimpleType)
							Common.getElementFromSchema(schema)
				  .SchemaType).Content).Facets;

			foreach (XmlSchemaFacet facet in constrColl)
			{
				Common.addFacet(facet, Common.getElementFromSchema(baseSchema));
			}

		}

		#region IField Members

		public string TypeName
		{
			set
			{
				//<xs:complexType name="ColorPickerN">
				((XmlSchemaComplexType)baseSchema.Items[0]).Name = value;
			}
			get
			{
				return ((XmlSchemaComplexType)baseSchema.Items[0]).Name;
			}
		}

		public System.Xml.Schema.XmlSchemaComplexType TypeSchema
		{
			get { return (XmlSchemaComplexType)baseSchema.Items[0]; }
		}

		public Control GetWebControl(System.Web.HttpServerUtility server, XmlNode renderingDocument)
		{
			PlaceHolder ph = new PlaceHolder();

			Label lbl = new Label();
			if (renderingDocument.Attributes["renderedLabel"] != null)
				lbl.Text = renderingDocument.Attributes["renderedLabel"].Value.FromXmlValue2Render(server);
			else
				lbl.Text = this.Name.FromXmlName2Render(server);
			lbl.CssClass = "label";
			ph.Controls.Add(lbl);

			ColorPickerControl colp = new ColorPickerControl(this);
			colp.CausesValidation = false;

			if (renderingDocument.Attributes["class"] != null)
				colp.CssClass = renderingDocument.Attributes["class"].Value.FromXmlValue2Render(server);
			if (renderingDocument.Attributes["rel"] != null)
				colp.Attributes.Add("rel", renderingDocument.Attributes["rel"].Value.FromXmlValue2Render(server));
			if (renderingDocument.Attributes["description"] != null)
				colp.ToolTip = renderingDocument.Attributes["description"].Value.FromXmlValue2Render(server);
            colp.Style.Add("z-index", "1800");
			ph.Controls.Add(colp);

			/*** Validators ***/

			// Base validator
			RegularExpressionValidator rev = new RegularExpressionValidator();
			rev.ControlToValidate = colp.ID;
			rev.Display = ValidatorDisplay.Dynamic;
			rev.ErrorMessage = "Color code not in the required format";
			rev.ValidationExpression = @"^\#([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$"; // if empty the validator is not checked, that's fine
			rev.ValidationGroup = "1";
			ph.Controls.Add(rev);

			// Required validator
			if (!Common.getElementFromSchema(baseSchema).IsNillable)
			{
				RequiredFieldValidator rqfv = new RequiredFieldValidator();
				rqfv.ErrorMessage = "Required fields shouldn't be empty";
				rqfv.ControlToValidate = colp.ID;
				rqfv.ValidationGroup = "1";
				rqfv.Display = ValidatorDisplay.Dynamic;
				ph.Controls.Add(rqfv);
			}

			if (((XmlSchemaSimpleType)Common.getElementFromSchema(baseSchema).SchemaType) == null)
				return ph;

			XmlSchemaObjectCollection constrColl =
			((XmlSchemaSimpleTypeRestriction)
				 (
					  (XmlSchemaSimpleType)
					  Common.getElementFromSchema(baseSchema).SchemaType
				 ).Content
			).Facets;

			foreach (XmlSchemaFacet facet in constrColl)
			{
				//TODO: No validators by now
			}

			return ph;
		}

		public string GetValue(Control ctrl1, XmlNode renderingDocument)
		{
			Control ctrl = ctrl1.Controls[1];

			XmlDocument doc = new XmlDocument();
			string root = String.IsNullOrEmpty(this.Name) ? "null" : this.Name;
			XmlNode nodeValue = doc.CreateElement(root);
			XmlElement elem = doc.CreateElement("Value");
			doc.AppendChild(nodeValue);
			nodeValue.AppendChild(elem);			// Encoding examples (when needed):
			//((ColorPickerControl)ctrl).Text.FromNotEncodedRender2XmlName();
			//XmlConvert.EncodeLocalName((((ColorPickerControl)ctrl).Text))
			nodeValue.FirstChild.InnerText = ((ColorPickerControl)ctrl).Text == null ? "" : ((ColorPickerControl)ctrl).Text; ;
			if (nodeValue.FirstChild.InnerText == "" && Common.getElementFromSchema(baseSchema).IsNillable)
			{
				elem.SetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			}
			return doc.OuterXml;
		}

		public void SetValue(List<XmlNode> nds)
		{
			this.text = nds[0].FirstChild.InnerText.Trim();
		}

		public Control GetUnrelatedControl()
		{
			return new ColorPickerControl(this);
		}

		public string Name { set; get; }

		public static WebControl Icon
		{
			get
			{
				return new CustomImage("Fields.images.colorpicker_icon.png", "Asks for a color");
			}
		}

		public static WebControl Preview
		{
			get
			{
				return new CustomImage("Fields.images.colorpicker_preview.png");
			}
		}

		public static WebControl PreviewTooltip
		{
			get
			{
				return new CustomImage("Fields.images.colorpicker_preview_tooltip.png");
			}
		}

		public void setExampleValue()
		{
			text = "#850322";
		}

		public string JSON_StyleProperties
		{
			get
			{
				return
				@"[
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
					},
					{ 
					""group"":""background"",
						""properties"": [
						{
							""name"":""background-color"",
							""type"":""text"",
							""validator"":""color"",
							""info"": ""Color of Field background. Example: #ff00ff""
						}
						]  
					},
					{ 
					""group"":""Height"",
						""properties"": [
						{
							""name"":""height"",
							""type"":""text"",
							""validator"":""size"",
							""info"": ""Height of inputbox.""
						}
						]  
					},
					{ 
					""group"":""Width"",
						""properties"": [
						{
							""name"":""width"",
							""type"":""text"",
							""validator"":""size"",
							""info"": ""Width of inputbox.""
						}
						]  
					},
					{ 
					""group"":""Text-Decoration"",
						""properties"": [
						{
							""name"":""text-decoration"",
							""type"":""text"",
							""validator"":""none"",
							""info"": ""Set a decoration of text. Acceptable value: none, underline, overline, line-through or blink""
						}
						]  
					}
				] ";
			}
		}

		#endregion

		#region Constraints

		[Constraint("Field Not Required", Description = "Field value may not be present")]
		public bool IsNotRequiredConstraint()
		{
			if (!Common.getElementFromSchema(baseSchema).IsNillable)
			{
				Common.getElementFromSchema(baseSchema).IsNillable = true;
				return true;
			}
			else
				return false;
		}

		#endregion


		/// <summary>
		/// ColorPicker WebControl
		/// </summary>
		public class ColorPickerControl : TextBox
		{
			ColorPicker father;

			public ColorPickerControl(ColorPicker field)
				: base()
			{
				father = field;
				this.ID = String.IsNullOrEmpty(father.Name) ? "_ColorPicker" : father.Name;
				this.MaxLength = 7;

				// for rollback
				if (father.text != null) this.Text = father.text;
			}

			protected override void OnPreRender(EventArgs e)
			{
				base.OnPreRender(e);

				string crosshairsImageUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "Fields.Resources.colorpicker_crosshairs.png");
				string hImageUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "Fields.Resources.colorpicker_h.png");
				string positionImageUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "Fields.Resources.colorpicker_position.png");
				string svImageUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "Fields.Resources.colorpicker_sv.png");

				string src =
					 @"<script type='text/javascript'>
		                var COLORPICKER_CROSSHAIRS_LOCATION = '" + crosshairsImageUrl + @"';
		                var COLORPICKER_HUE_SLIDER_LOCATION = '" + hImageUrl + @"';
		                var COLORPICKER_HUE_SLIDER_ARROWS_LOCATION = '" + positionImageUrl + @"';
		                var COLORPICKER_SAT_VAL_SQUARE_LOCATION = '" + svImageUrl + @"';
                    </script>"
				;
				string resourceName = "Fields.Resources.ColorPicker.js";

				ClientScriptManager cs = this.Page.ClientScript;
				cs.RegisterClientScriptBlock(this.GetType(), "ColorPickerSettings", src);
				cs.RegisterClientScriptResource(this.GetType(), resourceName);
			}

			protected override void Render(HtmlTextWriter writer)
			{
				this.Attributes.Add("onfocus", "COLORPICKER_start(this);");
				this.Attributes.Add("onblur", "COLORPICKER_hide();");

				base.Render(writer);
			}
		}
	}

}
