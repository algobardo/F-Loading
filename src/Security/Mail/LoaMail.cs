using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using Storage;

namespace Security
{
    public class LoaMail : MailMessage
    {
        private static string _senderMail = StorageManager.getEnvValue("senderMailAddress");
        private static string _senderDisplay = StorageManager.getEnvValue("senderDisplay");
        private static MailAddress senderAdd = new MailAddress(_senderMail, _senderDisplay);

        private static string fillerpage = "/FormFillier/Filling.aspx";
        private static string webServerAddress;

        private static string _bodyPlainTextFormat = StorageManager.getEnvValue("loaMailBodyPlainTextFormat"); //DANYTODO
        private static string _bodyHTMLFormat = StorageManager.getEnvValue("loaMailBodyHTMLFormat"); //DANYTODO


        //private static string _bodyPlainTextFormat = "Ciao,\n"
        //    + "sei stato/a invitato/a a compilare il questionario \"{0}\" raggiungibile attraverso questo link:\n"
        //    + "{1}\">{1}\n\n"
        //    + "Per compilarlo tramite l'applicazione dedicata ai dispositivi mobili,\n"
        //    + "utilizza i seguenti parametri:\n{2}\n\n"
        //    + "Questa mail è generata automaticamente, non rispondere direttamente.";

        //private static string _bodyHTMLFormat = "<html><head></head><body>"
        //    + "Ciao,<br/>"
        //    + "sei stato/a invitato/a a compilare il questionario \"{0}\" raggiungibile attraverso questo link:<br/>"
        //    + "<a href=\"{1}\">{1}</a><br/><br/>"
        //    + "Per compilarlo tramite l'applicazione dedicata ai dispositivi mobili,<br/>"
        //    + "utilizza i seguenti parametri:<br/>{3}<br/><br/>"
        //    + "Questa mail &egrave; generata automaticamente, non rispondere direttamente."
        //    + "</body></html>";


        /// <summary>
        /// Build an email conteining the link to the private form.
        /// </summary>
        /// <param name="receiverMail">the recipient mail address</param>
        /// <param name="questionnaireName">the name of the questionnaire</param>
        /// <param name="compilationRequest">the compilation request</param>
        public LoaMail(MailAddress receiverMail, string questionnaireName, Storage.CompilationRequest compilationRequest)
        {
            if (webServerAddress == null)
                webServerAddress = StorageManager.getEnvValue("webServerAddress");

            From = senderAdd;
            To.Add(receiverMail);

            Subject = StorageManager.getEnvValue("mailSubject");
            //Subject = "Floading - Un nuovo questionario da compilare!";

            Object[] parameters = new Object[4];
            parameters[0] = questionnaireName;
            StorageManager manager = new StorageManager();
            bool isPublic = compilationRequest.Publication.isPublic;

            parameters[1] = "http://"
                + webServerAddress + fillerpage
                + "?WorkflowID=" + compilationRequest.publicationID
                + "&CompilationRequestID=" + compilationRequest.compilReqID;
            parameters[2] = "CompilationRequestID: " + compilationRequest.compilReqID;
            parameters[3] = "CompilationRequestID: " + compilationRequest.compilReqID;
            if (!isPublic)
            {
                Storage.Contact con = compilationRequest.Contact;
                parameters[1] += "&Username=" + con.externalUserID
                    + "&Service=" + con.Service.nameService
                    + "&Token=" + compilationRequest.token;
                parameters[2] += "\nUsername: " + con.externalUserID
                    + "\nService: " + con.Service.nameService
                    + "\nToken: " + compilationRequest.token;
                parameters[3] += "<br/>Username: " + con.externalUserID
                    + "<br/>Service: " + con.Service.nameService
                    + "<br/>Token: " + compilationRequest.token;
            }
            /*try
            {
                IsBodyHtml = true;
                Body =String.Format(_bodyHTMLFormat, parameters);
                
            }
            catch (Exception) { return; }*/

            // Add the alternate body to the message.
            //AlternateView alternateHTML = AlternateView.CreateAlternateViewFromString(String.Format(_bodyHTMLFormat, parameters), null,System.Net.Mime.MediaTypeNames.Text.Html);
            //Metto caratteri speciali giusti. Il db li codifica diversamenti, qui li rimetto a posto.
            string message = String.Format(_bodyPlainTextFormat, parameters);
            int index;
            string sub;
            index = message.IndexOf("\\n");
            if (index != -1 && index < message.Length - 2)
            {
                sub = message.Substring(index, 2);
                message = message.Replace(sub, "\n");
            }
            index = message.IndexOf("\\r");
            if (index != -1 && index < message.Length - 2)
            {
                sub = message.Substring(index, 2);
                message = message.Replace(sub, "\n");
            }
            index = message.IndexOf("\\t");
            if (index != -1 && index < message.Length - 2)
            {
                sub = message.Substring(index, 2);
                message = message.Replace(sub, "\t");
            }
            AlternateView alternatePLAIN = AlternateView.CreateAlternateViewFromString(message, new System.Net.Mime.ContentType("text/plain"));
            alternatePLAIN.TransferEncoding = System.Net.Mime.TransferEncoding.SevenBit;
            //if (alternatePLAIN==null || alternateHTML==null) return;
            //if (alternatePLAIN == null) return;
            //AlternateViews.Add(alternateHTML);
            AlternateViews.Add(alternatePLAIN);


        }
    }
}
