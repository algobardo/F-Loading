using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using Storage;

namespace Security
{
    public class MailToFormCreator : MailMessage
    {
        private static string _senderMail = StorageManager.getEnvValue("senderMailAddress");
        private static string _senderDisplay = StorageManager.getEnvValue("senderDisplay");
        private static MailAddress senderAdd = new MailAddress(_senderMail, _senderDisplay);

        private static string fillerpage = "/FormFillier/Filling.aspx";
        private static string webServerAddress;

        //private static string _bodyPlainTextFormat = "Ciao,\n"
        //    + "il sistema non e' riuscito a contattare alcuni dei tuoi contatti per il questionario \"{0}\".\n\n"
        //    + "La lista e' la seguente:\n{2}\n\n"
        //    + "Se vuoi invitarli, inviagli il link piu' appropriato tra i seguenti:\n"
        //    + "{1}\n"
        //    + "Questa mail e' generata automaticamente, non rispondere direttamente.";

        //private static string _bodyHTMLFormat = "<html><head></head><body>"
        //    + "Ciao,<br/>"
        //    + "il sistema non &egrave; riuscito a contattare alcuni dei tuoi contatti per il questionario \"{0}\".<br/><br/>"
        //    + "La lista &egrave; la seguente:<br/>{2}<br/><br/>"
        //    + "Se vuoi invitarli, inviagli il link pi&ugrave; appropriato tra i seguenti:<br/>"
        //    + "{1}<br/>"
        //    + "Questa mail &egrave; generata automaticamente, non rispondere direttamente."
        //    + "</body></html>";

        private static string _bodyPlainTextFormat = StorageManager.getEnvValue("mailTFCBodyPlainTextFormat"); //DANYTODO
        private static string _bodyHTMLFormat = StorageManager.getEnvValue("mailTFCBodyHTMLFormat"); //DANYTODO
        
		private static string urlPlainTextFormat = "{0}\n";
		private static string urlHTMLFormat = "<a href=\"{0}\">{0}</a><br/>";

        /// <summary>
        /// Build an email to send to the form creator conteining the link to the form and all contacts name that the system couldn't contact automatically.
        /// </summary>
        /// <param name="receiverMail">the recipient mail address</param>
        /// <param name="questionnaireName">the name of the questionnaire</param>
        /// <param name="compilationRequest">the compilation request</param>
		public MailToFormCreator(MailAddress receiverMail, string questionnaireName, int publicationID, List<Security.Contact> contacts)
        {
            if (webServerAddress == null)
                webServerAddress = StorageManager.getEnvValue("webServerAddress");

            From = senderAdd;
            To.Add(receiverMail);

            //Subject = StorageManager.getEnvValue("mailTFCSubject"); //DANYTODO
            Subject = "Floading - non siamo riusciti a contattare alcune persone!";

            Object[] parametersPlain = new Object[3];
            Object[] parametersHTML = new Object[3];

            parametersHTML[0] = parametersPlain[0] = questionnaireName;            		            

            String allContactsPlain = "";
            String allContactsHTML = "";
			Dictionary<int, Service> usedServices = new Dictionary<int,Service>();

			contacts.OrderBy(Contact => Contact.Service).ThenBy(Contact => Contact.Name);
            foreach (Security.Contact contact in contacts)
            {
                if (contact == null) continue;
				Service service = contact.Service;
                if (service == null) continue;
                allContactsPlain += service.ServiceName + " - " + contact.Name + "\n";
				allContactsHTML += service.ServiceName + " - " + contact.Name + "<br/>";

				if (!usedServices.ContainsKey(service.ServiceId))
					usedServices.Add(service.ServiceId, service);				
            }

            parametersPlain[2] = allContactsPlain;
            parametersHTML[2] = allContactsHTML;

			string baseUrl = "http://"
                + webServerAddress + fillerpage
				+ "?WorkflowID=" + publicationID
				+ "&CompilationRequestID=" + "-1";

			string urlsHTML = "";
			string urlsPlainText = "";
			foreach (KeyValuePair<int, Service> pair in usedServices) 
			{
				string serviceParameter = "&Service=" + pair.Value.ServiceId;
				urlsHTML += pair.Value.ServiceName + " - " + String.Format(urlHTMLFormat, baseUrl + serviceParameter);
				urlsPlainText += pair.Value.ServiceName + " - " + String.Format(urlPlainTextFormat, baseUrl + serviceParameter);
			}

			parametersHTML[1] = urlsHTML;
			parametersPlain[1] = urlsPlainText;

            //Metto caratteri speciali giusti. Il db li codifica diversamenti, qui li rimetto a posto.
            string message = String.Format(_bodyPlainTextFormat, parametersPlain);
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
            alternatePLAIN.TransferEncoding = System.Net.Mime.TransferEncoding.SevenBit;            //if (alternatePLAIN==null || alternateHTML==null) return;
            
            if (alternatePLAIN == null) return;
            //AlternateViews.Add(alternateHTML);
            AlternateViews.Add(alternatePLAIN);
        }
    }
}
