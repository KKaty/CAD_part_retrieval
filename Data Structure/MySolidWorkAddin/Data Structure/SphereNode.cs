// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SphereNode.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the SphereNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace SWIntegration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The sphere node.
    /// </summary>
    public class SphereNode : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SphereNode"/> class.
        /// </summary>
        /// <param name="center">
        /// The center.
        /// </param>
        /// <param name="radius">
        /// The radius.
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
        public SphereNode(Array center, double radius, int numOfLoops, int numOfEdges, Array boundParameters, bool faceSense, List<AdiacenceNode> adjacents)
            : base(numOfLoops, numOfEdges, boundParameters, faceSense, adjacents)
        {
            this.Center = center;
            this.Radius = radius;
        }

        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        public Array Center { get; set; }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        public double Radius { get; set; }
    }
}
