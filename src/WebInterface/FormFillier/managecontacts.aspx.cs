using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Threading;
using Core.WF;
using System.Collections;
using WebInterface.FormFillier.action.contactmanager;
using WebInterface.WorkflowEditor.action;


namespace WebInterface.FormFillier
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private Security.User user;
        private Dictionary<string, string> services;
      
        #region [PAGE LOADING]

        protected override void OnInit(EventArgs e)
        {
            Security.Token tok = (Security.Token)Session["Token"];

            base.OnInit(e);

            if (!Page.IsPostBack)
            {
                if (tok != null && tok.Authenticated)
                {
                    user = (Security.User)tok.GetCurrentUser();
                    Session["user"] = user;
                    
                    if (user.Registered)
                    {
                        services = new Dictionary<string, string>();
                        foreach (Security.Service service in user.GetSubscribedServices())
                        {
                            services.Add(Convert.ToString(service.ServiceId), service.ServiceName);
                        }
                        Session["services"] = services;
                    }
                }
            }
            else
            {
                user = Session["user"] as Security.User;
                services = Session["services"] as Dictionary<string, string>;

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Control> listControl;

            listControl = new List<Control>();
            String cbReference;
            String callbackScript;

            listControl.Add(new AddContactsInGroup());
            listControl.Add(new UrlRet());
            listControl.Add(new CreateGroup());
            listControl.Add(new GetServices());
            listControl.Add(new ImportContacts());
            listControl.Add(new MoveContacts());
            listControl.Add(new RemoveGroup());
            listControl.Add(new RenameGroup());
            listControl.Add(new RemoveContacts());
            listControl.Add(new GetGroupsAndContacts());

            for (int i = 0; i < listControl.Count; i++)
            {
                call.Controls.Add(listControl[i]);
                cbReference = Page.ClientScript.GetCallbackEventReference(listControl[i], "arg", ((IControlWorkflowEditor)listControl[i]).getNameFunctionServerResponse(), "context");
                callbackScript = "function " + ((IControlWorkflowEditor)listControl[i]).getNameFunctionServerCall() + "(arg,context)" + "{ " + cbReference + ";}";
                Page.ClientScript.RegisterClientScriptBlock(listControl[i].GetType(), ((IControlWorkflowEditor)listControl[i]).getNameFunctionServerCall(), callbackScript, true);
            }
            ClientScript.RegisterStartupScript(ClientScript.GetType(), "startsession", "<script> GetGroupsAndContacts();</script>"); 
        }

        #endregion

    }
}
