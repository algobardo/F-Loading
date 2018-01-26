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
    public class WFF_getConstraints: System.Web.UI.Control, ICallbackEventHandler, IControlWorkflowEditor 
    {
        string constraints = "";
        #region ICallbackEventHandler Members

        /// <summary>
        /// Returns callback's result
        /// </summary>
        /// <returns></returns>
        public string GetCallbackResult() {
            return constraints;
        }

        /// <summary>
        /// Receives client's call and gets the predicates
        /// </summary>
        /// <param name="eventArgument">The field's type</param>
        public void RaiseCallbackEvent(string eventArgument) {
            IField f = getField(eventArgument);

            if (f == null) return;
            List<MethodInfo> constr = FieldsManager.GetConstraints(f.GetType());
            foreach (MethodInfo mi in constr)
            {
                object[] attributes = mi.GetCustomAttributes(true);
                Fields.ConstraintAttribute attribute = (Fields.ConstraintAttribute)attributes[0];

                constraints += attribute.Name +'&' + attribute.Description + '&' + mi.Name;                    
                foreach(ParameterInfo pi in mi.GetParameters())
                    constraints += "#" + pi.Name;
                constraints += "|";
            }

            
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Gets a field
        /// </summary>
        /// <param name="toNodeID">The field-required's type</param>
        private IField getField(string fieldType)
        {
            foreach (Type t in FieldsManager.FieldTypes)
            {
                if (t.ToString() == ("Fields." + fieldType))
                {
                    return FieldsManager.GetInstance(t);
                }
            }
            return null;
        }

        #endregion

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall() {
            return ComunicationResources.getConstraintsCall;
        }

        public string getNameFunctionServerResponse() {
            return ComunicationResources.getConstraintsResponse;
        }

        #endregion
    }

}
