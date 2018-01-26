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
using Mobile.Fields.Group;
using Mobile.Communication;

namespace Mobile.Workflow
{
    public class WorkflowManager
    {
        #region Private Classes

        private class Node
        {
            public Node(String id)
            {
                ID = id;
            }

            public String ID
            {
                get;
                private set;
            }

            public String Name
            {
                get;
                set;
            }

            public String Description
            {
                get;
                set;
            }

            public Edge[] Edges
            {
                get;
                set;
            }
            public Field[] Fields
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

            public void Clear()
            {
                foreach (Field element in Fields)
                    element.Clear();
            }

            public XmlElement ToXml()
            {
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateElement(ID));
                foreach (Field field in Fields)
                {
                    if (field is SequenceBox)
                    {
                        //If the field is a SequenceBox we must cut off the root
                        XmlElement element = field.ToXml();
                        foreach (XmlElement child in element.ChildNodes)
                            doc.FirstChild.AppendChild(doc.ImportNode(child, true));
                    }
                    else
                        doc.FirstChild.AppendChild(doc.ImportNode(field.ToXml(), true));
                }

                return doc.FirstChild as XmlElement;
            }

            public void FromXml(XmlElement node)
            {
                int index = 0;
                foreach (Field field in Fields)
                {
                    if (field is SequenceBox)
                    {
                        XmlDocument document = new XmlDocument();
                        XmlElement root = document.CreateElement("SequenceBox");

                        XmlSchemaElement element = field.Schema.GlobalElements[new XmlQualifiedName(field.FieldName)] as XmlSchemaElement;
                        XmlSchemaComplexType type = element.SchemaType as XmlSchemaComplexType;
                        XmlSchemaSequence sequence = type.Particle as XmlSchemaSequence;
                        String childName = (sequence.Items[0] as XmlSchemaElement).Name;

                        while (index < node.ChildNodes.Count && node.ChildNodes[index].Name == childName)
                        {
                            root.AppendChild(document.ImportNode(node.ChildNodes[index], true));
                            index++;
                        }

                        field.FromXml(root);
                    }
                    else
                    {
                        field.FromXml(node.ChildNodes[index] as XmlElement);
                        index++;
                    }
                }
            }

            public void ParsePresentation(XmlElement presentation)
            {
                if (presentation.HasChildNodes)
                {
                    if (presentation.FirstChild.Attributes["name"] != null)
                        Name = presentation.FirstChild.Attributes["name"].Value;

                    XmlNodeList fields = presentation.FirstChild.FirstChild.FirstChild.ChildNodes;
                    for (int i = 0; i < Math.Min(fields.Count, Fields.Length); i++)
                        Fields[i].ParsePresentation(fields[i] as XmlElement);
                }
            }
        }

        private class Edge
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

        private class Operation
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

        private FormInfo info;
        private Dictionary<String, Node> workflow;
        private Node initialNode;
        private Node lastFilledNode;
        private bool finished;

        public NodeController Current
        {
            get;
            private set;
        }

        public String FirstNodeID
        {
            get
            {
                return initialNode.ID;
            }
        }

        public int CurrentNodeCount
        {
            get;
            private set;
        }

        public int EstimatedRemainingNodesCount
        {
            get;
            private set;
        }

        public NodeController[] Summary
        {
            get
            {
                List<NodeController> summary = new List<NodeController>();
                if (lastFilledNode != null)
                {
                    Node nextNode = initialNode;
                    while (nextNode != lastFilledNode)
                    {
                        summary.Add(CreateController(nextNode, false));
                        nextNode = nextNode.Next;
                    }
                    summary.Add(CreateController(lastFilledNode, false));
                }
                return summary.ToArray();
            }
        }

