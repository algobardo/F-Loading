using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

[assembly: System.Web.UI.WebResource("Fields.images.calendar_icon.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.calendar_preview.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.calendar_preview_tooltip.png", "image/png")]
namespace Fields
{

	public class Calendar : IBaseType
	{
		private XmlSchema baseSchema;
		private DateTime? date = null;

		#region constructor

		public Calendar()
		{
			baseSchema = new XmlSchema();

			//<xs:complexType name="CalendarN"> first time is empty
			XmlSchemaComplexType newType = new XmlSchemaComplexType();
			newType.Name = "";
			baseSchema.Items.Add(newType);

			//<xs:complexContent>
			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			newType.ContentModel = complexContent;

			//<xs:extension base="Calendar">
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
			complexContent.Content = complexContentExtension;
			complexContentExtension.BaseTypeName = new XmlQualifiedName("Calendar");

			//<xs:sequence>
			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexContentExtension.Particle = seq;

			//<xs:element name="Value" type="xs:date"/>
			XmlSchemaElement elem = new XmlSchemaElement();
			seq.Items.Add(elem);
			elem.Name = "Value";

			elem.SchemaTypeName = new XmlQualifiedName("date", "http://www.w3.org/2001/XMLSchema");
		}

		public Calendar(XmlSchema schema, string name)
		{
			this.Name = name;

			baseSchema = new XmlSchema();

			//<xs:complexType name="CalendarN">
			XmlSchemaComplexType newType = new XmlSchemaComplexType();
			newType.Name = ((XmlSchemaComplexType)schema.Items[0]).Name;
			baseSchema.Items.Add(newType);

			//<xs:complexContent>
			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			newType.ContentModel = complexContent;

			//<xs:extension base="Calendar">
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
			complexContent.Content = complexContentExtension;
			complexContentExtension.BaseTypeName = new XmlQualifiedName("Calendar");

			//<xs:sequence>
			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexContentExtension.Particle = seq;

			//<xs:element name="Value" type="xs:date"/>
			XmlSchemaElement elem = new XmlSchemaElement();
			seq.Items.Add(elem);
			elem.Name = "Value";
			elem.SchemaTypeName = new XmlQualifiedName("date", "http://www.w3.org/2001/XMLSchema");

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
		/* For methods documentation's see the IField interface */

		public Control GetWebControl(System.Web.HttpServerUtility server, XmlNode renderingDocument)
		{
			string tip = (renderingDocument.Attributes["description"] == null) ?
					  "" : renderingDocument.Attributes["description"].Value.FromXmlValue2Render(server);

			PlaceHolder ph = new PlaceHolder();

			Label lbl = new Label();
			if (renderingDocument.Attributes["renderedLabel"] != null)
				lbl.Text = renderingDocument.Attributes["renderedLabel"].Value.FromXmlValue2Render(server);
			else
				lbl.Text = this.Name.FromXmlName2Render(server);
			lbl.CssClass = "label";
			lbl.ToolTip = tip;
			ph.Controls.Add(lbl);

			DateBoxControl dbox = new DateBoxControl(this);
			dbox.ToolTip = tip;

			if (renderingDocument.Attributes["class"] != null)
				dbox.tbox.CssClass = renderingDocument.Attributes["class"].Value.FromXmlValue2Render(server);
			if (renderingDocument.Attributes["rel"] != null)
				dbox.tbox.Attributes.Add("rel", renderingDocument.Attributes["rel"].Value.FromXmlValue2Render(server));

			ph.Controls.Add(dbox);

			// --- ADD VALIDATORS ---


			RegularExpressionValidator rev = new RegularExpressionValidator();
			rev.Display = ValidatorDisplay.Dynamic;
			rev.ControlToValidate = dbox.ID + "$" + dbox.tbox.ID;
			rev.ErrorMessage = "A value isn't a valid date (format: gg/mm/yyyy)";
			const string REgg = "([1-9]|0[1-9]|[12][0-9]|3[01])";
			const string REmm = "([1-9]|0[1-9]|1[012])";
			const string REyy = "[0-9]{4}";
			const string sep = "[/.-]";
			rev.ValidationExpression = "^" + REgg + sep + REmm + sep + REyy + "$";
			rev.ValidationGroup = "1";
			ph.Controls.Add(rev);

			if (!Common.getElementFromSchema(baseSchema).IsNillable)
			{
				RequiredFieldValidator rqfv = new RequiredFieldValidator();
				rqfv.ErrorMessage = "Required fields shouldn't be empty";
				rqfv.ControlToValidate = dbox.ID + "$" + dbox.tbox.ID;
				rqfv.ValidationGroup = "1";
				rqfv.Display = ValidatorDisplay.Dynamic;
				ph.Controls.Add(rqfv);
			}


			if (((XmlSchemaSimpleType)Common.getElementFromSchema(baseSchema).SchemaType) == null)
				return ph;
			XmlSchemaObjectCollection constrColl =
					((XmlSchemaSimpleTypeRestriction)
							((XmlSchemaSimpleType)
							Common.getElementFromSchema(baseSchema)
				  .SchemaType).Content).Facets;


			foreach (XmlSchemaFacet facet in constrColl)
			{
				if (facet is XmlSchemaMinInclusiveFacet)
				{
					RangeValidator rv = new RangeValidator();
					rv.Type = ValidationDataType.Date;
					rv.MinimumValue = DateTime.Parse(facet.Value).ToString().Substring(0, 10);
					rv.ErrorMessage = "Date must be equal or later than " + DateTime.Parse(facet.Value).ToString(CultureInfo.CurrentCulture.DateTimeFormat).Substring(0, 10);
					rv.MaximumValue = DateTime.MaxValue.ToString().Substring(0, 10);
					rv.ControlToValidate = dbox.ID + "$" + dbox.tbox.ID;
					rv.Display = ValidatorDisplay.Dynamic;
					rv.ValidationGroup = "1";
					ph.Controls.Add(rv);
				}
				if (facet is XmlSchemaMaxInclusiveFacet)
				{
					RangeValidator rv = new RangeValidator();
					rv.Type = ValidationDataType.Date;
					rv.MaximumValue = DateTime.Parse(facet.Value).ToString().Substring(0, 10);
					rv.ErrorMessage = "Date must be equal or previous to " + DateTime.Parse(facet.Value).ToString(CultureInfo.CurrentCulture.DateTimeFormat).Substring(0, 10);
					rv.MinimumValue = DateTime.MinValue.ToString().Substring(0, 10);
					rv.ControlToValidate = dbox.ID + "$" + dbox.tbox.ID;
					rv.Display = ValidatorDisplay.Dynamic;
					rv.ValidationGroup = "1";
					ph.Controls.Add(rv);
				}
			}
			return ph;
		}

		public string GetValue(Control ctrl, XmlNode renderingDocument)
		{
			DateBoxControl dbox = (DateBoxControl)ctrl.Controls[1];

			XmlDocument doc = new XmlDocument();
			string root = String.IsNullOrEmpty(this.Name) ? "null" : this.Name;

			XmlNode nodeValue = doc.CreateElement(root);
			XmlElement elem = doc.CreateElement("Value");
			doc.AppendChild(nodeValue);
			nodeValue.AppendChild(elem);
			nodeValue.FirstChild.InnerText = dbox.Data;
			if (String.IsNullOrEmpty(dbox.Data) && Common.getElementFromSchema(baseSchema).IsNillable)
			{
				elem.SetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			}
			return doc.OuterXml;
		}

		public void setExampleValue()
		{
			date = DateTime.Now;
		}

		public void SetValue(List<XmlNode> nds)
		{
			try
			{
				this.date = DateTime.Parse(nds[0].FirstChild.InnerText);
			}
			catch (Exception e)
			{
				Console.WriteLine("Calendar.SetValue: Exception occured\n" + e);
			}
		}

		public string Name { set; get; }

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

		public XmlSchemaComplexType TypeSchema
		{
			get { return (XmlSchemaComplexType)baseSchema.Items[0]; }
		}

		public Control GetUnrelatedControl()
		{
			return new DateBoxControl(this);
		}

		public string JSON_StyleProperties
		{
			get
			{
				return
				@"[
					{ 
					""group"":""background"",
						""properties"": [
						{
							""name"":""background-color"",
							""type"":""text"",
							""validator"":""color"",
							""info"": ""Color of Field background. Default: #ffffff""
						}
						]  
					},
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
					""group"":""Height"",
						""properties"": [
						{
							""name"":""height"",
							""type"":""text"",
							""validator"":""size"",
							""info"": ""Height of date inputbox.""
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
							""info"": ""Width of date inputbox.""
						}
						]  
					}
				] ";

			}//endGet
		}

