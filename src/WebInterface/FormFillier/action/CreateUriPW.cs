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
    public class CreateUriPW : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
    {
        string result;
        public string GetCallbackResult()
        {
            return result;
        }
        public void RaiseCallbackEvent(string eventArgument)
        {
            try
            {
                result = "";
                if (eventArgument != null && eventArgument != "")
                {

                    CommunicationService cs = CommunicationService.Instance;
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    string[] service = eventArgument.Split('$');
                    string[] temp = service[1].Split('&');
                    foreach (string couple in temp)
                    {
                        if (couple != "")
                            dic.Add(couple.Split('|')[0], couple.Split('|')[1]);
                    }
                    string uri = cs.BuildUri(service[0], dic);
                    Page.Session["uriFromComm"] = uri;
                }
                result = "ok";
            }
            catch (Exception e)
            {
                result = "Error on uri creation, contact an admin";
            }
        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "CreateUriPWCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "CreateUriPWResult";
        }

        #endregion
    }
}
