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

// RETURN "true" IF GROUP IS REMOVED
// RETURN "false" ELSE

namespace WebInterface.FormFillier.action.contactmanager
{
    public class RemoveGroup : System.Web.UI.Control, ICallbackEventHandler, WebInterface.WorkflowEditor.action.IControlWorkflowEditor
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
                            if (user.RemoveGroup(args[0],Convert.ToBoolean(args[1])))
                                result = "trueβ" + args[1] + "β" + args[0];
                }
                else result = "unrβ";
            }
        }

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return "RemoveGroupCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "RemoveGroupResponse";
        }
        #endregion
    }
}
