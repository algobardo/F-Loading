using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using WebInterface.ThemeEditor;

namespace WebInterface
{
    public partial class _CssUpload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            cssFile.Accept = "text/css";
            updateCss();
        }

        override protected void OnInit(EventArgs e)
        {
            // 
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            // 
            InitializeComponent();
            base.OnInit(e);
        }

        private void updateCss()
        {
            List<String> csss = customCss.getCSSList(0);
            int ccount = cssFiles.Controls.Count;
            for(int i =0 ; i <ccount; i++)
            {
                cssFiles.Controls.RemoveAt(0);
            }
            if (csss != null)
            {
                foreach (String css in csss)
                {
                    HtmlAnchor cssEditLink = new HtmlAnchor();
                    cssEditLink.HRef = "cssEdit.aspx";
                    cssEditLink.InnerText = css;
                    cssFiles.Controls.Add(cssEditLink);
                    cssFiles.Controls.Add(new System.Web.UI.LiteralControl("<br />"));
                }
            }
        }

        private void InitializeComponent()
        {
            this.cssSubmit.ServerClick += new System.EventHandler(this.cssSubmit_ServerClick);
            //this.Load += new System.EventHandler(this.Page_Load);
        }

        private void cssSubmit_ServerClick(object sender, System.EventArgs e)
        {
        //    try
        //    {
        //        Security.IThemeEditorToken tok = (Security.IThemeEditorToken) Session["Token"];
        //        customCss.resultIDs resultStatus = customCss.addCSS(0, cssFile.PostedFile, Server, tok);
        //        switch (resultStatus)
        //        {
        //            case customCss.resultIDs.ok:
        //                {
        //                    result.Visible = true;
        //                    result.Style["color"] = "Green";
        //                    result.InnerText = "File " + cssFile.PostedFile.FileName + " uploaded successfully";
        //                    resultIMG.Src = "ThemeEditor/resources/102.png";
        //                    resultIMG.Visible = true;
        //                    updateCss();
        //                    break;
        //                }
        //            case customCss.resultIDs.internalError:
        //                {
        //                    result.Visible = true;
        //                    result.Style["color"] = "Red";
        //                    result.InnerText = "Unable to store css file";
        //                    result.InnerHtml += "<br />" + customCss.getLog()[0];
        //                    resultIMG.Visible = false;
        //                    break;
        //                }
        //            case customCss.resultIDs.noFile:
        //                {
        //                    result.Visible = true;
        //                    result.Style["color"] = "Red";
        //                    result.InnerText = "Please select a file to upload";
        //                    resultIMG.Visible = false;
        //                    break;
        //                }
        //            case customCss.resultIDs.wrongFile:
        //                {
        //                    result.Visible = true;
        //                    result.Style["color"] = "Red";
        //                    result.InnerText = "Errors detected in the css file";
        //                    resultIMG.Visible = false;
        //                    break;
        //                }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write(ex.ToString());
        //    }

        }

    }
}
