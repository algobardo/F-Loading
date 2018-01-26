using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using System.Web;
using System.Xml;
using System.Web.UI.WebControls;
using System.Web.UI;

[assembly: System.Web.UI.WebResource("Fields.images.namedsequence_icon.png", "image/png")]
namespace Fields
{
	/// <summary>
	/// This class represent a Sequence
	/// with the assumption that EACH ELEMENT IN THE SEQUENCE IS A SCHEMA ELEMENT
	/// </summary>
	class Sequence : IComplexType
	{

		private XmlSchemaSet completeSchemas;
		private XmlSchemaSequence namedSequence;

		private List<XmlNode> value = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="NamedSequence"/> class from a XMLSchema
		/// </summary>
		/// <param name="schema">The schema xml that describe the NamedSequence state.</param>
		public Sequence(XmlSchemaSet schema, XmlSchemaSequence referredElement)
		{
			completeSchemas = schema;
			namedSequence = referredElement;
		}

		#region IField Members

		public void SetValue(List<XmlNode> nds) {
            value = nds;
		}

		public string Name
		{
			get; set;
		}

		public void setExampleValue()
		{
			throw new InvalidOperationException();
		}

		public Control GetWebControl(HttpServerUtility server, XmlNode renderingDocument)
		{
			/*WARNING: This MUST be the same procedure of the one in GetValue, otherwise nothing works*/

			Panel pn = new Panel();

			int schemaChildCount = 0;
			foreach (XmlNode n in renderingDocument.ChildNodes)
			{

				if (n.Name.StartsWith("xs_"))/*this is a schema element!*/
				{
					XmlSchemaObject rndObj = namedSequence.Items[schemaChildCount];
					var field = FieldsManager.GetInstance(n, rndObj, this.completeSchemas);


					/*If someone sets the value on this class i must set it to my child.*/
					if (value != null)
					{
						List<XmlNode> d = new List<XmlNode>();
						/*This foreach is needed for repeating elements
						 The schema specify just <element name=".." maxOccurs=".."> 
						 but here we can have many nodes with the same name matching that element*/
						foreach (XmlNode nvalue in value)
						{
							if (nvalue.SchemaInfo.SchemaElement.Name == ((XmlSchemaElement)rndObj).Name)
								d.Add(nvalue);
						}
						((IField)field).SetValue(d);
					}

					var wctrl = field.GetWebControl(server, n);

					pn.Controls.Add(wctrl);
					pn.Controls.Add(new LiteralControl("<BR>"));
					schemaChildCount++;
				}
				else /*this is a rendering only element*/
					pn.Controls.Add(FieldsManager.GetInstance(n, null, null).GetWebControl(server, n));
			}
			return pn;
		}

		public string JSON_StyleProperties
		{
			get { throw new InvalidOperationException(); }
		}

		public string GetValue(Control ctrl, XmlNode renderingDocument)
		{
			/*WARNING: This procedure assumes that GetWebControl uses 2 controls foreach schema element inside the sequence*/
			StringBuilder sb = new StringBuilder();

			Panel pn = (Panel)ctrl;
			int schemaChildCount = 0;
			int childControlCount = 0;
			foreach (XmlNode n in renderingDocument.ChildNodes)
			{
				if (n.Name.StartsWith("xs_"))
				{
					sb.Append(FieldsManager.GetInstance(n, namedSequence.Items[schemaChildCount], completeSchemas).GetValue(pn.Controls[childControlCount], n));
					schemaChildCount++;
					/*look at the assumption*/
					childControlCount += 2;
				}
				else
				{
					childControlCount++;
				}
			}
			return sb.ToString();
		}

		public static WebControl Icon
		{
			get
			{
				return new CustomImage("Fields.images.namedsequence_icon.png", "Creates a sequence group");
			}
		}

		#endregion

		#region obsolete

		[Obsolete]
		public XmlNode Value
		{
			get
			{
				throw new InvalidOperationException();
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		[Obsolete]
		public List<BaseValidator> GetValidators()
		{
			throw new NotImplementedException();
		}

		[Obsolete]
		public List<BaseValidator> GetValidators(Control controlId)
		{
			throw new NotImplementedException();
		}

		[Obsolete]
		public WebControl GetWebControl()
		{
			throw new NotImplementedException();
		}


		#endregion

	}
}
