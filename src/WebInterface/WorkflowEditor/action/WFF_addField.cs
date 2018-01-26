using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Xml.Schema;
using Core.WF;
using Fields;
using System.Xml;
using System.Reflection;

namespace WebInterface.WorkflowEditor.action {
    public class WFF_addField : System.Web.UI.Control, ICallbackEventHandler, IControlWorkflowEditor {
        private string serialized_field_types;
        private XmlSchema fieldTypes;
        private string error = null;

        #region ICallbackEventHandler Members        

        /// <summary>
        /// Returns callback's result
        /// </summary>
        /// <returns></returns>
        public string GetCallbackResult() {
            if (error != null) return "error|" + error;
            serialized_field_types += Utils.WriteSchema(fieldTypes);
            return serialized_field_types;
        }

        /// <summary>
        /// Receives client's call and adds a node in the workflow
        /// </summary>
        /// <param name="eventArgument">WorkflowId  & Node id</param>
        public void RaiseCallbackEvent(string eventArgument) {
            try {
                fieldTypes = new XmlSchema();   //Creating the schema that will contain the field types
                string[] args = eventArgument.Split('&');                
                string nodeID = args[0];
                string nodeName = args[1];

                //Initializing the string to return
                serialized_field_types += args[0] + '&';
                
                // idnodo & nodeName & basetype | id | label | position x | position y | width | height | rendered_label | constraints &  ...
                /**
                 *  EXAMPLE:
                 * workflow_1_node_1&nodeName&StringBox|1|pippo|120|208|100|30|pippo|AddMaxLengthConstraint#30@AddMinLengthConstraint#3@AddRangeLengthConstraint#3$30
                 **/
                
                for (int i = 2; i < args.Length; i++)
                {
                    string[] property = args[i].Split('|');

                    IBaseType f = getField(property[0]);
                    if(f!=null)
                    {
                        f.TypeName = property[0] + property[1];

                        f.Name = XmlConvert.DecodeName(property[2]);

                        //Applying costraints
                        if (!property[8].Equals("")) //apply only if some costraints are defined
                        {
                            //---------------------------
                            //  COSTRAINTS STRING FORMAT:
                            //  & CostraintName # CostraintParameter1 | .. | CostraintParameterNN
                            //---------------------------
                            
                            string[] constraints = property[8].Split('@');
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
                                    throw new Exception("No constraint with name \"" + constraintRapresentation[0] + "\" could be applied.");
                                }
                            }
                        }

                        //for the client
                        fieldTypes.Items.Add(f.TypeSchema);
                    }        
                }
            }
            catch (Exception e) {
                Console.Write(e.Message);
                error = e.Message;
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

        public string getNameFunctionServerCall() {
            return ComunicationResources.addFieldCall;
        }

        public string getNameFunctionServerResponse() {
            return ComunicationResources.addFieldResponse;
        }

        #endregion
    }
}
