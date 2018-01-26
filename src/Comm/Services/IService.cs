using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Comm.Services.Model;
using System.Xml;
using Storage;

namespace Comm.Services
{
    /// <summary>
    /// This interface must be implemented by all services.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// To call after publish.
        /// </summary>
        /// <param name="pub">The new publication</param>
        bool Setup(Publication pub);

        /// <summary>
        /// Send the result of publication via this service
        /// </summary>
        /// <param name="pub">A publication</param>
        /// <returns></returns>
        bool Send(Publication pub);

        /// <summary>
        /// Partial result 
        /// </summary>
        /// <param name="req">A single result</param>
        /// <returns></returns>
        bool Send(Result res);

        /// <summary>
        /// Returns a regular expression describing
        /// the syntax of the uri required from this service.
        /// </summary>
        /// <returns>A regular expression</returns>
        string GetUriRegexp();

        /// <summary>
        /// Returns a hash structure that associates
        /// every parameter's names composing service's uri
        /// with a regular expression describing allowed
        /// syntax
        /// </summary>
        /// <returns>A dictionary</returns>
        Dictionary<string, string> GetInputFields();
        
        /// <summary>
        /// Returns the number of field required to 
        /// build the uri's service
        /// </summary>
        /// <returns>Number of field composing the uri</returns>
        int GetInputFieldsCount();

        /// <summary>
        /// Build a valid uri using a hash structure 
        /// that associates for every field's name composing 
        /// the uri the desired values.
        /// </summary>
        /// <param name="data">A hash structure that contains
        /// the same keys given by GetInputFields with associated
        /// the value of field</param>
        /// <returns>A valid uri for this service</returns>
        string BuildUri(Dictionary<string, string> data);
    }
}
