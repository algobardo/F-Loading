using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Comm.Util
{

    /// <summary>
    /// A collection of regxp (internal use) for
    /// email address, url, ftp uri and godcs uri (custom uri format)
    /// </summary>
    class Validator
    {
        public static string UsernameRegexp = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*(@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)*";
        public static string PasswordRegexp = ".+"; // password security is delegated to ftp server.
        public static string CredentialsRegexp = UsernameRegexp + ":" + PasswordRegexp;
        public static string HostRegexp = "[a-zA-Z0-9_.@]+";
        public static string IpAddressRegexp = @"\b(?:\d{1,3}\.){3}\d{1,3}\b";
        public static string PortRegexp = "([0-9]{1,5})";
        //public static string EmailRegexp = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
        //public static string EmailRegexp = "^[a-z._-]+\\@[a-z._-]+\\.[a-z]{2,4}$";
        
        
         public static string EmailRegexp =@"^(([^<>()[\]\\.,;:\s@\""]+"

                        + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"

                        + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"

                        + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"

                        + @"[a-zA-Z]{2,}))$";
        
        public static string GdocsRegexp = "(gdocs:)" + CredentialsRegexp;
        public static string FtpRegexp = "((ftp|ftps)://)(" + CredentialsRegexp + ")*@(" + HostRegexp + "|" + IpAddressRegexp + ")" + PortRegexp + "*";
        public static string GenericUriRegexp = "(([a-zA-Z][0-9a-zA-Z+\\-\\.]*:)?/{0,2}[0-9a-zA-Z;/?:@&=+$\\.\\-_!~*'()%]+)?(#[0-9a-zA-Z;/?:@&=+$\\.\\-_!~*'()%]+)?";
        public static string PathRegexp = ".*"; // with this directory regexp user can insert arbitrary string. This is a security problem.. 

        /// <summary>
        /// Test if the passed string have a valid format as email address
        /// </summary>
        /// <param name="email">email string </param>
        /// <returns>boolean</returns>
        public static bool IsEmail(string email)
        {
            return new Regex(EmailRegexp).IsMatch(email);
        }

        /// <summary>
        /// Test if the passed string have a valid format as url address
        /// </summary>
        /// <param name="email">url string </param>
        /// <returns>a boolean</returns>
        public static bool IsUrl(string Url)
        {
            return new Regex(GenericUriRegexp).IsMatch(Url);
        }

        /// <summary>
        /// Test if the passed string have a valid format as godcs address
        /// </summary>
        /// <param name="email">godcs string </param>
        /// <returns>a boolean</returns>
        public static bool IsGdocs(string str)
        {
            return new Regex(GdocsRegexp).IsMatch(str);
        }

        /// <summary>
        /// Test if the passed string have a valid format as ftp address
        /// </summary>
        /// <param name="email">ftp string </param>
        /// <returns>a boolean</returns>
        public static bool IsFtp(string str)
        {
            return new Regex(FtpRegexp).IsMatch(str);
        }

        /// <summary>
        /// Test if the passed string have a valid format as password
        /// </summary>
        /// <param name="email">password string </param>
        /// <returns>a boolean</returns>
        public static bool IsPassword(string str)
        {
            return new Regex(PasswordRegexp).IsMatch(str);
        }
    }
}
