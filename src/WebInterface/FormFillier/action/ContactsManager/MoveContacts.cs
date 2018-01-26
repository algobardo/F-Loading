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

// RETURN "true" IF CONTACTS ARE MOVED TO THE NEW GROUP
// RETURN "false" ELSE

namespace WebInterface.FormFillier.action.contactmanager
{
    public class MoveContacts : System.Web.UI.Control, ICallbackEventHandler, WebInterface.WorkflowEditor.action.IControlWorkflowEditor
    {
        string result = "falseβ";

        public string GetCallbackResult()
        {
            return result;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            string ingroupname = "";
            string outgroupname = "";
            List<Security.Contact> contacts = new List<Security.Contact>();
            Security.Contact tempcontact;
            Security.Token tok = (Security.Token)Page.Session["Token"];
            Dictionary<string, string> services = (Dictionary<string, string>)Page.Session["services"];
            string groupandcontacts = "";

            if (tok != null && tok.Authenticated)
            {
                Security.User user = tok.GetCurrentUser();

                if (user.Registered)
                {
                    if (eventArgument != null && !eventArgument.Equals(""))
                    {
                        string[] args = eventArgument.Split('β');
                        string[] argcontact;

                        if (args.Length > 2)
                        {
                            ingroupname = args[0];
                            outgroupname = args[1];

                            groupandcontacts = "β" + ingroupname + "β" + outgroupname;

                            for (int i = 2; i < args.Length; i++)
                            {
                                argcontact = args[i].Split('γ');
                                string servicename = services[argcontact[2]];
                                tempcontact = new Security.Contact(argcontact[0], argcontact[1], new Security.Service(servicename, Convert.ToInt32(argcontact[2])));
                                contacts.Add(tempcontact);
                                groupandcontacts += "β" + args[i];
                            }

                            if (user.MoveContacts(outgroupname, ingroupname, contacts))
                                result = "true" + groupandcontacts;
                        }
                    }
                }
                else result = "unrβ";
            }
        }

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return "MoveContactsCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "MoveContactsResponse";
        }
        #endregion
    }
}
