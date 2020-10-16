// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlaneNode.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the PlaneNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace SWIntegration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The plane node.
    /// </summary>
    public class PlaneNode : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaneNode"/> class.
        /// </summary>
        /// <param name="normal">
        /// The normal.
        /// </param>
        /// <param name="rootPoint">
        /// The root point.
        /// </param>
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
        public PlaneNode(Array normal, Array rootPoint, int numOfLoops, int numOfEdges, Array boundParameters, bool faceSense, List<AdiacenceNode> adjacents)
            : base(numOfLoops, numOfEdges, boundParameters, faceSense, adjacents)
        { 
            this.Normal = normal;
            this.RootPoint = rootPoint;
        }

        /// <summary>
        /// Gets or sets the normal.
        /// </summary>
        public Array Normal { get; set; }

        /// <summary>
        /// Gets or sets the root point.
        /// </summary>
        public Array RootPoint { get; set; }
    }
}
