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
    public class ResultRetrieve : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
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
                Security.User usr = tok.GetCurrentUser();
                List<Security.FilledWorkflowReference> listForm = usr.GetCompiledForms();
                if (listForm.Count == 0) result = "You didn't create any form.";
                else
                {
                    foreach (Security.FilledWorkflowReference w in listForm)
                    {
                        
                        result += w.getFilledWorkflowName() + "\\|)//" + w.getPublicationDescription() + "\\|//";
                    }
                }
            }
            else result = "You haven't authenticated yet.";
        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "getResultRetCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "getResultRetResult";
        }

        #endregion
    }
}