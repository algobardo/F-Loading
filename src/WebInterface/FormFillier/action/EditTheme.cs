using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using Core.WF;
using Core;
using System.Xml;

namespace WebInterface.FormFillier.action
{
    public class EditTheme : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
    {

        string result;

        public string GetCallbackResult()
        {
            return result;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            Security.Token tok = (Security.Token)Page.Session["Token"];
            if (tok != null && tok.Authenticated && tok.GetCurrentUser().Registered)
            {
                List<Security.ComputableWorkflowReference> computableWf = Page.Session["ComputableWorkflowReferenceList"] as List<Security.ComputableWorkflowReference>;                                

                Security.ComputableWorkflowReference reference = computableWf[Int32.Parse(eventArgument)];
                if (reference != null)
                {
                    XmlDocument xmlDoc = reference.GetWorkflow().GetXMLDocument(); 
                    if (xmlDoc.GetElementsByTagName("TEXT").Count > 0 || xmlDoc.GetElementsByTagName("IMAGE").Count > 0 || xmlDoc.GetElementsByTagName("HTMLCODE").Count > 0)
                        Page.Session["UsingStaticFields"] = true;
                    else Page.Session["UsingStaticFields"] = false;

                    Page.Session["WFE_CurrentWorkflow"] = reference;
                    Page.Session["themeReturn"] = "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/FormFillier/index.aspx";
                    result = "OK";
                }
                else
                {
                    result = "Not found";
                }
            }
            else
            {
                result = "User not logged in";
            }
        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "openThemeEditorCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "openThemeEditorResult";
        }

        #endregion
    }
}