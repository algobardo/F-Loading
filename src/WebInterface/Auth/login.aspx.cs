using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Security;
using Security.Auth;
using System.Collections.Generic;
using Storage;
using System.Reflection;

namespace WebInterface
{
    /// <summary>
    /// This class provides the mechanisms needed to authenticate a user and the rescue of his contacts.
    /// </summary>
    public partial class login : System.Web.UI.Page
    {
        //public static string URLLogin = "http://loa.cli.di.unipi.it:49746/Auth/login.aspx";
        //private string URLHome = "http://loa.cli.di.unipi.it:49746/FormFillier/index.aspx";

        private string URLLogin;// = "http://loa.cli.di.unipi.it:49746/Auth/login.aspx";
        private string URLHome;// = "http://loa.cli.di.unipi.it:49746/FormFillier/index.aspx";
        private string URLReg;// = "http://loa.cli.di.unipi.it:49746/FormFillier/Registration.aspx";
        private string LoginUrl = "";
        private string LoginUserName = "";
        private StorageManager sto = new StorageManager();
        private int servID = -1;
        private ILoginService service = null;
        private int UserID = -1;


        public login()
        {
            URLLogin = "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/Auth/login.aspx";
            URLHome = "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/FormFillier/index.aspx";
            URLReg = "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/FormFillier/Registration.aspx";
        }

        /// <summary>
        /// Method called at each opening of page.
        /// </summary>
        /// <param name="sender"> Object. </param>
        /// <param name="e"> EventArgs. </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            bool LoginContact = false;
            try
            {
                LoginContact = (bool)Session["LoginContact"];
            }
            catch (Exception)
            {
                LoginContact = false;
            }
            if (LoginContact)
                ProcessLoginContact();
            else
                ProcessLoginUser();
        }

        /// <summary>
        /// Autenticate a user
        /// </summary>
        private void ProcessLoginUser()
        {
            string starting = (string)Session["LoginPhase"];
            service = (ILoginService)Session["LoginService"];
            try
            {
                servID = (int)Session["ServiceID"];
            }
            catch (Exception)
            {
                servID = -1;
            }
            
            if (string.IsNullOrEmpty(starting))
            {
                #region first phase

                Storage.Service serv = sto.getEntityByID<Storage.Service>(servID);
                if (serv == null) showError("An error occurred while retrieving the service used",false);
                string servName = serv.nameService;
                if (servName == null) showError("An error occurred while retrieving the service name used", false);

                Assembly assembly = null;
                if (serv.dllPath == null)
                    assembly = typeof(LoginServices.OAuthBase).Assembly;
                else
                    assembly = Assembly.LoadFrom(serv.dllPath);

                foreach (Type type in assembly.GetTypes())
                {

                    if (type.IsClass && type.Name == serv.className)
                    {
                        Object instance = Activator.CreateInstance(type);
                        if (instance == null) service = null;
                        else service = (Security.Auth.ILoginService)instance;
                    }
                }

                if (service == null) showError("An error occurred while creating the service", false);

                LoginUrl = service.StartLogin(Session, Server);
				if (LoginUrl == null) showError("An error occurred during the first phase of the login.<br />Please check that your time settings are correct.", false);

                Session["LoginPhase"] = "Second";
                Session["LoginService"] = service;
                Response.Redirect(LoginUrl);                

                #endregion
            }
            else
            {
                if (service == null) 
                    showError("An error occurred in the service", false);
                
                if (!starting.Equals("Second"))
                    showError("An error occurred during the second phase of login", false);
                
                if (!service.ProcessLogin(Session, Request, Server))
                    showError("An error occurred during the authentication process", false);

                //Process login correctly

                #region second phase

                LoginUserName = service.getUsername();
                if (LoginUserName == null) showError("An error occurred in the username", false);

                Token tok = (Token)Session["Token"];
                Security.User user = null;

                if ((tok == null) || (!tok.Authenticated))
                {                
                    #region first time login

                    Storage.User tempUser = sto.getUserByExternalAccount(servID, LoginUserName);                    

                    if (tempUser != null)
                    {
                        // user already registered
                        UserID = tempUser.userID;
                        user = new Security.User(UserID, tempUser.nickname, tempUser.mail);
                    }
                    else
                    {
                        //user not yet registered
                        UserID = -1;
                        user = new Security.User(UserID, LoginUserName, "");                        
                    }
                   
                    user.AddLoggedService(servID, service);                    

                    tok = new Token(user);
                    Session["Token"] = tok;

                    #endregion
                }
                else
                {
                    #region add service to the current user

                    user = tok.GetCurrentUser();
                    manageMultipleAccount(user);                                        
                    user.AddLoggedService(servID, service);

                    #endregion
                }                                       

                /* clean session */

                Session["LoginPhase"] = null;
                Session["LoginService"] = null;
                Session["ServiceID"] = null;
                Session["LoginContact"] = null;             
                Session["LoginError"] = null;

                string url = (string)Session["ReturnURL"];
                if (url == null) url = URLHome;
                Session["ReturnURL"] = null;

                if (url.Contains("?")) url += "&reg=true";
                else url += "?reg=true";
                Response.Redirect(url);

                #endregion

                #region registration
                  
                //DA CAMBIAREEEEEEEEEEEee
               
                /*if (user.Registered) 
                    //redirect to the url where the login process begin
                    Response.Redirect(url);
                else
                {
                    //redirect to registration page
                    Session["ReturnURL"] = url;
                    Response.Redirect(URLReg);
                }*/
                

                #endregion

            }
        }

