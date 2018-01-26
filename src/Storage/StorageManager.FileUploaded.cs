using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storage
{
    public partial class StorageManager
    {
        /// <summary>
        /// Generate a new instance of FilesUploaded; this is needed to store file's binary data.
        /// </summary>
        /// <param name="directoryID"></param>
        /// <param name="name"></param>
        /// <returns>FilesUploaded reference or null if errors occur</returns>
        public FilesUploaded uploadFile(int directoryID, string name) 
        {
            FilesUploaded f = new FilesUploaded()
            {
                directoryID = directoryID,
                fileName = name
            };

            return AddEntity(f);

        }
    }
}