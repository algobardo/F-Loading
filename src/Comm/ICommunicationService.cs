using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Storage;
using System.Xml;

namespace Comm
{
    public interface ICommunicationService
    {
        /// <summary>
        /// Do some preliminary work to 
        /// setup comunication service
        /// </summary>
        /// <param name="pub">The new publication</param>
        /// <returns></returns>
        bool Setup(Publication pub);

        /// <summary>
        /// Send the result of publication via associated service
        /// </summary>
        /// <param name="pub">A publication</param>
        /// <returns></returns>
        bool Send(Publication pub);

        /// <summary>
        /// Send only a partial result
        /// </summary>
        /// <param name="res">The result</param>
        /// <returns></returns>
        bool Send(Result res);

        /// <summary>
        /// Returns a regular expression describing
        /// the syntax of the uri required from the service
        /// passed as parameter.
        /// </summary>
        /// <param name="service">The service</param>
        /// <returns>A regular expression describing uri's service</returns>
        string GetRegexpValidator(string service);

        /// <summary>
        /// Returns a list of supported service
        /// </summary>
        /// <returns></returns>
        List<string> GetServices();

        /// <summary>
        /// Returns a hash structure that associates
        /// every parameter's names composing uri of the service
        /// passed as parameter, with a regular expression 
        /// describing allowed syntax
        /// </summary>
        /// <param name="service">The service</param>
        /// <returns>A dictionary</returns>
        Dictionary<string, string> GetInputFieldsForService(string service);
        
        /// <summary>
        /// Build a valid uri using a hash structure 
        /// that associates for every field's name composing 
        /// the uri the desired values.
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="parms">A hash structure that contains
        /// the same keys given by GetInputFields with associated
        /// the value of field</param>
        /// <returns>A valid uri for the service</returns>
        string BuildUri(string service, Dictionary<string, string> parms);

        /// <summary>
        /// Returns the max count of input field required to 
        /// build a uri for services supported by the system.
        /// This is useful for build form with sufficient dimension 
        /// to contains input fields  for every service.
        /// </summary>
        /// <returns>Max count of input field</returns>
        int GetMaxInputFieldsCount();

        /// <summary>
        /// Returns the count of input field required to 
        /// build a uri for the service specified
        /// </summary>
        /// <param name="service">the service</param>
        /// <returns> count of input field</returns>
        int GetInputFieldsCountForService(string service);

        bool GenerateFeed(Publication pub, System.IO.Stream stream);
    }
}
