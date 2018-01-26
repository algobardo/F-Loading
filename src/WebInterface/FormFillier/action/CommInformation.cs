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
    public class CommInformation : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
    {
        string result;
        public string GetCallbackResult()
        {
            return result;
        }
        public void RaiseCallbackEvent(string eventArgument)
        {
            result = "";

            CommunicationService cs = CommunicationService.Instance;
            List<string> list = cs.GetServices();
           // result = result + cs.GetMaxInputFieldsCount() + "||";
            if (list.Count > 0)
            {
                foreach (string s in list)
                {
                    result = result + s + "|";
                }
            }

        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "getCommListCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "getCommListResult";
        }

        #endregion
    }
}
