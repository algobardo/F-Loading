using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Mobile.Fields
{
    /// <summary>
    /// Severity level of Exception 
    /// (The Severity level determines the criticality of the error.)
    /// </summary>
    public enum SeverityLevel
    {
        Fatal,
        Critical,
        Information
    }

    
    /// <summary>
    /// Log level of Exception 
    /// (The Loglevel determines whether an entry needs to be made in Log. Based on the log level chosen , 
    /// entries can be made either in the Debug Log or System Event Log .)
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Event
    }
    
    /// <summary>
    /// These class allows developer to create FieldsException objects representing field's related errors
    /// </summary>
    class FieldException : ApplicationException
    {

        // defines the severity level of the Exception
        public SeverityLevel SeverityLevelOfException { get; set; }
        
        // defines the logLevel of the Exception
        public LogLevel LogLevelOfException { get; set; }
        
        // System Exception that is thrown
        public Exception InnerExc { get; private set; }
        
        // Custom Message
        public string CustomMessage { get; set; }
        

        /// <summary>
        /// Standard default Constructor
        /// </summary>
        public FieldException( ) { }

        /// <summary>
        /// Constructor with parameters specified below
        /// </summary>
        /// <param name="severityLevel">the enumeration representing the exception's criticality</param>
        /// <param name="logLevel">the enumeration representing the LogLevel of the excpetion</param>
        /// <param name="exception">representing the <see cref="Exception"/>object to be used as InnerException in order to create the <see cref="FieldException"/> element</param>
        /// <param name="customMessage">The message to be show</param>
        public FieldException(SeverityLevel severityLevel, LogLevel logLevel, Exception exception, string customMessage)
        {
            this.SeverityLevelOfException = severityLevel;
            this.LogLevelOfException = logLevel;
            this.InnerExc = exception;
            this.CustomMessage = customMessage;
        }

        /// <summary>
        /// Constructor with parameters specified below
        /// </summary>
        ///<param name="severityLevel">the enumeration representing the exception's criticality</param>
        /// <param name="logLevel">the enumeration representing the LogLevel of the excpetion</param>
        /// <param name="customMessage">The message to be show</param>
        public FieldException(SeverityLevel severityLevel, LogLevel logLevel, string customMessage)
        {
            this.SeverityLevelOfException = severityLevel;
            this.LogLevelOfException = logLevel;
            this.InnerExc = new Exception();
            this.CustomMessage = customMessage;
        }

    }

}
