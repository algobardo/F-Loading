using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Collections;
using System.Web;
using System.Web.SessionState;
using System.Collections.Generic;
using Security.Auth;

namespace LoginServices
{
    public class HTTPSService : ILoginService
    {
        public static string URLSite = "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/Auth/httpslogin.aspx";
        public string ServiceURL;
        public string ServiceName;

        bool logged;
        string username;

        public HTTPSService(string ServiceURL, string ServiceName)
        {
            this.ServiceURL = ServiceURL;
            this.ServiceName = ServiceName;
        }

        public string StartLogin(HttpSessionState session, HttpServerUtility Server)
        {
            session["HTTPSServiceURL"] = ServiceURL;
            session["HTTPSServiceName"] = ServiceName;
            return URLSite;
        }

        public bool ProcessLogin(HttpSessionState Session, HttpRequest Request, HttpServerUtility Server)
        {
            logged = (bool)Session["HTTPSServiceAuth"];
            username = (string)Session["HTTPSServiceAuthName"];
            return logged;
        }

        public string getUsername()
        {
            if (logged)
                return username;
            else return null;
        }

        public List<Security.Contact> getContactList()
        {
			return new List<Security.Contact>();
        }

        public bool isLoginValid()
        { return true; }
    }
}