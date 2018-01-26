using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebInterface.FormFillier
{
    public partial class logoBmp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Theme theme = ((Security.ComputableWorkflowReference)Session["wfr"]).GetTheme();

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            theme.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            Response.ClearContent();
            Response.ContentType = "image/Bmp";
            Response.BinaryWrite(ms.ToArray());

        }
    }
}
