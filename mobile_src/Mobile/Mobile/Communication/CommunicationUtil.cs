using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Mobile.Communication
{
    public class CommunicationUtil
    {
        private static readonly String workflowIdPattern = "WorkflowID";
        private static readonly String compilationRequestIdPattern = "CompilationRequestID";
        private static readonly String usernamePattern = "Username";
        private static readonly String tokenPattern = "Token";
        private static readonly String servicePattern = "Service";

        /// <summary>
        /// Parse the massage to obtain the information to construct a FormRequestInfo
        /// </summary>
        /// <param name="message">message contain a string that we need to parse</param>
        /// <returns>an FormRequestInfo object that contain the information about the workflow and the form to compile</returns>
        public static FormRequestInfo ParseNotificationParameters(String message)
        {
            int workflowId = 0;
            int compilationRequestId = 0;
            String username;
            String service;
            String token;

            String compilationRequestIdString = ParseParameter(message, compilationRequestIdPattern);
            if (compilationRequestIdString == null)
                return null;

            try
            {
                compilationRequestId = Int32.Parse(compilationRequestIdString);
            }
            catch (Exception exc)
            {
                return null;
            }

            String workflowIdString = ParseParameter(message, workflowIdPattern);
            try
            {
                workflowId = Int32.Parse(workflowIdString);
            }
            catch (Exception exc)
            {
                workflowId = -1;
            }

            username = ParseParameter(message, usernamePattern);
            service = ParseParameter(message, servicePattern);
            token = ParseParameter(message, tokenPattern);

            return new FormRequestInfo() { WorkflowId = workflowId, CompilationRequestId = compilationRequestId, Username = username, Service = service, Token = token };
        }

        /// <summary>
        /// Parse the parameter of the message
        /// </summary>
        /// <param name="message">message contain a string that we need to parse</param>
        /// <param name="parameterName">name of the parametrer that we consider to parse the massage</param>
        /// <returns></returns>
        private static String ParseParameter(String message, String parameterName)
        {
            if (!message.Contains(parameterName + ": "))
                return null;

            int parameterStartIndex = message.IndexOf(parameterName + ": ") + (parameterName + ": ").Length;
            int parameterEndIndex = message.IndexOf("\r\n", parameterStartIndex);

            try
            {
                String temp = message.Substring(parameterStartIndex, parameterEndIndex - parameterStartIndex);
                return temp;
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}
