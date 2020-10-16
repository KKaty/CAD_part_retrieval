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
    using System.Linq;
    /// <summary>
    /// The plane node.
    /// </summary>
    [Serializable()]
    public class PlaneNode : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaneNode"/> class.
        /// </summary>
        /// <param name="normal">
        /// The normal.
        /// </param>
        /// <param name="point">
        /// The point.
        /// </param>
        /// <param name="idNode">
        /// The id Node.
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
        /// <param name="realLink">
        /// The realLink.
        /// </param>
        /// <param name="virtualLink">
        /// The virtualLink.
        /// </param>
        /// 

        public PlaneNode() { }

        public PlaneNode(double[] normal, double[] point, int idNode, int numOfLoops, int numOfEdges, double[] boundParameters, bool faceSense, List<RealLink> realLink, List<VirtualLink> virtualLink)
            : base(idNode, numOfLoops, numOfEdges, boundParameters, faceSense, realLink, virtualLink)
        { 
            this.Normal = normal;
            this.Point = point;
            this.Equation = new double[4] {(double)normal.GetValue(0), (double)normal.GetValue(1), (double)normal.GetValue(2),
        -(double)normal.GetValue(0)*(double)point.GetValue(0) - (double)normal.GetValue(1)*(double)point.GetValue(1) - (double)normal.GetValue(2)*(double)point.GetValue(2)};
        }

        /// <summary>
        /// Gets or sets the normal.
        /// </summary>
        public double[] Normal { get; set; }

        /// <summary>
        /// Gets or sets the root point.
        /// </summary>
        public double[] Point { get; set; }

        /// <summary>
        /// Gets or sets the equarion of the plain.
        /// </summary>
        private double[] Equation { get; set; }

        protected bool Equals(PlaneNode other)
        {
            return base.Equals(other) && Equals(this.Normal, other.Normal) && Equals(this.Point, other.Point);
        }

        public bool IsSamePlane(PlaneNode other)
        {
            return this.Equation.SequenceEqual(other.Equation);
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
            return Equals((PlaneNode)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.Normal != null ? this.Normal.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Point != null ? this.Point.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
