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

namespace WebInterface.WorkflowEditor.action
{
    public class WFC_setProperties: System.Web.UI.WebControls.WebControl, ICallbackEventHandler, IControlWorkflowEditor {
        string result;

        #region ICallbackEventHandler Members

        /// <summary>
        /// Returns callback's result
        /// </summary>
        /// <returns></returns>
        public string GetCallbackResult() {
            return result;
        }

        /// <summary>
        /// Receives client's call and finds the predicate of an edge between 'Node id from' and 'Node id to'
        /// </summary>
        /// <param name="eventArgument">WorkflowId from client & Node id from & Node id to & Xml predicate</param>
        public void RaiseCallbackEvent(string eventArgument)
        {
            try
            {
                /*
                string[] args = eventArgument.Split('&');
                string wfID = args[0];
                string fromNodeID = args[1];
                string toNodeID = args[2];
                result = (string)Page.Session[fromNodeID + toNodeID];
                 */
                result = "setPropertiesOK";
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        #endregion

        #region Utility methods


        #endregion

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall() {
            return ComunicationResources.setPropertiesCall;
        }

        public string getNameFunctionServerResponse() {
            return ComunicationResources.setPropertiesResponse;
        }

        #endregion
    }
    
}
