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
using System.Reflection;
using Fields;
using System.Collections.Generic;

namespace WebInterface.WorkflowEditor.action
{
    //TODO: probably to be removed
    public class WFF_saveConstraints : System.Web.UI.Control, ICallbackEventHandler, IControlWorkflowEditor
    {
        string returnValue = "";
        #region ICallbackEventHandler Members

        /// <summary>
        /// Returns callback's result
        /// </summary>
        /// <returns></returns>
        public string GetCallbackResult()
        {
            return returnValue;
        }

        /// <summary>
        /// Receives client's call and gets the predicates
        /// </summary>
        /// <param name="eventArgument">The constraints string</param>
        public void RaiseCallbackEvent(string eventArgument)
        {
            string[] property = eventArgument.Split('|');

            //property[0] -> field.basType
            IBaseType f = getField(property[0]);
            if(f!=null)
            {
                returnValue = "";

                //Applying costraints
                if (!property[1].Equals("")) //apply only if some costraints are defined
                {
                    //---------------------------
                    //  COSTRAINTS STRING FORMAT:
                    //  & CostraintName # CostraintParameter1 | .. | CostraintParameterNN
                    //---------------------------
                    
                    string[] constraints = property[1].Split('@');
                    for (int k = 0; k < constraints.Length; k++)
                    {
                        string[] constraintRapresentation = constraints[k].Split('#'); //divide costraint name from parameters
                        
                        string[] tmpParams;
                        if(constraintRapresentation[1].Equals(""))
                            tmpParams = null;
                        else
                            tmpParams = constraintRapresentation[1].Split('$'); //divide each param
                  
                        List<MethodInfo> constraintList = FieldsManager.GetConstraints(f.GetType());
                        bool constraint_loaded = false;
                        foreach (MethodInfo c in constraintList)
                        {
                            if (constraintRapresentation[0].Equals(c.Name))
                            {
                                constraint_loaded = (bool)c.Invoke(f, tmpParams); //apply costraint to field
                                break;
                            }
                        }
                        if (!constraint_loaded)
                        {
                            List<MethodInfo> constr = FieldsManager.GetConstraints(f.GetType());
                            string constrName = "";
                            foreach (MethodInfo mi in constr)
                            {
                                if(mi.Name.Equals(constraintRapresentation[0]))
                                {
                                    object[] attributes = mi.GetCustomAttributes(true);
                                    Fields.ConstraintAttribute attribute = (Fields.ConstraintAttribute)attributes[0];
                                    constrName += attribute.Name;
                                    break;
                                }
                            }

                            returnValue = "Error applying constraint \"" + constrName + "\"";
                            if(tmpParams != null)
                            {
                                returnValue += " with parameter";
                                if (tmpParams.Length > 1)
                                {
                                    returnValue += "s ";
                                    for (int i = 0; i < tmpParams.Length; i++)
                                        returnValue += ((i!=0)?", ":"") + tmpParams[i];
                                }
                                else
                                    returnValue += " " + tmpParams[0];
                            }
                        }
                    }
                }
            }        
        }
        
        #endregion

        #region Utility methods

        /// <summary>
		/// Adds a field in the node
		/// </summary>
		/// <param name="toNodeID">The field-required's type</param>
        private IBaseType getField(string fieldType) 
        {
            foreach (Type t in FieldsManager.FieldTypes) {
                if (t.ToString() == ("Fields."+fieldType)) {
                    return (IBaseType)FieldsManager.GetInstance(t);
                }
            }
            return null;
        }

        #endregion

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return ComunicationResources.saveConstraintsCall;
        }

        public string getNameFunctionServerResponse()
        {
            return ComunicationResources.saveConstraintsResponse;
        }

        #endregion
    }

}
