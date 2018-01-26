using System;
using System.IO;
using System.Collections.Generic;
using System.Data.Linq;
using Rss;
using System.Text;
using Storage;
using Comm.Services.Model;
using Comm.Util;
using Comm.Report;
using System.Net;
using Comm.Services.Exceptions;
using System.Net.Mail;

namespace Comm.Services
{
    /// <summary>
    /// Implementation of Rss service
    /// </summary>
    class RssService : IService
    {
        private static Dictionary<string, string> fields;

        static RssService()
        {
            fields = new Dictionary<string, string>();
            fields.Add("email", Validator.EmailRegexp);
           
        }

        public static byte[] StringToByteArray(string str)
        {
            return new System.Text.ASCIIEncoding().GetBytes(str);
        }

        private RssItem GenerateFeedItem(int publicationId, Result res,  string token)
        {
            RssItem item = new RssItem();
            if (res.CompilationRequest != null)
                item.Title = "Loa Report " + res.CompilationRequest.Contact.nameContact;
            else
                item.Title = "Loa Report anonymous user ";
            item.Description = "New compilation ";
            item.Link = new Uri("http://" + Storage.StorageManager.getEnvValue("webServerAddress") +
                "/CommWeb/ViewPublicationResult.aspx?PubId=" + publicationId + "&ResultId=" + res.resultID + "&token=" + token);
            item.PubDate = DateTime.Now;

            return item;
        }

        public bool GenerateFeed(Publication pub, Stream stream)
        {   
            Uri uri = new Uri(pub.URIUpload);
            string token = uri.Query.Substring(7);
            RssFeed feed = new RssFeed();
            RssChannel channel = new RssChannel();
			channel.Title = "Rss Loa ";
			channel.Link = new Uri("http://" + Storage.StorageManager.getEnvValue("webServerAddress") + 
                "/CommWeb/RSSResult.aspx?PubId=" + pub.publicationID + "&token=" + token);
			channel.Description = "Floading Publish";
			channel.LastBuildDate = channel.Items.LatestPubDate();

            RssItem startItem = new RssItem();
            startItem.Title = "RSS Loa Report";
            startItem.Description = "RSS Loa";
            startItem.Link = new Uri("http://" + Storage.StorageManager.getEnvValue("webServerAddress") +
                "/CommWeb/ViewPublicationResult.aspx?PubId=" + pub.publicationID + "&ResultId=" + CommunicationService.StartRssFeedConstant + "&token=" + token);
            startItem.PubDate = DateTime.Now;
            channel.Items.Add(startItem);

            HashSet<int> resultAdded = new HashSet<int>();
            foreach (CompilationRequest req in pub.CompilationRequest)
            {
                foreach (Result res in req.Result)
                {
                    if (res.xmlResult != null)
                    {
                        channel.Items.Add(GenerateFeedItem(pub.publicationID, res, token));
                        resultAdded.Add(res.resultID);
                    }
                }
            }

            foreach (Result res in pub.Result)
            {
                if(resultAdded.Contains(res.resultID))
                    continue;
                channel.Items.Add(GenerateFeedItem(pub.publicationID, res, token));
            }
			feed.Channels.Add(channel);
            feed.Write(stream);

            return true;
        }

        #region IService Membri di

        public bool Setup(Publication result)
        {
            try
            {
                // L'uri nel db e' memorizzata come rss:user@host?token=ciccio 
                // solo per permettere al CommunicationService di riconoscere 
                // il corretto servizio da richiamare.
                string realUri = "mailto" + result.URIUpload.Substring(3);
                Uri dataUri = new Uri(realUri);
                if (!dataUri.Scheme.Equals(Uri.UriSchemeMailto))
                    throw new InvalidParameterException("RssService need a mail uri");
                
                string token = "&" + dataUri.Query.Substring(1);
                string mailAddressDest = dataUri.UserInfo + "@" + dataUri.Host;
                MailMessage message = new MailMessage(Storage.StorageManager.getEnvValue("senderMailAddress"),
                    dataUri.UserInfo + "@" + dataUri.Host);
                message.Subject = "Loa Report - No Reply";
                    
                message.Body = "Thanks for registration at Loa RSS Service Report.\n With this service you can keep update in trend of survey.  " +
                        "http://" + Storage.StorageManager.getEnvValue("webServerAddress") + "/CommWeb/RSSResult.aspx?PubId=" + result.publicationID + token;

                SmtpClient mailClient = new SmtpClient(Storage.StorageManager.getEnvValue("smtpAddress"));
                mailClient.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }

        public bool Send(Publication pub)
        {
            // do nothing
            return true;
        }

        public bool Send(Result res)
        {
            // do nothing
            return true;
        }

        public string GetUriRegexp()
        {
            return Validator.FtpRegexp;
        }

        public Dictionary<string, string> GetInputFields()
        {
            return fields;
        }

        public int GetInputFieldsCount()
        {
            return fields.Count;
        }

        public string BuildUri(Dictionary<string, string> results)
        {
            
            return "rss:" + results["email"] + "?token=" + RandomString(20);
        }

        #endregion

        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}
