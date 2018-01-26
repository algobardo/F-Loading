using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Comm.Services;
using Comm.Services.Exceptions;


namespace Comm
{
    public class ServiceLocator
    {
        public Dictionary<ServiceType, IService> services;
        public Dictionary<string, ServiceType> schemes;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ServiceLocator()
        {
            if (!UriParser.IsKnownScheme("gdocs"))
                UriParser.Register(new GoogleDocsUriParser(), "gdocs", -1);
            schemes = new Dictionary<string, ServiceType>();
            schemes.Add("mailto", new ServiceType("Email", "mailto"));
            schemes.Add("rss", new ServiceType("RSS Feed", "rss"));
            schemes.Add("ftp", new ServiceType("Ftp Server", "ftp"));
            schemes.Add("gdocs", new ServiceType("Google Docs", "gdocs"));

            services = new Dictionary<ServiceType, IService>();
            services.Add(new ServiceType("Email", "mailto"), new MailService());
            services.Add(new ServiceType("RSS Feed", "rss"), new RssService());
            services.Add(new ServiceType("Ftp Server", "ftp"), new FtpService());
            services.Add(new ServiceType("Google Docs", "gdocs"), new GoogleDocsService());
        }

        /// <summary>
        /// Returns the iservice that handle communication with the 
        /// specified service.
        /// </summary>
        /// <param name="serviceType">The name of service</param>
        /// <returns> A <see cref="Services.IService"/> service/returns>
        public IService GetServiceByName(string name)
        {
            return GetService(new ServiceType(name, ""));
        }

        /// <summary>
        /// Returns the iservice that handle communication with the 
        /// specified service.
        /// </summary>
        /// <param name="serviceType">The scheme of service</param>
        /// <returns> A <see cref="Services.IService"/> service/returns>
        public IService GetServiceByScheme(string scheme)
        {
            Console.WriteLine("schema: " + scheme);
            return GetService(schemes[scheme]);
        }

        /// <summary>
        /// Returns the iservice that handle communication with the 
        /// specified service.
        /// </summary>
        /// <param name="serviceType">The type of service</param>
        /// <returns> A <see cref="Services.IService"/> service/returns>
        public IService GetService(ServiceType serviceType)
        {
            try
            {
                return this.services[serviceType];
            }
            catch (KeyNotFoundException ex)
            {
                throw new ServiceNotFoundException("Service " + serviceType + " not found");
            }
        }

        /// <summary>
        /// Returns the iservice that handle communication with the 
        /// specified service.
        /// </summary>
        /// <param name="serviceType">The type of service</param>
        /// <returns> A <see cref="Services.IService"/> service/returns>
        public IService GetService(string serviceType)
        {
            return this.GetServiceByName(serviceType);
        }

        /// <summary>
        /// Returns a list of supported services
        /// </summary>
        /// <returns>List of supported services</returns>
        public List<string> GetServices()
        {
            List<string> serviceNames = new List<string>();
            foreach (ServiceType t in this.services.Keys)
                serviceNames.Add(t.Name);

            return serviceNames;
        }
    }
}
