using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Communication
{
    class MailService
    {
        public void send(String mailAddressDest, String subject, String contentMessage)
        {
            try
            {
                //setting mail address
                subject = "Report LOA";
                String mailAddressSend = "test@test.it";
                String serverAddress = "smtp.tiscali.it";

                System.Console.WriteLine(mailAddressDest + " " + subject + contentMessage);
                System.Console.ReadLine();
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(mailAddressSend, mailAddressDest, subject, contentMessage);
               
                //call procedure for create file (da inserire)
                
                String report = "C:\\Users\\gianpy\\Desktop\\specifiche.txt";
                Attachment reportAtt = new Attachment(report, MediaTypeNames.Application.Octet);
                message.Attachments.Add(reportAtt);
                System.Net.Mail.SmtpClient mailClient = new System.Net.Mail.SmtpClient(serverAddress);
                mailClient.Send(message);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.ReadLine();

            }

        }

        public static void Main()
        {
            MailService m = new MailService();
            m.send("gianpieroradano@gmail.com", "Report LOA", "provaprovaprova");
        }
    }
}
