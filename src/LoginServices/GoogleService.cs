using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Web;
using System.Web.SessionState;
using Security.Auth;

namespace LoginServices
{
    public class GoogleService : ILoginService
    {

        private static string oauth_consumer_key = Storage.StorageManager.getEnvValue("oauthConsumerKey");//your consumber key
        private static string shared_secret = Storage.StorageManager.getEnvValue("oauthSharedSecret");//your shared secret
        private static string oauth_signature_method = Storage.StorageManager.getEnvValue("oauthSignatureMethod");
        private static string oauth_callback = "http://" + Storage.StorageManager.getEnvValue("webServerAddress") + "/Auth/login.aspx";
        private static string scope = "http://www.google.com/m8/feeds/";
        private static string serviceName = "Google";
        private static int maxContacts = 32000;
        private static Security.Service _service;

        private string oauth_token = null;
        private string oauth_token_secret = null;

        private string username;
        
        
        //private List<Security.Contact> contacts;

        public GoogleService()
        {
            username = "";
            //contacts = new List<Security.Contact>();

            List<Security.Service> extServices = Security.ExternalService.List();
            foreach (Security.Service service in extServices)
            {
                if (service.ServiceName.Equals(serviceName))
                    _service = service;
            }
        }

        public string StartLogin(HttpSessionState session, HttpServerUtility Server)
        {
            String requestUrl = "https://www.google.com/accounts/OAuthGetRequestToken";

            string outParam;
            string normalUri;

            OAuthBase oAuth = new OAuthBase();

            Uri url = new Uri(requestUrl);
            string oauth_nonce = oAuth.GenerateNonce();
            string oauth_timestamp = oAuth.GenerateTimeStamp();

            string signatureBase = oAuth.GenerateSignatureBase(url, oauth_consumer_key, "", "", "GET", oauth_timestamp, oauth_nonce, oauth_signature_method, out normalUri, out outParam);
            signatureBase += oAuth.UrlEncode("&scope=" + oAuth.UrlEncode(scope));

            HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(oAuth.UrlEncode(shared_secret) + "&"));
            string oauth_signature = oAuth.GenerateSignatureUsingHash(signatureBase, hmacsha1);
            oauth_signature = oAuth.UrlEncode(oauth_signature);

            string richiesta = requestUrl + "?" + outParam + "&oauth_signature=" + oauth_signature + "&scope=" + oAuth.UrlEncode(scope);
            WebRequest request = WebRequest.Create(richiesta);
            request.Method = "GET";
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException) { return null; }
            StreamReader reader = new StreamReader(response.GetResponseStream());
            String resp = reader.ReadToEnd();
            reader.Close();
            response.Close();
            int start = resp.IndexOf('=') + 1;
            int length = resp.IndexOf('&') - start;
            string oauth_token_temp = resp.Substring(start, length);
            start = resp.IndexOf('=', start) + 1;
            string oauth_token_secret_temp = resp.Substring(start);

            richiesta = "https://www.google.com/accounts/OAuthAuthorizeToken?";
            richiesta += "oauth_token=" + oauth_token_temp;
            richiesta += "&oauth_callback=" + oauth_callback;

            //session["GoogleOauthTokenSecret"] = Server.UrlDecode(oauth_token_secret_temp);
            oauth_token_secret = Server.UrlDecode(oauth_token_secret_temp);

