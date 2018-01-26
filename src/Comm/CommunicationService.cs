using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Comm.Services;
using Storage;
using Comm.Services.Exceptions;

namespace Comm
{
    public sealed class CommunicationService : ICommunicationService
    {
        private ServiceLocator serviceLocator;
        private static object sync = new Object();
        private static volatile CommunicationService instance;
        private int maxFieldsCount = int.MinValue;

        public static int StartRssFeedConstant = -666;

        /// <summary>
        /// Default constructor.
        /// </summary>
        private CommunicationService()
        {
            serviceLocator = new ServiceLocator();
        }

        public static CommunicationService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (sync)
                    {
                        if(instance == null)
                            instance = new CommunicationService();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Private method to extract the scheme from
        /// a uri string.
        /// </summary>
        /// <param name="uri">Scheme of a uri</param>
        /// <returns></returns>
        private string getServiceName(string uri)
        {
            string str="error in getServiceName";
            if (uri != null && uri.IndexOf(":") != -1)
                str = uri.Substring(0, uri.IndexOf(":"));
            Console.WriteLine("str " +  str);
            return  str;
        }

        #region ICommunicationService Membri di

        public bool Setup(Publication publication)
        {
            IService srv = serviceLocator.GetServiceByScheme(getServiceName(publication.URIUpload));

            return srv.Setup(publication); 
        }

        public bool Send(Publication publication)
        {
            IService srv = serviceLocator.GetServiceByScheme(getServiceName(publication.URIUpload));
     
            return srv.Send(publication); 
        }

        public bool Send(Result res)
        {
            IService srv = serviceLocator.GetServiceByScheme(getServiceName(res.Publication.URIUpload));

            return srv.Send(res);
        }

        public string GetRegexpValidator(string service)
        {
            IService srv = serviceLocator.GetServiceByName(service);

            return srv.GetUriRegexp();
        }

        public List<string> GetServices()
        {
            return serviceLocator.GetServices();
        }

        public Dictionary<string, string> GetInputFieldsForService(string service)
        {
            return this.serviceLocator.GetService(new ServiceType(service)).GetInputFields();
        }

        public string BuildUri(string service, Dictionary<string, string> parms)
        {
            return this.serviceLocator.GetService(new ServiceType(service)).BuildUri(parms);
        }

        public int GetMaxInputFieldsCount()
        {
            if (maxFieldsCount != int.MinValue)
            {
                foreach (string s in this.GetServices())
                {
                    IService service = this.serviceLocator.GetServiceByName(s);
                    if (service.GetInputFieldsCount() > maxFieldsCount)
                        maxFieldsCount = service.GetInputFieldsCount();
                }
            }

            return maxFieldsCount;
        }

        public int GetInputFieldsCountForService(string service)
        {
            return this.serviceLocator.GetService(new ServiceType(service)).GetInputFieldsCount();
        }

        public bool GenerateFeed(Publication pub, System.IO.Stream stream)
        {
            return new RssService().GenerateFeed(pub, stream);
        }
        #endregion
    }
}
