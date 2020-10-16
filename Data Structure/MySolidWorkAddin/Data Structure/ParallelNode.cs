// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParallelNode.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ParallelNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SWIntegration.Data_Structure
{
    /// <summary>
    /// The parallel node.
    /// </summary>
    public class ParallelNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelNode"/> class.
        /// </summary>
        /// <param name="lenght">
        /// The lenght.
        /// </param>
        /// <param name="parallelFace">
        /// The parallel face.
        /// </param>
        public ParallelNode(double lenght, Node parallelFace)
        {
            this.Lenght = lenght;
            this.ParallelFace = parallelFace;
        }

        /// <summary>
        /// Gets or sets the lenght.
        /// </summary>
        public double Lenght { get; set; }

        /// <summary>
        /// Gets or sets the parallel face.
        /// </summary>
        public Node ParallelFace { get; set; }
    }
}