        /// <summary>
        /// Authenticate a contact
        /// </summary>
        private void ProcessLoginContact()
        {
            string starting = (string)Session["LoginContactPhase"];
            service = (ILoginService)Session["LoginContactService"];
            int servID;
            try
            {
                servID = (int)Session["LoginContactServiceID"];
            }
            catch (Exception)
            {
                servID = -1;
            }
            if (string.IsNullOrEmpty(starting))
            {
                Storage.Service serv = sto.getEntityByID<Storage.Service>(servID);
                if (serv == null) showError("An error occurred while retrieving the service used",true);
                string servName = serv.nameService;
                if (servName == null) showError("An error occurred while retrieving the service name used", true);

                Assembly assembly = null;
                if (serv.dllPath == null)
                    assembly = typeof(LoginServices.OAuthBase).Assembly;
                else
                    assembly = Assembly.LoadFrom(serv.dllPath);


                foreach (Type type in assembly.GetTypes())
                {

                    if (type.IsClass && type.Name == serv.className)
                    {
                        Object instance = Activator.CreateInstance(type);
                        if (instance == null) service = null;
                        else service = (Security.Auth.ILoginService)instance;
                    }
                }

                if (service == null) showError("An error occurred while creating the service", true);

                LoginUrl = service.StartLogin(Session, Server);
                if (LoginUrl == null) showError("An error occurred during the first phase of the login.<br />Please check that your time settings are correct.", true);

                Session["LoginContactPhase"] = "Second";
                Session["LoginContactService"] = service;
                Response.Redirect(LoginUrl);
            }
            else
            {
                if (service == null) showError("An error occurred in the service", true);
                else if (starting.Equals("Second"))
                {
                    if (service.ProcessLogin(Session, Request, Server))
                    {
                        LoginUserName = service.getUsername();

                        if (LoginUserName == null) showError("An error occurred in the username", true);
                        Session["LoginContactUserID"] = LoginUserName;
                    }
                    else showError("An error occurred during the authentication process", true);

                    string url = (string)Session["LoginContactReturnURL"];
                    if (url == null) showError("An error occurred while retrieving the return URL", true);

                    Session["LoginContactPhase"] = null;
                    Session["LoginContactService"] = null;

                    Session["LoginContactReturnURL"] = null;
                    Session["LoginContact"] = null;
                    /* Ripulisco la sessione da eventuali errori precedenti */
                    Session.Remove("LoginError");

                    if (String.IsNullOrEmpty(url)) Response.Redirect(LoginUrl);
                    else Response.Redirect(url);
                }
                else showError("An error occurred during the second phase of login", true);
            }
        }

        /// <summary>
        /// Checks whether the user is authenticated with a new or used service.
        /// </summary>
        private void manageMultipleAccount(Security.User user)
        {                        
            UserID = user.UserId;
            List<Storage.Service> services = sto.getServicesByUserID(UserID);
            if (services == null) 
                showError("An error occurred during recovery services", false);
            
            bool addExt = true;
            foreach (Storage.Service serv in services)
            {                
                if (serv.serviceID == servID)
                {
                    addExt = false;
                    break;
                }
            }
            if (addExt)
            {
                /*
                 * Controllo che non esista un altro user (diverso da quello corrente) che abbia questo account esterno.
                 * Se esiste devo fondere i due utenti perche' rappresentano la stessa persona.
                 */
                Storage.User userAlredyExist = sto.getUserByExternalAccount(servID, LoginUserName);
                if (userAlredyExist != null)
                    showError("External account already exists. UserId=" + userAlredyExist.userID, false);

                // Operazione da eseguire su db ora solo se gia' registrato
                if (user.Registered)
                {
                    ExternalAccount extAcc = sto.addExternalAccount(UserID, LoginUserName, servID);
                    if (extAcc == null) showError("An error occurred during the addition of new service", false);
                }
            }
        }

        /// <summary>
        /// Shows for a few seconds the message received and then redirect to home page.
        /// </summary>
        /// <param name="message"> The message to display </param>
        private void showError(string message, bool loginContact)
        {
            string url;
            // Common operation
            Session["LoginContact"] = null;
            if (loginContact)
            {
                Session["LoginContactPhase"] = null;
                Session["LoginContactService"] = null;
                Session["LoginContactServiceID"] = null;
                Session["LoginContactUserID"] = null;
                url = (string)Session["LoginContactReturnURL"];
                if (String.IsNullOrEmpty(url))
                    url = URLHome;
            }
            else
            {
                Session["LoginPhase"] = null;
                Session["LoginService"] = null;
                Session["ServiceID"] = null;
                url = (string)Session["ReturnURL"];
                if (String.IsNullOrEmpty(url))
                    url = URLHome;
            }

            Session["LoginError"] = message;                        
            Response.Redirect(url);            
        }
    }
}
