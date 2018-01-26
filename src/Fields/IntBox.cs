using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: System.Web.UI.WebResource("Fields.images.intbox_icon.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.intbox_preview.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.intbox_preview_tooltip.png", "image/png")]
namespace Fields
{
	public class IntBox : IBaseType
	{
		private XmlSchema baseSchema;
		private double text = double.NaN;	// double for future generalization

		#region constructors

		public IntBox()
		{
			baseSchema = new XmlSchema();

			//<xs:complexType name="IntBoxN"> first time is empty
			XmlSchemaComplexType newType = new XmlSchemaComplexType();
			newType.Name = "";
			baseSchema.Items.Add(newType);

			//<xs:complexContent>
			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			newType.ContentModel = complexContent;

			//<xs:extension base="IntBox">
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
			complexContent.Content = complexContentExtension;
			complexContentExtension.BaseTypeName = new XmlQualifiedName("IntBox");

			//<xs:sequence>
			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexContentExtension.Particle = seq;

			//<xs:element name="Value" type="xs:integer"/>
			XmlSchemaElement elem = new XmlSchemaElement();
			seq.Items.Add(elem);
			elem.Name = "Value";
			elem.SchemaTypeName = new XmlQualifiedName("integer", "http://www.w3.org/2001/XMLSchema");
		}

		public IntBox(XmlSchema schema, string name)
		{
			this.Name = name;

			baseSchema = new XmlSchema();

			//<xs:complexType name="IntBoxN">
			XmlSchemaComplexType newType = new XmlSchemaComplexType();
			newType.Name = ((XmlSchemaComplexType)schema.Items[0]).Name;
			baseSchema.Items.Add(newType);

			//<xs:complexContent>
			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			newType.ContentModel = complexContent;

			//<xs:extension base="IntBox">
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
			complexContent.Content = complexContentExtension;
			complexContentExtension.BaseTypeName = new XmlQualifiedName("IntBox");

			//<xs:sequence>
			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexContentExtension.Particle = seq;

			//<xs:element name="Value" type="xs:integer"/>
			XmlSchemaElement elem = new XmlSchemaElement();
			seq.Items.Add(elem);
			elem.Name = "Value";
			elem.SchemaTypeName = new XmlQualifiedName("integer", "http://www.w3.org/2001/XMLSchema");

			//Controlla se l'element potra' avere valore vuoto
			if (Common.getElementFromSchema(schema).IsNillable)
				elem.IsNillable = true;

			// TODO: find another way to get Items[0], it is not correct.
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

			IntBoxControl ibox = new IntBoxControl(this);
			//ibox.CausesValidation = false;

			if (renderingDocument.Attributes["class"] != null)
				ibox.CssClass = renderingDocument.Attributes["class"].Value.FromXmlValue2Render(server);
			if (renderingDocument.Attributes["rel"] != null)
				ibox.Attributes.Add("rel", renderingDocument.Attributes["rel"].Value.FromXmlValue2Render(server));
			if (renderingDocument.Attributes["description"] != null)
				ibox.ToolTip = renderingDocument.Attributes["description"].Value.FromXmlValue2Render(server);

			ph.Controls.Add(ibox);


			if (!Common.getElementFromSchema(baseSchema).IsNillable)
			{
				RequiredFieldValidator rqfv = new RequiredFieldValidator();
				rqfv.ErrorMessage = "Required fields shouldn't be empty";
				rqfv.ControlToValidate = ibox.ID;
				rqfv.ValidationGroup = "1";
				rqfv.Display = ValidatorDisplay.Dynamic;
				ph.Controls.Add(rqfv);
			}

			// Adding the regular expression validator to ensure the content to be an integer
			RegularExpressionValidator intValidator = new RegularExpressionValidator();
			intValidator.ValidationExpression = @"-?[0-9]+";
			intValidator.ControlToValidate = ibox.ID;
			intValidator.ErrorMessage = "Integer required!";
			intValidator.ValidationGroup = "1";
			intValidator.Display = ValidatorDisplay.Dynamic;
			ph.Controls.Add(intValidator);

			if (Common.getElementFromSchema(baseSchema).SchemaType == null)
				return ph;
			XmlSchemaObjectCollection constrColl =
				((XmlSchemaSimpleTypeRestriction)((XmlSchemaSimpleType)Common.getElementFromSchema(baseSchema).SchemaType).Content).Facets;

			foreach (XmlSchemaFacet facet in constrColl)
			{
				if (facet is XmlSchemaMinInclusiveFacet)
					ph.Controls.Add(getGreaterThanValidator(Int32.Parse(facet.Value), ibox.ID));
				else if (facet is XmlSchemaMaxInclusiveFacet)
					ph.Controls.Add(getLowerThanValidator(Int32.Parse(facet.Value), ibox.ID));
			}

			return ph;
		}

