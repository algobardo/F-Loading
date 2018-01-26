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

// RETURN "true" IF CONTACTS ARE REMOVED FROM THE GROUP
// RETURN "false" ELSE

namespace WebInterface.FormFillier.action.contactmanager
{
    public class RemoveContacts : System.Web.UI.Control, ICallbackEventHandler, WebInterface.WorkflowEditor.action.IControlWorkflowEditor
    {
        string result = "falseβ";

        public string GetCallbackResult()
        {
            return result;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            string groupname = "";
            string contactstring = "";
            List<Security.Contact> contacts = new List<Security.Contact>();
            Security.Contact tempcontact;
            Security.Token tok = (Security.Token)Page.Session["Token"];
            Dictionary<string, string> services = (Dictionary<string, string>)Page.Session["services"];

            if (tok != null && tok.Authenticated)
            {
                Security.User user = tok.GetCurrentUser();

                if (user.Registered)
                {
                    if (eventArgument != null && !eventArgument.Equals(""))
                    {
                        string[] args = eventArgument.Split('β');
                        string[] argcontact;

                        if (args.Length > 1)
                        {
                            groupname = args[0];

                            for (int i = 1; i < args.Length; i++)
                            {
                                argcontact = args[i].Split('γ');
                                string servicename = services[argcontact[2]];
                                tempcontact = new Security.Contact(argcontact[0], argcontact[1], new Security.Service(servicename, Convert.ToInt32(argcontact[2])));
                                contacts.Add(tempcontact);
                                contactstring += "β" + args[i];
                            }

                            if (user.RemoveContactsFromGroup(groupname,contacts))
                                result = "true" + "β" + groupname + contactstring;
                        }
                    }
                }
                else result = "unrβ";
            }
        }

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return "RemoveContactsCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "RemoveContactsResponse";
        }
        #endregion
    }
}
