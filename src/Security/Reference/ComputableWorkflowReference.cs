using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.WF;
using System.IO;
using System.Drawing;
using Storage;
using Comm;
//using Comm.Services;

namespace Security
{
    public class ComputableWorkflowReference : IWorkflowThemeReference
    {
        
        private int defaultthemeid = Int32.Parse(StorageManager.getEnvValue("defaultThemeId"));

        // Real code
        private bool isPrivate;

        private bool filling;

        private int userId;

        private int publicationId;

        private int themeId;

        private int compilationReqId;

        private bool isDefTheme;

        private string wfName;

        private string wfDescription;

        private IComputableWorkflow cwf_cache;

        private StorageManager sto; 

        private bool complete;

        private DateTime expirationDate;


        internal ComputableWorkflowReference(int userId, int publicationId, string wfName, string wfDescription, bool filling, DateTime expiry)
        {   
            this.sto = new StorageManager();
            this.userId = userId;
            this.publicationId = publicationId;
            this.wfName = wfName;
            this.wfDescription = wfDescription;

            this.filling = filling;
            complete = false;


            this.themeId = -1;
            this.isPrivate = false;
            this.compilationReqId = -1;
            cwf_cache = null;

            this.expirationDate = expiry; 
            
        }

        /// <summary>
        /// WorkflowReference constructor, usable only inside Security package
        /// NDR: Takes a great number of parameters to not make a DB access
        /// </summary>
        /// <param name="userId">The owner's userId</param>
        /// <param name="publicationId">The Storage Publication id</param>
        /// <param name="wfName">The workflow name</param>
        /// <param name="wfName">The workflow description</param>
        /// <param name="themeId">the theme id</param>
        /// <param name="creqid">The Storage Compilation Request id</param>
        /// <param name="isPrivate"><value>true</value>if is a private form, <value>false</value>otherwise</param>
        /// <param name="filling"><value>true</value>if the form is currently fillable, <value>false</value> otherwise</param>
        internal ComputableWorkflowReference(int userId, int publicationId, string wfName, string wfDescription, int themeId, int creqid, bool isPrivate, bool filling, DateTime expiry)
        {   
            this.sto = new StorageManager();
            this.userId = userId;
            this.publicationId = publicationId;
            this.wfName = wfName;
            this.wfDescription = wfDescription;
            this.themeId = themeId;
            this.isPrivate = isPrivate;
            this.filling = filling;
            this.compilationReqId = creqid;
            complete = true;
            this.expirationDate = expiry;

            if (themeId == defaultthemeid) 
                isDefTheme = true;
            else
                isDefTheme = false;

            cwf_cache = null;
        }

        /// <summary>
        /// Private method, used to retrieve wf from db and put into the object cache
        /// </summary>
        /// <returns><value>true</value>on success,<value>false</value> otherwise</returns>
        private bool _RetrieveWf(){
            if (cwf_cache == null)
            {
                // Must retrieve it from db
                
                Storage.Publication pub = sto.getEntityByID<Publication>(publicationId);
                if (pub == null)
                    return false;
                cwf_cache = (IComputableWorkflow)sto.byteArray2Object(pub.xml.ToArray());
                if (cwf_cache == null)
                    return false;
                cwf_cache.setWFname(pub.namePublication);
                return true;
            }
            else
                return false;
        }        

        /// <summary>
        /// Return the name of the Computable Workflow pointed by this reference
        /// </summary>
        /// <returns>a <see cref="string"/> containing the name of the pointed workflow</returns>
        public string GetWorkflowName()
        {
            return wfName;
        }

        /// <summary>
        /// Return the description of the Computable Workflow pointed by this reference
        /// </summary>
        /// <returns>a <see cref="string"/> containing the description of the pointed workflow</returns>
        public string GetWorkflowDescription()
        {
            return wfDescription;
        }

        public DateTime GetWorkflowExpirationDate()
        {
            return expirationDate;
        }

        /// <summary>
        /// Return the Computable Workflow pointed by this ComputableWorkflowReference
        /// </summary>
        /// <returns>The <see cref="Core.WF.IComputableWorkflow"/> Workflow pointed;<value>null</value>if the workflow cannot be retrieved</returns>
        public Core.WF.IComputableWorkflow GetWorkflow()
        {
            if (cwf_cache == null)
                _RetrieveWf();
            return cwf_cache;
        }

