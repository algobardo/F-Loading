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
{/*
    class GPSLocationManager : TypeManager
    {
        private XmlSchema schema;
        private int index;
        public string baseType { get { return "GPSLocation"; } }

        public GPSLocationManager(XmlSchema schema, int index)
        {
            this.schema = schema;
            this.index = index;
        }

        public Control Render(XmlNode n, Rectangle rect)
        {
            // Probably to change

            //inserted simply to make the source compilable

            TextBox intBox = new TextBox();
            intBox.Width = rect.Width;
            intBox.Height = CFMeasureString.MeasureString(intBox, intBox.Text, intBox.ClientRectangle).Height;
            return intBox;
        }

        public XmlNode ToXml(Control c, XmlNode element)  // <Anni><Valore>18</Valore></Anni>
        {
            /* PARAMS:
             * Control c filled by user
             * XmlNode element --> <xs:element name="Anni" type="IntBox1"/>
             

            Debug.WriteLine(String.Format("ToXml --> XmlNode received: name = {0}; type = {1}", element.Attributes["name"].Value, element.Attributes["type"].Value));

            //create an XmlNode, eg. <Anni><Valore>18</Valore></Anni>
            XmlDocument doc = new XmlDocument();
            XmlElement n = doc.CreateElement(element.Attributes["name"].Value);
            XmlElement elemChild = doc.CreateElement("Valore");
            XmlText textNode = doc.CreateTextNode(c.Text);
            elemChild.AppendChild(textNode);
            n.AppendChild(elemChild);
            doc.AppendChild(n); //<Anni><Valore>18</Valore></Anni>

            Debug.WriteLine(String.Format("ToXml --> Control received: c.text = {0}", c.Text));
            Debug.WriteLine(String.Format("ToXml --> XmlNode to return: {0}", doc.OuterXml));

            return n;
        }

        public bool CheckConstraints(XmlNode elem, XmlNode xmlNode)
        {
          // TO DO after toXML implementation

            return true;
        }



        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    Debug.WriteLine("Error: {0}", e.Message);
                    break;
                case XmlSeverityType.Warning:
                    Debug.WriteLine("Warning {0}", e.Message);
                    break;
            }

        }

        /*
         TODO PREDICATES AND OPERATIONS 
         
    }*/
}
