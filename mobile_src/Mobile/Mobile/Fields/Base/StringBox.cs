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
using Mobile.Fields;

namespace Mobile.Fields.Base
{
    /// <summary>
    /// This class represent <see cref="StringBox"/> element that allows
    /// user to insert strings into an apposite field
    /// </summary>
    [FieldAttribute("StringBox")]
    public class StringBox : Field
    {
        public String Value
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBox"/> class
        /// </summary>
        /// <param name="typeName">The name of the <see cref="StringBox"/> instance in the workflow.</param>
        /// <param name="elementName">The name of the element in the workflow.</param>
        /// <param name="schema">The schema xml that describe the BoolBox type.</param>
        public StringBox(String typeName, String elementName, XmlSchemaSet schema)
        {
            this.TypeName = typeName;
            this.FieldName = elementName;
            this.Schema = schema;
        }

        /// <summary>
        /// Override the Field's class ToString() method which returns the value of
        /// a StringBox as a string
        /// </summary>
        /// <returns>the value of the StringBox that invokes it</returns>
        public override string ToString()
        {
            return Value == null ? "" : Value;
        }

        /// <summary>
        /// Converts the current <see cref="StringBox"/> object into an <see cref="XmlElement"/> object
        /// </summary>
        /// <returns>an <see cref="XmlElement"/> rapresenting the <see cref="StringBox"/></returns>
        public override XmlElement ToXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement(FieldName));
            doc.FirstChild.AppendChild(doc.CreateElement("Value"));
            
            if(Value != null)
                doc.FirstChild.FirstChild.AppendChild(doc.CreateTextNode(Value));

            return doc.FirstChild as XmlElement;
        }

        /// <summary>
        /// Restore a StringBox element setting the correct element's state.
        /// </summary>
        /// <param name="element">The element to be restored.</param>
        public override void FromXml(XmlElement element)
        {
            FieldName = element.Name;
            if(element.FirstChild.HasChildNodes)
                Value = (element.FirstChild.FirstChild as XmlText).Value.Trim();
            else
                Value = null;
        }

        /// <summary>
        /// Parse the state of a StringBox object from a String
        /// </summary>
        /// <param name="element">The String containing the value</param>    
        public override void FromString(String element)
        {
            Value = element;
        }

        /// <summary>
        /// Override the Clear() method of the <see cref="Field"/> class
        /// </summary>
        public override void Clear()
        {
            Value = null;
        }

        #region Operators

        /// <summary>
        /// Determines whether the node1 value and node2 value have the same value.
        /// </summary>
        /// <param name="node1">The first value</param>
        /// <param name="node2">The second value</param>
        /// <returns>True if node1 value is equal than node2 value, false otherwise</returns>
        public static bool Equal(StringBox node1, StringBox node2)
        {
            return node1.Value == node2.Value;
        }


        /// <summary>
        /// Determines whether the node1 value is lexicographically greater than node2 value.
        /// </summary>
        /// <param name="node1">The first value</param>
        /// <param name="node2">The second value</param>
        /// <returns>True if the node1 value is lexicographically greater than node2 value, false otherwise</returns>
        public static bool Greater(StringBox node1, StringBox node2)
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
        public static IntBox Length(StringBox par)
        {
            IntBox element = new IntBox("IntBox", "IntBox", null);
            element.Value = par.Value.Length;
            return element;
        }

        #endregion
    }
}
