using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Schema;
using System.Xml;
using Core;
using Core.WF;
using System.IO;

namespace WebInterface
{
    public enum ResultStatus
    {
        OK,
        WRONG_COMPILATION_REQUEST_ID,
        WRONG_USERNAME_OR_SERVICE,
        WRONG_TOKEN,
        ALREADY_COMPILED
    }

    public class WorkflowInformations
    {
        public ResultStatus status;
        public WorkflowDescription description;
        public string publicationDescription;
    }
    
    /// <summary>
    /// Summary description for RemoteAccessPoint
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class RemoteAccessPoint : System.Web.Services.WebService
    {
        
        /// <summary>
        /// Get computableWorkflow information.
        /// </summary>
        /// <param name="compilationRequestId">Requested.</param>
        /// <param name="username">Required for private workflows, empty string otherwise</param>
        /// <param name="service">Required for private workflows, empty string otherwise</param>
        /// <param name="token">Required for private workflows, empty string otherwise</param>
        /// <returns></returns>
        [WebMethod(MessageName = "GetWorkflow")]
        public WorkflowInformations GetWorkflow(int compilationRequestId, string username, string service, string token)
        {
            Storage.StorageManager manager = new Storage.StorageManager();
            Storage.CompilationRequest compilationRequest = manager.getEntityByID<Storage.CompilationRequest>(compilationRequestId);

            WorkflowInformations infs = new WorkflowInformations();
            infs.status = ResultStatus.OK;

            if (compilationRequest == null)
            {
                infs.status = ResultStatus.WRONG_COMPILATION_REQUEST_ID;
                return infs;
            }			

            Storage.Publication publication = compilationRequest.Publication;
			Security.ComputableWorkflowReference cw = Security.Token.GetWorkflow("" + publication.publicationID, "" + compilationRequestId, username, service, token);
			if (cw == null)
			{
				infs.status = ResultStatus.WRONG_COMPILATION_REQUEST_ID;
				return infs;
			}

			infs.description =  cw.GetWorkflow().GetEntireWorkflowDescription();
			infs.publicationDescription = cw.GetWorkflowDescription();
			infs.status = ResultStatus.OK;

			/*
            if (publication.isPublic)
            {
                //pubblico
                IComputableWorkflow cw = (IComputableWorkflow)manager.getWorkflowByPublication(publication);
                cw.setWFname(publication.namePublication);
                infs.description = cw.GetEntireWorkflowDescription();
                infs.publicationDescription = publication.description;
            }
            else
            {
                //privato
                if (token==null || !compilationRequest.token.Equals(token))
                {
                    infs.status = ResultStatus.WRONG_TOKEN;
                    return infs;
                }
                Storage.Contact contact = compilationRequest.Contact;
                if (username==null || service == null ||
                    !(contact.externalUserID.Equals(username) && (contact.Service.nameService.Equals(service))))
                {
                    infs.status = ResultStatus.WRONG_USERNAME_OR_SERVICE;
                    return infs;
                }
                if (compilationRequest.compiled)
                {
                    infs.status = ResultStatus.ALREADY_COMPILED;
                }
                IComputableWorkflow cw = (IComputableWorkflow)manager.getWorkflowByPublication(publication);
                cw.setWFname(publication.namePublication);                
                infs.description = cw.GetEntireWorkflowDescription();
                infs.publicationDescription = publication.description;
            }
			*/
            return infs;
        }

        [WebMethod(MessageName = "SendFilledDocument")]
        public bool SendFilledDocument(int compilationRequestId, string username, string service, string token, string dataStr)
        {
            XmlDocument data = new XmlDocument();
            data.LoadXml(dataStr);

            Storage.StorageManager manager = new Storage.StorageManager();
            Storage.CompilationRequest compilationRequest = manager.getEntityByID<Storage.CompilationRequest>(compilationRequestId);
            if (compilationRequest == null)
            {
                return false;
            }
            Storage.Publication publication = compilationRequest.Publication;

            IComputableWorkflow cw;

            if (publication.isPublic)
            {
                //pubblico
                cw = (IComputableWorkflow)manager.getWorkflowByPublication(publication);                
            }
            else
            {
                //privato
                if (token==null || !compilationRequest.token.Equals(token))
                {
                    return false;
                }
                Storage.Contact contact = compilationRequest.Contact;
                if (username == null || service == null ||
                    !(contact.externalUserID.Equals(username) && (contact.Service.nameService.Equals(service))))
                {
                    return false;
                }
                if (compilationRequest.compiled)
                {
                    return false;
                }
                cw = (IComputableWorkflow)manager.getWorkflowByPublication(publication);
                
            }
            cw.setWFname(publication.namePublication);

            System.Xml.Schema.XmlSchemaSet schema = cw.GetCollectedDocumentSchemas();
            data.Schemas = schema;
            try
            {
                data.Validate(null);
            }
            catch (XmlSchemaValidationException)
            {
                return false;
            }
            Storage.Result res = null;
            System.Xml.Linq.XElement dataXElement = Storage.StorageManager.xmlDocumentToXElement(data);
            res = (Storage.Result)manager.addResult(compilationRequest.compilReqID, dataXElement);
            if (res == null)
                return false;
            else
                return true;            
        }
    }
}
