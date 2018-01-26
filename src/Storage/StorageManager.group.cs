using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storage
{
    public partial class StorageManager
    {
        /// <summary>
        /// Add a list of group for a user
        /// </summary>
        /// <param name="groups">List of groups</param>
        /// <param name="userID">ID of user</param>
        /// <returns>List of groups if all is correct, null otherwise</returns>
        public List<Group> addGroups(List<String> groups, int userID)
        {
            List<Group> alreadyIn = null;
            List<Group> toInsert;
            try
            {
                alreadyIn = context.Group.Where(g => g.userID == userID).ToList().Join(groups, oK => oK.nameGroup, iK => iK, (oK, ik) => oK).ToList(); //nameGroup is case-sensitive!
                if (alreadyIn.Count == groups.Count)
                    return alreadyIn;
                groups = groups.Except(alreadyIn.Select(g => g.nameGroup)).ToList();
            }
            catch (Exception) { }
            toInsert = groups.Select(g => new Group() { nameGroup = g, userID = userID }).ToList();
            return AddEntityAndUnion(toInsert, alreadyIn);
        }

        /// <summary>
        /// Get the groups of an user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<string> getGroupNames(int userId)
        {
            var gnames = from g in context.Group
                         where g.userID == userId
                         select g.nameGroup;
            return gnames.ToList<string>();
        }

        public List<Group> getGroupsByUserID(int userID)
        {
            return context.Group.Where(g => g.userID == userID).ToList();
        }

        /// <summary>
        /// Get group by user and name group
        /// </summary>
        /// <param name="userID">ID of user</param>
        /// <param name="nameGroup">name of group</param>
        /// <returns>Group object if the group exist, null otherwise</returns>
        public Group getGroupByUser(int userID, string nameGroup)
        {
            return context.Group.FirstOrDefault(g => g.userID == userID && g.nameGroup == nameGroup);
        }
    }
}
