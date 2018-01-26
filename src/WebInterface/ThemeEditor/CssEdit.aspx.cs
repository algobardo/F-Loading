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
using WebInterface.ThemeEditor;
using System.Collections.Generic;
using Security;
using System.Xml;

namespace WebInterface
{
    public partial class cssEditPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                try
                {
                    string css = cssArea.Value;
                    bool containsHTML = false;
                    try
                    {
                        //Validate that css does not contain any html
                        XElement x = XElement.Parse("<wrapper>" + css + "</wrapper>");
                        containsHTML = !(x.DescendantNodes().Count() == 1 && x.DescendantNodes().First().NodeType == XmlNodeType.Text);
                    }
                    catch (XmlException)
                    {
                        containsHTML = true;
                    }

                    if (containsHTML)
                        serverMsg.InnerHtml = "Error: Invalid css data";
                    else
                        ((Theme)Page.Session["CurrentTheme"]).CSS = cssArea.Value;
                }
                catch (Exception)
                {
                    serverMsg.InnerHtml = "ERROR: Unable to access the requested resource";
                }
            }
            else
            {
                try
                {
                    cssArea.Value = ((Theme)Page.Session["CurrentTheme"]).CSS;
                }
                catch (Exception)
                {
                    serverMsg.InnerHtml = "ERROR: Unable to access the requested resource";
                }
            }
        }
    }
}
