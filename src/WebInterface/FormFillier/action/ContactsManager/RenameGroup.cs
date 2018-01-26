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

// RETURN "true" IF GROUP NAME IS CHANGED
// RETURN "false" ELSE

namespace WebInterface.FormFillier.action.contactmanager
{
    public class RenameGroup : System.Web.UI.Control, ICallbackEventHandler, WebInterface.WorkflowEditor.action.IControlWorkflowEditor
    {
        string result = "falseβ";

        public string GetCallbackResult()
        {
            return result;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            Security.Token tok = (Security.Token)Page.Session["Token"];
            if (tok != null && tok.Authenticated)
            {
                Security.User user = tok.GetCurrentUser();

                if (user.Registered)
                {
                    string[] args = eventArgument.Split('β');

                    if (args != null)
                        if (user.ModifyGroupName(args[0], args[1]))
                            result = "trueβ" + args[0] + "β" + args[1];
                }
            }
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
