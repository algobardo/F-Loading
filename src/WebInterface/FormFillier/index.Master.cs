using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Core.WF;
using WebInterface.FormFillier.action;
using WebInterface.WorkflowEditor.action;
namespace Floading
{
    public partial class Master : System.Web.UI.MasterPage
    {
        private List<Control> listControl;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Callback function registration

            listControl = new List<Control>();
            listControl.Add(new WorkflowRetrieve());
            listControl.Add(new ContactsRetrieve());
            listControl.Add(new UserWorkflowSend());
            listControl.Add(new WorkflowSelection());
            listControl.Add(new PublicWFRetrieve());
            listControl.Add(new GroupSelectionToSend());
            listControl.Add(new ResultRetrieve());
            listControl.Add(new ExportResult());
            listControl.Add(new EditModel());
            listControl.Add(new EditTheme());
            listControl.Add(new CommInformation());
            listControl.Add(new ServiceFieldRetrieve());
            listControl.Add(new IdMOdelAuthServiceRetrieve());
            listControl.Add(new PublishWf());
            listControl.Add(new CommInformationPW());
            listControl.Add(new ServiceFieldPWRetrieve());
            listControl.Add(new CreateUriPW());
            listControl.Add(new CreateUri());
            listControl.Add(new RemovePublication());
            listControl.Add(new RemoveModel());
            listControl.Add(new Logout());

            String cbReference;
            String callbackScript;

            List<Control>.Enumerator it = listControl.GetEnumerator();
            while (it.MoveNext())
            {
                call.Controls.Add(it.Current);

                IControlFormFiller t = (IControlFormFiller)it.Current;
                cbReference = Page.ClientScript.GetCallbackEventReference(it.Current, "arg", t.getNameFunctionServerResponse(), "context");
                callbackScript = "function " + t.getNameFunctionServerCall() + "(arg, context)" + "{ " + cbReference + ";}";
                Page.ClientScript.RegisterClientScriptBlock(it.Current.GetType(), t.getNameFunctionServerCall(), callbackScript, true);


            }

            #endregion

            string reg = Request.Params.Get("reg");
            Security.Token tok = (Security.Token)Session["Token"];

            if (reg != null)
            {
                if (tok != null)
                {
                    if (!tok.GetCurrentUser().Registered)
                    {
                        string url = "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + Request.Url.PathAndQuery;//.AbsolutePath;
                       
                        Session["ReturnURL"] = url;
                        Response.Redirect("http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/FormFillier/Registration.aspx");
                    }
                    
                }
                else
                {
                    Response.Redirect("http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/FormFillier/index.aspx");
                }
            }


            if (!IsPostBack)
            {
                List<Security.Service> listTemp = new List<Security.Service>();
                if (tok != null && tok.Authenticated)
                {
                    Security.User user = tok.GetCurrentUser();
                    listTemp = user.LoggedServices;

                }
                List<Security.Service> listaServizi = Security.ExternalService.List();

                foreach (Security.Service s in listaServizi)
                {

                    ServiceList.Items.Add(s.ServiceName);

                }
                foreach (Security.Service s in listTemp)
                {
                    ServiceList.Items.Remove(s.ServiceName);
                }
            }
            
            if (tok != null && tok.Authenticated)
            {

                Security.User user = tok.GetCurrentUser();

                // if (user.Registered)
                //{
                labelUser.Text = "Welcome, " + user.GetNickname();
                if (ServiceList.Items.Count != 0)
                {
                    LoginService.ImageUrl = "~/lib/image/AddLogin.png";
                    LoginService.ToolTip = "Add Login";
                    LoginService.Style.Add("margin-top", "-2px");
                }
                else if (ServiceList.Items.Count == 0)
                {
                    LoginService.Visible = false;
                }
                Logout.Visible = true;
                ManageContacts.Visible = true;
                //ManageFiles.Visible = true;
                //}
                //else
                //{
                // labelUser.Text = "Welcome, " + user.ToString();
                //}
            }
            else
            {
                Logout.Visible = false;
                ManageContacts.Visible = false;
                labelUser.Visible = false;
            }

            //Login error management
            if (Session["LoginError"] != null)
            {
                string message = (string)Session["LoginError"];
                string returnUrl = (string)Session["ReturnUrl"];
                String script = "ShowLoginError(\" " + message + " \", \" " + returnUrl + " \");";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "LoginErrorScript", script, true);
                Session["LoginError"] = null;
            }
        }


        protected void vai_OnClick(object sender, EventArgs e)
        {
            List<Security.Service> services = Security.ExternalService.List();

            foreach (Security.Service s in services)
            {
                string servText = ServiceList.Text;
                if (ServiceList.Text.Equals(s.ServiceName))
                {
                    Session["ServiceID"] = s.ServiceId;
                }
            }
            string url = "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + Request.Url.PathAndQuery;//.AbsolutePath;
            //Session["ReturnURL"] = "http://loa.cli.di.unipi.it:49746/FormFillier/index.aspx";
            Session["ReturnURL"] = url;
            Response.Redirect("http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/Auth/login.aspx");

        }
    }
}
