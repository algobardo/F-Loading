using System;
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
using System.Collections.Generic;
using Core.WF;

namespace WebInterface.FormFillier.action
{
    public class EditModel : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
    {

        string result="";
        public string GetCallbackResult()
        {
            return result;
        }
        public void RaiseCallbackEvent(string eventArgument)
        {
            Security.Token tok = (Security.Token)Page.Session["Token"];
            if (tok != null && tok.Authenticated && tok.GetCurrentUser().Registered)
            {
                List<Security.WorkflowReference> editableWf = Page.Session["WorkflowReferenceList"] as List<Security.WorkflowReference>;
                Security.WorkflowReference model = editableWf[Int32.Parse(eventArgument)];
                if (model != null)
                {
                    //TOCHANGE: now can edit only ONE workflow >>>>>alla volta<<<<< LOL
                    string wfID = "workflow_1"; //model.GetWorkflowName();                    
                    result = "OK&" + wfID;
                    Workflow wf = model.GetWorkflow();
                    Page.Session[wfID] = wf;

                    //To can publish
                    Page.Session["WFE_CurrentWorkflowReference"] = model;

                    //Recovering wf's nodes                    
                    foreach (WFnode n in wf.GetNodes()) {
                        Page.Session[n.Name] = n;
                    }
                }
                else
                {
                    result = "Model"+model.GetWorkflowName()+" not found&";
                }
            }
            else
            {
                result = "User not logged in&";
            }
        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "openWorkflowEditorCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "openWorkflowEditorResult";
        }

        #endregion
    }
}
