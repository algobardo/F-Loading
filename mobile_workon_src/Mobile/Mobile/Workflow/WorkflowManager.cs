using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.IO;
using System.Reflection;
using Mobile.Fields;

namespace Mobile.Workflow
{
    public class WorkflowManager
    {
        #region Private classes

        public class Node
        {
            public Node(String name)
            {
                Name = name;
            }
            public String Name
            {
                get;
                private set;
            }
            public Edge[] Edges
            {
                get;
                set;
            }
            public Element[] Data
            {
                get;
                set;
            }
            public Node Next
            {
                get;
                set;
            }
            public Node Previous
            {
                get;
                set;
            }
            public XmlNode ToXml()
            {
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateElement(Name));
                foreach (Element el in Data)
                    doc.FirstChild.AppendChild(doc.ImportNode(el.ToXml(), true));

                return doc.FirstChild;
            }
        }

        public class Edge
        {
            public Node From 
            {
                get;
                set;
            }
            public Node To
            {
                get;
                set;
            }
            public Operation Precondition
            {
                get;
                set;
            }
        }

        public class Operation
        {
            public MethodInfo Method
            {
                get;
                set;
            }
            public Object Instance
            {
                get;
                set;
            }
            public Object[] Parameters
            {
                get;
                set;
            }
        }

        #endregion

        private Dictionary<String, Node> workflow;
        private Node initialNode;
        private FieldsManager manager;

        public WorkflowManager(FieldsManager manager, XmlDocument workflowNodes, XmlDocument workflowEdges, XmlSchemaSet types)
        {
            this.manager = manager;
            workflow = new Dictionary<string, Node>();
            try
            {
                BuildWorkflow(workflowNodes, workflowEdges, types);
            }
            catch (WorkflowException exc) 
            {
                throw exc;
            }
            catch (Exception exc)
            {
                throw new WorkflowException(WorkflowExceptionType.WorkflowSchemaBadFormat, "There is an error in the workflow schema", exc);
            }
        }

