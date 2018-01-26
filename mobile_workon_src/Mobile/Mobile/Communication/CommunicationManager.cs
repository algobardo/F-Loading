using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Threading;

namespace Mobile.Communication
{
    /// <summary>
    /// The results of a query to the service where the mobile
    /// looks for the workflows and the results
    /// </summary>
    public enum CommunicationResult { Success, Fail }

    /// <summary>
    /// The base class for all the possibile arguments
    /// of the events raised when the mobile obtains
    /// a form, a results, or submit the forms data
    /// </summary>
    public class FormEventArgs : EventArgs
    {
        public FormEventArgs(CommunicationResult result)
        {
            this.Result = result;
        }

        public CommunicationResult Result
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// The arguments of the event raised when the mobile
    /// has obtained all the data relative to a form to be compiled
    /// </summary>
    public class FormFetchEventArgs : FormEventArgs
    {
        public FormFetchEventArgs(
            CommunicationResult result,
            XmlDocument workflowNodes,
            XmlDocument workflowEdges,
            XmlSchemaSet types)
            : base(result)
        {
            this.WorkflowNodes = workflowNodes;
            this.WorkflowEdges = workflowEdges;
            this.Types = types;
        }

        public XmlDocument WorkflowNodes
        {
            get;
            private set;
        }

        public XmlDocument WorkflowEdges
        {
            get;
            private set;
        }

        public XmlSchemaSet Types
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// The arguments of the event raised when the mobile
    /// has obtained the description of all the forms the mobile
    /// has to fill
    /// </summary>
    public class FormListFetchEventArgs : FormEventArgs
    {
        public FormListFetchEventArgs(CommunicationResult result)
            : base(result)
        {
        }
    }

    /// <summary>
    /// The arguments of the event raised when the mobile
    /// has obtained all the data relative to the result of a form (poll)
    /// </summary>
    public class ResultFetchEventArgs : FormEventArgs
    {
        public ResultFetchEventArgs(CommunicationResult result)
            : base(result)
        {
        }
    }

    /// <summary>
    /// The arguments of the event raised when the mobile
    /// has obtained the description of all the results
    /// of the forms (polls) published or submitted by the mobile user
    /// </summary>
    public class ResultListFetchEventArgs : FormEventArgs
    {
        public ResultListFetchEventArgs(CommunicationResult result)
            : base(result)
        {
        }
    }

    /// <summary>
    /// The class whose behaviour is to interact with the
    /// "main system", thus with the service that provides
    /// the ability to retrieve the form and the results
    /// through the web
    /// </summary>
    public class CommunicationManager
    {
        #region Events

        //Event raised when a form has been fetched
        public event EventHandler<FormFetchEventArgs> FormFetched;
        //Event raised when the list of the forms that should be filled by the user has been fetched
        public event EventHandler<FormListFetchEventArgs> FormListFetched;
        //Event raised when the data of the form has been submitted to the system
        public event EventHandler<FormEventArgs> DataSubmitted;

        #endregion

        public CommunicationManager()
        {
        }

        /// <summary>
        /// Fetches a form dataset (workflow nodes schema, workflow edges, base types and types used in the workflow)
        /// stored locally, if the form was previously downloaded and saved,
        /// or remotely, asking the system to download it. Then stores the dataset locally
        /// and parse it, creating an XmlSchema for the types used in the workflow, an XmlDocument
        /// for the workflow nodes and an XmlDocument for the workflow edges. Finally raises 
        /// the event FormFetched to announce the dataset is available
        /// </summary>
        /// <param name="formId">the identificator of the form</param>
        public void FetchForm(int formId)
        {
            /*HttpWebRequest request = 
                (HttpWebRequest)WebRequest.Create(Floading_Mobile.Properties.Resources.ServiceEndpoint);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());

            string data = reader.ReadToEnd();

            response.Close();
            reader.Close();*/
            Thread worker = new Thread(
                new ThreadStart(
                    delegate()
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(@"\Program Files\Mobile\" + Mobile.Properties.Resources.FormDataSetFolder + formId);
                        if (dirInfo.Exists)
                        {
                            //Parse local files
                            XmlDocument nodes;
                            XmlDocument edges;
                            XmlSchemaSet types;
                            ParseDataSet(dirInfo, out nodes, out edges, out types);


                            if (FormFetched != null)
                                FormFetched(
                                    this,
                                    new FormFetchEventArgs(
                                        CommunicationResult.Success,
                                        nodes,
                                        edges,
                                        types));
                        }
                        else
                        {
                            //Fetch files from the web, store them locally and parse them

                        }
                    }));
            worker.Start();
        }
        /// <summary>
        /// Fetches the list of all the forms the user has to fill. This list is
        /// formed both by the forms the user didn't ask for and by the forms 
        /// the user has downloaded but not yet submitted. Then raises the event
        /// FormListFetched to announce the list is available
        /// </summary>
        public void FetchFormList()
        {
        }

        public void FetchResult()
        {
        }

        public void FetchResultList()
        {
        }

        public void SubmitData(XmlDocument data)
        {
        }
        
        /// <summary>
        /// Stores locally the (partial) informations specified by the user while filling the form,
        /// so the user can complete to fill the form later.
        /// </summary>
        /// <param name="formId">the identificator of the form</param>
        /// <param name="data">the xml document containing the current informations specified by the user</param>
        public void SaveData(int formId, XmlDocument data)
        {

            DirectoryInfo dirInfo = new DirectoryInfo(@"\Program Files\Mobile\" + Mobile.Properties.Resources.FormDataSetFolder + formId);
            XmlTextWriter writer = new XmlTextWriter(dirInfo.FullName + @"\tempout.xml", Encoding.UTF8);
            //per ogni elem chiamo il writer
            data.WriteTo(writer);
            writer.Flush();
            writer.Close();
        }

        public void XmlReader(int formID, XmlDocument data)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"\Program Files\Mobile\" + Mobile.Properties.Resources.FormDataSetFolder + formID);
            XmlDocument nodes = new XmlDocument();
            nodes.Load(new XmlTextReader(dirInfo.FullName + @"\nodes.xsd"));
            if(data.HasChildNodes)
            {
                foreach(XmlNode node in data.ChildNodes)
                {
                    //while(node.NodeType != )
                }
            }

        }
         

        #region Private methods

        private void ParseDataSet(DirectoryInfo basePath, out XmlDocument nodes, out XmlDocument edges, out XmlSchemaSet schemaSet)
        {
            nodes = new XmlDocument();
            nodes.Load(new XmlTextReader(basePath.FullName + @"\nodes.xsd"));

            edges = new XmlDocument();
            edges.Load(new XmlTextReader(basePath.FullName + @"\edges.xml"));

            XmlSchema types = XmlSchema.Read(new XmlTextReader(basePath.FullName + @"\types.xsd"), OnTypesValidation);
            XmlSchema baseTypes = XmlSchema.Read(new XmlTextReader(basePath.FullName + @"\base.xsd"), OnTypesValidation);
            schemaSet = new XmlSchemaSet();
            schemaSet.Add(baseTypes);
            schemaSet.Add(types);
            schemaSet.Compile();
        }

        private void OnTypesValidation(object source, ValidationEventArgs args)
        {
        }

        #endregion
    }
}
