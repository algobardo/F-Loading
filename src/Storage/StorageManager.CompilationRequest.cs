using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storage
{
    public partial class StorageManager
    {
        /// <summary>
        /// Add a contact to publication
        /// </summary>
        /// <param name="publicationID">ID of publication</param>
        /// <param name="contactID">ID of contact</param>
        /// <param name="token">token</param>
        public CompilationRequest addContactToPublication(int publicationID, int contactID, string token) 
        {
            CompilationRequest cr = new CompilationRequest()
            {
                contactID = contactID,
                publicationID = publicationID,
                token = token
            };

            return AddEntity(cr);
        }

        /// <summary>
        /// Add a public publication
        /// </summary>
        /// <param name="publicationID">ID of publication</param>
        public CompilationRequest addPublicPublication(int publicationID)
        {
            CompilationRequest cr = new CompilationRequest()
            {
                //public contact
                contactID = 1,
                publicationID = publicationID,
                token = null
            };

            return AddEntity(cr);
        }

        /// <summary>
        /// Return the CompilationRequest 
        /// </summary>
        /// <param name="contactID">Id of contact</param>
        /// <param name="publicationID">ID of publication</param>
        /// <returns>Compilation Request if exist, null otherwise</returns>
        public CompilationRequest getCompilationRequestByPulicationAndContact(int contactID, int publicationID)
        {
            return context.CompilationRequest.FirstOrDefault(cr => cr.contactID == contactID && cr.publicationID == publicationID);
        }
    }
}
