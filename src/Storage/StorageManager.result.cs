using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace Storage
{
    public partial class StorageManager
    {

        /// <summary>
        /// Convert XmlDocument to XElement
        /// </summary>
        /// <param name="xd">The XmlDocument</param>
        /// <returns></returns>
        public static XElement xmlDocumentToXElement(XmlDocument xd) {
            var xdoc = System.Xml.Linq.XDocument.Load(new XmlNodeReader(xd));
            return xdoc.Elements().First();
        }

        /// <summary>
        /// Convert XElement to XmlDocument
        /// </summary>
        /// <param name="xe">The XElement</param>
        /// <returns></returns>
        public static XmlDocument XElementToXmlDocument(XElement xe) {
            var xele = new XmlDocument();
            xele.LoadXml(xe.ToString());
            return xele;
        }

        /// <summary>
        /// Add a new Result
        /// </summary>
        /// <param name="compilationRequestID">The ID of compilation request</param>
        /// <param name="xmlResult">The xml</param>
        /// <returns>Result Object</returns>
        public Object addResult(int compilationRequestID, XElement xmlResult)
        {
            CompilationRequest cr = context.CompilationRequest.FirstOrDefault(f => f.compilReqID == compilationRequestID);
            Publication p = cr.Publication;

            cr.compiled = true;
            Result r = new Result();

            if (p.anonymResult)
            {
                r.publicationID = p.publicationID;
                r.xmlResult = xmlResult;
                return AddEntity(r); 
            }
            else
            {   
                r.compilReqID = compilationRequestID;
                r.xmlResult = xmlResult;
                return AddEntity(r);
            }

        }

        /// <summary>
        /// Get a Result list of a specific publication
        /// </summary>
        /// <param name="publicationID">The ID of publication</param>
        /// <returns>The list of result or null if not exists</returns>
        public List<Result> getResultsByPublicationID(int publicationID)
        {
            return context.Result.Where(r => r.publicationID == publicationID).ToList();
        }
        
    }
}
