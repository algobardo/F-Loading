using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Runtime.Serialization;

namespace Core.WF
{
    public static class Utils
    {
        /*Standard code for cloning*/
        ///*Detach events*/
        //    object[] saved = Utils.DetachEvents(this);
           
        //    /*Clone*/
        //    Workflow w = new Workflow();
        //    Utils.DeepCloneObject(this, w);
            
        //    /*Registering the new workflow to his nodes*/
        //    foreach(WFnode nd in w.connectionGraph.Nodes){
        //        nd.FieldModified += w.nd_FieldModified;
        //        nd.NodeNameModified += w.nd_NodeNameModified;
        //    }
        //    foreach (WFedgeLabel ed in w.connectionGraph.Edges)
        //    {
        //        ed.EdgeModified += w.ed_EdgeModified;
        //    }

        //    /*Reattach events*/
        //    Utils.ReattachEvents(this, saved);

        public static void Schema2RenderSync(XmlDocument renderDoc, XmlSchemaSet Schema, string nodeName)
        {

        }
        public static void DeepCloneObject(object source,object target)
        {
            Type sourceType = source.GetType();

            FieldInfo[] fields = sourceType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (FieldInfo fi in fields)
            {
                if (fi.FieldType.GetInterface(typeof(ICloneable).Name)!=null)
                {
                    object value = fi.GetValue(source);
                    if (value == null)
                        fi.SetValue(target, null);
                    else
                        fi.SetValue(target,((ICloneable)value).Clone());
                }
                else
                    fi.SetValue(target, fi.GetValue(source));
            }

        }
        
        public static object[] DetachEvents(object obj)
        {
            List<Delegate> evs = new List<Delegate>();
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo fi in fields)
            {
                if (fi.FieldType.IsSubclassOf(typeof(Delegate)))
                {
                    Delegate evh = (Delegate)fi.GetValue(obj);
                    if (evh != null)
                        evs.Add((Delegate)evh.Clone());
                    else
                        evs.Add(null);
                    Delegate.RemoveAll(evh, evh);                 
                    
                }             
            }
            return evs.ToArray();
        }
        public static void ReattachEvents(object obj,object[] savedEvents)
        {            
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            int i = 0;
            foreach (FieldInfo fi in fields)
            {
                if (fi.FieldType.IsSubclassOf(typeof(Delegate)))
                {
                    Delegate evh = (Delegate)savedEvents[i];
                    fi.SetValue(obj, evh);
                }
                i++;
            }        

        }
        public static string WriteSchema(XmlSchema s)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            s.Write(sw);
            return sb.ToString();
        }
        public static string[] WriteSchemaSet(XmlSchemaSet s)
        {
            List<string> scs = new List<string>();

            foreach (XmlSchema sc in s.Schemas())
            {
                scs.Add(WriteSchema(sc));
            }

            return scs.ToArray();
        }

        public static XmlSchema ReadSchema(string s)
        {            
            StringReader sr = new StringReader(s);
            return XmlSchema.Read(sr, null);
        }
        public static XmlSchemaSet ReadSchemaSet(string[] s)
        {
            XmlSchemaSet scst = new XmlSchemaSet();
            foreach (string st in s)
                scst.Add(ReadSchema(st));
            return scst;
        }

        public static void SerializeGeneric(object instance, SerializationInfo info, StreamingContext context)
        {
            FieldInfo[] fields = instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo fi in fields)
            {
                if (fi.DeclaringType.Equals(instance.GetType()))
                {
                    if (typeof(XmlSchema).Equals((fi.FieldType)))
                    {
                        if (((XmlSchema)fi.GetValue(instance)) != null)
                            info.AddValue(fi.Name, WriteSchema((XmlSchema)fi.GetValue(instance)));
                        else
                            info.AddValue(fi.Name, null);
                    }
                    else if (typeof(XmlDocument).Equals(fi.FieldType))
                    {
                        if (((XmlNode)fi.GetValue(instance)) != null)
                            info.AddValue(fi.Name, ((XmlNode)fi.GetValue(instance)).OuterXml);
                        else
                            info.AddValue(fi.Name, null);
                    }                    
                    else if (typeof(XmlSchemaSet).Equals(fi.FieldType))
                    {
                        XmlSchemaSet schemas = (XmlSchemaSet)fi.GetValue(instance);
                        if (schemas != null)
                        {
                            string[] schemasStr = WriteSchemaSet(schemas);
                            info.AddValue(fi.Name, schemasStr);
                        }
                        else
                            info.AddValue(fi.Name, null);
                    }
                    else
                        info.AddValue(fi.Name, fi.GetValue(instance));
                }
            }
        }
        public static void DeSerializeGeneric(object instance, SerializationInfo info, StreamingContext context)
        {
            FieldInfo[] fields = instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo fi in fields)
            {
                if (fi.DeclaringType.Equals(instance.GetType()))
                {
                    if (typeof(XmlSchema).Equals(fi.FieldType))
                    {
                        if ((string)info.GetValue(fi.Name, typeof(string)) != null)
                            fi.SetValue(instance, ReadSchema((string)info.GetValue(fi.Name, typeof(string))));
                        else
                            fi.SetValue(instance, null);
                    }
                    else if (typeof(XmlDocument).Equals(fi.FieldType))
                    {
                        XmlDocument d = new XmlDocument();
                        if ((string)info.GetValue(fi.Name, typeof(string)) != null)
                        {
                            string tmp = (string)info.GetValue(fi.Name, typeof(string));
                            if (!tmp.Equals(""))
                                d.LoadXml(tmp);
                                
                            fi.SetValue(instance, d);
                        }
                        else
                            fi.SetValue(instance, null);
                    }                    
                    else if (typeof(XmlSchemaSet).Equals(fi.FieldType))
                    {
                        if(((string[])info.GetValue(fi.Name, typeof(string[]))!=null))
                        {
                            string[] schemasStr = (string[])info.GetValue(fi.Name, typeof(string[]));                        
                            fi.SetValue(instance, ReadSchemaSet(schemasStr));
                        }else
                            fi.SetValue(instance,null);

                    }
                    else
                        fi.SetValue(instance, info.GetValue(fi.Name, fi.FieldType));
                }
            }
        }
    }
}
