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

// RETURN "groupname" IF THE NEW GROUP IS CREATED
// RETURN "groupnameβcontact1β...βcontactn" IF THE NEW GROUP IS CREATED AND CONTACTS ARE ADDED
// RETURN "false" ELSE

namespace WebInterface.FormFillier.action.contactmanager
{
    public class AddContactsInGroup : System.Web.UI.Control, ICallbackEventHandler, WebInterface.WorkflowEditor.action.IControlWorkflowEditor
    {
        string result = "false";

        public string GetCallbackResult()
        {
            return result;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {

            List<Security.Contact> contacts = new List<Security.Contact>();
            Security.Contact tempcontact;

            Security.Token tok = (Security.Token)Page.Session["Token"];
            Dictionary<string, string> services = (Dictionary<string, string>)Page.Session["services"];

            if (tok != null && tok.Authenticated && tok.GetCurrentUser().Registered)
            {
                Security.User user = tok.GetCurrentUser();

                if (user.Registered)
                {
                    if (eventArgument != null && !eventArgument.Equals(""))
                    {
                        string[] args = eventArgument.Split('β');

                        string groupname = services[args[0]];

                        if (user.CreateGroup(groupname))
                        {
                            string[] argcontact;
                            for (int i = 1; i < args.Length; i++)
                            {
                                argcontact = args[i].Split('γ');
                                string servicename = services[argcontact[2]];
                                tempcontact = new Security.Contact(argcontact[0], argcontact[1], new Security.Service(servicename, Convert.ToInt32(argcontact[2])));
                                contacts.Add(tempcontact);
                            }

                            if (user.AddContactsInGroup(groupname, contacts, Convert.ToInt32(args[0])))
                                result = "true";
                        }
                    }
                }
                else result = "unr";
            }
        }

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return "AddContactsInGroupCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "AddContactsInGroupResponse";
        }
        #endregion
    }
}
