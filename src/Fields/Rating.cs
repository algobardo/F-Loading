using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Web.UI;
using System.Web.UI.WebControls;


[assembly: System.Web.UI.WebResource("Fields.Rating.StarEmpty.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.Rating.StarSaved.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.Rating.StarFilled.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.Rating.rating.js", "text/javascript")]

[assembly: System.Web.UI.WebResource("Fields.images.rating_icon.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.rating_preview.png", "image/png")]
// [assembly: System.Web.UI.WebResource("Fields.images.rating_preview_tooltip.png", "image/png")]

namespace Fields
{
	public class Rating : IBaseType
	{
		private XmlSchema baseSchema;
		private int rate = -1;
		private XmlSchemaMaxInclusiveFacet maxRateFacet = new XmlSchemaMaxInclusiveFacet();

		#region constructors

		public Rating()
		{
			baseSchema = new XmlSchema();

			//<xs:complexType name="RatingN"> first time is empty
			XmlSchemaComplexType newType = new XmlSchemaComplexType();
			newType.Name = "";
			baseSchema.Items.Add(newType);

			//<xs:complexContent>
			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			newType.ContentModel = complexContent;

			//<xs:extension base="Rating">
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
			complexContent.Content = complexContentExtension;
			complexContentExtension.BaseTypeName = new XmlQualifiedName("Rating");

			//<xs:sequence>
			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexContentExtension.Particle = seq;

			//<xs:element name="Value" type="xs:integer"/>
			XmlSchemaElement elem = new XmlSchemaElement();
			seq.Items.Add(elem);
			elem.Name = "Value";
			elem.SchemaTypeName = new XmlQualifiedName("integer", "http://www.w3.org/2001/XMLSchema");

			//to Check
			XmlSchemaMinInclusiveFacet minFacet = new XmlSchemaMinInclusiveFacet();
			minFacet.Value = "0";
			Common.addFacet(minFacet, Common.getElementFromSchema(baseSchema));

			maxRateFacet.Value = "5";
			Common.addFacet(maxRateFacet, Common.getElementFromSchema(baseSchema));
		}

		public Rating(XmlSchema schema, string name)
		{
			this.Name = name;

			baseSchema = new XmlSchema();

			//<xs:complexType name="RatingN">
			XmlSchemaComplexType newType = new XmlSchemaComplexType();
			newType.Name = ((XmlSchemaComplexType)schema.Items[0]).Name;
			baseSchema.Items.Add(newType);

			//<xs:complexContent>
			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			newType.ContentModel = complexContent;

			//<xs:extension base="Rating">
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
			complexContent.Content = complexContentExtension;
			complexContentExtension.BaseTypeName = new XmlQualifiedName("Rating");

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
					  (
							(XmlSchemaSimpleType)Common.getElementFromSchema(schema)
						.SchemaType)
				.Content)
			.Facets;

			foreach (XmlSchemaFacet facet in constrColl)
			{
				Common.addFacet(facet, Common.getElementFromSchema(baseSchema));
				// update the maxRate pointer
				if (facet is XmlSchemaMaxInclusiveFacet)
					maxRateFacet = (XmlSchemaMaxInclusiveFacet)facet;
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

			int maxRate = 0;
			int.TryParse(maxRateFacet.Value, out maxRate);
			RatingControl starBox = new RatingControl(this, maxRate);

			//		if (renderingDocument.Attributes["class"] != null)
			//			starBox.CssClass = renderingDocument.Attributes["class"].Value.FromXmlValue2Render(server);
			//		if (renderingDocument.Attributes["rel"] != null)
			//			starBox.Attributes.Add("rel", renderingDocument.Attributes["rel"].Value.FromXmlValue2Render(server));
			//if (renderingDocument.Attributes["description"] != null)
			//    starBox.ToolTip = renderingDocument.Attributes["description"].Value.FromXmlValue2Render(server);

			ph.Controls.Add(starBox);


			//--- validators ---

			return ph;
		}

