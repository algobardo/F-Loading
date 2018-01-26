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
    

    class FieldsException : ApplicationException
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
        public FieldsException( ) { }

        /// <summary>
        /// Constructor with parameters specified below
        /// </summary>
        /// <param name="severityLevel"></param>
        /// <param name="logLevel"></param>
        /// <param name="exception"></param>
        /// <param name="customMessage"></param>
        public FieldsException(SeverityLevel severityLevel, LogLevel logLevel, Exception exception, string customMessage)
        {
            this.SeverityLevelOfException = severityLevel;
            this.LogLevelOfException = logLevel;
            this.InnerExc = exception;
            this.CustomMessage = customMessage;
        }

        /// <summary>
        /// Constructor with parameters specified below
        /// </summary>
        /// <param name="severityLevel"></param>
        /// <param name="logLevel"></param>
        /// <param name="exception"></param>
        /// <param name="customMessage"></param>
        public FieldsException(SeverityLevel severityLevel, LogLevel logLevel, string customMessage)
        {
            this.SeverityLevelOfException = severityLevel;
            this.LogLevelOfException = logLevel;
            this.InnerExc = new Exception();
            this.CustomMessage = customMessage;
        }

    }

}
