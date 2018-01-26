using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;


namespace Core.WF
{
    /// <summary>
    /// Graph is a rapresentation of a workflow
    /// </summary>
    /// <typeparam name="NodeType">The raw type of nodes</typeparam>
    /// <typeparam name="EdgeLabelType">The raw type of edges</typeparam>
    [Serializable]
    internal class Graph<NodeType, EdgeLabelType>:ICloneable
        where NodeType : class
        where EdgeLabelType : class
    {

        /// <summary>
        /// The dictionary 
        /// </summary>
        Dictionary<NodeType, int> nodeToMatrixPos = new Dictionary<NodeType, int>();
        Dictionary<int,NodeType> matrixPosToNode = new Dictionary<int,NodeType>();
        /// <summary>
        /// The size of graph
        /// </summary>
        int N = 20;
        /// <summary>
        /// the number of node in the graph
        /// </summary>
        int actualcount = 0;
        /// <summary>
        /// The matrix that represents the graph.
        /// </summary>
        EdgeLabelType[,] matrix = new EdgeLabelType[20,20];

        /// <summary>
        /// Adds a new node to the graph. Throws ArgumentException if node already exists.
        /// </summary>
        /// <param name="nd">The node to add</param>
        public void AddNode(NodeType nd)
        {
            if (nodeToMatrixPos.ContainsKey(nd))
                throw new ArgumentException("Node already exists");
            if (actualcount >= N)
            {
                N = N * 2;
                EdgeLabelType[,] newmat = new EdgeLabelType[N , N ];
                for (int i = 0; i < actualcount; i++)
                    for (int j = 0; j < actualcount; j++)
                        newmat[i, j] = matrix[i, j];

                matrix = newmat;
            }

            nodeToMatrixPos.Add(nd, actualcount);
            matrixPosToNode.Add(actualcount++, nd);

            
            for (int i = 0; i < actualcount; i++) 
                matrix[actualcount,i]=null;

            for (int i = 0; i < actualcount; i++)
                matrix[i, actualcount] = null;
                        
        }
        /// <summary>
        /// Adds an edge to the graph. Throws ArgumentException if a node doesn't exists or the edge already exists.
        /// </summary>
        /// <param name="ndFrom">the node where the edge starts </param>
        /// <param name="ndTo">the node where the edge arrives</param>
        /// <param name="edgeLabel">the edge that will connect ndFrom and ndTo</param>
        public void AddEdge(NodeType ndFrom, NodeType ndTo, EdgeLabelType edgeLabel)
        {
            if (!nodeToMatrixPos.ContainsKey(ndFrom) || !nodeToMatrixPos.ContainsKey(ndTo))
                throw new ArgumentException("A node doesn't exist");
            int ndFrmIdx = nodeToMatrixPos[ndFrom];
            int ndToIdx = nodeToMatrixPos[ndTo];

            if (matrix[ndFrmIdx, ndToIdx] != null)
                throw new ArgumentException("An edge already exists");

            matrix[ndFrmIdx,ndToIdx] = edgeLabel;

        }
        /// <summary>
        /// Removes the edge tha starts to ndFrom and arrives to ndTo. Throws ArgumentException if a node or the edge doesn't exists.
        /// </summary>
        /// <param name="ndFrom">the node where the edge starts</param>
        /// <param name="ndTo">the node where the edge arrives</param>
        public void RemoveEdge(NodeType ndFrom, NodeType ndTo)
        {
            if (!nodeToMatrixPos.ContainsKey(ndFrom) || !nodeToMatrixPos.ContainsKey(ndTo))
                throw new ArgumentException("A node doesn't exist");
            int ndFrmIdx = nodeToMatrixPos[ndFrom];
            int ndToIdx = nodeToMatrixPos[ndTo];

            if (matrix[ndFrmIdx,ndToIdx] == null)
                throw new ArgumentException("The edge doesn't exist");

            matrix[ndFrmIdx,ndToIdx] = null;

        }

       /// <summary>
        /// Remove the node and all his edges.Throws ArgumentException if nd doesn't exists
       /// </summary>
       /// <param name="node">the node to remove</param>
        public void RemoveEdgesANDNode(NodeType nd)
        {
            
            if (!nodeToMatrixPos.ContainsKey(nd))
                throw new ArgumentException("Node doesn't exists");
            int pos = nodeToMatrixPos[nd];
            NodeType app;
            int n;
            nodeToMatrixPos.Remove(nd);
            matrixPosToNode.Remove(pos);

            for (int i = 0; i < actualcount ; i++)
            {
                if (i != pos)
                {
                    for (int j = pos + 1; j < actualcount; j++)
                    {
                        matrix[i, j - 1] = matrix[i, j];//aggiorno le colonne
                    }
                }
                if (i > pos)
                {
                    //aggiorno i dizionari per i nodi che cambiano posizione nella matrice
                    app = matrixPosToNode[i];
                    matrixPosToNode.Remove(i);
                    matrixPosToNode.Add(i - 1, app);
                    n = nodeToMatrixPos[app];
                    nodeToMatrixPos.Remove(app);
                    nodeToMatrixPos.Add(app, i - 1);

                    for (int j = 0; j < actualcount - 1; j++)//actualcount -1 perchè gli elementi nelle colonne sono già stati spostati
                    {
                        matrix[i - 1, j] = matrix[i, j];//aggiorno la riga
                    }
                }
            }
            actualcount--;


        }

        /// <summary>
        /// Gives the edge from ndFrom to ndTo. Throws ArgumentException if a node or the edge doesn't exists.
        /// </summary>
        /// <param name="ndFrom">the node where the edge starts</param>
        /// <param name="ndTo">the node where the edge arrives</param>
        /// <returns>the edge between ndFrom and ndTo</returns>
        public EdgeLabelType GetEdge(NodeType ndFrom, NodeType ndTo)
        {
            if (!nodeToMatrixPos.ContainsKey(ndFrom) || !nodeToMatrixPos.ContainsKey(ndTo))
                throw new ArgumentException("A node doesn't exist");
            int ndFrmIdx = nodeToMatrixPos[ndFrom];
            int ndToIdx = nodeToMatrixPos[ndTo];

            if (matrix[ndFrmIdx,ndToIdx] == null)
                throw new ArgumentException("The edge doesn't exist");

            return matrix[ndFrmIdx,ndToIdx];

        }
        /// <summary>
        /// Removes the node nd from the graph if and only if it is not connected. Throws ArgumentException if nd doesn't exists or the node is connected to another node.
        /// </summary>
        /// <param name="nd">The node that will be removed</param>
        public void RemoveNode(NodeType nd) 
        {
            if (!nodeToMatrixPos.ContainsKey(nd))
                throw new ArgumentException("Node doesn't exists");
            int pos = nodeToMatrixPos[nd];
           
            for (int i = 0; i < actualcount; i++)
                if (matrix[i, pos] != null || matrix[pos, i] != null)
                    throw new ArgumentException("The node is connected");
            
            RemoveEdgesANDNode(nd);


            //NodeType app;
            //int n;
            //nodeToMatrixPos.Remove(nd);
            //matrixPosToNode.Remove(pos);
            
            //    for (int i = 0; i < actualcount - 1; i++)
            //    {                    
            //        if (i !=pos)
            //        {
            //            for (int j = pos + 1; j < actualcount; j++)
            //            {
            //                matrix[i, j - 1] = matrix[i, j];//aggiorno le colonne
            //            }
            //        }
            //        if (i > pos)
            //        {
            //            //aggiorno i dizionari per i nodi che cambiano posizione nella matrice
            //            app = matrixPosToNode[i];
            //            matrixPosToNode.Remove(i);
            //            matrixPosToNode.Add(i - 1, app);
            //            n = nodeToMatrixPos[app];
            //            nodeToMatrixPos.Remove(app);
            //            nodeToMatrixPos.Add(app, i - 1);

            //            for (int j =0 ; j < actualcount-1; j++)
            //            {
            //                matrix[i-1, j ] = matrix[i, j];//aggiorno la riga
            //            }
            //        }
            //    }
            //    if (actualcount - 1 < (3 * N / 4))
            //    {
            //        EdgeLabelType[,] newmat = new EdgeLabelType[actualcount, actualcount];
            //        for (int i = 0; i < actualcount; i++)
            //            for (int j = 0; j < actualcount; j++)
            //            {
            //                newmat[i, j] = matrix[i, j];
            //            }
            //    }
            //actualcount--;

            
        }

        /// <summary>
        /// Return a list of all incoming edges of node.Throws ArgumentException if a node doesn't exists
        /// </summary>
        /// <param name="node">the node</param>
        /// <returns>the list of incoming nodes</returns>
        public List<EdgeLabelType> GetIncomingEdges(NodeType node)
        {

            if (!nodeToMatrixPos.ContainsKey(node))
                throw new ArgumentException("Node doesn't exist");

            List<EdgeLabelType> list = new List<EdgeLabelType>();
            int ndId = nodeToMatrixPos[node];
            
            for (int i = 0; i < N;i++ )
            {
                    if (matrix[i,ndId] != null)
                    list.Add(matrix[i,ndId]);
            }
           return list;
        }
        /// <summary>
        /// Return a list of all outcoming edges of node.Throws ArgumentException if a node doesn't exists
        /// </summary>
        /// <param name="node">the node</param>
        /// <returns>the list of outcoming nodes</returns>
        public List<EdgeLabelType> GetOutgoingEdges(NodeType node)
        {
            if (!nodeToMatrixPos.ContainsKey(node))
                throw new ArgumentException("Node doesn't exist");
            int ndId = nodeToMatrixPos[node];

            List<EdgeLabelType> list = new List<EdgeLabelType>();
            for (int i = 0; i < N; i++)
            {
                if (matrix[ndId, i] != null)
                    list.Add(matrix[ndId, i]);
            }
            return list;
        }
        /// <summary>
        /// Returns a triple that contains the edge es and the two nodes that this edge connect.
        /// </summary>
        /// <param name="ed">the edge</param>
        /// <returns>a triple like (ed,nd1,nd2) if ed goes from nd1 to nd2. null if ed don't connect anything></returns>
        public Triple<EdgeLabelType, NodeType, NodeType> GetConnectingNodes(EdgeLabelType ed)
        {
            for (int i = 0; i < actualcount; i++)
                if(matrixPosToNode.ContainsKey(i))
                    for (int j = 0; j < actualcount; j++)
                        if (matrix[i, j] != null && matrix[i, j].Equals(ed))
                           return new Triple<EdgeLabelType, NodeType, NodeType>(ed, matrixPosToNode[i], matrixPosToNode[j]);
            return null;
        }
        /// <summary>
        /// return the list of node from which i can reach nd.Throws ArgumentException, if nd doesn't exists or ArgumentNullException il nd is null.
        /// </summary>
        /// <param name="nd">the node</param>
        /// <returns>the list of nodes</returns>
        public List<NodeType> getIncomingNodes(NodeType nd)
        {
            if(!nodeToMatrixPos.ContainsKey(nd))
                throw new ArgumentException("Node doesn't exixts");

            int posNd = nodeToMatrixPos[nd];
            List<NodeType> lis = new List<NodeType>();
            for (int i = 0; i < actualcount; i++)
                if (matrix[i, posNd] != null)
                    lis.Add(matrixPosToNode[i]);
            return lis;
        }
        /// <summary>
        /// Return the list of reachable nodes from nd. Throws ArgumentException, if nd doesn't exists or ArgumentNullException il nd is null.
        /// </summary>
        /// </summary>
        /// <param name="nd">the node</param>
        /// <returns>the list of nodes</returns>
        public List<NodeType> getOutcomingNodes(NodeType nd)
        {
            if (!nodeToMatrixPos.ContainsKey(nd))
                throw new ArgumentException("Node doesn't exixts");

            int posNd = nodeToMatrixPos[nd];
            List<NodeType> lis = new List<NodeType>();
            for (int i = 0; i < actualcount; i++)
                if (matrix[posNd,i] != null)
                    lis.Add(matrixPosToNode[i]);
            return lis;
        }
        /// <summary>
        /// Returns the edge between from and to. Throws ArgumentException if from or to doesn't exist
        /// </summary>
        /// <param name="from">the first node</param>
        /// <param name="to">the second node</param>
        /// <returns>the edge, or null if there isn't edge between from and to</returns>
        public EdgeLabelType GetEdgeBetween(NodeType from, NodeType to)
        {
            if (!nodeToMatrixPos.ContainsKey(from) || !nodeToMatrixPos.ContainsKey(to))
            {
                throw new ArgumentException("from or to Nodes are not in Graph");
            }

            return matrix[nodeToMatrixPos[from], nodeToMatrixPos[to]];

        }
        /// <summary>
        /// Returns the node that we will reach from actualState passing trough edgeIdToEval
        /// </summary>
        /// <param name="actualState">the node that represents the acuat state in the graph</param>
        /// <param name="edgeIdToEval">the edge to pass trough</param>
        /// <returns>the node found or null if there it can't reach te node</returns>
        internal NodeType GetDestination(NodeType actualState, EdgeLabelType edgeIdToEval)
        {
            int posNd = nodeToMatrixPos[actualState];
            int i=0;
            int found = -1;
            while (i < actualcount && found==-1)
            {
                if ( matrix[posNd, i]!=null && matrix[posNd, i].Equals(edgeIdToEval))
                    found = i;
                i++;
            }
            if (found == -1)
                return null;
            else
                return matrixPosToNode[found];
        }
        /// <summary>
        /// Checks if the graph contains nd
        /// </summary>
        /// <param name="nd">the node</param>
        /// <returns>true if the graph contains nd </returns>
        internal bool ContainsNode(NodeType nd)
        {
            return nodeToMatrixPos.ContainsKey(nd);
        }
        /// <summary>
        /// Checks if the graph contains edg
        /// </summary>
        /// <param name="nd">the node</param>
        /// <returns>true if the graph contains nd </returns>
        internal bool ContainsEdge(EdgeLabelType edg)
        {
            return (this.GetConnectingNodes(edg) != null);
        }
       
        internal ICollection<EdgeLabelType> Edges
        {
            get
            {
                List<EdgeLabelType> ls = new List<EdgeLabelType>();
                for (int i = 0; i < actualcount; i++)
                    if (matrixPosToNode.ContainsKey(i))
                        for (int j = 0; j < actualcount; j++)
                            if (matrix[i, j] != null)
                                ls.Add(matrix[i, j]);
                return ls;
            }
                
        }

        internal ICollection<NodeType> Nodes
        {
            get
            {
                return nodeToMatrixPos.Keys;
            }
        }

        #region ICloneable Members

        public object Clone()
        {
            Graph<NodeType, EdgeLabelType> g = new Graph<NodeType, EdgeLabelType>();
            Utils.DeepCloneObject(this, g);
            return g;
        }

        #endregion
    }   