		public string GetValue(Control ctrl1, XmlNode renderingDocument)
		{
			IntBoxControl ibox = (IntBoxControl)ctrl1.Controls[1];
			string root = String.IsNullOrEmpty(this.Name) ? "null" : this.Name;

			XmlDocument doc = new XmlDocument();
			XmlNode nodeValue = doc.CreateElement(root);
			doc.AppendChild(nodeValue);
			XmlElement elem = doc.CreateElement("Value");
			nodeValue.AppendChild(elem);

			try
			{
				nodeValue.FirstChild.InnerText = double.IsNaN(Double.Parse(ibox.Text)) ? "" : ibox.Text.FromNotEncodedRender2XmlValue();
			}
			catch (Exception) { nodeValue.FirstChild.InnerText = ""; }
			if (nodeValue.FirstChild.InnerText == "" && Common.getElementFromSchema(baseSchema).IsNillable)
			{

				elem.SetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			}
			return doc.OuterXml;
		}


		public void SetValue(List<XmlNode> nds)
		{
			try
			{
				this.text = double.Parse(nds[0].FirstChild.InnerText);
			}
			catch (Exception) { this.text = double.NaN; }
		}

		public Control GetUnrelatedControl() 
		{
			return new IntBoxControl(this);
		}

		public string Name { set; get; }

		public void setExampleValue() { text = 0; }

		public static WebControl Icon
		{
			get
			{
				return new CustomImage("Fields.images.intbox_icon.png", "Require a number.");
			}
		}

		public static WebControl Preview
		{
			get
			{
				return new CustomImage("Fields.images.intbox_preview.png");
			}
		}

		public static WebControl PreviewTooltip
		{
			get
			{
				return new CustomImage("Fields.images.intbox_preview_tooltip.png");
			}
		}

		public string JSON_StyleProperties
		{
			get
			{
				StringBox crazy = new StringBox();
				return crazy.JSON_StyleProperties;
			}
		}

		#endregion

		#region Constraints

