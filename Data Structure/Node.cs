// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Node.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Node type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace SWIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The node.
    /// </summary>
    [Serializable()]
    public class Node
    {


        protected bool Equals(Node other)
        {
            return this.NumOfLoops == other.NumOfLoops && this.NumOfEdges == other.NumOfEdges && Equals(this.BoundParameters, other.BoundParameters) && this.FaceSense.Equals(other.FaceSense) && Equals(this.RealLinks, other.RealLinks) && Equals(this.VirtualLinks, other.VirtualLinks) && this.IdNode == other.IdNode;
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
            return Equals((Node)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.NumOfLoops;
                hashCode = (hashCode * 397) ^ this.NumOfEdges;
                hashCode = (hashCode * 397) ^ (this.BoundParameters != null ? this.BoundParameters.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.FaceSense.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.RealLinks != null ? this.RealLinks.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.VirtualLinks != null ? this.VirtualLinks.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.IdNode;
                return hashCode;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="idNode">
        /// The id Node fornisce l'identificativo della faccia corrispondente al nodo.
        /// </param>
        /// <param name="numOfLoops">
        /// The num of loops fornisce il numero di loop della faccia.
        /// </param>
        /// <param name="numOfEdges">
        /// The num of edges fornisce il numero di spigoli della faccia
        /// </param>
        /// <param name="boundParameters">
        /// The bound parameters forniscono i parametri di bound della faccia
        /// </param>
        /// <param name="faceSense">
        /// The face Sense determina il tipo di orientamento di una faccia
        /// TRUE = senso opposto, FALSE = senso concorde.
        /// </param>
        /// <param name="realLink">
        /// The realLink.
        /// </param>
        /// <param name="virtualLink">
        /// The virtualLink.
        /// </param>
        public Node(
            int idNode,
            int numOfLoops,
            int numOfEdges,
            double[] boundParameters,
            bool faceSense,
            List<RealLink> realLink,
            List<VirtualLink> virtualLink)
        {
            this.IdNode = idNode;
            this.NumOfLoops = numOfLoops;
            this.NumOfEdges = numOfEdges;
            this.BoundParameters = boundParameters;
            this.FaceSense = faceSense;
            this.RealLinks = realLink;
            this.VirtualLinks = virtualLink;
        }

        /// <summary>
        /// Gets or sets the num of loops.
        /// </summary>
        public int NumOfLoops { get; set; }

        /// <summary>
        /// Gets or sets the num of edges.
        /// </summary>
        public int NumOfEdges { get; set; }

        /// <summary>
        /// Gets or sets the bound parameters.
        /// </summary>
       
        public double[] BoundParameters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether face sense.
        /// </summary>
        public bool FaceSense { get; set; }

        /// <summary>
        /// Gets or sets the realLink.
        /// </summary>
        public List<RealLink> RealLinks { get; set; }

        /// <summary>
        /// Gets or sets the virtualLink.
        /// </summary>
        public List<VirtualLink> VirtualLinks { get; set; }

        /// <summary>
        /// Gets or sets the id node.
        /// </summary>
        public int IdNode { get; set; }

        /// <summary>
        /// The my is same type of node.
        /// </summary>
        /// <param name="secondNode">
        /// The second node.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool MyIsSameTypeOfNode(Node secondNode)
        {
            if (this is PlaneNode && secondNode is PlaneNode)
            {
                return true;
            }
            else if (this is ConeNode && secondNode is ConeNode)
            {
                return true;
            }
            else if (this is CylinderNode && secondNode is CylinderNode)
            {
                return true;
/*                
                var thisCylinderNode = (CylinderNode)this;
                var secondCylinderNode = (CylinderNode)secondNode;

                if (thisCylinderNode.FaceSense == secondCylinderNode.FaceSense)
                {
                    if (Math.Abs(thisCylinderNode.Radius - secondCylinderNode.Radius) < 0.0001)
                    {
                        return true;
                    }
                }*/
            }
            else if (this is SphereNode && secondNode is SphereNode)
            {
                return true;
            }
            else if (this is TorusNode & secondNode is TorusNode)
            {
                return true;
            }

            return false;
        }

        public Node() { }


    }
}
