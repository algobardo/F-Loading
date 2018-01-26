using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;



namespace WebInterface.ThemeEditor
{
    public class customCss
    {
        //TODO: Substitute this structure with DB calls
        private static System.Collections.Generic.Dictionary<int, List<String>> cssFiles = new System.Collections.Generic.Dictionary<int, List<String>>();
        public enum resultIDs { ok, internalError, wrongFile, noFile };

        private static List<String> log = new List<string>();
        private static void logEvent(String message)
        {
            log.Add(message);
        }
        public static String[] getLog()
        {
            return log.ToArray();
        }

        public static String getCSSDir(HttpServerUtility Server)
        {
            return Server.MapPath("Data") + "\\";
        }

        /*
         * Returns the location of user-defined css file
         */
        //TODO: In case user did not specify any CSS, use the Default one.
        public static String getUserCss(int userID)
        {
            List<String> userCSSs = null;
            if (!cssFiles.TryGetValue(userID, out userCSSs))
                return null;
            
            if (userCSSs.Count == 0)
                return null;

            return userCSSs.ElementAt(0);
        }

        /*
         * Stores a css file in the system, associated with the defined user.
         */ 
        //public static resultIDs addCSS(int userID, HttpPostedFile cssFile, HttpServerUtility Server, Security.IThemeEditorToken tok)
        //{
        //    if ((cssFile != null) && (cssFile.ContentLength > 0))
        //    {
        //        //Store this in the Data subdirectory. In the future it should be changed using Storage APIs
        //        string fn = System.IO.Path.GetFileName(cssFile.FileName);
        //        string SaveLocation = getCSSDir(Server) + fn;
        //        try
        //        {
        //            //Security.IThemeEditorToken tok = Session["Token"];
        //            tok.SetCSS(new Security.Form(), new StreamReader(cssFile.InputStream).ReadToEnd());

        //            cssFile.SaveAs(SaveLocation);

        //            List<String> userCSSs = null;
        //            if (!cssFiles.TryGetValue(userID, out userCSSs))
        //            {
        //                userCSSs = new List<string>();
        //                cssFiles.Add(userID, userCSSs);
        //            }
        //            userCSSs.Add(cssFile.FileName);
        //        }
        //        catch (Exception ex)
        //        {
        //            logEvent(ex.ToString());
        //            return resultIDs.internalError;
        //        }
        //    }
        //    else
        //        return resultIDs.noFile;
        //    return resultIDs.ok;
        //}

        /*
         * Returns the list of CSS uploaded by the user.
         * Returns null if user is not valid
         */ 
        public static List<String> getCSSList(int userID)
        {
            List<String> userCSSs = null;
            cssFiles.TryGetValue(userID, out userCSSs);
            return userCSSs;
        }

    }
}
