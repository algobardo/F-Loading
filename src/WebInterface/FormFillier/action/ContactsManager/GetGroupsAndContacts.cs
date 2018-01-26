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


namespace WebInterface.FormFillier.action.contactmanager
{
    public class GetGroupsAndContacts : System.Web.UI.Control, ICallbackEventHandler, WebInterface.WorkflowEditor.action.IControlWorkflowEditor
    {
        string result = "falseα";

        public string GetCallbackResult()
        {
            return result;
        }

        public void RaiseCallbackEvent(string eventArgument)
        { 
            Security.Token tok = (Security.Token)Page.Session["Token"];
            List<string> listgroup = new List<string>();
            List<Security.Contact> contacts = new List<Security.Contact>();

            if (tok != null && tok.Authenticated && tok.GetCurrentUser().Registered)
            {
                Security.User user = tok.GetCurrentUser();

                if (user.Registered)
                {
                    listgroup = user.GetGroups();
                    if (listgroup != null || listgroup.Count > 0)
                    {
                        result = "true";
                        foreach (string s in listgroup)
                        {
                            result += "α" + s;
                            contacts = user.GetContactsByGroup(s);
                            foreach (Security.Contact con in contacts)
                                result += "β" + con.Name + "γ" + con.Email + "γ" + con.Service.ServiceId;
                        }
                        if (result.Equals("trueαOtherContacts"))
                            result = "falseα";
                    }
                    else result = "falseα";
                }
                else result = "unrα";
            }
        }

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return "GetGroupsAndContactsCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "GetGroupsAndContactsResponse";
        }
        #endregion
    }
}