        public WorkflowManager(FormInfo info, XmlSchema workflowNodes, XmlDocument workflowEdges, XmlSchema types, XmlDocument presentation, XmlDocument result)
        {
            this.info = info;

            finished = false;
            initialNode = null;
            lastFilledNode = null;
            workflow = new Dictionary<string, Node>();
            try
            {
                BuildWorkflow(workflowNodes, workflowEdges, types, presentation);
                CurrentNodeCount = 0;
                EstimatedRemainingNodesCount = NodesToEnd(initialNode);

                if (result != null)
                    FromXml(result);
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

        public XmlDocument ToXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement(XmlConvert.EncodeName(info.Name)));

            //if lastFilledNode is null no one state has been validated by the workflow and then the "product" of the workflow is empty
            if (lastFilledNode != null)
            {
                Node nextNode = initialNode;
                while (nextNode != lastFilledNode)
                {
                    doc.FirstChild.AppendChild(doc.ImportNode(nextNode.ToXml(), true));
                    nextNode = nextNode.Next;
                }
                doc.FirstChild.AppendChild(doc.ImportNode(lastFilledNode.ToXml(), true));

            }
            return doc;
        }

        public void FromXml(XmlDocument result)
        {
            //Restore the state of the workflow using result
            Node lastNode = null;
            foreach (XmlNode xmlNode in result.FirstChild.ChildNodes)
            {
                Node node = workflow[xmlNode.Name];

                //Restore node data
                node.FromXml(xmlNode as XmlElement);

                //Restore node links making the path followed by the user
                node.Previous = lastNode;
                if (lastNode != null)
                    lastNode.Next = node;

                lastNode = node;

                CurrentNodeCount++;
            }
            //last node is null if the result is empty, thus it doesn't contain any node (i.e. <workflow></workflow>), but that should be impossible
            if (lastNode == null)
                Current = null;
            else
                Current = CreateController(lastNode, true);

            //the last node the user filled and the one will be shown now is the last found in result
            lastFilledNode = lastNode;

            EstimatedRemainingNodesCount = NodesToEnd(lastNode);
        }

        public NodeController Previous()
        {
            //workflow[filledNode.Name].Data = filledNode.Data;
            if (finished)
            {
                finished = false;
                CurrentNodeCount--;

                Node nextNode = initialNode;
                while (nextNode.Next != null)
                    nextNode = nextNode.Next;

                Current = CreateController(nextNode, true);
                return Current;
            }
            if (Current != null)
            {
                Node previous = workflow[Current.ID].Previous;
                //For sake of consistency of lastFilledNode we have to test if the modified content of the node,
                //once the user has asked for the previous one, brings to a different "next" node
                Node nextNode = EvalEdges(workflow[Current.ID]);
                if (nextNode != workflow[Current.ID].Next)
                    lastFilledNode = workflow[Current.ID];

                if (previous != null)
                {
                    CurrentNodeCount--;
                    EstimatedRemainingNodesCount++;

                    Current = CreateController(previous, true);
                    return Current;
                }
            }
            return null;
        }

        public NodeController Next()
        {
            if (Current == null)
            {
                Current = CreateController(initialNode, true);
                CurrentNodeCount++;
                return Current;
            }

            Node node = workflow[Current.ID];

            //Evaluate conditions, check the next state and return it
            Node nextNode = EvalEdges(node);

            //If nextNode is null then "node" is a final node: the workflow has finished
            if (nextNode == null)
            {
                EstimatedRemainingNodesCount = 0;
                CurrentNodeCount++;

                finished = true;
                lastFilledNode = node;
                Current = null;
                return Current;
            }
            //If nextNode.Name == node.Name no transitions have not been performed: the workflow remains in the same state
            if (nextNode.ID == node.ID)
            {
                Current = CreateController(nextNode, true);
                lastFilledNode = node;
                return Current;
            }
            //This happens if the user goes back, change a node, asks for the next node and this one is different for that
            //he encountered going back: the user is entering a new branch
            if (node.Next == null || nextNode.ID != node.Next.ID)
            {
                //Rollback, clear the data filled in the nodes from the current to the end of the previous path
                Node toBeCleared = node.Next;
                while (toBeCleared != null)
                {
                    //Clear links
                    toBeCleared.Previous.Next = null;
                    toBeCleared.Previous = null;
                    //Clear content
                    toBeCleared.Clear();

                    toBeCleared = toBeCleared.Next;
                }
                //Update the current branch of the workflow
                node.Next = nextNode;
                nextNode.Previous = node;

                //The last filled node is now the root of the branch since the old branch has been deleted (cleared)

                CurrentNodeCount++;
                EstimatedRemainingNodesCount = NodesToEnd(nextNode);

                lastFilledNode = node;
                Current = CreateController(nextNode, true);
                return Current;
            }

            EstimatedRemainingNodesCount = NodesToEnd(nextNode);
            CurrentNodeCount++;
            Current = CreateController(nextNode, true);
            return Current;
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
            for (int i = 0; i < operation.Parameters.Length; i++)
                values[i] = Eval(operation.Parameters[i]);

            if (operation.Method.IsStatic)
                return operation.Method.Invoke(null, values);
            else
                return operation.Method.Invoke(operation.Instance, new Object[] { values });
        }

