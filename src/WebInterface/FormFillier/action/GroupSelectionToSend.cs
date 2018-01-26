using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebInterface.FormFillier.action
{
    public class GroupSelectionToSend : System.Web.UI.Control, System.Web.UI.ICallbackEventHandler, IControlFormFiller
    {
        string result;
        public string GetCallbackResult()
        {
            return result;
        }
        public void RaiseCallbackEvent(string eventArgument)
        {
            Security.Token tok = (Security.Token)Page.Session["Token"];
            Security.User usr = tok.GetCurrentUser();
            List<string> gruppi = usr.GetGroups();
            if (gruppi.Count == 0) result = "You did not create any group of contacts";
            else
            {
                result = eventArgument;
                foreach (string gruppo in gruppi)
                {
                    result += "\\|/" + gruppo;
                }
            }
        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "getUserGCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "getUserGResult";
        }

        #endregion
    }
}
