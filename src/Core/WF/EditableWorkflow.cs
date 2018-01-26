using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Core.WF
{
    ///*TO THE WF-EDITOR*/
    //public interface IWFid<T>
    //{
    //    string Save();        
    //}
    //public class WFedgeId:IEquatable<WFedgeId>,IComparable<WFedgeId>,IWFid<WFedgeId>{
    //    private int id;
  
    //    internal WFedgeId(int id){
    //        this.id = id;            
    //    }
    //    public WFedgeId(string val)
    //    {
    //        this.id = int.Parse(val);
    //    }
    //    protected int ID
    //    {
    //        get
    //        {
    //            return id;
    //        }
    //        set
    //        {
    //            id = value;
    //        }
    //    }

    //    #region IComparable<WFedgeId> Members

    //    public int CompareTo(WFedgeId other)
    //    {
    //        return this.id - other.id;
    //    }

    //    #endregion

    //    #region IEquatable<WFedgeId> Members

    //    public bool Equals(WFedgeId other)
    //    {
    //        return other.id == this.id;
    //    }

    //    #endregion

    //    #region IWFid<WFedgeId> Members

    //    public string Save()
    //    {
    //        return id.ToString();
    //    }

    //    #endregion
    //}
    //public class WFnodeId:IComparable<WFnodeId>,IEquatable<WFnodeId>,IWFid<WFnodeId> {
    //    private int id;
        
    //    internal WFnodeId(int id){
    //        this.id = id;
    //    }
    //    public WFnodeId(string val)
    //    {
    //        this.id = int.Parse(val);
    //    }
    //    protected int ID
    //    {
    //        get
    //        {
    //            return id;
    //        }
    //        set
    //        {
    //            id = value;
    //        }
    //    }

    //    #region IEquatable<WFnodeId> Members

    //    public bool Equals(WFnodeId other)
    //    {
    //        return other.id == this.id;
    //    }

    //    #endregion

    //    #region IComparable<WFnodeId> Members

    //    public int CompareTo(WFnodeId other)
    //    {
    //        return this.id - other.id;
    //    }

    //    #endregion

    //    #region IWFid<WFnodeId> Members

    //    public string Save()
    //    {
    //        return id.ToString();
    //    }

    //    #endregion
    //}
    public interface IEditableWorkflow
    {
        void AddEdge(WFedgeLabel edge, WFnode from, WFnode to);
        void RemoveEdge(WFedgeLabel id);
        void AddNode(WFnode nd);
        void RemoveNode(WFnode id);

        IEnumerable<WFedgeLabel> GetOutcoming(WFnode nd);
        IEnumerable<WFedgeLabel> GetIncoming(WFnode nd);
        IEnumerable<WFnode> GetConnectedNodes(WFnode nd);
        IEnumerable<WFedgeLabel> GetEdgeBetween(WFnode from, WFnode to);
        
        /// <summary>
        /// Marks the node id as initial node and returns the id of the old one or null if that node has been removed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The old one's id if it is still in the Wf</returns>
        WFnode MarkAsInitialNode(WFnode id);
        void MarkAsFinalNode(WFnode id);
        /// <summary>
        /// Check if the workflow is a valid workflow:
        /// A connected graph.
        /// </summary>
        /// <returns></returns>
        bool IsValid();
        /// <summary>
        /// Save the edited workflow in a IComputable workflow suitable for form filling.
        /// If the workflow is not valid throws an Exception.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the workflow isn't in a legal state.</exception>
        /// <returns></returns>
        IComputableWorkflow Save();

    }
}
