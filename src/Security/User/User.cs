using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Core.WF;

namespace Security
{
    public class User
    {
        // Default theme id, for now hard-coded
        private int defaultthemeid = Int32.Parse(Storage.StorageManager.getEnvValue("defaultThemeId"));
        private string _username;
        private string _email;
        private Dictionary<string, List<Contact>> _contactLists;
        private Dictionary<int, Auth.ILoginService> _loggedServices;
        public static string defGroup = Storage.StorageManager.getEnvValue("otherContacts");

        /// <summary>
        /// Return all services which the current user logged on
        /// </summary>
        public List<Service> LoggedServices
        {
            get
            {
                List<Service> allservices = ExternalService.List();
                List<Service> ret = new List<Service>();
                foreach (int serviceID in _loggedServices.Keys)
                {
                    foreach (Service service in allservices)
                    {
                        if (service.ServiceId == serviceID)
                            ret.Add(service);
                    }
                }
                return ret;
            }
        }

        private int _userId;
        /// <summary>
        /// The user id on the database. Return -1 if not yet registered.
        /// </summary>
        public int UserId
        {
            get
            {
                if (!_registered)
                    return -1;
                return _userId;
            }
        }

        private bool _registered;
        /// <summary>
        /// It's true if the user accepted the disclaimer. It's already saved on the database.
        /// </summary>
        public bool Registered
        {
            get { return _registered; }
        }

        /// <summary>
        /// User constructor. Parameters meaning is different in the case of registered or not user.
        /// </summary>
        /// <param name="UserId">The userId of the registered user, -1 if not yet registered</param>
        /// <param name="Username">The username of the registered user, or the username of the first login for this user in the current session</param>
        /// <param name="Email">The email of the registered user, empty string otherwise</param>
        public User(int UserId, string Username, string Email)
        {
            _userId = UserId;
            _username = Username;
            _contactLists = new Dictionary<string, List<Contact>>();
            _loggedServices = new Dictionary<int, Security.Auth.ILoginService>();
            if (UserId != -1)
            {
                _registered = true;
                _email = Email;
            }
            else
                _registered = false;
        }

        /// <summary>
        /// Return the username of the registered user, or the username of the first login for this user in the current session
        /// </summary>
        /// <returns>The username of the registered user, or the username of the first login for this user in the current session</returns>
        public override string ToString()
        {
            return _username;
        }

        /// <summary>
        /// Return the username of the registered user, or the username of the first login for this user in the current session
        /// </summary>
        /// <returns>The username of the registered user, or the username of the first login for this user in the current session</returns>
        public string GetNickname()
        {
            return _username;
        }

        /// <summary>
        /// Return the main email provided during registration, null if not yet registered.
        /// </summary>
        /// <returns>Return the main email provided during registration, null if not yet registered</returns>
        public string GetEmail() 
        {
            if (!_registered)
                return null;
            return _email;
        }

        #region AuthenticationMethods

		public enum RegisterMeResult {OK, E_NICKNAME, E_EMAIL, E_GENERIC};

