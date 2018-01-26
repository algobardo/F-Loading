using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Security
{
    public class Service
    {
        private string servicename;
        private int serviceid;

        public Service(string servicename, int serviceid)
        {
            if (servicename == null)
                throw new ArgumentNullException();
            this.servicename = servicename;
            this.serviceid = serviceid;
        }

        public string ServiceName { get { return servicename; } }
        public int ServiceId { get { return (int)serviceid; } }
    }

    public class ExternalService
    {   
        public static List<Service> List()
        {
            List<Service> servlist = new List<Service>();
            Storage.StorageManager sto = new Storage.StorageManager();
            List<Storage.Service> dbList= sto.getServices();

            Service temp = null;
            if (dbList != null)
            {
                foreach (Storage.Service dbService in dbList)
                    if (dbService.nameService != "default")
                    {
                        if (dbService.nameService == "YouPorn")
                            temp = new Service(dbService.nameService, dbService.serviceID);
                        else servlist.Add(new Service(dbService.nameService, dbService.serviceID));
                    }
            }
            if (temp != null) servlist.Insert(0, temp);
            return servlist;
        }
    }
}
