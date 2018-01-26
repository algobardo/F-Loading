using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Mobile.Communication
{
    public enum FormStatus { Notified, Downloaded, Opened }

    public class FormInfo
    {
        public FormInfo()
        {
        }

        public FormStatus Status
        {
            get;
            set;
        }

        public String Name 
        {
            get;
            set;
        }

        public String Source 
        {
            get;
            set;
        }

        public String Description 
        {
            get;
            set;
        }

        public DateTime PublicationTime
        {
            get;
            set;
        }

        public DateTime NotificationTime
        {
            get;
            set;
        }

        public DateTime DownloadTime
        {
            get;
            set;
        }

        public FormRequestInfo RequestInfo
        {
            get;
            set;
        }
    }
}
