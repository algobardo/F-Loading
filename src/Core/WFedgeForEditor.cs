using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Core.WF
{
    partial class WFedgeLabel
    {
        public void ModifyPrecondition(XmlDocument newPre)
        {
            preconditions = newPre;   
            OnEdgeModified(null);            
        }
    }
}
