using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Drawing;
using Mobile.Language;
using Mobile.Util;

namespace Mobile.Fields
{
    public class FieldAttribute : Attribute
    {
        public String TypeName
        {
            get;
            private set;
        }

        public FieldAttribute(String typeName)
            : base()
        {
            this.TypeName = typeName;
        }
    }

    public abstract class Field
    {
        /// <summary>
        /// The <see cref="XmlSchemaSet"/> that represents the Field.
        /// </summary>
        public XmlSchemaSet Schema
        {
            get;
            protected set;
        }

        /// <summary>
        /// The instance of the type that represents the Field.
        /// </summary>
        public String TypeName 
        { 
            get; 
            protected set; 
        }

        /// <summary>
        /// The name that identifies the Field.
        /// </summary>
        public String FieldName 
        { 
            get; 
            protected set;
        }

        /// <summary>
        /// The label associated to the Field and displayed on the screen.
        /// </summary>
        public String Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// The description associated to the Field and displayed on the screen.
        /// </summary>
        public String Description
        {
            get;
            protected set;
        }


        /// <summary>
        /// Converts the current object into an <see cref="XmlElement"/> object.
        /// </summary>
        /// <returns>an <see cref="XmlElement"/> rapresenting the current object.</returns>
        public abstract XmlElement ToXml();


        /// <summary>
        /// Restore the state of the element.
        /// </summary>
        /// <param name="element">The element to be restored.</param>
        public abstract void FromXml(XmlElement element);


        /// <summary>
        /// Parse the state of the object from a <see cref="String"/>
        /// </summary>
        /// <param name="value">The <see cref="String"/> containing the value</param>    
        public abstract void FromString(String value);


        /// <summary>
        /// Checks if the Field respects the constraints defined 
        /// into its own XML Schema catching possibile exceptions. 
        /// In this latter case, it highlights the elements that they 
        /// are not filled correctly.
        /// </summary>
        public virtual void CheckConstraints()
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.AppendChild(document.ImportNode(this.ToXml(), true));
                document.Schemas.Add(Schema);
                document.Validate(OnSchemaValidation);
            }
            catch (Exception ex)
            {
                throw new FieldException(SeverityLevel.Critical, LogLevel.Debug, ex.InnerException, ExceptionManager.ContentFormat);
            }
        }


        /// <summary>
        /// Recuperates the rendering informations to display the Field
        /// </summary>
        /// <param name="presentation">The element to be parsed</param>
        public virtual void ParsePresentation(XmlElement presentation)
        {
            if (presentation != null)
            {
                XmlAttribute label = presentation.Attributes["renderedLabel"];
                if (label != null)
                    Name = HttpUtility.HtmlDecode(label.Value);

                XmlAttribute description = presentation.Attributes["description"];
                if (description != null)
                    Description = HttpUtility.HtmlDecode(description.Value);
            }
        }


        /// <summary>
        /// Reset the state of the Field to initial state.
        /// </summary>
        public abstract void Clear();


        #region Private Methods

        private void OnSchemaValidation(object source, ValidationEventArgs args)
        {
            switch (args.Severity)
            {
                case XmlSeverityType.Error:
                    System.Diagnostics.Debug.WriteLine(string.Format("Error: {0}", args.Message));
                    throw new FieldException(SeverityLevel.Critical, LogLevel.Debug, args.Exception, "Validation error.");
                case XmlSeverityType.Warning:
                    System.Diagnostics.Debug.WriteLine(string.Format("Warning {0}", args.Message));
                    throw new FieldException(SeverityLevel.Information, LogLevel.Debug, args.Exception, "Validation warning.");
            }
        }

        #endregion
    }
}
