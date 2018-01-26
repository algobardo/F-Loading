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
using System.Collections.Specialized;
using Security;
using System.Drawing;

namespace WebInterface
{
    public partial class SaveTheme : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //Load Form variables into NameValueCollection variable.
            NameValueCollection coll = Request.Form;

            // Get names of all forms into a string array.
            string title = null;
            string css = null;
            Bitmap logo = null;

            if ( (string) coll.Get("skipTheme") == "false" ) {

                title = coll.Get("generatedTitle");
                css = coll.Get("generatedCss");
                logo = (Bitmap)Page.Session["TE_currentLogo"];
            

                if (title.Trim() == "")
                    title = null;
                if (css.Trim() == "")
                    css = null;

            }
            Response.Expires = -1; //required to keep the page from being cached on the client's browser
            Response.Clear();
            Response.ContentType = "text/plain";


            IWorkflowThemeReference wf = (IWorkflowThemeReference)Page.Session["WFE_CurrentWorkflow"];
            Security.IToken tok = (Security.IToken)Session["Token"];

            // Check if the user is authed
            if (tok == null)
            {
                Page.Session["CurrentTheme"] = (Theme) new Theme(title, logo, css);
                Response.Write("{\"status\":NO_TOKEN}");
            } // Check if wf is null
            else if (wf == null)
            {
                Page.Session["CurrentTheme"] = (Theme)new Theme(title, logo, css);
                Response.Write("{\"status\":NO_WF}");
            }
            else
            {
                try
                {
                    // Trying to save the Theme. title, logo and css are not required. 
                    // If passed null the system will use defaults
                    Security.SetThemeResult opResult = wf.SetTheme(new Theme(title, logo, css));
                    // Retrive the result status
                    SetThemeResult.Result status = opResult.getStatus();

                    Response.Write("{\"status\":\"" + status.ToString() + "\"}");
                    Page.Session["CurrentTheme"] = null;

                }
                catch (Exception ex)
                {
                    Response.Write("{\"status\":ERROR_EXCEPTION, errorContent:" + ex.ToString() + "}");
                }
            }


            Response.End();



        }

    }
}
