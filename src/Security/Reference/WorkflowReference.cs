using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.WF;
using Storage;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Drawing;
using System.Globalization;
using Comm;

namespace Security
{
    public class WorkflowReference : IWorkflowThemeReference
    {
        private int defaultthemeid = Int32.Parse(StorageManager.getEnvValue("defaultThemeId"));

        private int userId;

        private int modelId;

        private int themeId;

        private bool isDefTheme;

        private string wfName;

        private string wfDescription;

        private Workflow wf_cache;

        private bool complete;

        private Storage.StorageManager sto;

        /// <summary>
        /// WorkflowReference constructor, usable only inside Security package
        /// </summary>
        /// <param name="userId">The owner's userId</param>
        /// <param name="modelId">The model id</param>
        /// <param name="wfName">The model name</param>
        /// <param name="wfDescription">The model description</param>
        internal WorkflowReference(int userId, int modelId, string wfName, string wfDescription)
        {
            sto = new Storage.StorageManager();
            this.userId = userId;
            this.modelId = modelId;
            this.wfName = wfName;
            this.wfDescription = wfDescription;
            this.themeId = -1;
            this.complete = false;
            wf_cache = null;
        }

        /// <summary>
        /// WorkflowReference constructor, usable only inside Security package
        /// </summary>
        /// <param name="userId">The owner's userId</param>
        /// <param name="modelId">The model id</param>
        /// <param name="wfName">The model name</param>
        /// <param name="wfDescription">The model description</param>
        /// <param name="themeId">The theme id</param>
        internal WorkflowReference(int userId, int modelId, string wfName, string wfDescription, int themeId)
        {
            sto = new Storage.StorageManager();
            this.userId = userId;
            this.modelId = modelId;
            this.wfName = wfName;
            this.wfDescription = wfDescription;
            this.themeId = themeId;
            this.complete = true;

            if (themeId == defaultthemeid)
                isDefTheme = true;
            else
                isDefTheme = false;


            wf_cache = null;
        }