		public bool RegisterMe(string Nickname, string Email)
		{
			string error;
			return (RegisterMe(Nickname, Email, out error)==RegisterMeResult.OK);
		}
        /// <summary>
        /// Register the user on the database saving all external accounts that the user subscribed. On success, UserID property will be changed.
        /// </summary>
        /// <param name="Nickname">The nickname of the user</param>
        /// <param name="Email">The email of the user</param>        
        /// <returns>True on succes or if already registered, false otherwise (in this case nothing is saved on the database)</returns>
		public RegisterMeResult RegisterMe(string Nickname, string Email, out string messageError)
        {
			messageError = "";
            if (_registered)
				return RegisterMeResult.OK;
			if (String.IsNullOrEmpty(Nickname))
			{
				messageError = "Nickname is not valid.";
				return RegisterMeResult.E_EMAIL;
			}
			if (String.IsNullOrEmpty(Email)) 
			{
				messageError = "Email is not valid.";
				return RegisterMeResult.E_EMAIL;
			}
            try
            {
                new System.Net.Mail.MailAddress(Email);
            }
            catch (Exception) 
			{
				messageError = "Email is not valid.";
				return RegisterMeResult.E_EMAIL;
			}
            Storage.StorageManager sto = new Storage.StorageManager();
			Storage.User tempUser = sto.addUser(Nickname, Email);
			if (tempUser == null)
			{
				messageError = "An error has occured saving the new user on the database.";
				return RegisterMeResult.E_GENERIC;
			}
            _userId = tempUser.userID;

            List<string> groupNames = new List<string>();
			groupNames.Add(defGroup);
        
            List<Storage.Group> groupResult = sto.addGroups(groupNames, _userId);
            if (groupResult == null)
            {
                //rollback
                sto.removeEntity<Storage.User>(_userId);
                _userId = -1;
				{
					messageError = "An error has occured during the creation of default group.";
					return RegisterMeResult.E_GENERIC;
				}
            }
            // Save all externalAccount
            foreach (int serviceID in _loggedServices.Keys)
            {
                Storage.ExternalAccount extAcc = sto.addExternalAccount(_userId, _loggedServices[serviceID].getUsername(), serviceID);
                if (extAcc == null)
                {
                    //rollback
                    sto.removeEntity<Storage.User>(_userId);
                    sto.removeEntity<Storage.Group>(groupResult.First().groupID);
                    _userId = -1;
					messageError = "An error has occured saving currently logged accounts.";
					return RegisterMeResult.E_GENERIC;					
                }
            }

            _username = Nickname;            
            _email = Email;
            _registered = true;

			return RegisterMeResult.OK;
        }

        /// <summary>
        /// Return all services that the user subscribed.
        /// </summary>
        /// <returns>all services that the user subscribed</returns>
        public List<Service> GetSubscribedServices()
        {
            if (!_registered)
                return null;
            Storage.StorageManager sto = new Storage.StorageManager();
            Storage.User us = sto.getEntityByID<Storage.User>(_userId);
            //TODO: aspettare metodo getSubscribedService da storage
            List<Service> list = ExternalService.List();
            return list;
        }

