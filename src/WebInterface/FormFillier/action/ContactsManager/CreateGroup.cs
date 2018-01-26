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

// RETURN "true" IF NEW GROUP IS CREATED
// RETURN "false" ELSE

namespace WebInterface.FormFillier.action.contactmanager
{
    public class CreateGroup : System.Web.UI.Control, ICallbackEventHandler, WebInterface.WorkflowEditor.action.IControlWorkflowEditor
    {
        string result = "falseβ";

        public string GetCallbackResult()
        {
            return result;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            Security.Contact tempcontact;
            string newgroupname = "";
            List<Security.Contact> contacts = new List<Security.Contact>();
            Security.Token tok = (Security.Token)Page.Session["Token"];
            Dictionary<string, string> services = (Dictionary<string, string>)Page.Session["services"];

            if (tok != null && tok.Authenticated && tok.GetCurrentUser().Registered)
            {
                Security.User user = tok.GetCurrentUser();

                if (user.Registered)
                {
                    string contactsadded = "";
                    if (eventArgument != null && !eventArgument.Equals(""))
                    {
                        string[] args = eventArgument.Split('β');
                        string[] argcontact;

                        newgroupname = args[0];

                        if (args.Length > 1)
                        {
                            for (int i = 1; i < args.Length; i++)
                            {
                                argcontact = args[i].Split('γ');
                                contactsadded += "β" + args[i];
                                string servicename = services[argcontact[2]];
                                tempcontact = new Security.Contact(argcontact[0], argcontact[1], new Security.Service(servicename, Convert.ToInt32(argcontact[2])));
                                contacts.Add(tempcontact);
                            }

                            if (user.CreateGroup(newgroupname, contacts))
                                result = "trueβ" + newgroupname + contactsadded;
                        }
                        else
                            if (user.CreateGroup(newgroupname))
                                result = "trueβ" + newgroupname;
                    }
                }
                else result = "unrβ";
            }
        }

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return "CreateGroupCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "CreateGroupResponse";
        }
        #endregion
    }
}
