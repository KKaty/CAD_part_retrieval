// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualLink.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the VirtualLink type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
namespace SWIntegration
{
    /// <summary>
    /// The parallel node.
    /// </summary>
    [Serializable()]
    public class VirtualLink
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualLink"/> class.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="lenght">
        /// The lenght.
        /// </param>
        /// <param name="destinationNode">
        /// The parallel face.
        /// </param>
        /// 

        public VirtualLink() { }

        public VirtualLink(double connection, double lenght, Node destinationNode)
        {
            this.Connection = connection;
            this.Lenght = lenght;
            this.DestinationNode = destinationNode;
        }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        public double Connection { get; set; }
        
        /// <summary>
        /// Gets or sets the lenght.
        /// </summary>
        public double Lenght { get; set; }

        /// <summary>
        /// Gets or sets the destination node.
        /// </summary>
        public Node DestinationNode { get; set; }

        protected bool Equals(VirtualLink other)
        {
            return this.Connection.Equals(other.Connection) && this.Lenght.Equals(other.Lenght) && Equals(this.DestinationNode, other.DestinationNode);
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
            return Equals((VirtualLink)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Connection.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Lenght.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.DestinationNode != null ? this.DestinationNode.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}

