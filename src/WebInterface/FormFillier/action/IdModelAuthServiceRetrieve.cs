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
    public class IdMOdelAuthServiceRetrieve : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
    {

        string result = "";
        public string GetCallbackResult()
        {
            return result;
        }
        public void RaiseCallbackEvent(string eventArgument)
        {
            result = "";
            Security.Token tok = (Security.Token)Page.Session["Token"];
            if (tok != null && tok.Authenticated && tok.GetCurrentUser().Registered)
            {
                List<Security.WorkflowReference> editableWf = Page.Session["WorkflowReferenceList"] as List<Security.WorkflowReference>;
                Security.WorkflowReference model = editableWf[Int32.Parse(eventArgument)];
                if (model != null)
                {
                    //TOCHANGE: now can edit only ONE workflow alla volta
                    string wfID = "workflow_1"; //model.GetWorkflowName();
                    result = wfID;
                    
                    Workflow wf = model.GetWorkflow();
                    Page.Session[wfID] = wf;

                    //To can publish
                    Page.Session["WFE_CurrentWorkflowReference"] = model;

                    //Recovering wf's nodes
                    int i = 1;
                    foreach (WFnode n in wf.GetNodes())
                    {
                        Page.Session[wfID + "_node_" + i++] = n;
                    }
                }
                else
                {
                    result = "Model" + model.GetWorkflowName() + " not found";
                }
            }
            else
            {
                result = "User not logged in";
            }
            result = result + "\\//";
            List<Security.Service> service_list = Security.ExternalService.List();
            int count = 0;
            foreach (Security.Service s in service_list)
            {
                result += s.ServiceName + "\\|//" + s.ServiceId;
                count++;
                if (count < service_list.Count)
                {
                    result += "\\||//";
                }
            }
        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "getIdModelAuthServiceCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "getIdModelAuthServiceResult";
        }

        #endregion
    }
}
