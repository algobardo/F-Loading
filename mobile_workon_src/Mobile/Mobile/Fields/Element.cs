using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Drawing;

namespace Mobile.Fields
{
    public enum AdornerType { Error }

    public abstract class Element
    {
        public XmlSchemaSet Schema { get; protected set; }
        public String TypeName { get; protected set; }
        public String ElementName { get; protected set; }

        public static Element New(XmlSchemaSet schema, String name, String elementName)
        {
            return null;
        }

        /// <summary>
        /// Renders a control according to the type of the XmlNode n
        /// </summary>
        /// <param name="n"></param>
        /// <param name="rect"></param>
        /// <returns>The control for the XmlNode n</returns>
        public abstract Control Render(Rectangle rect);
        public abstract void Update(Control control);


        /// <summary>
        /// Converts the Control c into an XML node according to XMLNode element
        /// </summary>
        /// <param name="c">The control to be converted</param>
        /// <param name="element"></param>
        /// <returns>The XML node that represents the control c</returns>
        public abstract XmlElement ToXml();
        public abstract void FromXml(XmlElement element);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="t"></param>
        public abstract void RenderAdorner(Control control, AdornerType t);

        public void CheckConstraints()
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.AppendChild(document.ImportNode(this.ToXml(), true));
                document.Schemas.Add(Schema);
                document.Validate(OnSchemaValidation);
            }
            catch (Exception ex)
            {
                throw new FieldsException(SeverityLevel.Critical, LogLevel.Debug, ex.InnerException, "Il contenuto inserito non ha il formato corretto.");
            }
        }

        public abstract void Clear();

        #region Private Methods

        protected void AddElementSchema()
        {
            XmlSchema newSchema = new XmlSchema(); //StringBox1 Schema
            newSchema.Namespaces.Add("xs", "http://www.w3.org/2001/XMLSchema");

            XmlSchemaElement elementSchema = new XmlSchemaElement();
            newSchema.Items.Add(elementSchema);
            elementSchema.Name = ElementName;
            elementSchema.SchemaTypeName = new XmlQualifiedName(TypeName, newSchema.TargetNamespace);

            Schema.Add(newSchema);
            Schema.Compile();
        }

        private void OnSchemaValidation(object source, ValidationEventArgs args)
        {
        }

        #endregion
    }
}
