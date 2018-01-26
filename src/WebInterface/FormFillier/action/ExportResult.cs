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
using Core.WF;
using WebInterface.FormFillier;
using System.Xml;
using System.IO;
using System.Xml.Schema;

namespace WebInterface.FormFillier.action
{
    public class ExportResult : System.Web.UI.Control, ICallbackEventHandler, IControlFormFiller
    {
        string result;
        public string GetCallbackResult()
        {
            return result;
        }
        public void RaiseCallbackEvent(string eventArgument)
        {
            result = "";
            try
            {
                Security.Token tok = (Security.Token)Page.Session["Token"];
                Security.User usr = tok.GetCurrentUser();
                List<Security.FilledWorkflowReference> list = usr.GetCompiledForms();

                Security.FilledWorkflowReference work = list[Int32.Parse(eventArgument)];
                XmlSchemaSet bh = work.GetWorkflow().GetCollectedDocumentSchemas();

                XmlSchema schemaTemp = new XmlSchema();

                foreach (XmlSchemaObject tp in bh.GlobalAttributes.Values)
                {
                    schemaTemp.Items.Add(tp);
                }
                foreach (XmlSchemaObject s in bh.GlobalElements.Values)
                {
                    schemaTemp.Items.Add(s);
                }
                foreach (XmlSchemaObject tp in bh.GlobalTypes.Values)
                    schemaTemp.Items.Add(tp);

                string schemaContent = Utils.WriteSchema(schemaTemp);
                List<Storage.StorageManager.Pair<Security.Contact, XmlDocument>> doc2 = work.getFilledDocument();

                XmlDocument finaldoc = new XmlDocument();
                XmlDeclaration dec = finaldoc.CreateXmlDeclaration("1.0", "utf-8", null);
                XmlNode firstNode = finaldoc.CreateElement("ExportResults");
                finaldoc.InsertBefore(dec, finaldoc.DocumentElement);
                finaldoc.AppendChild(firstNode);
                XmlNode schema = finaldoc.CreateElement("Schema");

                XmlDocument fi = new XmlDocument();
                fi.LoadXml(schemaContent);
                fi.RemoveChild(fi.FirstChild);

                schema.InnerXml = fi.FirstChild.InnerXml;

                firstNode.AppendChild(schema);
                XmlNode data = finaldoc.CreateElement("Data");
                firstNode.AppendChild(data);
                int i = 0;

                string url = HttpContext.Current.Request.PhysicalApplicationPath + "FormFillier\\result.xml";
                foreach (Storage.StorageManager.Pair<Security.Contact, XmlDocument> coppia in doc2)
                {
                    i++;
                    Security.Contact c = coppia.First;
                    if (c != null)
                    {
                        XmlNode compiler = finaldoc.CreateElement("Compiler");
                        XmlNode mail = finaldoc.CreateElement("Mail");
                        XmlNode name = finaldoc.CreateElement("Name");
                        mail.InnerText = c.Email;
                        name.InnerText = c.Name;
                        compiler.AppendChild(name);
                        compiler.AppendChild(mail);
                        data.AppendChild(compiler);
                    }

                    XmlDocument doc = coppia.Second;
                    XmlNode nodeData = doc.DocumentElement;
                    data.AppendChild(finaldoc.ImportNode(nodeData, true));

                }
                
                Page.Session["resultXml"] = finaldoc;

            }
            catch (Exception e)
            {
                result = "no";
            }
        }

        #region IControlFormFiller methods

        public string getNameFunctionServerCall()
        {
            return "ExportCall";
        }

        public string getNameFunctionServerResponse()
        {
            return "ExportResult";
        }

        #endregion
    }
}
