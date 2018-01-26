using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;

namespace Mobile.Fields
{
    public class FieldsManager
    {
        private Dictionary<string, System.Type> typeManagersTable = new Dictionary<string, Type>();

        public FieldsManager(XmlSchemaSet schema)
        {
            RegisterElementType("IntBox", typeof(IntBoxElement));
            RegisterElementType("StringBox", typeof(StringBoxElement));
            foreach (Object obj in schema.GlobalTypes.Values)
            {
                XmlSchemaComplexType type = obj as XmlSchemaComplexType;
                if (type != null && type.Name != null && type.BaseXmlSchemaType.Name != null)
                    RegisterElementType(type.Name, GetElementType(type.BaseXmlSchemaType.Name));
            }
        }


        /// <summary>
        /// Returns the TypeManager for the type t
        /// </summary>
        /// <param name="t">The type of the TypeManager</param> 
        /// <returns>The TypeManager of type t if it is contained in the Dictionary, null otherwise</returns>
        public System.Type GetElementType(string t)
        {
            if (typeManagersTable.ContainsKey(t))
                return typeManagersTable[t];

            return null;
        }


        /// <summary>
        /// Adds new TypeManager of type t to the Dictionary typeManagersTable
        /// </summary>
        /// <param name="t">The type that TypeManager tm handles</param>
        /// <param name="tm">The TypeManager to be added</param>
        public void RegisterElementType(string t, System.Type type)
        {
            if (!typeManagersTable.ContainsKey(t))
                typeManagersTable.Add(t, type);
            else
                //overwrite the last type manager
                typeManagersTable[t] = type;
        }
    }
}
