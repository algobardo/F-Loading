using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Threading;
using System.Windows.Forms;
using Mobile.floading;
using System.Xml.Serialization;
using System.Reflection;
using Mobile.Language;

namespace Mobile.Communication
{

    /// <summary>
    /// The results of a query to the service where the mobile
    /// looks for the workflows and the results
    /// </summary>
    public enum CommunicationResult { Success, CommunicationFail, ParseFail, ConnectionClosed }
    public enum DataSetStatus { None, Notified, Downloaded, Opened }

    /// <summary>
    /// The base class for all the possibile arguments
    /// of the events raised when the mobile obtains
    /// a form, a results, or submit the forms data
    /// </summary>
    public class FormEventArgs : EventArgs
    {
        public FormEventArgs(CommunicationResult result, FormInfo info, String message)
        {
            this.Result = result;
            this.Info = info;
            this.Message = message;
        }

        /// <summary>
        /// Contain the information 
        /// </summary>
        public FormInfo Info
        {
            get;
            private set;
        }

        /// <summary>
        /// result of the communication
        /// </summary>
        public CommunicationResult Result
        {
            get;
            private set;
        }

        /// <summary>
        /// massage considerated
        /// </summary>
        public String Message
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
            FormInfo info,
            XmlSchema workflowNodes,
            XmlDocument workflowEdges,
            XmlSchema types,
            XmlDocument data,
            XmlDocument presentation,
            String message)
            : base(result, info, message)
        {
            this.WorkflowNodes = workflowNodes;
            this.WorkflowEdges = workflowEdges;
            this.Types = types;
            this.Data = data;
            this.Presentation = presentation;
        }

        /// <summary>
        /// Property of the workflow schema
        /// </summary>
        public XmlSchema WorkflowNodes
        {
            get;
            private set;
        }

        /// <summary>
        /// The property about the workflow's edge
        /// </summary>
        public XmlDocument WorkflowEdges
        {
            get;
            private set;
        }

        /// <summary>
        /// Property about the schema
        /// </summary>
        public XmlSchema Types
        {
            get;
            private set;
        }

        public XmlDocument Data
        {
            get;
            private set;
        }

