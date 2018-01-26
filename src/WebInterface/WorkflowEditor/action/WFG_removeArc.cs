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

namespace WebInterface.WorkflowEditor.action
{
    public class WFG_removeArc : System.Web.UI.Control, ICallbackEventHandler, IControlWorkflowEditor
    {
        private string callbackResult;

        #region ICallbackEventHandler Members

        /// <summary>
        /// Returns callback's result
        /// </summary>
        /// <returns></returns>
        public string GetCallbackResult()
        {
            return callbackResult;
        }

        /// <summary>
        /// Receives client's call and adds an edge between 'Node id from' and 'Node id to'
        /// </summary>
        /// <param name="eventArgument">WorkflowId from client & Node id from & Node id to & Xml predicate</param>
        public void RaiseCallbackEvent(string eventArgument)
        {
            string[] args = eventArgument.Split('&');
            string workflowId = args[0];
            string fromNodeNameCodified = args[1];
            string toNodeNameCodified = args[2];

            callbackResult = "OK";

            Workflow wf = (Workflow)Page.Session[workflowId];
            wf.WorkflowInvalidationEvent += new EventHandler<WorkflowValidationEventArgs>(wf_WorkflowInvalidationEvent);

            WFedgeLabel edge = wf.GetEdgeBetween((WFnode)Page.Session[fromNodeNameCodified], (WFnode)Page.Session[toNodeNameCodified]);
            
            wf.RemoveEdge(edge);
            
        }

        void wf_WorkflowInvalidationEvent(object sender, WorkflowValidationEventArgs e) {
            if (!e.Valid)
                callbackResult = e.Message;
        }

        #endregion

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return ComunicationResources.removeArcCall;
        }

        public string getNameFunctionServerResponse()
        {
            return ComunicationResources.removeArcResponse;
        }

        #endregion
    }
}
