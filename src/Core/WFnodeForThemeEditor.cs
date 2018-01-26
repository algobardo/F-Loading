using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fields;
using System.Xml.Schema;
using System.Xml;

namespace Core.WF {
    public partial class WFnode {


        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="nodeName">Node name, not encoded.</param>
        public WFnode(string nodeName) {
            renderDocument = new XmlDocument();


            //Loading the base types
            myBaseTypes = Fields.FieldsManager.FieldTypesXSD;

            //Creating the node's type
            XmlSchemaComplexType nodeType = new XmlSchemaComplexType();
            this.nodeTypeName = nodeType.Name = XmlConvert.EncodeLocalName(nodeName);
            nodeType.Particle = new XmlSchemaSequence();

            //Creating a schema that contains the node's type
            XmlSchema nodeSchema = new XmlSchema();
            nodeSchema.Items.Add(nodeType);

            //Creating the node's inner types schema set 
            nodeSchemas = new XmlSchemaSet();
            nodeSchemas.Add(nodeSchema);
            nodeSchemas.Compile();

        }         
    }
}
