using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Threading;
using System.Net.Mail;
using System.Text;
using Storage;

namespace WebInterface
{
    public class Global : System.Web.HttpApplication
    {
        static DateTime lastCleanUp;
        static Global()
        {
            lastCleanUp = DateTime.Now;
            /*Cleanup*/
            ThreadPool.QueueUserWorkItem(removeOld);

        }

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        private static void removeOld(object state)
        {
                /*Cleanup*/
                TimeSpan pt = TimeSpan.Parse(Security.EnvironmentManagement.getEnvValue("pruningTime"));
                Storage.StorageManager sm = new Storage.StorageManager();

                var pbs = sm.getPublicationsExpired((int)pt.TotalDays);
                foreach (var it in pbs)
                {
                    sm.removeEntity<Publication>(it.Key);
                }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            TimeSpan dt = TimeSpan.Parse(Security.EnvironmentManagement.getEnvValue("deletingTime"));

            if (DateTime.Now - lastCleanUp > dt)
            {
                ThreadPool.QueueUserWorkItem(removeOld);
            }

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

            var smtpServer = Security.EnvironmentManagement.getEnvValue("smtpAddress");
            var mailSender = Security.EnvironmentManagement.getEnvValue("senderMailAddress");
            var errorRec = Security.EnvironmentManagement.getEnvValue("errorReceiverAddress");

            MailMessage mail = new MailMessage();
            mail.To.Add(errorRec);
            mail.From = new MailAddress(mailSender);

            mail.Subject = "[LOA] Floading crash";
            mail.Body = "Ciao,\nsono Floading e sono in uno stato eccezionale.\nSono crashato e devi risolvermi questo problema.\n" +
                "PAGE REQUESTED:\n" + Request.Path + "\n\n\n" +
                "ECCEZIONE:\n" + Server.GetLastError().ToString() + "\n\n\n";
                

            SmtpClient smtpC = new SmtpClient(smtpServer);
            try
            {
                smtpC.Send(mail);
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.Message);
            }

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}