            return richiesta;
        }

        public bool ProcessLogin(HttpSessionState Session, HttpRequest Request, HttpServerUtility Server)
        {
            if (String.IsNullOrEmpty(Request.QueryString.Get("oauth_token")))
            {
                return false;
            }

            string requestUrl = "https://www.google.com/accounts/OAuthGetAccessToken";

            string outParam;
            string normalUri;

            OAuthBase oAuth = new OAuthBase();

            Uri url = new Uri(requestUrl);
            string oauth_nonce = oAuth.GenerateNonce();
            string oauth_timestamp = oAuth.GenerateTimeStamp();
            oauth_token = oAuth.UrlEncode(Request.QueryString.Get("oauth_token"));
            //string oauth_token_secret = (String)Session["GoogleOauthTokenSecret"];

            string oauth_signature = oAuth.GenerateSignature(url, oauth_consumer_key, shared_secret, oauth_token, oauth_token_secret, "GET", oauth_timestamp, oauth_nonce, out normalUri, out outParam);
            oauth_signature = oAuth.UrlEncode(oauth_signature);

            string richiesta = requestUrl + "?" + outParam + "&oauth_signature=" + oauth_signature;
            WebRequest request = WebRequest.Create(richiesta);
            request.Method = "GET";

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                String resp = reader.ReadToEnd();
                reader.Close();
                response.Close();

                int start = resp.IndexOf('=') + 1;
                int length = resp.IndexOf('&') - start;
                oauth_token = resp.Substring(start, length);
                start = resp.IndexOf('=', start) + 1;
                oauth_token_secret = resp.Substring(start);
                oauth_token_secret = Server.UrlDecode(oauth_token_secret);
                //Session["GoogleOauthToken"] = oauth_token;
                //Session["GoogleOauthTokenSecret"] = oauth_token_secret;

                /* Prelevo i contatti e sopratutto l'username*/

                // In maxResult c'e' il numero massimo di contatti da prelevare
                /*int maxResult = 500;
                string noLimit = "?max-results=" + maxResult;
                requestUrl = "http://www.google.com/m8/feeds/contacts/default/full/";
                url = new Uri(requestUrl + noLimit);
                oauth_nonce = oAuth.GenerateNonce();
                oauth_timestamp = oAuth.GenerateTimeStamp();
                oauth_signature = oAuth.GenerateSignature(url, oauth_consumer_key, shared_secret, oauth_token, oauth_token_secret, "GET", oauth_timestamp, oauth_nonce, out normalUri, out outParam);
                oauth_signature = oAuth.UrlEncode(oauth_signature);


                richiesta = requestUrl + "?" + outParam + "&oauth_signature=" + oauth_signature;
                request = WebRequest.Create(richiesta);
                request.Method = "GET";

                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                resp = reader.ReadToEnd();
                reader.Close();
                response.Close();*/
                //Session.Remove("GoogleOauthToken");
                //Session.Remove("GoogleOauthTokenSecret");
                //Prendo l'username
                resp = GetUserInfo(1);
                if (resp == null)
                    return false;
                username = GetUsernameFromXML(resp);
                if (username == null)
                    return false;
                else
                    return true;
            }
            catch (WebException exc)
            {
                StreamReader reader = new StreamReader(exc.Response.GetResponseStream());
                String resp = reader.ReadToEnd();
                reader.Close();
                return false;
            }
        }

        public string getUsername()
        {
            return username;
        }
    
        public List<Security.Contact> getContactList()
        {
            string respXml=GetUserInfo(maxContacts);
            if (respXml == null)
                return null;
            return getContactsFromXML(respXml);
        }

        public bool isLoginValid()
        {

            if (oauth_token == null || oauth_token_secret == null)
                return false;
          
            return true;
        }

        private string GetUserInfo(int maxResult)
        {
            OAuthBase oAuth = new OAuthBase();
            string outParam;
            string normalUri;

            string noLimit = "?max-results=" + maxResult;
            string requestUrl = "http://www.google.com/m8/feeds/contacts/default/full/";
            Uri url = new Uri(requestUrl + noLimit);
            string oauth_nonce = oAuth.GenerateNonce();
            string oauth_timestamp = oAuth.GenerateTimeStamp();
            string oauth_signature = oAuth.GenerateSignature(url, oauth_consumer_key, shared_secret, oauth_token, oauth_token_secret, "GET", oauth_timestamp, oauth_nonce, out normalUri, out outParam);
            oauth_signature = oAuth.UrlEncode(oauth_signature);


            string richiesta = requestUrl + "?" + outParam + "&oauth_signature=" + oauth_signature;
            WebRequest request = WebRequest.Create(richiesta);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string resp = reader.ReadToEnd();
            reader.Close();
            response.Close();
            return resp;
        }

        private string GetUsernameFromXML(string xmlStr)
        {
            byte[] byteArray = new byte[xmlStr.Length];
            ASCIIEncoding encoding = new ASCIIEncoding();
            byteArray = encoding.GetBytes(xmlStr);
            MemoryStream memoryStream = new MemoryStream(byteArray);
            memoryStream.Seek(0, SeekOrigin.Begin);
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(memoryStream);
                IEnumerator en = xml.GetElementsByTagName("feed").GetEnumerator();
                en.MoveNext();
                XmlNode feed = en.Current as XmlNode;
                return feed.FirstChild.InnerText;
            }
            catch (XmlException)
            {
                return null;
            }
        }
       
        private List<Security.Contact> getContactsFromXML(string xmlStr)
        {
            byte[] byteArray = new byte[xmlStr.Length];
            ASCIIEncoding encoding = new ASCIIEncoding();
            byteArray = encoding.GetBytes(xmlStr);
            MemoryStream memoryStream = new MemoryStream(byteArray);
            memoryStream.Seek(0, SeekOrigin.Begin);
            XmlDocument xml = new XmlDocument();
            List<Security.Contact> contacts = new List<Security.Contact>();
            string usr;
            try
            {
                xml.Load(memoryStream);
                IEnumerator en = xml.GetElementsByTagName("feed").GetEnumerator();
                en.MoveNext();
                XmlNode feed = en.Current as XmlNode;
                usr = feed.FirstChild.InnerText;
                en = feed.ChildNodes.GetEnumerator();
                foreach (XmlNode node in feed.ChildNodes)
                {
                    if (node.Name == "entry")
                    {
                        //Controllo caso contatto senza mail, metto mail vuota
                        string email;
                        if (node["gd:email"] != null)
                            email = node["gd:email"].GetAttribute("address");
                        else
                            email = "";
                        contacts.Add(new Security.Contact(email, email, _service));
                    }
                }
                return contacts;
            }
            catch (XmlException)
            {
                return null;
            }
        }
    }
}