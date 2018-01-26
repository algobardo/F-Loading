using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using Storage;

namespace Security
{
    class MailForPublicForm : MailMessage
    {
        private static string _senderMail = StorageManager.getEnvValue("senderMailAddress");
        private static string _senderDisplay = StorageManager.getEnvValue("senderDisplay");
        private static MailAddress senderAdd = new MailAddress(_senderMail, _senderDisplay);

        private static string webServerAddress = StorageManager.getEnvValue("webServerAddress");

        private static string _Subject = StorageManager.getEnvValue("mailFPFSubject");

        //private static string _bodyPlainTextFormat = "Ciao,\n"
        //    + "Il questionario pubblico \"{0}\" e' raggiungibile attraverso questo link:\n"
        //    + "{1}\">{1}\n\n"
        //    + "Usa il link sopra riportato per invitare tutte le persone che vuoi alla compilazione del tuo questionario\n"
        //    + "Se pensi che alcune persone possano utilizzare un dispositivo mobile per compilarlo, allora"
        //    + "invia anche i seguenti parametri:\n{2}\n\n"
        //    + "Questa mail è generata automaticamente, non rispondere direttamente.";

        //private static string _bodyHTMLFormat = "<html><head></head><body>"
        //    + "Ciao,<br/>"
        //    + "Il questionario pubblico\"{0}\" &egrave raggiungibile attraverso questo link:<br/>"
        //    + "<a href=\"{1}\">{1}</a><br/><br/>"
        //    + "Usa il link sopra riportato per invitare tutte le persone che vuoi alla compilazione del tuo questionario,<br/>"
        //    + "Se pensi che alcune persone possano utilizzare un dispositivo mobile per compilarlo, allora"
        //    + "utilizza i seguenti parametri:<br/>{3}<br/><br/>"
        //    + "Questa mail &egrave; generata automaticamente, non rispondere direttamente."
        //    + "</body></html>";

        private static string _bodyPlainTextFormat = StorageManager.getEnvValue("mailFPFBodyPlainTextFormat");
        private static string _bodyHTMLFormat = StorageManager.getEnvValue("mailFPFBodyHTMLFormat"); 
        


        /// <summary>
        /// Build an email conteining the link to the private form.
        /// </summary>
        /// <param name="receiverMail">the recipient mail address</param>
        /// <param name="publication">the publication</param>
        /// <param name="compilationRequest">the compilation request</param>
		public MailForPublicForm(MailAddress receiverMail, Storage.Publication publication, Storage.CompilationRequest compilationRequest)
        {
            From = senderAdd;
            To.Add(receiverMail);            

            //Subject = "Floading - Form pubblica creata!";
            Subject = _Subject;
            
            Object[] parameters = new Object[4];
            parameters[0] = publication.namePublication;
            parameters[1] = Token.ToLink(publication, compilationRequest);
            if (compilationRequest == null)
            {
                parameters[2] = "CompilationRequestID: " + "-1";
                parameters[3] = "CompilationRequestID: " + "-1";
            }
            else
            {
                parameters[2] = "CompilationRequestID: " + compilationRequest.compilReqID;
                parameters[3] = "CompilationRequestID: " + compilationRequest.compilReqID;
            }
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
            if (alternatePLAIN == null) return;
            //AlternateViews.Add(alternateHTML);
            AlternateViews.Add(alternatePLAIN);  
        }
    }
}
