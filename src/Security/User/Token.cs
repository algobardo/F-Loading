using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security
{
    public enum FormType
    {
        PUBLIC_WITH_REPLICATION,
        PUBLIC_WITHOUT_REPLICATION,
        PUBLIC_BY_SERVICE,
        PRIVATE_ANONYM,
        PRIVATE_NOT_ANONYM
    }

    /// <summary>
    /// token 
    /// </summary>
    public class Token : IToken
    {
        private User currentUser = null;
        private bool authenticated = false;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="loggeduser"> the current user</param>
        public Token(User loggeduser)
        {
            if (loggeduser != null)
            {
                authenticated = true;
                currentUser = loggeduser;
            }
        }

        public static ComputableWorkflowReference GetWorkflow(string wid, string crid, string username, string service, string token)
        {
            Storage.StorageManager sto = new Storage.StorageManager();
            int pubid = -1;
            int creqid = -1;
            try
            {
                pubid = int.Parse(wid);
                creqid = int.Parse(crid);
            }
            catch (Exception)
            { // Storage-style exception catching :P
                return null;
            }
            Storage.Publication pub = sto.getEntityByID<Storage.Publication>(pubid);
            if (pub == null)
                return null;
            FormType ftype = GetWorkflowType(pub);
            switch (ftype)
            {
                case FormType.PUBLIC_WITH_REPLICATION:
                    {
                        Storage.CompilationRequest creq = sto.getEntityByID<Storage.CompilationRequest>(creqid);
                        if (creq == null)
                            return null;
                        // Public, check strings
                        if (creq.publicationID == pub.publicationID &&
                            String.IsNullOrEmpty(username) &&
                            String.IsNullOrEmpty(service) &&
                            String.IsNullOrEmpty(token))
                        {
                            // All ok, create and returns the wf
                            return new ComputableWorkflowReference(pub.userID, pub.publicationID, pub.namePublication, pub.description, pub.themeID, creqid, false, true, pub.expirationDate);
                        }
                        else
                        {
                            // Public publication, but not null parameters, forged link
                            return null;
                        }
                    }
                case FormType.PUBLIC_WITHOUT_REPLICATION:
                    {
                        // Check with contact authentication
                        int serviceID = -1;
                        try
                        {
                            serviceID = int.Parse(service);
                        }
                        catch (Exception)
                        {
                            serviceID = -1;
                        }
                        if (serviceID == -1)
                        {
                            // Errore
                            return null;
                        }
                        Storage.Contact contact = sto.getContactByUserService(username, serviceID);
                        if (contact == null)
                        {
                            // All right, we create it now
                            contact = sto.addContact(username, serviceID, "Filler_Only_Contact");
                            if(contact==null)
                                // No way
                                return null;
                        }
                        // Get the right CompilationRequest
                        Storage.CompilationRequest creq = sto.getCompilationRequestByPulicationAndContact(contact.contactID, pub.publicationID);
                        if (creq == null)
                        {
                            // The user has surely not compiled it before
                            creq = sto.addContactToPublication(pub.publicationID, contact.contactID, "DUMMY_TOKEN");
                            if(creq==null)
                                // No way
                                return null;
                        }
                        if (creq.compiled)
                            // Already Compiled
                            return null;
                        else
                            return new ComputableWorkflowReference(pub.userID, pub.publicationID, pub.namePublication, pub.description, pub.themeID, creq.compilReqID, true, true, pub.expirationDate);
                    }
                case FormType.PUBLIC_BY_SERVICE:
                    {
                        // Check with contact authentication
                        int serviceID = -1;
                        try
                        {
                            serviceID = int.Parse(service);
                        }
                        catch (Exception)
                        {
                            serviceID = -1;
                        }
                        if (serviceID == -1)
                        {
                            // Errore
                            return null;
                        }
                        if (pub.Service.serviceID != serviceID)
                        {
                            // This serviceID is not allowed to fill the publication
                            return null;
                        }
                        Storage.Contact contact = sto.getContactByUserService(username, serviceID);
                        if (contact == null)
                        {
                            // All right, we create it now
                            contact = sto.addContact(username, serviceID, "Filler_Only_Contact");
                            if (contact == null)
                                // No way
                                return null;
                        }
                        // Get the right CompilationRequest
                        Storage.CompilationRequest creq = sto.getCompilationRequestByPulicationAndContact(contact.contactID, pub.publicationID);
                        if (creq == null)
                        {
                            // The user has surely not compiled it before
                            creq = sto.addContactToPublication(pub.publicationID, contact.contactID, "DUMMY_TOKEN");
                            if (creq == null)
                                // No way
                                return null;
                        }
                        if (creq.compiled)
                            // Already Compiled
                            return null;
                        else
                            return new ComputableWorkflowReference(pub.userID, pub.publicationID, pub.namePublication, pub.description, pub.themeID, creq.compilReqID, true, true, pub.expirationDate);
                    }
                case FormType.PRIVATE_NOT_ANONYM:
                case FormType.PRIVATE_ANONYM:
                    {
                        // Private, check strings
                        if (creqid != -1)
                        {
                            // Check with token
                            Storage.CompilationRequest creq = sto.getEntityByID<Storage.CompilationRequest>(creqid);
                            if (creq == null)
                                return null;
                            if (creq.publicationID==pub.publicationID &&
                                creq.token.Equals(token) &&
                                ((creq.Contact.nameContact).ToUpper()).Equals(username) &&
                                creq.Contact.Service.nameService.Equals(service) &&
                                !creq.compiled)
                            {
                                // Right compilation request, all done!
                                return new ComputableWorkflowReference(pub.userID, pub.publicationID, pub.namePublication, pub.description, pub.themeID, creqid, true, true, pub.expirationDate);
                            }
                            else
                            {
                                // Wrong authentication parameters
                                return null;
                            }
                        }
                        else
                        {
                            // Check with contact authentication
                            int serviceID = -1;
                            try
                            {
                                serviceID = int.Parse(service);
                            }
                            catch (Exception)
                            {
                                serviceID = -1;
                            }
                            if(serviceID==-1){
                                // Errore
                                return null;
                            }
                            Storage.Contact contact = sto.getContactByUserService(username, serviceID);
                            if (contact == null)
                            {
                                // In this case, if the contact doesn't exists, is an error
                                return null;
                            }
                            // Get the right CompilationRequest
                            Storage.CompilationRequest creq = sto.getCompilationRequestByPulicationAndContact(contact.contactID, pub.publicationID);
                            if (creq == null)
                            {
                                // L'utente non ha il permesso per riempire la form
                                return null;
                            }
                            if (creq.compiled)
                            {
                                // L'utente ha già inserito la form
                                return null;
                            }
                            else
                                return new ComputableWorkflowReference(pub.userID, pub.publicationID, pub.namePublication, pub.description, pub.themeID, creq.compilReqID, true, true, pub.expirationDate);
                        }
                    }
                default:
                    {
                        return null;
                    }
            }

        }

        private static FormType GetWorkflowType(Storage.Publication pub)
        {
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

        /// <summary>
        /// Get all public forms links with names and descriptions
        /// </summary>
        /// <returns>A Dictionary containing pairs of FormLink/&lt;FormName,FormDescription&rt;</returns>
        public static Dictionary<string, Storage.StorageManager.Pair<string,string>> GetPublicFormsLinks()
        {
            int fakeId = 1;
            Storage.StorageManager sto = new Storage.StorageManager();

            // We likes very long types
            List<Storage.StorageManager.Pair<
                Storage.StorageManager.PartialPublication,
                Storage.CompilationRequest
                >> completelypublicpubs = sto.getPublicationsCompilationRequestByContactID(fakeId);            
            List<Storage.StorageManager.PartialPublication> publicpubs = sto.getPublicationsPublic();
            Dictionary<string, Storage.StorageManager.Pair<string, string>> result = new Dictionary<string, Storage.StorageManager.Pair<string, string>>();
            int i = 0;
            foreach (Storage.StorageManager.PartialPublication pub in publicpubs)
            {
                Storage.CompilationRequest creq = null;
                if (pub.publicationID == completelypublicpubs[i].First.publicationID)
                {
                    // Completely public
                    creq = completelypublicpubs[i].Second;
                    i++;
                    // If reached the last one, stay there
                    if (i == completelypublicpubs.Count)
                        i--;
                }
                Storage.StorageManager.Pair<string, string> pair = new Storage.StorageManager.Pair<string, string>();
                pair.First = pub.namePublication;
                pair.Second = pub.descr;
                result.Add(ToLink(pub,creq), pair);
                Console.WriteLine(ToLink(pub, creq));
            }
            return result;
        }

        internal static string ToLink(Storage.Publication publication, Storage.CompilationRequest compilationRequest)
        {
            // Used only for public publications; for the private ones it's better to check
            Storage.StorageManager.PartialPublication ppublication = new Storage.StorageManager.PartialPublication();
            ppublication.publicationID = publication.publicationID;
            ppublication.externalServiceID = (int) publication.externalServiceID;
            ppublication.isPublic = publication.isPublic;

            return ToLink(ppublication, compilationRequest);
        }

        internal static string ToLink(Storage.StorageManager.PartialPublication publication, Storage.CompilationRequest compilationRequest)
        {
            string webServerAddress = Storage.StorageManager.getEnvValue("webServerAddress");
            string fillingPage = "/FormFillier/Filling.aspx";
            string ret = null;
            if (publication.isPublic)
            {
                if (compilationRequest == null)
                {
                    ret = "http://"
                        + webServerAddress + fillingPage
                        + "?WorkflowID=" + publication.publicationID
                        + "&CompilationRequestID=" + "-1";
                    if (publication.externalServiceID == 1)
                        ret += "&Service=-1";
                    else
                        ret += "&Service=" + publication.externalServiceID;
                }
                else
                {
                    ret = "http://"
                        + webServerAddress + fillingPage
                        + "?WorkflowID=" + publication.publicationID
                        + "&CompilationRequestID=" + compilationRequest.compilReqID;
                }
            }
            return ret;
        }    

        #region IToken Members

        /// <summary>
        /// get the authentication status of the current user
        /// </summary>
        /// <returns> true if authenticated, false otherwise</returns>
        public bool Authenticated
        {
            get { return authenticated; }
        }

        /// <summary>
        /// get the current user
        /// </summary>
        /// <returns>the current user</returns>
        public User GetCurrentUser()
        {
            return currentUser;
        }

        /// <summary>
        /// logs off the current user
        /// </summary>
        /// <returns>true if the current user was authenticated, false otherwise</returns>
        public bool Logout()
        {
            if (authenticated)
            {
                authenticated = false;
                currentUser = null;
                return true;
            }
            else
                return false;
        }

        #endregion
    }
}
