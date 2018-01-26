using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security
{
    public class EnvironmentManagement
    {
        /// <summary>
        /// method that returns the value associated to the input variable
        /// </summary>
        /// <param name="envVariable">the environment's variable name</param>
        /// <returns>the value associated to the input variable</returns>
        public static string getEnvValue(string envVariable)
        {

            return Storage.StorageManager.getEnvValue(envVariable);
        }

        /// <summary>
        /// sets the environment variables (to do only the first time)
        /// </summary>
 
        public static void setEnvValue()
        {

            //TO CHECK AND TO DELETE THE USELESS

            Storage.StorageManager.setEnvValue("defaultGroup", "OtherContacts");
            Storage.StorageManager.setEnvValue("default", "default");
            Storage.StorageManager.setEnvValue("webServerAddress", "floading.di.unipi.it");
            Storage.StorageManager.setEnvValue("isAdminDefault", "false");
            Storage.StorageManager.setEnvValue("mailLimitDefault", "50");
            Storage.StorageManager.setEnvValue("senderMailAddress", "no-reply@cli.di.unipi.it");
            Storage.StorageManager.setEnvValue("senderDisplay", "Floading-NoReply");
            Storage.StorageManager.setEnvValue("fillerpage", "/FormFillier/Filling.aspx");//toSUB
            Storage.StorageManager.setEnvValue("smtpAddress", "mailserver.cli.di.unipi.it");
            Storage.StorageManager.setEnvValue("defaultThemeId", "1");
            Storage.StorageManager.setEnvValue("otherContacts", "OtherContacts"); //toCheck
            Storage.StorageManager.setEnvValue("fbServiceApplicationKey", "14b265db21cbf9068fc31e7f1a625d09");
            Storage.StorageManager.setEnvValue("fbServiceSecret", "09237e7733e8aaab94f0c716a5e3b569");
            Storage.StorageManager.setEnvValue("liveAppId", "000000004C01680E");
            Storage.StorageManager.setEnvValue("liveSecretKey", "1u1vYyCQfRlEuPohxa5jN2tUT5cjvqxm");
            Storage.StorageManager.setEnvValue("liveSecurityAlgorithm", "wsignin1.0");
            Storage.StorageManager.setEnvValue("livePolicyUrl", "http://floading.di.unipi.it/Auth/policy.html"); //toSUB
            Storage.StorageManager.setEnvValue("liveReturnUrl", "http://floading.di.unipi.it/Auth/login.aspx"); //toSUB
            Storage.StorageManager.setEnvValue("oauthConsumerKey", "floading.di.unipi.it");//toSUB?????????
            Storage.StorageManager.setEnvValue("oauthSharedSecret", "wytwTFZ6xWSW00rYWObtbIUO");
            Storage.StorageManager.setEnvValue("oauthSignatureMethod", "HMAC-SHA1");
            Storage.StorageManager.setEnvValue("oauthCallback", "http://floading.di.unipi.it/Auth/login.aspx"); //toSUB
            Storage.StorageManager.setEnvValue("deletingTime", "1:0:0:0");
            Storage.StorageManager.setEnvValue("pruningTime", "10:0:0:0");
            Storage.StorageManager.setEnvValue("wrDirectory", @"c:\users\propr\");

            //LOA MAIL
            Storage.StorageManager.setEnvValue("mailSubject", "Floading - Un nuovo questionario da compilare!");

            Storage.StorageManager.setEnvValue("loaMailBodyPlainTextFormat", "Ciao,\n"
            + "sei stato/a invitato/a a compilare il questionario \"{0}\" raggiungibile attraverso questo link:\n"
            + "{1}\">{1}\n\n"
            + "Per compilarlo tramite l'applicazione dedicata ai dispositivi mobili,\n"
            + "utilizza i seguenti parametri:\n{2}\n\n"
            + "Questa mail è generata automaticamente, non rispondere direttamente.");

            Storage.StorageManager.setEnvValue("loaMailBodyHTMLFormat", "<html><head></head><body>"
            + "Ciao,<br/>"
            + "sei stato/a invitato/a a compilare il questionario \"{0}\" raggiungibile attraverso questo link:<br/>"
            + "<a href=\"{1}\">{1}</a><br/><br/>"
            + "Per compilarlo tramite l'applicazione dedicata ai dispositivi mobili,<br/>"
            + "utilizza i seguenti parametri:<br/>{3}<br/><br/>"
            + "Questa mail &egrave; generata automaticamente, non rispondere direttamente."
            + "</body></html>");

            //MAIL FPF
            Storage.StorageManager.setEnvValue("mailFPFSubject", "Floading - Form pubblica creata!");

            Storage.StorageManager.setEnvValue("mailFPFBodyPlainTextFormat", "Ciao,\n"
            + "Il questionario pubblico \"{0}\" e' raggiungibile attraverso questo link:\n"
            + "{1}\">{1}\n\n"
            + "Usa il link sopra riportato per invitare tutte le persone che vuoi alla compilazione del tuo questionario\n"
            + "Se pensi che alcune persone possano utilizzare un dispositivo mobile per compilarlo, allora"
            + "invia anche i seguenti parametri:\n{2}\n\n"
            + "Questa mail è generata automaticamente, non rispondere direttamente.");


            Storage.StorageManager.setEnvValue("mailFPFBodyHTMLFormat", "<html><head></head><body>"
            + "Ciao,<br/>"
            + "Il questionario pubblico\"{0}\" &egrave raggiungibile attraverso questo link:<br/>"
            + "<a href=\"{1}\">{1}</a><br/><br/>"
            + "Usa il link sopra riportato per invitare tutte le persone che vuoi alla compilazione del tuo questionario,<br/>"
            + "Se pensi che alcune persone possano utilizzare un dispositivo mobile per compilarlo, allora"
            + "utilizza i seguenti parametri:<br/>{3}<br/><br/>"
            + "Questa mail &egrave; generata automaticamente, non rispondere direttamente."
            + "</body></html>");

            //MAIL TFC
            Storage.StorageManager.setEnvValue("mailTFCSubject", "Floading - non siamo riusciti a contattare alcune persone!");

            Storage.StorageManager.setEnvValue("mailTFCBodyPlainTextFormat", "Ciao,\n"
            + "il sistema non e' riuscito a contattare alcuni dei tuoi contatti per il questionario \"{0}\".\n\n"
            + "La lista e' la seguente:\n{2}\n\n"
            + "Se vuoi invitarli, inviagli il link piu' appropriato tra i seguenti:\n"
            + "{1}\n"
            + "Questa mail e' generata automaticamente, non rispondere direttamente.");

            Storage.StorageManager.setEnvValue("mailTFCBodyHTMLFormat", "<html><head></head><body>"
            + "Ciao,<br/>"
            + "il sistema non &egrave; riuscito a contattare alcuni dei tuoi contatti per il questionario \"{0}\".<br/><br/>"
            + "La lista &egrave; la seguente:<br/>{2}<br/><br/>"
            + "Se vuoi invitarli, inviagli il link pi&ugrave; appropriato tra i seguenti:<br/>"
            + "{1}<br/>"
            + "Questa mail &egrave; generata automaticamente, non rispondere direttamente."
            + "</body></html>");

            Storage.StorageManager.setEnvValue("registration","To register, please complete the registration form below."
                    + "All required fields are marked with an asterisk (*).  You can choose"
                    + "more than one service to login into the system." 
                    + "We use external authentication so no password will be keep by the system."
                    + "As you complete the registration form and accept the terms" 
                    + "and conditions, press the OK button to continue.</h3>");
            
            //Storage.StorageManager.setEnvValue("termsAndConditions","

//            1) Prohibited Use Customers and Users may not:
//a)  	Utilize the Services to post adult, obscene, graphically violent, and any content that is deemed questionable shall be prohibited at floading.di.unipi.it sole discretion. Content must be appropriate for all audiences.
//b) 	Utilize the Services to send unsolicited bulk and/or commercial messages over the Internet known as "spam" or "spamming".
//c) 	Utilize the Services to infringe the copyrights or other intellectual property rights of any third party;
//d) 	Utilize the Services in connection with any illegal activity.
//e) 	Display advertisements on their site.

//2) Disclaimer and Indemnity
//a) 	Disclaimer. floading.di.unipi.it expressly disclaims any obligation to monitor its customers and other users. floading.di.unipi.it has no liability or responsibility for the actions of any of its Customers or other Users or any content any User may post on any Web site. floading.di.unipi.it reserve the right to remove any website that is in violation of our terms and conditions. Members will be held legally responsible for the content of their account and account activity.
//b) 	Clients are strongly encouraged to retain backup copies of their data in the event of data loss. Client indemnifies and holds floading.di.unipi.it harmless for any data loss while files are retained on floading.di.unipi.it equipment. floading.di.unipi.it is not liable for any of its services.
//c) 	You agree to indemnify and hold floading.di.unipi.it , and its subsidiaries, officers, agents, or other partners, and employees, harmless from any claim or demand, including reasonable attorneys' fees, made by any third party due to or arising out of your Content, your use of the Service, your connection to the Service, your violation of the Terms of Service, or your violation of any rights of another.
//d) 	The floading.di.unipi.it service is not available for anyone who is under the age of 13 and are not allowed to register and become a member of floading.di.unipi.it. By using the service or the site, you represent and warrant that you agree to and to abide by all of the terms of service. floading.di.unipi.it may cancel your membership and/or prohibit you from using or accessing the service or the Web site for any reason, at any time.
//e) 	floading.di.unipi.it reserves the right to revise these terms and conditions at any time and at its own discretion and members will be obligated to adhere and comply with these revisions. Members have the right to cancel their account at any time if they no longer agree to these revisions; however, continued use of the service will constitute your acceptance to the terms of service changes. Moreover, WebNG reserves the right to cancel any user account for any reason or for no reason at all, without prior notice, or any notice.
//f) 	floading.di.unipi.it reserves the right to cooperate with appropriate legal authorities in investigations of claims of illegal activity involving WebNG.com's Services, Customers and other Users.
//3) Reporting Violations
//To report a site in violation of our Terms of Service please fill our Online Abuse Form. We take abuse incidents very seriously and have a zero tolerance policy.

        }
    }


}