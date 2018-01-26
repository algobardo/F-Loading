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
    /// This class represents a <see cref="BoolBox"/> object that 
    /// enables user to set a box representing a boolean choice setting it
    /// to false or true otherwise
    /// </summary>
    [FieldAttribute("CheckBoxField")]
    public class BoolBox : Field
    {
        public bool? Value
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new object from the the <see cref="BoolBox"/> class.
        /// </summary>
        /// <param name="typeName">The name of the <see cref="BoolBox"/> instance in the workflow.</param>
        /// <param name="elementName">The name of the element in the workflow.</param>
        /// <param name="schema">The schema xml that describe the BoolBox type.</param>
        public BoolBox(String typeName, String elementName, XmlSchemaSet schema)
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
            Value = false;
        }

        /// <summary>
        /// Override the ToString() method of the <see cref="Field"/> class
        /// </summary>
        public override string ToString()
        {
            return Value == null? "" : Value.ToString();
        }

        /// <summary>
        /// Converts the current <see cref="BoolBox"/> object into an <see cref="XmlElement"/> object
        /// </summary>
        public override XmlElement ToXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement(FieldName));
            doc.FirstChild.AppendChild(doc.CreateElement("Value"));

            if(Value != null)
                doc.FirstChild.FirstChild.AppendChild(doc.CreateTextNode(Value.ToString().ToLower()));

            return doc.FirstChild as XmlElement;
        }

        /// <summary>
        /// Restore a BoolBox element setting the correct element's state.
        /// </summary>
        /// <param name="element">The element to be restored.</param>
        public override void FromXml(XmlElement element)
        {
            FieldName = element.Name;
            if (element.FirstChild.HasChildNodes)
                Value = bool.Parse((element.FirstChild.FirstChild as XmlText).Value.Trim());
            else
                Value = null;
        }

        /// <summary>
        /// Parse the state of a BoolBox object from a String
        /// </summary>
        /// <param name="element">The String containing the value</param>
        public override void FromString(String element)
        {
            try
            {
                bool value = Boolean.Parse(element);
                Value = value;
            }
            catch (Exception)
            {
                Value = null;
            }
        }        

        #region Operators

        /// <summary>
        /// Return true if a BoolBox element is checked, false otherwise.    
        /// </summary>
        /// <param name="par1">The element to be analyzed</param>
        public static bool IsChecked(BoolBox par1)
        {
            return par1.Value != null && par1.Value.Value;
        }

        /// <summary>
        /// Implements the AND operation between two BoolBox elements
        /// </summary>
        /// <param name="par1">The first Boolbox object</param>
        /// <param name="par2">The second one</param>
        public static BoolBox And(BoolBox par1, BoolBox par2)
        {
            BoolBox field = new BoolBox("BoolBox", "BoolBox", null);
            field.Value = par1.Value.Value && par2.Value.Value;
            return field;
        }

        /// <summary>
        /// Implements the OR operation between two BoolBox elements
        /// </summary>
        /// <param name="par1">The first Boolbox object</param>
        /// <param name="par2">The second one</param>
        public static BoolBox Or(BoolBox par1, BoolBox par2)
        {
            BoolBox field = new BoolBox("BoolBox", "BoolBox", null);
            field.Value = par1.Value.Value || par2.Value.Value;
            return field;
        }

        /// <summary>
        /// Implements the negation of a BoolBox element
        /// </summary>
        /// <param name="par1">The element to negate</param>
        public static BoolBox Not(BoolBox par1)
        {
            BoolBox field = new BoolBox("BoolBox", "BoolBox", null);
            field.Value = !par1.Value.Value;
            return field;
        }

        #endregion
    }
}
