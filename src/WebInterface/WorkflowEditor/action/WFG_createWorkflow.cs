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
using System.IO;
using Core.WF;
using Fields;

namespace WebInterface.WorkflowEditor.action
{
    public class WFG_createWorkflow : System.Web.UI.Control, ICallbackEventHandler, IControlWorkflowEditor {
        #region ICallbackEventHandler Members

        string to_return = "";

		/// <summary>
		/// Returns callback's result
		/// </summary>
		/// <returns></returns>
        public string GetCallbackResult() {
            return to_return;
        }

		/// <summary>
		/// Receives client's call and creates a new Workflow
		/// </summary>
		/// <param name="eventArgument">WorkflowId from client</param>
        public void RaiseCallbackEvent(string eventArgument) {
            try {
                string[] args = eventArgument.Split('|');
                string wfID = args[0];
                string wfName = args[1];

                Workflow wf = new Workflow(wfName);
                Page.Session[wfID] = wf;                

            }
            catch (Exception e) {
                Console.Write(e.Message);
                to_return = e.Message;
            }
        }

        #endregion

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return ComunicationResources.createWorkflowCall;
        }

        public string getNameFunctionServerResponse()
        {
            return ComunicationResources.createWorkflowResponse;
        }

        #endregion
    }
}
