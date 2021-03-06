﻿using System;
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

namespace WebInterface.FormFillier.action
{
    public class RemovePublication : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
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
                    if (reference.Remove())
                        result = "OK";
                    else
                        result = "Error";
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
            return "removePublicationCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "removePublicationResult";
        }

        #endregion
    
    }
}
