// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealLink.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the RealLink type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SWIntegration
{
    using System;

    /// <summary>
    /// The adiacence node.
    /// </summary>
    [Serializable()]
    public class RealLink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RealLink"/> class.
        /// </summary>
        /// <param name="angle">
        /// The angle identifica il tipo di connessione presente tra il nodo e il suo "destination node".
        /// Se angle [-1,0) ==> Concavo
        /// Se angle (0,1] ==> Convesso
        /// Se angle 0 ==> Liscio
        /// </param>
        /// <param name="lenght">
        /// The lenght.
        /// </param>
        /// <param name="destinationNode">
        /// The destination Node.
        /// </param>
        /// 

        public RealLink() { }

        public RealLink(double angle, double lenght, Node destinationNode)
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

        public bool assegnato = false;

        protected bool Equals(RealLink other)
        {
            return this.assegnato.Equals(other.assegnato) && this.Angle.Equals(other.Angle) && this.Lenght.Equals(other.Lenght) && Equals(this.DestinationNode, other.DestinationNode);
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
            return Equals((RealLink)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.assegnato.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Angle.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Lenght.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.DestinationNode != null ? this.DestinationNode.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
