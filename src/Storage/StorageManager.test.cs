using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Storage
{
    class StorageTester
    {
        const string prompt = "$ ";
        private static StorageManager sm;
        private delegate String[] cmd(String[] tk);
        private static Dictionary<String, cmd> cmds;
        private static char[] sep = new char[] { ' ' };
        private static char[] pip = new char[] { '|' };
        private static char[] cma = new char[] { ',' };
        private static String[] tkn;
        private static String[] tkp;
        private static cmd toRun;

        public static void Main()
        {
            fillCmds();
            banner();
            sm = new StorageManager();
            while (true)
            {
                Console.Write(prompt);
                Console.WriteLine(parse(Console.ReadLine()));
            }
        }

        private static String parse(String str)
        {
            try
            {
                tkp = str.Split(pip);
                tkn = tkp[0].Split(sep, StringSplitOptions.RemoveEmptyEntries);
                if (tkn.Length == 0)
                    return "";
                cmds.TryGetValue(tkn[0], out toRun);
                return toRun == null ? "Command not found! Type 'help' to see available commands." : " " + String.Join("\n ", tkp.Length > 1 ? toRun.Invoke(tkn).Where(r => r.Contains(tkp[1].Substring(0).Trim())).ToArray() : toRun.Invoke(tkn));
            }
            catch (Exception ex)
            {
                return "Exception: " + ex.ToString();
            }
        }

        private static void fillCmds()
        {
            cmds = new Dictionary<string, cmd>();
            foreach (MethodInfo mi in typeof(StorageTester).GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
            {
                {
                    try
                    {
                        cmds.Add(mi.Name, (cmd)cmd.CreateDelegate(typeof(cmd), mi));
                    }
                    catch (Exception) { }
                }
            }

        }

        private static void banner()
        {
            Console.WriteLine(@"___________.__                    .___.__                 ________ __________ ");
            Console.WriteLine(@"\_   _____/|  |   _________     __| _/|__| ____    ____   \______ \\______   \");
            Console.WriteLine(@" |    __)  |  |  /  _ \__  \   / __ | |  |/    \  / ___\   |    |  \|    |  _/");
            Console.WriteLine(@" |     \   |  |_(  <_> ) __ \_/ /_/ | |  |   |  \/ /_/  >  |    `   \    |   \");
            Console.WriteLine(@" \___  /   |____/\____(____  /\____ | |__|___|  /\___  /  /_______  /______  /");
            Console.WriteLine(@"     \/                    \/      \/         \//_____/           \/       \/ ");
            Console.WriteLine("");
            Console.WriteLine("Storage Manager Console\n");
        }

        private static String[] str2arr(String str)
        {
            return new String[] { str };
        }

        #region "Testing methods"

        private static String[] setEnv(String[] tk)
        {
            return tk.Length < 3 ? str2arr("Missing arguments. Usage: setEnv <varName> <value>") : str2arr(StorageManager.setEnvValue(tk[1], tk[2]) ? "Done." : "Error!");
        }

        private static String[] getEnv(String[] tk)
        {
            if (tk.Length < 2)
                return str2arr("Missing arguments. Usage: getEnv <varName>");
            String ret = StorageManager.getEnvValue(tk[1]);
            return str2arr(tk[1] + (ret == null ? " not found!" : " = " + ret));
        }

        private static String[] exit(String[] tk)
        {
            Environment.Exit(0);
            return tk;
        }

        private static String[] help(String[] tk)
        {
            return cmds.Keys.ToArray();
        }

        private static String[] getUserGroups(String[] tk)
        {
            if (tk.Length < 2)
                return str2arr("Missing arguments. Usage: getUserGroups <userID>");
            return sm.getGroupsByUserID(int.Parse(tk[1])).Select(g => g.groupID + "\t" + g.nameGroup).ToArray();
        }

        private static String[] addUserGroups(String[] tk)
        {
            List<Group> gl;
            if (tk.Length < 3)
                return str2arr("Missing arguments. Usage: addUserGroups <userID> <groupName1> [groupName2] .. [groupNameN]");
            gl = sm.addGroups(tk.Skip(2).ToList(), int.Parse(tk[1]));
            return gl == null ? str2arr("Error!") : gl.Select(g => g.groupID + "\t" + g.nameGroup).ToArray();

        }

        private static String[] addContact(String[] tk)
        {
            Contact c;
            if (tk.Length < 4)
                return str2arr("Missing arguments. Usage: addContact <extUserIDorEmail> <extServiceID> <nameContact>");
            c = sm.addContact(tk[1], int.Parse(tk[2]), tk[3]);
            return c == null ? str2arr("Error!") : str2arr("Contact added with ID: " + c.contactID);
        }

        private static String[] getGroupContacts(String[] tk)
        {
            if (tk.Length < 3)
                return str2arr("Missing arguments. Usage: getGroupContacts <userID> <groupName>");
            return sm.getContactsByGroup(int.Parse(tk[1]), tk[2]).Select(c => c.contactID.ToString() + "\t" + c.externalUserID + "\t" + c.nameContact).ToArray();
        }

        private static String[] getExtServices(String[] tk)
        {
            return sm.getServices().Select(s => s.serviceID.ToString() + " " + s.nameService).ToArray();
        }

        private static String[] removeOrphanContacts(String[] tk)
        {
            return str2arr(sm.removeOrphanContacts() ? "Done." : "Error!");
        }

        private static String[] addContacts(String[] tk)
        {
            List<Contact> cl;
            if (tk.Length < 3)
                return str2arr("Missing arguments. Usage: addContacts <extServiceID> <extUserID,nameContact> [extUserID,nameContact] .. [extUserID,nameContact]");
            cl = sm.addContacts(tk.Skip(2).ToDictionary(k => k.Split(cma)[0], v => v.Split(cma)[1]), int.Parse(tk[1]));
            return cl == null ? str2arr("Error!") : cl.Select(c => c.contactID + "\t" + c.nameContact).ToArray();
        }

        // addContacts 4 bill@gates.com,Billy steve@jobs.com,AppleMan

        private static String[] getContacts(String[] tk)
        {
            return sm.getContacts().Select(c => c.contactID + " " + c.externalServiceID + "\t" + c.externalUserID + "\t" + c.nameContact).ToArray();
        }

        private static String[] removeContact(String[] tk)
        {
            if (tk.Length < 2)
                return str2arr("Missing arguments. Usage: removeContact <contactID>");
            return str2arr(sm.removeEntity<Contact>(int.Parse(tk[1])) ? "Done." : "Error!");
        }

        private static String[] addContactToGroup(String[] tk)
        {
            if (tk.Length < 4)
                return str2arr("Missing arguments. Usage: addContactToGroup <contactID> <userID> <groupName>");
            var x = sm.addContactsToGroup(new List<Contact>() { sm.getEntityByID<Contact>(int.Parse(tk[1])) }, sm.getGroupByUser(int.Parse(tk[2]), tk[3]));
            return str2arr(x != null ? "Done." : "Error!");
        }

        private static String[] moveContactToGroup(String[] tk)
        {
            if (tk.Length < 4)
                return str2arr("Missing arguments. Usage: moveContactToGroup <contactID> <sGroupID> <dGroupID>");
            var x = sm.moveContactsToGroup(new List<Contact>() { sm.getEntityByID<Contact>(int.Parse(tk[1])) }, int.Parse(tk[2]), int.Parse(tk[3]));
            return str2arr(x != null ? "Done." : "Error!");
        }

        private static String[] getContactsTest(String[] tk)
        {
            List<Contact> test = sm.getContacts();
            List<String> temp = new List<String>();
            foreach (Contact c in test)
            {
                temp.Add(c.contactID + "\t\t" + c.externalUserID + "\t\t\t\t\t" +c.nameContact);

            }
            Console.WriteLine("contactID\texternalUserID\t\t\t\tnameContact\n");
            return temp.ToArray();
        }
        
        //use for testing userId 3
        /*
        private static String[] getModelsByUserIDTest(String[] tk)
        {
            try
            {
                Dictionary<int, String> tempDictionary = sm.getModelsByUserID(int.Parse(tk[1]));
                List<String> temp = new List<String>();

                foreach (KeyValuePair<int, String> c in tempDictionary)
                {
                    temp.Add(c.Key.ToString() + "\t" + c.Value);
                }
                return temp.ToArray();
            }
            catch
            {
                return str2arr("Missing argument.Usage: getModelsByUserIDTest <UserID>");
            }

        }
         */

        private static String[] getExternalAccountByUserIDTest(String[] tk)
        {
            try
            {
                List<ExternalAccount> testList = sm.getExternalAccountByUserID(int.Parse(tk[1]));
                List<String> accountList = new List<String>();

                foreach (ExternalAccount c in testList)
                {
                    accountList.Add(c.externalAccountID + "\t\t\t " + c.userID + "\t\t" + c.username);
                }
                Console.WriteLine("ExternalAccountID\tUserID\t\tUsername\n");
                return accountList.ToArray();
            }

            catch
            {
                return str2arr("Missing argument.Usage: getExternalAccountByUserIDTest <UserID>");
            }

        }

        private static String[] getContactsByUserIDTest(String[] tk)
        {
            try
            {
                List<Contact> contactList = sm.getContactsByUserID(int.Parse(tk[1]));
                Console.WriteLine("contactID\texternalUserID\t\t\t\tnameContact\n");
                return contactList.Select(c => c.contactID + "\t\t" + c.externalUserID + "\t\t\t\t" + c.nameContact).ToArray();
            }
            catch
            {
                return str2arr("Missing/Invalid argument.Usage: getContactsByUserIDTest <User ID>");
            }
        }

        private static String[] getContactsByExtIDTest(String[] tk)
        {
            FloadingDataContext context = new FloadingDataContext();
            var tmpList = from z in context.Contact
                          select z;
            List<StorageManager.Pair<String, int>> UserAndServiceID = new List<StorageManager.Pair<String, int>>();

            foreach (Contact c in tmpList)
            {
                UserAndServiceID.Add(new StorageManager.Pair<String, int> { First = c.externalUserID, Second = c.externalServiceID });
            }

            List<Contact> outTest = sm.getContactsByExtID(UserAndServiceID);
            Console.WriteLine("contactID externalServiceID\texternalUserID\t\t\t\tnameContact\n");
            return outTest.Select(c => c.contactID + "\t\t" + c.externalServiceID + "\t\t" + c.externalUserID + "\t\t\t\t\t" + c.nameContact).ToArray();
        }

        private static String[] getServicesByUserIDTest(String[] tk)
        {
            try
            {
                List<Service> serviceList=sm.getServicesByUserID(int.Parse(tk[1]));
                Console.WriteLine("serviceID\t\tnameService\n");
                return serviceList.Select(c => c.serviceID + "\t\t\t" + c.nameService).ToArray();
            }
            catch
            {

                return str2arr("Missing/Invalid argument.Usage: getServicesByUserIDTest <UserID>");
            }
        }

        private static String[] getListPublicationsByContactIDTest(String[] tk)
        {
            try
            {
                List<Publication> temp=sm.getListPublicationsByContactID(int.Parse(tk[1]));
                Console.WriteLine("publicationID\t\texpirationDate\t\t\tnamePublication\n");
                return temp.Select(z=>z.publicationID + "\t\t\t"+ z.expirationDate +"\t\t" +z.namePublication).ToArray();
            }
            catch
            {
                return str2arr("Missing/Invalid argument.Usage: getListPublicationsByContactIDTest <contactID>");
            }
        }

        private static String[] getResultsByPublicationIDTest(String[] tk)
        {
            try
            {
                List<Result> resultList = sm.getResultsByPublicationID(int.Parse(tk[1]));
                Console.WriteLine();
                return resultList.Select(x => x.xmlResult + "\t\t\t" + x.publicationID + "\t\t\t" + x.Publication).ToArray();
            }
            catch
            {
                return str2arr("Missing/Invalid argument.Usage: getResultsByPublicationIDTest <publicationID>");
            }

        }
        
        private static String[] getContactsByUserServiceTest(String[] tk)
        {
            try
            {
                Contact resultContact = sm.getContactByUserService(tk[1], int.Parse(tk[2]));
                Console.WriteLine("ContactID\t\tnameContact\t\t\t\t\t\tGroupContact\n");
                return str2arr(resultContact.contactID +"\t\t\t" + resultContact.nameContact + "\t\t\t\t\t" + resultContact.GroupContact);
            }
            catch
            {
                return str2arr("Missing/Invalid argument.Usage:getContactByUserServiceTest <externalUserID> <serviceID>");
            }
        }

        #endregion
    }
}
