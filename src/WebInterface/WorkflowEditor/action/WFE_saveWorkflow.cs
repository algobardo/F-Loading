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
using System.Xml.Schema;
using Fields;
using System.Xml;
using System.Reflection;
using Security;

namespace WebInterface.WorkflowEditor.action {

	public class WFE_saveWorkflow : System.Web.UI.Control, ICallbackEventHandler, IControlWorkflowEditor {

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
		/// Receives client's call and saves the Workflow
		/// </summary>
		/// <param name="eventArgument">WorkflowId from client</param>
		public void RaiseCallbackEvent(string eventArgument) {
            try
            {
                string[] args = eventArgument.Split('|');
                string wfID = args[0];
                string description = args[1];

			    Workflow wf = (Workflow)Page.Session[wfID];            
                
                //Saving WF
                Token t = (Token)Page.Session["Token"];
                if( t != null && t.Authenticated  ){
                    User currentUser = t.GetCurrentUser();
                    if (!currentUser.Registered)
                    {
                        //Page.Response.Redirect("/FormFillier/Registration.aspx");
                        //L'utente non e' registrato!!! deve fare redirect sulla pagina di registrazione
                        callbackResult = "UNREGISTERED|Non sei registrato al sistema!";

                    }
                    else
                    {
                        WorkflowReference wfRef = currentUser.AddNewWorkFlow(wf, description);
                        if (wfRef == null)
                        {
                            callbackResult = "Errore in SECURITY!!!.. devono fixare qualcosa..";
                            return;
                        }

                        Page.Session["WFE_CurrentWorkflowReference"] = wfRef;

                        callbackResult = "OK|Save OK!|" + wfID;
                    }
                }
                else
                    callbackResult = "You are not logged in!";
			}
			catch (Exception e) {
				callbackResult = e.Message;
			}
		}

		#endregion

        #region Utility Methods

        private void SaveFieldsListOnPageSession(Workflow wf) {

        }

        #endregion

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall() {
			return ComunicationResources.saveWorkflowCall;
		}

		public string getNameFunctionServerResponse() {
			return ComunicationResources.saveWorkflowResponse;
		}

		#endregion
	}
}
