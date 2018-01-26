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
using System.IO;
using Core.WF;
using System.Xml.Schema;
using Fields;
using System.Xml;
using System.Reflection;
using Security;

namespace WebInterface.WorkflowEditor.action {

	public class WFC_getAuthServiceList : System.Web.UI.Control, ICallbackEventHandler, IControlWorkflowEditor {

        private string callbackResult;

		#region ICallbackEventHandler Members
		
		/// <summary>
		/// Returns callback's result
		/// </summary>
		/// <returns></returns>
		public string GetCallbackResult() {
			return callbackResult;
		}

		/// <summary>
		/// Receives client's call and saves the Workflow
		/// </summary>
		/// <param name="eventArgument">WorkflowId from client</param>
		public void RaiseCallbackEvent(string eventArgument) {

            //Services will be returned in this format: ServiceName1@ServiceId1$ServiceName2@ServiceId2 ..
			try {
                //Retrieving the Service list
                List<Security.Service> service_list = Security.ExternalService.List();
                callbackResult = "OK|";
                int count =0;
                foreach(Security.Service s in service_list){
                    callbackResult += s.ServiceName + '@' + s.ServiceId;
                    count++;
                    if(count < service_list.Count){
                        callbackResult += "$";
                    }
                }
			}
			catch (Exception e) {
				callbackResult = "ERROR|"+e.Message;
			}
		}

		#endregion

        #region Utility Methods

        

        #endregion

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall() {
			return ComunicationResources.getAuthServiceListCall;
		}

		public string getNameFunctionServerResponse() {
            return ComunicationResources.getAuthServiceListResponse;
		}

		#endregion
	}
}

