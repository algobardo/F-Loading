using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Security;
using System.Web.UI;

namespace WebInterface
{
    public class ThemeEditorToken : System.Web.UI.Page
    {


        public ThemeEditorToken()
        {
        }

        #region IThemeEditor Members

        String SkipThemeStep()
        {
            IWorkflowThemeReference wf = (IWorkflowThemeReference)Page.Session["WFE_CurrentWorkflow"];

            try
            {
                // Trying to save the Theme. title, logo and css are not required. 
                // If passed null the system will use defaults
                Security.SetThemeResult opResult = wf.SetTheme(new Theme(null, null, null));
                // Retrive the result status
                SetThemeResult.Result status = opResult.getStatus();
                /*if ( status == Security.SetThemeResult.Result.STATUS_OK )
                     Response.Redirect("../FormFillier/managecontacts.aspx", true);*/

                return "{\"status\":\"" + status.ToString() + "\"}";

            }
            catch (Exception ex)
            {
                return "{\"status\":ERROR_EXCEPTION, errorContent:" + ex.ToString() + "}";
            }
        }


        #endregion

    }
}
