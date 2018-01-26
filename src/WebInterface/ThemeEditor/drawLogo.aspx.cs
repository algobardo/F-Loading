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
using System.Drawing;
using System.Drawing.Imaging;

namespace WebInterface.ThemeEditor
{
    public partial class drawLogo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)Page.Session["TE_currentLogo"];

            if (bmp != null)
            {

                Response.Expires = 0;
                Response.Cache.SetNoStore();
                Response.AppendHeader("Pragma", "no-cache");

                //Graphics g = Graphics.FromImage(bmp);
                // need to be changed. Need to find a way to retrive the right myme type  
                Response.ContentType = "image/jpeg";
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                

                bmp.Save(Response.OutputStream, ImageFormat.Jpeg);

                Response.End();

            }
            

        }
    }
}
