using System;
using System.Web;
using System.Web.SessionState;
using System.Collections.Generic;
using Security;

namespace Security
{
    namespace Auth
    {
        /// <summary>
        /// This interface must be implemented by all classes that represent a service through which you can authenticate.
        /// Provides methods for authentication and to retrieve information related to account.
        /// </summary>
        public interface ILoginService
        {
            /// <summary>
            /// The first step of the login phase.
            /// </summary>
            /// <param name="session">The current session.</param>
            /// <param name="Server">The current server utility.</param>
            /// <returns>A string that represents the URL used for authentication.</returns>
            string StartLogin(HttpSessionState session, HttpServerUtility Server);

            /// <summary>
            /// The second step of the login phase.
            /// </summary>
            /// <param name="session">The current session.</param>
            /// <param name="Request">The current Http Request.</param>
            /// <param name="Server">The current server utility.</param>
            /// <returns>True if the login was successful, false otherwise.</returns>
            bool ProcessLogin(HttpSessionState session, HttpRequest Request, HttpServerUtility Server);

            /// <summary>
            /// Returns the list of contacts of the owner.
            /// </summary>
            /// <returns>The list of contacts of the owner.</returns>
            List<Contact> getContactList();

            /// <summary>
            /// Returns the username of the owner.
            /// </summary>
            /// <returns>The username of the owner.</returns>
            string getUsername();

            /// <summary>
            /// Indicates whether the current session id valid
            /// </summary>
            /// <returns>A bool that indicates whether the current session is valid</returns>
            bool isLoginValid();
        }
    }
}