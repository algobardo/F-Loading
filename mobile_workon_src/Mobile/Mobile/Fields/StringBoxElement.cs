using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Windows.Forms;
using System.IO;

namespace Mobile.Fields
{
    class StringBoxElement : Element
    {
        public String Value
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBoxManager"/> class.
        /// </summary>
        public StringBoxElement(XmlSchemaSet schema, String typeName, String elementName)
        {
            this.Schema = schema;
            this.TypeName = typeName;
            this.ElementName = elementName;
            AddElementSchema();
        }

        public static new Element New(XmlSchemaSet schema, String name, String elementName)
        {
            return new StringBoxElement(schema, name, elementName);
        }

        public override Control Render(Rectangle rect)
        {
            /* PARAMS:
             * XmlAnnotatedElement n --> <Nome>[StringBox1]<Value>value or nothing</Value></Nome>
             */

            TextBox stringBox = new TextBox();
            stringBox.Text = Value;
            stringBox.Width = rect.Width;
            stringBox.Height = CFMeasureString.MeasureString(stringBox, stringBox.Text, stringBox.ClientRectangle).Height;
            return stringBox;
        }

        public override void Update(Control control)
        {
            Value = (control as TextBox).Text;
        }

        public override XmlElement ToXml()
        {
            /* PARAMS:
             * Control c filled by user
             * XmlAnnotatedElement filled --> <Nome>[StringBox1]<Value>value or nothing</Value></Nome>
             */
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement(ElementName));
            doc.FirstChild.AppendChild(doc.CreateElement("Value"));
            
            if(Value!=null)
            {
                doc.FirstChild.FirstChild.AppendChild(doc.CreateTextNode(Value));
            }

            return doc.FirstChild as XmlElement;
        }

        public override void FromXml(XmlElement element)
        {
            ElementName = element.Name;
            Value = (element.FirstChild.FirstChild as XmlText).Value.Trim();
        }

        public override void RenderAdorner(Control control, AdornerType t)
        {
            if (t == AdornerType.Error)
                control.BackColor = Color.Red;
        }

        /// <summary>
        /// Determines whether the node1 value and node2 value have the same value.
        /// </summary>
        /// <param name="node1">The first value</param>
        /// <param name="node2">The secondo value</param>
        /// <returns>True if node1 value is equal than node2 value, false otherwise</returns>
        public static bool Equal(StringBoxElement node1, StringBoxElement node2)
        {
            return node1.Value == node2.Value;
        }


        /// <summary>
        /// Determines whether the node1 value is lexicographically greater than node2 value.
        /// </summary>
        /// <param name="node1">The first value</param>
        /// <param name="node2">The second value</param>
        /// <returns>True if the node1 value is lexicographically greater than node2 value, false otherwise</returns>
        public static bool Greater(StringBoxElement node1, StringBoxElement node2)
        {
            if (String.Compare(node1.Value, node2.Value) > 0)
                return true;
            return false;
        }

        /// <summary>
        /// Returns the length of the node value.
        /// </summary>
        /// <param name="node1">The first value</param>
        /// <param name="node2">The second value</param>
        /// <returns>The sum of the length of the nodes value.</returns>
        public static IntBoxElement Lenght(StringBoxElement par)
        {
            IntBoxElement element = new IntBoxElement(null, "IntBox", "IntBox");
            element.Value = par.Value.Length;
            return element;
        }

        public override void Clear()
        {
            Value = null;
        }
    }
}
