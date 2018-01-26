using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comm.Services.Exceptions
{
    /// <summary>
    /// Exception launched if the service is not present in the list of available service
    /// </summary>  
    class ServiceNotFoundException : System.ApplicationException
	{
		    public ServiceNotFoundException() {}
    		public ServiceNotFoundException(string message) {}
            public ServiceNotFoundException(string message, System.Exception inner) { }

	}
}
