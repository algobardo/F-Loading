using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Core.WF
{
    /*TO THE GUI*/

    /// <summary>
    /// Theese flags are used to pinpoint which sort of interaction with the workflow is needed.
    /// </summary>
    public enum WFeventType
    {
        /// <summary>
        /// A general interaction that could cause transition to a new state on the workflow.
        /// In case of a previous rollback the new state could be a previous discarded one.
        /// </summary>
        TRYGOON,
        /// <summary>
        /// A general interaction that cause transition to the previous state on the workflow.
        /// </summary>
        ROLLBACK
    }
    /// <summary>
    /// This class represents the result of an interaction with workflow.
    /// </summary>
    public class ActionResult
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="ActionResult"/> is OK.
        /// </summary>
        /// <value><c>true</c> if OK; otherwise, <c>false</c>.</value>        
        public bool OK
        {
            get
            {
                return ok;
            }
        }
        private bool ok=false;

        public ActionResult()
        {
            ok = true;
        }
        public ActionResult(bool isOk)
        {
            ok = isOk;
        }
    }

    public class WorkflowDescription
    {        
        public string NodesSchema;
        public string ExtendedTypesSchema;
        public string EdgesDescription;
        public string RenderingDocument;
    }

    /// <summary>
    /// This interface defines the behaviour of a workflow during its filling lifetime.
    /// </summary>
    public interface IComputableWorkflow
    {
        string GetDescription();
        void SetDescription(string descr);

        WFnode GetState();
        bool IsFinalState();
        bool IsInitialState();
        
        void ResetComputation();

        /// <summary>
        /// returns a list of wfnode
        /// 
        /// method requested from themeeditor
        /// </summary>
        /// <returns></returns>
        IEnumerable<WFnode> getNodeList();

        List<string> GetThroughPath();

        ActionResult ComputeNewStatus(WFeventType evt, XmlDocument data, ValidationEventHandler handler);

        bool IsEndComputationState { get; }

        void setWFname(string name);

        string getWFname();

        List<string> GetNodesTypeName();
        /// <summary>
        /// Returns the filled document.
        /// This document is validated using the collected document schema.
        /// </summary>
        /// <returns>The document so defined.</returns>
        XmlDocument GetCollectedDocument();

        /// <summary>
        /// reverse the document 
        /// call this method before calling getCollectedDocument();
        /// call this metod only once!!!
        /// </summary>
        void ReverseFinalDocument();
        
        
        /// <summary>
        /// Returns schemas for final document. Theese include the schema defining complex types (for fields),
        /// the schema defining node types and the schema describing the final document structure.
        /// </summary>
        /// <returns>Schemas so defined.</returns>
        XmlSchemaSet GetCollectedDocumentSchemas();






        WorkflowDescription GetEntireWorkflowDescription();

        /// <summary>
        /// Returns a document contaning all edges of the workflow as specified in Doc/Workflowgroup/WorkflowXMLDescription/WorkflowEdges
        /// </summary>
        /// <returns>the document so defined</returns>
        XmlDocument GetEdges();
        /// <summary>
        /// return a SchemaSet that contains base types definitions, extended types definitions, node type definition and nodeSchema schemas 
        /// as shown in: 
        /// Doc/Fields/TypesPuzzle/Basetypes.xsd 
        /// Doc/Fields/TypesPuzzle/nodeDescription.xsd
        /// Doc/Fields/TypesPuzzle/typesAssociatedEachNode.xsd
        /// </summary>
        /// <returns>the schema so defined</returns>
        XmlSchemaSet GetNodesSchemas();

        XmlDocument GetXMLDocument();


    }

}
