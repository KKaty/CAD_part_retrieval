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
    [Serializable()]
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

        public SphereNode() { }
        public SphereNode(double[] center, double radius, int idNode, int numOfLoops, int numOfEdges, double[] boundParameters, bool faceSense, List<RealLink> realLink, List<VirtualLink> virtualLink)
            : base(idNode, numOfLoops, numOfEdges, boundParameters, faceSense, realLink, virtualLink)
        {
            this.Center = center;
            this.Radius = radius;
        }

        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        public double[] Center { get; set; }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        public double Radius { get; set; }

        protected bool Equals(SphereNode other)
        {
            return base.Equals(other) && Equals(this.Center, other.Center) && this.Radius.Equals(other.Radius);
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
            return Equals((SphereNode)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.Center != null ? this.Center.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.Radius.GetHashCode();
                return hashCode;
            }
        }
    }
}
