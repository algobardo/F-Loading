using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Core.WF;
using System.Xml;

namespace WebInterface.WorkflowEditor.action {
    public class WFG_addArc : System.Web.UI.Control, ICallbackEventHandler, IControlWorkflowEditor {

        private string callbackResult;

        #region ICallbackEventHandler Members

        /// <summary>
        /// Returns callback's result
        /// </summary>
        /// <returns></returns>
        public string GetCallbackResult() {
            return callbackResult;
        }

        /// <summary>
        /// Receives client's call and adds an edge between 'Node id from' and 'Node id to'
        /// </summary>
        /// <param name="eventArgument">WorkflowId from client & Node id from & Node id to & Xml predicate</param>
        public void RaiseCallbackEvent(string eventArgument) {
            string[] args = eventArgument.Split('&');
            string wfID = args[0];
            string fromNodeName = args[1];
            string toNodeName = args[2];
            string edgeXML = args[3];

            callbackResult = "OK";

            //Registering to wf's events
            Workflow wf = (Workflow)Page.Session[wfID];
            wf.WorkflowInvalidationEvent += new EventHandler<WorkflowValidationEventArgs>(wf_WorkflowInvalidationEvent);

            if (edgeXML == "") return;

            AddEdge(wfID, fromNodeName, toNodeName, edgeXML);
        }

        void wf_WorkflowInvalidationEvent(object sender, WorkflowValidationEventArgs e) {
            if (!e.Valid)
                callbackResult = e.Message;            
                
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Adds an edge between two nodes, saving its information in Page's Session
        /// </summary>
        /// <param name="wfID">The WF's ID</param>
        /// <param name="fromNodeID">The edge's head</param>
        /// <param name="toNodeID">The edge's tail</param>
        /// <param name="edgeXML">The edge's XML value</param>
        private void AddEdge(string wfID, string fromNodeID, string toNodeID, string edgeXML) {
            WFnode from = (WFnode)Page.Session[fromNodeID];
            WFnode to = (WFnode)Page.Session[toNodeID];

            if (from == null || to == null || from == to) return;

            XmlDocument edgeDocument = new XmlDocument();
            edgeDocument.InnerXml = edgeXML;

            //TOCHANGE
            Workflow wf = (Workflow)Page.Session[wfID];
            WFedgeLabel edge = wf.GetEdgeBetween(from, to);

            if (edge != null)
                try {
                    edge.ModifyPrecondition(edgeDocument);
                }
                catch (Exception e) {
                    callbackResult = e.Message;
                }
            else
            {
                edge = new WFedgeLabel(edgeDocument, null);
                wf.AddEdge(edge, from, to);
            }

        }

        #endregion

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall() {
            return ComunicationResources.addArcCall;
        }

        public string getNameFunctionServerResponse() {
            return ComunicationResources.addArcResponse;
        }

        #endregion
    }
}
