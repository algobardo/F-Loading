using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebInterface
{
    public class FloadingButton
    {
        #region METHODS TO ADD A BUTTON

        //add a disabled button
        public static void createFloadingButton(Page page, String button_id, String button_text)
        {
            string buttonHTML;
            buttonHTML = "<div id=\"" + button_id + "\" class=\"floading-button-disabled\" >" + //"\n" +
                         "<div class=\"floading-button-disabled-left\" ></div>" + //"\n" +
                         "<div class=\"floading-button-disabled-center\" >" + //"\n" +
                         button_text + //"\n" +
                         "</div>" + //"\n" +
                         "<div class=\"floading-button-disabled-right\" ></div>" + //"\n" +
                         "</div>";//+ "\n";
            page.Response.Write(buttonHTML);
        }

        //add an activated button
        public static void createFloadingButton(Page page, String button_id, String button_text, String function_to_call_on_click)
        {
            string buttonHTML;
            buttonHTML = "<div id=\"" + button_id + "\" class=\"floading-button-normal\" onclick=\"" + function_to_call_on_click + "\">" + //"\n" +
                         "<div class=\"floading-button-left\" onmouseover=\"buttonOver('" + button_id + "');\" onmouseout=\"buttonOut('" + button_id + "');\" onmousedown=\"buttonDown('" + button_id + "');\" onmouseup=\"buttonUp('" + button_id + "');\"></div>" + //"\n" +
                         "<div class=\"floading-button-center\" onmouseover=\"buttonOver('" + button_id + "');\" onmouseout=\"buttonOut('" + button_id + "');\" onmousedown=\"buttonDown('" + button_id + "');\" onmouseup=\"buttonUp('" + button_id + "');\">" + //"\n" +
                         button_text + //"\n" +
                         "</div>" + //"\n" +
                         "<div class=\"floading-button-right\" onmouseover=\"buttonOver('" + button_id + "');\" onmouseout=\"buttonOut('" + button_id + "');\" onmousedown=\"buttonDown('" + button_id + "');\" onmouseup=\"buttonUp('" + button_id + "');\"></div>" + //"\n" +
                         "</div>";// +"\n";
            page.Response.Write(buttonHTML);
        }
        #endregion

    }
}