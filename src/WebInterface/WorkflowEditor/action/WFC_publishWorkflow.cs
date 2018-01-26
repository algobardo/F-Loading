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

namespace WebInterface.WorkflowEditor.action
{

    public class WFC_publishWorkflow : System.Web.UI.Control, ICallbackEventHandler, IControlWorkflowEditor
    {

        private string callbackResult;

        #region ICallbackEventHandler Members

        /// <summary>
        /// Returns callback's result
        /// </summary>
        /// <returns></returns>
        public string GetCallbackResult()
        {
            return callbackResult;
        }

        /// <summary>
        /// Receives client's call and saves the Workflow
        /// </summary>
        /// <param name="eventArgument">WorkflowId from client</param>
        public void RaiseCallbackEvent(string eventArgument)
        {
            //Publishing WF
            try
            {
                string[] args = eventArgument.Split('|');
                string wfID = args[0];
                string type = args[1];
                string description = args[2];
                string expiration_date = args[3];
                string checkOption = args[4];
                string serviceList = args[5];
                string usingStaticField = args[6];

                //Publishing WF
                Token t = (Token)Page.Session["Token"];
                if (t != null && t.Authenticated && t.GetCurrentUser().Registered)
                {
                    string uri = (string)Page.Session["uriFromComm"];
                    Page.Session["uriFromComm"] = "";

                    WorkflowReference wfRef = (WorkflowReference)Page.Session["WFE_CurrentWorkflowReference"];

                    ComputableWorkflowReference computebleWfRef = null;
                    if (type.Equals("public"))
                    {
                        if (checkOption.Equals("false"))
                        {

                            //computebleWfRef = wfRef.MakeComputable(FormType.PUBLIC_WITH_REPLICATION, description, expiration_date);
                            computebleWfRef = wfRef.MakeComputable(FormType.PUBLIC_WITH_REPLICATION, 1, description, expiration_date, uri);
                        }
                        else
                        {
                            if (serviceList.Equals("allservices"))
                            {
                                computebleWfRef = wfRef.MakeComputable(FormType.PUBLIC_WITHOUT_REPLICATION, 1, description, expiration_date, uri);
                            }
                            else
                            {
                                computebleWfRef = wfRef.MakeComputable(FormType.PUBLIC_BY_SERVICE, int.Parse(serviceList), description, expiration_date, uri);
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
                        callbackResult = "Database Error: An error occurred while publishing!";
                    }
                    else {
                        //For Theme-Editor
                        Page.Session["WFE_CurrentWorkflow"] = computebleWfRef;
                        Page.Session["themeReturn"] = "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/WorkflowEditor/WorkflowEditor.aspx";
                        if (usingStaticField.Equals("true")) Page.Session["UsingStaticFields"] = true;
                        else Page.Session["UsingStaticFields"] = false;

                        callbackResult = "OK|" + type + "|Publish OK!|" + wfID;
                        Page.Session["uriFromComm"] = "";
                    }             

                }
                else
                    callbackResult = "You are not logged in!";
            }
            catch (Exception e)
            {
                callbackResult = e.Message;
            }
        }

        #endregion

        #region Utility Methods

        private void SaveFieldsListOnPageSession(Workflow wf)
        {

        }

        #endregion

        #region IControlWorkflowEditor Members

        public string getNameFunctionServerCall()
        {
            return ComunicationResources.publishWorkflowCall;
        }

        public string getNameFunctionServerResponse()
        {
            return ComunicationResources.publishWorkflowResponse;
        }

        #endregion
    }
}