    [TestFixture]
    public class GraphTest
    {
        [Test]
        public void TestSerialization()
        {
            Graph<String, String> graph = new Graph<string, string>();
            graph.AddNode("Pippo");
            graph.AddNode("Pluto");
            graph.AddEdge("Pippo", "Pluto", "Miao");

            // SoapFormatter sf = new SoapFormatter();
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            MemoryStream ms2 = new MemoryStream();
            //sf.Serialize(ms, graph);
            bf.Serialize(ms, graph);

            byte[] ris = ms.ToArray();

        }

        [Test]
        public void TestGraph()
        {
            Graph<String, String> graph = new Graph<string, string>();
            graph.AddNode("Nodo1");
            graph.AddNode("Nodo2");
            graph.AddNode("Nodo3");
            graph.AddEdge("Nodo1", "Nodo2", "Arco1");
            graph.AddEdge("Nodo3", "Nodo2", "Arco2");
            graph.AddEdge("Nodo1","Nodo3","Arco3");
            Assert.IsTrue(graph.ContainsEdge("Arco2"));
            Assert.IsFalse(graph.ContainsNode("Arco4"));
            Assert.IsTrue(graph.GetDestination("Nodo3", "Arco2").Equals("Nodo2"));

            List<String> l1 = graph.getOutcomingNodes("Nodo1");
            Assert.IsTrue(l1[0].Equals("Nodo2") || l1[0].Equals("Nodo3"));
            Assert.IsTrue(l1[1].Equals("Nodo2") || l1[1].Equals("Nodo3"));
            List<String> l2 = new List<string>();
            l2.Add("Nodo1");
            l2.Add("Nodo2");
            l2.Add("Nodo3");
        }
    }
}