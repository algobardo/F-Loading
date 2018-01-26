using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Core.WF;
using System.Xml.Schema;

namespace Core
{
    public static class BrowserWithServerSync
    {
        private static void parse(string browserStateString,out XmlDocument rdoc,out XmlSchemaSet xset,out string nodeName)
        {
            XmlDocument browserState = new XmlDocument();
            browserState.LoadXml(browserStateString);

            nodeName = browserState.SelectNodes("*/SCHEMA/NODETYPE")[0].FirstChild.Attributes["name"].Value;
            var extTypes = browserState.SelectNodes("*/SCHEMA/EXTENDEDTYPES")[0].InnerXml;
            var nodeType = browserState.SelectNodes("*/SCHEMA/NODETYPE")[0].InnerXml;

            string extTypesSchema = "<?xml version=\"1.0\" encoding=\"utf-8\"?><xs:schema elementFormDefault=\"qualified\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">" + extTypes + "</xs:schema>";
            string nodeTypeSchema = "<?xml version=\"1.0\" encoding=\"utf-8\"?><xs:schema elementFormDefault=\"qualified\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">" + nodeType + "</xs:schema>";

            xset = Utils.ReadSchemaSet(new string[] { extTypesSchema, nodeTypeSchema });

            rdoc = new XmlDocument();
            rdoc.LoadXml(browserState.SelectNodes("*/RENDERING")[0].OuterXml);

        }

        public static WFnode CreateServerNode(string browserStateString)
        {
            string nodeName;
            XmlSchemaSet xset;
            XmlDocument rdoc;
            parse(browserStateString, out rdoc, out xset, out nodeName);

            var node = new WFnode(nodeName);
            node.ModifyNode(xset, rdoc, nodeName);

            return node;
        }
        public static void SyncServerNode(string browserStateString,WFnode ndToSync)
        {
            string nodeName;
            XmlSchemaSet xset;
            XmlDocument rdoc;
            parse(browserStateString, out rdoc, out xset, out nodeName);

            ndToSync.ModifyNode(xset, rdoc, nodeName);            
        }

        public static string GetXMLDocument(Workflow wf)
        {
            return wf.GetXMLDocument();
        }

        public static string CreateBrowserNode(WFnode nd)
        {
            return null;
        }
        public static string GetSubRenderingDocument(XmlSchemaComplexType tp, XmlSchemaSet schemas)
        {
            return null;
        }
    }
}
