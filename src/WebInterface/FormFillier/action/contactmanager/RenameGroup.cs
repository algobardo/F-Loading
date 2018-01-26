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

namespace WebInterface.FormFillier.action.contactmanager
{
    public class RenameGroup: System.Web.UI.Control, ICallbackEventHandler
    {
        string result = "false";

        public string GetCallbackResult()
        {
            return result;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            result = "true";
            //Security.Token tok = (Security.Token)Page.Session["Token"];
            //if (tok != null && tok.Authenticated)
            //{
            //    Security.User user = tok.GetCurrentUser();
            //    string[] args = eventArgument.Split('|');

            //    if (args != null)
            //        if (user.ModifyGroupName(args[o], args[1]))
            //            result = "true";
            //}
        }

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return "RenameGroupCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "RenameGroupResponse";
        }
        #endregion
    }
}
