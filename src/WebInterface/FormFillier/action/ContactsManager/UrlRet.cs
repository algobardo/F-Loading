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

namespace WebInterface.FormFillier.action.contactmanager
{
    public class UrlRet : System.Web.UI.Control, ICallbackEventHandler, WebInterface.WorkflowEditor.action.IControlWorkflowEditor
    {
        string result = "false";

        public string GetCallbackResult()
        {
            return result;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {

            String URLret = (String) Page.Session["contactReturn"];
            result = URLret;
        }

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return "UrlRetCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "UrlRetResponse";
        }
        #endregion
    }
}
