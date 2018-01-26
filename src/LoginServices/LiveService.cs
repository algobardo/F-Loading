using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.SessionState;
using Security;
using Security.Auth;

namespace LoginServices
{
    /// <summary>
    /// This class represents the Windows Live Service.
    /// </summary>
    public class LiveService : ILoginService
    {
        /// <summary>
        /// This object contains all methods necessary for 
        /// authentication and token management.
        /// </summary>
        private WindowsLiveLogin wll = null;

        /// <summary>
        /// The username used by owner of the account.
        /// </summary>
        private string userName = "";

        /// <summary>
        /// The list of friends of the owner of the account. 
        /// </summary>
        private List<Contact> contacts = new List<Contact>();

        private WindowsLiveLogin.ConsentToken consent = null;

        /// <summary>
        /// Initializes the Live Authentication Service.
        /// </summary>
        public LiveService()
        {
            String AppId = Storage.StorageManager.getEnvValue("liveAppId");
            String SecretKey = Storage.StorageManager.getEnvValue("liveSecretKey");
            String securityAlgorithm = Storage.StorageManager.getEnvValue("liveSecurityAlgorithm");
            String policyUrl = Storage.StorageManager.getEnvValue("livePolicyUrl");
            String returnUrl = Storage.StorageManager.getEnvValue("liveReturnUrl");

            wll = new WindowsLiveLogin(AppId, SecretKey, securityAlgorithm, true, policyUrl, returnUrl);
        }

        /// <summary>
        /// The first step of the login phase.
        /// </summary>
        /// <param name="session"> The current session. </param>
        /// <param name="Server"> The current server utility. </param>
        /// <returns> The string representing the url used to connect. </returns>
        public string StartLogin(HttpSessionState session, HttpServerUtility Server)
        {
            return wll.GetConsentUrl("Contacts.View");
        }

        /// <summary>
        /// The second step of the login phase.
        /// </summary>
        /// <param name="Session"> The current session. </param>
        /// <param name="Request"> The current request. </param>
        /// <param name="Server"> The current server utility. </param>
        /// <returns> True if the login was successful, otherwise false. </returns>
        public bool ProcessLogin(HttpSessionState Session, HttpRequest Request, HttpServerUtility Server)
        {
            if (Request["action"] == null) return false;
            if (Request["action"] != "delauth") return false;
            consent = wll.ProcessConsent(Request.Form);
            if (consent != null && consent.IsValid()) return getContactsFromLive();
            return false;
        }

        /// <summary>
        /// Returns the username of the account owner.
        /// </summary>
        /// <returns> The username. </returns>
        public string getUsername()
        {
            return userName;
        }

        /// <summary>
        /// returns a list of friends of the account owner.
        /// </summary>
        /// <returns> The list of friends. </returns>
        public List<Contact> getContactList()
        {
            contacts.Clear();
            getContactsFromLive();
            return contacts;
        }

        /// <summary>
        /// Indicates whether the current session id valid
        /// </summary>
        /// <returns>A bool that indicates whether the current session is valid</returns>
        public bool isLoginValid()
        {
            return consent.IsValid();
        }

        /// <summary>
        /// This method sends a request to windows live contacts with 
        /// the id of the account owner, waiting for reply and finally 
        /// parse the xml content to retrieve the name and email of contacts.
        /// </summary>
        /// <param name="ct"> The consent token used to retrieve the list of contacts. </param>
        /// <param name="Session"> The current session. </param>
        /// <returns> Returns true if the informations has been retrieved correctly, otherwise false. </returns>
        private bool getContactsFromLive()
        {
            UriBuilder uriBuilder = new UriBuilder();

            uriBuilder.Scheme = "HTTPS";
            uriBuilder.Path = "/users/@L@" + consent.LocationID + "/rest/livecontacts";
            uriBuilder.Host = "livecontacts.services.live.com";
            uriBuilder.Port = 443;

            string uriPath = uriBuilder.Uri.AbsoluteUri;
            HttpWebRequest req = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(uriPath);
            }
            catch (Exception)
            {
                return false;
            }
            if (req == null) return false;

            req.Method = "GET";
            req.Headers.Add("Authorization", "DelegatedToken dt=\"" + consent.DelegationToken + "\"");
            try
            {
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                LiveContactsParser pars = new LiveContactsParser();
                pars.parse(new StreamReader(res.GetResponseStream()).ReadToEnd());
                for (int i = 0; i < pars.Contacts.Count; i++)
                    contacts.Add(pars.Contacts[i]);//new Contact(pars.Contacts[i].Name, pars.Contacts[i].Email));
                this.userName = pars.Username;
                res.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}