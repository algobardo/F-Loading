using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Mobile.Fields;

namespace Mobile.Workflow
{
    public class NodeController
    {
        public String ID
        {
            get;
            private set;
        }

        public String Name
        {
            get;
            private set;
        }

        public FieldController[] Controllers 
        {
            get;
            private set;
        }

        public NodeController(String id, String name, FieldController[] controllers)
        {
            this.ID = id;
            this.Name = name;
            this.Controllers = controllers;
        }
    }
}
