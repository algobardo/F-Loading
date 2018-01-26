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
using System.Xml;
using System.IO;
using System.Xml.Schema;

namespace WebInterface.FormFillier.action
{
    public class Logout : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
    {
        string result;
        public string GetCallbackResult()
        {
            return result;
        }
        public void RaiseCallbackEvent(string eventArgument)
        {
            Security.Token tok = (Security.Token)Page.Session["Token"];
            if (tok != null)
            {
                tok.Logout();
                if (((Page.Session["firstAuth"]) == null) || ((bool)Page.Session["firstAuth"]))
                {
                    Page.Session["firstAuth"] = false;
                }
            }
            //Logout.Visible = false;
            //labelUser.Visible = false;
            //ManageContacts.Visible = false;
            //ManageFiles.Visible = false;
            /* Clean the session for security */
            Page.Session["LoginPhase"] = null;
            Page.Session["LoginService"] = null;
            Page.Session["ServiceID"] = null;
            Page.Session["ReturnURL"] = null;
            Page.Session["LoginContact"] = null;
            Page.Session["LoginError"] = null;
            result = "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/FormFillier/index.aspx";
        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "LogoutCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "LogoutResult";
        }

        #endregion
    }
}
