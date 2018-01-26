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
    /// This class represents a <see cref="IntBox"/> object that 
    /// enables user to insert int values in a apposite field
    /// </summary>
    [FieldAttribute("IntBox")]
    public class IntBox : Field
    {
        public int? Value
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new object from the the <see cref="IntBox"/> class.
        /// </summary>
        /// <param name="typeName">The name of the <see cref="IntBox"/> instance in the workflow.</param>
        /// <param name="elementName">The name of the element in the workflow.</param>
        /// <param name="schema">The schema xml that describe the IntBox type.</param>
        public IntBox(String typeName, String elementName, XmlSchemaSet schema)
        {
            this.TypeName = typeName;
            this.FieldName = elementName;
            this.Schema = schema;
        }

        /// <summary>
        /// Override the Clear() method of the <see cref="Field"/> class
        /// </summary>
        public override void Clear()
        {
            Value = null;
        }

        /// <summary>
        /// Override the ToString() method of the <see cref="Field"/> class that
        /// returns the value of the Intbox as a string
        /// </summary>
        public override string ToString()
        {
            return Value == null ? "" : Value.Value.ToString();
        }

        /// <summary>
        /// Converts the current <see cref="IntBox"/> object into an <see cref="XmlElement"/> object
        /// </summary>
        public override XmlElement ToXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement(FieldName));
            doc.FirstChild.AppendChild(doc.CreateElement("Value"));

            if (Value != null)
                doc.FirstChild.FirstChild.AppendChild(doc.CreateTextNode(Value.ToString()));
          
            return doc.FirstChild as XmlElement;
        }

        /// <summary>
        /// Restore a IntBox element setting the correct element's state.
        /// </summary>
        /// <param name="element">The element to be restored.</param>
        public override void FromXml(XmlElement element)
        {
            FieldName = element.Name;
            if (element.FirstChild.HasChildNodes)
                Value = Int32.Parse((element.FirstChild.FirstChild as XmlText).Value.Trim());
            else
                Value = null;
        }

        /// <summary>
        /// Parse the state of a IntBox object from a String
        /// </summary>
        /// <param name="element">The String containing the value</param>
        public override void FromString(String element)
        {
            try
            {
                int value = int.Parse(element);
                Value = value;
            }
            catch (Exception)
            {
                Value = null;
            }
        }

        #region Operators
        /// <summary>
        /// Implements the Equal operation between two <see cref="Intbox"/> returning true 
        /// if the elements have the same value, false otherwise
        /// </summary>
        /// <param name="par1">The first element to analize</param>
        /// <param name="par2">The second one</param>
        /// <returns></returns>
        public static bool Equal(IntBox par1, IntBox par2)
        {
            return par1.Value == par2.Value;
        }

        /// <summary>
        /// Implements the Greater operation between two <see cref="IntBox"/> returning true
        /// if the first element is greater than the second, false otherwise
        /// </summary>
        /// <param name="par1">The first element to analize</param>
        /// <param name="par2">The second one</param>
        public static bool Greater(IntBox par1, IntBox par2)
        {
            return par1.Value > par2.Value;
        }

        /// <summary>
        /// Implements the Less operation between two <see cref="IntBox"/> returning true
        /// if the second element Value is greater than the second, false otherwise
        /// </summary>
        /// <param name="par1">The first element to analize</param>
        /// <param name="par2">The second one</param>
        public static bool Less(IntBox par1, IntBox par2)
        {
            return par1.Value < par2.Value;
        }

        /// <summary>
        /// Implements the Sum operation between two <see cref="IntBox"/> returning the
        /// sum of the two elements as an <see cref="IntBox"/>
        /// </summary>
        /// <param name="par1">The first element to sum</param>
        /// <param name="par2">The second one</param>
        public static IntBox Sum(IntBox par1, IntBox par2)
        {
            IntBox element = new IntBox("IntBox", "IntBox", null);
            element.Value = par1.Value + par2.Value;
            return element;
        }

        #endregion;
    }
}
