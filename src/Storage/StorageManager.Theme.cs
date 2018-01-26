using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Data;
using System.Data.Linq;
using System.IO;
using System.Drawing.Imaging;

namespace Storage
{
    public partial class StorageManager
    {
        /// <summary>
        /// Add a new Theme
        /// </summary>
        /// <param name="userID">The ID of owner user</param>
        /// <param name="nameTheme">Name theme</param>
        /// <param name="css">css theme</param>
        /// <param name="logo">image theme</param>
        /// <returns>Theme object</returns>
        public Theme addTheme(int userID, string nameTheme, string css, Image logo)
        {            
            MemoryStream ms = new MemoryStream();
            logo.Save(ms, ImageFormat.Gif);

            Theme t = new Theme()
            {
                userID = userID,
                themeTitle = nameTheme,
                CSS = css,
                logo = ms.ToArray()
            };

            return AddEntity(t);
        }
    }
}