        /// <summary>
        /// Document that contain the workflow 
        /// </summary>
        public XmlDocument Presentation
        {
            get;
            private set;
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
        #region Constants

        private static String BaseDir;
        private static String DataDir;

        #endregion

        #region Fields

        private RemoteAccessPoint service;
        private Thread formFetcher;

        private bool isFetching;

        public bool IsFetching
        {
            get
            {
                return isFetching;
            }
            set
            {
                isFetching = value;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Listen for the form fetch
        /// </summary>
        public event EventHandler<FormFetchEventArgs> FormFetched;
        /// <summary>
        /// Listen for the submitted data event
        /// </summary>
        public event EventHandler<FormEventArgs> DataSubmitted;

        #endregion

        #region Constructors

        /// <summary>
        /// construct an object for create a communication manager
        /// </summary>
        /// <param name="address">ip address to connect</param>
        /// <param name="port">port to establish a connection</param>
        public CommunicationManager(String address, int port)
        {
            this.service = new RemoteAccessPoint(address, port);
            BaseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            DataDir = BaseDir + @"\" + Mobile.Properties.Resources.FormDataSetFolder;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Select parametres to connect with server
        /// </summary>
        /// <param name="address">address for connection</param>
        /// <param name="port">port for connection</param>
        public void UpdateService(String address, int port)
        {
            service.Dispose();
            service = new RemoteAccessPoint(address, port);
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
        public void GetForm(FormRequestInfo info)
        {
            //try to call method getEdges from webservices
            if (IsFetching)
            {
                return;
            }
            formFetcher = new Thread(
            new ThreadStart(
                delegate()
                {
                    WorkflowInformations dataSet = null;
                    XmlSchema nodes = null;
                    XmlDocument edges = null;
                    XmlSchema types = null;
                    XmlDocument data = null;
                    XmlDocument presentation = null;

                    CommunicationResult result = CommunicationResult.Success;
                    String message = null;

                    DirectoryInfo dirInfo = new DirectoryInfo(DataDir + info.CompilationRequestId);
                    if (CheckForm(dirInfo) == DataSetStatus.Notified)
                    {
                        try
                        {
                            //Retrieve and store data into local files
                            dataSet = service.GetWorkflow(info.CompilationRequestId, info.Username == null ? "" : info.Username, info.Service == null ? "" : info.Service, info.Token == null ? "" : info.Token);

                            if (dataSet.status == ResultStatus.OK)
                            {
                                StoreForm(dirInfo, dataSet.description.NodesSchema, dataSet.description.EdgesDescription, dataSet.description.ExtendedTypesSchema, dataSet.description.RenderingDocument);

                                FormInfo formInfo = LoadInfo(dirInfo + @"\info.xml");
                                formInfo.DownloadTime = DateTime.Now;
                                formInfo.Status = FormStatus.Downloaded;
                                formInfo.Description = dataSet.publicationDescription;
                                StoreFormInfo(formInfo);
                            }
                            else if (dataSet.status == ResultStatus.ALREADY_COMPILED)
                            {
                                result = CommunicationResult.CommunicationFail;
                                message = ExceptionManager.FormRequestedSubmitted; 

                                if (FormFetched != null)
                                    FormFetched(
                                        this,
                                        new FormFetchEventArgs(
                                            result,
                                            null,
                                            nodes,
                                            edges,
                                            types,
                                            data,
                                            presentation,
                                            message));

                                return;
                            }
                            else
                            {
                                result = CommunicationResult.CommunicationFail;
                                message = ExceptionManager.WrongInformationAuthentication;

                                if (FormFetched != null)
                                    FormFetched(
                                        this,
                                        new FormFetchEventArgs(
                                            result,
                                            null,
                                            nodes,
                                            edges,
                                            types,
                                            data,
                                            presentation,
                                            message));

                                return;
                            }
                        }
                        catch (WebException)
                        {
                            result = CommunicationResult.CommunicationFail;
                            message = ExceptionManager.WebServiceWrongConnection;

                            if (FormFetched != null)
                                FormFetched(
                                    this,
                                    new FormFetchEventArgs(
                                        result,
                                        null,
                                        nodes,
                                        edges,
                                        types,
                                        data,
                                        presentation,
                                        message));
                            return;
                        }
                        catch (ThreadAbortException)
                        {
                            result = CommunicationResult.ConnectionClosed;
                            message = "";

                            if (FormFetched != null)
                                FormFetched(
                                    this,
                                    new FormFetchEventArgs(
                                        result,
                                        null,
                                        nodes,
                                        edges,
                                        types,
                                        data,
                                        presentation,
                                        message));

                            return;
                        }
                        catch (Exception)
                        {
                            result = CommunicationResult.ParseFail;
                            message = ExceptionManager.StoringForm;

                            if (FormFetched != null)
                                FormFetched(
                                    this,
                                    new FormFetchEventArgs(
                                        result,
                                        null,
                                        nodes,
                                        edges,
                                        types,
                                        data,
                                        presentation,
                                        message));

                            return;
                        }
                    }

                    try
                    {
                        //Parse local files
                        //ParseDataSet(data.NodesSchema, data.EdgesDescription, data.BaseTypesSchema, data.ExtendedTypesSchema, out nodes, out edges, out types, out result);
                        ParseForm(dirInfo, out nodes, out edges, out types, out presentation, out data);

                        FormInfo formInformation = LoadInfo(dirInfo + @"\info.xml");
                        if (FormFetched != null)
                            FormFetched(
                                this,
                                new FormFetchEventArgs(
                                    result,
                                    formInformation,
                                    nodes,
                                    edges,
                                    types,
                                    data,
                                    presentation,
                                    message));
                    }
                    catch (ThreadAbortException)
                    {
                        result = CommunicationResult.ConnectionClosed;
                        message = "";

                        if (FormFetched != null)
                            FormFetched(
                                this,
                                new FormFetchEventArgs(
                                    result,
                                    null,
                                    nodes,
                                    edges,
                                    types,
                                    data,
                                    presentation,
                                    message));

                        return;
                    }
                    catch (Exception)
                    {
                        result = CommunicationResult.ParseFail;
                        message = ExceptionManager.LoadingForm;

                        FormInfo formInformation = LoadInfo(dirInfo + @"\info.xml");
                        if (FormFetched != null)
                            FormFetched(
                                this,
                                new FormFetchEventArgs(
                                    result,
                                    formInformation,
                                    nodes,
                                    edges,
                                    types,
                                    data,
                                    presentation,
                                    message));

                        return;
                    }
                }));
            formFetcher.Start();
            IsFetching = true;
        }

        public void AbortFormFetcher()
        {
            if(formFetcher != null)
                formFetcher.Abort();
        }

        /// <summary>
        /// Submit the the informations specified by the user in to the storage
        /// </summary>
        /// <param name="formId">the identificator of the form</param>
        /// <param name="data">the xml document containing the current informations specified by the user</param>
        public void SubmitForm(int formId, XmlDocument data)
        {
            Thread worker = new Thread(
                new ThreadStart(
                    delegate()
                    {
                        FormInfo info = GetFormInfo(formId);
                        try
                        {
                            if (info != null)
                            {
                                bool done = service.SendFilledDocument(info.RequestInfo.CompilationRequestId, info.RequestInfo.Username == null ? "" : info.RequestInfo.Username, info.RequestInfo.Service == null ? "" : info.RequestInfo.Service, info.RequestInfo.Token == null ? "" : info.RequestInfo.Token, data.OuterXml);
                                if (DataSubmitted != null)
                                    DataSubmitted(this, new FormEventArgs(done? CommunicationResult.Success : CommunicationResult.ParseFail, info, null));
                            }
                            else
                                DataSubmitted(this, new FormEventArgs(CommunicationResult.ParseFail, info, null));
                        }
                        catch (WebException we)
                        {
                            if (DataSubmitted != null)
                                DataSubmitted(this, new FormEventArgs(CommunicationResult.CommunicationFail, info, we.Message));
                        }
                    }));
            worker.Start();
        }

        /// <summary>
        /// Load the form information of the form identified to formId
        /// </summary>
        /// <param name="formId">the identificator to the form</param>
        /// <returns></returns>
        public FormInfo GetFormInfo(int formId)
        {
            FileInfo info = new FileInfo(DataDir + formId + "/info.xml");
            if (!info.Exists)
                return null;

            return LoadInfo(info.FullName);
        }

        /// <summary>
        /// Delete the form identified to formId
        /// </summary>
        /// <param name="formId">the identificator of the form</param>
        public void RemoveForm(int formId)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(DataDir + formId);
            if (dirInfo.Exists)
            {
                foreach (FileInfo file in dirInfo.GetFiles())
                    file.Delete();

                dirInfo.Delete();
            }
        }

        /// <summary>
        /// check the list of the local form available for the user.
        /// </summary>
        /// <returns>Return the list of form available for the user</returns>
        public FormInfo[] ListLocalForms()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(DataDir);
            List<FormInfo> infos = new List<FormInfo>();
            foreach (DirectoryInfo subdir in dirInfo.GetDirectories())
            {
                try
                {
                    if (CheckForm(subdir) != DataSetStatus.None)
                    {
                        FormInfo info = LoadInfo(subdir.FullName + @"\info.xml");
                        infos.Add(info);
                    }
                    else
                        subdir.Delete(true);
                }
                catch (Exception)
                {
                }
            }
            return infos.ToArray();
        }

        /// <summary>
        /// Stores locally the (partial) informations specified by the user while filling the form,
        /// so the user can complete to fill the form later.
        /// </summary>
        /// <param name="formId">the identificator of the form</param>
        /// <param name="data">the xml document containing the current informations specified by the user</param>
        public void SaveForm(int formId, XmlDocument data)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(DataDir + formId);
            using (XmlTextWriter writer = new XmlTextWriter(dirInfo.FullName + @"\result.xml", Encoding.UTF8))
            {
                data.WriteTo(writer);
                writer.Flush();
                writer.Close();
            }

            FormInfo formInfo = LoadInfo(dirInfo + @"\info.xml");
            formInfo.Status = FormStatus.Opened;
            StoreFormInfo(formInfo);
        }

        /// <summary>
        /// store the info for the form, if the directory for insert the form doesn't exist the method create a directory
        /// </summary>
        /// <param name="info">contain the informationa about the form</param>
        public void StoreFormInfo(FormInfo info)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(DataDir + info.RequestInfo.CompilationRequestId);
            if (!dirInfo.Exists)
                dirInfo.Create();

            StoreInfo(info, dirInfo.FullName + @"\info.xml");
        }

        #endregion

        #region Private Methods

        private void ParseForm(DirectoryInfo basePath, out XmlSchema nodes, out XmlDocument edges, out XmlSchema types, out XmlDocument presentation, out XmlDocument result)
        {
            nodes = null;
            using (FileStream fs = new FileStream(basePath.FullName + @"\nodes.xsd", FileMode.Open, FileAccess.Read))
                nodes = XmlSchema.Read(fs, new ValidationEventHandler(delegate(object source, ValidationEventArgs args) { }));

            edges = new XmlDocument();
            using (FileStream fs = new FileStream(basePath.FullName + @"\edges.xml", FileMode.Open, FileAccess.Read))
                edges.Load(fs);

            presentation = new XmlDocument();
            using (FileStream fs = new FileStream(basePath.FullName + @"\presentation.xml", FileMode.Open, FileAccess.Read))
                presentation.Load(fs);

            result = null;
            if (File.Exists(basePath.FullName + @"\result.xml"))
            {
                result = new XmlDocument();
                using (FileStream fs = new FileStream(basePath.FullName + @"\result.xml", FileMode.Open, FileAccess.Read))
                    result.Load(fs);
            }

            using (FileStream fs = new FileStream(basePath.FullName + @"\types.xsd", FileMode.Open, FileAccess.Read))
                types = XmlSchema.Read(fs, new ValidationEventHandler(delegate(object source, ValidationEventArgs args) { }));

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(types);
            schemaSet.Compile();

            //Try to take the name of the workflow
            if (edges.DocumentElement.Attributes["workflowName"] != null)
            {
                FormInfo formInfo = LoadInfo(basePath + @"\info.xml");
                formInfo.Name = edges.DocumentElement.Attributes["workflowName"].Value;
                StoreFormInfo(formInfo);
            }
        }

        private void StoreForm(DirectoryInfo basePath, string nodes, string edges, string types, string presentation)
        {
            if (!basePath.Exists)
                basePath.Create();

            using (StreamWriter writer = new StreamWriter(
                new FileStream(basePath.FullName + @"\nodes.xsd", FileMode.Create, FileAccess.ReadWrite),
                Encoding.Unicode))
                writer.Write(nodes);

            using (StreamWriter writer = new StreamWriter(
                new FileStream(basePath.FullName + @"\types.xsd", FileMode.Create, FileAccess.ReadWrite),
                Encoding.Unicode))
                writer.Write(types);

            using (StreamWriter writer = new StreamWriter(
                new FileStream(basePath.FullName + @"\edges.xml", FileMode.Create, FileAccess.ReadWrite),
                Encoding.Unicode))
                writer.Write(edges);

            using (StreamWriter writer = new StreamWriter(
                new FileStream(basePath.FullName + @"\presentation.xml", FileMode.Create, FileAccess.ReadWrite),
                Encoding.Unicode))
                writer.Write(presentation);
        }

        private DataSetStatus CheckForm(DirectoryInfo basePath)
        {
            if (!Directory.Exists(basePath.FullName))
                return DataSetStatus.None;

            bool infoFound = false, nodesFound = false, presentationFound = false, edgesFound = false, typesFound = false, resultFound = false;

            DataSetStatus status = DataSetStatus.Notified;

            FileInfo[] files = basePath.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Name == "types.xsd")
                    typesFound = true;
                if (file.Name == "edges.xml")
                    edgesFound = true;
                if (file.Name == "nodes.xsd")
                    nodesFound = true;
                if (file.Name == "result.xml")
                    resultFound = true;
                if (file.Name == "presentation.xml")
                    presentationFound = true;
                if (file.Name == "info.xml")
                    infoFound = true;
            }

            if (infoFound)
            {
                status = DataSetStatus.Notified;
                if (nodesFound && edgesFound && typesFound && presentationFound)
                {
                    if (resultFound)
                        status = DataSetStatus.Opened;
                    else
                        status = DataSetStatus.Downloaded;
                }
            }
            else
                status = DataSetStatus.None;

            return status;
        }

        private FormInfo LoadInfo(String file)
        {
            FormInfo info;

            XmlSerializer serializer = new XmlSerializer(typeof(FormInfo));
            using (FileStream reader = new FileStream(file, FileMode.Open))
                info = serializer.Deserialize(reader) as FormInfo;

            return info;
        }

        private void StoreInfo(FormInfo info, String file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FormInfo));
            using (StreamWriter writer = new StreamWriter(
                new FileStream(file, FileMode.Create, FileAccess.ReadWrite)))
                serializer.Serialize(writer, info);
        }

        #endregion
    }
}
