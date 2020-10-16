// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssociativeNode.cs" company="">
//   
// </copyright>
// <summary>
//   The associative node.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SWIntegration.Data_Structure
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;

    using Accord.Math;

    /// <summary>
    /// The associative node.
    /// </summary>
    [Serializable()]
    public class AssociativeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssociativeNode"/> class.
        /// </summary>
        /// <param name="nodeFirstGraph">
        /// The node first graph.
        /// </param>
        /// <param name="nodeSecondGraph">
        /// The node second graph.
        /// </param>
        /// <param name="associativeLinks">
        /// The associative Links.
        /// </param>
        /// <param name="visitato">
        /// The visitato.
        /// </param>
        public AssociativeNode(Node nodeFirstGraph, Node nodeSecondGraph, List<AssociativeNode> associativeLinks, bool Visitato = false)
        {
            this.NodeFirstGraph = nodeFirstGraph;
            this.NodeSecondGraph = nodeSecondGraph;
            this.AssociativeLinks = associativeLinks;
     
        }

        /// <summary>
        /// Gets or sets the node first graph.
        /// </summary>
        public Node NodeFirstGraph { get; set; }

        /// <summary>
        /// Gets or sets the node second graph.
        /// </summary>
        public Node NodeSecondGraph { get; set; }

        /// <summary>
        /// Gets or sets the associative links.
        /// </summary>
        public List<AssociativeNode> AssociativeLinks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether visitato.
        /// </summary>
        public bool Visitato { get; set; }

        protected bool Equals(AssociativeNode other)
        {
            return this.NodeFirstGraph.Equals(other.NodeFirstGraph) && this.NodeSecondGraph.Equals(other.NodeSecondGraph);
        }

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
            return Equals((AssociativeNode)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.NodeFirstGraph.GetHashCode() * 397) ^ this.NodeSecondGraph.GetHashCode();
            }
        }
    }
}
