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

namespace WebInterface
{
    public partial class CssUp : System.Web.UI.Page
    {
        private const string SCRIPT_TEMPLATE = "<" + "script " + "type=\"text/javascript\">window.parent.cssUploadDone('{0}',{1});" + "<" + "/script" + ">";

        public static String getCSSDir(HttpServerUtility Server)
        {
            return Server.MapPath("Data") + "\\";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            String script = "";
            if (Request.Files.Count == 0)
            {
                script = string.Format(SCRIPT_TEMPLATE, "Error", "false");
                return;
            }
            try
            {
                HttpFileCollection uploads = Request.Files;
                
                
                uploads[0].SaveAs(getCSSDir(Server) + "themeCss.css");



                IWorkflowThemeReference wf = (IWorkflowThemeReference)Page.Session["WFE_CurrentWorkflow"];
                Security.IToken tok;
                tok = (Security.IToken)Session["Token"];
                if (tok == null)
                {
                    script = string.Format(SCRIPT_TEMPLATE, "No Token in Session", "false");
                }
                else if (wf == null)
                {
                    script = string.Format(SCRIPT_TEMPLATE, "No WFE_CurrentWorkflow in Session", "false");
                }
                else
                {
                    try
                    {
                        System.IO.StreamReader sr = new System.IO.StreamReader(uploads[0].InputStream);
                        String ss = sr.ReadToEnd();


                        if (ss.Length > 0)
                        {
                            wf.SetCSS(ss);
                            //Security.SetThemeResult opResult = tok.SetCSS(new Security.Form(), ss);
                            //Security.SetThemeResult.Result status = opResult.getStatus();
                            script = string.Format(SCRIPT_TEMPLATE, "OK", "true");
                        }
                        else
                        {
                            script = string.Format(SCRIPT_TEMPLATE, "Empty file!", "false");
                        }
                    }
                    catch (Exception ex)
                    {
                        script = string.Format(SCRIPT_TEMPLATE, ex.GetType().ToString(), "false");
                    }
                }
                
            }
            catch (Exception ex) {
                //String message = ex.Message.Replace('\'', ' ');
                script = string.Format(SCRIPT_TEMPLATE, ex.GetType().ToString(), "false");
            }

            ClientScript.RegisterStartupScript(this.GetType(), "uploadNotify", script);
        }
    }
}