        private Field[] InstantiateNode(XmlSchemaComplexType schema, XmlSchema types)
        {
            List<Field> elements = new List<Field>();
            foreach (XmlSchemaElement elementSchema in (schema.Particle as XmlSchemaSequence).Items)
            {
                //Build a schema customized for the element and containing the type and the base type definition
                Field element = FieldFactory.CreateField(elementSchema);
                if (element == null)
                    throw new WorkflowException(WorkflowExceptionType.TypeNotSupported, elementSchema.SchemaType != null ? "The type " + elementSchema.SchemaType.Name + " is not supported" : "There is an error in the type system");

                elements.Add(element);
            }
            return elements.ToArray();
        }

        #region Build Methods

        private void BuildWorkflow(XmlSchema nodes, XmlDocument edges, XmlSchema types, XmlDocument presentation)
        {
            XmlDocument xpathContext = new XmlDocument();
            xpathContext.AppendChild(xpathContext.CreateElement("workflow"));

            //Instantiate all the nodes and put them into the workflow nodes list
            foreach (XmlSchemaObject obj in nodes.Items)
            {
                if (obj is XmlSchemaComplexType && ((XmlSchemaComplexType)obj).Name != null)
                {
                    Node node = new Node(((XmlSchemaComplexType)obj).Name);
                    node.Fields = InstantiateNode(((XmlSchemaComplexType)obj), types);
                    workflow[node.ID] = node;
                }
            }

            //Parse edges
            foreach (Node node in workflow.Values)
            {
                //Append the xml of the node to the context (xml document) used to resolve xpath references
                xpathContext.FirstChild.AppendChild(xpathContext.ImportNode(node.ToXml(), true));

                //Parse the outgoing edges
                IEnumerable<XmlNode> outgoingEdges =
                    from XmlNode edge in edges.SelectNodes("/EDGES/EDGE")
                    where edge.Attributes["from"] != null && edge.Attributes["from"].Value.Equals(node.ID)
                    select edge;

                List<Edge> nodeEdges = new List<Edge>();
                foreach (XmlNode xmlEdge in outgoingEdges)
                {
                    Edge edge = new Edge();
                    edge.From = node;
                    //If to == null then the node is a final node
                    edge.To = xmlEdge.Attributes["to"] == null ? null : workflow[xmlEdge.Attributes["to"].Value];

                    //If <EDGE> or <PRECONDITION> nodes have no children then the precondition is null and the arch is always followed
                    if (xmlEdge.ChildNodes.Count == 0 || xmlEdge.FirstChild.ChildNodes.Count == 0)
                        edge.Precondition = null;
                    else
                        edge.Precondition = BuildOperation(xmlEdge.FirstChild.FirstChild, types, xpathContext) as Operation;

                    nodeEdges.Add(edge);
                }
                node.Edges = nodeEdges.ToArray();
            }

            //Look for the ONLY edge that has no "from" attributes. This points to the initial node
            XmlNode[] edgeToInitial =
                (from XmlNode edge in edges.SelectNodes("/EDGES/EDGE")
                 where edge.Attributes["from"] == null && edge.Attributes["to"] != null
                 select edge).ToArray();

            if (edgeToInitial.Length != 1)
                throw new WorkflowException(WorkflowExceptionType.WorkflowSchemaBadFormat, "The workflow must have a single initial node");

            initialNode = workflow[edgeToInitial[0].Attributes["to"].Value];

            //Associate description e labels (names) to nodes and fields
            foreach (XmlNode child in presentation.DocumentElement.ChildNodes)
            {
                XmlElement element = child as XmlElement;
                if (workflow.ContainsKey(element.Name))
                {
                    Node node = workflow[element.Name];
                    node.ParsePresentation(element);
                }
            }
        }

