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
    [Serializable()]
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

        public CylinderNode() { }
        public CylinderNode(double[] origin, double[] axis, double radius, bool complete, int idNode, int numOfLoops, int numOfEdges, double[] boundParameters, bool faceSense, List<RealLink> realLink, List<VirtualLink> virtualLink)
            : base(idNode, numOfLoops, numOfEdges, boundParameters, faceSense, realLink, virtualLink)
        {
            this.Origin = origin;
            this.Axis = axis;
            this.Radius = radius;
            this.Complete = complete;
        }

        /// <summary>
        /// Gets or sets the axis.
        /// </summary>
        public double[] Axis { get; set; }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        public double[] Origin { get; set; }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Gets or sets if the cylinder is complete or non.
        /// </summary>
        public bool Complete { get; set; }
        
        public bool IsSameCylinder(CylinderNode other)
        {
            double[,] matrix =
            {
                {(double)this.Axis.GetValue(0) + (double)this.Origin.GetValue(0), (double)this.Axis.GetValue(1) + (double)this.Origin.GetValue(1), (double)this.Axis.GetValue(2) + (double)this.Origin.GetValue(2)},
                {(double)this.Origin.GetValue(0), (double)this.Origin.GetValue(1), (double)this.Origin.GetValue(2)},
                {(double)other.Axis.GetValue(0) + (double)other.Origin.GetValue(0), (double)other.Axis.GetValue(1) + (double)other.Origin.GetValue(1), (double)other.Axis.GetValue(2) + (double)other.Origin.GetValue(2)},
                {(double)other.Origin.GetValue(0), (double)other.Origin.GetValue(1), (double)other.Origin.GetValue(2)},
            };

            //return Accord.Math.Matrix.Rank(matrix);

            if (Accord.Math.Matrix.Rank(matrix) == 1)
            {
                return true;
            }
            
                return false;
            
        }
        
        protected bool Equals(CylinderNode other)
        {
            return base.Equals(other) && Equals(this.Axis, other.Axis) && Equals(this.Origin, other.Origin);
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
            return Equals((CylinderNode)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.Axis != null ? this.Axis.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Origin != null ? this.Origin.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
