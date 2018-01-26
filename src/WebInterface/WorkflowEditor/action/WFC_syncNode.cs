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
using Fields;
using Core;

namespace WebInterface.WorkflowEditor.action {
    public class WFC_syncNode : System.Web.UI.Control, ICallbackEventHandler, IControlWorkflowEditor {

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
        /// Receives client's call and syncs a node in the workflow
        /// </summary>
        /// <param name="eventArgument">WorkflowId  & Node id</param>
        public void RaiseCallbackEvent(string eventArgument) {
            try {
                string[] args = eventArgument.Split('&');
                string wfID = args[0];
                string nodeID = args[1];

                string nodeName = args[2];

                callbackResult = "OK";

                //Registering to wf's events
                Workflow wf = (Workflow)Page.Session[wfID];
                wf.WorkflowInvalidationEvent += new EventHandler<WorkflowValidationEventArgs>(wf_WorkflowInvalidationEvent);

                //Creating the new node
                //WFnode node = new WFnode(nodeID);
                WFnode node = (WFnode)Page.Session[nodeName];
                
                //This solves the bug of apostrophes and other charachter wich use & in the coding
                string to_sync = "";
                for (int k = 3; k < args.Length; k++){
                    to_sync += args[k];
                    if(k<args.Length-1) to_sync += '&';
                }
                //BrowserWithServerSync.SyncServerNode(args[2], node);
                BrowserWithServerSync.SyncServerNode(to_sync, node);
            }
            catch (Exception e) {
                callbackResult = e.Message;
            }
        }

        void wf_WorkflowInvalidationEvent(object sender, WorkflowValidationEventArgs e) {
            if (!e.Valid)
                callbackResult += '|' + e.Message;
        }

        #endregion

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall() {
            return ComunicationResources.syncNodeCall;
        }

        public string getNameFunctionServerResponse() {
            return ComunicationResources.syncNodeResponse;
        }

        #endregion
    }
}
