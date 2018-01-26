using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Drawing;
using System.Xml.Schema;
using System.Web.UI;
using System.Diagnostics;

namespace Fields
{
    class ChoicePanel : Panel, INamingContainer
    {
        System.Xml.XmlNode renderingDocument;
        XmlSchemaChoice obj;
        XmlSchemaSet schemas;
        List<XmlNode> value;

        public ChoicePanel(System.Xml.XmlNode renderingDocument, XmlSchemaChoice obj, XmlSchemaSet schemas, List<XmlNode> value)
        {
            this.renderingDocument = renderingDocument;
            this.obj = obj;
            this.schemas = schemas;
            this.value = value;

            this.BorderStyle = BorderStyle.Dashed;
            this.BorderColor = Color.FromArgb(167, 167, 167);
            this.BorderWidth = 1;
        }

        public void AddItems()
        {
            int schemaChildCount = 0;
            foreach (XmlNode n in renderingDocument.ChildNodes)
            {
                if (n.Name.StartsWith("xs_"))
                {
                    bool itemChecked = false;

                    XmlSchemaObject rndObj = obj.Items[schemaChildCount];
                    var field = FieldsManager.GetInstance(n, rndObj, schemas);
                    
                    if (value != null)
                    {   
                        List<XmlNode> d = null;
                        foreach (XmlNode nvalue in value)
                        {
                            if (nvalue.SchemaInfo.SchemaElement.Name == ((XmlSchemaElement)rndObj).Name)
                            {
                                d = new List<XmlNode>();
                                d.Add(nvalue);
                                itemChecked = true;
                            }
                        }
                        if(itemChecked)
                            ((IField)field).SetValue(d);
                        
                    }
                    
                    var wctrl = field.GetWebControl(Page.Server, n);

                    RadioButton rb = new RadioButton();
                    rb.AutoPostBack = true;
                    rb.CheckedChanged+=new EventHandler(rb_CheckedChanged);
        
                    rb.GroupName = this.ID + "choice";

                    if (rndObj is XmlSchemaElement)
                    {
                        var rndElem = (XmlSchemaElement)rndObj;
                        if (n.Attributes["label"] != null)
                            rb.Text = n.Attributes["label"].Value.FromXmlValue2Render(Page.Server);
                        else
                            rb.Text = rndElem.Name.FromXmlName2Render(Page.Server);
                    }
                    else
                        rb.Text = ("UNNAMED CHOICE");

                    rb.Text += "<BR>";
                 
                    if(!itemChecked)
                        wctrl.Visible = false;
    
                    if (itemChecked)
                        rb.Checked = true;

                    this.Controls.Add(rb);                    
                    this.Controls.Add(wctrl);
                    this.Controls.Add(new LiteralControl("<BR>"));

                    schemaChildCount++;
                }
                else
                {
                    var wctrl = FieldsManager.GetInstance(n, null, null).GetWebControl(Page.Server, n);
                    this.Controls.Add(wctrl);
                    this.Controls.Add(new LiteralControl("<BR>"));

                }

            }
        }

        void rb_CheckedChanged(object sender, EventArgs e)
        {
            bool next = false;
            bool nextHidden = false;
            foreach (Control c in this.Controls)
            {
                if (next)
                {
                    c.Visible = true;
                    next = false;
                }
                else if (nextHidden)
                {
                    c.Visible = false;
                    nextHidden = false;
                }
                else if (c is RadioButton && (c as RadioButton).Checked)
                {
                    next = true;
                }
                else if (c is RadioButton && !(c as RadioButton).Checked)
                {
                    nextHidden = true;
                }

            }
        }
  
        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this);
            base.OnInit(e);
            AddItems(); 
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
    }
}