        public XmlDocument Result
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateElement("workflow"));
                Node nextNode = initialNode;
                while (nextNode != null)
                {
                    doc.FirstChild.AppendChild(doc.ImportNode(nextNode.ToXml(), true));
                    nextNode = nextNode.Next;
                }
                return doc;
            }
        }

        public Node PreviousNode(Node filledNode)
        {
            workflow[filledNode.Name].Data = filledNode.Data;

            Node previous = workflow[filledNode.Name].Previous;
            if (previous != null)
                return previous;
            return null;
        }

        public Node NextNode(Node filledNode)
        {
            if (filledNode == null)
                return initialNode;

            Node node = workflow[filledNode.Name];
            node.Data = filledNode.Data;

            //Evaluate conditions, check the next state and return it
            Node nextNode = EvalEdges(node);
            if (nextNode == null)
                return null;
            
            if (node.Next == null || nextNode.Name != node.Next.Name)
            {
                node.Next = nextNode;
                nextNode.Previous = node;
            }
            return nextNode;
        }

        private void OnWorkflowSchemaValidation(object source, ValidationEventArgs args)
        {
        }

        private Node EvalEdges(Node node)
        {
            foreach (Edge edge in node.Edges)
            {
                if (edge.Precondition == null || (bool)Eval(edge.Precondition))
                    return edge.To;
            }
            //If there are no outgoing edges then the state is a final state
            if (node.Edges.Length == 0)
                return null;

            return node;
        }

        private Object Eval(Object param)
        {
            Operation operation = param as Operation;
            if (operation == null)
                return param;

            Object[] values = new Object[operation.Parameters.Length];
            for (int i = 0; i < operation.Parameters.Length; i++ )
                values[i] = Eval(operation.Parameters[i]);

            if (operation.Method.IsStatic)
                return operation.Method.Invoke(null, values);
            else
                return operation.Method.Invoke(operation.Instance, new Object[] { values });
        }

        private Element[] InstantiateNode(XmlElement schema, XmlSchemaSet types)
        {
            List<Element> elements = new List<Element>();
            foreach (XmlNode elementSchema in schema.FirstChild.ChildNodes)
            {
                //Build a schema customized for the element and containing the type and the base type definition
                XmlSchemaType type = types.GlobalTypes[new XmlQualifiedName(elementSchema.Attributes["type"].Value)] as XmlSchemaType;
                XmlSchemaType baseType = type.BaseXmlSchemaType;

                if (type == null || baseType == null)
                    throw new WorkflowException(WorkflowExceptionType.TypesSchemaBadFormat, "The type " + elementSchema.Attributes["type"].Value + " and its base type cannot be found");

                XmlSchema customizedSchema = new XmlSchema();
                customizedSchema.Namespaces.Add("xs", "http://www.w3.org/2001/XMLSchema");
                customizedSchema.Items.Add(baseType);
                customizedSchema.Items.Add(type);
                XmlSchemaSet customizedSet = new XmlSchemaSet();
                customizedSet.Add(customizedSchema);
                customizedSet.Compile();

                Type t = manager.GetElementType(elementSchema.Attributes["type"].Value);
                if (t == null)
                    throw new WorkflowException(WorkflowExceptionType.TypeNotSupported, "The type " + elementSchema.Attributes["type"].Value + " is not supported");

                Element element = (Element)t.GetMethod("New").Invoke(null, new Object[] { customizedSet, elementSchema.Attributes["type"].Value, elementSchema.Attributes["name"].Value });
                elements.Add(element);
            }
            return elements.ToArray();
        }

        #region Build Methods

        private void BuildWorkflow(XmlDocument nodes, XmlDocument edges, XmlSchemaSet types)
        {
            XmlDocument xpathContext = new XmlDocument();
            xpathContext.AppendChild(xpathContext.CreateElement("workflow"));

            //Instantiate all the nodes and put them into the workflow nodes list
            foreach (XmlElement xmlNode in nodes.ChildNodes[1].ChildNodes)
            {
                Node node = new Node(xmlNode.Attributes["name"].Value);
                node.Data = InstantiateNode(xmlNode, types);
                workflow[node.Name] = node;
            }

            //Parse edges
            bool initialNodeFound = false;
            foreach(Node node in workflow.Values) 
            {
                //Append the xml of the node to the context (xml document) used to resolve xpath references
                xpathContext.FirstChild.AppendChild(xpathContext.ImportNode(node.ToXml(), true));
            
                //If the node has no ingoing edges it is the initial nod
                IEnumerable<XmlNode> ingoingEdges =
                    from XmlNode edge in edges.SelectNodes("/EDGES/EDGE")
                    where edge.Attributes["to"].Value.Equals(node.Name)
                    select edge;

                if (ingoingEdges.Count() == 0)
                {
                    if (initialNodeFound)
                        throw new WorkflowException(WorkflowExceptionType.WorkflowSchemaBadFormat, "The workflow must have a single initial node");

                    initialNode = node;
                    initialNodeFound = true;
                }

                //Parse the outgoing edges
                IEnumerable<XmlNode> outgoingEdges =
                    from XmlNode edge in edges.SelectNodes("/EDGES/EDGE")
                    where edge.Attributes["from"].Value.Equals(node.Name)
                    select edge;

                List<Edge> nodeEdges = new List<Edge>();
                foreach (XmlNode xmlEdge in outgoingEdges)
                {
                    Edge edge = new Edge();
                    edge.From = node;
                    edge.To = workflow[xmlEdge.Attributes["to"].Value];
                    edge.Precondition = BuildOperation(xmlEdge.FirstChild, xpathContext) as Operation;
                    nodeEdges.Add(edge);
                }
                node.Edges = nodeEdges.ToArray();
            }
        }

        private Object BuildOperation(XmlNode operation, XmlDocument xpathContext)
        {
            if (operation.Name == "AND")
            {
                Object[] pars =
                    (from XmlNode child in operation.ChildNodes
                     select BuildOperation(child, xpathContext)).ToArray();

                foreach (Object par in pars)
                    if (!(par is bool || (par is Operation && ((Operation)par).Method.ReturnType == typeof(bool))))
                        throw new WorkflowException(WorkflowExceptionType.TypeError, "The AND predicate must be evalutated on boolean parameters");

                return
                    new Operation()
                    {
                        Method = this.GetType().GetMethod("And"),
                        Instance = this,
                        Parameters = pars
                    };
            }
            if (operation.Name == "OR")
            {
                Object[] pars =
                    (from XmlNode child in operation.ChildNodes
                     select BuildOperation(child, xpathContext)).ToArray();

                foreach (Object par in pars)
                    if (!(par is bool || (par is Operation && ((Operation)par).Method.ReturnType == typeof(bool))))
                        throw new WorkflowException(WorkflowExceptionType.TypeError, "The OR predicate must be evalutated on boolean parameters");

                return
                    new Operation()
                    {
                        Method = this.GetType().GetMethod("Or"),
                        Instance = this,
                        Parameters = pars
                    };
            }
            if (operation.Name == "NOT")
            {
                Object[] pars =
                    (from XmlNode child in operation.ChildNodes
                     select BuildOperation(child, xpathContext)).ToArray();

                if (pars.Length != 1)
                    throw new WorkflowException(WorkflowExceptionType.TypeError, "The NOT predicate must be evalutated on a single parameter");

                foreach (Object par in pars)
                    if (!(par is bool || (par is Operation && ((Operation)par).Method.ReturnType == typeof(bool))))
                        throw new WorkflowException(WorkflowExceptionType.TypeError, "The NOT predicate must be evalutated on boolean parameters");

                return
                    new Operation()
                    {
                        Method = this.GetType().GetMethod("Not"),
                        Instance = this,
                        Parameters = pars
                    };
            }

            Object[] parameters =
                (from XmlNode child in operation.ChildNodes
                 select BuildParameter(child, xpathContext)).ToArray();

            List<Type> parTypes = new List<Type>();
            foreach (Object obj in parameters)
            {
                if (obj is Operation)
                    parTypes.Add(((Operation)obj).Method.ReturnType);
                else
                    parTypes.Add(obj.GetType());
            }

            foreach (Type parType in parTypes)
            {
                MethodInfo method = parType.GetMethod(operation.Name, parTypes.ToArray());
                if (method == null)
                    throw new WorkflowException(WorkflowExceptionType.OperationNotFound,
                        String.Format("Cannot found the operation {0}({1})",
                        method.Name,
                        ConcatenateTypeNames(parTypes, ",")));
                return
                    new Operation()
                    {
                        Method = method,
                        Parameters = parameters
                    };
            }
            return null;
        }

        private Object BuildParameter(XmlNode parameter, XmlDocument xpathContext)
        {
            if (parameter.Attributes["type"].Value == "value")
            {
                Type type = manager.GetElementType(parameter.FirstChild.Name);
                if (type == null)
                    throw new WorkflowException(WorkflowExceptionType.TypeNotSupported, "The type " + parameter.FirstChild.Name + " is not supported");

                Element element = (Element)type.GetMethod("New").Invoke(null, new Object[] { null, parameter.FirstChild.Name, parameter.FirstChild.Name });
                element.FromXml(parameter.FirstChild as XmlElement);
                return element;
            }
            if (parameter.Attributes["type"].Value == "path")
            {
                //Map xpath to an object reference
                String xpath = (parameter.FirstChild as XmlNode).InnerText.Trim();
                XmlElement selected = xpathContext.SelectSingleNode(xpath) as XmlElement;
                if (selected == null)
                    throw new WorkflowException(WorkflowExceptionType.WorkflowSchemaBadFormat, "The XPath " + xpath + " cannot be resolved");

                for (int i = 0; i < selected.ParentNode.ChildNodes.Count; i++)
                {
                    if (selected.ParentNode.ChildNodes[i] == selected)
                        return workflow[selected.ParentNode.Name].Data[i];
                }
            }
            if (parameter.Attributes["type"].Value.Equals("operation", StringComparison.InvariantCultureIgnoreCase))
            {
                return
                    BuildOperation(parameter.FirstChild, xpathContext);
            }
            return null;
        }

        #endregion
        #region Embedded Operations

        public bool And(Object[] pars)
        {
            bool result = true;
            foreach (Object par in pars)
                result = result && par is bool && (bool)par;

            return result;
        }

        public bool Or(Object[] pars) 
        {
            bool result = false;
            foreach(Object par in pars)
                result = result || (par is bool && (bool)par);

            return result;
        }

        public bool Not(Object[] pars) 
        {
            return pars[0] is bool && !((bool)pars[0]);
        }

        #endregion

        private String ConcatenateTypeNames(IEnumerable<Type> types, String separator)
        {
            String concatenation = "";
            foreach (Type t in types)
                concatenation += t.FullName + separator;

            if (concatenation.Length > 0)
                return concatenation.Substring(0, concatenation.LastIndexOf(separator));

            return concatenation;
        }
    }
}
