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
    public class UserWorkflowSend : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
    {
        private List<string> merge(string[] array)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < array.Length; i++)
            {
                bool conf = true;
                for (int j = 0; j < result.Count; j++)
                {
                    if (array[i] == result[j] && i != j) conf = false;
                }
                if (conf) result.Add(array[i]);
            }
            return result;
        }
        string result;
        public string GetCallbackResult()
        {
            return result;
        }
        public void RaiseCallbackEvent(string eventArgument)
        {
            string[] array = eventArgument.Split('|');
            Security.Token tok = (Security.Token)Page.Session["Token"];
            Security.User usr = tok.GetCurrentUser();
            Security.ComputableWorkflowReference wf = null;
            if (array[0].Equals("-200"))
            {
                wf = (Security.ComputableWorkflowReference)Page.Session["WFE_CurrentWorkflow"];
            }
            else
            {
                wf = ((List<Security.ComputableWorkflowReference>)Page.Session["ComputableWorkflowReferenceList"])[Int32.Parse(array[0])];
            }
            string gruppo = "";
            List<Security.Contact> contUsr = new List<Security.Contact>();
            bool flag = false;
            List<Security.Contact> contSend = new List<Security.Contact>();
            string[] array2 = merge(array).ToArray();
            for (int i = 1; i < array2.Length - 1; i++)
            {
                string[] grp = array2[i].Split('/');

                if (grp[1] != gruppo)
                {
                    if (i > 1) { flag = flag || wf.PermitContacts(contSend); }
                    gruppo = grp[1];
                    contUsr = usr.GetContactsByGroup(gruppo);
                    contSend = new List<Security.Contact>();
                }

                contSend.Add(contUsr[Int32.Parse(grp[0])]);
            }
            flag = flag || wf.PermitContacts(contSend);
            if (flag) result = "OK";
            else result = "ERROR";
        }
                
        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "SendCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "SendResult";
        }

        #endregion
    }
}
