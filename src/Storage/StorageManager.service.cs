using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storage
{
    public partial class StorageManager
    {
        /// <summary>
        /// Insert a new External Service
        /// </summary>
        /// <param name="newExternalService">Service Name</param>
        /// <param name="newClassName">Class name</param>
        /// <param name="newDllPath">Dll Path</param>
        /// <returns>The external service or null if errors occur</returns>
        public Service addService(string newExternalService, string newClassName, string newDllPath, bool externalUserIdMail)
        {
            Service s = new Service()
            {
                nameService = newExternalService,
                className = newClassName,
                dllPath = newDllPath,
                externalUserIDMail = externalUserIdMail
            };
            return AddEntity(s);
        }

        /// <summary>
        /// Get a list of all external services
        /// </summary>
        /// <returns>List of services</returns>
        public List<Service> getServices()
        {
            var serv = from s in context.Service
                           select s;
            return serv.ToList<Service>();            
        }

        /// <summary>
        /// Get a list of service used by User for login
        /// </summary>
        /// <param name="userID">ID of user</param>
        /// <returns>List of services</returns>
        public List<Service> getServicesByUserID(int userID)
        {            
            var serv = from ea in context.ExternalAccount
                       where ea.userID == userID
                       select ea.Service;
            return serv.ToList();            
        }
    }
}