        /// <summary>
        /// Authorizes a list of contacts to fill this ComputableWorkflow.
        /// This Method can be used only in the _NOT_ filling case
        /// This Method can be used only for private Workflows
        /// </summary>
        /// <param name="contactList">The list of <see cref="Security.Contact"/>s to be allowed</param>
        /// <returns><value>true</value> on success, <value>false</value>otherwise (including operation not permitted)</returns>
        public bool PermitContacts(List<Security.Contact> contactList){
            if (!complete)
                fillReference();

            if (!isPrivate || filling)
                return false;

            if (contactList == null)
                return false;
            
            LoaMailSender mailSender = new LoaMailSender();
                       
			List<Security.Contact> rejected = new List<Contact>();

            foreach (Security.Contact contact in contactList)
            {
                string token = RandomStringGenerator.GetRandomString(10);
                Storage.CompilationRequest creq = sto.addContactToPublication(this.publicationId, contact.ContactID, token);
				if (creq == null)
				{
                    creq = sto.getCompilationRequestByPulicationAndContact(contact.ContactID, publicationId);
                    if (creq == null)
						return false;					
				}

                Storage.Service s = sto.getEntityByID<Storage.Service>(contact.Service.ServiceId);
                if (s == null)
                    return false;
                
                // Check if the service uses emails and if the email seems correct
				
				if (s.externalUserIDMail)
				{
					try
					{
						System.Net.Mail.MailAddress address = new System.Net.Mail.MailAddress(contact.Email);
						LoaMail mail = new LoaMail(address, GetWorkflowName(), creq);
						if (!mailSender.SendMail(mail))
						{
							rejected.Add(contact);
						}
					}
					catch (Exception)
					{
						rejected.Add(contact);
					}					
				}
				else
				{
					rejected.Add(contact);
				}
            }


			if (rejected.Count>0)
            {
				Storage.User user = sto.getEntityByID<Storage.User>(userId);
                //Genero email per creatore
				try
				{
					System.Net.Mail.MailAddress userMail = new System.Net.Mail.MailAddress(user.mail);
					MailToFormCreator mail = new MailToFormCreator(userMail, GetWorkflowName(), publicationId, rejected);
					LoaMailSender sender = new LoaMailSender();
					return (sender.SendMail(mail));
				}
				catch 
				{
					//non dovrebbe mai arrivare qui, la mail dello user e' controllata in fase di registrazione
                    return false;
				}
            }            
            return true;
        }

        /// <summary>
        /// Saves the filling contained into the ComputableWorkflow associated onto the DB
        /// This Method can be used only in the filling case
        /// </summary>
        /// <returns><value>true</value>on success, <value>false</value>otherwise</returns>
        public bool SaveFilling()
        {
            if (!complete)
                fillReference();

            if (!filling)
                return false;

            if (compilationReqId == -1)
                return false;

            if (cwf_cache == null)
                // If we don't have a Computable Workflow, we can't save the filling
                return false;

            System.Xml.XmlDocument doc = cwf_cache.GetCollectedDocument();

            object res = sto.addResult(compilationReqId, StorageManager.xmlDocumentToXElement(doc));
            
            if (res == null)
                return false;
            else
            {
                Publication pub = sto.getEntityByID<Publication>(publicationId);
                if (pub!= null && !String.IsNullOrEmpty(pub.URIUpload))
                    CommunicationService.Instance.Send((Result)res);             
                return true;
            }
        }

        //is it possible to delete computable workflows??? if not delete this method..
        public bool Remove()
        {             
            if (sto.removeEntity<Storage.Publication>(publicationId))
            {
                isPrivate=false;
                filling=false;
                userId=-1;
                publicationId=-1;
                themeId=-1;
                compilationReqId=-1;
                isDefTheme=false;
                cwf_cache=null;

                return true;
            }
            else
                return false;
        }

        #region IWorkflowThemeReference Members

        public SetThemeResult SetTheme(Theme theme)
        {
            if (!complete)
                fillReference();

            Storage.Theme stotheme;
            if (isDefTheme)
            {
                // Duplicate the default them
                stotheme = duptheme(themeId);
                isDefTheme = false;
            }
            else
            {
                // Take the current theme
                stotheme = sto.getEntityByID<Storage.Theme>(themeId);
            }
            if (stotheme == null)
            {
                // Error, theme id not valid
                return new SetThemeResult(SetThemeResult.Result.STATUS_ERROR_THEME, null);
            }

            // Edit the theme
            if (theme.Title != null)
                stotheme.themeTitle = theme.Title;
            if (theme.CSS != null)
                stotheme.CSS = theme.CSS;
            if (theme.Image != null)
            {
                MemoryStream ms = new MemoryStream();
                theme.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                stotheme.logo = ms.ToArray();
            }

            Security.Theme newtheme = new Theme(stotheme.themeTitle, Image.FromStream(new MemoryStream(stotheme.logo.ToArray())), stotheme.CSS);

            if (sto.commit())
                return new SetThemeResult(SetThemeResult.Result.STATUS_OK, newtheme);
            else
                return new SetThemeResult(SetThemeResult.Result.STATUS_ERROR_THEME, null);
        }

        public SetThemeResult SetImage(System.Drawing.Image image)
        {
            if (!complete)
                fillReference();

            Storage.Theme stotheme;
            if (isDefTheme)
            {
                // Duplicate the default them
                stotheme = duptheme(themeId);
                isDefTheme = false;
            }
            else
            {
                // Take the current theme
                stotheme = sto.getEntityByID<Storage.Theme>(themeId);
            }
            if (stotheme == null)
            {
                // Error, theme id not valid
                return new SetThemeResult(SetThemeResult.Result.STATUS_ERROR_THEME, null);
            }

            // Edit the theme
            if (image != null)
            {
                MemoryStream ms = new MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                stotheme.logo = ms.ToArray();
            }

            Security.Theme newtheme = new Theme(stotheme.themeTitle, System.Drawing.Image.FromStream(new MemoryStream(stotheme.logo.ToArray())), stotheme.CSS);

            if (sto.commit())
                return new SetThemeResult(SetThemeResult.Result.STATUS_OK, newtheme);
            else
                return new SetThemeResult(SetThemeResult.Result.STATUS_ERROR_IMAGE, null);
        }

