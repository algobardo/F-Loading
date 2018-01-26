using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Web.UI.WebControls;
using System.Web.UI;

[assembly: System.Web.UI.WebResource("Fields.images.radiobutton_icon.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.radiobutton_preview.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.radiobutton_preview_tooltip.png", "image/png")]
namespace Fields
{
	class RadioButtonList : IBaseType
	{
		private XmlSchema baseSchema;
		private XmlSchemaElement elemPointer;
		private string text;
		private const int MAX_ELEM_BEFORE_VERTICAL_ALIGN = 3; //great name! XD
		private const int MAX_ELEM_BEFORE_DROPDOWN = 10; //very great Name

		#region constructors

		public RadioButtonList()
		{
			baseSchema = new XmlSchema();

			XmlSchemaComplexType ctype = new XmlSchemaComplexType();
			ctype.Name = "";
			baseSchema.Items.Add(ctype);

			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();

			complexContentExtension.BaseTypeName = new XmlQualifiedName("RadioButtonList");

			ctype.ContentModel = complexContent;
			complexContent.Content = complexContentExtension;

			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexContentExtension.Particle = seq;

			XmlSchemaElement elem = new XmlSchemaElement();
			elem.Name = "Value";
			seq.Items.Add(elem);

			XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
			XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
			restriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

			elem.SchemaType = simpleType;
			simpleType.Content = restriction;
			//for direct access
			elemPointer = elem;
		}

		public RadioButtonList(XmlSchema schema, string name)
		{
			this.Name = name;

			baseSchema = new XmlSchema();

			XmlSchemaComplexType ctype = new XmlSchemaComplexType();
			ctype.Name = ((XmlSchemaComplexType)schema.Items[0]).Name;
			baseSchema.Items.Add(ctype);

			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();

			complexContentExtension.BaseTypeName = new XmlQualifiedName("RadioButtonList");

			ctype.ContentModel = complexContent;
			complexContent.Content = complexContentExtension;

			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexContentExtension.Particle = seq;

			XmlSchemaElement elem = new XmlSchemaElement();
			elem.Name = "Value";
			seq.Items.Add(elem);

			if (Common.getElementFromSchema(schema).IsNillable)
				elem.IsNillable = true;

			XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
			XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
			restriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

			elem.SchemaType = simpleType;
			simpleType.Content = restriction;
			//for direct access
			elemPointer = elem;

			//addning enumerations
			XmlSchemaElement schemaElem = ((XmlSchemaElement)((XmlSchemaSequence)((XmlSchemaComplexContentExtension)((XmlSchemaComplexContent)((XmlSchemaComplexType)schema.Items[0]).ContentModel).Content).Particle).Items[0]);
			XmlSchemaSimpleTypeRestriction schemaRestriction = ((XmlSchemaSimpleTypeRestriction)((XmlSchemaSimpleType)schemaElem.SchemaType).Content);
			foreach (XmlSchemaFacet i in schemaRestriction.Facets)
			{
				XmlSchemaEnumerationFacet enumeration = new XmlSchemaEnumerationFacet();
				enumeration.Value = i.Value;
				restriction.Facets.Add(enumeration);
			}
		}

		#endregion

		#region Constraints
		/// <summary>
		/// Sets radiobutton options. A new call to this method will replace previous options.
		/// </summary>
		/// <param name="str">A list of options separated with commas</param>
		/// <returns>true if options are setted, false otherwise</returns>
		[Constraint("Set Options", Description = "Sets options splitting %0 usind ',' as delimiter")]
		public bool setOptions(string str)
		{
			if (elemPointer == null) return false;
			string[] options = str.Split(',');
			if (options.Length <= 0) return false;

			XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
			XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
			restriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

			elemPointer.SchemaType = simpleType;
			simpleType.Content = restriction;
			foreach (string option in options)
			{
				XmlSchemaEnumerationFacet enumeration = new XmlSchemaEnumerationFacet();
				enumeration.Value = option;
				restriction.Facets.Add(enumeration);
			}
			return true;
		}

