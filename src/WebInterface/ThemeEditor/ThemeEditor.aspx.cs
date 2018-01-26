using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Security;
using Core.WF;
using System.Collections.Generic;
using Fields;
using System.Xml;
using System.Xml.Schema;
using System.Text;
using System.IO;


/*
 * TODO:
 *  - Add Image resize with ratio (WTFFFFF)
 * 
*/
namespace WebInterface.ThemeEditor
{
    public partial class ThemeEditor : System.Web.UI.Page
    {

        private const string CssAddJS = "toolbox.addChanges('{0}', '{1}', '{2}', '{3}');";



        protected void Page_Load(object sender, EventArgs e)
        {

            // Init the Session element that bring the logo to null
            if (Page != null)
            {
                Page.Session["TE_currentLogo"] = null;
            }
            // Taking the CurrentWorkflow from the session elemnt.
            ComputableWorkflowReference wfr = null;
            IComputableWorkflow wf = null;



            try
            {
                wfr = (ComputableWorkflowReference)Page.Session["WFE_CurrentWorkflow"];
                wf = wfr.GetWorkflow();
            }
            catch (Exception)
            {
                Response.Write("No workflow in session!");
                Response.End();
                return;
            }


            bool useStaticLabel = false;
            try
            {
                useStaticLabel = (bool)Page.Session["UsingStaticFields"];
            }
            catch (Exception)
            {
                Response.Write("No UsingStaticFields in session!");
                Response.End();
                return;
            }

            if (useStaticLabel)
                editStaticLabel.InnerHtml = "<label for=\"color_staticLabel\">Static Label Color:</label><input rel=\"staticLabel\" class=\"gccolor\" onblur=\"toolbox.applyChanges('staticLabel','color', this.value, 'color' );\" title=\"Color of the Static Label. Example: #ff00ff\" id=\"color_staticLabel\" type=\"text\"/><div style=\"display:none;\"><input id=\"staticLabelApplyToID\" type=\"checkbox\" /></div>";
            else
                editStaticLabel.InnerHtml = "";

            // bla bla bla
            int tabCounter = 1;

            List<Panel> divPanel = new List<Panel>();

            presenPanel.Controls.Add(new LiteralControl("<ul class=\"tabs\">"));

            // Iterating each workflow node to render the form elemnt list

            foreach (WFnode nd in wf.getNodeList())
            {

                Panel p = new Panel();
                //p.Enabled = false;
                XmlDocument renderDoc = nd.GetRenderingDocument();



                string nodeName = XmlConvert.DecodeName(renderDoc.DocumentElement.Attributes["name"].InnerText);

                presenPanel.Controls.Add(new LiteralControl("<li><a href=\"#tabs" + tabCounter + "\" >" + nodeName + "</a></li>"));

                //p.Controls.Add(new LiteralControl("<div id=\"tabs" + tabCounter + "\" style=\"position: relative; height: 500px;\" >"));

                // Hangling the webcontrol
                XmlNode cmplexRenering = renderDoc.DocumentElement.FirstChild;
                Control wc = nd.GetWebControl(Page.Server, cmplexRenering);

                //Disabling all BaseValidator to remove useless text <.<
                DisableControl(wc);

                p.Controls.Add(new LiteralControl("<div>"));
                p.Controls.Add(wc);
                p.Controls.Add(new LiteralControl("</div>"));

                divPanel.Add(p);

                tabCounter++;
            }

            presenPanel.Controls.Add(new LiteralControl("</ul>"));
            presenPanel.Controls.Add(new LiteralControl("<div class=\"panes\"> "));
            // Add each panel to presenPanel
            foreach (var p in divPanel)
                presenPanel.Controls.Add(p);
            presenPanel.Controls.Add(new LiteralControl("</div>"));

            // Retriving the current Theme informations
            IWorkflowThemeReference iwfr = (IWorkflowThemeReference)Page.Session["WFE_CurrentWorkflow"];

            if (Page.Session["CurrentTheme"] == null)
                Page.Session["CurrentTheme"] = (Theme)iwfr.GetTheme();

            Theme theme = (Theme)Page.Session["CurrentTheme"];

            //Theme theme = (Theme)iwfr.GetTheme();

            string propertyListJson = "<script language='javascript'> initTE('" + theme.Title + "');";
            propertyListJson += " var PropertyList=function(){";
            IBaseType field;
            foreach (Type t in FieldsManager.FieldTypes)
            {
                field = (IBaseType)FieldsManager.GetInstance(t);
                if (!String.IsNullOrEmpty(field.JSON_StyleProperties))
                    propertyListJson += "var " + field.GetType().Name + "PropertyList = " + field.JSON_StyleProperties + ";";

            }
            propertyListJson +=
                 @"
                    return{
                        getList:function( type ){
                            if( eval( ""typeof "" + type + ""PropertyList == 'undefined'"" ) ) {
                                type = ""StringBox"";
                            }            
                            return eval( type + ""PropertyList"");
                        },
                        validate:function( type, value ){
			                if( type == ""color"" )
				                return (value.match( /#[a-fA-F0-9]{6}/ ) || value.match(/rgb\([ ]*[0-9]{1,3}[ ]*,[ ]*[0-9]{1,3}[ ]*,[ ]*[0-9]{1,3}[ ]*\)/i));
			                if( type == ""size"" ) 
				                return ( value.match( /\d+px/ ) || value.match( /\d+\%/ ) || value.match( /\d+em/ ));
			                if( type == ""none"" )
				                return true;
                            if( type == ""font"" )
                                return true;
			                return false;            
                        }
                   };
                }(); ";


            CssParser cssParser = new CssParser();

            cssParser.AddStyleSheet(theme.CSS);
            foreach (var styles in cssParser.Styles)
            {
                foreach (KeyValuePair<string, string> pv in styles.Value.Attributes)
                    propertyListJson += string.Format(CssAddJS, styles.Key.Substring(0, 1), styles.Key.Substring(1), pv.Key, pv.Value);

            }
            if (theme.Image != null)
            {
                Page.Session["TE_currentLogo"] = theme.Image;
                propertyListJson += "stopUpload('drawLogo.aspx', true);";
            }

            propertyListJson += "</script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", propertyListJson);

            //Adding the CSS to the Page Header.
            Page.Header.Controls.Add(
                 new LiteralControl(
                      @"<style type='text/css'>" + theme.CSS + "</style" + ">"
                      )
            );

            //for manage contacts back button (GUI modify)
            Session["contactReturn"] = "/ThemeEditor/ThemeEditor.aspx";
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            DisableControl(presenPanel);
        }


        private void DisableControl(Control c)
        {
            try
            {
                c.Visible = true;
            }
            catch (Exception e)
            {
                ;
            }
            if (c is BaseValidator)
            {
                ((BaseValidator)c).Enabled = false;
                ((BaseValidator)c).Visible = false;
            }
            else if (c is Rating.RatingControl)		// very BAD Control (too specific)
            {
                Rating.RatingControl r = (Rating.RatingControl)c;
                if (r.MaxRating > 2) r.CurrentRating = 2;
                r.Enabled = false;
            }
            /* slightly better code
             * else if ((c is WebControl) && !(c is TextBox))
             *		((WebControl)c).Enabled = false;
             */

            // else ??? <--- mi sa che ci vuole un else.. a meno che tu non debba controllare anche i figli dei baseValidator
            if (c is UpdatePanel)
            {
                foreach (Control child in ((UpdatePanel)c).ContentTemplateContainer.Controls)
                    DisableControl(child);
            }
            else
            {
                foreach (Control child in c.Controls)
                    DisableControl(child);
            }
        }
    }
}
