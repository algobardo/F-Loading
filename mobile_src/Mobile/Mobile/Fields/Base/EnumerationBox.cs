using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using Mobile.Fields;

namespace Mobile.Fields.Base
{
    /// <summary>
    /// This class represents an <see cref="EnumerationBox"/> Object which allows
    /// the user to choose an option between a list of mutually exclusive choices
    /// </summary>
    [FieldAttribute("RadioButtonList")]
    public class EnumerationBox : Field
    {
        private int selectedIndex;

        public String[] Values
        {
            get;
            private set;
        }

        public int SelectedIndex 
        {
            get 
            {
                return selectedIndex;
            }
            set 
            {
                if(value >= 0 && value < Values.Length)
                    selectedIndex = value;
            }
        }


        /// <summary>
        /// Create a new instance of the <see cref="EnumerationBox"/> class initilizing all the
        /// list's value
        /// </summary>
        /// <param name="typeName">The name of the <see cref="EnumerationBox"/> instance in the workflow.</param>
        /// <param name="elementName">The name of the element in the workflow.</param>
        /// <param name="schema">The schema xml that describe the BoolBox type.</param>
        public EnumerationBox(String typeName, String elementName, XmlSchemaSet schema)
        {
            this.TypeName = typeName;
            this.FieldName = elementName;
            this.Schema = schema;

            List<String> values = new List<string>();

            //Instatiate all the possible values
            XmlSchemaComplexType myType = schema.GlobalTypes[new XmlQualifiedName(typeName)] as XmlSchemaComplexType;
            XmlSchemaComplexContentExtension ext = myType.ContentModel.Content as XmlSchemaComplexContentExtension;
            XmlSchemaSequence seq = ext.Particle as XmlSchemaSequence;
            foreach (XmlSchemaObject subf in seq.Items)
            {
                XmlSchemaElement element = subf as XmlSchemaElement;
                XmlSchemaSimpleType type = element.SchemaType as XmlSchemaSimpleType;
                XmlSchemaSimpleTypeRestriction restriction = type.Content as XmlSchemaSimpleTypeRestriction;
                foreach (XmlSchemaFacet enumeration in restriction.Facets)
                    values.Add(enumeration.Value);

                Values = values.ToArray();               
            }
        }

        /// <summary>
        /// Override the Clear() method of the <see cref="Field"/> class
        /// </summary>
        public override void Clear()
        {
            selectedIndex = 0;
        }

        /// <summary>
        /// Override the Field's class ToString() method which returns the value of
        /// a StringBox as a string
        /// </summary>
        /// <returns>the value of the StringBox that invokes it</returns>
        public override string ToString()
        {
            return Values[selectedIndex];
        }

        /// <summary>
        /// Converts the current <see cref="EnumerationBox"/> object into an <see cref="XmlElement"/> object
        /// </summary>
        /// <returns>an <see cref="XmlElement"/> rapresenting the <see cref="EnumerationBox"/></returns>
        public override XmlElement ToXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement(FieldName));
            doc.FirstChild.AppendChild(doc.CreateElement("Value"));

            doc.FirstChild.FirstChild.AppendChild(doc.CreateTextNode(Values[selectedIndex]));

            return doc.FirstChild as XmlElement;
        }

        /// <summary>
        /// Restore a EnumerationBox element setting the correct element's state
        /// represented by the choice's list
        /// </summary>
        /// <param name="element">The element to be restored.</param>
        public override void FromXml(XmlElement element)
        {
            FieldName = element.Name;
            String value = (element.FirstChild.FirstChild as XmlText).Value.Trim();
            if (element.FirstChild.HasChildNodes)
            {
                for (int i = 0; i < Values.Length; i++)
                    if (Values[i] == value)
                        selectedIndex = i;
            }
            else
                selectedIndex = 0;
        }

        /// <summary>
        /// Override the FromString method from Field class
        /// </summary>
        /// <param name="element"></param>
        public override void FromString(String element)
        {
        }
    }
}
