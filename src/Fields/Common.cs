using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace Fields
{
	class Common
	{
		public static XmlSchemaSimpleType getSimpleType(XmlSchemaFacet faceset, string typeqname)
		{
			XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
			XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
			restriction.Facets.Add(faceset);
			restriction.BaseTypeName = new XmlQualifiedName(typeqname, "http://www.w3.org/2001/XMLSchema");
			simpleType.Content = restriction;
			return simpleType;
		}

		public static XmlSchemaElement getElementFromSchema(XmlSchema baseSchema)
		{
			//DO NOT REMOVE
			//XmlSchemaComplexType ext = ((XmlSchemaComplexType)baseSchema.Items[0]);
			//XmlSchemaComplexContent xcc = (XmlSchemaComplexContent)ext.ContentModel;
			//XmlSchemaComplexContentExtension xext = (XmlSchemaComplexContentExtension)xcc.Content;
			//XmlSchemaSequence seq = (XmlSchemaSequence)xext.Particle;      
			//XmlSchemaElement elem = (XmlSchemaElement)seq.Items[0];

			//it can be done better, maybe with XSL Transformation.
			//Gets sequence and element node.
			//For the sake of readability...
			XmlSchemaSequence seq = (XmlSchemaSequence)((XmlSchemaComplexContentExtension)((XmlSchemaComplexContent)(((XmlSchemaComplexType)baseSchema.Items[0])).ContentModel).Content).Particle;
			return (XmlSchemaElement)seq.Items[0];
		}

		public static List<XmlSchemaElement> getElementsFromSchema(XmlSchema baseSchema)
		{
			List<XmlSchemaElement> lxmls = new List<XmlSchemaElement>();
			XmlSchemaSequence seq =
				 (XmlSchemaSequence)(
					  (XmlSchemaComplexContentExtension)(
							(XmlSchemaComplexContent)(
								 ((XmlSchemaComplexType)
								 baseSchema.Items[0])
							 ).ContentModel
						).Content
				 ).Particle;

			foreach (XmlSchemaElement elem in seq.Items)
				lxmls.Add(elem);

			return lxmls;
		}

		public static void addFacet(XmlSchemaFacet facet, XmlSchemaElement elem)
		{
			if (elem.SchemaType == null)
			{
				//create <xs:SimpleType> to contain restrictions if doesn't exist
				XmlSchemaSimpleType simpleType = getSimpleType(facet, elem.SchemaTypeName.Name);
				elem.SchemaTypeName = null;
				elem.SchemaType = simpleType;
			}
			else
			{
				//add restriction
				((XmlSchemaSimpleTypeRestriction)((XmlSchemaSimpleType)elem.SchemaType).Content).Facets.Add(facet);
			}
		}

	}
}
