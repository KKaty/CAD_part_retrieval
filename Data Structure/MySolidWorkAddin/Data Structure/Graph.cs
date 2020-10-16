// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Graph.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Graph type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SWIntegration.Data_Structure
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The graph.
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Graph"/> class.
        /// </summary>
        /// <param name="nodes">
        /// The nodes.
        /// </param>
        public Graph(List<Node> nodes)
        {
            this.Nodes = nodes;
        }

        /// <summary>
        /// The my comparison.
        /// </summary>
        /// <param name="secondGraph">
        /// The second graph.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool MyComparisonToTwoGraphs(int searchType, Graph secondGraph)
        {
            return MyIsInList(searchType, secondGraph);
        }

        /// <summary>
        /// The my is in list.
        /// </summary>
        /// <param name="firstGraph">
        /// The first graph.
        /// </param>
        /// <param name="secondGraph">
        /// The second graph.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool MyIsInList(int searchType, Graph secondGraph)
        {
            HashSet<Node> equalNodes = new HashSet<Node>();
            int prova = 0;
            foreach (Node firstNode in Nodes)
            {
                foreach (Node secondNode in secondGraph.Nodes)
                {
                    if (MySameTopology(firstNode, secondNode)
                        && MyIsInAdjacence(searchType, firstNode, secondNode))
                    {
                        if (equalNodes.Add(firstNode))
                        {
                            prova += 1;
                        }
          
                    }

                }
            }

                if (prova == Nodes.Count )
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }

        /// <summary>
        /// The my is in adjacence.
        /// </summary>
        /// <param name="firstNode">
        /// The first node.
        /// </param>
        /// <param name="secondNode">
        /// The second node.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// 
        // TO DO: STESSO LAVORO CON ININLIST, CREARE HASHSET PER IL CONTO DEI NODI ADIACENTI CORRETTI.
        private bool MyIsInAdjacence(int searchType, Node firstNode, Node secondNode)
        {
            const double TollAngolo = 0.1;
            const double TollSpigolo = 0.1;
            var adjacenceLenght = firstNode.Adjacents.Count;
            var adjacenceEqualLenght = 0;
            foreach (var firstAdjacent in firstNode.Adjacents)
            {
                foreach (var secondAdiacent in secondNode.Adjacents)
                {
                    switch (searchType)
                    {
                        default:
                            if (MySameTopology(firstAdjacent.DestinationNode, secondAdiacent.DestinationNode))
                            {
                                if (firstNode is PlaneNode && firstAdjacent.DestinationNode is PlaneNode)
                                {
                                    if (Math.Abs(firstAdjacent.Angle - secondAdiacent.Angle) <TollAngolo)
                                    {
                                        adjacenceEqualLenght += 1;
                                    }
                                }
                                else
                                {
                                    adjacenceEqualLenght += 1;
                                }
                            }
                            break;
                    }
                }
            }
            if (adjacenceEqualLenght >= adjacenceLenght)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// The my same topoligy.
        /// </summary>
        /// <param name="firstNode">
        /// The first node.
        /// </param>
        /// <param name="secondNode">
        /// The second node.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool MySameTopology(Node firstNode, Node secondNode)
        {
            if (MySameTypeOfNode(firstNode, secondNode) && firstNode.NumOfEdges == secondNode.NumOfEdges
                && firstNode.NumOfLoops == secondNode.NumOfLoops)
            {                
                if (firstNode is PlaneNode)
                {
                    return true;
                }
                else
                {
                    if (firstNode.FaceSense == secondNode.FaceSense)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                
            }
            else
            {
                return false;
            }
        }

        private bool MySameTypeOfNode(Node firstNode, Node secondNode)
        {
            
           if ((firstNode is PlaneNode) /*&& (secondNode is PlaneNode)*/)
            {
                return true;
            }
            else if ((firstNode is CylinderNode) && (secondNode is CylinderNode))
            {
                return true;
            }
            else if ((firstNode is ConeNode) && (secondNode is ConeNode))
            {
                return true;
            }
            else if ((firstNode is SphereNode) && (secondNode is SphereNode))
            {
                return true;
            }
            else if ((firstNode is TorusNode) && (secondNode is TorusNode))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        /// <summary>
        /// The my same ratio.
        /// </summary>
        /// <param name="firstNode">
        /// The first node.
        /// </param>
        /// <param name="secondNode">
        /// The second node.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private double MyRatio(Node firstNode, Node secondNode)
        {
            var firstBound = firstNode.BoundParameters;
            var secondBound = secondNode.BoundParameters;

            var firstRatio = Math.Abs((double)firstBound.GetValue(0) - (double)firstBound.GetValue(1)) * Math.Abs((double)firstBound.GetValue(2) - (double)firstBound.GetValue(3));
            var secondRatio = Math.Abs((double)secondBound.GetValue(0) - (double)secondBound.GetValue(1)) * Math.Abs((double)secondBound.GetValue(2) - (double)secondBound.GetValue(3));

            return firstRatio / secondRatio;
                        
        }

        /// <summary>
        /// The my joint.
        /// </summary>
        /// <param name="firstNode">
        /// The first node.
        /// </param>
        /// <param name="secondNode">
        /// The second node.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool MyJoint(Node firstNode, Node secondNode)
        {
            return false;
        }

        /// <summary>
        /// The my exact.
        /// </summary>
        /// <param name="firstNode">
        /// The first node.
        /// </param>
        /// <param name="secondNode">
        /// The second node.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool MyExact(Node firstNode, Node secondNode)
        {
            return false;
        }
        /// <summary>
        /// Gets or sets the nodes.
        /// </summary>
        public List<Node> Nodes { get; set; }


    }
}
