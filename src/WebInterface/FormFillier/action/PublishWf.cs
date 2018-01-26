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
using System.Collections.Generic;
using Core.WF;
using Security;

namespace WebInterface.FormFillier.action
{
    public class PublishWf : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
    {

        string result = "";
        public string GetCallbackResult()
        {
            return result;
        }
        public void RaiseCallbackEvent(string eventArgument)
        {
            if (eventArgument == null || eventArgument == "") {
                result = "Error on publishing, contact an administrator";
            }
            else
            {
                string[] args = eventArgument.Split('|');
                string type = args[0];
                string description = args[1];
                string expiration_date = args[2];
                string checkOption = args[3];
                string serviceList = args[4];
                string usingStaticField = args[5];

                try
                {
                    //Publishing WF
                    Security.Token tok = (Security.Token)Page.Session["Token"];
                    if (tok != null && tok.Authenticated)
                    {
                        if (tok.GetCurrentUser().Registered)
                        {
                            string uri = (string)Page.Session["uriFromComm"];
                           

                            WorkflowReference wfRef = (WorkflowReference)Page.Session["WFE_CurrentWorkflowReference"];

                            ComputableWorkflowReference computebleWfRef = null;
                            if (type.Equals("public"))
                            {
                                if (checkOption.Equals("false"))
                                {
                                    computebleWfRef = wfRef.MakeComputable(FormType.PUBLIC_WITH_REPLICATION, 1, description, expiration_date, uri);
                                }
                                else
                                {
                                    if (serviceList.Equals("allservices"))
                                    {
                                        computebleWfRef = wfRef.MakeComputable(FormType.PUBLIC_WITHOUT_REPLICATION, 1, description, expiration_date,uri);
                                    }
                                    else
                                    {
                                        computebleWfRef = wfRef.MakeComputable(FormType.PUBLIC_BY_SERVICE, int.Parse(serviceList), description, expiration_date,uri);
                                    }
                                }

                            }
                            else
                            {
                                if (checkOption.Equals("false"))
                                {
                                    computebleWfRef = wfRef.MakeComputable(FormType.PRIVATE_NOT_ANONYM, 1, description, expiration_date, uri);
                                }
                                else
                                {
                                    computebleWfRef = wfRef.MakeComputable(FormType.PRIVATE_ANONYM, 1, description, expiration_date, uri);
                                }
                            }

                            if (computebleWfRef == null) {
                                result = "Database Error: An error occurred while publishing!";
                            }
                            else {
                                //For Theme-Editor
                                Page.Session["WFE_CurrentWorkflow"] = computebleWfRef;
                                if (usingStaticField.Equals("true")) Page.Session["UsingStaticFields"] = true;
                                else Page.Session["UsingStaticFields"] = false;

                                result = "Workflow published";
                                Page.Session["uriFromComm"] = "";
                            }                            
                        }
                    }
                    else
                        result = "You are not logged in!";
                }
                catch (Exception e)
                {
                    result = e.Message;
                }
            }
        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "PublishWfCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "PublishWfResult";
        }

        #endregion
    }
}
