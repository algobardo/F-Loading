using System;
using System.Collections.Generic;
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
using Core.WF;
using WebInterface.FormFillier;

namespace WebInterface.FormFillier.action
{
    public class PublicWFRetrieve : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
    {
        string result;
        public string GetCallbackResult()
        {
            return result;
        }
        public void RaiseCallbackEvent(string eventArgument)
        {
            result = "";
            Dictionary<string, Storage.StorageManager.Pair<string, string>> dic = Security.Token.GetPublicFormsLinks();

            foreach (KeyValuePair<string, Storage.StorageManager.Pair<string, string>> w in dic)
            {
                Storage.StorageManager.Pair<string, string> pair = w.Value;
                result += pair.First + "\\|)//" + pair.Second + "\\|//" + w.Key + "\\||//";
            }
            if (result == "")
                result = "Doens't exist any public form";
            
        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "getFormPCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "getFormPResult";
        }

        #endregion
    }
}