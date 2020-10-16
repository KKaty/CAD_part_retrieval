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
    [Serializable()]
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

        public TorusNode() { }

        public TorusNode(double[] axis, double radiusMin, double radiusMax, int idNode, int numOfLoops, int numOfEdges, double[] boundParameters, bool faceSense, List<RealLink> realLink, List<VirtualLink> virtualLink)
            : base(idNode, numOfLoops, numOfEdges, boundParameters, faceSense, realLink, virtualLink)
        {
            this.Axis = axis;
            this.RadiusMax = radiusMax;
            this.RadiusMin = radiusMin;
        }

        /// <summary>
        /// Gets or sets the axis.
        /// </summary>
        public double[] Axis { get; set; }

        /// <summary>
        /// Gets or sets the radius min.
        /// </summary>
        public double RadiusMin { get; set; }

        /// <summary>
        /// Gets or sets the radius max.
        /// </summary>
        public double RadiusMax { get; set; }

        protected bool Equals(TorusNode other)
        {
            return base.Equals(other) && Equals(this.Axis, other.Axis) && this.RadiusMin.Equals(other.RadiusMin) && this.RadiusMax.Equals(other.RadiusMax);
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
            return Equals((TorusNode)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.Axis != null ? this.Axis.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.RadiusMin.GetHashCode();
                hashCode = (hashCode * 397) ^ this.RadiusMax.GetHashCode();
                return hashCode;
            }
        }
    }
}
