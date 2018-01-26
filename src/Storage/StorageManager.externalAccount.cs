using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storage
{
    public partial class StorageManager
    {

        /// <summary>
        /// Insert an External Account for a User
        /// </summary>
        /// <param name="userID">ID of User</param>
        /// <param name="username">Registration account username</param>
        /// <param name="serviceID">ID of registration account service</param>
        /// <returns>The external account or null if errors occur</returns>
        public ExternalAccount addExternalAccount(int userID, string username, int serviceID)
        {
            ExternalAccount ea = new ExternalAccount()
            {
                userID = userID,
                username = username,
                serviceID = serviceID
            };
            return AddEntity(ea);
        }


        public List<ExternalAccount> getExternalAccountByUserID(int userID)
        {
            return context.ExternalAccount.Where(e => e.userID == userID).ToList();
        }


        /// <summary>
        /// Get the User of specific External Account
        /// </summary>
        /// <param name="serviceID">ID of service</param>
        /// <param name="username">username</param>
        /// <returns>User object if the User exist, null otherwise</returns>
        /// 
        /*
        public User getUserByExternalAccount(int serviceID, string username)
        {
            ExternalAccount temp = context.ExternalAccount.FirstOrDefault(ea => ea.serviceID == serviceID && ea.username == username);
            if (temp == null) return null;
            return getEntityByID<User>(temp.userID);
        }
        */
    }
}