		public string GetValue(Control ctrl1, XmlNode renderingDocument)
		{
			RatingControl ctrl = (RatingControl)ctrl1.Controls[1];
			string root = String.IsNullOrEmpty(this.Name) ? "null" : this.Name;

			XmlDocument doc = new XmlDocument();
			XmlNode nodeValue = doc.CreateElement(root);
			doc.AppendChild(nodeValue);
			nodeValue.AppendChild(doc.CreateElement("Value"));
			nodeValue.FirstChild.InnerText = ctrl.CurrentRating.ToString();
			return doc.OuterXml;
		}

		public void SetValue(List<XmlNode> nds)
		{
			try
			{
				this.rate = int.Parse(nds[0].FirstChild.InnerText);
			}
			catch (Exception e)
			{
				Console.WriteLine("Rating.SetValue exception: " + e.Message);
				this.rate = 0;
			}
		}

		public string Name { set; get; }

		public void setExampleValue() { rate = 2; }

		public static WebControl Icon
		{
			get
			{
				return new CustomImage("Fields.images.rating_icon.png", "Rating star");
			}
		}

		public static WebControl Preview
		{
			get
			{
				return new CustomImage("Fields.images.rating_preview.png");
			}
		}

		public static WebControl PreviewTooltip
		{
			get
			{
				return null;
			}
		}

		public string JSON_StyleProperties
		{
			get
			{
				return "";
			}
		}

		#endregion


		#region Predicates

		[Predicate("Equals")]
		public static bool Equals(Rating r1, Rating r2)
		{
			return (r1.rate - r2.rate) == 0 ? true : false;
		}

		[Predicate("Greater Than")]
		public static bool Greater(Rating r1, Rating r2)
		{
			return (r1.rate > r2.rate) ? true : false;
		}

		[Predicate("Lesser Than")]
		public static bool Lesser(Rating r1, Rating r2)
		{
			return (r1.rate < r2.rate) ? true : false;
		}


		#endregion

		#region constraints

		[Constraint("Max Rate", Description = "Change the max value that the user can select")]
		public bool MaxRateConstraint(string maxVal)
		{
			int max;
			if (int.TryParse(maxVal, out max) && (max > 0))
			{
				maxRateFacet.Value = max.ToString();
				return true;
			}
			else
				return false;
		}

		#endregion

		#region PreviewControl

		public Control GetUnrelatedControl()
		{
			return new RatingControl(this, 5);
		}

		#endregion

		#region RatingControl

#if true

		public class RatingControl : AjaxControlToolkit.Rating
		{
			Rating father;
			int maxRate;

			public RatingControl(Rating ib, int maxRate)
				: base()
			{
				father = ib;
				this.maxRate = maxRate;
				this.ID = String.IsNullOrEmpty(father.Name) ? "_Rating" : father.Name;
			}

			protected override void OnChanged(AjaxControlToolkit.RatingEventArgs e)
			{
				base.OnChanged(e);
				this.CurrentRating = int.Parse(e.Value);
			}

			protected override void LoadControlState(object savedState)
			{
				base.CurrentRating = (int)savedState;
			}

			protected override object SaveControlState()
			{
				return base.CurrentRating;
			}

			protected override void OnInit(EventArgs e)
			{
				base.OnInit(e);
				Page.RegisterRequiresControlState(this);
			}

