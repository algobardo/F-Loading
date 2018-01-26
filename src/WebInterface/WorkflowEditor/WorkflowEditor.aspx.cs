using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebInterface.WorkflowEditor.action;
using Fields;
using Core.WF;
using System.Web.UI.HtmlControls;

namespace WebInterface.WorkflowEditor
{
    public partial class WorkflowEditor : System.Web.UI.Page
    {

        //public UpdatePanel upcontrol;
        private List<Control> listControl;

        protected void Page_Load(object sender, EventArgs e)
        {            
            listControl = new List<Control>();
            listControl.Add(new WFG_addNode());
            listControl.Add(new WFG_addArc());
            listControl.Add(new WFG_createWorkflow());
            listControl.Add(new WFE_saveWorkflow());
            listControl.Add(new WFC_publishWorkflow());
            listControl.Add(new WFG_initialNode());
            listControl.Add(new WFG_finalNode());
            listControl.Add(new WFF_addField());
            listControl.Add(new WFG_removeArc());
            listControl.Add(new WFG_removeNode());
            listControl.Add(new WFC_getPredicates());
            listControl.Add(new WFC_getOperations());
            listControl.Add(new WFC_setProperties());
            listControl.Add(new WFC_syncNode());
            listControl.Add(new WFF_getConstraints());
            listControl.Add(new WFF_saveConstraints());
            listControl.Add(new WFC_LoadXmlDocFromSession());
            listControl.Add(new WFC_getAuthServiceList());
		    //listControl.Add(new WebInterface.FormFillier.action.ContactsRetrieve());
            //listControl.Add(new WebInterface.FormFillier.action.UserWorkflowSend());

            String cbReference;
            String callbackScript;

            List<Control>.Enumerator it = listControl.GetEnumerator();
            while (it.MoveNext())
            {
                Page.Controls.Add(it.Current);
                IControlWorkflowEditor t = (IControlWorkflowEditor)it.Current;
                cbReference =
                Page.ClientScript.GetCallbackEventReference(it.Current, "arg", t.getNameFunctionServerResponse(), "context");
                callbackScript = "function " + t.getNameFunctionServerCall() + "(arg, context)" +
                    "{ " + cbReference + ";}";

                Page.ClientScript.RegisterClientScriptBlock(it.Current.GetType(),
                    t.getNameFunctionServerCall(), callbackScript, true);
            }

            //Add WebControls to the UpdatePanel
            IBaseType field;
            //OLD System.Drawing.Image field_icon;
            WebControl field_icon, field_preview, field_rendering, special_icon, field_precondition;
            foreach (Type t in FieldsManager.FieldTypes) {
                //instantiate the Field for each type of field
                field = (IBaseType)FieldsManager.GetInstance(t);
                //get the type name
                field.TypeName = t.ToString();
                //get the icon
                field_icon = FieldsManager.GetIconControl(t);
                //get the preview html()
                field_preview = FieldsManager.GetPreviewControl(t);
                //get the rendering (big image) to be added onmouseover
                field_rendering = FieldsManager.GetPreviewTooltipControl(t);
                field_precondition = FieldsManager.GetUnrelatedControl(t);


                String[] s1 = t.ToString().Split('.');
                field_preview.Attributes.Add("class", "WFF_" + s1[1]);
                HtmlGenericControl p1 = new HtmlGenericControl("div");
                p1.Controls.Add(field_preview);
                p1.Attributes.Add("id", s1[1]);
                upcontrols.ContentTemplateContainer.Controls.Add(p1);

                HtmlGenericControl p2 = new HtmlGenericControl("div");
                p2.Attributes.Add("id", s1[1]);
                p2.Attributes.Add("class", "WFF_widget_in_uppanel");
                p2.Controls.Add(field_icon);
                upicons.ContentTemplateContainer.Controls.Add(p2);

                HtmlGenericControl p3 = new HtmlGenericControl("div");
                p3.Attributes.Add("id", s1[1]);
                p3.Attributes.Add("class", "WFF_rendering_preview");
                p3.Controls.Add(field_rendering);
                uprendering.ContentTemplateContainer.Controls.Add(p3);

                field_precondition.Attributes.Add("class", "precondition_webcontrol_preview");
                HtmlGenericControl p4 = new HtmlGenericControl("div");
                p4.Controls.Add(field_precondition);
                p4.Attributes.Add("id", "precondition_"+s1[1]);
                //preconditionup.ContentTemplateContainer.Controls.Add(p4);
            }

            foreach (Type s in FieldsManager.SpecialFieldTypes)
            {
                //get the type name
                string sname = (s.ToString().Split('.'))[1];

                //get the icon
                special_icon = FieldsManager.GetIconControl(s);

                special_icon.Attributes.Add("class", "WFF_" + sname);
                HtmlGenericControl p1 = new HtmlGenericControl("div");
                p1.Controls.Add(special_icon);
                p1.Attributes.Add("id", sname);
                p1.Attributes.Add("class", "WFF_widget_in_upspecial");
                upspecial.ContentTemplateContainer.Controls.Add(p1);
            }

            //for manage contacts back button
            Session["contactReturn"] = "/WorkflowEditor/WorkflowEditor.aspx";
        }

        protected void add_field_btn_Click(object sender, EventArgs e)
        {

        }

}
}
