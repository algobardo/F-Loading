using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Schema;
using System.Xml;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace Fields
{
	/// <summary>
	/// FieldsManager
	/// </summary>
	public static class FieldsManager
	{

		#region Variables

		/*
         * TOFIX: problems loading assemblies from specific locations (f.i. 'plugins' directory, not just from GAC or /bin). 
         * Problem description: Inability to acccess WebResources from DLLs located in custom locations (URL returned by 
         * Page.ClientScript.GetWebResourceUrl(Type, Addr) gives FileNotFoundException).
         */

		/// <summary>
		/// Relative path to the plugins directory
		/// </summary>
		private static string PLUGINS_DIR = Path.GetDirectoryName(typeof(FieldsManager).Assembly.Location) + "/plugins";

		/// <summary>
		/// Contains the associations between a type and the constraints defined on that type.
		/// constraints[T] returns all constraints defined in T class
		/// </summary>
		private static Dictionary<Type, List<MethodInfo>> constraints;


		/// <summary>
		/// Contains the associations between a type T and all predicates that have a parameter with type T 
		/// predicates[T] returns all predicate methods that have at least a parameter with type T
		/// </summary>
		private static Dictionary<Type, List<MethodInfo>> predicates;

		/// <summary>
		/// Contains the associations between a type T and all operations that have a parameter with type T 
		/// operations[T] returns all operation methods that have at least a parameter with type T
		/// </summary>
		private static Dictionary<Type, List<MethodInfo>> operations;

		/// <summary>
		/// Contains the associations between a type T and all properties defined on its
		/// properties[T] returns all properties of the type T
		/// </summary>
		private static Dictionary<Type, List<PropertyAttribute>> properties;

		#endregion

		static FieldsManager()
		{
			LoadTypes();
		}

		#region Properties

		/// <summary>
		/// List of available field types
		/// </summary>
		private static Dictionary<string, Type> _fieldTypes;

		/// <summary>
		/// Gets the list of available field types.
		/// </summary>
		/// <value>The field types.</value>
		public static List<Type> FieldTypes
		{
			get
			{
				if (_fieldTypes == null)
					LoadTypes();
				return new List<Type>(_fieldTypes.Values);
			}
		}

		/// <summary>
		/// XSD Schema of field types
		/// </summary>
		private static XmlSchema _fieldTypesXSD;

		/// <summary>
		/// Gets the XSD with the description of available field types.
		/// </summary>
		/// <value>The XSD description of the field types.</value>
		public static XmlSchema FieldTypesXSD
		{
			get
			{
				if (_fieldTypesXSD == null)
					LoadTypes();
				return _fieldTypesXSD;
			}
		}

		/// <summary>
		/// Gets the list of available special field types. 
		/// Used by the WFE for some special purpose, f.e. to automatically get icons.
		/// (f.i. NamedSequence, NamedChoice, StaticImage, StaticText, etc.)
		/// </summary>
		/// <value>The special field types.</value>
		public static List<Type> SpecialFieldTypes
		{
			get
			{
				List<Type> sfts = new List<Type>();
				sfts.Add(typeof(StaticText));
				sfts.Add(typeof(StaticImage));
				sfts.Add(typeof(StaticHtmlCode));
				sfts.Add(typeof(Sequence));
				sfts.Add(typeof(Choice));
				return sfts;
			}
		}

		#endregion



		/// <summary>
		/// Load all types (fields) from the plugins directory.
		/// </summary>
		public static void LoadTypes()
		{
			_fieldTypes = new Dictionary<string, Type>();
			_fieldTypesXSD = new XmlSchema();
			constraints = new Dictionary<Type, List<MethodInfo>>();
			predicates = new Dictionary<Type, List<MethodInfo>>();
			operations = new Dictionary<Type, List<MethodInfo>>();
			properties = new Dictionary<Type, List<PropertyAttribute>>();

			List<string> dlls = new List<string>();

			// adding this dll file (builtin fields)
			if (!PLUGINS_DIR.Equals(Path.GetDirectoryName(typeof(FieldsManager).Assembly.Location)))
				dlls.Add(typeof(FieldsManager).Assembly.Location);

			// adding plugin files
			if (Directory.Exists(PLUGINS_DIR))
				dlls.AddRange(Directory.GetFiles(PLUGINS_DIR, "*.dll"));

			foreach (string dll in dlls)
			{
				//adds an entry inside the list of namespaces 
				Assembly asm = Assembly.LoadFrom(dll);
				foreach (Type fieldType in asm.GetTypes())
				{

					if (fieldType.GetInterface("IBaseType") == null)
					{  /* Invalid Plugin */
						continue;
					}

					// Inserting the new type in the types list
					_fieldTypes.Add(fieldType.Name, fieldType);

					// Updating the types XML Schema with the new type
					XmlSchemaComplexType newBaseType = new XmlSchemaComplexType();
					newBaseType.Name = fieldType.Name;
					_fieldTypesXSD.Items.Add(newBaseType);

					List<MethodInfo> constraintsList = new List<MethodInfo>();
					constraints.Add(fieldType, constraintsList);

					foreach (MethodInfo method in fieldType.GetMethods())
					{
						if (!method.IsPublic) continue;

						foreach (object attr in method.GetCustomAttributes(false))
						{
							// Constraints
							if (attr is ConstraintAttribute && method.ReturnType == typeof(bool))
							{
								//TO CHECK: all parameters must be string
								constraintsList.Add(method);
								break;
							}

							// Predicates + Operations (parametric way)
							if ((attr is PredicateAttribute || attr is OperationAttribute)
								 && method.IsStatic
								 && method.GetParameters().Length > 0
								 && method.GetParameters()[0].ParameterType.Equals(fieldType))
							{
								/* TO CHECK:
								 * Predicates can return only bool value
								 * Predicates can have only IBaseTypes parameters
								 * Operations can have and return only IBaseTypes
								 */

								Dictionary<Type, List<MethodInfo>> dictRef = (attr is PredicateAttribute ? predicates : operations);

								Dictionary<Type, bool> foundParameters = new Dictionary<Type, bool>();
								/*foreach (ParameterInfo parameter in method.GetParameters())
								{
                               
									if (foundParameters.ContainsKey(parameter.ParameterType))
									{
										// Type already met: SKIP
										continue;
									}
                                */
                                //ADDED PART
                                if(method.GetParameters().Length>0){
                                    ParameterInfo parameter = method.GetParameters()[0];
                                //ADDED PART
									List<MethodInfo> methodsList;
									if (dictRef.ContainsKey(parameter.ParameterType))
									{
										methodsList = dictRef[parameter.ParameterType];
									}
									else
									{
										methodsList = new List<MethodInfo>();
										dictRef.Add(parameter.ParameterType, methodsList);
									}
									methodsList.Add(method);
									foundParameters.Add(parameter.ParameterType, true);
								}
								break;
							}

						}
					}

					// reading Properties
					List<PropertyAttribute> propertiesList = new List<PropertyAttribute>();
					properties.Add(fieldType, propertiesList);
					foreach (object attr in fieldType.GetCustomAttributes(false))
						if (attr is PropertyAttribute)
							propertiesList.Add((PropertyAttribute)attr);
				}
			}
		}

		/// <summary>
		/// Returns the list of constraints description defined on a given field type
		/// </summary>
		/// <param name="t">The field type.</param>
		/// <returns>The list of constraints description or null</returns> 
		public static List<MethodInfo> GetConstraints(Type t)
		{
			if (constraints.ContainsKey(t))
				return constraints[t];
			else
				return null;
		}

		/// <summary>
		/// Returns the list of properties description defined on a given field type
		/// </summary>
		/// <param name="t">The field type.</param>
		/// <returns>The list of properties description or null</returns> 
		public static List<PropertyAttribute> GetProperties(Type t)
		{
			if (properties.ContainsKey(t))
				return properties[t];
			else
				return null;
		}

		/// <summary>
		/// Returns the description of an operation
		/// </summary>
		/// <param name="operationName">Name of the operation.</param>
		/// <param name="paramTypes">The paramaters.</param>
		/// <returns>The MethodInfo that describe the operation or null if method not found</returns>
		public static MethodInfo GetOperation(String operationName, List<Type> paramTypes)
		{
			if (operationName == null || paramTypes == null || paramTypes.Count < 1
					  || operations.ContainsKey(paramTypes[0]) == false
				 )
				return null;

			foreach (MethodInfo op in operations[paramTypes[0]])
			{
				if (operationName.Equals(op.Name) && op.GetParameters().Length == paramTypes.Count)
				{
					// copy each parameter Type to a List
					List<Type> opParamType = new List<Type>();
					foreach (ParameterInfo param in op.GetParameters())
						opParamType.Add(param.ParameterType);
					//if (paramTypes.Equals(opParamType))
					//    return op;
					bool eq = true;
					for (int i = 0; i < opParamType.Count; i++)
					{
						if (!opParamType[i].Equals(paramTypes[i]))
							eq = false;
					}
					if (eq)
						return op;
				}
			}
			// Operation not found
			return null;
		}

		/// <summary>
		/// Returns the list of operations with the given type as a parameter
		/// </summary>
		/// <param name="t">
		/// The <see cref="Type"/> of which the operations description has to be returned
		/// </param>
		/// <returns>
		/// A <see cref="List"/> with the operations descriptions
		/// </returns>
		public static List<MethodInfo> GetOperations(Type t)
		{
			if (operations.ContainsKey(t))
				return operations[t];
			else
				return new List<MethodInfo>(0);
		}

		/// <summary>
		/// Returns the description of a predicate identified by a field type, 
		/// a predicate name and a set of param types.
		/// </summary>
		/// <param name="fieldType">Type of the field.</param>
		/// <param name="predicateName">Name of the predicate.</param>
		/// <param name="paramTypes">The param types.</param>
		/// <returns>The predicate description or null if not found</returns>
		public static MethodInfo GetPredicate(String predicateName, List<Type> paramTypes)
		{
			if (predicateName == null || paramTypes == null || paramTypes.Count < 1
					  || predicates.ContainsKey(paramTypes[0]) == false
				 )
				return null;

			foreach (MethodInfo pred in predicates[paramTypes[0]])
			{
				if (predicateName.Equals(pred.Name) && pred.GetParameters().Length == paramTypes.Count)
				{
					// copy each parameter Type to a List
					List<Type> predParamType = new List<Type>();
					foreach (ParameterInfo param in pred.GetParameters())
						predParamType.Add(param.ParameterType);

					//if (paramTypes.Equals(predParamType))
					//    return pred;
					bool eq = true;
					for (int i = 0; i < predParamType.Count; i++)
					{
						if (!predParamType[i].Equals(paramTypes[i]))
							eq = false;
					}
					if (eq)
						return pred;
				}
			}
			// Predicate not found
			return null;
		}


		/// <summary>
		/// Returns the List of predicates with the given type as a parameter
		/// </summary>
		/// <param name="t">
		/// The <see cref="Type"/> of which the predicates has to be returned
		/// </param>
		/// <returns>
		/// A <see cref="List"/> with the predicates on the type t
		/// </returns>
		public static List<MethodInfo> GetPredicates(Type t)
		{
			if (predicates.ContainsKey(t))
				return predicates[t];
			else
				return new List<MethodInfo>(0);
		}


		/// <summary>
		/// Returns the field type of the given xml node
		/// </summary>
		/// <param name="node">The XML node.</param>
		/// <returns>The field type</returns>
		/// <exception cref="ArgumentException">Invalid Node or XMLSchema missing</exception>
		public static Type GetType(XmlNode node)
		{
			Type type = null;
			try
			{
				XmlSchemaType schematype = node.SchemaInfo.SchemaType;

				type = _fieldTypes[schematype.BaseXmlSchemaType.Name];
			}
			catch (Exception e)
			{
				throw new ArgumentException("FieldsManager.getType:: received an invalid Xml node\n" + e);
			}

			return type;

			// or it should be...
			// return Type.GetType(((XmlSchemaComplexContentExtension)((XmlSchemaComplexType)node.SchemaInfo.SchemaElement.ElementSchemaType).ContentModel.Content).BaseTypeName.Name); 
		}



		/// <summary>
		/// Creates an istance of the given field type
		/// </summary>
		/// <param name="t">
		/// The <see cref="Type"/> of the instance that has to be created
		/// </param>
		/// <returns>
		/// A Field of the specified Type or null if an error occured
		/// </returns>
		public static IField GetInstance(Type field)
		{
			if (field == null || _fieldTypes.ContainsValue(field) == false)
				return null;

			try
			{
				return (IField)Activator.CreateInstance(field);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static IRenderable GetInstance(XmlNode renderingNode, XmlSchemaObject relatedSchemaObject, XmlSchemaSet releatedSchemaSet)
		{
			if (renderingNode.Name.StartsWith("xs_"))
			{
				if (relatedSchemaObject is XmlSchemaElement
					 && !((XmlSchemaElement)relatedSchemaObject).SchemaTypeName.IsEmpty
					 && !(((XmlSchemaElement)relatedSchemaObject).MaxOccurs > 1))
					return GetInstance(releatedSchemaSet, relatedSchemaObject);
				else if (relatedSchemaObject is XmlSchemaElement)
				{
					return new ComplexElement(releatedSchemaSet, (XmlSchemaElement)relatedSchemaObject);
				}
				else if (relatedSchemaObject is XmlSchemaSequence)
				{
					return new Sequence(releatedSchemaSet, (XmlSchemaSequence)relatedSchemaObject);
				}
				else if (relatedSchemaObject is XmlSchemaChoice)
				{
					return new Choice(releatedSchemaSet, (XmlSchemaChoice)relatedSchemaObject);
				}
				else if (relatedSchemaObject is XmlSchemaComplexType)
				{
					return new ComplexType(releatedSchemaSet, (XmlSchemaComplexType)relatedSchemaObject);
				}
			}
			// SPECIAL FIELDS 
			else if (renderingNode.Name.Equals("TEXT"))
			{
				return new StaticText();
			}
			else if (renderingNode.Name.Equals("IMAGE"))
			{
				return new StaticImage();
			}
			else if (renderingNode.Name.Equals("HTMLCODE"))
			{
				return new StaticHtmlCode();
			}

			return null;
		}

		/// <summary>
		/// Creates a field instance from his XML schema representation, taking the field structure from a schema and the constraints from another.
		/// </summary>
		/// <param name="schema">
		/// A <see cref="XmlSchemaSet"/> which contains the whole form to render
		/// </param>
		/// <param name="elm">
		/// A <see cref="XmlSchemaElement"/> which contains the definition of the field to instantiate
		/// </param>
		/// <returns>
		/// A <see cref="Field"/> built following the schemas above
		/// </returns>
		public static IField GetInstance(XmlSchemaSet schema, XmlSchemaObject scobj)
		{
			if (scobj is XmlSchemaElement && !((XmlSchemaElement)scobj).SchemaTypeName.IsEmpty)
			{
				XmlSchemaElement elm = (XmlSchemaElement)scobj;
				try
				{
					foreach (XmlSchema xsd in schema.Schemas())
					{
						string searchname = elm.SchemaTypeName.ToString();
						//controlla se nello schema è presente l'elemento cercato
						if (xsd.SchemaTypes.Contains(new XmlQualifiedName(searchname)))
						{
							foreach (XmlSchemaObject obj in xsd.Items)
							{
								if (obj is XmlSchemaComplexType && ((XmlSchemaComplexType)obj).Name.ToString() == searchname)
								{
									XmlSchema fieldschema = new XmlSchema();
									fieldschema.Items.Add(obj);
									XmlSchemaComplexContent xcc = (XmlSchemaComplexContent)((XmlSchemaComplexType)obj).ContentModel;
									XmlSchemaComplexContentExtension xext = (XmlSchemaComplexContentExtension)xcc.Content;

									Type tipo = Type.GetType("Fields." + xext.BaseTypeName.ToString(), true);
									Type[] par = { fieldschema.GetType(), typeof(string) };
									//elm.Name prende il nome dell'elemento contenuto in elm
									Object[] vals = { fieldschema, elm.Name };
									return (IField)tipo.GetConstructor(par).Invoke(vals);
								}

							}
						}
					}
				}
				catch (Exception)
				{
					return null;
				}
				return null;
			}
			return null;
		}


		/// <summary>
		/// Gets the icon of the specified field.
		/// </summary>
		/// <param name="field">The field type.</param>
		/// <returns>The field icon, or the default icon if not found</returns>
		public static WebControl GetIconControl(Type field)
		{
			WebControl icon;
			try
			{
				icon = (WebControl)field.GetProperty("Icon").GetGetMethod().Invoke(null, null);
				if (icon == null) throw new ArgumentNullException("Image null");
			}
			catch (Exception)
			{
				Console.WriteLine("Failure while retrieving the " + field.Name + "field preview");
				icon = new CustomImage("Fields.images.default_icon.png");
			}
			return icon;
		}

		/// <summary>
		/// Gets the preview of the specified field.
		/// </summary>
		/// <param name="field">The field type.</param>
		/// <returns>The field preview, or the default preview image if not found</returns>
		public static WebControl GetPreviewControl(Type field)
		{
			WebControl preview;
			try
			{
				preview = (WebControl)field.GetProperty("Preview").GetGetMethod().Invoke(null, null);
				if (preview == null) throw new ArgumentNullException("Image null");
			}
			catch (Exception)
			{
				Console.WriteLine("Failure while retrieving the " + field.Name + "field preview");
				preview = new CustomImage("Fields.images.default_preview.png");
			}
			return preview;
		}

		/// <summary>
		/// Gets the real preview of the specified field, to use as a tooltip.
		/// </summary>
		/// <param name="field">The field type.</param>
		/// <returns>The field preview, or the default preview image if not found</returns>
		public static WebControl GetPreviewTooltipControl(Type field)
		{
			WebControl preview;
			try
			{
				preview = (WebControl)field.GetProperty("PreviewTooltip").GetGetMethod().Invoke(null, null);
				if (preview == null) throw new ArgumentNullException("Image null");
			}
			catch (Exception)
			{
				Console.WriteLine("Failure while retrieving the " + field.Name + "field preview");
				preview = new CustomImage("Fields.images.default_preview_tooltip.png");
			}
			return preview;
		}

        public static WebControl GetUnrelatedControl(Type fieldtype)

        {
            if (FieldTypes.Contains(fieldtype))
            {

                return (WebControl)fieldtype.GetMethod("GetUnrelatedControl").Invoke(GetInstance(fieldtype), null);

            }
            else
                return null;
        }


		/*
		[Obsolete]
		public static string GetJSON_StyleProperties(Type field)
		{
			string style;
			try
			{
				style = (string)field.GetProperty("JSON_StyleProperties").GetGetMethod().Invoke(null, null);
			}
			catch (Exception)
			{
				Console.WriteLine("Failure while retrieving the " + field.Name + " style properties.");
				style = "";
			}
			return style;
		}
		*/
	}

}
