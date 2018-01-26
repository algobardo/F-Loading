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
    /// Naive strategy for build a publication result.
    /// Iterate over all compilation result and build
    /// a string that contains a list of node's names,
    /// its attributes and the text value.
    /// </summary>
    public class NaivePublicationResultStrategy : ReportStrategy
    {

        private string buildNode(XElement doc)
        {
            StringBuilder buf = new StringBuilder();
            IEnumerable<XElement> elements = doc.Elements();
            foreach (XElement el in elements)
            {
                buf.Append(el.Name).Append("=").Append(el.Value).Append("\n");
            }
            foreach (XElement child in doc.Descendants())
                buf.Append("\t").Append(buildNode(child));

            return buf.ToString();
        }

        #region ReportStrategy Membri di

        public override PublicationResult ConvertOutput(Result data)
        {
            PublicationResult buf = new PublicationResult();
            buf.Text = buildNode(data.xmlResult);
            return buf;
        }

        public override PublicationResult ConvertOutput(Publication pub)
        {
            StringBuilder buf = new StringBuilder();
            foreach (Result dataResult in pub.Result)
                buf.Append(ConvertOutput(dataResult).Text).Append("\n");

            PublicationResult result = new PublicationResult();
            result.Text = buf.ToString();
            return result;
        }

        public override string Description
        {
            get { return "Simple text"; }
        }

        #endregion
    }
}
