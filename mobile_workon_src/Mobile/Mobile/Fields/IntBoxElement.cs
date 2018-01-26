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

namespace Mobile.Fields
{
    /// <summary>
    /// This class represents the Manager for the type IntBox
    /// </summary>
    /// 
    public class IntBoxElement : Element
    {
        public int? Value
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBoxManager"/> class.
        /// </summary>
        public IntBoxElement(XmlSchemaSet schema, String typeName, String elementName)
        {
            this.Schema = schema;
            this.TypeName = typeName;
            this.ElementName = elementName;
            AddElementSchema();
        }

        public static new Element New(XmlSchemaSet schema, String name, String elementName)
        {
            return new IntBoxElement(schema, name, elementName);
        }

        public override Control Render(Rectangle rect)
        {
            TextBox stringBox = new TextBox();
            stringBox.Text = Value.ToString();
            stringBox.Width = rect.Width;
            stringBox.Height = CFMeasureString.MeasureString(stringBox, stringBox.Text, stringBox.ClientRectangle).Height;
            return stringBox;
        }

        public override void Update(Control control)
        {
            Value = Int32.Parse((control as TextBox).Text);
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

            if (Value != null)
            {
                doc.FirstChild.FirstChild.AppendChild(doc.CreateTextNode(Value.ToString()));
            }

            return doc.FirstChild as XmlElement;
        }

        public override void FromXml(XmlElement element)
        {
            ElementName = element.Name;
            Value = Int32.Parse(element.FirstChild.Value.Trim());
        }

        public override void RenderAdorner(Control control, AdornerType t)
        {
            if (t == AdornerType.Error)
                control.BackColor = Color.Red;
        }

        public static bool Equal(IntBoxElement par1, IntBoxElement par2)
        {
            return par1.Value == par2.Value;
        }

        public static bool Greater(IntBoxElement par1, IntBoxElement par2)
        {
            return par1.Value > par2.Value;
        }

        public static bool Less(IntBoxElement par1, IntBoxElement par2)
        {
            return par1.Value < par2.Value;
        }

        public static IntBoxElement Sum(IntBoxElement par1, IntBoxElement par2)
        {
            IntBoxElement element = new IntBoxElement(null, "IntBox", "IntBox");
            element.Value = par1.Value + par2.Value;
            return element;
        }

        public override void Clear()
        {
            Value = null;
        }
    }
}
