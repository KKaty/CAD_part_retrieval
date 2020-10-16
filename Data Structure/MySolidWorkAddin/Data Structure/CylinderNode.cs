// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CylinderNode.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the CylinderNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace SWIntegration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The cylinder node.
    /// </summary>
    public class CylinderNode : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CylinderNode"/> class.
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="axis">
        /// The axis.
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
      
        public CylinderNode(Array origin, Array axis, double radius, int numOfLoops, int numOfEdges, Array boundParameters, bool faceSense, List<AdiacenceNode> adjacents)
            : base(numOfLoops, numOfEdges, boundParameters, faceSense, adjacents)
        {
            this.Origin = origin;
            this.Axis = axis;
            this.Radius = radius;
        }

        /// <summary>
        /// Gets or sets the axis.
        /// </summary>
        public Array Axis { get; set; }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        public Array Origin { get; set; }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        public double Radius { get; set; }
    }
}