		[Constraint("Add Option", Description = "Add option to xml schema")]
		public bool addOption(string str)
		{
			if (elemPointer == null) return false;
			if (str.Length <= 0) return false;

			if (elemPointer.SchemaType == null)
			{
				XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
				XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
				restriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

				elemPointer.SchemaType = simpleType;
				simpleType.Content = restriction;
				XmlSchemaEnumerationFacet enumeration = new XmlSchemaEnumerationFacet();
				enumeration.Value = str;
				restriction.Facets.Add(enumeration);
			}
			else
			{
				XmlSchemaEnumerationFacet enumeration = new XmlSchemaEnumerationFacet();
				enumeration.Value = str.FromNotEncodedRender2XmlValue();
				((XmlSchemaSimpleTypeRestriction)((XmlSchemaSimpleType)elemPointer.SchemaType).Content).Facets.Add(enumeration);
			}

			return true;
		}
        /*
		[Constraint("Field value may not be present")]
		public bool AddIsNotRequiredConstraint()
		{
			if (!Common.getElementFromSchema(baseSchema).IsNillable)
			{
				 Common.getElementFromSchema(baseSchema).IsNillable = true;
				 return true;
			}
			else

				 return false;
			
		}
       */
		#endregion

#region Predicates
        [Predicate ("Is Selected")]
        public static bool IsSelected(RadioButtonList radio,StringBox str)
        {
            return radio.text == str.text;
        }
#endregion
        #region IField Members

        public string TypeName
		{
			get
			{
				return ((XmlSchemaComplexType)baseSchema.Items[0]).Name;
			}
			set
			{
				((XmlSchemaComplexType)baseSchema.Items[0]).Name = value; ;
			}
		}

		public System.Xml.Schema.XmlSchemaComplexType TypeSchema
		{
			get { return (XmlSchemaComplexType)baseSchema.Items[0]; }
		}


		public void SetValue(List<XmlNode> nds)
		{
			this.text = nds[0].FirstChild.InnerText.Trim();
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

			bool isRadio = ((XmlSchemaSimpleTypeRestriction)((XmlSchemaSimpleType)elemPointer.SchemaType).Content).Facets.Count <= MAX_ELEM_BEFORE_DROPDOWN;
			ListControl radio;
			if (isRadio)
				radio = new RadioButtonControl(this);
			else
				radio = new DropDownListControl(this);

			foreach (ListItem i in radio.Items)
				i.Text = i.Text.FromXmlValue2Render(server);

			if (isRadio && radio.Items.Count > 0)
				radio.Items[0].Selected = true;

			//--Rendering Document
			XmlAttribute clas = renderingDocument.Attributes["class"];
			XmlAttribute rel = renderingDocument.Attributes["rel"];
			if (clas != null && rel != null)
			{
				foreach (ListItem el in isRadio ? ((RadioButtonControl)radio).Items : ((DropDownListControl)radio).Items)
				{
					el.Attributes.Add("class", clas.Value.FromXmlValue2Render(server));
					//el.Attributes.Add("rel", rel.Value.FromXmlValue2Render(server));
				}
			}
			if (renderingDocument.Attributes["description"] != null)
				radio.ToolTip = renderingDocument.Attributes["description"].Value.FromXmlValue2Render(server);
			//--
			if (isRadio)
			{
				if (radio.Items.Count > MAX_ELEM_BEFORE_VERTICAL_ALIGN)
				{
					((RadioButtonControl)radio).RepeatDirection = RepeatDirection.Vertical;
					// ((RadioButtonControl)radio).RepeatColumns = 5;
				}
				else
					((RadioButtonControl)radio).RepeatDirection = RepeatDirection.Horizontal;
			}

           /* if (!Common.getElementFromSchema(baseSchema).IsNillable)
            {
                RequiredFieldValidator rqfv = new RequiredFieldValidator();
                rqfv.ErrorMessage = "Required field: select an option";
                rqfv.ControlToValidate = radio.ID;
                rqfv.ValidationGroup = "1";
                ph.Controls.Add(rqfv);
            }*/
			ph.Controls.Add(radio);
			return ph;
		}