        /// <summary>
        /// Private method, used to retrieve wf from db and put into the object cache
        /// </summary
        /// <returns><value>true</value>on success,<value>false</value> otherwise</returns>
        private bool _RetrieveWf()
        {
            if (wf_cache == null)
            {
                // Must retrieve it from db
                StorageManager sto = new StorageManager();
                Storage.Model mod = sto.getEntityByID<Model>(modelId);
                if (mod == null)
                    return false;
                wf_cache = (Workflow)sto.byteArray2Object(mod.xml.ToArray());
                if (wf_cache == null)
                    return false;
                wf_cache.WorkflowName = mod.nameModel;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Return the workflow pointed by this WorkflowReference
        /// </summary>
        /// <returns>The <see cref="Core.WF.Workflow"/> Workflow pointed;<value>null</value>if the workflow cannot be retrieved</returns>
        public Core.WF.Workflow GetWorkflow()
        {
            if (wf_cache == null)
            {
                if (!_RetrieveWf())
                    return null;
            }
            return wf_cache;
        }

        /// <summary>
        /// Update the workflow pointed by this WorkflowReference
        /// </summary>
        /// <param name="newwf">The (new or modified) <see cref="Core.WF.Workflow"/> Workflow </param>
        /// <returns><value>true</value>on success,<value>false</value> otherwise</returns>
        public bool UpdateWorkflow(Core.WF.Workflow newwf)
        {

            Model mod = sto.getEntityByID<Model>(modelId);
            if (mod != null)
            {
                mod.xml = sto.object2ByteArray(newwf);
                mod.nameModel = newwf.WorkflowName;
                if (sto.commit())
                {
                    //Ok, update cached object
                    wf_cache = newwf;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Return the name of the Workflow pointed by this reference
        /// </summary>
        /// <returns>a <see cref="string"/> containing the name of the pointed workflow</returns>
        public string GetWorkflowName()
        {
            return wfName;
        }

        /// <summary>
        /// Return the description of the Workflow pointed by this reference
        /// </summary>
        /// <returns>a <see cref="string"/> containing the description of the pointed workflow</returns>
        public string GetWorkflowDescription()
        {
            return wfDescription;
        }

        /// <summary>
        /// Make the workflow pointed by this WorkflowReference computable
        /// </summary>
        /// <param name="ftype">The <see cref="Security.FormType"/>of the computable workflow</param>
        /// <param name="serviceId"> The serviceId allowed to fill this workflow</param>
        /// <param name="publicationDescription"> The workflowdescription of the publication, empty string for empty description</param>
        /// <param name="expireDate"> The expire date gg/mm/aaaa</param>
		/// <param name="communicationUri"> The uri used by communication package</param>
        /// <returns>A <see cref="Security.ComputableWorkflowReference"/> pointing the newly created <see cref="Core.WF.IComputableWorkflow"/>; <value>null</value> if an error happens</returns>		
		public ComputableWorkflowReference MakeComputable(FormType ftype, int serviceId, string publicationDescription, string expireDate, string communicationUri)
        {
            if (publicationDescription == null)
                return null;
            if (!complete)
                fillReference();

            bool publicp = ((ftype == FormType.PUBLIC_BY_SERVICE) || (ftype == FormType.PUBLIC_WITH_REPLICATION) || (ftype == FormType.PUBLIC_WITHOUT_REPLICATION));
            bool anonym = ((ftype == FormType.PRIVATE_ANONYM) || (ftype == FormType.PUBLIC_BY_SERVICE) || (ftype == FormType.PUBLIC_WITH_REPLICATION) || (ftype == FormType.PUBLIC_WITHOUT_REPLICATION));
            bool resultReplicated = (ftype == FormType.PUBLIC_WITH_REPLICATION);

            if (ftype == FormType.PUBLIC_BY_SERVICE && (serviceId == 1))
            {
                // Not allowed, we need a valid serviceId
                return null;
            }


            if (wf_cache == null)
            {
                if (!_RetrieveWf())
                    return null;
            }

            IFormatProvider provider = CultureInfo.InvariantCulture;
            DateTime date;
            if (!DateTime.TryParse(expireDate, provider, DateTimeStyles.AdjustToUniversal, out date))
            {
                provider = CultureInfo.GetCultureInfo("it-IT");
                if (!DateTime.TryParse(expireDate, provider, DateTimeStyles.AdjustToUniversal,out date))
                {
                    date = DateTime.Now.AddMonths(1);
                }
            }

            Publication pub = sto.publish(modelId, DateTime.Now, date, "", wf_cache.Save(), anonym, publicp, resultReplicated, serviceId, communicationUri, "Inutile", publicationDescription);

            if (pub == null)
                return null;

            // Make it public
            int creqid = -1;
            if (publicp)
            {
                CompilationRequest creq = null;
                if (resultReplicated)
                {
                    creq = sto.addPublicPublication(pub.publicationID);
                    if (creq == null)
                        return null;
                    creqid = creq.compilReqID;
                }

				Storage.User user = sto.getEntityByID<Storage.User>(userId);
                //Genero email per creatore
				try
				{
					System.Net.Mail.MailAddress userMail = new System.Net.Mail.MailAddress(user.mail);
					MailForPublicForm mail = new MailForPublicForm(userMail, pub, creq);
					LoaMailSender sender = new LoaMailSender();
					sender.SendMail(mail);
				}
				catch 
				{
					//non dovrebbe mai arrivare qui visto che la mail dell'utente e' controllata in fase di registrazione
                    return null;
				}
            }
            if (!String.IsNullOrEmpty(communicationUri))
                CommunicationService.Instance.Setup(pub);

            return new ComputableWorkflowReference(userId, pub.publicationID, pub.namePublication, pub.description, themeId, creqid, !publicp, false, date);
        }

        public bool Remove()
        {
            if (sto.removeEntity<Storage.Model>(modelId))
            {
                wf_cache = null;
                modelId = -1;
                userId = -1;
                themeId = -1;
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

        private Storage.Theme duptheme(int themeId)
        {
            Storage.Theme theme = sto.getEntityByID<Storage.Theme>(themeId);
            if (theme == null)
                return null;
            Storage.Theme result = sto.addTheme(userId, theme.themeTitle, theme.CSS, Image.FromStream(new MemoryStream(theme.logo.ToArray())));
            Storage.Model mod = sto.getEntityByID<Storage.Model>(modelId);
            if (result != null && mod != null)
            {
                this.themeId = result.themeID;
                mod.themeID = result.themeID;
                if (sto.commit())
                    return result;
                else
                    return null;
            }
            else
                return null;
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

        #endregion

        private bool fillReference()
        {
            StorageManager sto = new StorageManager();
            Storage.Model mod = sto.getEntityByID<Storage.Model>(modelId);
            if (mod == null)
                return false;
            this.themeId = mod.themeID;
            if (themeId == defaultthemeid)
                isDefTheme = true;
            else
                isDefTheme = false;
            this.complete = true;
            return true;
        }
    }
}
