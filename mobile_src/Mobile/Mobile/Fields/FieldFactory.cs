using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;
using System.Drawing;
using Mobile.Fields.Base;
using Mobile.Fields.Group;
using System.IO;

namespace Mobile.Fields
{
    /// <summary>
    /// The <see cref="FieldFactory"/> class
    /// </summary>
    public class FieldFactory
    {
        /// <summary>
        /// This class contains all the information needed to instatiate e new fiell representing a type and the associated controller
        /// </summary>
        private class FieldControllerPair
        {
            public String TypeName
            {
                get;
                set;
            }
            public ConstructorInfo FieldConstructor
            {
                get;
                set;
            }
            public ConstructorInfo FieldControllerConstructor
            {
                get;
                set;
            }
        }

        /// <summary>
        /// A <see cref="Dictionary"/> element contains an association between a string representing the name of a type and all nformations necessaries to it's representation
        /// </summary>
        private static Dictionary<String, FieldControllerPair> fieldControllerManagers;

        /// <summary>
        /// 
        /// </summary>
        static FieldFactory()
        {
            fieldControllerManagers = new Dictionary<string, FieldControllerPair>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.Namespace != null && type.Namespace.StartsWith("Mobile.Fields"))
                {                    
                    ConstructorInfo fieldConstructor = type.GetConstructor(new Type[] { typeof(String), typeof(String), typeof(XmlSchemaSet)});
                                    
                    foreach (object obj in type.GetCustomAttributes(typeof(FieldAttribute), true))
                    {
                        FieldAttribute attribute = obj as FieldAttribute;
                        if (!fieldControllerManagers.ContainsKey(attribute.TypeName))
                        {
                            fieldControllerManagers[attribute.TypeName] =
                                new FieldControllerPair()
                                {
                                    TypeName = attribute.TypeName,
                                    FieldConstructor = fieldConstructor
                                };
                        }
                        else
                            fieldControllerManagers[attribute.TypeName].FieldConstructor = fieldConstructor;
                    }
                }
            }
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.Namespace != null && type.Namespace.StartsWith("Mobile.Fields"))
                {
                    foreach (object obj in type.GetCustomAttributes(typeof(ControllerAttribute), true))
                    {
                        ControllerAttribute attribute = obj as ControllerAttribute;

                        bool found = false;
                        Dictionary<string, FieldControllerPair>.ValueCollection values = fieldControllerManagers.Values;
                        for (int i = 0; !found && i < values.Count; i++)
                        {
                            if (values.ElementAt(i).FieldConstructor.DeclaringType == attribute.FieldType)
                            {
                                ConstructorInfo controllerConstructor = type.GetConstructor(new Type[] { attribute.FieldType, typeof(bool) });
                                fieldControllerManagers[values.ElementAt(i).TypeName].FieldControllerConstructor = controllerConstructor;
                            }
                        }
                    }
                }
            }
        }

        public static XmlSchema Types
        {
            get;
            set;
        }


        /// <summary>
        /// Returns the TypeManager for the type t
        /// </summary>
        /// <param name="t">The type of the TypeManager</param> 
        /// <returns>The TypeManager of type t if it is contained in the Dictionary, null otherwise</returns>
        public static Field CreateField(XmlSchemaElement schema)
        {
            Field element = null;
            try
            {
                XmlSchemaType type = schema.ElementSchemaType;

                if (type == null)
                    type = schema.SchemaType;
                if (type == null)
                    type = Types.SchemaTypes[schema.SchemaTypeName] as XmlSchemaType;

                if (type.Name != null)
                    element = CreateField(type.Name, schema.Name, null);
                else
                {
                    if (type is XmlSchemaComplexType && (type as XmlSchemaComplexType).Particle is XmlSchemaChoice)
                    {
                        XmlSchema onTheFlySchema = new XmlSchema();
                        onTheFlySchema.Items.Add(schema);

                        element = CreateField("ChoiceBox", schema.Name, onTheFlySchema);
                    }
                    if (type is XmlSchemaComplexType && (type as XmlSchemaComplexType).Particle is XmlSchemaSequence)
                    {
                        if (schema.MinOccurs <= schema.MaxOccurs)
                        {
                            XmlSchema onTheFlySchema = new XmlSchema();
                            String onTheFlyName = "Sequence" + new Random().Next().ToString();

                            XmlSchemaElement baseElement = new XmlSchemaElement();
                            XmlSchemaComplexType baseType = new XmlSchemaComplexType();
                            XmlSchemaSequence baseSequence = new XmlSchemaSequence();
                            baseSequence.Items.Add(schema);

                            baseType.Particle = baseSequence;
                            baseElement.SchemaType = baseType;

                            onTheFlySchema.Items.Add(baseElement);

                            baseElement.Name = onTheFlyName;

                            element = CreateField("SequenceBox", onTheFlyName, onTheFlySchema);
                        }
                    }
                }
            }
            catch (ArgumentException)
            {
                return null;
            }

            return element;
        }

        /// <summary>
        /// Instatiate a new <see cref="Field"/> element
        /// </summary>
        /// <param name="typeName">The name of the class that rapresents the generic type of the instance.</param>
        /// <param name="elementName">the instance's name</param>
        /// <param name="elementSchema">The schema xml that describe the type represented by these field.</param>
        /// <returns>The instantiated <see cref="Field"/> object</returns>
        public static Field CreateField(string typeName, String elementName, XmlSchema elementSchema)
        {
            String managerTypeName = null;
            XmlSchemaSet customSchema = null;

            if (elementSchema == null)
            {
                XmlSchemaType baseTypeName = GetBaseType(typeName);
                customSchema = BuildCustomSchema(typeName, elementName);
                managerTypeName = baseTypeName.Name;
            }
            else
            {
                customSchema = BuildCustomSchema(elementSchema);
                managerTypeName = typeName;
            }

            if (managerTypeName != null)
            {
                ConstructorInfo constructor = fieldControllerManagers[managerTypeName].FieldConstructor;
                if (constructor != null)
                    return constructor.Invoke(new object[] { typeName, elementName, customSchema }) as Field;
            }
            return null;
        }

        /// <summary>
        /// Instantiate a <see cref="FieldController"/> element related to the specified input Field object
        /// </summary>
        /// <param name="element">The <see cref="Field"/> element</param>
        /// <param name="enabled">the <see cref="Field"/> will be editable if true, not editable otherwise</param>
        /// <returns>the <see cref="FieldController"/> object</returns>
        public static FieldController CreateController(Field element, bool enabled)
        {
            XmlSchemaType baseType = GetBaseType(element.TypeName);
            String typeName = element.TypeName;
            if (baseType != null)
                typeName = baseType.Name;

            ConstructorInfo constructor = fieldControllerManagers[typeName].FieldControllerConstructor;
            if (constructor != null)
                return constructor.Invoke(new object[] { element, enabled }) as FieldController;
            
            return null;
        }

        /// <summary>
        /// This method return the baseSchemaType of a base type retrieving it form the Type propertiy
        /// </summary>
        /// <param name="typeName">the string representing the type's name</param>
        /// <returns>the type's <see cref="XMLSchemaType"/></returns>
        private static XmlSchemaType GetBaseType(String typeName)
        {
            XmlSchemaComplexType schemaType = Types.SchemaTypes[new XmlQualifiedName(typeName)] as XmlSchemaComplexType;
            if (schemaType == null)
                return null;

            XmlSchemaType baseSchemaType = schemaType.BaseXmlSchemaType;
            if (baseSchemaType.Name == null)
                return schemaType;

            return baseSchemaType;
        }

        /// <summary>
        /// Build the <see cref="XmlSchemaSet"/> of a type 
        /// </summary>
        /// <param name="typeName">the name of the type</param>
        /// <param name="elementName">the name of the instance</param>
        /// <returns>the <see cref="XmlSchemaSet"/> of a type returned by the <see cref="BuildCustomSchema"/> method</returns>
        private static XmlSchemaSet BuildCustomSchema(String typeName, string elementName)
        {
            XmlSchema elementSchema = new XmlSchema();
            XmlSchemaElement elementDefinition = new XmlSchemaElement();
            elementSchema.Items.Add(elementDefinition);
            elementDefinition.Name = elementName;
            elementDefinition.SchemaTypeName = new XmlQualifiedName(typeName, elementSchema.TargetNamespace);

            return BuildCustomSchema(elementSchema);
        }

        /// <summary>
        /// This method returns a <see cref="XmlSchemaSet"/> containing the validation's necessary information
        /// </summary>
        /// <param name="elementSchema">the <see cref="XmlSchema"/> of a type</param>
        /// <returns>the <see cref="XmlSchemaSet"/> of a specified type</returns>
        private static XmlSchemaSet BuildCustomSchema(XmlSchema elementSchema)
        {
            XmlSchemaSet customSchema = new XmlSchemaSet();
            customSchema.Add(Types);
            customSchema.Add(elementSchema);
            customSchema.Compile();

            return customSchema;
        }
    }
}
