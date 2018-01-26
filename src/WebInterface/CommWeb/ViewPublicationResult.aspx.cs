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
using Comm.Report;
using System.Xml;
using Storage;
using Comm;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WebInterface.CommWeb
{
    public partial class ViewPublicationResult : System.Web.UI.Page
    {
        protected string InnerText { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                int resultId = Int32.Parse(Request["ResultId"]);
                if (resultId == CommunicationService.StartRssFeedConstant)
                {
                    InnerText = "<h1>Benvenuto</h1>";
                }
                else
                {
                    StorageManager manager = new StorageManager();
                    Result result = manager.getEntityByID<Result>(resultId);
                
             
                    if (result != null)
                    {
                        Publication pub = result.Publication;
                        if(pub == null)
                            pub = result.CompilationRequest.Publication;

                        Uri uri = new Uri(pub.URIUpload);
                        string token = uri.Query.Substring(7);
                        if (pub.publicationID != Int32.Parse(Request["PubId"]) ||
                            !(Request["token"] != null && token.Equals(Request["token"])))
                        {
                            Response.StatusCode = 400;
                            Response.Write("HTTP 400 BAD REQUEST");
                            Response.End();
                            return;
                        }
                        StringBuilder builder = new StringBuilder();
                        System.Xml.Linq.XElement xmlRes = result.xmlResult;
                        builder.Append("<h1>" + XmlConvert.DecodeName(xmlRes.Name.LocalName) + "</h1>");
                        System.Collections.Generic.IEnumerable<XElement> elements = xmlRes.Elements();
                        foreach (System.Xml.Linq.XElement el in elements)
                        {
                            builder.Append("<div>");
                            builder.Append("<h3 class=\"step\">" + XmlConvert.DecodeName(el.Name.LocalName) + "</h3>");
                            System.Collections.Generic.IEnumerable<XElement> childsEnum = el.Elements();
                            foreach (System.Xml.Linq.XElement quest in childsEnum)
                            {
                                builder.Append("<p> <span class=\"question\">" + XmlConvert.DecodeName(quest.Name.LocalName) +
                                        " = </span>  <span class=\"answer\">" + XmlConvert.DecodeName(quest.Value) + "</span> </p>");
                            }
                            builder.Append("</div>");
                        }
                        InnerText = builder.ToString();
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        Response.Write("HTTP 400 BAD REQUEST");
                        Response.End();
                    }
                }   
            }
            catch (Exception ex)
            {
                if (Response.StatusCode != 400)
                {
                    Response.StatusCode = 400;
                    Response.Write("HTTP 400 BAD REQUEST");
                    Response.End();
                }
            }
        }
    }
}

