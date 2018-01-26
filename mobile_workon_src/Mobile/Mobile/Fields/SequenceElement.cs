using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.Xml.Schema;

namespace Mobile.Fields
{
    class SequenceElement : Element
    {
        private List<Element> elements = new List<Element>();

        public Element[] Elements
        {
            get { return elements.ToArray(); }
        }

        public SequenceElement(XmlSchemaSet schema, String typeName, String elementName)
        {
            this.Schema = schema;
            this.TypeName = typeName;
            this.ElementName = elementName;
            AddElementSchema(); 
        }

        public static new Element New(XmlSchemaSet schema, String typeName, String elementName)
        {
            return new SequenceElement(schema,typeName,elementName);
        }

        public override Control Render(Rectangle rect)
        {
            ListBox tempList=new ListBox();

            foreach (Element elem in elements)
            {
                Control elemControl = elem.Render(rect);
                tempList.Items.Add(elemControl);
            }

            return tempList;
        }

        public override void Update(Control control)
        {
            for (int i = 0;i< elements.Count;i++)
            {
                elements[i].Update(((ListBox)control).Items[i] as Control);
            }
        }

        public override XmlElement ToXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement(ElementName));

            foreach (Element elem in elements)
            {
                doc.FirstChild.AppendChild(elem.ToXml());
            }

            return doc.FirstChild as XmlElement;
        }

        public override void FromXml(System.Xml.XmlElement element)
        {
            ElementName = element.Name;

            for (int i = 0; i < element.ChildNodes.Count; i++)
            {
                elements[i].FromXml((XmlElement)element.ChildNodes[i]);
            }
        }

        public override void RenderAdorner(System.Windows.Forms.Control control, AdornerType t)
        {
            if (t == AdornerType.Error)
                control.BackColor = Color.Red;
        }

        public override void Clear()
        {
            elements.Clear();
        }
    }
}
