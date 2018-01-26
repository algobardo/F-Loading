using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using System.Web.UI;


[assembly: System.Web.UI.WebResource("Fields.images.namedchoice_icon.png", "image/png")]
namespace Fields
{
    /// <summary>
    /// This class represents a 
    /// <choice>
    /// ...
    /// </choice>
    /// 
    /// </summary>
    class Choice:IComplexType
    {
        private XmlSchemaChoice namedChoice;
        private XmlSchemaSet schemas;

        private List<XmlNode> value;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedChoice"/> class from a XMLSchema
        /// </summary>
        /// <param name="schema">The schema xml that describe the NamedChoice state.</param>
        public Choice(XmlSchemaSet schemas, XmlSchemaChoice namedChoice)
        {
            this.schemas = schemas;
            this.namedChoice = namedChoice;
        }
                
        #region IField Members

        public XmlNode Value
        {
            get
            {
                throw new InvalidOperationException();
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public void SetValue(List<XmlNode> nds) { this.value = nds; }

        public string GetValue(Control src, XmlNode renderingDocument)
        {
            UpdatePanel up = ((UpdatePanel)src);
            ChoicePanel cp = ((ChoicePanel)up.ContentTemplateContainer.Controls[0]);

            int schemaChildCount = 0;
            int controlCount=0;            
            foreach (XmlNode n in renderingDocument.ChildNodes)
            {
                if (n.Name.StartsWith("xs_")){
                     if(((RadioButton)cp.Controls[controlCount]).Checked){
                         return FieldsManager.GetInstance(n,namedChoice.Items[schemaChildCount],schemas).GetValue(cp.Controls[controlCount+1] ,n);
                         
                     }
                    schemaChildCount++;
                    controlCount+=3;
                }
                else
                    controlCount++;
            }
            return "";
        }

        public void setExampleValue()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRenderable Members

        public WebControl GetWebControl()
        {
            throw new NotImplementedException();
        }

        public Control GetWebControl(System.Web.HttpServerUtility server, System.Xml.XmlNode renderingDocument)
        {
            UpdatePanel up = new UpdatePanel();            
            ChoicePanel cp = new ChoicePanel(renderingDocument, namedChoice, schemas, value);
            up.ContentTemplateContainer.Controls.Add(cp);
            return up;
        }

        public List<BaseValidator> GetValidators()
        {
            throw new NotImplementedException();
        }

        public List<BaseValidator> GetValidators(Control controlId)
        {
            throw new NotImplementedException();
        }

      

        public string JSON_StyleProperties
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Icon

        public static WebControl Icon
        {
            get
            {
                return new CustomImage("Fields.images.namedchoice_icon.png", "Creates a choice group");
            }
        }

        #endregion
    }
}
