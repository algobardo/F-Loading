using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Mobile.Communication
{
    public class FormRequestInfo
    {
        public FormRequestInfo()
        {
        }

        /// <summary>
        /// the workflow's identificator 
        /// </summary>
        public int WorkflowId
        {
            get;
            set;
        }


        public int CompilationRequestId
        {
            get;
            set;
        }

        public String Username
        {
            get;
            set;
        }

        public String Service
        {
            get;
            set;
        }

        public String Token
        {
            get;
            set;
        }

        public String From
        {
            get;
            set;
        }

        public DateTime Time
        {
            get;
            set;
        }
    }
}
