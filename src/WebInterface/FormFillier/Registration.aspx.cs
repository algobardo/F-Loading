using System;
using System.Collections;
using System.Collections.Generic;
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

namespace WebInterface.FormFillier
{
    public partial class Registration : System.Web.UI.Page
    {
        private static string registrationText = Security.EnvironmentManagement.getEnvValue("registration"); //spaces = <br />;
        private static string termsText = Security.EnvironmentManagement.getEnvValue("terms").Replace("\\n", "\n"); //spaces = \n

        protected override void OnInit(EventArgs e)
        {

            base.OnInit(e);
            descriptionPage.InnerHtml = registrationText;
            textCondition.Text = termsText;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Token tok = (Security.Token)Session["Token"];

            //if (!IsPostBack)
            //{

            if (tok != null)
            {
                Security.User user = tok.GetCurrentUser();
                if (user.Registered)
                {
                    TextBoxNick.Text = user.GetNickname();
                    TextBoxMail.Text = user.GetEmail();
                    TextBoxNick.Enabled = false;
                    TextBoxMail.Enabled = false;
                }
                List<Security.Service> listaServizi = Security.ExternalService.List();

                List<Security.Service> listaServiziUsati = user.LoggedServices;

                if (listaServiziUsati.Count > 0)
                {
                    chkTandCs.Enabled = true;
                    //valTandCs.Enabled = true;
                    foreach (Security.Service s in listaServizi)
                    {
                        DropDownList1.Items.Add(s.ServiceName);
                        foreach (Security.Service su in listaServiziUsati)
                        {
                            if (su.ServiceName == s.ServiceName)
                            {

                                Session["ServiceID"] = s.ServiceId;

                                Label lab = new Label();
                                lab.Text = s.ServiceName;
                                Panel p = new Panel();

                                p.Controls.Add(lab);
                                PanelServReg.Controls.Add(p);
                                PanelServReg.Style.Add("margin-left", "40px");
                                DropDownList1.Items.Remove(s.ServiceName);
                            }
                        }
                    }
                }
                //else
                    if (listaServiziUsati.Count == listaServizi.Count)
                    {
                        DropDownList1.Visible = false;
                        register.Visible = false;
                    }
                    //else
                    //{
                    //    foreach (Security.Service s in listaServizi)
                    //    {
                    //        DropDownList1.Items.Add(s.ServiceName);
                    //    }
                    //}
            }
            else
            {
                List<Security.Service> listaServizi = Security.ExternalService.List();
                foreach (Security.Service s in listaServizi)
                {
                    DropDownList1.Items.Add(s.ServiceName);
                }
            }
            if (!IsPostBack)
            {
                TextBoxNick.Text = ((string)Session["Nick"] != null) ? (string)Session["Nick"] : "";
                TextBoxMail.Text = ((string)Session["Mail"] != null) ? (string)Session["Mail"] : "";
            }
        //}else{}

        }



        protected void registerClick(object sender, EventArgs e)
        {

            List<Security.Service> services = Security.ExternalService.List();

            foreach (Security.Service s in services)
            {
                if (DropDownList1.SelectedValue.Equals(s.ServiceName))
                {
                    Session["ServiceID"] = s.ServiceId;
                }
            }
            string url = "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/FormFillier/Registration.aspx";

            if (string.IsNullOrEmpty((string)Session["OriginalURL"])) Session["OriginalURL"] = Session["ReturnURL"];
            Session["ReturnURL"] = url;
            Session["Nick"] = TextBoxNick.Text;
            Session["Mail"] = TextBoxMail.Text;

            Response.Redirect("http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/Auth/login.aspx");
        }

        protected void okButt_Click(object sender, EventArgs e)
        {
            if (chkTandCs.Checked)
            {
                Security.Token tok = (Security.Token)Session["Token"];
                if (tok != null)
                {
                    Security.User user = tok.GetCurrentUser();
                    string errorReg;
                    Security.User.RegisterMeResult registration = user.RegisterMe(TextBoxNick.Text, TextBoxMail.Text, out errorReg);
                    
                    if (registration == Security.User.RegisterMeResult.OK)
                    {
                        Session["LoginPhase"] = null;
                        Session["LoginService"] = null;
                        string url = "";
                        if (string.IsNullOrEmpty((string)Session["OriginalURL"])) url = (string)Session["ReturnURL"];
                        else url = (string)Session["OriginalURL"];
                        Session["OriginalURL"] = null;
                        if (string.IsNullOrEmpty(url)) url = "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/FormFillier/index.aspx";
                        Response.Redirect(url);
                    }
                    else
                    {
                        //Gestisci no reg
                        Page.Controls.Add(new LiteralControl("<script>alert('"+errorReg+"');</script>"));
                    }
                }
                else
                {
                    //Gestione errore token
                }
            }
            else
            {
                //check non marcata
                Page.Controls.Add(new LiteralControl("<script>alert('Accept term & condition before submitting');</script>"));
            }

        }


    }
}