			protected override void OnLoad(EventArgs e)
			{
				base.OnLoad(e);

				this.MaxRating = maxRate;
                this.AutoPostBack = true;
				this.EnableViewState = true;

				this.CssClass = "ratingStar";
				this.StarCssClass = "ratingItem";
				this.WaitingStarCssClass = "Saved";
				this.FilledStarCssClass = "Filled";
				this.EmptyStarCssClass = "Empty";

				// for rollback..
				if (father.rate > 0 && father.rate <= this.MaxRating)
					this.CurrentRating = father.rate;


				//// Find if a parent WebControl is disabled
				Control x = this.Parent;
				while (x != null)
				{
				    if (x is WebControl && (((WebControl)x).Enabled == false))
				    {
				        this.ReadOnly = true;
				        break;
				    }
				    x = x.Parent;
				}

				ClientScriptManager cs = this.Page.ClientScript;
                
				string css =
			@"<style type='text/css'>
						.ratingStar{ 
								white-space:nowrap;
								margin:1em;
								height:14px;
							}
							.ratingStar .ratingItem {
								 font-size: 0pt;
								 width: 13px;
								 height: 12px;
								 margin: 0px;
								 padding: 0px;
								 display: block;
								 background-repeat: no-repeat;
								 cursor:pointer;
							}
							.ratingStar .Filled {
								background-image: url(" +
										cs.GetWebResourceUrl(this.GetType(), "Fields.Rating.StarFilled.png") + @");
								}
							.ratingStar .Empty {
								background-image: url(" +
										cs.GetWebResourceUrl(this.GetType(), "Fields.Rating.StarEmpty.png") + @");
							}
							.ratingStar .Saved {
								background-image: url(" +
										cs.GetWebResourceUrl(this.GetType(), "Fields.Rating.StarSaved.png") + @");
							}
			</style>";

				cs.RegisterClientScriptBlock(this.GetType(), "stylesheet", css, false);
			}


			public override bool Enabled
			{
				get { return !this.ReadOnly; }
				set { this.ReadOnly = !value; }
			}

		}

#else

		private class RatingControl : System.Web.UI.WebControls.RadioButtonList
		{
			Rating father;

			public RatingControl(Rating field, int maxRate)
				: base()
			{
				father = field;
				this.ID = father.Name == null ? "_rating" : father.Name;
				this.RepeatDirection = RepeatDirection.Horizontal;

				for (int i = 1; i <= maxRate; i++)
					this.Items.Add(i.ToString());

				// for rollback..
				if (father.rate > 0 && father.rate <= maxRate)
					this.CurrentRating = father.rate;
				else this.CurrentRating = 0;
			}

			public int CurrentRating
			{
				set {
					try
					{
						this.SelectedValue = value.ToString();
					}
					catch (Exception) {}
				}
				get
				{
					int tmp;
					if (int.TryParse(this.SelectedValue, out tmp))
						return tmp;
					else return 0;
				}
			}

		}

#endif



		#region RatingJScontrol
		/*		
		public class RatingJSControl : TextBox{
			private int maxRate = 5;
			private Rating father;

			public RatingJSControl(Rating fath, int maxRate){
				//this.Visible = false;
				this.maxRate = maxRate;
				this.father = fath;
				this.ID = String.IsNullOrEmpty(father.Name) ? "_Rating" : father.Name;
				
				// rollback
				if (fath.rate > 0)
					this.CurrentRating = fath.rate;

				CurrentRating = 2;
			}

			protected override void OnPreRender(EventArgs e)
			{
				base.OnPreRender(e);

				ClientScriptManager cs = this.Page.ClientScript;
				cs.RegisterClientScriptResource(this.GetType(), "Fields.Rating.rating.js");

				string css =
					@"<style type='text/css'>
						.rater
						{
						float:left;
						cursor: pointer;
						width:96px;
						z-index:6;
						}

						span.mouseOn
						{
						 display:block;
						 float:left;
						 background-image: url(" +
								cs.GetWebResourceUrl(this.GetType(), "Fields.Rating.StarSaved.png") + 
						@");
						 width:16px;
						 height:15px;
						}

						span.mouseOff
						{
						 display:block;
						 float:left;
						background-image: url(" +
								cs.GetWebResourceUrl(this.GetType(), "Fields.Rating.StarFilled.png") +
						@");
						 width:16px;
						 height:15px;
						}

						span.blank
						{
						 display:block;
						 float:left;
						 background-image: url(" +
								cs.GetWebResourceUrl(this.GetType(), "Fields.Rating.StarEmpty.png") +
						@");
						 width:16px;
						 height:15px;
						}

						div.clear
						{
						 clear:both;
						}
				</style>";

					cs.RegisterClientScriptBlock(this.GetType(), "css", css, false);

			}

			protected override void Render(HtmlTextWriter writer)
			{
				
				AddAttributesToRender(writer);

				writer.Write(@"<input type='hidden' id='ciccio' value='1' />");
				writer.Write(@"<div id='rater' class='rater'><div id='stars'></div></div>");
				writer.Write(writer.NewLine);

				
				writer.Write(
					"<script type=\"text/javascript\">" +
					@"var r = new rating('stars'," + maxRate + "," + CurrentRating + " , true, 'ciccio');" +
					@"</script>");

				
			}

			public int CurrentRating
			{
				get;
				set;
			}

		}
*/
		#endregion

		#endregion

	}
}
