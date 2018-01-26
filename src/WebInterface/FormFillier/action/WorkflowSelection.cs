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
    public class WorkflowSelection : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
    {
        string result;
        public string GetCallbackResult()
        {
            return result;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            if(eventArgument!=null){
            Int32 ind = Int32.Parse(eventArgument);
                Page.Session["wfind"] = ind;
                result = "success";
            }
            else result = "error";
        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "SelectWFCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "selectWFResult";
        }

        #endregion
    }
}