        private Object BuildOperation(XmlNode operation, XmlSchema types, XmlDocument xpathContext)
        {
            if (operation.Name == "AND")
            {
                Object[] pars =
                    (from XmlNode child in operation.ChildNodes
                     select BuildOperation(child, types, xpathContext)).ToArray();

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
                     select BuildOperation(child, types, xpathContext)).ToArray();

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
                     select BuildOperation(child, types, xpathContext)).ToArray();

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
                 select BuildParameter(child, types, xpathContext)).ToArray();

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

        private Object BuildParameter(XmlNode parameter, XmlSchema types, XmlDocument xpathContext)
        {
            if (parameter.Attributes["type"].Value == "value")
            {
                XmlSchema onTheFlySchema = new XmlSchema();

                XmlSchemaElement valueSchema = new XmlSchemaElement();

                valueSchema.Name = parameter.FirstChild.Name;
                valueSchema.SchemaType = types.SchemaTypes[new XmlQualifiedName(parameter.FirstChild.Name)] as XmlSchemaType;

                Field field = FieldFactory.CreateField(valueSchema);
                if (field == null)
                    throw new WorkflowException(WorkflowExceptionType.TypeNotSupported, "The type " + parameter.FirstChild.Name + " is not supported");

                field.FromXml(parameter.FirstChild as XmlElement);
                return field;
            }
            if (parameter.Attributes["type"].Value == "path")
            {
                //Map xpath to an object reference
                String xpath = (parameter.FirstChild as XmlNode).InnerText.Trim();
                XmlElement selected = xpathContext.SelectSingleNode(xpath) as XmlElement;
                if (selected == null)
                    throw new WorkflowException(WorkflowExceptionType.WorkflowSchemaBadFormat, "The XPath " + xpath + " cannot be resolved");

                for (int i = 0; i < workflow[selected.ParentNode.Name].Fields.Length; i++)
                {
                    if (workflow[selected.ParentNode.Name].Fields[i].FieldName == selected.Name)
                        return workflow[selected.ParentNode.Name].Fields[i];
                }
            }
            if (parameter.Attributes["type"].Value.Equals("operation", StringComparison.InvariantCultureIgnoreCase))
            {
                return
                    BuildOperation(parameter.FirstChild, types, xpathContext);
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
            foreach (Object par in pars)
                result = result || (par is bool && (bool)par);

            return result;
        }

        public bool Not(Object[] pars)
        {
            return pars[0] is bool && !((bool)pars[0]);
        }

        #endregion
        #region Private Methods

        private String ConcatenateTypeNames(IEnumerable<Type> types, String separator)
        {
            String concatenation = "";
            foreach (Type t in types)
                concatenation += t.FullName + separator;

            if (concatenation.Length > 0)
                return concatenation.Substring(0, concatenation.LastIndexOf(separator));

            return concatenation;
        }

        private NodeController CreateController(Node node, bool enabled)
        {
            List<FieldController> fieldControllers = new List<FieldController>();
            foreach (Field field in node.Fields)
                fieldControllers.Add(FieldFactory.CreateController(field, enabled));

            NodeController controller = new NodeController(node.ID, node.Name, fieldControllers.ToArray());
            return controller;
        }

        private int NodesToEnd(Node node)
        {
            if (node == null)
                return 0;

            int max = 0;
            foreach (Edge edge in node.Edges)
            {
                Node to = edge.To;
                int temp = NodesToEnd(to);
                if (temp > max)
                    max = temp;
            }
            return 1 + max;
        }

        #endregion
    }
}
