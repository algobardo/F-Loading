using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storage
{
    public partial class StorageManager
    {

        /// <summary>
        /// Add a new directory in the user file system
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="nameDirectory"></param>
        /// <param name="parentDirectoryID"></param>
        /// <returns>Directory reference or null if error occurs</returns>
        public Directory addDirectory(int userID, string nameDirectory, int parentDirectoryID) 
        {

            Directory d = new Directory()
            {
                nameDirectory = nameDirectory,
                parentID = parentDirectoryID,
                User = getEntityByID<User>(userID)
            };

            return AddEntity(d);

        }

        /// <summary>
        /// Move a file to a new directory.
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="newDirectoryID"></param>
        /// <returns>true = ok, false if errors occur</returns>
        public bool moveFileToDirectory(int fileID, int newDirectoryID) {
            FilesUploaded f = getEntityByID<FilesUploaded>(fileID);
            f.directoryID = newDirectoryID;

            return commit();
        }

        /// <summary>
        /// Move a file to the default directory.
        /// This method does NOT delete the file!
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="directoryID"></param>
        /// <returns>true = ok, false if errors occur</returns>
        public bool removeFileFromDirectory(int fileID, int directoryID) 
        {
            FilesUploaded f = getEntityByID<FilesUploaded>(fileID);
            f.directoryID = null;

            return commit();

        
        }
	
    }
}