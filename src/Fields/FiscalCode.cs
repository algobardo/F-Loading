using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Web.UI.WebControls;
using System.Web.UI;

[assembly: System.Web.UI.WebResource("Fields.images.fiscalcode_icon.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.fiscalcode_preview.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.fiscalcode_preview_tooltip.png", "image/png")]
namespace Fields
{
	public class FiscalCode : IBaseType
	{
		protected XmlSchema baseSchema;
		protected string text;

		#region constructor

		public FiscalCode()
		{
			baseSchema = new XmlSchema();

			//<xs:complexType name="FiscalCodeN">
			XmlSchemaComplexType newType = new XmlSchemaComplexType();
			// the name must be filled later
			newType.Name = "";
			baseSchema.Items.Add(newType);

			//<xs:complexContent>
			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			newType.ContentModel = complexContent;

			//<xs:extension base="FiscalCode">
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
			complexContent.Content = complexContentExtension;
			complexContentExtension.BaseTypeName = new XmlQualifiedName("FiscalCode");

			//<xs:sequence>
			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexContentExtension.Particle = seq;

			//<xs:element name="Value" type="xs:string"/>
			XmlSchemaElement elem = new XmlSchemaElement();
			seq.Items.Add(elem);
			elem.Name = "Value";
			elem.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
			this.AddPatternConstraint(@"[a-zA-Z]{6}\d\d[a-zA-Z]\d\d[a-zA-Z]\d\d\d[a-zA-Z]");
		}

		public FiscalCode(XmlSchema schema, string name)
		{
			// in this constructor it's useful to set the ID, cause GUI is calling it
			this.Name = name;

			baseSchema = new XmlSchema();

			//<xs:complexType name="FiscalCodeN">
			XmlSchemaComplexType newType = new XmlSchemaComplexType();
			newType.Name = ((XmlSchemaComplexType)schema.Items[0]).Name;
			baseSchema.Items.Add(newType);

			//<xs:complexContent>
			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			newType.ContentModel = complexContent;

			//<xs:extension base="FiscalCode">
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
			complexContent.Content = complexContentExtension;
			complexContentExtension.BaseTypeName = new XmlQualifiedName("FiscalCode");

			//<xs:sequence>
			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexContentExtension.Particle = seq;

			//<xs:element name="Value" type="xs:string"/>
			XmlSchemaElement elem = new XmlSchemaElement();
			seq.Items.Add(elem);
			elem.Name = "Value";
			elem.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

			//check empty element value
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


		public static WebControl Icon
		{
			get
			{
				return new CustomImage("Fields.images.fiscalcode_icon.png", "Require a valid fiscal code");
			}
		}

		public static WebControl Preview
		{
			get
			{
				return new CustomImage("Fields.images.fiscalcode_preview.png");
			}
		}

		public static WebControl PreviewTooltip
		{
			get { return null; }
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

			FiscalCodeControl sbox = new FiscalCodeControl(this);
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
				rqfv.ValidationGroup = "1";
				rqfv.Display = ValidatorDisplay.Dynamic;
				ph.Controls.Add(rqfv);
			}

			if (((XmlSchemaSimpleType)Common.getElementFromSchema(baseSchema).SchemaType) == null)
				return ph;

			XmlSchemaObjectCollection constrColl =
			  ((XmlSchemaSimpleTypeRestriction)((XmlSchemaSimpleType)Common.getElementFromSchema(baseSchema).SchemaType).Content).Facets;

			// Set validators from XSD
			foreach (XmlSchemaFacet facet in constrColl)
			{
				if (facet is XmlSchemaPatternFacet)
					ph.Controls.Add(getPatternValidator(facet.Value, sbox.ID));
			}

			//--- 

			return ph;
		}

		public string GetValue(Control ctrl1, XmlNode renderingDocument)
		{
			FiscalCodeControl ctrl = (FiscalCodeControl)ctrl1.Controls[1];
			XmlDocument doc = new XmlDocument();
			string root = String.IsNullOrEmpty(this.Name) ? "null" : this.Name;
			XmlNode nodeValue = doc.CreateElement(root);
			XmlElement elem = doc.CreateElement("Value");
			doc.AppendChild(nodeValue);
			nodeValue.AppendChild(elem);
			nodeValue.FirstChild.InnerText = ctrl.Text.FromNotEncodedRender2XmlValue();
			if (nodeValue.FirstChild.InnerText == "" && Common.getElementFromSchema(baseSchema).IsNillable)
			{
				elem.SetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			}
			return doc.OuterXml;
		}

		public void SetValue(List<XmlNode> nds)
		{
			this.text = nds[0].FirstChild.InnerText.ToUpper();
		}

		public string Name
		{
			get;
			set;
		}

		public void setExampleValue()
		{
			text = "cstntn77t22l419b";
		}

		public string JSON_StyleProperties
		{
			get
			{
				return new StringBox().JSON_StyleProperties;
			}
		}

		public Control GetUnrelatedControl()
		{
			return new FiscalCodeControl(this);
		}

		#endregion


		#region Predicates

		[Predicate("Equals", Description = "Lexical Comparation between two FiscalCode")]
		public static bool Equals(FiscalCode sBox1, FiscalCode sBox2)
		{
			return sBox1.text.Equals(sBox2.text);
		}

		[Predicate("IsEmpty", Description = "Checks whether user has put content inside the field")]
		public static bool IsEmpty(FiscalCode s)
		{
			return String.IsNullOrEmpty(s.text);
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
			rev.Display = ValidatorDisplay.Dynamic;
			rev.ValidationExpression = pattern;
            rev.ValidationGroup = "1";
			rev.ErrorMessage = "This field must be a valid FiscalCode (e.g. FLDPJC09H29G702R)";
			return rev;
		}

		#endregion

		#region WebControl

		/// <summary>
		/// FiscalCode WebControl...................
		/// </summary>
		public class FiscalCodeControl : TextBox
		{
			FiscalCode father;

			public FiscalCodeControl(FiscalCode field)
				: base()
			{
				father = field;
				this.ID = String.IsNullOrEmpty(father.Name) ? "_fiscalCode" : father.Name;
				this.MaxLength = 16;

				// for rollback..
				if (father.text != null) 
					this.Text = father.text.ToUpper();
			}

		}

		#endregion

	}
}