		[Constraint("Field not required", Description = "Field value may not be present")]
		public bool AddIsNotRequiredConstraint()
		{
			/**
			 * 
			 */
			//((XmlSchemaComplexType) baseSchema.Items[0]).ContentModel
			if (!Common.getElementFromSchema(baseSchema).IsNillable)
			{
				Common.getElementFromSchema(baseSchema).IsNillable = true;
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Adds a lower than constraint to the content
		/// </summary>
		/// <param name="maxVal"> the maximum value the content of this field can have</param>
		/// <returns>true</returns>
		[Constraint("Lower Than", Description = "Field value must be lower than the specified value (inclusive)")]
		public bool AddLowerThanConstraint(string maxVal)
		{
			int max;
			if (int.TryParse(maxVal, out max))
			{
				XmlSchemaMaxInclusiveFacet facet = new XmlSchemaMaxInclusiveFacet();
				facet.Value = max.ToString();
				Common.addFacet(facet, Common.getElementFromSchema(baseSchema));
				return true;
			}
			else
				return false;
		}


		/// <summary>
		/// Adds a greater than constraint to the content
		/// </summary>
		/// <param name="maxVal"> the maximum value the content of this field can have</param>
		/// <returns>true</returns>
		[Constraint("Grater Than", Description = "Field value must be greater than the specified value (inclusive)")]
		public bool AddGreaterThanConstraint(string minVal)
		{
			int min;
			if (int.TryParse(minVal, out  min))
			{
				XmlSchemaMinInclusiveFacet facet = new XmlSchemaMinInclusiveFacet();
				facet.Value = min.ToString();
				Common.addFacet(facet, Common.getElementFromSchema(baseSchema));
				return true;
			}
			else
				return false;
		}

		private BaseValidator getGreaterThanValidator(int minVal, string controlID)
		{
			CompareValidator compValid = new CompareValidator();
			compValid.ControlToValidate = controlID;
			compValid.Type = ValidationDataType.Integer;
			compValid.ValueToCompare = minVal.ToString();
            compValid.ValidationGroup = "1";
			compValid.Operator = ValidationCompareOperator.GreaterThanEqual;
			compValid.ErrorMessage = "The value has to be greater than or equal to " + minVal;

			return compValid;
		}

		private BaseValidator getLowerThanValidator(int maxVal, string controlID)
		{
			CompareValidator compValid = new CompareValidator();
			compValid.ControlToValidate = controlID;
			compValid.Type = ValidationDataType.Integer;
			compValid.ValueToCompare = maxVal.ToString();
            compValid.ValidationGroup = "1";
			compValid.Operator = ValidationCompareOperator.LessThanEqual;
			compValid.ErrorMessage = "The value has to be lower than or equal to " + maxVal;

			return compValid;
		}

		/// <summary>
		/// Adds an inclusive interval in which the content of the text box has to be
		/// </summary>
		/// <param name="minVal">the lower bound of the interval</param>
		/// <param name="maxVal">the upper bound of the interval</param>
		/// <returns>true if maxVal >= minVal, false otherwise</returns>
		[Constraint("Interval accepted", Description = "Field must have a value between %0 and %1 included")]
		public bool AddIntervalConstraint(string minVal, string maxVal)
		{
			int min, max;
			if (!int.TryParse(minVal, out  min) ||
						!int.TryParse(maxVal, out max) ||
						min > max)
				return false;

			//max facet
			XmlSchemaMaxInclusiveFacet maxFacet = new XmlSchemaMaxInclusiveFacet();
			maxFacet.Value = max.ToString();
			Common.addFacet(maxFacet, Common.getElementFromSchema(baseSchema));
			//min facet
			XmlSchemaMinInclusiveFacet minFacet = new XmlSchemaMinInclusiveFacet();
			minFacet.Value = min.ToString();
			Common.addFacet(minFacet, Common.getElementFromSchema(baseSchema));

			return true;
		}

		/// <summary>
		/// Private method used by AddLowerThanConstraint, AddGreaterThanConstraint
		/// and AddIntervalConstraint
		/// </summary>
		/// <param name="value">threshold to check</param>
		/// <param name="greater">true if the value is the min value, false otherwise</param>
		/// <returns>true</returns>
		private bool AddCompareConstraint(int value, bool greater)
		{
			XmlSchemaFacet facet = (greater ?
					(XmlSchemaFacet)new XmlSchemaMinInclusiveFacet() :
					(XmlSchemaFacet)new XmlSchemaMaxInclusiveFacet());

			facet.Value = value.ToString();
			Common.addFacet(facet, Fields.Common.getElementFromSchema(baseSchema));

			return true;
		}

		#endregion

		#region Operations

		/// <summary>
		/// Sums the content of two IntBox instances and retuns
		/// a new instance of IntBox with the result of the operation
		/// </summary>
		/// <param name="add1">the first addendum</param>
		/// <param name="add2">the second addendum</param>
		/// <returns>the sum result</returns>
		[Operation("Sum")]
		public static IntBox Sum(IntBox add1, IntBox add2)
		{
			IntBox res = new IntBox();
			res.text = add1.text + add2.text;
			return res;
		}

		[Operation("Substraction")]
		public static IntBox Sub(IntBox sub1, IntBox sub2)
		{
			IntBox res = new IntBox();
			res.text = sub1.text - sub2.text;
			return res;

		}

		[Operation("Multiplication")]
		public static IntBox Mul(IntBox mul1, IntBox mul2)
		{
			IntBox res = new IntBox();
			res.text = mul1.text * mul2.text;
			return res;

		}

		[Operation("Division")]
		public static IntBox Div(IntBox div1, IntBox div2)
		{
			IntBox res = new IntBox();
			res.text = div1.text / div2.text;
			return res;
		}
		
		[Operation("Module")]
		public static IntBox Mod(IntBox mod1, IntBox mod2)
		{
			IntBox res = new IntBox();
			res.text = (int)mod1.text % mod2.text;
			return res;
		}

		[Operation("Absolute Value")]
		public static IntBox Abs(IntBox abs1)
		{
			IntBox res = new IntBox();
			res.text = Math.Abs(abs1.text);
			return res;
		}
	
		[Operation("Tuncate")]
		public static IntBox Truncate(IntBox trunc1)
		{
			IntBox res = new IntBox();
			res.text = (int)Math.Truncate(trunc1.text);
			return res;
		}

		#endregion

		#region Predicates

		[Predicate("Equals")]
		public static bool Equals(IntBox eq1, IntBox eq2)
		{
			return (eq1.text - eq2.text) == 0 ? true : false;
		}

		[Predicate("Greater Than")]
		public static bool Greater(IntBox gt1, IntBox gt2)
		{
			return (gt1.text > gt2.text) ? true : false;
		}

		[Predicate("Lesser Than")]
		public static bool Lesser(IntBox lt1, IntBox lt2)
		{
			return (lt1.text < lt2.text) ? true : false;
		}

		[Predicate("IsEmpty", Description = "Checks whether user has put content inside the field")]
		public static bool IsEmpty(IntBox s)
		{
			return double.IsNaN(s.text);
		}


		#endregion


		#region IntBox Control

		public class IntBoxControl : TextBox
		{
			IntBox father;

			public IntBoxControl(IntBox ib)
				: base()
			{
				father = ib;
				this.ID = String.IsNullOrEmpty(father.Name) ? "_intBox" : father.Name;

				// for rollback..
				if (!double.IsNaN(father.text))
					this.Text = father.text.ToString();
			}

		}

		#endregion

	}
}
