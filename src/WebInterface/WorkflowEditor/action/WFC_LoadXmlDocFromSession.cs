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
using Core;

namespace WebInterface.WorkflowEditor.action {
    public class WFC_LoadXmlDocFromSession : System.Web.UI.Control, ICallbackEventHandler, IControlWorkflowEditor {
        #region ICallbackEventHandler Members

        string callbackResult;

        /// <summary>
        /// Returns callback's result
        /// </summary>
        /// <returns></returns>
        public string GetCallbackResult() {
            return callbackResult;
        }

        /// <summary>
        /// Receives client's call and creates a new Workflow
        /// </summary>
        /// <param name="eventArgument">WorkflowId from client</param>
        public void RaiseCallbackEvent(string eventArgument) {
            try {
                string wfID  = eventArgument;                

                if (Page.Session[wfID] == null)
                    throw new Exception("Workflow with ID == " + wfID + " does not exist!");

                Workflow wf = (Workflow)Page.Session[wfID];
                
                string xmlDoc = BrowserWithServerSync.GetXMLDocument(wf);
                if (xmlDoc.Contains("TEXT") || xmlDoc.Contains("IMAGE") || xmlDoc.Contains("HTMLCODE"))
                    Page.Session["UsingStaticFields"] = true;
                else Page.Session["UsingStaticFields"] = false;

                callbackResult += "OK|" + wfID + '|' + xmlDoc;

            }
            catch (Exception e) {
                callbackResult = e.Message;
            }
        }

        #endregion

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall() {
            return ComunicationResources.loadXmlDocFromSessionCall;
        }

        public string getNameFunctionServerResponse() {
            return ComunicationResources.loadXmlDocFromSessionResponse;
        }

        #endregion
    }
}
