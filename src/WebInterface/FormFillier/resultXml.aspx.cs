using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace WebInterface.FormFillier
{
    public partial class resultXml : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                XmlDocument xml = null;
                Response.ClearContent();
                xml = (XmlDocument)Session["resultXml"] == null ? null : (XmlDocument)Session["resultXml"];
                Response.ContentType = "text/xml";
                xml.Save(Response.Output);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
