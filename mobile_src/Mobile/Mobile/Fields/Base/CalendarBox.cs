using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Mobile.Fields;

namespace Mobile.Fields.Base
{
    /// <summary>
    /// This class represents a <see cref="CalendarBox"/> object that 
    /// enables user to perform operation on a calendar object such as choose
    /// dates
    /// </summary>
    [FieldAttribute("Calendar")]
    public class CalendarBox : Field
    {
        public DateTime? Value
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarBox"/> class.
        /// </summary>
        /// <param name="typeName">The name of the <see cref="CalendarBox"/> instance in the workflow.</param>
        /// <param name="elementName">The name of the element in the workflow.</param>
        /// <param name="schema">The schema xml that describe the CalendarBox type.</param>
        public CalendarBox(String typeName, String elementName, XmlSchemaSet schema)
        {
            this.TypeName = typeName;
            this.FieldName = elementName;
            this.Schema = schema;
        }

        /// <summary>
        /// Override the ToString() method of the <see cref="Field"/> class
        /// </summary>
        public override string ToString()
        {
            return Value == null? "" : Value.Value.ToString();
        }

        /// <summary>
        /// Converts the current <see cref="CalendarBox"/> object into an <see cref="XmlElement"/> object
        /// </summary>
        public override XmlElement ToXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement(FieldName));
            doc.FirstChild.AppendChild(doc.CreateElement("Value"));

            if (Value != null)
            {
                doc.FirstChild.FirstChild.AppendChild(doc.CreateTextNode(Value.Value.ToString("yyyy-MM-dd")));
            }

            return doc.FirstChild as XmlElement;
        }

        /// <summary>
        /// Restore a CalendarBox element setting the correct element's state.
        /// </summary>
        /// <param name="element">The element to be restored.</param>
        public override void FromXml(XmlElement element)
        {
            FieldName = element.Name;
            Value = DateTime.Parse(element.FirstChild.InnerText);
        }

        /// <summary>
        /// Parse date to be used by a CalendarBox object from a string
        /// </summary>
        /// <param name="element">The String containing the date</param>
        public override void FromString(String element)
        {
            try
            {
                DateTime value = DateTime.Parse(element);
                Value = value;
            }
            catch (Exception)
            {
                Value = null;
            }
        }

        /// <summary>
        /// Override the Clear() method of the <see cref="Field"/> class
        /// </summary>
        public override void Clear()
        {
            Value = null;

        }

        /// <summary>
        /// Determines whether the node1 value and node2 value have the same value.
        /// </summary>
        /// <param name="node1">The first value</param>
        /// <param name="node2">The second value</param>
        /// <returns>True if node1 value is equal than node2 value, false otherwise</returns>
        public static bool Equal(CalendarBox node1, CalendarBox node2)
        {
            return node1.Value.Equals(node2.Value);
        }


        /// <summary>
        /// Determines whether the node1 value is greater than node2 value.
        /// </summary>
        /// <param name="node1">The first value</param>
        /// <param name="node2">The second value</param>
        /// <returns>True if the node1 value is greater than node2 value, false otherwise</returns>
        public static bool After(CalendarBox par1, CalendarBox par2)
        {
            if (par1.Value == null || par2.Value == null) 
                return false;
            return DateTime.Compare(par1.Value.Value, par2.Value.Value) > 0;
        }

    }
}
