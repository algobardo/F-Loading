using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.Xml.Schema;
using Mobile.Fields;

namespace Mobile.Fields.Group
{
    /// <summary>
    /// This class represents a <see cref="GroupBox"/> object that 
    /// contains a set of different fields.
    /// </summary>
    [FieldAttribute("GroupBox")]
    public class GroupBox : FieldGroup
    {
        /// <summary>
        /// List of type that forms the GroupBox instance
        /// </summary>
        private List<Field> fields;

        public override Field[] Fields
        {
            get
            {
                return fields.ToArray();
            }
        }

        public String Annotation
        {
            get;
            set;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBox"/> class.
        /// </summary>
        /// <param name="typeName">The name of the class that rapresents the generic type of the instance.</param>
        /// <param name="elementName">The name of the <see cref="GroupBox"/> instance in the presentation.xml file of the workflow.</param>
        /// <param name="schema">The schema xml that describe the <see cref="GroupBox"/> type.</param>
        public GroupBox(String typeName, String elementName, XmlSchemaSet schema)
            : base(typeName, elementName, schema)
        {
            fields = new List<Field>();

            //Instatiate subfields
            XmlSchemaComplexType myType = (schema.GlobalElements[new XmlQualifiedName(elementName)] as XmlSchemaElement).SchemaType as XmlSchemaComplexType;
            XmlSchemaSequence seq = myType.Particle as XmlSchemaSequence;
            foreach (XmlSchemaObject subf in seq.Items)
            {
                XmlSchemaElement element = subf as XmlSchemaElement;
                Field subfield = FieldFactory.CreateField(element);
                if (subfield == null)
                    throw new ArgumentException(element.SchemaType != null? "The type " + element.SchemaType.Name + " is not supported" : "One of the children is not supported");
                
                fields.Add(subfield);
            }
        }
    }
}
