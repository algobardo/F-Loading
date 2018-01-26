using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.IO;
using System.Net;
using System.Timers;
using Comm.Services.Model;
using Storage;
using Comm.Services.Exceptions;
using Comm.Util;
using Comm.Report;

namespace Comm.Services
{
    /// <summary>
    /// Implementation of ftp service
    /// Create a text document and send it in a ftp server
    /// </summary>
    class FtpService : IService
    {
        private static Dictionary<string, string> fields;
        
        static FtpService()
        {
            fields = new Dictionary<string, string>();
            fields.Add("username", Validator.UsernameRegexp);
            fields.Add("password", Validator.PasswordRegexp);
            fields.Add("host", Validator.HostRegexp + "|" + Validator.IpAddressRegexp);
            fields.Add("directory", Validator.PathRegexp);
        }

        public static byte[] StringToByteArray(string str)
        {
            return new System.Text.ASCIIEncoding().GetBytes(str);
        }
        
        #region IService Membri di

        public bool Setup(Publication pub)
        {
            // do nothing
            return true;
        }

        public bool Send(Publication result)
        {
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(result.URIUpload + "//Report_Floading_" + DateTime.Now.ToString("ddMMyyyymmss") + ".txt");
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                NaivePublicationResultStrategy strategy = new NaivePublicationResultStrategy();
                PublicationResult pubResult = strategy.ConvertOutput(result);
                byte[] byteRep = StringToByteArray(pubResult.Text);
                Stream writerFtp = ftpRequest.GetRequestStream();
                writerFtp.Write(byteRep, 0, byteRep.Length);
                writerFtp.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Send(Result singleResult)
        {
            try
            {
                Publication result = singleResult.Publication;
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(result.URIUpload + "//Report_Floading_" + DateTime.Now.ToString("ddMMyyyymmss") + ".txt");
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                NaivePublicationResultStrategy strategy = new NaivePublicationResultStrategy();
                PublicationResult pubResult = strategy.ConvertOutput(singleResult);
                byte[] byteRep = StringToByteArray(pubResult.Text);
                Stream writerFtp = ftpRequest.GetRequestStream();
                writerFtp.Write(byteRep, 0, byteRep.Length);
                writerFtp.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
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
            // e se fosse un ftps? O si cambia la gestione dei field specificandone
            // il tipo... oppure si crea un service Ftps identico a questo con
            // l'unica differenza di un uri ftps e EnableSsl nella fase di connect...
            // ftp://user:password@host:port/path
            
            return "ftp://" + results["username"] + ":" + results["password"] + "@" +
                results["host"] + "/" + results["directory"];
        }

        #endregion
    }
}
