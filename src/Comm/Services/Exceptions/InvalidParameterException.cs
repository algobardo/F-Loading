using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comm.Services.Exceptions
{
    /// <summary>
    /// Exception launched if the parameter passed is not valid
    /// </summary>  
    public class InvalidParameterException : System.ApplicationException
	{
		    public InvalidParameterException() {}
    		public InvalidParameterException(string message) {}
    		public InvalidParameterException(string message, System.Exception inner) {}

	}
}
