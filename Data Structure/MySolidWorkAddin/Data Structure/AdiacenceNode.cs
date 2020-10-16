// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdiacenceNode.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the AdiacenceNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SWIntegration
{
    using System;

    /// <summary>
    /// The adiacence node.
    /// </summary>
    public class AdiacenceNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdiacenceNode"/> class.
        /// </summary>
        /// <param name="angle">
        /// The angle.
        /// </param>
        /// <param name="lenght">
        /// The lenght.
        /// </param>
        /// <param name="destinationNode">
        /// The destination Node.
        /// </param>
        public AdiacenceNode(double angle, double lenght, Node destinationNode)
        {
            if (destinationNode == null)
            {
                throw new ArgumentNullException("destinationNode");
            }

            this.Angle = angle;
            this.Lenght = lenght;
            this.DestinationNode = destinationNode;
        }

        /// <summary>
        /// Gets or sets the angle.
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// Gets or sets the lenght.
        /// </summary>
        public double Lenght { get; set; }

        /// <summary>
        /// Gets or sets the destination node.
        /// </summary>
        public Node DestinationNode { get; set; }
    }
}
