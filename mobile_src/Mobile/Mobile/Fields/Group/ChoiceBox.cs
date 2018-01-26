using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using System.Xml;

namespace Mobile.Fields.Group
{
    /// <summary>
    /// This class represents a <see cref="ChoiceBox"/> object that 
    /// enables the user to select a single option from a group of choices. 
    /// </summary>
    [FieldAttribute("ChoiceBox")]
    public class ChoiceBox : FieldGroup
    {
        /// <summary>
        /// List of choices that forms the <see cref="ChoiceBox"/> instance
        /// </summary>
        private List<Field> fields;

        /// <summary>
        /// Index of the selected field
        /// </summary>
        private int selectedIndex;

        public override Field[] Fields
        {
            get
            {
                return fields.ToArray();
            }
        }

        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                if (value >= 0 && value < Fields.Length)
                    selectedIndex = value;
            }
        }
        

        /// <summary>
        /// Initializes a new instance of the <see cref="ChoiceBox"/> class.
        /// </summary>
        /// <param name="typeName">The name of the class that rapresents the generic type of the instance.</param>
        /// <param name="elementName">The name of the <see cref="ChoiceBox"/> instance in the presentation.xml file of the workflow.</param>
        /// <param name="schema">The schema xml that describe the <see cref="ChoiceBox"/> type.</param>
        public ChoiceBox(String typeName, String elementName, XmlSchemaSet schema)
            : base(typeName, elementName, schema)
        {
            fields = new List<Field>();

            //Instatiate all the possible fields
            XmlSchemaType myType = (schema.GlobalElements[new XmlQualifiedName(elementName)] as XmlSchemaElement).SchemaType;
            XmlSchemaChoice choice = (myType as XmlSchemaComplexType).Particle as XmlSchemaChoice;
            foreach(XmlSchemaElement element in choice.Items) 
            {
                Field subfield = FieldFactory.CreateField(element);
                fields.Add(subfield);
            }
        }


        /// <summary>
        /// Restore the state of the element setting the selected choice.
        /// </summary>
        /// <param name="element">The element to be restored.</param>
        public override void FromXml(XmlElement element)
        {
            FieldName = element.Name;
            for (int i = 0; i < Fields.Length; i++)
            {
                if (Fields[i].FieldName == ((XmlElement)element.FirstChild).Name)
                {
                    Fields[i].FromXml((XmlElement)element.FirstChild);
                    selectedIndex = i;
                }
            }
        }


        /// <summary>
        /// Converts the <see cref="ChoiceBox"/> instance into an <see cref="XmlElement"/> object
        /// </summary>
        public override XmlElement ToXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement(FieldName));

            doc.FirstChild.AppendChild(doc.ImportNode(Fields[SelectedIndex].ToXml(), true));
            return doc.FirstChild as XmlElement;
        }


        /// <summary>
        /// Recuperates the rendering informations to display the type
        /// </summary>
        /// <param name="presentation">The element to be parsed</param>
        public override void ParsePresentation(XmlElement presentation)
        {
            base.ParsePresentation(presentation);
            if (presentation.HasChildNodes && presentation.FirstChild.HasChildNodes)
            {
                for (int i = 0; i < Fields.Length; i++)
                    Fields[i].ParsePresentation(presentation.FirstChild.FirstChild.ChildNodes[i] as XmlElement);
            }
        }
    }
}
