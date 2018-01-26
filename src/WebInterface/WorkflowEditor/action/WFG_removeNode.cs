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

namespace WebInterface.WorkflowEditor.action
{
    public class WFG_removeNode : System.Web.UI.Control, ICallbackEventHandler, IControlWorkflowEditor
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
        /// Receives client's call and adds a node in the workflow
        /// </summary>
        /// <param name="eventArgument">WorkflowId  & Node id</param>
        public void RaiseCallbackEvent(string eventArgument)
        {
            try
            {
                string[] args = eventArgument.Split('&');
                string wfID = args[0];

                string nodeID = args[1];

                string nodeName = args[2];

                callbackResult = "OK";

                //Adding node to the WF
                Workflow wf = (Workflow)Page.Session[wfID];
                wf.WorkflowInvalidationEvent += new EventHandler<WorkflowValidationEventArgs>(wf_WorkflowInvalidationEvent);
                wf.RemoveNode((WFnode)Page.Session[nodeName]);
                Page.Session[nodeName] = null;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                callbackResult = e.Message;
            }
        }

        void wf_WorkflowInvalidationEvent(object sender, WorkflowValidationEventArgs e)
        {
            if (!e.Valid)
                callbackResult = e.Message;
        }

        #endregion

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return ComunicationResources.removeNodeCall;
        }

        public string getNameFunctionServerResponse()
        {
            return ComunicationResources.removeNodeResponse;
        }

        #endregion
    }
}
