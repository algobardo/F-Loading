using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Storage;
using System.Xml.Linq;

namespace Comm.Report
{
    /// <summary>
    /// Html strategy for build a publication result.
    /// </summary>
    class HtmlPublicationResultStrategy : ReportStrategy
    {

        private static string templateHeader = "<html xmlns=\"http://www.w3.org/1999/xhtml\" >" +
                "<head>" +
                "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />" +
                "<title>Floading Rss Channel</title>" +
                "<link href=\"http://" + Storage.StorageManager.getEnvValue("webServerAddress") + "/CommWeb/css/rss.css\" rel=\"stylesheet\" type=\"text/css\" />" +
                "</head>" +
                "<body>" +
                "<div>" +
                "<div class=\"header\">  </div>";

        private static string templateEnd = "</div></body></html>";

        private string BuildSubDiv(XElement el)
        {
            StringBuilder builder = new StringBuilder();
            System.Collections.Generic.IEnumerable<XElement> childsEnum = el.Elements();
            foreach (System.Xml.Linq.XElement quest in childsEnum)
            {
                if ((quest.FirstNode.NodeType != XmlNodeType.Text) && ((XElement) quest.FirstNode).Name != "Value")
                {
                    builder.Append("<div class=\"inside\">");
                    builder.Append("<h3 class=\"step\">" + XmlConvert.DecodeName(quest.Name.LocalName) + "</h3>");
                    System.Collections.Generic.IEnumerable<XElement> childsQuestEnum = quest.Elements();
                    foreach (System.Xml.Linq.XElement childOfChild in childsQuestEnum)
                    {
                        builder.Append(BuildSubDiv(childOfChild));
                    }
                    builder.Append("</div>");
                }
                else
                {
                    builder.Append("<p> <span class=\"question\">" + 
                        XmlConvert.DecodeName((quest.FirstNode.NodeType == XmlNodeType.Text) ? el.Name.LocalName : quest.Name.LocalName) +
                            " = </span>  <span class=\"answer\">" + XmlConvert.DecodeName(quest.Value)+ "</span> </p>");
                }
            }

            return builder.ToString();
        }

        private string BuildDiv(Result res)
        {
            XElement xmlRes = res.xmlResult;
            StringBuilder builder = new StringBuilder();
            builder.Append("<div class=\"content\">");
            builder.Append("<h1>" + XmlConvert.DecodeName(xmlRes.Name.LocalName) + "</h1>");
            if (res.CompilationRequest != null)
                builder.Append("<h2>Report Loa per l'utente" + res.CompilationRequest.Contact.nameContact + "</h2>");
            else
                builder.Append("<h2>Report Loa utente anonimo</h2>");
            System.Collections.Generic.IEnumerable<XElement> elements = xmlRes.Elements();
            foreach (System.Xml.Linq.XElement el in elements)
            {
                builder.Append("<div>");
                builder.Append("<h3 class=\"step\">" + XmlConvert.DecodeName(el.Name.LocalName) + "</h3>");
                builder.Append(BuildSubDiv(el));
                builder.Append("</div>");
            }
            builder.Append("</div>");
            return builder.ToString();
        }

        public override PublicationResult ConvertOutput(Publication pub)
        {
            PublicationResult result = new PublicationResult();
            StringBuilder builder = new StringBuilder();
            builder.Append(templateHeader);
            HashSet<int> resultAdded = new HashSet<int>();
            foreach (CompilationRequest req in pub.CompilationRequest)
            {
                foreach (Result res in req.Result)
                {
                    Console.WriteLine("res: " + res.resultID);
                    if (res.xmlResult != null)
                    {
                        builder.Append(BuildDiv(res));
                        resultAdded.Add(res.resultID);
                    }
                }
            }
            foreach(Result res in pub.Result) 
            {
                if (resultAdded.Contains(res.resultID))
                    continue;
                builder.Append(BuildDiv(res));
            }
            builder.Append(templateEnd);
            result.Text = builder.ToString();
            return result;
        }

        public override PublicationResult ConvertOutput(Result res)
        {
            PublicationResult result = new PublicationResult();
            
            result.Text = templateHeader + BuildDiv(res) + templateEnd;
            return result;
        }

        public override string Description
        {
            get { return "Cool Email Result"; }
        }
    }
}
