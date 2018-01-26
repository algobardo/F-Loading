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

namespace WebInterface.WorkflowEditor.action
{
    public class WFC_getPredicates: System.Web.UI.Control, ICallbackEventHandler, IControlWorkflowEditor {

        //fieldType & precidateName | paramenter_1_Type | paramenter_2_Type & precidateName | paramenter_1_Type | paramenter_2_Type &
        string predicates;  

        #region ICallbackEventHandler Members

        /// <summary>
        /// Returns callback's result
        /// </summary>
        /// <returns></returns>
        public string GetCallbackResult() {
            return predicates;
        }

        /// <summary>
        /// Receives client's call and gets the predicates
        /// </summary>
        /// <param name="eventArgument">The field's type</param>       
        public void RaiseCallbackEvent(string eventArgument)
        {
            IField f = getField(eventArgument);

            predicates += eventArgument;

            if (f == null) return;

            List<MethodInfo> listPredicates = FieldsManager.GetPredicates(f.GetType());

            foreach (MethodInfo m in listPredicates) {
                predicates += '&' + m.Name;

                ParameterInfo[] parameters = m.GetParameters();

                foreach (ParameterInfo p in parameters)
                    predicates += '|' + p.ParameterType.Name;
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
            return ComunicationResources.getPredicatesCall;
        }

        public string getNameFunctionServerResponse() {
            return ComunicationResources.getPredicatesResponse;
        }

        #endregion
    }
    
}

