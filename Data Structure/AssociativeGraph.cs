// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssociativeGraph.cs" company="">
//   
// </copyright>
// <summary>
//   The associative graph creation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SWIntegration.Data_Structure
{
    using System.Collections.Generic;

    /// <summary>
    /// The associative graph creation.
    /// </summary>
    /// 
    public class AssociativeGraph
    {
        /// <summary>
        /// The list node.
        /// </summary>
        public List<AssociativeNode> listNode;

        public List<AssociativeNode> listNodeCopy;
        /// <summary>
        /// Initializes a new instance of the <see cref="AssociativeGraph"/> class.
        /// </summary>
        /// <param name="listNode">
        /// The list Node.
        /// </param>
        public AssociativeGraph(List<AssociativeNode> listNode)
        {
            this.listNode = listNode;
        }

        public AssociativeGraph(AssociativeGraph localCopy)
        {
            this.listNodeCopy = localCopy.listNode;
        }

        // Aggiungo due metodi che restituisco le liste dei nodi del primo e secondo grafo che formano quello associato
        
        public List<Node> getFirstNodeList()
        {
            var nodeFirstGraph = new List<Node>();

            foreach(AssociativeNode assNode in this.listNode)
            {
                nodeFirstGraph.Add(assNode.NodeFirstGraph);
            }

            return nodeFirstGraph;

        }

        public List<Node> getSecondNodeList()
        {
            var nodeSecondGraph = new List<Node>();

            foreach (AssociativeNode assNode in this.listNode)
            {
                nodeSecondGraph.Add(assNode.NodeSecondGraph);
            }

            return nodeSecondGraph;

        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool Equals(AssociativeGraph other)
        {
            return Equals(this.listNode, other.listNode);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((AssociativeGraph)obj);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return (this.listNode != null ? this.listNode.GetHashCode() : 0);
        }
    }
}
