// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Graph.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Graph type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SWIntegration.Data_Structure
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    

    /// <summary>
    /// The graph.
    /// </summary>
    /// 
    [Serializable()]
    public class Graph 
    {
        public Graph()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Graph"/> class.
        /// </summary>
        /// <param name="nodes">
        /// The nodes.
        /// </param>
        public Graph(List<Node> nodes)
        {
            this.Nodes = nodes;
        }

        /// <summary>
        /// Gets or sets the nodes.
        /// </summary>
        /// 
        public List<Node> Nodes { get; set; }

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

            return this.Equals((Graph)obj);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Nodes != null ? this.Nodes.GetHashCode() : 0;
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
        private bool Equals(Graph other)
        {
            return Equals(this.Nodes, other.Nodes);
        }

        public void Add(string _value)
        {
        }
    }
}
