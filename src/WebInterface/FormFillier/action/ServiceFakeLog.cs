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
    public class ServiceFakeLog : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
    {
        string result = "ok";
        public string GetCallbackResult()
        {
            return result;
        }
        public void RaiseCallbackEvent(string eventArgument)
        {
            Page.Session["LoginContact"] = true;
            List<Security.Service> listaServizi = Security.ExternalService.List();
            int serv=-1;
            foreach (Security.Service s in listaServizi)
            {
                if (s.ServiceName == eventArgument)
                {
                    serv = s.ServiceId;
                }
            }
            Page.Session["LoginContactServiceID"] = serv;
            Page.Session["LoginContactReturnURL"] = "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + Page.Request.Url.AbsolutePath + Page.Request.Url.Query;
            if (serv != -1)
                result = "ok|" + "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/Auth/login.aspx";
            else{
                result = "Contact an admnistrator|" + "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/FormFillier/index.aspx";

            }
        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "fakeLogCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "fakeLogResult";
        }

        #endregion
    }
}
