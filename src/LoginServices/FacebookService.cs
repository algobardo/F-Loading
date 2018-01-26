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
using Storage;

namespace LoginServices
{
    public class FacebookService : ILoginService
    {
        facebook.Components.FacebookService _fbService;
        private static Security.Service _service;
        private static string serviceName = "FaceBook";

        /// <summary>
        /// Initializes the Facebook Authentication Service using the Application Key and Secret assigned
        /// by Facebook.com
        /// </summary>
        public FacebookService()
        {
            _fbService = new facebook.Components.FacebookService();
            //  _fbService.ApplicationKey = "6560061f1ff3e1765a19102f085580d5";
            //  _fbService.Secret = "09237e7733e8aaab94f0c716a5e3b569";
            _fbService.ApplicationKey = StorageManager.getEnvValue("fbServiceApplicationKey");
            _fbService.Secret = StorageManager.getEnvValue("fbServiceSecret");
            _fbService.IsDesktopApplication = false;
            if (_service == null)
            {
                List<Security.Service> extServices = Security.ExternalService.List();
                foreach (Security.Service service in extServices)
                {
                    if (service.ServiceName.Equals(serviceName))
                        _service = service;
                }
            }
        }

        public string StartLogin(HttpSessionState Session, HttpServerUtility Server)
        {
            return "http://www.Facebook.com/login.php?api_key=" + _fbService.ApplicationKey + @"&v=1.0";
        }

        public bool ProcessLogin(HttpSessionState Session, HttpRequest Request, HttpServerUtility Server)
        {
            string sessionKey = (string)Session["Facebook_session_key"];

            // When the user uses the Facebook login page, the redirect back here will will have the auth_token in the query params
            string authToken = Request.QueryString["auth_token"];
            try
            {
                _fbService.CreateSession(authToken);
            }
            catch (Exception)
            {
            }

            if (_fbService.uid != 0)
                return true;
            else
                return false;
        }

        public string getUsername()
        {
            facebook.Schema.user myuser = _fbService.users.getInfo();
            return "" + myuser.uid;
            /*int size = list.Count();
            string result = "";
            for (int i = 0; i < list.Count(); i++)
            {
                result += list[i];
            }*/
            //return "" + _fbService.users.getInfo();
        }

        public List<Security.Contact> getContactList()
        {
            List<Security.Contact> contacts = new List<Security.Contact>();

            IList<facebook.Schema.user> friends = _fbService.friends.getUserObjects();

            foreach (facebook.Schema.user f in friends)
            {
                contacts.Add(new Security.Contact(f.name, "" + f.uid, _service));
            }

            //            facebook.Schema.user myuser = _fbService.con

            //            myuser.
            return contacts;
        }

        public bool isLoginValid()
        { return true; }
    }
}