using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


namespace Comm
{
    /// <summary>
    /// Object that incapsulates publication result.
    /// </summary>
    public class PublicationResult
    {
        private string text;

        public string Text
        {
            get { return text; }
            set { this.text = value; }
        }
    }
}
