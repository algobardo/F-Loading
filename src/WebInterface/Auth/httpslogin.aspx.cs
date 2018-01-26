using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace WebInterface.Auth
{
    public partial class httpsLogin : System.Web.UI.Page, ICallbackEventHandler
    {
       

        public static string URLLogin = "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/Auth/login.aspx";
        private string URLHome = "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/FormFillier/index.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            String cbReference;
            String callbackScript;
            cbReference = Page.ClientScript.GetCallbackEventReference(this, "arg", "httpsLoginResult", "");
            callbackScript = @"function  httpsLoginCall(arg, context)" + "{ " + cbReference + ";}";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "httpsLoginCall", callbackScript, true);
            String script = "openFakeMessage();";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "boh", script, true);
            

        }

        private static bool ValidateRemoteCertificate(
        object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors policyErrors
        )
        {
            return true;

        }


        /// <summary>
        /// Shows for a few seconds the message received and then redirect to home page.
        /// </summary>
        /// <param name="message"> The message to display </param>
        private void showError(string message)
        {
            

            result = message + "|";
        }

        string result = "";
        public string GetCallbackResult()
        {
            return result;
        }
        public void RaiseCallbackEvent(string eventArgument)
        {
            try
            {
                result = "";
                char[] sep = new char[1] { '§' };
                //f[0] = '§';
                string username = eventArgument.Split(sep)[0];
                string password = eventArgument.Split(sep)[1];
                string url = (string)Page.Session["HTTPSServiceURL"];

                CredentialCache myCache = new CredentialCache();

                if (username.Equals("") == true) showError("Error: insert a valid username or password");
                if (password.Equals("") == true) showError("Error: insert a valid username or password");

                myCache.Add(new Uri(url), "Basic", new NetworkCredential(username, password));
                myCache.Add(new Uri(url), "Digest", new NetworkCredential(username, password, "www.cli.di.unipi.it"));

                WebRequest request = WebRequest.Create(url);

                // allows for validation of SSL conversations
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateRemoteCertificate);


                request.Credentials = myCache;

                bool logged = true;

                // Contact url sending username/password
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    response.Close();
                }
                catch (WebException exc)
                {
                    if (((HttpWebResponse)exc.Response).StatusCode == HttpStatusCode.Unauthorized)
                        logged = false;
                }

                Page.Session["HTTPSServiceAuth"] = logged;
                Page.Session["HTTPSServiceAuthName"] = username;

                result = "ok|" + URLLogin;
            }
            catch (Exception e)
            {
                result = e.Message + "|";
            }
        }
    }
}
