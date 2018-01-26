using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.WF
{
    /// <summary>
    /// Defines a triple with three raw types.
    /// </summary>
    /// <typeparam name="T">a raw type</typeparam>
    /// <typeparam name="U">a raw type</typeparam>
    /// <typeparam name="V">a raw type</typeparam>
    /*
     * 090422 Changed to public, it's used also by WebInterface.WorkflowEditor.action.WFF_addField
     */ 
    public class Triple<T, U, V>
    {

        private T first;
        private U second;
        private V third;
        /// <summary>
        /// Creates a new triple with arg1,arg2,arg3
        /// </summary>
        /// <param name="arg1">the first element of triple</param>
        /// <param name="arg2">the second element of triple</param>
        /// <param name="arg3">the third element of triple</param>
        public Triple(T arg1, U arg2, V arg3)
        {
            first = arg1;
            second = arg2;
            third = arg3;
        }
        /// <summary>
        /// Returns the first member of the triple
        /// </summary>
        public T FirstMember
        {
            get { return first; }
        }
        /// <summary>
        /// Returns the second member of the triple
        /// </summary>
        public U SecondMember
        {
            get { return second; }
        }
        /// <summary>
        /// Returns the third member of the triple
        /// </summary>
        public V ThirdMember
        {
            get { return third; }
        }
    }
}
