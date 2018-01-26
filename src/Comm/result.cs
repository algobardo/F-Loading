using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Comm
{
    class Result
    {
        /* commentato perchè c'è un problema di dipendenza circolare sulla libreria security
        public List<XmlDocument> getResults(int publicationId)
        {
            Security.FilledWorkflowReference fwr = new Security.FilledWorkflowReference(publicationId);
            List<XmlDocument> results = new List<XmlDocument>(); 
            results = fwr.getResult();
            return results;
        }
         */
    }
}
