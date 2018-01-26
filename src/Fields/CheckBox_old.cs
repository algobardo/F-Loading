using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;
using System.Web.UI.WebControls;

namespace Fields
{
    public class CheckBoxField : IBaseType
    {
        private XmlSchema baseSchema;
        private bool text = false;

        public XmlNode GetValue(WebControl ctrl) { return null; }

        public CheckBoxField()
        {
            baseSchema = new XmlSchema();

            //<xs:complexType name="CheckBoxN">
            XmlSchemaComplexType newType = new XmlSchemaComplexType();
            // the name must be filled later
            newType.Name = "";
            baseSchema.Items.Add(newType);

            //<xs:complexContent>
            XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
            newType.ContentModel = complexContent;

            //<xs:extension base="CheckBox">
            XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
            complexContent.Content = complexContentExtension;
            complexContentExtension.BaseTypeName = new XmlQualifiedName("CheckBoxField");

            //<xs:sequence>
            XmlSchemaSequence seq = new XmlSchemaSequence();
            complexContentExtension.Particle = seq;

            //<xs:element name="Value" type="xs:boolean" defeult="false" />
            XmlSchemaElement elem = new XmlSchemaElement();
            seq.Items.Add(elem);
            elem.Name = "Value";
            elem.SchemaTypeName = new XmlQualifiedName("boolean", "http://www.w3.org/2001/XMLSchema");
            elem.DefaultValue = "false";
        }

        public CheckBoxField(XmlSchema schema, string name)
        {
            // in this constructor it's useful to set the ID, cause GUI is calling it
            this.Name = name;

            //why don't use the former constructor? <---
            baseSchema = new XmlSchema();

            //<xs:complexType name="CheckBoxN">
            XmlSchemaComplexType newType = new XmlSchemaComplexType();
            newType.Name = ((XmlSchemaComplexType)schema.Items[0]).Name;
            baseSchema.Items.Add(newType);

            //<xs:complexContent>
            XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
            newType.ContentModel = complexContent;

            //<xs:extension base="CheckBox">
            XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
            complexContent.Content = complexContentExtension;
            complexContentExtension.BaseTypeName = new XmlQualifiedName("CheckBox");

            //<xs:sequence>
            XmlSchemaSequence seq = new XmlSchemaSequence();
            complexContentExtension.Particle = seq;

            //<xs:element name="Value" type="xs:boolean" default="false"/>
            XmlSchemaElement elem = new XmlSchemaElement();
            seq.Items.Add(elem);
            elem.Name = "Value";
            elem.SchemaTypeName = new XmlQualifiedName("boolean", "http://www.w3.org/2001/XMLSchema");
            elem.DefaultValue = "false";
        }

        #region predicates

        [Predicate("Check if checkbox %0 is checked")]
        public static bool IsChecked(CheckBoxField c)
        {
            return c.text;
        }

        [Predicate("Check if checkbox %0 is not checked")]
        public static bool IsNotChecked(CheckBoxField c)
        {
            return !c.text;
        }


        #endregion

        #region IField Members

        public WebControl GetWebControl()
        {
            return new CheckBoxControl(this);
        }

        public List<System.Web.UI.WebControls.BaseValidator> GetValidators()
        {
            //checkbox has no validators...
            return null;
        }
        public void setExampleValue()
        {
            text = false;
        }
        public System.Xml.XmlNode Value
        {
            get
            {
                //if (text == null) text = "false";
                string root = String.IsNullOrEmpty(Name) ? "null" : Name;

                XmlDocument doc = new XmlDocument();
                XmlNode nodeValue = doc.CreateElement(root);
                nodeValue.AppendChild(doc.CreateElement("Value"));
                nodeValue.FirstChild.InnerText = text ? "true" : "false";
                return nodeValue;
            }
            set
            {
                string temp = value.FirstChild.InnerText.Trim();
                Console.WriteLine("TEMP: " + temp);
                text = temp.Equals("true");
            }
        }

        public string Name { get; set; }

        public string TypeName
        {
            set { ((XmlSchemaComplexType)baseSchema.Items[0]).Name = value; }

            get
            {
                return ((XmlSchemaComplexType)baseSchema.Items[0]).Name;
            }
        }

        public System.Xml.Schema.XmlSchemaComplexType TypeSchema
        {
            get { return (XmlSchemaComplexType)baseSchema.Items[0]; }
        }

        #endregion


        public string JSON_StyleProperties
        {
            get
            {
                return
                @"[
						{ 
						""group"": ""border"",
							""properties"": [
							{
								 ""name"":""border-width"",
								 ""type"":""text"",
								 ""validator"":""size"",
								 ""info"": ""Width of Field Border. Example: 2px""
							},
							{
								 ""name"":""border-color"",
								 ""type"":""text"",
								 ""validator"":""color"",
								 ""info"": ""Color of Field Border. Example: #ff00ff""
							},
							{
								 ""name"":""border-style"",
								 ""type"":""text"",
								 ""validator"":""none"",
								 ""info"": ""Type of Field Border. Example: solid""
							}
						]                     
					},
					{ 
					""group"":""background"",
						""properties"": [
						{
							""name"":""background-color"",
							""type"":""text"",
							""validator"":""color"",
							""info"": ""Color of Field background. Example: #ff00ff""
						}
						]  
					}
				] ";

            }//endGet

        }
        #region static methods
        /// <summary>
		  /// Gets the icon that rappresent this Field.
		  /// </summary>
		  /// <value>The field icon.</value>
		  public static System.Drawing.Image Icon
		  {
              
			  get
			  {
				  try
				  {
                      return icons.checkbox;
				  }
                  catch (Exception e) { Console.WriteLine("Eccezione: " + e); return null; }
			  }
		  }

        #endregion

        #region InnerControl
        public class CheckBoxControl : CheckBox ,IFieldControl
        {
            CheckBoxField father;

            public CheckBoxControl(CheckBoxField field) : base(){
                father = field;
                this.ID = field.Name;
                if (father.text)
                    this.Checked = true;
                

            }

            //protected override void OnCheckedChanged(EventArgs e)
            //{
            //    father.text = !father.text;//(father.text != null && father.text.Equals("true")) ? "false" : "true";
            //    base.OnCheckedChanged(e);
            //}


           

            #region IFieldControl Membri di

            public List<string> FieldProperties
            {
                get 
                {
                    List<string> l = new List<string>();
                    l.Add("");
                    return l; 
                }
            }

            public XmlNode SetState(List<string> properties)
            {
                if (properties.Count != 1)
                    return null;
                switch (properties[0])
                {
                    case "on": { father.text = true; } break;
                    case "off": { father.text = false; } break;
                    default: { father.text = false; } break;
                }
                
                return father.Value;
            }

            public XmlNode SetState(System.Web.UI.StateBag state)
            {
                father.text = (bool)state[FieldProperties[0]];
                return father.Value;

            }

            public string GetResult()
            {
                return father.text.ToString();
            }

            #endregion
        }
        #endregion

        #region IRenderable Members


        public WebControl GetWebControl(System.Web.HttpServerUtility server, XmlNode renderingDocument)
        {
            throw new NotImplementedException();
        }

        public List<BaseValidator> GetValidators(string controlId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
