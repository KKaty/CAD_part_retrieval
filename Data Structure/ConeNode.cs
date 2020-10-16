// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConeNode.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ConeNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SWIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The cone node.
    /// </summary>
    [Serializable()]
    public class ConeNode : Node
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ConeNode"/> class.
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
        /// <param name="angle">
        /// The angle.
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
        /// <param name="senseFace">
        /// The sense Face.
        /// </param>
        /// <param name="realLink">
        /// The realLink.
        /// </param>
        /// <param name="virtualLink">
        /// The virtualLink.
        /// </param>
        /// 
        public ConeNode() { }

        public ConeNode(double[] origin, double[] axis, double radius, double angle, int idNode, int numOfLoops, int numOfEdges, double[] boundParameters, bool senseFace, List<RealLink> realLink, List<VirtualLink> virtualLink)
            : base(idNode, numOfLoops, numOfEdges, boundParameters, senseFace, realLink, virtualLink)
        {
            this.Origin = origin;
            this.Axis = axis;
            this.Radius = radius;
            this.Angle = angle;
        }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        public double[] Origin { get; set; }

        /// <summary>
        /// Gets or sets the axis.
        /// </summary>
        public double[] Axis { get; set; }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Gets or sets the angle.
        /// </summary>
        public double Angle { get; set; }

        protected bool Equals(ConeNode other)
        {
            return base.Equals(other) && Equals(this.Origin, other.Origin) && Equals(this.Axis, other.Axis) && this.Angle.Equals(other.Angle) && this.Radius.Equals(other.Radius);
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
            return Equals((ConeNode)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.Origin != null ? this.Origin.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Axis != null ? this.Axis.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.Angle.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Radius.GetHashCode();
                return hashCode;
            }
        }
    }
}
