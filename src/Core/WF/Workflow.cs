using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;
using Fields;
using System.Runtime.Serialization;

namespace Core.WF
{
    public class WorkflowValidationEventArgs : EventArgs
    {
        public WorkflowValidationEventArgs(string info, bool valid)
        {
            Valid = valid;
            Message = info;
        }

        public bool Valid { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// This class represent Worksflow as defined in /Doc/WorkflowGroup/Workflow.txt
    /// </summary>    
    [Serializable]
    public class Workflow:IDisposable
    {
        [field:NonSerialized]
        public event EventHandler<WorkflowValidationEventArgs> WorkflowInvalidationEvent;
        
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            /*Register again all event handler*/
            foreach (WFnode nd in connectionGraph.Nodes)
            {
                nd.NodeNameModified += nd_NodeNameModified;
                nd.FieldModified += nd_FieldModified;
            }
            foreach (WFedgeLabel ed in connectionGraph.Edges)
            {
                ed.EdgeModified += ed_EdgeModified;
            }
        }


        protected virtual void OnWorkflowInvalidationEvent(WorkflowValidationEventArgs e)
        {
            if (WorkflowInvalidationEvent != null)
                WorkflowInvalidationEvent(this, e);
        }

        /// <summary>
        /// Empty costructor for Workflow
        /// </summary>
        private Workflow() { }


        /// <summary>
        /// Constructor for workflow
        /// </summary>
        /// <param name="name">The name of the workflow</param>
        public Workflow(string name)
        {
            XmlConvert.VerifyNCName(name);           
            workflowName = name;
        }

        public string WorkflowName
        {
            get {
                return XmlConvert.DecodeName(workflowName);
            }
            set
            {
                workflowName = XmlConvert.EncodeLocalName(value);
            }
        }

        /// <summary>
            /// This method return the representation of all edge in the workflow for WFeditor
            /// </summary>
            /// <returns>All edge in the workflow</returns>
            public XmlDocument GetEdgesForWFE()
            {
                XmlDocument rsult = new XmlDocument();
                XmlElement allEdges = rsult.CreateElement("EDGES", "");
                rsult.AppendChild(allEdges);

                foreach (WFedgeLabel edgeId in connectionGraph.Edges)
                {
                    WFedgeLabel edgeValue = edgeId;

                    Triple<WFedgeLabel, WFnode, WFnode> edgeInfos = connectionGraph.GetConnectingNodes(edgeId);

                    XmlElement singleEdgeNode = rsult.CreateElement("EDGE");
                    XmlAttribute aFrom = rsult.CreateAttribute("from");
                    XmlAttribute aTo = rsult.CreateAttribute("to");
                    aFrom.Value = XmlConvert.EncodeLocalName(edgeInfos.SecondMember.NodeTypeName);                    
                    aTo.Value = XmlConvert.EncodeLocalName(edgeInfos.ThirdMember.NodeTypeName);
                    singleEdgeNode.Attributes.Append(aFrom);
                    singleEdgeNode.Attributes.Append(aTo);

                    /*Appending preconditions and actions*/
                    singleEdgeNode.AppendChild(rsult.ImportNode(edgeValue.GetEntireDescription().DocumentElement, true));

                    allEdges.AppendChild(singleEdgeNode);

                }
                return rsult;
            }

        [Serializable]
        private class ComputableWorkflow : IComputableWorkflow,IDisposable
        {
            private Workflow editedWf;

            private WFnode actualState = null;

            private Stack<WFnode> rollbackWFnodes;

            private FinalWFdocument finalDocument;

            private string description;

            /// <summary>
            /// Empty constructor for ComputableWorkflow
            /// </summary>
            
            private ComputableWorkflow() { }

           

            /// <summary>
            /// Constructor for ComputableWorkflow
            /// </summary>
            /// <param name="wf">A workflow to extract the information</param>
           
            public ComputableWorkflow(Workflow wf)
			{
                editedWf = wf;
                actualState = wf.initialNode;
                finalDocument = new FinalWFdocument(wf.workflowName);                
                rollbackWFnodes = new Stack<WFnode>();
            }


                           


            #region IComputableWorkflow Members

            public void setWFname(string name)
            {
                editedWf.workflowName = XmlConvert.EncodeLocalName(name);
            }

            public XmlDocument GetXMLDocument() {
                XmlDocument doc = new XmlDocument();
                XmlNode node = doc.CreateElement("root");

                foreach(WFnode n in editedWf.connectionGraph.Nodes){
                    node.AppendChild(doc.ImportNode(n.GetRenderingDocument().DocumentElement, true));
                }
                doc.AppendChild(node);
                return doc;
            }


            public string getWFname()
            {
                return XmlConvert.DecodeName(editedWf.workflowName);
            }

            /// <summary>
            /// Getter for the actual node
            /// </summary>
            /// <returns>the actual node in the workflow</returns>
            public WFnode GetState()
            {
                return actualState;
            }

            /// <summary>
            /// Check if the actual node is the final node of the workflow
            /// </summary>
            /// <returns>True if the the actual node is the final node, false otherwise</returns>
            public bool IsFinalState()
            {
                return editedWf.finalNodes.Contains(actualState);
            }

            /// <summary>
            /// Check if the actual node is the initial node of the workflow
            /// </summary>
            /// <returns>True if the the actual node is the initial node, false otherwise</returns>
            public bool IsInitialState()
            {
                return actualState == editedWf.initialNode;
            }

            /// <summary>
            /// Getter for the workflow description
            /// </summary>
            /// <returns></returns>
            public string GetDescription()
            {
                return description;
            }

            /// <summary>
            /// Setter for the workflow description
            /// </summary>
            /// <param name="descr"></param>
            public void SetDescription(string descr)
            {
                description = descr;
            }


            /// <summary>
            /// Return the names of the node that the workflow had through
            /// </summary>
            /// <returns>A list contains the names of the WFnodes through</returns>
            public List<string> GetThroughPath()
            {
                List<string> list = new List<string>();

                for (int i = 0; i < finalDocument.Count; i++)
                    list.Add(finalDocument.ElementAt(i).Name);

                return list;
            }

            /// <summary>
            /// Transit from one state to another, after consider the event verified and which edge verify precondition
            /// </summary>
            /// <param name="evt">The event verified</param>
            /// <param name="data">The actual node</param>
            /// <param name="handler">Validation Handler</param>
            /// <returns></returns>
            public ActionResult ComputeNewStatus(WFeventType evt, XmlDocument data, ValidationEventHandler handler)
            {
                if (evt == WFeventType.TRYGOON)
                {                                       
                    actualState.Value = data.DocumentElement;


                    if (actualState.Validate(handler))
                    {
                 
                        if (editedWf.finalNodes.Contains(actualState))
                        {
                            /*Save data */
                            finalDocument.Push(actualState);

                            actualState = null;
                            return new ActionResult(true);
                        }
                        
                        /*Compute new status*/
                        List<WFedgeLabel> edges = editedWf.connectionGraph.GetOutgoingEdges(actualState);
                        edges.Sort((x, y) => x.Priority - y.Priority);

                        foreach (WFedgeLabel edgeIdToEval in edges)
                        {
                            //COMMENTATO PER SICUREZZA DI FUNZIONAMENTO
                            WFedgeLabel edgeToEval = edgeIdToEval;
                            if (edgeToEval.VerifyPrecondition(actualState))
                            {
                                /*Save data */
                                finalDocument.Push(actualState);

                                //aggiunto controllo sullo stack di rollback (vuoto / non vuoto)
                                if (rollbackWFnodes.Count != 0)
                                {
                                    WFnode rollbackNode = rollbackWFnodes.Pop(); //recupero sia il WFnode che la sua istanza dal top dei 2 stack
                                    WFedgeLabel rollbackEdge = editedWf.connectionGraph.GetEdge(actualState, rollbackNode);
                                    if (edgeIdToEval != rollbackEdge)
                                    {
                                        rollbackWFnodes = new Stack<WFnode>();
                                        actualState = editedWf.connectionGraph.GetDestination(actualState, edgeIdToEval);
                                    }
                                    else
                                    {
                                        actualState = rollbackNode;
                                    }
                                }                                
                                else
                                    actualState = editedWf.connectionGraph.GetDestination(actualState, edgeIdToEval);
                                return new ActionResult(true);
                            }
                        }
                        Debug.WriteLine("Not avery edges precondition is satisfied");
                        return new ActionResult(false);

                    }
                    else
                        actualState.Value = null;

                    Debug.WriteLine("Passed node is not valid! Validation failed");
                    return new ActionResult(false);

                    
                }
                else if(evt == WFeventType.ROLLBACK)
                {
                    if (IsInitialState())
                    {
                        Debug.WriteLine("Cannot rollback from initial node!");
                        return new ActionResult(false);
                    }
                    if(actualState!=null)
                        rollbackWFnodes.Push(actualState);
                    actualState = finalDocument.Pop();                                        
                    return new ActionResult(true);

                }
                return new ActionResult(false);
            }

            public bool IsEndComputationState
            {
                get
                {
                    return actualState == null;
                }
            }

            //requested from theme editor
            public IEnumerable<WFnode> getNodeList()
            {
                return editedWf.connectionGraph.Nodes;
            }

            /// <summary>
            /// This method return a final document represent all node that compose the workflow
            /// </summary>
            /// <returns>The final Document of workflow</returns>
            public XmlDocument GetCollectedDocument()
            {                
                return finalDocument.GetDocument();
            }

            public void ReverseFinalDocument()
            {
                FinalWFdocument doc = new FinalWFdocument(editedWf.workflowName);
                FinalWFdocument buff = finalDocument;

                while (buff.Count != 0)
                    doc.Push(buff.Pop());

                finalDocument = doc;                
            }
            private string getCDSchema(WFnode node)                
            {
                string ret = "<xs:sequence>";

                ret += "<xs:sequence>";
                ret += "<xs:element name=\"" + XmlConvert.EncodeLocalName(node.NodeTypeName) + "\" type=\"" + XmlConvert.EncodeLocalName(node.NodeTypeName) + "\"/>";

                WFnode endNode = node;
                List<WFnode> tmp = null;
                while (endNode == null || (tmp = editedWf.connectionGraph.getOutcomingNodes(endNode)).Count == 1)
                {
                    endNode = tmp[0];
                    ret += "<xs:element name=\"" + XmlConvert.EncodeLocalName(tmp[0].NodeTypeName) + "\" type=\"" + XmlConvert.EncodeLocalName(tmp[0].NodeTypeName) + "\"/>";
                }
                ret += "</xs:sequence>";
                if (editedWf.connectionGraph.getOutcomingNodes(endNode).Count != 0)
                {
                    ret += "<xs:choice>";
                    foreach (WFnode nn in editedWf.connectionGraph.getOutcomingNodes(endNode))
                    {
                        ret += getCDSchema(nn);
                    }
                    ret += "</xs:choice>";
                }
                ret += "</xs:sequence>";
                return ret;
            }
    

            /// <summary>
            /// This method return all workflow's schemas
            /// </summary>
            /// <returns>A set of workflow schemas</returns>
            public XmlSchemaSet GetCollectedDocumentSchemas()
            {
                var res = "<?xml version=\"1.0\" encoding=\"utf-8\"?><xs:schema id=\"XMLSchema1\" elementFormDefault=\"qualified\"  xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">";
                
                res+="<xs:element name=\""+this.editedWf.workflowName+"\"><xs:complexType>"+getCDSchema(editedWf.initialNode)+"</xs:complexType></xs:element>";

                res += "</xs:schema>";

                XmlSchemaSet st = new XmlSchemaSet();
                st.Add(Utils.ReadSchema(res));
                st.Add(this.GetNodesSchemas());
                st.Compile();

                return st;                
            }


            public List<string> GetNodesTypeName()
            {
                List<string> lst = new List<string>();
                foreach (WFnode nd in editedWf.connectionGraph.Nodes)
                {
                    lst.Add(nd.NodeTypeName);
                }
                return lst;
            }

            /// <summary>
            /// This method return the representation of all edge in the workflow
            /// </summary>
            /// <returns>All edge in the workflow</returns>
            public XmlDocument GetEdges()
            {
                XmlDocument rsult = new XmlDocument();
                XmlElement allEdges = rsult.CreateElement("EDGES", "");
                var attr = rsult.CreateAttribute("workflowName");
                attr.Value=editedWf.WorkflowName.FromNotEncodedRender2XmlValue();
                allEdges.Attributes.Append(attr);
                rsult.AppendChild(allEdges);
                
                /*Add initial edge*/
                XmlElement initialEdgeElement = rsult.CreateElement("EDGE");                
                XmlAttribute initialEdgeTo = rsult.CreateAttribute("to");                
                initialEdgeTo.Value = XmlConvert.EncodeLocalName(editedWf.initialNode.NodeTypeName);                
                initialEdgeElement.Attributes.Append(initialEdgeTo);
                allEdges.AppendChild(initialEdgeElement);
                
                
                foreach (WFedgeLabel edgeId in editedWf.connectionGraph.Edges)
                {
                    WFedgeLabel edgeValue = edgeId;

                    Triple<WFedgeLabel,WFnode,WFnode>  edgeInfos = editedWf.connectionGraph.GetConnectingNodes(edgeId);

                    XmlElement singleEdgeNode = rsult.CreateElement("EDGE");
                    XmlAttribute aFrom = rsult.CreateAttribute("from");
                    XmlAttribute aTo = rsult.CreateAttribute("to");
                    aFrom.Value = XmlConvert.EncodeLocalName(edgeInfos.SecondMember.NodeTypeName);
                    aTo.Value = XmlConvert.EncodeLocalName(edgeInfos.ThirdMember.NodeTypeName);
                    singleEdgeNode.Attributes.Append(aFrom);
                    singleEdgeNode.Attributes.Append(aTo);

                    /*Appending preconditions and actions*/                    
                    singleEdgeNode.AppendChild(rsult.ImportNode( edgeValue.GetEntireDescription().DocumentElement,true ));

                    allEdges.AppendChild(singleEdgeNode);
                    
                }

                /*Adding edges for final nodes*/
                foreach (WFnode finalNd in editedWf.finalNodes)
                {
                    XmlElement singleEdgeNode = rsult.CreateElement("EDGE");
                    XmlAttribute aFrom = rsult.CreateAttribute("from");                    
                    aFrom.Value = XmlConvert.EncodeLocalName(finalNd.NodeTypeName);                    
                    singleEdgeNode.Attributes.Append(aFrom);
                    allEdges.AppendChild(singleEdgeNode);   
                }

                return rsult;
            }

            

            /// <summary>
            /// Return the schemas of all node present in the workflow
            /// </summary>
            /// <returns>The set of node schema</returns>
            public XmlSchemaSet GetNodesSchemas()
            {
                XmlSchemaSet resSchema = new XmlSchemaSet();
                resSchema.Add(Fields.FieldsManager.FieldTypesXSD);
                foreach (WFnode nd in editedWf.connectionGraph.Nodes)
                {
                    resSchema.Add(nd.GetNodeSchemasWithoutBaseTypes());
                }
                
                resSchema.Compile();

                return resSchema;
            }
           
            /// <summary>
            /// Reset the computation and clear the final Document
            /// </summary>
            public void ResetComputation()
            {
                actualState = editedWf.initialNode;
                foreach (WFnode n in editedWf.connectionGraph.Nodes)
                {
                    var ls = new List<XmlNode>();
                    ls.Add(null);
                    n.SetValue(ls);
                }
                finalDocument.Clear();
            }

            public WorkflowDescription GetEntireWorkflowDescription()
            {
                WorkflowDescription winf = new WorkflowDescription();

                winf.EdgesDescription = this.GetEdges().OuterXml;
                XmlSchemaSet schemaCpy = Utils.ReadSchemaSet(Utils.WriteSchemaSet(this.GetNodesSchemas()));
                List<string> nodes = this.GetNodesTypeName();
                schemaCpy.Compile();
                XmlSchema ndSchema = new XmlSchema();
                foreach (string name in nodes)
                {
                    ndSchema.Items.Add(schemaCpy.GlobalTypes[new XmlQualifiedName(XmlConvert.EncodeLocalName(name))]);
                }
                winf.NodesSchema = Utils.WriteSchema(ndSchema);

                XmlSchema s2 = new XmlSchema();
                foreach (XmlSchema sch in schemaCpy.Schemas())
                {
                    foreach (XmlSchemaObject obj in sch.Items)
                    {
                        if (!(obj is XmlSchemaComplexType && nodes.Contains(((XmlSchemaComplexType)obj).Name)))
                            s2.Items.Add(obj);
                    }
                }

                winf.ExtendedTypesSchema = Utils.WriteSchema(s2);

                StringBuilder sb = new StringBuilder();
                sb.Append("<RENDERINGS>");
                foreach (WFnode rendNode in editedWf.connectionGraph.Nodes)
                {
                    sb.Append("<" + XmlConvert.EncodeLocalName(rendNode.NodeTypeName) + ">");
                    sb.Append(rendNode.GetRenderingDocument().OuterXml);
                    sb.Append("</" + XmlConvert.EncodeLocalName(rendNode.NodeTypeName) + ">");
                }
                sb.Append("</RENDERINGS>");
                winf.RenderingDocument = sb.ToString();

                return winf;
            }
           

            #endregion


            #region IDisposable Members

            public void Dispose()
            {
                this.editedWf.Dispose();
            }

            #endregion

            
        }
        /// <summary>
        /// The name of the workflow
        /// </summary>
        [NonSerialized]
        private string workflowName;

        /// <summary>
        /// The graph containing workflow representation.
        /// </summary>
        private Graph<WFnode, WFedgeLabel> connectionGraph = new Graph<WFnode, WFedgeLabel>();        

        /// <summary>
        /// Initial node of the workflow.
        /// </summary>
        private WFnode initialNode = null;

        /// <summary>
        /// Set of final node of workflow.
        /// </summary>
        private HashSet<WFnode> finalNodes = new HashSet<WFnode>();
                

        #region IEditableWorkflow Members
        /// <summary>
        /// Adds the edge edge between nodes from and to. Throws ArgumentException if a node doesnt'exist.
        /// </summary>
        /// <param name="edge">the edge to add</param>
        /// <param name="from">the starting node of edge</param>
        /// <param name="to">the target node of edge</param>
    
        public void AddEdge(WFedgeLabel edge,WFnode from,WFnode to)
        {
            if(!(connectionGraph.ContainsNode(from) && connectionGraph.ContainsNode(to)))
                throw new ArgumentException("Starting/Target node not inserted in workflow or removed.");
                                    
            connectionGraph.AddEdge(from,to,edge);

            edge.EdgeModified += ed_EdgeModified;
                        
            string info;
            if ((info = informativeIsValid()) != null)
                OnWorkflowInvalidationEvent(new WorkflowValidationEventArgs(info,false));
        }
        /// <summary>
        /// Remove an edge from the workflow.
        /// </summary>
        /// <param name="id">the edge to remove</param>
        public void RemoveEdge(WFedgeLabel id)
        {
            var rs = connectionGraph.GetConnectingNodes(id);
            connectionGraph.RemoveEdge(rs.SecondMember, rs.ThirdMember);

	        id.EdgeModified -= ed_EdgeModified;

            string info;
            if ((info = informativeIsValid()) != null)
                OnWorkflowInvalidationEvent(new WorkflowValidationEventArgs(info, false));
        }
        /// <summary>
        /// Adds a node to the workflow
        /// </summary>
        /// <param name="nd">the node to add </param>
        public void AddNode(WFnode nd)
        {           
            connectionGraph.AddNode(nd);

            //FIXME: Must check doubled names in types and nodes!!
            
            nd.FieldModified += nd_FieldModified;
            nd.NodeNameModified += nd_NodeNameModified;

            string info;
            if ((info = informativeIsValid()) != null)
                OnWorkflowInvalidationEvent(new WorkflowValidationEventArgs(info, false));

        }
        /// <summary>
        /// Return the collection of nodes
        /// </summary>
        /// <returns>acollection that contains all nodes of the workflow</returns>
        public ICollection<WFnode> GetNodes() {
            return connectionGraph.Nodes;
        }
        /// <summary>
        /// Removes  a node from the workflow
        /// </summary>
        /// <param name="id">the node that will be removed</param>
        public void RemoveNode(WFnode id)
        {
            connectionGraph.RemoveEdgesANDNode(id);

            id.FieldModified -= nd_FieldModified;
            id.NodeNameModified -= nd_NodeNameModified;

            string info;
            if ((info = informativeIsValid()) != null)
                OnWorkflowInvalidationEvent(new WorkflowValidationEventArgs(info, false));
        }

        /// <summary>
        /// Returns a string that contains the XML representation of the WF
        /// </summary>
        /// <returns>the string </returns>
        public string GetXMLDocument()
        {
            
            StringBuilder str = new StringBuilder();
           
            str.Append("<WORKFLOW name=\""+workflowName+'"'+'>');
            XmlDocument edg = GetEdgesForWFE();

            //archi
            str.Append(edg.DocumentElement.OuterXml);

            //nodi
            str.Append("<NODES>");
            string app;
            int i=0,offset=0;
            foreach (WFnode nd in connectionGraph.Nodes)
            {
                str.Append('<'+XmlConvert.EncodeLocalName(nd.Name)+'>');
                
                string [] starr=Utils.WriteSchemaSet(nd.GetNodeSchemasWithoutBaseTypes());                
                
                app = starr[0];
                str.Append("<SCHEMA><EXTENDEDTYPES>");
                i = 0;
                offset = 0;
                while ((i = app.IndexOf("<xs:complexType name=",i)) != -1)
                {
                    str.Append(app.Substring(i, 16));//appendo xs:complextype
                    i += 16;
                    str.Append(" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" ");
                    offset = app.IndexOf("<xs:complexType name=", i);
                    if (offset == -1)
                    {
                        // non ci sono + occorrenze di complexType, quindi copio tutta la stringa
                        str.Append(app.Substring(i,app.Length-i-12));
                    }
                    else
                    {
                        str.Append(app.Substring(i, offset - i));
                    }

                }
                
                
                str.Append("</EXTENDEDTYPES>");
               
                str.Append("<NODETYPE>");
                app = starr[1];
                i = 0;
                offset = 0;

                while ((i = app.IndexOf("<xs:complexType name=", i)) != -1)
                {
                    offset = app.IndexOf('>', i);
                    str.Append(app.Substring(i, offset-i));//appendo xs:complextype name="..."
                    i = offset;
                    str.Append(" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"");
                    offset = app.IndexOf("<xs:complexType name=", i);//cerco il tag complextype successivo
                    if (offset == -1)
                    {
                        // non ci sono + occorrenze di complexType, quindi copio tutta la stringa
                        str.Append(app.Substring(i, app.Length - i - 12));
                    }
                    else
                    {
                        str.Append(app.Substring(i, offset - i));
                    } 
                }
                
                str.Append("</NODETYPE></SCHEMA>");
                XmlDocument dd = nd.GetRenderingDocument();
                str.Append(dd.DocumentElement.OuterXml);               
                str.Append("</" + XmlConvert.EncodeLocalName(nd.Name) + '>');
            }

            str.Append("</NODES>");
            str.Append("</WORKFLOW>");
            return str.ToString();
        }
        /// <summary>
        /// Mark the node id as the InitialNode of the workflow.Throws ArgumentException if the node doesn't exists
        /// </summary>
        /// <param name="id">the node</param>
        /// <returns></returns>
        public WFnode MarkAsInitialNode(WFnode id)
        {
            if (!connectionGraph.ContainsNode(id))
                throw new ArgumentException("Node doesn't exist");
            WFnode tmp = initialNode;
            initialNode = id;
            return tmp;
        }

        [Obsolete]
        public void MarkAsFinalNode(WFnode id)
        {
            if (!connectionGraph.ContainsNode(id))
                throw new ArgumentException("Node not in the workflow");
            finalNodes.Add(id);
        }
        /// <summary>
        /// Checks if there are the intial node and the final nodes
        /// </summary>
        /// <returns>True there are the intial node and the final nodes. False otherwise </returns>
        private bool findInitialAndFinalNodes()
        {
            initialNode = null;
            finalNodes.Clear();
            foreach (WFnode nd in connectionGraph.Nodes)
            {
                if (connectionGraph.GetOutgoingEdges(nd).Count == 0)
                {
                    finalNodes.Add(nd);                    
                }
                if (connectionGraph.GetIncomingEdges(nd).Count == 0)
                {
                    if(initialNode!=null)
                        return false;
                    else
                        initialNode = nd;
                }
            }
            if (!(initialNode != null && finalNodes.Count > 0))
                return false;
            return true;
        }
        /// <summary>
        /// Checks if the workflow is valid
        /// </summary>
        /// <returns>true if the workflow is valid. false Otherwise</returns>
        public bool IsValid()
        {
            string err = informativeIsValid();
            Debug.WriteLine(err);
            return err == null;
        }
        
        private string informativeIsValid()
        {
            /*Every node is valid...itself*/
            foreach (WFnode wnd in connectionGraph.Nodes)
            {
                if (!wnd.isValid)
                {
                    try
                    {
                        return "The editor you are using pass me some invalid datas for node " + wnd.Name;
                    }
                    catch
                    {
                        return "The editor you are using pass me some invalid datas for a node";
                    }
                }
            }
            
            if(!findInitialAndFinalNodes())
                return "No Initial node or no recognized final nodes in workflow";
            
            /*Initial should be really initial*/
            if (connectionGraph.GetIncomingEdges(initialNode).Count != 0)
                return "Initial node has incoming edges";

            /*Each node should be reached from initial node*/
            Stack<WFnode> stack = new Stack<WFnode>();
            HashSet<WFnode> visited = new HashSet<WFnode>();
            HashSet<WFnode> nodeInStack = new HashSet<WFnode>();

            stack.Push(initialNode);
            nodeInStack.Add(initialNode);
            visited.Add(initialNode);
  
            WFnode popped=null;
            while (stack.Count > 0)
            {
                WFnode top = stack.Peek();
                List<WFnode> lst = connectionGraph.getOutcomingNodes(top);
                if (lst.Count > 0)
                {
                    if (popped != null)
                    {
                        int num = lst.IndexOf(popped);
                        if (num + 1 < lst.Count)
                        {
                            if (nodeInStack.Contains(lst[num + 1]))
                                return "Workflow contains cycle";
                            nodeInStack.Add(lst[num + 1]);
                            stack.Push(lst[num + 1]);
                            visited.Add(lst[num + 1]);
                        }
                        else
                        {
                            popped = stack.Pop();
                            nodeInStack.Remove(popped);
                        }
                    }
                    else
                    {
                        if (stack.Contains(lst[0]))
                            return "Workflow contains cycle";
                        nodeInStack.Add(lst[0]);
                        stack.Push(lst[0]);
                        visited.Add(lst[0]);
                    }
                }
                else
                {
                    popped = stack.Pop();
                    nodeInStack.Remove(popped);
                }                
            }

            if (visited.Count != connectionGraph.Nodes.Count)
                return "Not all nodes could be reached from initial node";
                        
            try
            {
                XmlSchemaSet resSchema = new XmlSchemaSet();
                resSchema.Add(Fields.FieldsManager.FieldTypesXSD);
                foreach (WFnode nd in connectionGraph.Nodes)
                {
                    resSchema.Add(nd.GetNodeSchemasWithoutBaseTypes());
                }

                resSchema.Compile();
            }
            catch (Exception err)
            {
                return "Problems with nodes schemas:\n\n" + err.Message;
            }

            /*Verify edge precondition*/
            foreach (WFedgeLabel edg in connectionGraph.Edges)
            {
                var tr = connectionGraph.GetConnectingNodes(edg);
                
                if (!EdgeLabelInterpreter.VerifyPreconditionsCorrectness(tr.SecondMember.GetNodeSchemas(), XmlConvert.EncodeLocalName(tr.SecondMember.NodeTypeName), edg.GetPrecondition().FirstChild))
                    return "The edge between " + XmlConvert.EncodeLocalName(tr.SecondMember.NodeTypeName) + " and " + XmlConvert.EncodeLocalName(tr.ThirdMember.NodeTypeName) + " contains wrong preconditions";
            }            

            return null;
        }

        public IComputableWorkflow Save()
        {
            if (!IsValid())
                throw new InvalidOperationException("Workflow is invalid");
            
            return new ComputableWorkflow(this);
        }
        public IEnumerable<WFedgeLabel> GetOutcoming(WFnode nd)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WFedgeLabel> GetIncoming(WFnode nd)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WFnode> GetConnectedNodes(WFnode nd)
        {
            throw new NotImplementedException();
        }

        public WFedgeLabel GetEdgeBetween(WFnode from, WFnode to)
        {
            return connectionGraph.GetEdgeBetween(from, to);
            
        }

        #endregion
               
        #region IDisposable Members

        public void Dispose()
        {
            foreach (WFnode nd in connectionGraph.Nodes)
            {
                nd.NodeNameModified -= nd_NodeNameModified;
                nd.FieldModified -= nd_FieldModified;
            }
            foreach (WFedgeLabel ed in connectionGraph.Edges)
            {
                ed.EdgeModified -= ed_EdgeModified;
            }
        }

        #endregion

        private void nd_NodeNameModified(object sender, EventArgs e)
        {
            string info;
            if ((info = informativeIsValid()) != null)
                OnWorkflowInvalidationEvent(new WorkflowValidationEventArgs(info, false));
        }

        private void nd_FieldModified(object sender, EventArgs e)
        {
            string info;
            if ((info = informativeIsValid()) != null)
                OnWorkflowInvalidationEvent(new WorkflowValidationEventArgs(info, false));
        }


        void ed_EdgeModified(object sender, EventArgs e)
        {
            string info;
            if ((info = informativeIsValid()) != null)
                OnWorkflowInvalidationEvent(new WorkflowValidationEventArgs(info, false));
        }
    }

    /// <summary>
    /// This class represent the final Workflow document description and value included.
    /// The schema is still to be defined!!! 
    /// The value as represented by the stacking of NodeInstance
    /// </summary>
    [Serializable]
    internal class FinalWFdocument : Stack<WFnode>
    {
        private string name;
        public FinalWFdocument(string name)
        {
            this.name = name;
        }
        public XmlDocument GetDocument() 
        {
            XmlDocument document = new XmlDocument();
            WFnode[] stack = this.ToArray();

            XmlElement nodo = document.CreateElement(name);
            if (stack.Length == 0)
                return document;
            document.AppendChild(document.CreateXmlDeclaration("1.0", "UTF-8", ""));
            for (int i=stack.Length-1; i >= 0; i--)
            {
                nodo.AppendChild(document.ImportNode(stack[i].Value,true));
            }
            document.AppendChild(nodo);
            return document;
        }
        
    }
   
}
