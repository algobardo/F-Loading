using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web;
using System.Xml.Schema;

namespace Fields
{
	/// <summary>
	/// This class represents a classical panel BUT INamingContainer.
	/// This panel manage all ID of controls inside him guaranting they don't throw any exception.
	/// </summary>
	public class NamingPanel : Panel, INamingContainer { }
	public class NamingUpdatePanel : UpdatePanel, INamingContainer { }

	/// <summary>
	/// This class contains a render and an xml value getter for an Xml Schema Element of this form:
	/// <element name="..." minOccurs=y maxOccurs=x type="complexType"> with x > 1.
	/// or
	/// <element name="...">
	/// <complexType>
	/// ...
	/// </complexType>
	/// </element>
	/// 
	/// </summary>
	class ComplexElement : IComplexType
	{
		private XmlSchemaElement referredElement;
		private XmlSchemaSet schemas;

		/// <summary>
		/// This is the xml value referring to this Xml Schema Element.
		/// 
		/// Value will be just a List with one element.
		/// This element will be a node of the form
		///  <nodename>
		///     
		///  </nodename>
		///
		/// </summary>
		private List<XmlNode> value = null;

		public ComplexElement(XmlSchemaSet schemas, XmlSchemaElement referredElement)
		{
			this.referredElement = referredElement;
			this.schemas = schemas;
		}


		#region IField Members

		public void SetValue(List<XmlNode> nds) { value = nds; }


		public string Name
		{
			get; set;
		}

		public void setExampleValue()
		{
			throw new InvalidOperationException();
		}

		public Control GetWebControl(HttpServerUtility server, System.Xml.XmlNode renderingDocument)
		{
			/* if it is a repeatable element, of any type, complexType or our base types*/
			if (referredElement.MaxOccurs > 1)
			{
				XmlSchemaElement newElm = new XmlSchemaElement();
              
				/*I copy the schema element omitting min e max Occurs*/
				newElm.Name = referredElement.Name;
				newElm.SchemaType = referredElement.SchemaType;
				newElm.SchemaTypeName = referredElement.SchemaTypeName;

				/*and i create a repeater panel that manages the repetition of that single element (without the max e min occurs)*/
				RepeaterPanel rp = new RepeaterPanel(schemas, renderingDocument, newElm, value, (int)referredElement.MinOccurs, (int)referredElement.MaxOccurs);
				rp.ID = referredElement.Name + "_Repeating";
				UpdatePanel up = new UpdatePanel();
				up.ContentTemplateContainer.Controls.Add(new LiteralControl("<BR>"));
				up.ContentTemplateContainer.Controls.Add(rp);

				return up;
			}
			else /*if it is just an element with an inline complex type*/
			{
				NamingPanel pn = new NamingPanel();

				string name;
				if (renderingDocument.Attributes["renderedLabel"] != null)
					name = renderingDocument.Attributes["renderedLabel"].Value.FromXmlValue2Render(server);
				else
					name = referredElement.Name.FromXmlName2Render(server);

				pn.GroupingText = name;
				pn.ID = referredElement.Name;
				/*This field should be a ComplexType*/
				var field = FieldsManager.GetInstance(renderingDocument.FirstChild, referredElement.SchemaType, schemas);
				/*I set the value to the ComplexType inside the element*/
				if (value != null)
					((IField)field).SetValue(value[0].ChildNodes.ToList());
				pn.Controls.Add(field.GetWebControl(server, renderingDocument.FirstChild));

				return pn;
			}
		}


		public string JSON_StyleProperties
		{
			get { throw new InvalidOperationException(); }
		}

		public string GetValue(Control ctrl, XmlNode renderingDocument)
		{
			/*if, once here, the getWebControl created a RepeaterPanel*/
			if (referredElement.MaxOccurs > 1)
			{
				string rs = "";

				RepeaterPanel rp = ((ctrl as UpdatePanel).ContentTemplateContainer.Controls[1] as RepeaterPanel);
				XmlSchemaElement newElm = new XmlSchemaElement();
				newElm.Name = referredElement.Name;
				newElm.SchemaType = referredElement.SchemaType;
				newElm.SchemaTypeName = referredElement.SchemaTypeName;
				newElm.IsNillable = referredElement.IsNillable;

				foreach (Control ct in rp.RepeatedItems())
				{
					rs += FieldsManager.GetInstance(renderingDocument, newElm, schemas).GetValue(ct, renderingDocument);
				}
				return rs;
			}
			/*else there was just a standard complexType inside the <element> */
			else
			{
				Panel pn = ctrl as Panel;
				/*i create a node with the element name with inside the value of my child (complexType)*/
				return "<" + referredElement.Name + ">"
					 + FieldsManager.GetInstance(renderingDocument.FirstChild, referredElement.SchemaType, schemas).GetValue(pn.Controls[0], renderingDocument.FirstChild) +
					 "</" + referredElement.Name + ">";
			}
		}

		#endregion
	}
}
