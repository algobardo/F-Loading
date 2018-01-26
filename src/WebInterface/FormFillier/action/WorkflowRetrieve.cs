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
using WebInterface.FormFillier;

namespace WebInterface.FormFillier.action
{
    public class WorkflowRetrieve : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
    {
        string result;
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
                Security.User user = tok.GetCurrentUser();
                List<Security.ComputableWorkflowReference> computableWf = user.GetComputableWorkflows();
                List<Security.WorkflowReference> editableWf = user.GetEditableWorkflows();

                if (computableWf.Count == 0 && editableWf.Count == 0)
                    result = "You didn't create or publish any form.";
                else
                {
                    if (computableWf.Count == 0)
                        result += "You didn't publish any form";
                    else
                    {
                        int i = 0;
                        foreach (Security.ComputableWorkflowReference w in computableWf)
                        {                            
                            result += w.GetWorkflowName() + "\\|)//" + w.GetWorkflowDescription() + "\\|)//" + i + "\\|)//" + w.GetWorkflowExpirationDate().ToString("d") + "\\|//";
                            i++;
                        }
                    }

                    result += "\\||//";

                    if (editableWf.Count == 0)
                        result += "You didn't create any models";
                    else
                    {
                        int i = 0;
                        foreach (Security.WorkflowReference w in editableWf)
                        {
                            result += w.GetWorkflowName() + "\\|)//" + w.GetWorkflowDescription() + "\\|)//" + i + "\\|//";
                            i++;
                        }
                    }
                    Page.Session["ComputableWorkflowReferenceList"] = computableWf;
                    Page.Session["WorkflowReferenceList"] = editableWf;
                }

            }
            else
                result = "You haven't authenticated yet.";

        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "getWorkFlowListCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "getWorkFlowListResult";
        }

        #endregion
    }
}