		public string GetValue(Control ctrl, XmlNode renderingDocument)
		{
			ListControl list = (ListControl)ctrl.Controls[1];
			//this.text = rb.SelectedValue;

			XmlDocument doc = new XmlDocument();
			string root = String.IsNullOrEmpty(this.Name) ? "null" : this.Name;
			XmlNode nodeValue = doc.CreateElement(root);
			doc.AppendChild(nodeValue);
			nodeValue.AppendChild(doc.CreateElement("Value"));

			nodeValue.FirstChild.InnerText = list.SelectedValue.FromNotEncodedRender2XmlValue();
			return doc.OuterXml;
		}

		[Obsolete]
		public System.Xml.XmlNode Value
		{
			get
			{
				string root = String.IsNullOrEmpty(Name) ? "null" : Name;

				XmlDocument doc = new XmlDocument();
				XmlNode nodeValue = doc.CreateElement(root);
				nodeValue.AppendChild(doc.CreateElement("Value"));
				nodeValue.FirstChild.InnerText = text == null ? "" : this.text;
				return nodeValue;
			}
			set
			{
				this.text = value.FirstChild.InnerText.Trim();
			}
		}

		public string Name { get; set; }

		public void setExampleValue()
		{
			//fixme: check if it's right
			text = "";
		}

		public static WebControl Icon
		{
			get
			{
				return new CustomImage("Fields.images.radiobutton_icon.png", "Insert a list of radioButton");
			}
		}

		public static WebControl Preview
		{
			get
			{
				return new CustomImage("Fields.images.radiobutton_preview.png");
			}
		}

		public static WebControl PreviewTooltip
		{
			get
			{
				return new CustomImage("Fields.images.radiobutton_preview_tooltip.png");
			}
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
					},
					{
					""group"":""Font"",
							""properties"": 
						[
                     {
								""name"":""font-family"",
								""type"":""text"",
								""validator"":""font"",
								""info"": ""Font face of the text.""
							},
							{
								""name"":""font-size"",
								""type"":""text"",
								""validator"":""size"",
								""info"": ""Font face of the text.""
							},
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

        #region PreviewControl
        public Control GetUnrelatedControl()
        {
            return new RadioButtonControl(this);
        }

        #endregion 

		#region InnerControl

		private class RadioButtonControl : System.Web.UI.WebControls.RadioButtonList
		{
			RadioButtonList father;

			public RadioButtonControl(RadioButtonList field)
				: base()
			{
				father = field;

				this.ID = father.Name == null ? "_radioButtonList" : father.Name;

				XmlSchemaObjectCollection facets = ((XmlSchemaSimpleTypeRestriction)((XmlSchemaSimpleType)father.elemPointer.SchemaType).Content).Facets;
				//((XmlSchemaSimpleTypeRestriction)((XmlSchemaSimpleType )father.elemPointer.ElementType).Content).Facets;
				foreach (XmlSchemaFacet v in facets)
				{
					this.Items.Add(v.Value);	// must be decode before render it
				}

				// for rollback
				try
				{
					this.SelectedValue = father.text;
				}
				catch (ArgumentOutOfRangeException)
				{
					// invalid value
					Console.WriteLine("RadioButtonList: Invalid selected value");
				}
			}

		}


		private class DropDownListControl : System.Web.UI.WebControls.DropDownList
		{
			RadioButtonList father;

			public DropDownListControl(RadioButtonList field)
				: base()
			{
				father = field;

				this.ID = father.Name == null ? "_radioButtonList" : father.Name;

				XmlSchemaObjectCollection facets = ((XmlSchemaSimpleTypeRestriction)((XmlSchemaSimpleType)father.elemPointer.SchemaType).Content).Facets;
				//((XmlSchemaSimpleTypeRestriction)((XmlSchemaSimpleType )father.elemPointer.ElementType).Content).Facets;
				foreach (XmlSchemaFacet v in facets)
				{
					this.Items.Add(v.Value);	// must be decode before render it
				}

				// for rollback
				try
				{
					this.SelectedValue = father.text;
				}
				catch (ArgumentOutOfRangeException)
				{
					// invalid value
					Console.WriteLine("DropDownList: Invalid selected value");
				}
			}

		}

		#endregion

	}
}
