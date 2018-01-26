using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Xml;
using Comm.Services.Exceptions;
using Storage;
using Comm.Services.Model;
using Comm.Util;
using Comm.Report;

namespace Comm.Services
{
    /// <summary>
    /// Implementation of mail service
    /// Send a mail with publication's result as attachment
    /// </summary>
    class MailService : IService
    {
       private static Dictionary<string, string> fields;
        
        static MailService()
        {
            fields = new Dictionary<string, string>();
            fields.Add("email", Validator.EmailRegexp);
        }

        private bool SendMail(string dest, string text)
        {
            try
            {
                Uri dataUri = new Uri(dest);
                if (!dataUri.Scheme.Equals(Uri.UriSchemeMailto))
                    throw new InvalidParameterException("MailService need a mail uri");

                string mailAddressDest = dataUri.UserInfo + "@" + dataUri.Host;
                MailMessage message = new MailMessage(Storage.StorageManager.getEnvValue("senderMailAddress"),
                    dataUri.UserInfo + "@" + dataUri.Host);
                message.Subject = "Loa Report - No Reply";
                AlternateView view = AlternateView.CreateAlternateViewFromString(text, Encoding.UTF8, MediaTypeNames.Text.Html);
                message.AlternateViews.Add(view);
                SmtpClient mailClient = new SmtpClient(Storage.StorageManager.getEnvValue("smtpAddress"));
                mailClient.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region IService Membri di

        public bool Setup(Publication pub)
        {
            // do nothing;
            return true;
        }

        public bool Send(Publication result)
        {
            HtmlPublicationResultStrategy strategy = new HtmlPublicationResultStrategy();
            PublicationResult pubResult = strategy.ConvertOutput(result);
            return SendMail(result.URIUpload, pubResult.Text);
        }

        public bool Send(Result req)
        {
            HtmlPublicationResultStrategy strategy = new HtmlPublicationResultStrategy();
            PublicationResult pubResult = strategy.ConvertOutput(req);
            return SendMail(req.Publication.URIUpload, pubResult.Text);
        }

        public string GetUriRegexp()
        {
            return Validator.EmailRegexp;
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
            return "mailto:" + results["email"];
        }

        #endregion
    }
}
