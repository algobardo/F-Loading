using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Schema;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Collections;
using System.IO;
using Fields;
using System.Reflection;

namespace Core.WF
{
    [Serializable]
    public partial class WFnode:ISerializable,IField
    {
        protected XmlSchemaSet nodeSchemas;

        protected XmlSchema myBaseTypes;

        protected string nodeTypeName;

        protected XmlDocument renderDocument;       

        [field:NonSerialized]
        public event EventHandler<EventArgs> FieldModified;
        [field: NonSerialized]
        public event EventHandler<EventArgs> NodeNameModified;

        protected virtual void OnFieldModified(EventArgs e)
        {
            if (FieldModified != null)
                FieldModified(this, e);
        }

        protected virtual void OnNodeNameModified(EventArgs e)
        {
            if (NodeNameModified != null)
                NodeNameModified(this, e);
        }

        public string NodeTypeName
        {
            get
            {
                return XmlConvert.DecodeName(nodeTypeName);
            }
            set
            {
                nodeTypeName = XmlConvert.EncodeLocalName(value);
                var tp = (XmlSchemaComplexType)nodeSchemas.GlobalTypes[new XmlQualifiedName(nodeTypeName)];
                tp.Name = nodeTypeName;
                foreach (XmlSchema s in nodeSchemas.Schemas())
                {
                    nodeSchemas.Reprocess(s);
                }
                OnNodeNameModified(null);
            }
        }

