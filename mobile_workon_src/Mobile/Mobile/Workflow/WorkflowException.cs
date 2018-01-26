using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Mobile.Workflow
{
    public enum WorkflowExceptionType { WorkflowSchemaBadFormat, TypesSchemaBadFormat, TypeError, OperationNotFound, TypeNotSupported } 
    class WorkflowException : Exception
    {
        public WorkflowException(WorkflowExceptionType type, String message)
            : base(message)
        {
        }

        public WorkflowException(WorkflowExceptionType type, String message, Exception inner)
            : base(message, inner)
        {
        }

        public WorkflowExceptionType Type
        {
            get;
            private set;
        }
    }
}
