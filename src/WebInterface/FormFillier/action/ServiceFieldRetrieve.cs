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
using Comm;
namespace WebInterface.FormFillier.action
{
    public class ServiceFieldRetrieve : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
    {
        string result;
        public string GetCallbackResult()
        {
            return result;
        }
        public void RaiseCallbackEvent(string eventArgument)
        {
            result = "";
            if (eventArgument != "none")
            {
                CommunicationService cs = CommunicationService.Instance;

                int numField = cs.GetInputFieldsCountForService(eventArgument);

                result = eventArgument + "&" + numField + "||";
                Dictionary<string, string> field = cs.GetInputFieldsForService(eventArgument);

                foreach (KeyValuePair<string, string> k in field)
                {
                    //must be consider the field value, are regular expressions
                    result = result + k.Key + "|";
                }
            }


        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "getServiceFieldCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "getServiceFieldResult";
        }

        #endregion
    }
}
