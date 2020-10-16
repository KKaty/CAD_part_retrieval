// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidNode.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the InvalidNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SWIntegration
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    /// <summary>
    /// The invalid node.
    /// </summary>
    public class InvalidNode : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidNode"/> class.
        /// </summary>
        /// <param name="numOfLoop">
        /// The num of loop.
        /// </param>
        /// <param name="numOfEdge">
        /// The num of edge.
        /// </param>
        /// <param name="boundParameters">
        /// The bound parameters.
        /// </param>
        /// <param name="faceSense">
        /// The face Sense.
        /// </param>
        /// <param name="adjacence">
        /// The adjacence.
        /// </param>
        public InvalidNode(int numOfLoop, int numOfEdge, Array boundParameters, bool faceSense, List<AdiacenceNode> adjacence)
            : base(numOfLoop, numOfEdge, boundParameters, faceSense, adjacence)
        {
        }
    }
}
