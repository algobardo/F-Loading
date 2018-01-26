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
    /// This class represents a <see cref="RatingBox"/> object that 
    /// allows user to evaluate form's elements int values in a apposite field
    /// </summary>
    [FieldAttribute("Rating")]
    public class RatingBox : Field
    {
        /// <summary>
        /// Number of the selected stars
        /// </summary>
        private int selectedIndex;

        /// <summary>
        /// Interval of values related to rating
        /// </summary>
        public String[] Values
        {
            get;
            private set;
        }

        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                if (value >= 0 && value < Values.Length)
                    selectedIndex = value;
            }
        }

            /// <summary>
            /// Initializes a new object from the the <see cref="RatingBox"/> class.
            /// </summary>
            /// <param name="typeName">The name of the <see cref="RatingBox"/> instance in the workflow.</param>
            /// <param name="elementName">The name of the element in the workflow.</param>
            /// <param name="schema">The schema xml that describe the IntBox type.</param>
            public RatingBox(String typeName, String elementName, XmlSchemaSet schema)
            {
                this.TypeName = typeName;
                this.FieldName = elementName;
                this.Schema = schema;

                List<String> values = new List<string>();

                //Instatiate all the possible values
                XmlSchemaComplexType myType = schema.GlobalTypes[new XmlQualifiedName(typeName)] as XmlSchemaComplexType;
                XmlSchemaComplexContentExtension ext = myType.ContentModel.Content as XmlSchemaComplexContentExtension;
                XmlSchemaSequence seq = ext.Particle as XmlSchemaSequence;
                foreach (XmlSchemaObject subf in seq.Items)
                {
                    XmlSchemaElement element = subf as XmlSchemaElement;
                    XmlSchemaSimpleType type = element.SchemaType as XmlSchemaSimpleType;
                    XmlSchemaSimpleTypeRestriction restriction = type.Content as XmlSchemaSimpleTypeRestriction;
                    XmlSchemaMinInclusiveFacet min = restriction.Facets[0] as XmlSchemaMinInclusiveFacet;
                    XmlSchemaMaxInclusiveFacet max = restriction.Facets[1] as XmlSchemaMaxInclusiveFacet;
                    int minValue = Int32.Parse(min.Value);
                    int maxValue = Int32.Parse(max.Value);

                    for (int i = minValue; i <= maxValue; i++)
                        values.Add(i.ToString());

                    Values = values.ToArray();

                    selectedIndex = 0;
                }
            }

            /// <summary>
            /// Overrides the Clear() method of the <see cref="Field"/> class
            /// </summary>
            public override void Clear()
            {
                selectedIndex = 0;
            }

            /// <summary>
            /// Overrides the ToString() method of the <see cref="Field"/> class to
            /// return the value of the <see cref="RatingBox"/> as a <see cref="String"/>
            /// </summary>
            public override string ToString()
            {
                return Values[selectedIndex];
            }

            /// <summary>
            /// Converts the current <see cref="RatingBox"/> object into an <see cref="XmlElement"/> object
            /// </summary>
            public override XmlElement ToXml()
            {
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateElement(FieldName));
                doc.FirstChild.AppendChild(doc.CreateElement("Value"));

                doc.FirstChild.FirstChild.AppendChild(doc.CreateTextNode(Values[selectedIndex]));

                return doc.FirstChild as XmlElement;
            }

            /// <summary>
            /// Restore a <see cref="RatingBox"/> element setting the correct state of the element.
            /// </summary>
            /// <param name="element">The element to be restored.</param>
            public override void FromXml(XmlElement element)
            {
                FieldName = element.Name;
                String value = (element.FirstChild.FirstChild as XmlText).Value.Trim();
                if (element.FirstChild.HasChildNodes)
                {
                    for (int i = 0; i < Values.Length; i++)
                        if (Values[i] == value)
                            selectedIndex = i;
                }
                else
                    selectedIndex = 0;
            }

            /// <summary>
            /// Parse the state of the <see cref="RatingBox"/> object from a string
            /// </summary>
            /// <param name="element">The string containing the value</param>
            public override void FromString(String element)
            {
            }

            #region Operators
            /// <summary>
            /// Implements the Equal operation between two <see cref="RatingBox"/> returning true 
            /// if the elements have the same value, false otherwise
            /// </summary>
            /// <param name="par1">The first element to analize</param>
            /// <param name="par2">The second one</param>
            /// <param name="par1Index">The value's index referred to the first element</param>
            /// <param name="par2Index">The value's index referred to the second element</param>
            /// <returns>true if the <see cref="Ratingbox"/>values is the same false otherwise</returns>
            public static bool Equal(RatingBox par1, RatingBox par2,int par1Index,int par2Index)
            {
                return Int32.Parse(par1.Values[par1Index]) == Int32.Parse(par1.Values[par2Index]);
            }

            /// <summary>
            /// Implements the Greater operation between two <see cref="RatingBox"/> returning true
            /// if the first element is greater than the second, false otherwise
            /// </summary>
            /// <param name="par1">The first element to analize</param>
            /// <param name="par2">The second one</param>
            /// <param name="par1Index">The value's index referred to the first element</param>
            /// <param name="par2Index">The value's index referred to the second element</param>
            /// <returns>true if the value of the first <see cref="RatingBox"/> is greater than
            /// the second, false otherwise</returns>
            public static bool Greater(RatingBox par1, RatingBox par2, int par1Index, int par2Index)
            {
                return Int32.Parse(par1.Values[par1Index]) > Int32.Parse(par1.Values[par2Index]);
            }

            /// <summary>
            /// Implements the Greater operation between two <see cref="RatingBox"/> returning true
            /// if the first element is greater than the second, false otherwise
            /// </summary>
            /// <param name="par1">The first element to analize</param>
            /// <param name="par2">The second one</param>
            /// <param name="par1Index">The value's index referred to the first element</param>
            /// <param name="par2Index">The value's index referred to the second element</param>
            /// <returns>true if the value of the second <see cref="RatingBox"/> is greater than
            /// the first, false otherwise</returns>
            public static bool Less(RatingBox par1, RatingBox par2, int par1Index, int par2Index)
            {
                return Int32.Parse(par1.Values[par1Index]) < Int32.Parse(par1.Values[par2Index]);
            }
            #endregion;
        }
}
