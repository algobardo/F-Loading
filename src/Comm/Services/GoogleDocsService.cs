using System;
using System.Collections.Generic;
using System.Text;
using Google.GData.Documents;
using Google.GData.Client;
using Google.GData.Extensions;
using Storage;
using Comm.Services.Model;
using Comm.Services.Exceptions;
using Comm.Util;
using Comm.Report;
using System.IO;

namespace Comm.Services
{
    /// <summary>
    /// Implementation of google Docs service
    /// Create a Docs document in google account with the results of publication
    /// </summary>
    class GoogleDocsService : IService
    {
        private static Dictionary<string, string> fields;
        
        static GoogleDocsService()
        {
            fields = new Dictionary<string, string>();
            fields.Add("username", Validator.UsernameRegexp);
            fields.Add("password", Validator.PasswordRegexp);
        }

        #region IService Membri di

        public bool Setup(Publication result)
        {
            return true;
        }

        private bool SendFile(string uriUpload, string htmlBuf)
        {
            try
            {
                Uri uri = new Uri(uriUpload);
                Console.WriteLine("uri in send: " + uri);
                string[] token = uri.UserInfo.Split(':');
                string filename = "Report Floading " + DateTime.Now.ToString("dd-MM-yyyy hh.ss") + ".html";
                string fullPathName = StorageManager.getEnvValue("wrDirectory") + filename;
                DocumentsService service = new DocumentsService("Floading report");
                Console.WriteLine("token[0] in send: " + token[0] + " token[1] in send: " + token[1]);
                service.setUserCredentials(token[0], token[1]);
                FileStream stream = new FileStream(fullPathName, FileMode.CreateNew, FileAccess.Write);
                byte[] htmlBytes = new System.Text.ASCIIEncoding().GetBytes(htmlBuf);
                stream.Write(htmlBytes, 0, htmlBytes.Length);
                stream.Close();
                DocumentsListQuery query = new DocumentsListQuery();
                DocumentsFeed feed = service.Query(query);

                DocumentEntry newEntry = service.UploadDocument(fullPathName, filename);

                File.Delete(fullPathName);
            }
            catch(Exception ex) 
            {
                return false;
            }
            return true;
        }

        public bool Send(Publication result)
        {
            return SendFile(result.URIUpload, new HtmlPublicationResultStrategy().ConvertOutput(result).Text);
        }

        public bool Send(Result singleResult)
        {
            return SendFile(singleResult.Publication.URIUpload, new HtmlPublicationResultStrategy().ConvertOutput(singleResult).Text);
        }

        public string GetUriRegexp()
        {
            return Validator.GdocsRegexp;
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
            string username = results["username"];
            string password = results["password"];
            if (username == null || password == null)
            {
                throw new InvalidParameterException();
            }
            return "gdocs:" + username + ":" + password;
        }

        #endregion
    }
}
