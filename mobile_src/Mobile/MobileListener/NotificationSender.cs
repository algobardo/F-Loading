using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsMobile.PocketOutlook;
using Microsoft.WindowsMobile.PocketOutlook.MessageInterception;
using Microsoft.WindowsMobile.Status;
using System.Runtime.InteropServices;
using System.Threading;
using MAPIdotnet;
using System.Xml;
using System.Net.Sockets;
using System.Net;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace MobileListener
{
    /// <summary>
    /// The handler of incoming email and sms notifications
    /// </summary>
    public class NotificationSender
    {
        private MessageInterceptor interceptor;
        private XmlSerializer serializer;
        
        private MAPI mapi;
        private IMAPIMsgStore[] mapiStores;
        private bool detectSms;
        private bool detectEmail;

        /// <summary>
        /// Whether to listen and notify incoming sms 
        /// </summary>
        public bool DetectSms 
        {
            get
            {
                return detectSms;
            }
            set
            {
                if (!detectSms && value)
                {
                    Debug.WriteLine("LISTENER: Sender detect sms changed " + value);
                    interceptor.MessageReceived += OnSmsMessageEvent;
                }
                if (detectSms && !value)
                {
                    Debug.WriteLine("LISTENER: Sender detect sms changed " + value);
                    interceptor.MessageReceived -= OnSmsMessageEvent;
                }
                detectSms = value;
            }
        }

        /// <summary>
        /// Whether to listen and notify incoming emails
        /// </summary>
        public bool DetectEmail
        {
            get
            {
                return detectEmail;
            }
            set
            {
                if (!detectEmail && value)
                {
                    //Create session with MAPI and listen to the events
                    mapi = new MAPI();
                    mapiStores = mapi.MessageStores;

                    foreach (IMAPIMsgStore store in mapiStores)
                    {
                        store.EventNotifyMask = EEventMask.fnevNewMail | EEventMask.fnevObjectModified;
                        store.MessageEvent += OnEmailMessageEvent;
                    }
                }
                if (detectEmail && !value)
                {
                    mapi = new MAPI();
                    mapiStores = mapi.MessageStores;

                    foreach (IMAPIMsgStore store in mapiStores)
                        store.MessageEvent -= OnEmailMessageEvent;
                }
                detectEmail = value;
            }
        }

        public NotificationSender()
        {
            serializer = new XmlSerializer(typeof(FormRequestInfo));
            interceptor = new MessageInterceptor(InterceptionAction.Notify, false);
            detectEmail = false;
            detectSms = false;
        }

        #region Event Handlers
        
        //Called for every incoming email(or unreaded one)
        private void OnEmailMessageEvent(IMAPIMessageID newMessageID, IMAPIMessageID oldMessageID, EEventMask messageFlags)
        {
            try
            {
                IMAPIMessage msg = newMessageID.OpenMessage();
                msg.PopulateProperties(EMessageProperties.Body | EMessageProperties.DeliveryTime | EMessageProperties.Sender | EMessageProperties.Subject);
                if ((msg.Flags & EMessageFlags.MSGFLAG_READ) == 0)
                    CheckMessage(msg.Body, msg.Sender.FullAddress, msg.LocalDeliveryTime);
            }
            catch (Exception exc)
            {
            }
        }

        //Called for every incoming sms
        private void OnSmsMessageEvent(object sender, MessageInterceptorEventArgs e)
        {
            SmsMessage message = e.Message as SmsMessage;
            CheckMessage(message.Body, message.From.Address, message.Received);
        }

        //Check if the body of the email/sms contains a form notification
        private void CheckMessage(String message, String from, DateTime received)
        {
            if (message.StartsWith("X-Greylist"))
            {
                int encodingStart = message.IndexOf("\r\n\r\n");
                if (encodingStart >= 0)
                {
                    String base64Message = message.Substring(encodingStart + 4);
                    Byte[] bytes = Convert.FromBase64String(base64Message);
                    String decodedMessage = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                    CheckMessage(decodedMessage, from, received);
                }
            }
            else
            {
                int start = message.LastIndexOf(MobileListener.Properties.Resources.NotificationPattern);
                if (start >= 0)
                {
                    int end = message.IndexOf("\n", start);
                    if (end > start)
                    {
                        String changed = message.Substring(start, end - start - 2);

                        FormRequestInfo info = CommunicationUtil.ParseUrlParameters(changed);

                        //Da cambiare: invece di mandare l'evento comunica info con WCF all'applicazione
                        if (info != null)
                        {
                            info.From = from;
                            info.Time = received;

                            //If the main application is not running start it                
                            StringBuilder builder = new StringBuilder();
                            StringWriter writer = new StringWriter(builder);
                            serializer.Serialize(writer, info);

                            String data = XmlConvert.EncodeName(builder.ToString());

                            String mobilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                            String mobileAppPath = mobilePath + @"\Mobile.exe";
                            Process proc = Process.Start(new ProcessStartInfo(mobileAppPath, data));
                            SendNotification(info);
                        }
                    }
                }
            }
        }

        //Sends the notification to the main application
        private void SendNotification(FormRequestInfo info)
        {
            TcpClient client = null;
            NetworkStream stream = null;
            try
            {
                client = new TcpClient("localhost", 11111);

                StringBuilder builder = new StringBuilder();
                StringWriter writer = new StringWriter(builder);
                serializer.Serialize(writer, info);

                Byte[] data = Encoding.UTF8.GetBytes(builder.ToString());

                stream = client.GetStream(); 
                stream.Write(data, 0, data.Length);
                stream.Flush();
                
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                if(stream != null)
                    stream.Close();
                if(client != null)
                    client.Close();
            }
            catch (SocketException e)
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
        
        #endregion
    }
}
