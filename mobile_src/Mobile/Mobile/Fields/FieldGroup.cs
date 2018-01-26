 using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Mobile.Fields
{
    public abstract class FieldGroup : Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldGroup"/> class.
        /// </summary>
        /// <param name="typeName">The name of the class that rapresents the generic type of the instance.</param>
        /// <param name="elementName">The name of the <see cref="FieldGroup"/> instance</param>
        /// <param name="schema">The schema xml that describe the <see cref="FieldGroup"/> type.</param>
        public FieldGroup(String typeName, String elementName, XmlSchemaSet schema)
        {
            this.TypeName = typeName;
            this.FieldName = elementName;
            this.Schema = schema;
        }

        /// <summary>
        /// The <see cref="Field"/> elements contained in the <see cref="FieldGroup"/> object
        /// </summary>
        public abstract Field[] Fields
        {
            get;
        }

        /// <summary>
        /// Override the Clear() method of the <see cref="Field"/> class
        /// </summary>
        public override void Clear()
        {
            foreach (Field field in Fields)
                field.Clear();
        }

        /// <summary>
        /// Gives a String representation of the <see cref="FieldGroup"/> object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Fields.Length == 0 ? "" : Fields[0].ToString();
        }

        /// <summary>
        /// Converts the current <see cref="FieldGroup"/> object into an <see cref="XmlElement"/> object
        /// </summary>
        public override XmlElement ToXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement(FieldName));

            foreach (Field elem in Fields)
                doc.FirstChild.AppendChild(doc.ImportNode(elem.ToXml(), true));

            return doc.FirstChild as XmlElement;
        }

        public override void FromString(String element)
        {}

        /// <summary>
        /// Restore a <see cref="FieldGroup"/> element setting the correct element's state.
        /// </summary>
        /// <param name="element">The element to be restored.</param>
        public override void FromXml(System.Xml.XmlElement element)
        {
            FieldName = element.Name;
            for (int i = 0; i < Fields.Length; i++)
                Fields[i].FromXml((XmlElement)element.ChildNodes[i]);
        }

        /// <summary>
        /// This method associate the presentation of a Field to the correct child Field parsing if from the Presentation's<see cref="XMLSchema"/> object
        /// </summary>
        /// <param name="presentation"></param>
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
