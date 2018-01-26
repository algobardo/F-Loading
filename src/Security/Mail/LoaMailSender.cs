using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Security
{
    public class LoaMailSender
    {
        private SmtpClient smtpClient;

        /// <summary>
        /// Build an smtp client for cli.di.unipi.it mail server.
        /// </summary>
        public LoaMailSender()
        {
            smtpClient = new SmtpClient();

			// Work-Around: impostato staticamente per lavorare dalla rete del dipartimento
            //smtpClient.Host = "mailserver.di.unipi.it";
            smtpClient.Host = Storage.StorageManager.getEnvValue("smtpAddress");
        }

        /// <summary>
        /// Send a loa mail message
        /// </summary>
        /// <param name="mail">the mail message</param>
        /// <returns>true on success, false otherwise</returns>
		public bool SendMail(MailMessage mail)
        {
            try
            {
                smtpClient.Send(mail);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
