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
	/// This class represents a schema complexType.
	/// </summary>
	class ComplexType : IComplexType
	{
		private XmlSchemaComplexType complexType;
		private XmlSchemaSet schemas;

		/// <summary>
		/// The value of this class can vary.
		/// If it is a complexType with a sequence inside then his value is the "sequence" of every element's value inside the sequence.
		/// else if it is a complexType with a choice inside then his value is a List with just one element.
		/// </summary>
		private List<XmlNode> value = null;

		public ComplexType(XmlSchemaSet schemas, XmlSchemaComplexType referredType)
		{
			this.complexType = referredType;
			this.schemas = schemas;
		}

		#region IField Members

		public void SetValue(List<XmlNode> nds)
		{
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
			var field = FieldsManager.GetInstance(renderingDocument.FirstChild, complexType.Particle, schemas);
			if (this.value != null)
				((IField)field).SetValue(this.value);
			return field.GetWebControl(server, renderingDocument.FirstChild);
		}

		public string JSON_StyleProperties
		{
			get { throw new InvalidOperationException(); }
		}

		public string GetValue(Control ctrl, XmlNode renderingDocument)
		{
			var field = FieldsManager.GetInstance(renderingDocument.FirstChild, complexType.Particle, schemas);

			return field.GetValue(ctrl, renderingDocument.FirstChild);
		}

		#endregion

	}
}
