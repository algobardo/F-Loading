using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Storage;

namespace Comm.Report
{
    
    /// <summary>
    /// Strategy Pattern.
    /// </summary>
    public abstract class ReportStrategy
    {
        /// <summary>
        /// Returns a rappresentation, in some human readable format,
        /// of all compilation request of publication
        /// </summary>
        /// <param name="builder">The builder</param>
        public abstract PublicationResult ConvertOutput(Publication pub);

        public abstract PublicationResult ConvertOutput(Result res);
        /// <summary>
        /// Returns a short description of strategy method
        /// </summary>
        public abstract string Description
        {
            get;
        }
    }
}
