// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Node.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Node type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace SWIntegration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The node.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="numOfLoops">
        /// The num of loops.
        /// </param>
        /// <param name="numOfEdges">
        /// The num of edges.
        /// </param>
        /// <param name="boundParameters">
        /// The bound parameters.
        /// </param>
        /// <param name="faceSense">
        /// The face Sense.
        /// </param>
        /// <param name="adjacents">
        /// The adjacents.
        /// </param>
        public Node(int numOfLoops, int numOfEdges, Array boundParameters, bool faceSense, List <AdiacenceNode> adjacents)
        {
            this.NumOfLoops = numOfLoops;
            this.NumOfEdges = numOfEdges;
            this.BoundParameters = boundParameters;
            this.FaceSense = faceSense;
            this.Adjacents = adjacents;
        }

        /// <summary>
        /// Gets or sets the num of loops.
        /// </summary>
        public int NumOfLoops { get; set; }

        /// <summary>
        /// Gets or sets the num of edges.
        /// </summary>
        public int NumOfEdges { get; set; }

        /// <summary>
        /// Gets or sets the bound parameters.
        /// </summary>
        public Array BoundParameters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether face sense.
        /// </summary>
        public bool FaceSense { get; set; }

        /// <summary>
        /// Gets or sets the adjacents.
        /// </summary>
        public List<AdiacenceNode> Adjacents { get; set; }

    }
}
