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

// THIS FUNCTION DON'T SAVE CONTACTS, SHOW THEM ONLY
// RETURN "true" IF THE NEW GROUP IS CREATED
// RETURN "trueβcontact1β...βcontactn" IF THE NEW GROUP IS CREATED AND CONTACTS ARE ADDED
// RETURN "false" ELSE

namespace WebInterface.FormFillier.action.contactmanager
{
    public class ImportContacts : System.Web.UI.Control, ICallbackEventHandler, WebInterface.WorkflowEditor.action.IControlWorkflowEditor
    {
        string result = "false";

        public string GetCallbackResult()
        {
            return result;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {

            List<Security.Contact> contacts = new List<Security.Contact>();

            Security.Token tok = (Security.Token) Page.Session["Token"];
            if (tok != null && tok.Authenticated)
            {
                Security.User user = tok.GetCurrentUser();

                if (user.Registered)
                {
                    if (eventArgument != null && !eventArgument.Equals(""))
                    {
                        foreach (Security.Service serv in user.GetSubscribedServices())
                        {
                            if (eventArgument.Equals(serv.ServiceName))
                            {
                                contacts = user.ImportContacts(serv.ServiceId);
                                result = "true";
                                break;
                            }
                        }
                    }

                    if (contacts != null && contacts.Count > 0)
                    {
                        foreach (Security.Contact contact in contacts)
                        {
                            result += "β" + contact.Name + "γ" + contact.Email + "γ" + contact.Service.ServiceId;
                        }
                    }
                    else result = "false";
                }
                else result = "unr";
            }
        }

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return "ImportContactsCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "ImportContactsResponse";
        }
        #endregion
    }
}
