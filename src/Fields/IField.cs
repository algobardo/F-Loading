using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml;
using System.Web;
using System.Web.UI;

namespace Fields
{
	public interface IBaseType : IField
	{
		// -------------- tipically called by WFE --------------

		/// <summary>
		/// Sets the name of the subtype (ex: StingBox1)
		/// (if it's a fresh type, you have to set the name before to get the XSD)
		/// </summary>
		/// <value>The name of the type.</value>
		string TypeName { get; set; }

		/// <summary>
		/// XML schema of the field instance with the extended constraints
		/// </summary>
		/// <value>The field schema.</value>
		XmlSchemaComplexType TypeSchema { get; }

		/// <summary>
		/// Gets the unrelated control 
		/// (the WebControl without label, PlaceHolder, etc..)
		/// </summary>
		Control GetUnrelatedControl();


		// -------------- STATIC METHOD IMPLEMENTED INTO EACH FIELD --------------

		// get the WebControl to draw the icon
		// static WebControl Icon { get; }

		// get the WebControl to draw the preview
		// static WebControl Preview { get; }

		// get the WebControl to draw the preview tooltip
		// static WebControl PreviewTooltip { get; }
	}

	public interface IComplexType : IField
	{

	}



	public interface IRenderable
	{
		// -------------- tipically called by GUI --------------

		/// <summary>
		/// Returns the WebControl associated with the field
		/// (you should set the name before)
		/// </summary>
		/// <returns>The WebControl that you can render</returns>
		Control GetWebControl(HttpServerUtility server, XmlNode renderingDocument);


		/// <summary>
		/// Gets XML from this field
		/// </summary>
		/// <param name="ctrl">Control returned by GetWebControl invocation</param>
		/// <param name="renderingDocument">
		///		The rendering document for this field 
		///		(it can contain information about the value to return)
		/// </param>
		/// <returns>XMLNode generated</returns>
		string GetValue(Control ctrl, XmlNode renderingDocument);


		/// <summary>
		/// Gets the style properties in JSON format
		/// </summary>
		/// <value>The JSON style properties.</value>
		string JSON_StyleProperties { get; }

	}

	public interface IField : IRenderable
	{
		/// <summary>    
		/// Sets the name of the field (i.e. Label1)
		/// (usually you should set the name before to get WebControls and Validators,
		/// or to get the value)
		/// </summary>
		string Name { get; set; }

		// ---------------- USED BY WF --------------------------------------------

		/// <summary>
		/// Sets the value of this field
		/// </summary>
		/// <param name="nds">The list of XmlNode that represent the values</param>
		void SetValue(List<XmlNode> nds);


		/// <summary>
		/// Sets an example value for the field.
		/// </summary>
		void setExampleValue();

	}

}