        public SetThemeResult SetCSS(string css)
        {
            if (!complete)
                fillReference();
            
            Storage.Theme stotheme;
            if (isDefTheme)
            {
                // Duplicate the default them
                stotheme = duptheme(themeId);
                isDefTheme = false;
            }
            else
            {
                // Take the current theme
                stotheme = sto.getEntityByID<Storage.Theme>(themeId);
            }
            if (stotheme == null)
            {
                // Error, theme id not valid
                return new SetThemeResult(SetThemeResult.Result.STATUS_ERROR_THEME, null);
            }

            // Edit the theme
            if (css != null)
                stotheme.CSS = css;

            Security.Theme newtheme = new Theme(stotheme.themeTitle, System.Drawing.Image.FromStream(new MemoryStream(stotheme.logo.ToArray())), stotheme.CSS);

            if (sto.commit())
                return new SetThemeResult(SetThemeResult.Result.STATUS_OK, newtheme);
            else
                return new SetThemeResult(SetThemeResult.Result.STATUS_ERROR_CSS, null);
        }

        public SetThemeResult SetTitle(string title)
        {
            if (!complete)
                fillReference();
            
            Storage.Theme stotheme;
            if (isDefTheme)
            {
                // Duplicate the default them
                stotheme = duptheme(themeId);
                isDefTheme = false;
            }
            else
            {
                // Take the current theme
                stotheme = sto.getEntityByID<Storage.Theme>(themeId);
            }
            if (stotheme == null)
            {
                // Error, theme id not valid
                return new SetThemeResult(SetThemeResult.Result.STATUS_ERROR_THEME, null);
            }

            // Edit the theme
            if (title != null)
                stotheme.themeTitle = title;

            Security.Theme newtheme = new Theme(stotheme.themeTitle, System.Drawing.Image.FromStream(new MemoryStream(stotheme.logo.ToArray())), stotheme.CSS);

            if (sto.commit())
                return new SetThemeResult(SetThemeResult.Result.STATUS_OK, newtheme);
            else
                return new SetThemeResult(SetThemeResult.Result.STATUS_ERROR_TITLE, newtheme);
        }

        public Theme GetTheme()
        {
            if (!complete)
                fillReference();

            // Take current theme
            Storage.Theme stotheme = sto.getEntityByID<Storage.Theme>(themeId);
            if (stotheme == null)
                return null;

            Security.Theme newtheme = new Theme(stotheme.themeTitle, System.Drawing.Image.FromStream(new MemoryStream(stotheme.logo.ToArray())), stotheme.CSS);

            return newtheme;
        }
        
        public FormType GetFormType()
        {
             
            Storage.Publication pub = sto.getEntityByID<Storage.Publication>(publicationId);

            bool isPublic = pub.isPublic;
            bool isAnonymous = pub.anonymResult;
            bool isResultReplicated = pub.isResultReplicated;

            if (isPublic)
            {
                if (isResultReplicated) return FormType.PUBLIC_WITH_REPLICATION;
                else
                    if (pub.Service.serviceID != 1)
                        return FormType.PUBLIC_BY_SERVICE;
                    else
                        return FormType.PUBLIC_WITHOUT_REPLICATION;
            }
            else
            {
                if (isAnonymous)
                    return FormType.PRIVATE_ANONYM;
                else
                    return FormType.PRIVATE_NOT_ANONYM;
            }
        }
        
        private bool fillReference(){
            StorageManager sto = new StorageManager();
            Storage.Publication pub = sto.getEntityByID<Publication>(publicationId);
            if (pub == null)
                return false;
            this.themeId = pub.themeID;
            
            this.isPrivate = !pub.isPublic;
            if (themeId == defaultthemeid)
                isDefTheme = true;
            else
                isDefTheme = false;
            if (filling)
            {
                // Find the compilationRequest
                Storage.CompilationRequest creq = sto.getCompilationRequestByPulicationAndContact(userId, publicationId);
                if (creq == null)
                    return false;
                else
                {
                    compilationReqId = creq.compilReqID;
                }                 
            }
            this.complete = true;
            return true;
        }

        private Storage.Theme duptheme(int themeId)
        {
            Storage.Theme theme = sto.getEntityByID<Storage.Theme>(themeId);
            if (theme == null)
                return null;
            Storage.Theme result = sto.addTheme(userId, theme.themeTitle, theme.CSS, Image.FromStream(new MemoryStream(theme.logo.ToArray())));
            Storage.Publication pub = sto.getEntityByID<Storage.Publication>(publicationId);
            if (result != null && pub != null)
            {
                this.themeId = result.themeID;
                pub.themeID = result.themeID;
                if (sto.commit())
                    return result;
                else
                    return null;
            }
            else
                return null;
        }

        #endregion
    }
}
