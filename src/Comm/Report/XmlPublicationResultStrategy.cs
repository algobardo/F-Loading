using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Storage;
using System.Xml;
using System.Xml.Linq;

namespace Comm.Report
{
    /// <summary>
    /// Xml strategy for build a publication result.
    /// </summary>
    public class XmlPublicationResultStrategy : ReportStrategy
    {
        private string buildNode(XElement doc)
        {
            StringBuilder buf = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(buf);
            doc.Save(writer);
            writer.Close();

            return XmlConvert.DecodeName(buf.ToString());
        }


        #region ReportStrategy Membri di

        public override PublicationResult ConvertOutput(Result data)
        {
            PublicationResult result = new PublicationResult();
            result.Text = buildNode(data.xmlResult);
            return result;
        }

        public override PublicationResult ConvertOutput(Publication pub)
        {
            StringBuilder buf = new StringBuilder();

            buf.Append("<XML_FLOADING_RESULT>");
            foreach (Result res in pub.Result)
                buf.Append(ConvertOutput(res).Text).Append("\n");

            buf.Append("</XML_FLOADING_RESULT>");
            PublicationResult result = new PublicationResult();
            result.Text = buf.ToString();
            return result;
        }

        public override string Description
        {
            get { return "Xml Raw Result"; }
        }

        #endregion
    }
}
