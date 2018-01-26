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
    public class ContactsRetrieve : System.Web.UI.Control, ICallbackEventHandler, WebInterface.WorkflowEditor.action.IControlWorkflowEditor, IControlFormFiller
    {
        string result;
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
                string[] args = eventArgument.Split('|');

                List<Security.Contact> contacts = user.GetContactsByGroup(args[1]);

                if (contacts.Count == 0)
                    result = "There is not any contact";
                else
                {    
                    result = args[0] + "\\|/";
                
                    foreach (Security.Contact c in contacts)
                    {
                        result += c.Name + "\\|/";
                    }
                }
            }
            else
                result = "You haven't authenticated yet.";

        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "getContactsListCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "getContactsListResult";
        }

        #endregion

    }
}