        public bool isValid
        {
            get
            {                
                try
                {
                    nodeSchemas.Add(this.myBaseTypes);
                    nodeSchemas.Compile();
                    return !string.IsNullOrEmpty(nodeTypeName) && nodeSchemas.GlobalTypes[new XmlQualifiedName(nodeTypeName)] != null;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Construct a WFnode clone of ndClone.
        /// </summary>
        /// <param name="ndClone">The node to clone.</param>
        public WFnode(WFnode ndClone)
        {
            this.nodeSchemas = ndClone.nodeSchemas;
            this.myBaseTypes = ndClone.myBaseTypes;
            this.nodeTypeName = ndClone.nodeTypeName;
            this.renderDocument = ndClone.renderDocument;

        }
        /// <summary>
        /// Costruct a Workflow Node starting from types and node description (the structure).
        /// Base types schema is implicit
        /// </summary>
        /// <param name="typesSchema">Schemas with extended type definitions and node complex type definition</param>
        /// <param name="nodeType">The complex type definition of node in the SchemaSet</param>     
        
        public WFnode(XmlSchemaSet typesSchema, XmlSchemaComplexType nodeType)
        {

            this.nodeTypeName = nodeType.Name;

            myBaseTypes = Fields.FieldsManager.FieldTypesXSD;
                      
            nodeSchemas = new XmlSchemaSet();
            nodeSchemas.Add(typesSchema);

        }

        private WFnode():this("WfNode")
        {
            
        }

        /// <summary>
        /// Returns a <c>Compiled</c> XMLSchemaSet contaning 
        /// extended types definitions, node type definition (his structure), base types definitions.
        /// As defined in:
        /// /Doc/FieldsGroup/TypesPuzzle/nodeDescription.xsd (up to the first element)
        /// /Doc/FieldsGroup/TypesPuzzle/typesAssociatedEachNode.xsd
        /// </summary>
        /// <returns>The XmlSchemaSet so defined</returns>
        public XmlSchemaSet GetNodeSchemas()
        {
            nodeSchemas.Add(this.myBaseTypes);
            nodeSchemas.Compile();
            return nodeSchemas;
        }

        public XmlDocument GetRenderingDocument()
        {
            return renderDocument;
        }

        /// <summary>
        /// Returns a <c>Un-Compiled</c> XMLSchemaSet contaning 
        /// extended types definitions, node type definition (his structure).
        /// As defined in:
        /// /Doc/FieldsGroup/TypesPuzzle/nodeDescription.xsd (up to the first element)
        /// /Doc/FieldsGroup/TypesPuzzle/typesAssociatedEachNode.xsd
        /// </summary>
        /// <returns>The XmlSchemaSet so defined</returns>
        public XmlSchemaSet GetNodeSchemasWithoutBaseTypes()
        {
            nodeSchemas.RemoveRecursive(myBaseTypes);
            return nodeSchemas;
        }

        /// <summary>
        /// Review the node information
        /// </summary>
        /// <param name="newSchemas">New schema for node</param>
        /// <param name="newRenderingDocument">New information to render the node</param>
        /// <param name="nodeTypeName">New Type Name, Encoded.</param>
        public void ModifyNode(XmlSchemaSet newSchemas, XmlDocument newRenderingDocument, string nodeTypeName)
        {
            XmlConvert.VerifyNCName(nodeTypeName);         
            nodeSchemas = newSchemas;
            renderDocument = newRenderingDocument;
            this.nodeTypeName = nodeTypeName;
            nodeSchemas.Add(this.myBaseTypes);
            nodeSchemas.Compile();
            OnFieldModified(null);
            OnNodeNameModified(null);
        }

        public void ModifyNode(XmlDocument newRenderingDocument)
        {
            renderDocument = newRenderingDocument;
        }
      
        #region ISerializable Members

        protected WFnode(SerializationInfo info, StreamingContext context)
        {
            Utils.DeSerializeGeneric(this, info, context);
            this.myBaseTypes = FieldsManager.FieldTypesXSD;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.nodeSchemas.RemoveRecursive(this.myBaseTypes);
            Utils.SerializeGeneric(this, info, context);
        }

        #endregion

        #region IRenderable Members

        public WebControl GetWebControl()
        {
            throw new NotImplementedException();
           
        }
        
        public string GetValue(Control src,XmlNode renderingDocument)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<" + nodeTypeName + ">");
            
            var cmplxTypeSchema = (XmlSchemaComplexType)this.GetNodeSchemas().GlobalTypes[new XmlQualifiedName(nodeTypeName)];

            sb.Append(
                FieldsManager.GetInstance(
                renderingDocument,cmplxTypeSchema,GetNodeSchemas()).GetValue(((NamingUpdatePanel)src).ContentTemplateContainer.Controls[0],renderingDocument));

            sb.Append("</" + nodeTypeName + ">");

            return sb.ToString();
        }

        public Control GetWebControl(HttpServerUtility server, XmlNode renderingDocument)
        {
            NamingUpdatePanel np = new NamingUpdatePanel();
            
            np.ID = this.NodeTypeName;

            var cmplxType = renderingDocument;
            var cmplxTypeSchema = (XmlSchemaComplexType)this.GetNodeSchemas().GlobalTypes[new XmlQualifiedName(nodeTypeName)];
            var fld = FieldsManager.GetInstance(cmplxType, cmplxTypeSchema, this.GetNodeSchemas());
            
            if (IsWfNodeInstance && nodeIstance.DocumentElement.SchemaInfo != null)
                ((IField)(fld)).SetValue(nodeIstance.DocumentElement.ChildNodes.ToList());
            else if (IsWfNodeInstance && nodeIstance.DocumentElement.SchemaInfo == null)
            {
                throw new InvalidOperationException("Value not conforming the schema!");
            }
                
            np.ContentTemplateContainer.Controls.Add(fld.GetWebControl(server,cmplxType));
            return np;
        }

        public List<BaseValidator> GetValidators()
        {
            throw new NotImplementedException();
        }

        public List<BaseValidator> GetValidators(Control controlId)
        {
            throw new NotImplementedException();
        }

        public string JSON_StyleProperties
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        public override string ToString()
        {
            return this.NodeTypeName + "\r" + Utils.WriteSchemaSet(nodeSchemas) + "\r" + renderDocument.OuterXml;
        }



        #region IField Members

        public void SetValue(List<XmlNode> nds)
        {
            Value = nds[0];
        }

        /// <summary>
        /// Please note, this method (setting) try to validate passed node with the node schema.
        /// </summary>
        public XmlNode Value
        {
            get
            {
                if (nodeIstance == null)
                    throw new InvalidOperationException("This is not a node instance");
                return nodeIstance.DocumentElement;
            }
            set
            {
                if (value != null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.AppendChild(doc.ImportNode(value, true));
                    nodeIstance = doc;

                    this.Validate(null);
                }
                else
                    nodeIstance = null;
            }
        }

        public string Name
        {
            get
            {
                return NodeTypeName;
            }
            set
            {
                NodeTypeName = value;
            }
        }

        public void setExampleValue()
        {
            throw new NotImplementedException();
        }

        #endregion
    
        private XmlDocument nodeIstance = null;

        #region trash2
        /*Example
           <?xml version="1.0" encoding="utf-8" ?> 

- <Node1>
- <Anni>
  <Valore>5</Valore> 
  </Anni>
- <PosizioneInItalia>
  <LatitudeField>100</LatitudeField> 
  <LongitudeField>100</LongitudeField> 
  </PosizioneInItalia>
  </Node1>*/
        #endregion

       
        private class errorHandler
        {
            private ValidationEventHandler handler;
            public bool valid;

            public errorHandler(ValidationEventHandler handler)
            {
                this.handler = handler;
                valid = true;
            }

            public void handlerError(object sender, ValidationEventArgs e)
            {
                if (valid)
                    valid = false;
                if (handler != null && handler.GetInvocationList().Length > 0)
                    handler(sender, e);

            }
        }


        public bool IsWfNodeInstance
        {
            get
            {
                return nodeIstance != null;
            }
        }

        /// <summary>
        /// Validate the XmlNode with WFnode schema
        /// </summary>
        /// <param name="handler">The handler that XmlDocument.Validate() call if the xml is not valid</param>
        public bool Validate(ValidationEventHandler handler)
        {
            if(nodeIstance==null)
                throw new InvalidOperationException("This is not a node instance");

            errorHandler eh = new errorHandler(handler);

            XmlSchemaSet allSchemas = new XmlSchemaSet();
            allSchemas.Add(nodeSchemas);
            allSchemas.Add(myBaseTypes);

            XmlSchema elementSchema = new XmlSchema();

            XmlSchemaElement elm = new XmlSchemaElement();
            elm.Name = nodeTypeName;
            elm.SchemaTypeName = new XmlQualifiedName(nodeTypeName);
            elementSchema.Items.Add(elm);

            allSchemas.Add(elementSchema);

            nodeIstance.Schemas = allSchemas;

            nodeIstance.Validate(eh.handlerError);

            return eh.valid;

        }

    }

}


