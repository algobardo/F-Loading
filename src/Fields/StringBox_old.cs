#define DEBUG

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;
using System.Web.UI;

namespace Fields
{
    public class StringBox : IBaseType
    {

        private XmlSchema baseSchema;

        private string text;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBox"/> class.
        /// </summary>
        public StringBox()
        {
            baseSchema = new XmlSchema();

            //<xs:complexType name="StringBoxN">
            XmlSchemaComplexType newType = new XmlSchemaComplexType();
            // the name must be filled later
            newType.Name = "";
            baseSchema.Items.Add(newType);

            //<xs:complexContent>
            XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
            newType.ContentModel = complexContent;

            //<xs:extension base="StringBox">
            XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
            complexContent.Content = complexContentExtension;
            complexContentExtension.BaseTypeName = new XmlQualifiedName("StringBox");

            //<xs:sequence>
            XmlSchemaSequence seq = new XmlSchemaSequence();
            complexContentExtension.Particle = seq;

            //<xs:element name="Value" type="xs:string"/>
            XmlSchemaElement elem = new XmlSchemaElement();
            seq.Items.Add(elem);
            elem.Name = "Value";
            elem.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBox"/> class from a XMLSchema
        /// </summary>
        /// <param name="schema">The schema xml that describe the StringBox state.</param>
        public StringBox(XmlSchema schema, string name)
        {
            // in this constructor it's useful to set the ID, cause GUI is calling it
            this.Name = name;

            baseSchema = new XmlSchema();

            //<xs:complexType name="StringBoxN">
            XmlSchemaComplexType newType = new XmlSchemaComplexType();
            newType.Name = ((XmlSchemaComplexType)schema.Items[0]).Name;
            baseSchema.Items.Add(newType);

            //<xs:complexContent>
            XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
            newType.ContentModel = complexContent;

            //<xs:extension base="StringBox">
            XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
            complexContent.Content = complexContentExtension;
            complexContentExtension.BaseTypeName = new XmlQualifiedName("StringBox");

            //<xs:sequence>
            XmlSchemaSequence seq = new XmlSchemaSequence();
            complexContentExtension.Particle = seq;

            //<xs:element name="Value" type="xs:string"/>
            XmlSchemaElement elem = new XmlSchemaElement();
            seq.Items.Add(elem);
            elem.Name = "Value";
            elem.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

            //Controlla se l'element potra' avere valore vuoto
            if (Common.getElementFromSchema(schema).IsNillable)
                elem.IsNillable = true;

            if (((XmlSchemaSimpleType)Common.getElementFromSchema(schema).SchemaType) == null)
                return;
            XmlSchemaObjectCollection constrColl =
                 ((XmlSchemaSimpleTypeRestriction)
                     ((XmlSchemaSimpleType)
                     Common.getElementFromSchema(schema)
                .SchemaType).Content).Facets;


            foreach (XmlSchemaFacet facet in constrColl)
            {
                Common.addFacet(facet, Common.getElementFromSchema(baseSchema));
            }

        }


        #region IField Members
        /* For methods documentation's see the IField interface */


        public WebControl GetWebControl()
        {
            return new StringBoxControl(this);
        }
        public void setExampleValue()
        {
            text = "";
        }
        public List<BaseValidator> GetValidators()
        {
            // reading the XSD and creating validators
            List<BaseValidator> validatorList = new List<BaseValidator>();
            if (!Common.getElementFromSchema(baseSchema).IsNillable)
            {
                RequiredFieldValidator rqfv = new RequiredFieldValidator();
                rqfv.ErrorMessage = "Required fields shouldn't be empty";
                rqfv.ControlToValidate = this.Name;
                validatorList.Add(rqfv);

            }
            if (((XmlSchemaSimpleType)Common.getElementFromSchema(baseSchema).SchemaType) == null)
                return validatorList;
            XmlSchemaObjectCollection constrColl =
              ((XmlSchemaSimpleTypeRestriction)((XmlSchemaSimpleType)Common.getElementFromSchema(baseSchema).SchemaType).Content).Facets;
            // Set validators from XSD
            foreach (XmlSchemaFacet facet in constrColl)
            {
                if (facet is XmlSchemaMaxLengthFacet)
                {
                    validatorList.Add(getMaxLengthValidator(Int32.Parse(facet.Value)));
                }
                else if (facet is XmlSchemaMinLengthFacet)
                {
                    validatorList.Add(getMinLengthValidator(Int32.Parse(facet.Value)));
                }
                else if (facet is XmlSchemaPatternFacet)
                {
                    validatorList.Add(getPatternValidator(facet.Value));
                }
            }

            return validatorList;
        }

        public System.Xml.XmlNode Value
        {
            set
            {
                this.text = value.FirstChild.InnerText.Trim();
            }

            get
            {
                //if (text == null) return null;
                string root = String.IsNullOrEmpty(Name) ? "null" : Name;

                XmlDocument doc = new XmlDocument();
                XmlNode nodeValue = doc.CreateElement(root);
                nodeValue.AppendChild(doc.CreateElement("Value"));
                nodeValue.FirstChild.InnerText = text == null ? "" : this.text;
                return nodeValue;
            }


        }

        /* private XmlSchemaSimpleType getSimpleType(XmlSchemaFacet faceset)
         {
             XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
             XmlSchemaSimpleTypeRestriction restriction = new XmlSchemaSimpleTypeRestriction();
             restriction.Facets.Add(faceset);
             restriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
             simpleType.Content = restriction;
             return simpleType;
         }*/

        /*  private XmlSchemaElement Common.getElementFromSchema()
          {
              //DO NOT REMOVE
              //XmlSchemaComplexType ext = ((XmlSchemaComplexType)baseSchema.Items[0]);
              //XmlSchemaComplexContent xcc = (XmlSchemaComplexContent)ext.ContentModel;
              //XmlSchemaComplexContentExtension xext = (XmlSchemaComplexContentExtension)xcc.Content;
              //XmlSchemaSequence seq = (XmlSchemaSequence)xext.Particle;      
              //XmlSchemaElement elem = (XmlSchemaElement)seq.Items[0];

              //it can be done better, maybe with XSL Transformation.
              //Gets sequence and element node.
              //For the sake of readability...
              XmlSchemaSequence seq =
                  (XmlSchemaSequence)
                      ((XmlSchemaComplexContentExtension)
                          ((XmlSchemaComplexContent)
                              (((XmlSchemaComplexType)
                  baseSchema.Items[0])).
                          ContentModel).
                              Content).
                                  Particle;

              return (XmlSchemaElement)seq.Items[0];
          }*/

        public string TypeName
        {
            set
            {
                //<xs:complexType name="StringBoxN">
                ((XmlSchemaComplexType)baseSchema.Items[0]).Name = value;
            }

            get
            {
                return ((XmlSchemaComplexType)baseSchema.Items[0]).Name;
            }
        }


        public string Name { set; get; }

        public System.Xml.Schema.XmlSchemaComplexType TypeSchema
        {
            get { return (XmlSchemaComplexType)baseSchema.Items[0]; }
        }


        /* 
        // it should be correct, but maybe useless..
        // warning: webcontrols and validators should be added to a page to make validation working
        public bool IsValid()
        {
              bool valid = true;
              foreach (BaseValidator val in validatorList)
              {
                   val.Validate();
                   valid = valid & val.IsValid;
              }
              return valid;
        }
        */

        /*
        private void printSchema()
        {
              XmlSchemaSet schemaSet = new XmlSchemaSet();
              schemaSet.ValidationEventHandler += new ValidationEventHandler(ValidationCallbackOne);

              XmlSchema compiledSchema = null;

              XmlSchema s = new XmlSchema();
              XmlSchemaComplexType c1 = new XmlSchemaComplexType();
              c1.Name = "StringBox";
              s.Items.Add(c1);
              schemaSet.Add(s);
              schemaSet.Add(baseSchema);
              schemaSet.Compile();

              foreach (XmlSchema schema1 in schemaSet.Schemas())
              {
                   compiledSchema = schema1;
              }

              //compiledSchema.Write(Console.Out, nsmgr);
        }
        */

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
                    /*return System.Drawing.Image.FromStream(
                       Assembly.GetExecutingAssembly().GetManifestResourceStream("Fields.stringbox.jpg")
                    );*/
                    return icons.stringbox;
                }
                catch (Exception) { return null; }
            }
        }

        #endregion


        #region Constraints
        /******************************************************
         ******************************************************
         *          StringBox - Constraints                   *
         ******************************************************
         ******************************************************/
        [Constraint("Field value may not be present")]
        public bool AddIsNotRequiredConstraint()
        {
            if (!Common.getElementFromSchema(baseSchema).IsNillable)
            {
                Common.getElementFromSchema(baseSchema).IsNillable = true;
                return true;
            }
            else

                return false;

        }

        [Constraint("Range Length", Description = "String length must be in the range (%0,%1)")]
        public bool AddRangeLengthConstraint(int minLen, int maxLen)
        {
            if (maxLen < 0 || minLen < 0 || minLen > maxLen)
                return false;
            XmlSchemaMaxLengthFacet maxLenFacet = new XmlSchemaMaxLengthFacet();
            maxLenFacet.Value = maxLen.ToString();
            Common.addFacet(maxLenFacet, Common.getElementFromSchema(baseSchema));
            XmlSchemaMinLengthFacet minLenFacet = new XmlSchemaMinLengthFacet();
            minLenFacet.Value = minLen.ToString();
            Common.addFacet(minLenFacet, Common.getElementFromSchema(baseSchema));
            return true;
        }

        /* The range constrint in the XSD is translated in a min and a max constraint
        private BaseValidator getRangeLengthValidator(int minLen, int maxLen)
        {
            RegularExpressionValidator regexpr = new RegularExpressionValidator();
            regexpr.ControlToValidate = this.fieldID;
            regexpr.ValidationExpression = ".{" + minLen + "," + maxLen + "}";
            regexpr.ErrorMessage = "String length not in the range " + minLen + "-" + maxLen;
            return regexpr;
        }
         */

        /// <summary>
        /// Adds a constraint which limits the maximum length of
        /// the string inside the StringBox,adding <code><xs:maxLength value=maxLen/></code>
        /// to the schema.
        /// </summary>
        /// <param name="maxLen">maximum string length, a non negative integer</param>
        /// <returns>true if the constraint is added succesfully, false otherwise.</returns>
        [Constraint("Max Length", Description = "String must not be longer than %0")]
        public bool AddMaxLengthConstraint(int maxLen)
        {
            if (maxLen < 0) return false;

            XmlSchemaMaxLengthFacet maxLenFacet = new XmlSchemaMaxLengthFacet();
            maxLenFacet.Value = maxLen.ToString();
            Common.addFacet(maxLenFacet, Common.getElementFromSchema(baseSchema));

            return true;
        }

        private BaseValidator getMaxLengthValidator(int maxLen)
        {
            RegularExpressionValidator regexpr = new RegularExpressionValidator();
            regexpr.ControlToValidate = this.Name;
            regexpr.ValidationExpression = ".{0," + maxLen + "}";
            regexpr.ErrorMessage = "String longher than " + maxLen;
            return regexpr;
        }

        /// <summary>
        /// Adds a constraint which limits the minimum length of
        /// the string inside the StringBox, adding <code><xs:minLength value=minLen/></code>
        /// to the schema.
        /// </summary>
        /// <param name="maxLen">minimum string length, a non negative integer.</param>
        /// <returns>true if the constraint is added succesfully, false otherwise.</returns>
        [Constraint("Min Length", Description = "String must not be shorter than %0")]
        public bool AddMinLengthConstraint(int minLen)
        {
            if (minLen < 0) return false;
            XmlSchemaMinLengthFacet minLenFacet = new XmlSchemaMinLengthFacet();
            minLenFacet.Value = minLen.ToString();
            Common.addFacet(minLenFacet, Common.getElementFromSchema(baseSchema));

            return true;
        }

        private BaseValidator getMinLengthValidator(int minLen)
        {
            RegularExpressionValidator regexpr = new RegularExpressionValidator();
            regexpr.ControlToValidate = this.Name;
            regexpr.ValidationExpression = ".{" + minLen + ",}";
            regexpr.ErrorMessage = "String shorter than " + minLen;

            return regexpr;
        }

        /// <summary>
        /// Adds a pattern restriction to the stringBox Schema.
        /// Assuming that the pattern parameter is well formed it adds 
        /// <code><xs:pattern value="pattern"/></code>
        /// to the schema.
        /// </summary>
        /// <param name="pattern">The regular expression to use as restriction.</param>
        /// <returns>true if the costraint is added succesfully, false otherwise.</returns> 
        [Constraint("Reg Exp", Description = "String must adhere to regular expression %0")]
        public bool AddPatternConstraint(String pattern)
        {
            //TODO: check the pattern correctness. is there a nice way?
            if (pattern == null || pattern.Equals(""))
                return false;

            XmlSchemaPatternFacet schemaPattern = new XmlSchemaPatternFacet();
            schemaPattern.Value = pattern;
            Common.addFacet(schemaPattern, Common.getElementFromSchema(baseSchema));

            return true;
        }

        public BaseValidator getPatternValidator(String pattern)
        {
            RegularExpressionValidator rev = new RegularExpressionValidator();
            rev.ControlToValidate = this.Name;
            rev.ValidationExpression = pattern;
            return rev;
        }

        //TODO: COMMENT ME :)
        /* public void addFacet(XmlSchemaFacet facet, XmlSchemaElement elem)
         {
             if (elem.SchemaType == null)
             {
                 //create <xs:SimpleType> to contain restrictions if doesn't exist
                 XmlSchemaSimpleType simpleType = getSimpleType(facet);
                 elem.SchemaTypeName = null;
                 elem.SchemaType = simpleType;
             }
             else
             {
                 //add patternRestriction
                 ((XmlSchemaSimpleTypeRestriction)((XmlSchemaSimpleType)elem.SchemaType).Content).Facets.Add(facet);
             }
         }
         */
        #endregion

        #region Operations
        /******************************************************
		******************************************************
		*          StringBox - Operations                    *
		******************************************************
		******************************************************/

        /**
        * Notes :
        * All Operations results are new instances of the return types
        */

        [Operation("Lower Case")]
        public static StringBox ToLowerCase(StringBox source)
        {
            StringBox result = new StringBox();
            result.text = source.text.ToLower();
            return result;
        }

        [Operation("Upper Case")]
        public static StringBox ToUpperCase(StringBox source)
        {
            StringBox result = new StringBox();
            result.text = source.text.ToUpper();
            return result;
        }


        [Operation("Length", Description = "Length of this string...")]
        public static IntBox Length(StringBox source)
        {
            IntBox result = new IntBox();
            XmlDocument doc = new XmlDocument();

            XmlNode nodeValue = doc.CreateElement("null");
            nodeValue.AppendChild(doc.CreateElement("Value"));
            nodeValue.FirstChild.InnerText = source.text.Length.ToString();

            result.Value = nodeValue;

            return result;
        }


        [Operation("String Concatenation")]
        public static StringBox Concat(StringBox source1, StringBox source2)
        {
            StringBox result = new StringBox();
            result.text = source1.text + source2.text;
            return result;
        }

        #endregion

        #region Predicates

        [Predicate("Equals", Description = "Lexical Comparation between two StringBox")]
        public static bool Equals(StringBox sBox1, StringBox sBox2)
        {
            return sBox1.text.Equals(sBox2.text);
        }

        [Predicate("RegEx match", Description = "Return if the first StringBox value match with regular expression in second parameter")]
        public static bool RegexMatch(StringBox s1, StringBox pattern)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(s1.text, pattern.text);
        }

        [Predicate("IsEmpty", Description = "Checks whether user has put content inside the field")]
        public static bool IsEmpty(StringBox s)
        {
            return s.text == null;
        }
        #endregion


        /// <summary>
        /// StringBox WebControl...................
        /// </summary>
        public class StringBoxControl : TextBox, IFieldControl
        {
            StringBox father;


            public StringBoxControl(StringBox field)
                : base()
            {
                father = field;
                this.ID = father.Name;
                if (father.text != null)
                    this.Text = father.text;
            }

            /*
            public static void ValidationCallbackOne(object sender, ValidationEventArgs args)
            {
                Console.WriteLine(args.Message);
            }
            */

            //protected override void OnTextChanged(EventArgs e)
            //{
            //    father.text = this.Text;
            //    base.OnTextChanged(e);
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

                father.text = properties[0];
                return father.Value;
            }

            public XmlNode SetState(StateBag state)
            {
                father.text = (string)state[FieldProperties[0]];
                return father.Value;
            }

            public string GetResult()
            {
                return father.text;
            }

            #endregion
        }




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
