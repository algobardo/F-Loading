using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace Storage
{
    public partial class StorageManager
    {
        //public List<PublicationCredit> getCredential(int userID)
        //{
            
        //}

        
        /// <summary>
        /// Get a list of all externalAccount related a user by ID
        /// </summary>
        /// <param name="userID">The userID</param>
        /// <returns>The list of ExternalAccount</returns>
        /// 
        /*
        public List<ExternalAccount> getExternalAccountByUserID(int userID)
        {
            var extAccountList = from e in context.ExternalAccount
                                 join u in context.User
                                 on e.userID equals u.userID
                                 where u.userID == userID
                                 select e;
            return extAccountList.ToList<ExternalAccount>();
        }

        */


        public User getUserByExternalAccount(int extServiceID, String extUserID)
        {
           // var exa = context.ExternalAccount.Where(ea => ea.serviceID == extServiceID && ea.username == extUserID).FirstOrDefault();
           // return exa == null ? null : exa.User;
            return context.ExternalAccount.Where(ea => ea.serviceID == extServiceID && ea.username == extUserID).Join(context.User, oK => oK.userID, iK => iK.userID, (oK, iK) => iK).FirstOrDefault();
        }
        
        public User addUser(String nickname, String mail)
        {
            User u = new User()
            {
                mailLimit = int.Parse(getEnvValue("mailLimitDefault")),
                isAdmin = bool.Parse(getEnvValue("isAdminDefault")),
                nickname = nickname,
                mail = mail
            };
            return AddEntity(u);
        }
    }
}

