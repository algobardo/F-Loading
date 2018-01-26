using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;
using Fields;
using System.Diagnostics;

using System.IO;
using System.Xml.XPath;

namespace Core.WF
{
    /// <summary>
    /// Interpreter of the XMLNode preconditions and actions of edges.
    /// </summary>
    class EdgeLabelInterpreter
    {
        private static XmlSchemaSet generatedSchema = null;
        private static XmlSchema baseSchema = null;

        public static XmlSchemaSet GetSchema()
        {
            if (baseSchema != Fields.FieldsManager.FieldTypesXSD || generatedSchema == null)
            {
                baseSchema = Fields.FieldsManager.FieldTypesXSD;

                XmlSchemaSet s = new XmlSchemaSet();
                s.Add(baseSchema);

                XmlSchema sc = new XmlSchema();
                XmlSchema sc2 = new XmlSchema();

                XmlSchemaElement sc2elm = new XmlSchemaElement();
                sc2elm.Name = "TypesValue";
                sc2elm.SchemaTypeName = new XmlQualifiedName("Types");
                sc2.Items.Add(sc2elm);

                XmlSchemaComplexType sc2s = new XmlSchemaComplexType();
                sc2s.Name = "Types";
                sc2.Items.Add(sc2s);

                XmlSchemaChoice sc2ch = new XmlSchemaChoice();
                sc2s.Particle = sc2ch;

       
                foreach (var i in Fields.FieldsManager.FieldTypes)
                {
                    IBaseType fld = (IBaseType)Fields.FieldsManager.GetInstance(i);
                    fld.TypeName = i.Name + "0";
                    sc.Items.Add(fld.TypeSchema);
                }
                foreach (var i in Fields.FieldsManager.FieldTypes)
                {
                    XmlSchemaElement el = new XmlSchemaElement();
                    el.Name = i.Name;
                    el.SchemaTypeName = new XmlQualifiedName(i.Name + "0");                    
                    sc2ch.Items.Add(el);
                }
                s.Add(sc);
                s.Add(sc2);
                s.Compile();

                generatedSchema = s;

            }
            return generatedSchema;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actualState"> is an istance of a node eg <Node1>...</Node1> </param>
        /// <param name="preconditions"> is a xml representing preconditions on an edge eg <PRECONDITIONS>...</PRECONDITIONS></param>
        /// <returns></returns>
        public static bool InterpretPreconditions(XmlNode actualState, XmlElement preconditions)
        {
            if (preconditions.Name == "PRECONDITIONS" && preconditions.HasChildNodes)
            {
                ///returns everything included within the <PRECONDITIONS></PRECONDITIONS> tags
                XmlNode andOrNotList = preconditions.FirstChild;
                bool ret = recSemPreconditions(actualState, andOrNotList);
                return ret;
            }
            else if (preconditions.Name == "PRECONDITIONS" && !preconditions.HasChildNodes)
            {
                return true;
            }


            return false;
			 
        }    

        private static bool recSemPreconditions(XmlNode actualState, XmlNode preconditions)
        {
            if (preconditions.Name == "AND")
            {
                bool calcValue = true;
                foreach (XmlNode predicate in preconditions.ChildNodes)
                {
                    calcValue = calcValue && recSemPreconditions(actualState, predicate);
                    if (!calcValue) break;
                }
                return calcValue;
            }
            else if (preconditions.Name == "OR")
            {
                bool calcValue = false;
                foreach (XmlNode predicate in preconditions.ChildNodes)
                {
                    calcValue = calcValue || recSemPreconditions(actualState, predicate);
                    if (calcValue) break;
                }
                return calcValue;
            }
            else if (preconditions.Name == "NOT")
            {
                return !recSemPreconditions(actualState, preconditions.FirstChild);
            }          
            else
            {
               
                List<IField> arg = new List<IField>();

                foreach (XmlNode buff in preconditions.ChildNodes)
                {
                    if (buff.Attributes["type"].Value == "value")
                    {
                        XmlDocument xd = new XmlDocument();
                        XmlElement xdl = xd.CreateElement("TypesValue");
                        xd.AppendChild(xdl);

                        xdl.AppendChild(xd.ImportNode(buff.FirstChild,true));
                        xd.Schemas = GetSchema();
                        xd.Validate(null);
                        Type vtype = Fields.FieldsManager.GetType(xdl.FirstChild);                       
                        IField obj = Fields.FieldsManager.GetInstance(vtype);
                        List<XmlNode> l=new List<XmlNode>();
                        l.Add(xdl.FirstChild);
                        obj.SetValue(l);
                        arg.Add(obj);
                    }
                    else if (buff.Attributes["type"].Value == "path")
                    {
                        XmlNode nd = actualState.SelectNodes(buff.FirstChild.Value)[0];
                        Type vtype = Fields.FieldsManager.GetType(nd);
                        IField obj = Fields.FieldsManager.GetInstance(vtype);
                        List<XmlNode> l=new List<XmlNode>();
                        l.Add(nd);
                        obj.SetValue(l);
                        arg.Add(obj);
                    }
                    else if (buff.Attributes["type"].Value == "operation")
                    {
                        arg.Add(recSemOperations(actualState, (XmlElement)buff.FirstChild));

                    }
                }
                List<Type> tps = arg.ConvertAll(x => x.GetType());
                MethodInfo op = Fields.FieldsManager.GetPredicate(preconditions.Name, tps);
                                
                return (bool)op.Invoke(null, arg.ToArray());
            }

        }

        public static Fields.IField recSemOperations(XmlNode actualState, XmlElement operations)
        {
            List<IField> arg = new List<IField>();

            foreach (XmlNode buff in operations.ChildNodes)
            {
                if (buff.Attributes["type"].Value == "value")
                {
                    XmlDocument xd = new XmlDocument();
                    XmlElement xdl = xd.CreateElement("TypesValue");
                    xd.AppendChild(xdl);

                    xdl.AppendChild(xd.ImportNode(buff.FirstChild, true));
                    xd.Schemas = GetSchema();
                    xd.Validate(null);
                    Type vtype = Fields.FieldsManager.GetType(xdl.FirstChild);
                    IField obj = Fields.FieldsManager.GetInstance(vtype);
                    List<XmlNode> l=new List<XmlNode>();
                    l.Add(xdl.FirstChild);
                    obj.SetValue(l);
                    arg.Add(obj);
                }
                else if (buff.Attributes["type"].Value == "path")
                {
                    XmlNode nd = actualState.SelectNodes(buff.FirstChild.Value)[0];
                    Type vtype = Fields.FieldsManager.GetType(nd);
                    IField obj = Fields.FieldsManager.GetInstance(vtype);
                    List<XmlNode> l=new List<XmlNode>();
                    l.Add(nd);
                    obj.SetValue(l);
                    arg.Add(obj);
                }
                else if (buff.Attributes["type"].Value == "operation")
                {
                    arg.Add(recSemOperations(actualState, (XmlElement)buff.FirstChild));

                }
            }
            List<Type> tps = arg.ConvertAll(x => x.GetType());
            MethodInfo op = Fields.FieldsManager.GetOperation(operations.Name, tps);

            return (IField)op.Invoke(null, arg.ToArray());
        }
        
        /*This is similar the Interpreter but EAGER and without method calling*/
        public static bool VerifyPreconditionsCorrectness(XmlSchemaSet S, string ndTypeName, XmlNode preconditions)
        {
            return VPC(S, ndTypeName, preconditions);
        }

        public static bool VPC(XmlSchemaSet S, string ndTypeName, XmlNode preconditions)
        {

            XmlSchemaElement nodeElement = new XmlSchemaElement();
            nodeElement.Name = ndTypeName;
            nodeElement.SchemaTypeName = new XmlQualifiedName(ndTypeName);
            XmlSchema ns = new XmlSchema();
            ns.Items.Add(nodeElement);
            XmlSchemaSet newSchema = new XmlSchemaSet();
            newSchema.Add(S);
            newSchema.Add(ns);


            if (preconditions.Name == "PRECONDITIONS" && preconditions.HasChildNodes)
            {
                bool ret = false;
                try
                {
                    ret = recCheckPreconditions(newSchema, ndTypeName, preconditions.FirstChild);
                }
                catch (NullReferenceException)
                {
                }
                return ret;
            }
            else if (preconditions.Name == "PRECONDITIONS" && !preconditions.HasChildNodes)
            {
                return true;
            }


            return false;
        }

        public static bool recCheckPreconditions(XmlSchemaSet S, string ndTypeName, XmlNode preconditions)
        {
            bool calcValue = true;
            if (preconditions.Name == "AND" || preconditions.Name == "OR" || preconditions.Name == "NOT")
            {
                foreach (XmlNode predicate in preconditions.ChildNodes)
                {
                    calcValue = calcValue && recCheckPreconditions(S, ndTypeName, predicate);
                }
                return calcValue;
            }
            else
            {
                List<Type> tps = new List<Type>();

                foreach (XmlNode buff in preconditions.ChildNodes)
                {
                    if (buff.Attributes["type"].Value == "value")
                    {
                        XmlDocument xd = new XmlDocument();
                        XmlElement xdl = xd.CreateElement("TypesValue");
                        xd.AppendChild(xdl);

                        xdl.AppendChild(xd.ImportNode(buff.FirstChild, true));
                        xd.Schemas = GetSchema();
                        xd.Validate(null);
                        Type vtype = Fields.FieldsManager.GetType(xdl.FirstChild);
                        if (vtype == null)
                            return false;
                        tps.Add(vtype);
                        
                    }
                    else if (buff.Attributes["type"].Value == "path")
                    {

                        var btypes = FieldsManager.FieldTypes;
                        var rslt = btypes.Find(x => x.Name == buff.Attributes["fieldType"].Value);
                        if (rslt == null)
                            throw new ArgumentException("Declared XPath Type Not found");
                        tps.Add(rslt);
                        
                    }
                    else if (buff.Attributes["type"].Value == "operation")
                    {
                        tps.Add(recCheckOperations(S, ndTypeName, (XmlElement)buff.FirstChild));

                    }
                    else
                    {
                        return false;
                    }
                    
                }

                MethodInfo op = Fields.FieldsManager.GetPredicate(preconditions.Name, tps);
                if (op != null)
                    return true;
               

                return false;
            }

        }

        
        public static Type recCheckOperations(XmlSchemaSet S, string ndTypeName, XmlElement operations)

        {

            List<Type> tps = new List<Type>();

            foreach (XmlNode buff in operations.ChildNodes)
            {
                if (buff.Attributes["type"].Value == "value")
                {
                    XmlDocument xd = new XmlDocument();
                    XmlElement xdl = xd.CreateElement("TypesValue");
                    xd.AppendChild(xdl);

                    xdl.AppendChild(xd.ImportNode(buff.FirstChild, true));
                    xd.Schemas = GetSchema();
                    xd.Validate(null);
                    Type vtype = Fields.FieldsManager.GetType(xdl.FirstChild);
                    if (vtype == null)
                    {
                        //vtype = new Type();
                        //return false;
                    }  
                    tps.Add(vtype);
                
                }
                else if (buff.Attributes["type"].Value == "path")
                {

                    var btypes = FieldsManager.FieldTypes;
                    var rslt = btypes.Find(x => x.Name == buff.Attributes["fieldType"].Value);
                    if (rslt == null)
                        throw new ArgumentException("Declared XPath Type Not found");
                    tps.Add(rslt);
                    
                   
                }
                else if (buff.Attributes["type"].Value == "operation")
                {
                    tps.Add(recCheckOperations(S, ndTypeName, (XmlElement)buff.FirstChild));
                    
                }
            }
            MethodInfo op = Fields.FieldsManager.GetOperation(operations.Name, tps);
            
            return op.ReturnType;
            
        }

      
    }

     
}
