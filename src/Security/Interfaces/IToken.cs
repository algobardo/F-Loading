using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security
{
    // Generic Token Interface, contains common session information
    public interface IToken
    {
        bool Authenticated
        {
            get;
        }

        /// <summary>
        /// Returns the currently logged user
        /// </summary>
        /// <returns> A <see cref="Security.User"/> containing the logged user</returns>
        User GetCurrentUser();

        /// <summary>
        /// Log out the current user
        /// </summary>
        /// <returns></returns>
        bool Logout();
    }
}
