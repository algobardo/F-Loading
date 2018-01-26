using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace MobileListener
{

    public class CommunicationUtil
    {
        private static readonly String workflowIdPattern = "WorkflowID";
        private static readonly String compilationRequestIdPattern = "CompilationRequestID";
        private static readonly String usernamePattern = "Username";
        private static readonly String tokenPattern = "Token";
        private static readonly String servicePattern = "Service";

        public static FormRequestInfo ParseUrlParameters(String url)
        {
            int workflowId = 0;
            int compilationRequestId = 0;
            String username = null;
            String service = null;
            String token = null;

            Uri uri = new Uri(url);
            String query = uri.PathAndQuery;

            //Remove the ? character from the query
            query = query.Substring(1);

            String[] parameters = query.Split('&');
            foreach (String par in parameters)
            {
                String[] pair = par.Split('=');
                if (pair[0] == compilationRequestIdPattern)
                {
                    try
                    {
                        compilationRequestId = Int32.Parse(pair[1]);
                    }
                    catch (Exception exc)
                    {
                        return null;
                    }
                }
                if (pair[0] == workflowIdPattern)
                {
                    try
                    {
                        workflowId = Int32.Parse(pair[1]);
                    }
                    catch (Exception exc)
                    {
                        workflowId = -1;
                    }
                }
                if (pair[0] == usernamePattern)
                    username = pair[1];
                if (pair[0] == tokenPattern)
                    token = pair[1];
                if (pair[0] == servicePattern)
                    service = pair[1];
            }
            return new FormRequestInfo() { WorkflowId = workflowId, CompilationRequestId = compilationRequestId, Username = username, Service = service, Token = token };    
        }

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

        private static String ParseParameter(String message, String parameterName)
        {
            if (!message.Contains(parameterName + ": "))
                return null;

            int parameterStartIndex = message.IndexOf(parameterName + ": ") + (parameterName +  ": ").Length;
            int parameterEndIndex = message.IndexOf("\r\n", parameterStartIndex);
            if (parameterEndIndex < 0)
                parameterEndIndex = message.Length;

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

        public static bool ConnectTo(String host, int receivePort, int sendPort, bool firstReceive)
        {
            if (firstReceive)
                return WaitIncomingConnection(host, receivePort) & EstabilishOutcomingConnection(host, sendPort);
            else
                return EstabilishOutcomingConnection(host, sendPort) & WaitIncomingConnection(host, receivePort);
        }

        public static bool WaitIncomingConnection(String host, int port)
        {
            TcpListener tcpListener = new TcpListener(Dns.GetHostEntry(host).AddressList[0], port);
            TcpClient tcpClient = null;
            NetworkStream stream = null;

            try
            {
                //Wait for incoming connection
                tcpClient = tcpListener.AcceptTcpClient();

                byte[] bytes = new byte[1024];
                stream = tcpClient.GetStream();
                stream.Read(bytes, 0, bytes.Length);

                stream.Close();
                tcpClient.Close();

                return true;
            }
            catch (Exception exc)
            {
                if (stream != null)
                    stream.Close();
                if (tcpClient != null)
                    tcpClient.Close();
            }

            return false;
        }

        public static bool EstabilishOutcomingConnection(String host, int port)
        {
            TcpClient client = null;
            NetworkStream stream = null;

            try
            {
                client = new TcpClient(host, port);

                stream = client.GetStream();
                stream.WriteByte(0x1);
                stream.Flush();

                stream.Close();
                client.Close();

                return true;
            }
            catch (ArgumentNullException e)
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
            catch (SocketException e)
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }

            return false;
        }
    }
}
