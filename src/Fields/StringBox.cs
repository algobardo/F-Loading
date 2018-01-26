using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Web.UI.WebControls;
using System.Web.UI;

[assembly: System.Web.UI.WebResource("Fields.images.stringbox_icon.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.stringbox_preview.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.stringbox_preview_tooltip.png", "image/png")]
namespace Fields
{
	[Property("HiddenText", Description = "Password-like field", Attribute = "hiddentext", Default = "False")]
	[Property("Multiline", Description = "Number of rows", Attribute = "rows", Default = "1")]
	public class StringBox : IBaseType
	{
		protected XmlSchema baseSchema;
		internal string text;

		#region constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="StringBox"/> class.
		/// </summary>
		public StringBox()
		{
			baseSchema = new XmlSchema();

			//<xs:complexType name="StringBoxN">
			XmlSchemaComplexType newType = new XmlSchemaComplexType();
			// the name must be filled later
			newType.Name = "";
			baseSchema.Items.Add(newType);

			//<xs:complexContent>
			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			newType.ContentModel = complexContent;

			//<xs:extension base="StringBox">
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
			complexContent.Content = complexContentExtension;
			complexContentExtension.BaseTypeName = new XmlQualifiedName("StringBox");

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
		/// Initializes a new instance of the <see cref="StringBox"/> class from a XMLSchema
		/// </summary>
		/// <param name="schema">The schema xml that describe the StringBox state.</param>
		public StringBox(XmlSchema schema, string name)
		{
			// in this constructor it's useful to set the ID, cause GUI is calling it
			this.Name = name;

			baseSchema = new XmlSchema();

			//<xs:complexType name="StringBoxN">
			XmlSchemaComplexType newType = new XmlSchemaComplexType();
			newType.Name = ((XmlSchemaComplexType)schema.Items[0]).Name;
			baseSchema.Items.Add(newType);

			//<xs:complexContent>
			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			newType.ContentModel = complexContent;

			//<xs:extension base="StringBox">
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
			complexContent.Content = complexContentExtension;
			complexContentExtension.BaseTypeName = new XmlQualifiedName("StringBox");

			//<xs:sequence>
			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexContentExtension.Particle = seq;

			//<xs:element name="Value" type="xs:string"/>
			XmlSchemaElement elem = new XmlSchemaElement();
			seq.Items.Add(elem);
			elem.Name = "Value";
			elem.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

			//Controlla se l'element potra' avere valore vuoto
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

		#endregion

		#region IField Members

		public string TypeName
		{
			set
			{
				//<xs:complexType name="StringBoxN">
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

			StringBoxControl sbox = new StringBoxControl(this);
			//sbox.CausesValidation = false;

			if (renderingDocument.Attributes["class"] != null)
				sbox.CssClass = renderingDocument.Attributes["class"].Value.FromXmlValue2Render(server);
			if (renderingDocument.Attributes["rel"] != null)
				sbox.Attributes.Add("rel", renderingDocument.Attributes["rel"].Value.FromXmlValue2Render(server));
			if (renderingDocument.Attributes["description"] != null)
				sbox.ToolTip = renderingDocument.Attributes["description"].Value.FromXmlValue2Render(server);

			// Setting properties
			sbox.TextMode = TextBoxMode.SingleLine;
			if (renderingDocument.Attributes["rows"] != null)
			{
				string ns = renderingDocument.Attributes["rows"].Value.FromXmlValue2Render(server);
				int n;
				if (int.TryParse(ns, out n) && n > 1)
				{
					sbox.TextMode = TextBoxMode.MultiLine;
					sbox.Rows = n;
				}
			}
			if (renderingDocument.Attributes["hiddentext"] != null)
				if (renderingDocument.Attributes["hiddentext"].Value.FromXmlValue2Render(server).ToLower().Equals("true"))
					sbox.TextMode = TextBoxMode.Password;


			ph.Controls.Add(sbox);

			//--- ADD validators ---

			if (!Common.getElementFromSchema(baseSchema).IsNillable)
			{
				RequiredFieldValidator rqfv = new RequiredFieldValidator();
				rqfv.ErrorMessage = "Required fields shouldn't be empty";
				rqfv.ControlToValidate = sbox.ID;
				rqfv.Display = ValidatorDisplay.Dynamic;
				rqfv.ValidationGroup = "1";
				ph.Controls.Add(rqfv);
			}

			if (((XmlSchemaSimpleType)Common.getElementFromSchema(baseSchema).SchemaType) == null)
				return ph;

			XmlSchemaObjectCollection constrColl =
			  ((XmlSchemaSimpleTypeRestriction)((XmlSchemaSimpleType)Common.getElementFromSchema(baseSchema).SchemaType).Content).Facets;
			// Set validators from XSD
			foreach (XmlSchemaFacet facet in constrColl)
			{
				if (facet is XmlSchemaMaxLengthFacet)
					ph.Controls.Add(getMaxLengthValidator(Int32.Parse(facet.Value), sbox.ID));
				else if (facet is XmlSchemaMinLengthFacet)
					ph.Controls.Add(getMinLengthValidator(Int32.Parse(facet.Value), sbox.ID));
				else if (facet is XmlSchemaPatternFacet)
					ph.Controls.Add(getPatternValidator(facet.Value, sbox.ID));
			}

			//--- 

			return ph;
		}

		public string GetValue(Control ctrl1, XmlNode renderingDocument)
		{
			StringBoxControl ctrl = (StringBoxControl)ctrl1.Controls[1];
			this.text = ctrl.Text;
			XmlDocument doc = new XmlDocument();
			string root = String.IsNullOrEmpty(this.Name) ? "null" : this.Name;
			XmlNode nodeValue = doc.CreateElement(root);
			XmlElement elem = doc.CreateElement("Value");
			doc.AppendChild(nodeValue);
			nodeValue.AppendChild(elem);
			nodeValue.FirstChild.InnerText = ctrl.Text == null ? "" : ctrl.Text.FromNotEncodedRender2XmlValue();
			if (text == "" && Common.getElementFromSchema(baseSchema).IsNillable)
			{
				elem.SetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			}
			return doc.OuterXml;
		}

		public void SetValue(List<XmlNode> nds)
		{
			this.text = nds[0].FirstChild.InnerText;
		}

		public string Name
		{
			get;
			set;
		}

		public static WebControl Icon
		{
			get
			{
				return new CustomImage("Fields.images.stringbox_icon.png", "Require a string");
			}
		}

		public static WebControl Preview
		{
			get
			{
				return new CustomImage("Fields.images.stringbox_preview.png");
			}
		}

		public static WebControl PreviewTooltip
		{
			get
			{
				return new CustomImage("Fields.images.stringbox_preview_tooltip.png");
			}
		}

		public void setExampleValue()
		{
			text = "example";
		}

		public Control GetUnrelatedControl()
		{
			return new StringBoxControl(this);
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


		#region Predicates

		[Predicate("Equals", Description = "Lexical Comparation between two StringBox")]
		public static bool Equals(StringBox sBox1, StringBox sBox2)
		{
			return sBox1.text.Equals(sBox2.text);
		}

		[Predicate("contains", Description = "Check if first StringBox parameter contains specified text")]
		public static bool Contains(StringBox sBox, StringBox parBox)
		{
			return sBox.text.Contains(parBox.text);
		}

		[Predicate("starts With", Description = "Check if first StringBox parameter starts with the specified text")]
		public static bool StartsWith(StringBox sBox, StringBox startBox)
		{
			return sBox.text.StartsWith(startBox.text);
		}

		[Predicate("ends with", Description = "Check if first StringBox parameter ends with the specified text")]
		public static bool EndsWith(StringBox sBox, StringBox endBox)
		{
			return sBox.text.EndsWith(endBox.text);
		}

		[Predicate("is Empty", Description = "Checks whether user has put content inside the field")]
		public static bool IsEmpty(StringBox s)
		{
			return String.IsNullOrEmpty(s.text);
		}

		[Predicate("RegExp match", Description = "Return if the first StringBox value match with regular expression in second parameter")]
		public static bool RegexMatch(StringBox s1, StringBox pattern)
		{
			return System.Text.RegularExpressions.Regex.IsMatch(s1.text, pattern.text);
		}

		#endregion

		#region Operations
		/**
        * Notes :
        * All Operations results are new instances of the return types
        */

		[Operation("Lower Case")]
		public static StringBox ToLowerCase(StringBox source)
		{
			StringBox result = new StringBox();
			result.text = source.text.ToLower();
			return result;
		}

		[Operation("Upper Case")]
		public static StringBox ToUpperCase(StringBox source)
		{
			StringBox result = new StringBox();
			result.text = source.text.ToUpper();
			return result;
		}


		[Operation("Length", Description = "Length of this string...")]
		public static IntBox Length(StringBox source)
		{
			IntBox result = new IntBox();
			XmlDocument doc = new XmlDocument();

			XmlNode nodeValue = doc.CreateElement("null");
			nodeValue.AppendChild(doc.CreateElement("Value"));
			nodeValue.FirstChild.InnerText = source.text.Length.ToString();
			List<XmlNode> listres = new List<XmlNode>();
			listres.Add(nodeValue);
			result.SetValue(listres);

			return result;
		}


		[Operation("String Concatenation")]
		public static StringBox Concat(StringBox source1, StringBox source2)
		{
			StringBox result = new StringBox();
			result.text = source1.text + source2.text;
			return result;
		}

		#endregion

		#region Constraints

		[Constraint("Field not required", Description = "Field value may not be present")]
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

		[Constraint("Range Length", Description = "String length must be in the range specified")]
		public bool AddRangeLengthConstraint(string sMinLen, string sMaxLen)
		{
			int minLen, maxLen;

			if (!int.TryParse(sMinLen, out minLen) ||
					!int.TryParse(sMaxLen, out maxLen) ||
					maxLen < 0 || minLen < 0 || minLen > maxLen)	// FIXME
				return false;

			XmlSchemaMaxLengthFacet maxLenFacet = new XmlSchemaMaxLengthFacet();
			maxLenFacet.Value = maxLen.ToString();
			Common.addFacet(maxLenFacet, Common.getElementFromSchema(baseSchema));
			XmlSchemaMinLengthFacet minLenFacet = new XmlSchemaMinLengthFacet();
			minLenFacet.Value = minLen.ToString();
			Common.addFacet(minLenFacet, Common.getElementFromSchema(baseSchema));
			return true;
		}

		/// <summary>
		/// Adds a constraint which limits the maximum length of
		/// the string inside the StringBox,adding <code><xs:maxLength value=maxLen/></code>
		/// to the schema.
		/// </summary>
		/// <param name="maxLen">maximum string length, a non negative integer</param>
		/// <returns>true if the constraint is added succesfully, false otherwise.</returns>
		[Constraint("Max Length", Description = "String must not be longer than %0")]
		public bool AddMaxLengthConstraint(string max_length)
		{
			int maxLen;

			if (!int.TryParse(max_length, out maxLen) || maxLen < 0)
				return false;

			XmlSchemaMaxLengthFacet maxLenFacet = new XmlSchemaMaxLengthFacet();
			maxLenFacet.Value = max_length;

			Common.addFacet(maxLenFacet, Common.getElementFromSchema(baseSchema));

			return true;
		}

		private BaseValidator getMaxLengthValidator(int maxLen, string controlID)
		{
			RegularExpressionValidator regexpr = new RegularExpressionValidator();
			regexpr.ControlToValidate = controlID;
			regexpr.Display = ValidatorDisplay.Dynamic;
			regexpr.ValidationExpression = ".{0," + maxLen + "}";
            regexpr.ValidationGroup = "1";
			regexpr.ErrorMessage = "String longer than " + maxLen;
			return regexpr;
		}

		/// <summary>
		/// Adds a constraint which limits the minimum length of
		/// the string inside the StringBox, adding <code><xs:minLength value=minLen/></code>
		/// to the schema.
		/// </summary>
		/// <param name="maxLen">minimum string length, a non negative integer.</param>
		/// <returns>true if the constraint is added succesfully, false otherwise.</returns>
		[Constraint("Min Length", Description = "String must not be shorter than %0")]
		public bool AddMinLengthConstraint(string min_length)
		{
			int minLen;

			if (!int.TryParse(min_length, out minLen) || minLen < 0)
				return false;

			XmlSchemaMinLengthFacet minLenFacet = new XmlSchemaMinLengthFacet();
			minLenFacet.Value = minLen.ToString();
			Common.addFacet(minLenFacet, Common.getElementFromSchema(baseSchema));

			return true;
		}

		private BaseValidator getMinLengthValidator(int minLen, string controlID)
		{
			RegularExpressionValidator regexpr = new RegularExpressionValidator();
			regexpr.ControlToValidate = controlID;
			regexpr.ValidationExpression = ".{" + minLen + ",}";
			regexpr.ErrorMessage = "String shorter than " + minLen;
			regexpr.Display = ValidatorDisplay.Dynamic;
            regexpr.ValidationGroup = "1";
			return regexpr;
		}

		/// <summary>
		/// Adds a pattern restriction to the stringBox Schema.
		/// Assuming that the pattern parameter is well formed it adds 
		/// <code><xs:pattern value="pattern"/></code>
		/// to the schema.
		/// </summary>
		/// <param name="pattern">The regular expression to use as restriction.</param>
		/// <returns>true if the costraint is added succesfully, false otherwise.</returns> 
		[Constraint("Reg Exp", Description = "String must adhere to regular expression %0")]
		public bool AddPatternConstraint(String pattern)
		{
			//TODO: check the pattern correctness. is there a nice way?
			if (pattern == null || pattern.Equals(""))
				return false;

			XmlSchemaPatternFacet schemaPattern = new XmlSchemaPatternFacet();
			schemaPattern.Value = pattern;
			Common.addFacet(schemaPattern, Common.getElementFromSchema(baseSchema));

			return true;
		}



		public BaseValidator getPatternValidator(string pattern, string controlID)
		{
			RegularExpressionValidator rev = new RegularExpressionValidator();
			rev.ControlToValidate = controlID;
			rev.ValidationExpression = pattern;
			rev.Display = ValidatorDisplay.Dynamic;
			rev.ErrorMessage = "This field must be match following RegularExpression:\n " + pattern;
            rev.ValidationGroup = "1";
			return rev;
		}

		#endregion


		#region innerClass

		/// <summary>
		/// StringBox WebControl...................
		/// </summary>
		public class StringBoxControl : TextBox
		{
			StringBox father;

			public StringBoxControl(StringBox field)
				: base()
			{
				father = field;
				this.ID = String.IsNullOrEmpty(father.Name) ? "_stringBox" : father.Name;
				// for rollback..
				if (father.text != null) this.Text = father.text;
			}

		}
		#endregion

	}
}
