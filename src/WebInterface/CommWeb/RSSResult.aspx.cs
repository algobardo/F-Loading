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
using Comm.Services;
using Storage;
using Comm;

namespace WebInterface.CommWeb
{
    public partial class RSSResult : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Response.Clear();
            StorageManager manager = new StorageManager();
            try
            {
                Publication pub = manager.getEntityByID<Publication>(Int32.Parse(Request["PubId"]));
                if (pub == null)
                {
                    Response.StatusCode = 400;
                    Response.Write("HTTP 400 Bad Request");
                }
                else
                {
                    Uri uri = new Uri(pub.URIUpload);
                    string token = uri.Query.Substring(7);
                    if (Request["token"] != null && token.Equals(Request["token"]))
                    {
                        CommunicationService service = CommunicationService.Instance;
                        Response.ContentType = "text/xml";
                        service.GenerateFeed(pub, Response.OutputStream);
                    }
                }
            }
            catch (Exception ex)
            {
                if (Response.StatusCode != 400)
                {
                    Response.StatusCode = 400;
                    Response.Write("HTTP 400 Bad Request");
                }
            }
            Response.End();
        }
    }
}
