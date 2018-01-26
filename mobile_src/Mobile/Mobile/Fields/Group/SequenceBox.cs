using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.Xml.Schema;
using Mobile.Fields;

namespace Mobile.Fields.Group
{
    /// <summary>
    /// This class represents a <see cref="SequenceBox"/> object that 
    /// contains a list of elements. It enables the user to add new 
    /// elements, move and delete the allready elements.
    /// </summary>
    [FieldAttribute("SequenceBox")]
    public class SequenceBox : FieldGroup
    {
        /// <summary>
        /// List of type that forms the SequenceBox instance
        /// </summary>
        private List<Field> fields;


        private XmlSchemaElement childSchema;

        public override Field[] Fields
        {
            get
            {
                return fields.ToArray();
            }
        }

        public XmlElement FieldsPresentation
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceBox"/> class.
        /// </summary>
        /// <param name="typeName">The name of the class that rapresents the generic type of the instance.</param>
        /// <param name="elementName">The name of the <see cref="SequenceBox"/> instance in the presentation.xml file of the workflow.</param>
        /// <param name="schema">The schema xml that describe the <see cref="SequenceBox"/> type.</param>
        public SequenceBox(String typeName, String elementName, XmlSchemaSet schema)
            : base(typeName, elementName, schema)
        {
            fields = new List<Field>();

            XmlSchemaComplexType myType = (schema.GlobalElements[new XmlQualifiedName(elementName)] as XmlSchemaElement).SchemaType as XmlSchemaComplexType;
            XmlSchemaSequence seq = myType.Particle as XmlSchemaSequence;
            XmlSchemaElement element = seq.Items[0] as XmlSchemaElement;

            element.MinOccursString = null;
            element.MaxOccursString = null;

            childSchema = element;
        }

        public override void Clear()
        {
            base.Clear();
            fields.Clear();
        }


        /// <summary>
        /// Restore the state of the element.
        /// </summary>
        /// <param name="element">The element to be restored.</param>
        public override void FromXml(XmlElement element)
        {
            fields.Clear();
            foreach(XmlElement child in element.ChildNodes)
            {
                Field f = CreateField(childSchema);
                if (f == null)
                    throw new ArgumentException(childSchema.SchemaType != null ? "The type " + childSchema.SchemaType.Name + " is not supported" : "One of the children is not supported");
                
                f.FromXml(child);
                fields.Add(f);
            }
        }

        private Field CreateField(XmlSchemaElement schema)
        {
            XmlSchema onTheFlySchema = new XmlSchema();
            onTheFlySchema.Items.Add(schema);

            return FieldFactory.CreateField("GroupBox", schema.Name, onTheFlySchema);
        }

        /// <summary>
        /// Recuperates the rendering informations to display the type
        /// </summary>
        /// <param name="presentation">The element to be parsed</param>
        public override void ParsePresentation(XmlElement presentation)
        {
            base.ParsePresentation(presentation);
            if (presentation.HasChildNodes && presentation.FirstChild.HasChildNodes)
            {
                foreach (Field child in Fields)
                    child.ParsePresentation(presentation);

                FieldsPresentation = presentation as XmlElement;
            }
        }

        /// <summary>
        /// Manages the addition of new elements to the list of fields
        /// </summary>
        public void AddNew()
        {
            Field f = CreateField(childSchema);

            if(f == null)
                throw new ArgumentException(childSchema.SchemaType != null ? "The type " + childSchema.SchemaType.Name + " is not supported" : "One of the children is not supported");
                
            f.ParsePresentation(FieldsPresentation);
            fields.Add(f);
        }

        /// <summary>
        /// Maneges the displacement of a field in the list.
        /// </summary>
        /// <param name="index">The index of the field to be moved.</param>
        /// <param name="newIndex">The new index where to insert the field.</param>
        public void Move(int index, int newIndex)
        {
            if (index >= 0 && index < fields.Count && newIndex >= 0 && newIndex < fields.Count - 1)
            {
                Field field = fields[index];
                fields.RemoveAt(index);
                fields.Insert(newIndex, field);
            }
        }

        /// <summary>
        /// Maneges the displacement of a field in the list.
        /// </summary>
        /// <param name="index">The index of the field to be removed.</param>
        public Field Remove(int index)
        {
            if (index >= 0 && index < fields.Count)
            {
                Field field = fields[index];
                fields.RemoveAt(index);
                return field;
            }
            return null;
        }
    }
}
