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

// RETURN "true" + service1 + service2 IF USER HAS SERVICES SUBSCRIVED
// RETURN "false" ELSE

namespace WebInterface.FormFillier.action.contactmanager
{
    public class GetServices : System.Web.UI.Control, ICallbackEventHandler, WebInterface.WorkflowEditor.action.IControlWorkflowEditor
    {
        string result = "falseβ";

        public string GetCallbackResult()
        {
            return result;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {            
            Security.Token tok = (Security.Token)Page.Session["Token"];
            

            if (tok != null && tok.Authenticated && tok.GetCurrentUser().Registered)
            {
                Security.User user = tok.GetCurrentUser();

                if (user.Registered)
                {
                    if (user != null)
                    {
                        if (eventArgument != null && eventArgument.Equals(""))
                        {
                            result = "true";
                            foreach (Security.Service serv in user.GetSubscribedServices())
                                result += "β" + serv.ServiceName + "γ" + Convert.ToString(user.LoginNeeded(serv.ServiceId));
                        }
                    }
                }
                else result = "unrβ";
            }
        }

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return "GetServicesCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "GetServicesResponse";
        }
        #endregion
    }
}
