using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Runtime.Serialization;

namespace Core.WF
{
    /// <summary>
    /// This class represent the label of a edge in the workflow.
    /// It contains precondition, actions,.. as defined:
    /// Doc/WorkflowGroup/Workflow.txt
    /// </summary>
    [Serializable]       
    public partial class WFedgeLabel:ISerializable
    {
        public int Priority { get; set; }

        private XmlDocument preconditions;
        private XmlDocument actions;
        
        
        [field: NonSerialized]
        public event EventHandler<EventArgs> EdgeModified;
        
        protected virtual void OnEdgeModified(EventArgs e)
        {
            if (EdgeModified != null)
                EdgeModified(this, e);
        }


        /// <summary>
        /// Return an XmlNode contanins the precondition of the Edge as defined:
        /// Doc/Workflowgroup/WorkflowXMLDescription/WorkflowEdge
        /// </summary>
        /// <returns>the XmlDocument so defined</returns>
        public XmlDocument GetPrecondition() { return preconditions; }

        /// <summary>
        /// Not still defined!! as shown in: Doc/Workflowgroup/WorkflowXMLDescription/WorkflowEdge
        /// </summary>
        /// <returns>Not still defined!!</returns>
        public XmlDocument GetActions() { return actions; }

        /// <summary>
        /// Not still defined!! as shown in: Doc/Workflowgroup/WorkflowXMLDescription/WorkflowEdge
        /// </summary>
        /// <returns>Not still defined!!</returns>
        public XmlDocument GetEntireDescription() {
            /*Appending preconditions*/
            return preconditions;
        }


        /// <summary>
        /// Return true if the edge could be traversed, false otherwise. 
        /// This valutation is done having the previous node (filled)
        /// </summary>
        /// <param name="previousNode"></param>
        /// <returns>Control </returns>
        internal bool VerifyPrecondition(WFnode previousNode) {          
            if (EdgeLabelInterpreter.InterpretPreconditions(previousNode.Value, preconditions.DocumentElement))
                return true;
            return false;
        }


        
        public WFedgeLabel()
        {
        }

        public WFedgeLabel(XmlDocument preconditions, XmlDocument actions)
        {
            this.preconditions = preconditions;
            this.actions = actions;
        }
/// <summary>
/// NOT still defined!!
/// </summary>
/// <param name="nextNode"></param>
/// <param name="previousNode"></param>
/// <returns></returns>
        internal WFnode ExecuteActions(WFnode nextNode, WFnode previousNode) { return null; }


        #region ISerializable Members

        protected WFedgeLabel(SerializationInfo info, StreamingContext context)
        {
            XmlDocument doc = new XmlDocument();
            doc.InnerXml = info.GetString("Preconditions");
            preconditions = doc;

        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Preconditions", preconditions.InnerXml);
        }

        #endregion
 
    }
}