        /// <summary>
        /// Add the reference to the service manager to the logged service list.
        /// </summary>
        /// <param name="serviceId">the service id</param>
        /// <param name="service">the service manager</param>
        public void AddLoggedService(int serviceId, Auth.ILoginService service)
        {
            try
            {
                _loggedServices.Add(serviceId, service);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Indicates if login is needed for contacts fetching from the service
        /// </summary>
        /// <param name="serviceId">ID of the service</param>
        /// <returns>A bool that indicates whether login is needed</returns>
        public bool LoginNeeded(int serviceId)
        {
            if (!_registered)
                return true;
            Auth.ILoginService serv;
            try
            {
                serv = _loggedServices[serviceId];
            }
            catch { return true; }
            if (serv == null) return true;
            return !serv.isLoginValid();
        }

        #endregion

        #region WorkflowMethods
        /// <summary>
        /// Return all editable workflow.
        /// </summary>
        /// <returns>all editable workflow</returns>
        public List<WorkflowReference> GetEditableWorkflows()
        {
            List<WorkflowReference> result = new List<WorkflowReference>();
            if (!_registered)
                return result;            
            Storage.StorageManager sto = new Storage.StorageManager();
            SortedList<int,Storage.StorageManager.Pair<string,string>> modlist = sto.getModelsByUserID(_userId);
            foreach (KeyValuePair<int, Storage.StorageManager.Pair<string, string>> models in modlist)
            {
                result.Add(new WorkflowReference(_userId, models.Key, models.Value.First, models.Value.Second));
            }
            return result;
        }

        /// <summary>
        /// Return all computable workflow.
        /// </summary>
        /// <returns>all computable workflow</returns>
        public List<ComputableWorkflowReference> GetComputableWorkflows()
        {
            List<ComputableWorkflowReference> result = new List<ComputableWorkflowReference>();
            if (!_registered)
                return result;
            
             Storage.StorageManager sto = new Storage.StorageManager();

             List<Storage.StorageManager.PartialPublication> publist = sto.getPublicationsByUserID2(_userId);
                foreach (Storage.StorageManager.PartialPublication publication in publist)
                {
                    result.Add(new ComputableWorkflowReference(_userId, publication.publicationID,
                    publication.namePublication, publication.descr, false, publication.expirationDate));
                }
        
            
            return result;
        }

        
        /// <summary>
        /// FORSE QUESTO METODO NON SERVE
        /// SERVE ECCOMEEEE!!
        /// </summary>
        /// <returns>una lista vuota</returns>
        public List<ComputableWorkflowReference> GetToBeCompiledWorkflows()
        {
            List<ComputableWorkflowReference> ret = new List<ComputableWorkflowReference>();

            if (!_registered)
                return ret;
                        
            Storage.StorageManager db = new Storage.StorageManager();
            Storage.User user = db.getEntityByID<Storage.User>(_userId);
            List<Storage.StorageManager.Pair<string,int>> tempcontacts = new List<Storage.StorageManager.Pair<string,int>>();
            foreach (Storage.ExternalAccount extAcc in user.ExternalAccount)
            {
                Storage.StorageManager.Pair<string,int> pair = new Storage.StorageManager.Pair<string,int>();
                pair.First = extAcc.username;
                pair.Second = extAcc.serviceID;
                tempcontacts.Add(pair);
            }
            List<Storage.Contact> mycontacts = db.getContactsByExtID(tempcontacts);
            foreach(Storage.Contact mycontact in mycontacts)
            {
                List<Storage.Publication> pubs = db.getListPublicationsByContactID(mycontact.contactID);
                foreach(Storage.Publication pub in pubs)
                {
                    Storage.CompilationRequest req = db.getCompilationRequestByPulicationAndContact(mycontact.contactID, pub.publicationID);
                    if (!req.compiled)
                    {
                        ret.Add(new ComputableWorkflowReference(_userId, pub.publicationID, pub.namePublication, pub.description,
                            pub.themeID, req.compilReqID, !pub.isPublic, true,pub.expirationDate));
                    }
                }
            }            
            return ret;
        }
        

        /// <summary>
        /// Return a list of 
        /// </summary>
        /// <returns></returns>
        public List<FilledWorkflowReference> GetCompiledForms()
        {
            List<FilledWorkflowReference> ret = new List<FilledWorkflowReference>();

            if (!_registered)
                return ret;
            
            Storage.StorageManager db = new Storage.StorageManager();
            Storage.User user = db.getEntityByID<Storage.User>(_userId);
            foreach (Storage.Publication pub in user.Publication)
                ret.Add(new FilledWorkflowReference(pub.publicationID));
            return ret;
        }

        /// <summary>
        /// Add a new workflow.
        /// </summary>
        /// <param name="wf">the workflow</param>
        /// <returns>a new workflow reference</returns>
        public WorkflowReference AddNewWorkFlow(Workflow wf, string description)
        {
            if (!_registered)
                return null;
            if (wf == null || description == null)
                throw new ArgumentNullException();

            WorkflowReference result;
            Storage.StorageManager sto = new Storage.StorageManager();
            Storage.Model mod = sto.addModel(_userId, defaultthemeid, wf.WorkflowName, wf, description);
            if (mod == null)
                return null;
            result = new WorkflowReference(_userId, mod.modelID, wf.WorkflowName, description, mod.themeID);
            return result;
        }

        #endregion

        #region ContactsMethods

        /// <summary>
        /// Retrieve all contacts from a service.
        /// </summary>
        /// <param name="serviceId">the service id</param>
        /// <returns>a list of contacts</returns>
        public List<Contact> ImportContacts(int serviceId)
        {
            if (!_registered)
                return null;
            Auth.ILoginService serv = null;
            try
            {
                serv = _loggedServices[serviceId];
            }
            catch (Exception) { return null; }
            if (serv == null) return new List<Contact>();
            return serv.getContactList();
        }


        /// <summary>
        /// Returns all of the contacts indexed by DB logic key (email, serviceId).
        /// </summary>
        /// <returns>User's contacts.</returns>
        public Dictionary<Storage.StorageManager.Pair<string, int>, Contact> GetContacts()
        {
            if (!_registered)
                return null;
            Storage.StorageManager sto = new Storage.StorageManager();
            List<Storage.Contact> contacts = sto.getContactsByUserID(_userId);
            Dictionary<Storage.StorageManager.Pair<string, int>, Contact> ret = new Dictionary<Storage.StorageManager.Pair<string, int>, Contact>();
            if (contacts == null) return null;
            foreach (Storage.Contact contact in contacts)
            {
                Storage.StorageManager.Pair<string, int> temPair = new Storage.StorageManager.Pair<string, int>();
                Contact tempContact = new Contact(contact.nameContact, contact.externalUserID, new Service(contact.Service.nameService, contact.Service.serviceID));
                temPair.First = contact.externalUserID;
                temPair.Second = contact.externalServiceID;
                //Controllo per evitare duplicati nel dictionary
                if(!ret.ContainsKey(temPair))
                    ret.Add(temPair, tempContact);
            }
            return ret;
        }

        /// <summary>
        /// Resurns a List containing the contacts which the group is composed of.
        /// </summary>
        /// <param name="groupName">Tha name of a contacts group</param>
        /// <returns>A List containing the contacts wich the group is composed of</returns>
        public List<Contact> GetContactsByGroup(string groupName)
        {
            if (!_registered)
                return null;
            if (String.IsNullOrEmpty(groupName))
                return null;
            Storage.StorageManager sto = new Storage.StorageManager();
            List<Storage.Contact> contacts = sto.getContactsByGroup(_userId, groupName);
            List<Contact> ret = new List<Contact>();
            if (contacts == null) return null;
            foreach (Storage.Contact contact in contacts)
                ret.Add(new Contact(contact.contactID, contact.nameContact, contact.externalUserID, new Service(contact.Service.nameService, contact.Service.serviceID)));
            return ret;
        }


        /// <summary>
        /// Convert a list of Security.Contact in a list of StorageManager.Contact
        /// </summary>
        /// <param name="contactsList"></param>
        /// <returns></returns>
        private List<Storage.Contact> ConvertToStorageContact(List<Security.Contact> contactsList)
        {
            if (!_registered)
                return null;
            if (contactsList == null) return null;

            Storage.StorageManager sto = new Storage.StorageManager();
            List<Storage.StorageManager.Pair<string, int>> mycontacts = new List<Storage.StorageManager.Pair<string, int>>();
            foreach (Contact secContact in contactsList)
            {
                Storage.StorageManager.Pair<string, int> pair;
                pair = new Storage.StorageManager.Pair<string, int>();
                pair.First = secContact.Email;
                pair.Second = secContact.Service.ServiceId;
                mycontacts.Add(pair);
            }
            return sto.getContactsByExtID(mycontacts);
        }


        #endregion

        #region GroupsMethods

        /// <summary>
        /// Creates a new group with the name passed as parameter if it does not exist.
        /// </summary>
        /// <param name="name">The name of the new group.</param>
        /// <returns>True if the creation was successful, false otherwise.</returns>
        public bool CreateGroup(string name)
        {
            if (!_registered)
                return false;
            if (String.IsNullOrEmpty(name)) return false;

            if (name == defGroup) return false;
            Storage.StorageManager sto = new Storage.StorageManager();
            List<string> groupNames = new List<string>();
            groupNames.Add(name);
            List<Storage.Group> results = sto.addGroups(groupNames, _userId);
            if (results == null) return false;
            return true;
        }

        /// <summary>
        /// Creates a new group with the name passed to the parameter if the group does not exist
        /// and adds the contacts passed to the parameter.
        /// </summary>
        /// <param name="name">The group name to create.</param>
        /// <param name="contactList"> The list of contacts to be added to the group.</param>
        /// <returns> True if the group was successfully created and contacts have been added, false otherwise.</returns>
        public bool CreateGroup(string name, List<Contact> contactList)
        {
            if (!_registered)
                return false;
            if (String.IsNullOrEmpty(name) || contactList == null)
                return false;

            if (name == defGroup) return false;
            Storage.StorageManager sto = new Storage.StorageManager();
            List<string> groupNames = new List<string>();
            groupNames.Add(name);
            List<Storage.Group> groups = sto.addGroups(groupNames, _userId);
            if (groups == null || groups.Count==0) return false;

            List<Storage.Contact> contacts = ConvertToStorageContact(contactList);
            Storage.Group defStoGroup = null;
            foreach (Storage.Group g in sto.getGroupsByUserID(_userId))
            {
                if (g.nameGroup.Equals(defGroup))
                {
                    defStoGroup = g;
                    break;
                }
            }            
            List<Storage.GroupContact> result2 = sto.addContactsToGroup(contacts, groups[0]);           
            if (result2 == null) return false;

            sto.removeContactsFromGroup(contacts, defStoGroup);

            return true;
        }

        /// <summary>
        /// Return all contact group's names.
        /// </summary>
        /// <returns>A List containing the user's groups names</returns>
        public List<string> GetGroups()
        {
            if (!_registered)
                return null;
            Storage.StorageManager sto = new Storage.StorageManager();
            return sto.getGroupNames(_userId);
        }


        /// <summary>
        /// Removes the group with the name passed as parameter if it does exist.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        /// <returns>True if the removal was successful, false otherwise.</returns>
        public bool RemoveGroup(string name, bool removeContacts)
        {
            if (!_registered)
                return false;
            if (String.IsNullOrEmpty(name)) return false;

            if (name == defGroup) return false;
            Storage.StorageManager sto = new Storage.StorageManager();
            Storage.Group temp = sto.getGroupByUser(_userId, name);
            if (temp == null) return false;

            bool result;
            if (!removeContacts)
            {
                List<Storage.Contact> contacts = sto.getContactsByGroup(_userId, name);
                List<Storage.Contact> orphans = sto.getContactsOrphanCandidates(_userId, name);
                result = sto.removeEntity<Storage.Group>(temp.groupID);
                if (sto.addContactsToGroup(orphans, sto.getGroupByUser(_userId, defGroup)) != null)
                    return result;
                else
                    return false;
            }
            else return sto.removeEntity<Storage.Group>(temp.groupID);
        }


        /// <summary>
        /// Added the contacts passed as parameter to the group that has the name as parameter passed.
        /// QUANDO SI INVOCA STO METODO IL GRUPPO DEVE ESSERE CREATO.
        /// </summary>
        /// <param name="name"> The group name to add contacts</param>
        /// <param name="contactList"> The list of contacts to be added.</param>
        /// <returns> True if the contacts have been added successfully, false otherwise.</returns>
        public bool AddContactsInGroup(string name, List<Contact> contactList, int servID)
        {
            if (!_registered)
                return false;
            if (String.IsNullOrEmpty(name) || contactList == null)
                return false;            

            Storage.StorageManager sto = new Storage.StorageManager();
            Storage.Group tempGroup = sto.getGroupByUser(_userId, name);
            if (tempGroup == null) return false;

            Dictionary<string, string> contactsToAdd = new Dictionary<string, string>();
            foreach (Security.Contact con in contactList)
            {
                if (con == null || contactsToAdd.ContainsKey(con.Email)) continue;
                contactsToAdd.Add(con.Email, con.Name);
            }

            // Insert contacts into the database
            List<Storage.Contact> contactsAdded = sto.addContacts(contactsToAdd, servID);

            List<Storage.Contact> contactsToRemove = new List<Storage.Contact>();
			foreach (Storage.Contact c in sto.getContactsByGroup(_userId, defGroup))
            {
                if (c == null) continue;
                if (contactsAdded.Contains(c))
                    contactsToRemove.Add(c);
            }
			Storage.Group otherGroup = sto.getGroupByUser(_userId, defGroup);
            sto.removeContactsFromGroup(contactsToRemove, otherGroup);

            // Link the contacts to the group
            List<Storage.GroupContact> result = sto.addContactsToGroup(contactsAdded, tempGroup);
            if (result == null) return false;
            return true;
        }

        /// <summary>
        /// Remove contacts passed as parameter by the group with the name passed as parameter.
        /// </summary>
        /// <param name="name"> The name of the group from which to remove contacts.</param>
        /// <param name="contactList"> The list of contacts to be removed.</param>
        /// <returns> True if the contacts have been successfully removed, false otherwise.</returns>
        public bool RemoveContactsFromGroup(string name, List<Contact> contactList)
        {
            if (!_registered)
                return false;
            if (String.IsNullOrEmpty(name) || contactList == null)
                return false;
            
            Storage.StorageManager sto = new Storage.StorageManager();
            Storage.Group tempGroup = sto.getGroupByUser(_userId, name);
            if (tempGroup == null) return false;

            List<Storage.Contact> contacts = ConvertToStorageContact(contactList);
            return sto.removeContactsFromGroup(contacts, tempGroup);
        }

        /// <summary>
        /// Change the name of a group. Old and new name must be both different from the default group name.
        /// </summary>
        /// <param name="oldName"> The old name of the group</param>
        /// <param name="newName"> The new name of the group</param>
        /// <returns> True if the group name was changed successfully, false otherwise.</returns>
        public bool ModifyGroupName(string oldName, string newName)
        {
            if (!_registered)
                return false;
            if (String.IsNullOrEmpty(oldName) || String.IsNullOrEmpty(newName))
                return false;

            if (oldName == defGroup || newName == defGroup)
                return false;

            if (oldName == newName)
                return true;

            Storage.StorageManager sto = new Storage.StorageManager();
            Storage.Group tempGroup = sto.getGroupByUser(_userId, oldName);
            if (tempGroup == null) return false;
            tempGroup.nameGroup = newName;
            return sto.commit();
        }

        /// <summary>
        /// Moved contacts, passed as parameter, from one group to another.
        /// </summary>
        /// <param name="groupIn"> The group name from which the contacts take.</param>
        /// <param name="groupOut"> The name of the group to move contacts.</param>
        /// <param name="contactList"> The list of contacts to be moved.</param>
        /// <returns> True if the contacts are moved successfully, false otherwise. In case of moving to the default group, contacts that are associated in more groups, will not move.</returns>
        public bool MoveContacts(string groupIn, string groupOut, List<Contact> contactList)
        {
            if (!_registered)
                return false;
            if (String.IsNullOrEmpty(groupOut) || String.IsNullOrEmpty(groupIn) || contactList == null)
                return false;

            if (groupIn == groupOut)
                return true;

            Storage.StorageManager sto = new Storage.StorageManager();
            Storage.Group tempGroupIn = sto.getGroupByUser(_userId, groupIn);
            Storage.Group tempGroupOut = sto.getGroupByUser(_userId, groupOut);
            if (tempGroupIn == null || tempGroupOut == null) return false;

            List<Storage.Contact> stoContacts = ConvertToStorageContact(contactList);

            // se il groupOut e' other contacts, devo fare il controllo che gli elementi che vado a spostare
            // non stiano in altri gruppi
            if (groupOut.Equals(defGroup))
            {
                List<Storage.Contact> orphan = sto.getContactsOrphanCandidates(_userId, groupIn);
                stoContacts = orphan.Intersect(stoContacts) as List<Storage.Contact>;
            }
            if (stoContacts == null)
                //Nel caso che non sposto niente, cosa ritorno?
               return true;
            else { 
                List<Storage.GroupContact> results = sto.moveContactsToGroup(stoContacts, tempGroupIn.groupID, tempGroupOut.groupID);
                if (results == null) return false;

            }           
            return true;
        }
        #endregion
    }
}