		#endregion

		#region Image

		public static WebControl Icon
		{
			get
			{
				return new CustomImage("Fields.images.calendar_icon.png", "Show a small calendar to ask a date");
			}
		}

		public static WebControl Preview
		{
			get
			{
				return new CustomImage("Fields.images.calendar_preview.png");
			}
		}

		public static WebControl PreviewTooltip
		{
			get
			{
				return new CustomImage("Fields.images.calendar_preview_tooltip.png");
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


		[Constraint("Minimum date", Description = "Minimum date accepted (Ex: 22/12/1985)")]
		public bool MinimumDateConstraint(string minDate)
		{
			DateTime min;
			if (!DateTime.TryParse(minDate, out min))
				return false;
			else
			{
				XmlSchemaMinInclusiveFacet facet = new XmlSchemaMinInclusiveFacet();
				facet.Value = min.ToString("u").Substring(0, 10);
				Common.addFacet(facet, Common.getElementFromSchema(baseSchema));
				return true;
			}
		}

		[Constraint("Maximum date", Description = "Maximum date (inclusive) accepted (Ex: 31/12/2010)")]
		public bool MaximumDateConstraint(string maxDate)
		{
			DateTime max;
			if (!DateTime.TryParse(maxDate, out max))
				return false;
			else
			{
				XmlSchemaMaxInclusiveFacet facet = new XmlSchemaMaxInclusiveFacet();
				facet.Value = max.ToString("u").Substring(0, 10);
				Common.addFacet(facet, Common.getElementFromSchema(baseSchema));
				return true;
			}
		}

		[Constraint("Acceptable date range", Description = "Range of acceptable dates, the minimum and the maximum date are included inside the range")]
		public bool RangeDateConstraint(string minDate, string maxDate)
		{
			DateTime max, min;
			if (!DateTime.TryParse(maxDate, out max) || !DateTime.TryParse(minDate, out min))
				return false;
			if (min.CompareTo(max) > 0)
				return false;
			XmlSchemaMinInclusiveFacet facmin = new XmlSchemaMinInclusiveFacet();
			facmin.Value = min.ToString("u").Substring(0, 10);
			XmlSchemaMaxInclusiveFacet facmax = new XmlSchemaMaxInclusiveFacet();
			facmax.Value = max.ToString("u").Substring(0, 10);
			Common.addFacet(facmax, Common.getElementFromSchema(baseSchema));
			Common.addFacet(facmin, Common.getElementFromSchema(baseSchema));
			return true;
		}
		#endregion

		#region Predicates


		/// <summary>
		/// Equalses the specified date.
		/// </summary>
		/// <param name="date1">The date1.</param>
		/// <param name="date2">The date2.</param>
		/// <returns></returns>
		[Predicate("Equals", Description = "Compare 2 dates")]
		public static bool Equals(Calendar c1, Calendar c2)
		{
			if (c1 == null || c2 == null || c1.date == null) return false;
			return c1.date.Equals(c2.date);
		}


		[Predicate("Greater Than")]
		public static bool Greater(Calendar c1, Calendar c2)
		{
			if (c1 == null || c2 == null
					  || c1.date == null || c2.date == null)
				return false;
			return c1.date.Value.CompareTo(c2.date.Value) > 0 ? true : false;
		}

		[Predicate("Lesser Than")]
		public static bool Lesser(Calendar c1, Calendar c2)
		{
			if (c1 == null || c2 == null
					  || c1.date == null || c2.date == null)
				return false;
			return c1.date.Value.CompareTo(c2.date.Value) < 0 ? true : false;
		}

		[Predicate("IsEmpty", Description = "Checks whether user has put content inside the field")]
		public static bool IsEmpty(Calendar s)
		{
			return s.date == null;
		}

		#endregion

		public class DateBoxControl : CompositeControl
		{
			public AjaxControlToolkit.CalendarExtender cal;
			public TextBox tbox;
			Calendar father;

			public DateBoxControl(Calendar cal_field)
				: base()
			{
				this.father = cal_field;
				this.ID = cal_field.Name;

				EnsureChildControls();

				cal.OnClientShown = "onClientShownCalendarHandler";
				cal.OnClientHidden = "onClientHiddenCalendarHandler";

				// for rollback
				if (father.date != null)
					this.tbox.Text = father.date.Value.ToString().Substring(0, 10);

			}

			protected override void CreateChildControls()
			{
				if (this.ChildControlsCreated) 
					return;

				base.CreateChildControls();
				Controls.Clear();

				tbox = new TextBox();
				tbox.ID = "txtDate";
				tbox.MaxLength = 10;
				this.Controls.Add(tbox);

				cal = new AjaxControlToolkit.CalendarExtender();
				cal.ID = "calend";
				cal.TargetControlID = tbox.ID;
				cal.Format = "dd/MM/yyyy";
				this.Controls.Add(cal);
			}

			protected override void RecreateChildControls()
			{
				EnsureChildControls();
			}

			/// <summary>
			/// Gets the date in Universal Sortable format or an empty string if the data is invalid
			/// </summary>
			public string Data
			{
				get
				{
					EnsureChildControls();
					DateTime d;
					if (DateTime.TryParse(tbox.Text, out d))
						return d.ToString("u").Substring(0, 10);
					else
						return "";
				}
			}

		}

	}
}