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
using Fields;
using System.Reflection;
using System.Collections.Generic;

namespace WebInterface.WorkflowEditor.action {
    public class WFC_getOperations : System.Web.UI.Control, ICallbackEventHandler, IControlWorkflowEditor {

        //fieldType & operationName | return_type | paramenter_1_Type | paramenter_2_Type & operationName | return_type | paramenter_1_Type | paramenter_2_Type &
        string operations;

        #region ICallbackEventHandler Members

        /// <summary>
        /// Returns callback's result
        /// </summary>
        /// <returns></returns>
        public string GetCallbackResult() {
            return operations;
        }

        /// <summary>
        /// Receives client's call and gets the predicates
        /// </summary>
        /// <param name="eventArgument">The field's type</param>
        public void RaiseCallbackEvent(string eventArgument) {
            IField f = getField(eventArgument);

            operations += eventArgument;

            if (f == null) return;            

            List<MethodInfo> listOperations = FieldsManager.GetOperations(f.GetType());

            foreach (MethodInfo m in listOperations) {
                operations += '&' + m.Name + '|' + m.ReturnParameter.ParameterType.Name;

                ParameterInfo[] parameters = m.GetParameters();

                foreach (ParameterInfo p in parameters)
                    operations += '|' + p.ParameterType.Name;
            }
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Gets a field
        /// </summary>
        /// <param name="toNodeID">The field-required's type</param>
        private IField getField(string fieldType) {
            foreach (Type t in FieldsManager.FieldTypes) {
                if (t.ToString() == ("Fields." + fieldType)) {
                    return FieldsManager.GetInstance(t);
                }
            }
            return null;
        }


        #endregion

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall() {
            return ComunicationResources.getOperationsCall;
        }

        public string getNameFunctionServerResponse() {
            return ComunicationResources.getOperationsResponse;
        }

        #endregion
    }

}

