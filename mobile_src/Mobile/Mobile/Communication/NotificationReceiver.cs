using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Xml.Serialization;
using System.Net;
using System.Diagnostics;
using Mobile.Communication;
using System.Windows.Forms;

namespace Mobile
{
    public class NotificationReceivedEventArgs : EventArgs
    {
        public NotificationReceivedEventArgs(FormRequestInfo info)
        {
            RequestInfo = info;
        }

        public FormRequestInfo RequestInfo
        {
            get;
            private set;
        }
    }

    public class NotificationReceiver
    {
        private Thread listener;
        private TcpListener tcpListener;

        private XmlSerializer serializer;

        /// <summary>
        /// listen to the event for receive the notifications
        /// </summary>
        public event EventHandler<NotificationReceivedEventArgs> NotificationReceived;

        public bool IsRunning
        {
            get;
            private set;
        }
        
        public NotificationReceiver()
        {
            serializer = new XmlSerializer(typeof(FormRequestInfo));
            tcpListener = new TcpListener(Dns.GetHostEntry("localhost").AddressList[0], Int32.Parse(Mobile.Properties.Resources.NotificationPort));
            IsRunning = false;
        }

        #region Public Methods

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                tcpListener.Stop();
                listener.Abort();
            }
        }

        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                listener = new Thread(new ThreadStart(WaitForMessages)) { IsBackground = true };
                tcpListener.Start();
                listener.Start();
            }
        }

        #endregion
        #region Private Methods

        private void WaitForMessages()
        {
            while (true)
            {
                TcpClient tcpClient = null;
                NetworkStream stream = null;

                try
                {
                    FormRequestInfo info = null;

                    if (tcpListener.Pending())
                    {
                        tcpClient = tcpListener.AcceptTcpClient();

                        byte[] bytes = new byte[1024];
                        stream = tcpClient.GetStream();
                        stream.Read(bytes, 0, bytes.Length);

                        String message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                        Debug.WriteLine("LISTENER: Receiver message received " + message);

                        using (StringReader reader = new StringReader(message))
                            info = serializer.Deserialize(reader) as FormRequestInfo;

                        if (info != null && NotificationReceived != null)
                            NotificationReceived(this, new NotificationReceivedEventArgs(info));

                        stream.Close();
                        tcpClient.Close();
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception exc)
                {
                    if (stream != null)
                        stream.Close();
                    if (tcpClient != null)
                        tcpClient.Close();
                }
            }
        }

        #endregion
    }
}
