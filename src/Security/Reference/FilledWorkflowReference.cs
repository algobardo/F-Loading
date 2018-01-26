using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Security
{
    public class FilledWorkflowReference
    {
        private int publicationId;
        private Storage.StorageManager sto;

        private string _workflowName = null;
        private string _publicationDescription = null;

        public FilledWorkflowReference(int publicationId)
        {
            this.publicationId = publicationId;
            this.sto = new Storage.StorageManager();
        }
        
        public string getFilledWorkflowName()
        {
            if (_workflowName == null)                            
            {
                Storage.Publication pub = sto.getEntityByID<Storage.Publication>(publicationId);
                if (pub == null)
                    return null;
                _workflowName = pub.namePublication;
            }            
            return _workflowName;
        }

        public string getPublicationDescription()
        {
            if (_publicationDescription == null)
            {
                Storage.Publication pub = sto.getEntityByID<Storage.Publication>(publicationId);
                if (pub == null)
                    return null;
                _publicationDescription = pub.description;
            }
            return _publicationDescription;
        }
        [Obsolete("Using getFilledDocument")]
        public List<XmlDocument> getResults()
        {
            List<XmlDocument> results = new List<XmlDocument>();

            Storage.StorageManager db = new Storage.StorageManager();
            Storage.Publication pub = db.getEntityByID<Storage.Publication>(publicationId);
            if (pub == null)
                return null;

            if (pub.anonymResult || pub.isPublic)
            {
                foreach (Storage.Result res in pub.Result)
                    results.Add(Storage.StorageManager.XElementToXmlDocument(res.xmlResult));
            }
            else
            {
                //private non anonime
                foreach (Storage.CompilationRequest req in pub.CompilationRequest)
                {
                    if (req.contactID!=-1)
                    foreach (Storage.Result res in req.Result)
                        results.Add(Storage.StorageManager.XElementToXmlDocument(res.xmlResult));
                }
            }
            return results;
        }
        /// <summary>
        /// Get the workflow's result 
        /// </summary>
        /// <returns>A list of pair composte of: contact that fill and xmlDocument that contain the result. If the form is anonim, the value of the contact is always null</returns>
       public List<Storage.StorageManager.Pair<Contact,XmlDocument>> getFilledDocument()
        {
          
           List<Storage.StorageManager.Pair<Contact,XmlDocument>> results = new List<Storage.StorageManager.Pair<Contact,XmlDocument>>();

            Storage.StorageManager db = new Storage.StorageManager();
            Storage.Publication pub = db.getEntityByID<Storage.Publication>(publicationId);
            if (pub == null)
                return null;

            if (pub.anonymResult || pub.isPublic)
            {
                foreach (Storage.Result res in pub.Result)
                {
                    Storage.StorageManager.Pair<Contact,XmlDocument> el=new Storage.StorageManager.Pair<Contact,XmlDocument>();
                    el.First=null;
                    el.Second=Storage.StorageManager.XElementToXmlDocument(res.xmlResult);
                    results.Add(el);
                }
            }
            else
            {
                //private non anonime
                foreach (Storage.CompilationRequest req in pub.CompilationRequest)
                {
                    if (req.contactID!=-1)
                    foreach (Storage.Result res in req.Result){
                        Storage.StorageManager.Pair<Contact,XmlDocument> el=new Storage.StorageManager.Pair<Contact,XmlDocument>();
                        el.First=new Contact(req.Contact.nameContact,req.Contact.externalUserID,new Service(req.Contact.Service.nameService,req.Contact.Service.serviceID));
                        el.Second=Storage.StorageManager.XElementToXmlDocument(res.xmlResult);
                        results.Add(el);
                    }
                }
            }
            return results;
          
        }

        /// <summary>
        /// Reurns the workflow for this filling
        /// </summary>
        /// <returns>the workflow for this filling</returns>
        public Core.WF.IComputableWorkflow GetWorkflow()
        {
            Storage.Publication pub = sto.getEntityByID<Storage.Publication>(publicationId);
            Core.WF.IComputableWorkflow cwf = (Core.WF.IComputableWorkflow)sto.byteArray2Object(pub.xml.ToArray());
            cwf.setWFname(pub.namePublication);
            cwf.SetDescription(pub.description);
            return cwf;
        }
    }
}
