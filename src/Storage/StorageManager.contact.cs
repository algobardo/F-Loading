using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace Storage
{
    public partial class StorageManager
    {
        
        /// <summary>
        /// Get the list of contacts of a user
        /// </summary>
        /// <param name="groupID">The user ID</param>
        /// <returns>The list of contacts</returns>
        /// 
        /*
        public List<Contact> getContactsByUserID(int userID) 
        {
            var groupContact = from g in context.GroupContact
                               join c in context.Contact on g.contactID equals c.contactID
                               join t in context.Group on g.groupID equals t.groupID
                               where t.userID == userID
                               select c; //Sbagliato
            return groupContact.ToList<Contact>(); 
        }
        */
        
        // Ma che minkia ci fa sto metodo qua?
        public List<CompilationRequest> getCompilationRequestById(int contactID, int publicationID)
        {
            var CRList = from p in context.Publication
                         join cr in context.CompilationRequest
                         on p.publicationID equals cr.publicationID
                         where (cr.contactID == contactID) && (p.publicationID == publicationID)
                         select cr;
            return CRList.ToList<CompilationRequest>();
        }

        #region Contacts management

        /// <summary>
        /// Add contact if not exists or get/return contact if exists
        /// </summary>
        /// <param name="extUserID">userID of external account</param>
        /// <param name="extServiceID">ID of external service</param>
        /// <param name="nameContact">contact name</param>
        /// <returns>Contact object</returns>
        public Contact addContact(string extUserID, int extServiceID, string nameContact)
        {
            Contact c = getContactByUserService(extUserID, extServiceID);
            if (c != null)
                return c;
            c = new Contact()
            {
                externalUserID = extUserID,
                externalServiceID = extServiceID,
                nameContact = nameContact
            };
            return AddEntity(c);
        }


        public List<Contact> addContacts(Dictionary<String, String> extUserIDs_nameContacts, int extServiceID)
        {
            List<Contact> alreadyIn = null;
            List<Contact> toInsert;
            try
            {
                alreadyIn = context.Contact.Where(c => c.externalServiceID == extServiceID).ToList().Join(extUserIDs_nameContacts.Keys, c => c.externalUserID, k => k.ToUpper(), (c, k) => c).ToList();
                if (alreadyIn.Count == extUserIDs_nameContacts.Count)
                    return alreadyIn; // All contacts are already in the DB
                var duplicates = extUserIDs_nameContacts.Join(alreadyIn, oK => oK.Key.ToUpper(), iK => iK.externalUserID, (o, i) => o).ToList();  //nameContact is not Unique (DB costr)!!
                extUserIDs_nameContacts = extUserIDs_nameContacts.Except(duplicates).ToDictionary(c => c.Key, c => c.Value);
            }
            catch (Exception) { }
            toInsert = extUserIDs_nameContacts.Select(c => new Contact() { externalUserID = c.Key.ToUpper(), nameContact = c.Value, externalServiceID = extServiceID }).ToList();
            return AddEntityAndUnion(toInsert, alreadyIn);
        }

        /// <summary>
        /// Add a list of contact to a groups
        /// </summary>
        /// <param name="contacts">Contacts list</param>
        /// <param name="group">Group</param>
        /// <returns>GroupContact list</returns>
        public List<GroupContact> addContactsToGroup(List<Contact> contacts, Group group)
        {
            List<GroupContact> alreadyIn = null;
            List<GroupContact> toInsert;
            try
            {
                alreadyIn = context.GroupContact.Where(gc => gc.groupID == group.groupID).ToList().Join(contacts, oK => oK.contactID, iK => iK.contactID, (gc, c) => gc).ToList();
                if (alreadyIn.Count == contacts.Count)
                    return alreadyIn;
                contacts = contacts.Except(alreadyIn.Select(gc => gc.Contact)).ToList();
            }
            catch (Exception) { }
            toInsert = contacts.Select(c => new GroupContact { groupID = group.groupID, contactID = c.contactID }).ToList();
            return AddEntityAndUnion(toInsert, alreadyIn);
        }


        /// <summary>
        /// Get a contact
        /// </summary>
        /// <param name="externalUserID">externalUserID</param>
        /// <param name="serviceID">ID of service</param>
        /// <returns>Object contact if exist, null otherwise</returns>
        public Contact getContactByUserService(string externalUserID, int serviceID)
        {
            return context.Contact.FirstOrDefault(ct => ct.externalUserID == externalUserID && ct.externalServiceID == serviceID);
        }

        public Boolean removeOrphanContacts()
        {
            List<Contact> orphans;
            var cl = context.GroupContact.Join(context.Contact, gc => gc.contactID, c => c.contactID, (gc, c) => c).Distinct();
            orphans = context.Contact.Except(cl).ToList();
            return orphans.Count > 0 ? removeEntity(orphans) : true;
            //TODO: check also in compilation requests
        }

        public List<Contact> getContactsByExtID(List<Pair<String, int>> extUserID_extServiceID)
        {
            return context.Contact.ToList().Join(extUserID_extServiceID, c => c.externalUserID + c.externalServiceID.ToString(), ext => ext.First + ext.Second, (c, ext) => c).ToList();
        }

        /// <summary>
        /// Gets all contacts related to an user.
        /// </summary>
        /// <returns>List of all contacts related to specified user.</returns>
        public List<Contact> getContactsByUserID(int userID)
        {
            return context.Group.Where(g => g.userID == userID).Join(context.GroupContact, g => g.groupID, gc => gc.groupID, (g, gc) => gc).Join(context.Contact, gc => gc.contactID, c => c.contactID, (gc, c) => c).Distinct().ToList(); // Distinct eheh
        }

        /// <summary>
        /// Gets all contacts.
        /// </summary>
        /// <returns>List of all contacts.</returns>
        public List<Contact> getContacts()
        {
            return context.Contact.ToList();
        }

        /// <summary>
        /// Return all contacts that will be orphan after removing the specified group.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupName">The name of the group that you will remove.</param>
        /// <returns>List of contacts</returns>
        public List<Contact> getContactsOrphanCandidates(int userID, string groupName)
        {   
            return context.Group.Where(g => g.userID == userID && g.nameGroup == groupName).Join(context.GroupContact, g => g.groupID, gc => gc.groupID, (g, gc) => gc).Join(context.Group.Where(g => g.userID == userID),gc=> gc.groupID,g=>g.groupID,(gc,g) => gc).Join(context.Contact, gc => gc.contactID, c => c.contactID, (gc, c) => c).GroupBy(c => c).Where(ig => ig.Count() == 1).Select(x => x.Key).ToList();
        }

        public bool isOrphan(int userID, int contactID)
        {
            var group = from gc in context.GroupContact
                        join g in context.Group
                        on gc.groupID equals g.groupID
                        where (gc.contactID == contactID && g.userID == userID)
                        select gc;
            return group.Count() < 2 ? true : false;
        }


        #region removeContactsFromGroup

        public Boolean removeContactsFromGroup(List<Contact> contacts, Group group)
        {
            return removeContactsFromGroup(contacts, group.groupID);
        }

        public Boolean removeContactsFromGroup(List<Contact> contacts, int groupID)
        {
            List<GroupContact> gcl;
            gcl = context.GroupContact.Where(gc => gc.groupID == groupID).ToList().Join(contacts, gc => gc.contactID, c => c.contactID, (gc, c) => gc).ToList();
            return removeEntity(gcl);
        }

        #endregion

        #region getContactsByGroup

        public List<Contact> getContactsByGroup(int userID, String groupName)
        {
            return context.Group.Where(g => g.userID == userID && g.nameGroup == groupName).Join(context.GroupContact, g => g.groupID, gc => gc.groupID, (g, gc) => gc).Join(context.Contact, gc => gc.contactID, c => c.contactID, (gc, c) => c).ToList();
        }

        public List<Contact> getContactsByGroup(int groupID)
        {
            return context.GroupContact.Where(gc => gc.groupID == groupID).Join(context.Contact, gc => gc.contactID, c => c.contactID, (gc, c) => c).ToList();
        }

        public List<Contact> getContactsByGroup(Group group)
        {
            return getContactsByGroup(group.groupID);
        }

        #endregion

        #region moveContactsToGroup
        public List<GroupContact> moveContactsToGroup(List<Contact> contacts, int userID, String sGroupID, String dGroupID)
        {
            return moveContactsToGroup(contacts, getGroupByUser(userID, sGroupID).groupID, getGroupByUser(userID, dGroupID).groupID);
        }

        public List<GroupContact> moveContactsToGroup(List<Contact> contacts, int sGroupID, int dGroupID)
        {
            List<GroupContact> alreadyIn;
            List<GroupContact> toMove;
            alreadyIn = context.GroupContact.Where(gc => gc.groupID == sGroupID).Join(context.GroupContact.Where(gc => gc.groupID == dGroupID), gcO => gcO.contactID, gcI => gcI.contactID, (gcO, gcI) => gcO).ToList().Join(contacts, gc => gc.contactID, c => c.contactID, (gc, c) => gc).ToList();
            if (!removeEntity(alreadyIn))
                return null;
            toMove = context.GroupContact.Where(gc => gc.groupID == sGroupID).ToList().Join(contacts, gc => gc.contactID, c => c.contactID, (gc, c) => gc).ToList();
            toMove.ForEach(gc => gc.groupID = dGroupID);
            return commit() ? toMove : null;
        }
        #endregion
        #endregion
    }

}
