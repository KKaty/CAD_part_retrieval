// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TorusNode.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the TorusNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace SWIntegration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The torus node.
    /// </summary>
    public class TorusNode : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TorusNode"/> class.
        /// </summary>
        /// <param name="axis">
        /// The axis.
        /// </param>
        /// <param name="radiusMin">
        /// The radius min.
        /// </param>
        /// <param name="radiusMax">
        /// The radius max.
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
        public TorusNode(Array axis, double radiusMin, double radiusMax, int numOfLoops, int numOfEdges, Array boundParameters, bool faceSense, List<AdiacenceNode> adjacents)
            : base(numOfLoops, numOfEdges, boundParameters, faceSense, adjacents)
        {
            this.Axis = axis;
            this.RadiusMax = radiusMax;
            this.RadiusMin = radiusMin;
        }

        /// <summary>
        /// Gets or sets the axis.
        /// </summary>
        public Array Axis { get; set; }

        /// <summary>
        /// Gets or sets the radius min.
        /// </summary>
        public double RadiusMin { get; set; }

        /// <summary>
        /// Gets or sets the radius max.
        /// </summary>
        public double RadiusMax { get; set; }
    }
}
