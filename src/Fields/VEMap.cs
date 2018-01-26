using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;
using System.Web.UI;

using System.Linq;
using System.Web;

using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;

[assembly: System.Web.UI.WebResource("Fields.Resources.VEMapScript.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("Fields.Resources.pencil.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.Resources.cpencil.png", "image/png")]

[assembly: System.Web.UI.WebResource("Fields.images.vemap_icon.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.vemap_preview.png", "image/png")]
[assembly: System.Web.UI.WebResource("Fields.images.vemap_preview_tooltip.png", "image/png")]
namespace Fields
{
    /// <summary>
    /// This Field prenders a Microsoft Virtual Earth map, and gives the possibility to search locations using both 
    /// GPS coordinates and 
    /// </summary>
	public class VEMap : IBaseType
	{

		private XmlSchema baseSchema;
		private string latitude;
		private string longitude;

		/// <summary>
		/// Initializes a new instance of the <see cref="VEMap"/> class.
		/// </summary>
		public VEMap()
		{
			baseSchema = new XmlSchema();

			//<xs:complexType name="VEMapN">
			XmlSchemaComplexType newType = new XmlSchemaComplexType();
			newType.Name = "";
			baseSchema.Items.Add(newType);

			//<xs:complexContent>
			XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
			newType.ContentModel = complexContent;

			//<xs:extension base="VEMap">
			XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
			complexContent.Content = complexContentExtension;
			complexContentExtension.BaseTypeName = new XmlQualifiedName("VEMap");

			//<xs:sequence>
			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexContentExtension.Particle = seq;

			//<xs:element name="Latitude" type="xs:string"/>
			XmlSchemaElement elem1 = new XmlSchemaElement();
			seq.Items.Add(elem1);
			elem1.Name = "Latitude";
			elem1.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

			//<xs:element name="Longitude" type="xs:string"/>
			XmlSchemaElement elem2 = new XmlSchemaElement();
			seq.Items.Add(elem2);
			elem2.Name = "Longitude";
			elem2.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="VEMap"/> class from a XMLSchema
		/// </summary>
		/// <param name="schema">The schema xml that describe the VEMap state.</param>
		public VEMap(XmlSchema schema, string name)
			: this()
		{
			this.Name = name;

			//Setting subtype name in the XSD
			//<xs:complexType name="VEMapN">
			((XmlSchemaComplexType)baseSchema.Items[0]).Name = ((XmlSchemaComplexType)schema.Items[0]).Name;

			// Warning: I'll trust in the elements order

			// Check nillable elements
			List<XmlSchemaElement> schemaElements = Common.getElementsFromSchema(schema);
			List<XmlSchemaElement> baseSchemaElements = Common.getElementsFromSchema(baseSchema);
			if (schemaElements[0].IsNillable)
				baseSchemaElements[0].IsNillable = true;
			if (schemaElements[1].IsNillable)
				baseSchemaElements[1].IsNillable = true;

			// adding latitude constraints
			if (((XmlSchemaSimpleType)schemaElements[0].SchemaType) != null)
			{
				XmlSchemaObjectCollection latConstrColl =
						  ((XmlSchemaSimpleTypeRestriction)
									 ((XmlSchemaSimpleType)
									 schemaElements[0]
						.SchemaType).Content).Facets;

				foreach (XmlSchemaFacet facet in latConstrColl)
				{
					Common.addFacet(facet, baseSchemaElements[0]);
				}
			}

			// adding longitude constraints
			if (((XmlSchemaSimpleType)schemaElements[1].SchemaType) != null)
			{
				XmlSchemaObjectCollection longConstrColl =
						  ((XmlSchemaSimpleTypeRestriction)
									 ((XmlSchemaSimpleType)
									 schemaElements[1]
						.SchemaType).Content).Facets;

				foreach (XmlSchemaFacet facet in longConstrColl)
				{
					Common.addFacet(facet, baseSchemaElements[1]);
				}
			}

			// adding custom constraints (close your eyes =)
			XmlSchemaComplexContentExtension cce =
				 (XmlSchemaComplexContentExtension)(
					  (XmlSchemaComplexContent)(
							((XmlSchemaComplexType)
							schema.Items[0])
					  ).ContentModel
				 ).Content;
			if (cce.Annotation != null)
				foreach (XmlSchemaDocumentation sd in cce.Annotation.Items)
					if (sd.Markup[0].Name == "maxDistanceFrom")
						AddMaxDistanceConstraint(sd.Markup[0].ChildNodes[0].InnerText, sd.Markup[0].ChildNodes[1].InnerText, sd.Markup[0].ChildNodes[2].InnerText);
		}


		#region IField Members

		public string TypeName
		{
			set
			{
				//<xs:complexType name="VEMapN">
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

		public string GetValue(Control ctrl1, XmlNode renderingDocument)
		{
			Control ctrl = ctrl1.Controls[1];

			string root = String.IsNullOrEmpty(this.Name) ? "null" : this.Name;

			XmlDocument doc = new XmlDocument();
			XmlNode nodeValue = doc.CreateElement(root);
			doc.AppendChild(nodeValue);
			XmlElement el1 = doc.CreateElement("Latitude");
			XmlElement el2 = doc.CreateElement("Longitude");
			nodeValue.AppendChild(el1);
			nodeValue.AppendChild(el2);
			// Encoding examples (when needed):
			//((VEMapControl)ctrl).latitude.FromNotEncodedRender2XmlName(); 
			//XmlConvert.EncodeLocalName((((VEMapControl)ctrl).latitude))
			nodeValue.ChildNodes[0].InnerText = ((VEMapControl)ctrl).latitude == null ? "" : ((VEMapControl)ctrl).latitude;
			nodeValue.ChildNodes[1].InnerText = ((VEMapControl)ctrl).longitude == null ? "" : ((VEMapControl)ctrl).longitude;
			if (nodeValue.ChildNodes[0].InnerText == "" && nodeValue.ChildNodes[1].InnerText == "" && Common.getElementFromSchema(baseSchema).IsNillable)
			{
				el1.SetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			}
			if (nodeValue.ChildNodes[1].InnerText == "" && Common.getElementFromSchema(baseSchema).IsNillable)
			{
				el2.SetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			}
			return doc.OuterXml;
		}

		public void SetValue(List<XmlNode> nds)
		{
			this.latitude = nds[0].ChildNodes[0].InnerText.Trim();
			this.longitude = nds[0].ChildNodes[1].InnerText.Trim();
		}

		[Obsolete]
		public System.Xml.XmlNode Value
		{
			set
			{
				this.latitude = value.ChildNodes[0].InnerText.Trim();
				this.longitude = value.ChildNodes[1].InnerText.Trim();
			}

			get
			{
				//if (latitude == null || longitude == null) return null;
				string root = String.IsNullOrEmpty(Name) ? "null" : Name;

				XmlDocument doc = new XmlDocument();
				XmlNode nodeValue = doc.CreateElement(root);
				nodeValue.AppendChild(doc.CreateElement("Latitude"));
				nodeValue.AppendChild(doc.CreateElement("Longitude"));
				nodeValue.ChildNodes[0].InnerText = latitude == null ? "" : latitude;
				nodeValue.ChildNodes[1].InnerText = longitude == null ? "" : longitude;
				return nodeValue;
			}
		}

		public string Name { set; get; }

		public static WebControl Icon
		{
			get
			{
				return new CustomImage("Fields.images.vemap_icon.png", "Requires a position on a map");
			}
		}

		public static WebControl Preview
		{
			get
			{
				return new CustomImage("Fields.images.vemap_preview.png");
			}
		}

		public static WebControl PreviewTooltip
		{
			get
			{
				return new CustomImage("Fields.images.vemap_preview_tooltip.png");
			}
		}

		public void setExampleValue()
		{
			latitude = "43,7212";
			longitude = "10,4077";
		}

		public Control GetWebControl(System.Web.HttpServerUtility server, XmlNode renderingDocument)
		{
			PlaceHolder ph = new PlaceHolder();

			Label lbl = new Label();
			if (renderingDocument.Attributes["label"] != null)
				lbl.Text = renderingDocument.Attributes["renderedLabel"].Value.FromXmlValue2Render(server);
			else
				lbl.Text = this.Name.FromXmlName2Render(server);
			lbl.CssClass = "label";
			ph.Controls.Add(lbl);

			VEMapControl vem = new VEMapControl(this);

			if (renderingDocument.Attributes["class"] != null)
				vem.CssClass = renderingDocument.Attributes["class"].Value.FromXmlValue2Render(server);
			if (renderingDocument.Attributes["rel"] != null)
				renderingDocument.Attributes["rel"].Value.FromXmlValue2Render(server);
			if (renderingDocument.Attributes["description"] != null)
				vem.ToolTip = renderingDocument.Attributes["description"].Value.FromXmlValue2Render(server);
            
			ph.Controls.Add(vem);

			// Validators

			List<XmlSchemaElement> baseSchemaElements = Common.getElementsFromSchema(baseSchema);

			if (!baseSchemaElements[0].IsNillable)
			{
				RequiredFieldValidator rqfv = new RequiredFieldValidator();
				rqfv.ErrorMessage = "Required fields shouldn't be empty";
				rqfv.ControlToValidate = vem.ID + "$latitude";
				rqfv.ValidationGroup = "1";
				rqfv.Display = ValidatorDisplay.Dynamic;
				ph.Controls.Add(rqfv);
			}
			if (!baseSchemaElements[1].IsNillable)
			{
				RequiredFieldValidator rqfv = new RequiredFieldValidator();
				rqfv.ErrorMessage = "Required fields shouldn't be empty";
				rqfv.ControlToValidate = vem.ID + "$longitude";
				rqfv.Display = ValidatorDisplay.Dynamic;
				ph.Controls.Add(rqfv);
				rqfv.ValidationGroup = "1";
			}

			// Setting up base validators
			// ( validators for min and max of latitude (-90,90) and longitude (-180,180) )

			CompareValidator maxLatitudeValidator = new CompareValidator();
			maxLatitudeValidator.ID = this.Name + "_maxLatitudeValidator";
			maxLatitudeValidator.ControlToValidate = vem.ID + "$latitude"; ;
			maxLatitudeValidator.Type = ValidationDataType.Integer;
			maxLatitudeValidator.ValueToCompare = "90";
			maxLatitudeValidator.Operator = ValidationCompareOperator.LessThanEqual;
			maxLatitudeValidator.ErrorMessage = "The value has to be lower than or equal to 90";
			maxLatitudeValidator.Text = maxLatitudeValidator.ErrorMessage;
			maxLatitudeValidator.Display = ValidatorDisplay.Dynamic;
			maxLatitudeValidator.Type = ValidationDataType.Double;
			maxLatitudeValidator.ValidationGroup = "1";

			ph.Controls.Add(maxLatitudeValidator);

			CompareValidator minLatitudeValidator = new CompareValidator();
			minLatitudeValidator.ID = this.Name + "_minLatitudeValidator";
			minLatitudeValidator.ControlToValidate = vem.ID + "$latitude"; ;
			minLatitudeValidator.Type = ValidationDataType.Integer;
			minLatitudeValidator.ValueToCompare = "-90";
			minLatitudeValidator.Operator = ValidationCompareOperator.GreaterThanEqual;
			minLatitudeValidator.ErrorMessage = "The value has to be greater than or equal to -90";
			minLatitudeValidator.Text = minLatitudeValidator.ErrorMessage;
			minLatitudeValidator.Display = ValidatorDisplay.Dynamic;
			minLatitudeValidator.Type = ValidationDataType.Double;
			minLatitudeValidator.ValidationGroup = "1";
			ph.Controls.Add(minLatitudeValidator);

			CompareValidator maxLongitudeValidator = new CompareValidator();
			maxLongitudeValidator.ID = this.Name + "_maxLongitudeValidator";
			maxLongitudeValidator.ControlToValidate = vem.ID + "$longitude"; ;
			maxLongitudeValidator.Type = ValidationDataType.Integer;
			maxLongitudeValidator.ValueToCompare = "180";
			maxLongitudeValidator.Operator = ValidationCompareOperator.LessThanEqual;
			maxLongitudeValidator.ErrorMessage = "The value has to be lower than or equal to 180";
			maxLongitudeValidator.Text = maxLongitudeValidator.ErrorMessage;
			maxLongitudeValidator.Display = ValidatorDisplay.Dynamic;
			maxLongitudeValidator.Type = ValidationDataType.Double;
			maxLongitudeValidator.ValidationGroup = "1";
			ph.Controls.Add(maxLongitudeValidator);

			CompareValidator minLongitudeValidator = new CompareValidator();
			minLongitudeValidator.ID = this.Name + "_minLongitudeValidator";
			minLongitudeValidator.ControlToValidate = vem.ID + "$longitude"; ;
			minLongitudeValidator.Type = ValidationDataType.Integer;
			minLongitudeValidator.ValueToCompare = "-180";
			minLongitudeValidator.Operator = ValidationCompareOperator.GreaterThanEqual;
			minLongitudeValidator.ErrorMessage = "The value has to be greater than or equal to -180";
			minLongitudeValidator.Text = minLongitudeValidator.ErrorMessage;
			minLongitudeValidator.Display = ValidatorDisplay.Dynamic;
			minLongitudeValidator.Type = ValidationDataType.Double;
			minLongitudeValidator.ValidationGroup = "1";
			ph.Controls.Add(minLongitudeValidator);

			// setting up latitude constraints
			if (((XmlSchemaSimpleType)baseSchemaElements[0].SchemaType) != null)
			{
				XmlSchemaObjectCollection latConstrColl =
						  ((XmlSchemaSimpleTypeRestriction)
									 ((XmlSchemaSimpleType)
									 baseSchemaElements[0]
						.SchemaType).Content).Facets;

				foreach (XmlSchemaFacet facet in latConstrColl)
				{
					// TODO
					// No Contraints yet :D
				}
			}

			// setting up longitude constraints
			if (((XmlSchemaSimpleType)baseSchemaElements[1].SchemaType) != null)
			{
				XmlSchemaObjectCollection longConstrColl =
						  ((XmlSchemaSimpleTypeRestriction)
									 ((XmlSchemaSimpleType)
									 baseSchemaElements[1]
						.SchemaType).Content).Facets;

				foreach (XmlSchemaFacet facet in longConstrColl)
				{
					// TODO
					// No Contraints yet :D
				}
			}

			// setting up custom constraints (close your eyes =)
			XmlSchemaComplexContentExtension cce = (XmlSchemaComplexContentExtension)((XmlSchemaComplexContent)(((XmlSchemaComplexType)baseSchema.Items[0])).ContentModel).Content;
			if (cce.Annotation != null)
				foreach (XmlSchemaDocumentation sd in cce.Annotation.Items)
				{
					XmlNode node = sd.Markup[0];
					if (node.Name == "maxDistanceFrom")
					{
						double km, lat, lon;
						if (double.TryParse(node.ChildNodes[0].InnerText, out km)
							 && double.TryParse(node.ChildNodes[1].InnerText, out lat)
							 && double.TryParse(node.ChildNodes[2].InnerText, out lon))
						{
							if (maxDistancesFrom == null)
							{
								maxDistancesFrom = new List<List<double>>();
								ph.Controls.Add(getMaxDistanceValidator(vem.ID));
							}
							List<double> newDist = new List<double>();
							newDist.Add(km);
							newDist.Add(lat);
							newDist.Add(lon);
							maxDistancesFrom.Add(newDist);
						}
					}
				}

			return ph;
		}

		[Obsolete]
		public WebControl GetWebControl()
		{
			return null;
		}

		[Obsolete]
		public List<BaseValidator> GetValidators()
		{
			return null;
		}


		#endregion


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

		#region Constraints

		/// <summary>
		/// Adds the is not required constraint.
		/// </summary>
		/// <returns></returns>
		[Constraint("Field not required", Description = "Field values may not be present")]
		public bool AddIsNotRequiredConstraint()
		{
			List<XmlSchemaElement> baseSchemaElements = Common.getElementsFromSchema(baseSchema);
			if (baseSchemaElements[0].IsNillable && baseSchemaElements[1].IsNillable)
			{
				return false;
			}
			else
			{
				baseSchemaElements[0].IsNillable = true;
				baseSchemaElements[1].IsNillable = true;
				return true;
			}
		}

		/// <summary>
		/// Adds the max distance constraint.
		/// </summary>
		/// <param name="Km">The km.</param>
		/// <param name="Latitude">The latitude.</param>
		/// <param name="Longitude">The longitude.</param>
		/// <returns></returns>
		[Constraint("Max Distance From", Description = "The location must be closer than %0 Km from the location of latitude %1 and longitude %2")]
		public bool AddMaxDistanceConstraint(string Km, string Latitude, string Longitude)
		{
			double km, lat, lon;

			if (!double.TryParse(Km, out km) || km < 0)
				return false;
			if (!double.TryParse(Latitude, out lat))
				return false;
			if (!double.TryParse(Longitude, out lon))
				return false;

			if (lat < -90 || lat > 90)
				return false;
			if (lon < -180 || lon > 180)
				return false;

			// custom constraint (close your eyes =)
			XmlSchemaComplexContentExtension cce = (XmlSchemaComplexContentExtension)((XmlSchemaComplexContent)(((XmlSchemaComplexType)baseSchema.Items[0])).ContentModel).Content;

			XmlSchemaAnnotation sa = (cce.Annotation == null) ? new XmlSchemaAnnotation() : cce.Annotation;
			cce.Annotation = sa;

			XmlSchemaDocumentation sd = new XmlSchemaDocumentation();
			sa.Items.Add(sd);

			XmlDocument doc = new XmlDocument();
			XmlNode node = doc.CreateElement("maxDistanceFrom");
			sd.Markup = new XmlNode[1] { node };

			node.AppendChild(doc.CreateElement("Distance"));
			node.AppendChild(doc.CreateElement("Latitude"));
			node.AppendChild(doc.CreateElement("Longitude"));
			node.ChildNodes[0].InnerText = km.ToString();
			node.ChildNodes[1].InnerText = lat.ToString();
			node.ChildNodes[2].InnerText = lon.ToString();

			return true;
		}

		/// <summary>
		/// The handler for getMaxDistanceValidator
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		private void maxDistanceValidateServer(Object source, ServerValidateEventArgs args)
		{
			// AND of all MaxDistance constraints
			//args.IsValid = true;

			// OR of all MaxDistance constraints
			args.IsValid = false;

			string[] latlon = args.Value.Trim().Split(' ');

			if (maxDistancesFrom == null)
				return;

			foreach (List<double> maxdist in maxDistancesFrom)
			{
				double lat1 = 0, lon1 = 0, dist = 0;
				if (!double.TryParse(latlon[0], out  lat1)) dist = double.PositiveInfinity;
				else if (!double.TryParse(latlon[1], out  lon1)) dist = double.PositiveInfinity;
				else dist = distance(lat1, lon1, maxdist[1], maxdist[2]);

				// AND of all MaxDistance constraints
				//if (dist > maxdist[0]) args.IsValid = false;

				// OR of all MaxDistance constraints
				if (dist <= maxdist[0])
				{
					args.IsValid = true;
					//test ((BaseValidator)source).IsValid = false;
				}
			}


		}
		List<List<double>> maxDistancesFrom = null;

		/// <summary>
		/// A validator that checks all the max distances set with constraints.
		/// </summary>
		/// <param name="controlID">The control ID.</param>
		/// <returns></returns>
		private BaseValidator getMaxDistanceValidator(string controlID)
		{
			CustomValidator cv = new CustomValidator();
			cv.ControlToValidate = controlID;
            cv.ValidationGroup = "1";
			cv.ErrorMessage = "The location should be closer than: ";
			foreach (List<double> maxdist in maxDistancesFrom)
				cv.ErrorMessage += maxdist[0] + " Km from [" + maxdist[1] + ", " + maxdist[2] + "]. ";
			cv.ServerValidate += new ServerValidateEventHandler(this.maxDistanceValidateServer);
			//cv.ClientValidationFunction = "validate" + controlID + "MaxDistances";

			return cv;
		}



		#endregion

		#region Operations

		/// <summary>
		/// Computes the distance between two points on the map
		/// </summary>
		/// <param name="map1"></param>
		/// <param name="map2"></param>
		/// <returns>the distance. infinity on error</returns>
		[Operation("Distance in Km")]
		public static IntBox Distance(VEMap map1, VEMap map2)
		{
			double lat1 = 0, lon1 = 0, lat2 = 0, lon2 = 0, dist = 0;
			if (!double.TryParse(map1.latitude, out  lat1)) dist = double.PositiveInfinity;
			else if (!double.TryParse(map1.longitude, out  lat1)) dist = double.PositiveInfinity;
			else if (!double.TryParse(map2.latitude, out  lat1)) dist = double.PositiveInfinity;
			else if (!double.TryParse(map2.longitude, out  lat1)) dist = double.PositiveInfinity;
			else dist = distance(lat1, lon1, lat2, lon2);

			XmlDocument doc = new XmlDocument();
			XmlNode nodeValue = doc.CreateElement("null");
			nodeValue.AppendChild(doc.CreateElement("Value"));
			nodeValue.FirstChild.InnerText = dist.ToString();

			IntBox result = new IntBox();
            List<XmlNode> listres = new List<XmlNode>();
            listres.Add(nodeValue);
            result.SetValue(listres);
			

			return result;
		}
        /// <summary>
        /// Computes the distance between two locations
        /// </summary>
        /// <param name="lat1">The latitude of the first location</param>
        /// <param name="lon1">The longitude of the first location</param>
        /// <param name="lat2">The latitude of the second location</param>
        /// <param name="lon2">The longitude of the second location</param>
        /// <returns></returns>
		private static double distance(double lat1, double lon1, double lat2, double lon2)
		{
			double theta = lon1 - lon2;
			double dist = Math.Sin(deg2rad(lat1))
								 * Math.Sin(deg2rad(lat2))
								 + Math.Cos(deg2rad(lat1))
								 * Math.Cos(deg2rad(lat2))
								 * Math.Cos(deg2rad(theta));
			dist = Math.Acos(dist);
			dist = rad2deg(dist);
			dist = dist * 60 * 1.1515; // Miles
			dist = dist * 1.609344; // to Kilometers
			return dist;
		}
		private static double rad2deg(double rad) { return (rad / Math.PI * 180.0); }
		private static double deg2rad(double deg) { return (deg * Math.PI / 180.0); }

		#endregion

		#region Predicates


        /// <summary>
        /// Return true if the two locations are equal
        /// </summary>
        /// <param name="map1">The first map</param>
        /// <param name="map2">The second map</param>
        /// <returns></returns>
		[Predicate("Equals")]
		public static bool Equals(VEMap map1, VEMap map2)
		{
			return (map1.latitude.Equals(map2.latitude) && map1.longitude.Equals(map2.longitude));
		}

        /// <summary>
        /// Return true if the first location is norther than the second location
        /// </summary>
        /// <param name="map1">The first map</param>
        /// <param name="map2">The second map</param>
        /// <returns></returns>
		[Predicate("Norther Than")]
		public static bool Norther(VEMap map1, VEMap map2)
		{
			return double.Parse(map1.latitude) > double.Parse(map2.latitude);
		}

        /// <summary>
        /// Return true if the first location is souther than the second location
        /// </summary>
        /// <param name="map1">The first map</param>
        /// <param name="map2">The second map</param>
        /// <returns></returns>
		[Predicate("Souther Than")]
		public static bool Souther(VEMap map1, VEMap map2)
		{
			return double.Parse(map1.latitude) < double.Parse(map2.latitude);
		}


		#endregion

		#region PreviewControl
		public Control GetUnrelatedControl()
		{
			return new VEMapControl(this);
		}

		#endregion

		/// <summary>
		/// VEMap WebControl
		/// </summary>
		[
		AspNetHostingPermission(SecurityAction.Demand,
			 Level = AspNetHostingPermissionLevel.Minimal),
		AspNetHostingPermission(SecurityAction.InheritanceDemand,
			 Level = AspNetHostingPermissionLevel.Minimal),
		DefaultEvent("Submit"),
		  DefaultProperty("latlong"),
		  ValidationProperty("latlong"),
		ToolboxData("<{0}:VEMapControl runat=\"server\"> </{0}:VEMapControl>"),
		]
		public class VEMapControl : CompositeControl
		{

			VEMap father;

			private Label latitudeLabel;
			private Label longitudeLabel;
			private Label searchLabel;
			private TextBox latitudeTextBox;
			private TextBox longitudeTextBox;
			private TextBox searchTextBox;

			private bool viewOnly = false;

			public VEMapControl(VEMap field)
				: base()
			{
				father = field;
				this.ID = String.IsNullOrEmpty(father.Name) ? "_VEMap" : father.Name;

				if (father.latitude != null)
					latitude = father.latitude;
				if (father.longitude != null)
					longitude = father.longitude;
			}

			/// <summary>
			/// Gets the latitude and longitude encoded in a single string just for validation.
			/// </summary>
			/// <value>The latlong.</value>
			[
			Bindable(true),
			Category("Appearance"),
			DefaultValue(""),
			Description("A string with latitude and longitude.")
			]
			public string latlong
			{
				get
				{
					return latitude + " " + longitude;
				}
			}

			[
			Bindable(true),
			Category("Appearance"),
			DefaultValue(""),
			Description("The latitude.")
			]
			public string latitude
			{
				get
				{
					EnsureChildControls();
					return latitudeTextBox.Text;
				}
				set
				{
					EnsureChildControls();
					latitudeTextBox.Text = value;
				}
			}

			[
			Bindable(true),
			Category("Appearance"),
			DefaultValue(""),
			Description("The longitude.")
			]
			public string longitude
			{
				get
				{
					EnsureChildControls();
					return longitudeTextBox.Text;
				}
				set
				{
					EnsureChildControls();
					longitudeTextBox.Text = value;
				}
			}

			protected override void RecreateChildControls()
			{
				EnsureChildControls();
			}

			protected override void OnInit(EventArgs e)
			{
				base.OnInit(e);

                // VEMapScript.js specializes (setting visualization, adding circles, managing pushpin, ..) the Microsoft script 
				string resourceName = "Fields.Resources.VEMapScript.js";
				string remoteResource = "http://dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2";

				ClientScriptManager cs = this.Page.ClientScript;
				cs.RegisterClientScriptInclude(typeof(VEMapControl), "VEMap", remoteResource);
				cs.RegisterClientScriptResource(typeof(VEMapControl), resourceName);

				if (!this.ChildControlsCreated)
					this.CreateChildControls();
			}

			protected override void OnPreRender(EventArgs e)
			{
				base.OnPreRender(e);
			}

			protected override void CreateChildControls()
			{
				if (this.ChildControlsCreated)
					return;

				base.CreateChildControls();

				Controls.Clear();

				latitudeLabel = new Label();
				latitudeLabel.Text = "Latitude";

				longitudeLabel = new Label();
				longitudeLabel.Text = "Longitude";

				searchLabel = new Label();
				searchLabel.Text = "Search";

				latitudeTextBox = new TextBox();
				latitudeTextBox.ID = "latitude";

				longitudeTextBox = new TextBox();
				longitudeTextBox.ID = "longitude";

				searchTextBox = new TextBox();
				searchTextBox.ID = "search";
				this.Controls.Add(latitudeTextBox);
				this.Controls.Add(longitudeTextBox);
				this.Controls.Add(searchTextBox);

				ChildControlsCreated = true;
			}

			protected override void Render(HtmlTextWriter writer)
			{
				string mapDivID = this.ClientID + "Div"; //this.ID;
				string mapObjID = this.ClientID + "Obj"; //this.ID;

				AddAttributesToRender(writer);

				writer.RenderBeginTag(HtmlTextWriterTag.Div);

				writer.AddAttribute(HtmlTextWriterAttribute.Id, mapDivID, false);
				writer.AddAttribute(HtmlTextWriterAttribute.Style, "position:relative;width:400px;height:300px;z-index:0;", false);

				writer.RenderBeginTag(HtmlTextWriterTag.Div);
				writer.RenderEndTag();

				writer.RenderBeginTag(HtmlTextWriterTag.Table);

				writer.RenderBeginTag(HtmlTextWriterTag.Tr);

				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				latitudeLabel.RenderControl(writer);
				writer.RenderEndTag();

				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				longitudeLabel.RenderControl(writer);
				writer.RenderEndTag();

				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				searchLabel.RenderControl(writer);
				writer.RenderEndTag();

				writer.RenderEndTag();

				writer.RenderBeginTag(HtmlTextWriterTag.Tr);

				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				latitudeTextBox.Attributes.Add("size", "6");
				latitudeTextBox.Attributes.Add("onkeyup", mapObjID + ".updateMapLatLong()");
				latitudeTextBox.Attributes.Add("onblur", mapObjID + ".updateLatLong()");
				latitudeTextBox.RenderControl(writer);
				writer.RenderEndTag();

				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				longitudeTextBox.Attributes.Add("size", "6");
				longitudeTextBox.Attributes.Add("onkeyup", mapObjID + ".updateMapLatLong()");
				longitudeTextBox.Attributes.Add("onblur", mapObjID + ".updateLatLong()");
				longitudeTextBox.RenderControl(writer);
				writer.RenderEndTag();

				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				searchTextBox.Attributes.Add("size", "10");
				searchTextBox.Attributes.Add("onkeyup", mapObjID + ".updateMapWhere()");
				searchTextBox.RenderControl(writer);
				writer.RenderEndTag();

				writer.RenderEndTag();

				writer.RenderEndTag();

				writer.RenderEndTag();
			}

			protected override void OnLoad(EventArgs e)
			{
				base.OnLoad(e);

				Control x = this;
				while ((x = x.Parent) != null) //Checks if a parent is disabled
					if (x is WebControl && (this.viewOnly = !((WebControl)x).Enabled))
						break;

				string mapDivID = this.ClientID + "Div"; //this.ID;
				string mapObjID = this.ClientID + "Obj"; //this.ID;

				string ImageUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "Fields.Resources.pencil.png");
				string CImageUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "Fields.Resources.cpencil.png");

				System.IO.StringWriter writer = new System.IO.StringWriter();

				writer.WriteLine("<script type=\"text/javascript\">");

				writer.WriteLine("	var " + mapObjID + " = new MyVEMap(\"" + mapObjID + "\", \"" + mapDivID + "\", \"" + latitudeTextBox.ClientID + "\", \"" + longitudeTextBox.ClientID + "\", \"" + searchTextBox.ClientID + "\", \"" + ImageUrl + "\", \"" + CImageUrl + "\", " + (this.viewOnly ? "true" : "false") + ");");

				// drawing circles of maxDistanceValidator
				if (father.maxDistancesFrom != null)
					foreach (List<double> maxdist in father.maxDistancesFrom)
						writer.WriteLine("	" + mapObjID + ".addMaxDist(\"" + maxdist[1] + "\",\"" + maxdist[2] + "\",\"" + maxdist[0] + "\");");

				writer.WriteLine("	" + mapObjID + ".draw();");

				writer.WriteLine("</script>");

				ScriptManager.RegisterClientScriptBlock(this, this.GetType(), this.ClientID + "Script", writer.ToString(), false);



			}
		}

	}

}