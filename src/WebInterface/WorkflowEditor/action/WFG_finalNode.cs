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
    public class WFG_finalNode : System.Web.UI.Control, ICallbackEventHandler, IControlWorkflowEditor
    {

        #region ICallbackEventHandler Members

        string toReturn;

        /// <summary>
        /// Returns callback's result
        /// </summary>
        /// <returns></returns>
        public string GetCallbackResult()
        {
            return toReturn;
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
                string wfId = args[0];
                string nodeId = args[1];

                WFnode node = (WFnode)Page.Session[nodeId];
                Workflow wf = (Workflow)Page.Session[wfId];
                wf.MarkAsFinalNode(node);
                toReturn = nodeId;                
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        #endregion

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return ComunicationResources.finalNodeCall;
        }

        public string getNameFunctionServerResponse()
        {
            return ComunicationResources.finalNodeResponse;
        }

        #endregion
    }
}
