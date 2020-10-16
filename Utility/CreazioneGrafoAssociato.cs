namespace SolidWorksAddinUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using SWIntegration;
    using SWIntegration.Data_Structure;
    /// <summary>
    /// The utility.
    /// </summary>
    public partial class Utility
    { 
            /// <summary>
            /// The my set associative graph.
            /// </summary>
            /// <param name="originalGraph">
            /// The original graph.
            /// </param>
            /// <param name="comparisonGraph">
            /// The comparison graph.
            /// </param>
            /// <returns>
            /// The <see cref="AssociativeGraph"/>.
            /// </returns>
            public static AssociativeGraph MyCreateAssociativeGraph(Graph originalGraph, Graph comparisonGraph)
            {
                // Dai due grafi creo i nodi dell'associative graph
                var associativeNodeList = MySetAssociativeNodeList(originalGraph, comparisonGraph);

                // Creo gli archi dell'associative graph
                MySetAssociativeLink(associativeNodeList);
                var associativeGraph = new AssociativeGraph(associativeNodeList);

                // Dopo aver creato i nodi del grafo associato devo creare i link.
                // Capire dove è più opportuno.
                // Probabilmente nella classe associativeGraph.
                return associativeGraph;
            }

            /// <summary>
            /// The my set associative link.
            /// </summary>
            /// <param name="associativeNodeList">
            /// The associative node list.
            /// </param>
            private static void MySetAssociativeLink(List<AssociativeNode> associativeNodeList)
            {
                foreach (AssociativeNode firstAssociativeNode in associativeNodeList)
                {
                    foreach (AssociativeNode secondAssociativeNode in associativeNodeList)
                    {
                        // Dai nodi Aa e Bb dell'associative graph ricavo i singoli nodi dei vari grafi.
                        var firstOriginalNode = firstAssociativeNode.NodeFirstGraph; // A
                        var firstDestinationNode = secondAssociativeNode.NodeFirstGraph; // B

                        var secondOriginalNode = firstAssociativeNode.NodeSecondGraph; // a
                        var secondDestinationNode = secondAssociativeNode.NodeSecondGraph; // b

                        // Verifico se tra le coppie di nodi esiste un link, poi controllo che sia lo stesso
                        var linkFirstGraph = MyGetTypeOfLink(firstOriginalNode, firstDestinationNode);
                        var linkSecondGraph = MyGetTypeOfLink(secondOriginalNode, secondDestinationNode);

                        if (MyAddAssociativeLink(linkFirstGraph, linkSecondGraph))
                        {
                            firstAssociativeNode.AssociativeLinks.Add(secondAssociativeNode);
                        }
                    }
                }
            }

            /// <summary>
            /// The my add associative link.
            /// </summary>
            /// <param name="linkFirstGraph">
            /// The link first graph.
            /// </param>
            /// <param name="linkSecondGraph">
            /// The link second graph.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            private static bool MyAddAssociativeLink(object linkFirstGraph, object linkSecondGraph)
            {
                List<AssociativeNode> associativeLink;
                AssociativeNode secondAssociativeNode;

                // Controllo se i link sono entrambi reali.
                if (linkFirstGraph.GetType() == typeof(RealLink) && linkSecondGraph.GetType() == typeof(RealLink))
                {
                    var linkRealFirstGraph = (RealLink)linkFirstGraph;
                    var linkRealSecondGraph = (RealLink)linkSecondGraph;

                    // Verifico che abbiamo lo stesso tipo di connessione
                    if (Math.Abs(linkRealFirstGraph.Angle + linkRealSecondGraph.Angle) < 0.01)
                    {
                        return true;
                    }
                }
                else if (linkFirstGraph.GetType() == typeof(VirtualLink) && linkSecondGraph.GetType() == typeof(VirtualLink))
                {
                    var linkVirtualFirstGraph = (VirtualLink)linkFirstGraph;
                    var linkVirtualSecondGraph = (VirtualLink)linkSecondGraph;

                    // Verifico che abbiamo lo stesso tipo di connessione e la stessa distanza FINIRE IMPLEMENTARE LA DISTANZA
                    if (Math.Abs(linkVirtualFirstGraph.Connection + linkVirtualSecondGraph.Connection) < 0.01)
                    {
                        return true;
                    }
                }

                // Se un link è reale e uno virtuale allora verifico che abbiano lo stesso tipo di connessione.
                else if (linkFirstGraph.GetType() == typeof(RealLink) && linkSecondGraph.GetType() == typeof(VirtualLink))
                {
                    var linkRealFirstGraph = (RealLink)linkFirstGraph;
                    var linkVirtualSecondGraph = (VirtualLink)linkSecondGraph;

                    // Verifico che abbiamo lo stesso tipo di connessione e la stessa distanza FINIRE IMPLEMENTARE LA DISTANZA
                    if (Math.Abs(linkRealFirstGraph.Angle + linkVirtualSecondGraph.Connection) < 0.01)
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// The my get type of link.
            /// </summary>
            /// <param name="firstOriginalNode">
            /// The first original node.
            /// </param>
            /// <param name="firstDestinationNode">
            /// The first destination node.
            /// </param>
            /// <returns>
            /// The <see cref="T"/>.
            /// </returns>
            private static object MyGetTypeOfLink(Node firstOriginalNode, Node firstDestinationNode)
            {
                foreach (var realLink in firstOriginalNode.RealLinks)
                {
                    if (realLink.DestinationNode.IdNode == firstDestinationNode.IdNode)
                    {
                        // Ho un link reale tra A e B
                        var linkFirstGraph = realLink;
                        return linkFirstGraph;
                    }
                }

                foreach (VirtualLink virtualLink in firstOriginalNode.VirtualLinks)
                {
                    if (virtualLink.DestinationNode.IdNode == firstDestinationNode.IdNode)
                    {
                        // Ho un link virtuale tra A e B
                        var linkFirstGraph = virtualLink;
                        return linkFirstGraph;
                    }
                }

                return null;
            }

            /// <summary>
            /// The my set associative node list.
            /// </summary>
            /// <param name="originalGraph">
            /// The original graph.
            /// </param>
            /// <param name="comparisonGraph">
            /// The comparison graph.
            /// </param>
            /// <returns>
            /// The <see cref="List"/>.
            /// </returns>
            private static List<AssociativeNode> MySetAssociativeNodeList(Graph originalGraph, Graph comparisonGraph)
            {
                var associativeNodeList = new List<AssociativeNode>();
                foreach (Node originalNode in originalGraph.Nodes)
                {
                    foreach (Node comparisonNode in comparisonGraph.Nodes)
                    {
                        if (originalNode.MyIsSameTypeOfNode(comparisonNode))
                        {
                            var associativeLinks = new List<AssociativeNode>();
                            var associativeNode = new AssociativeNode(originalNode, comparisonNode, associativeLinks);
                            associativeNodeList.Add(associativeNode);
                        }
                    }
                }

                return associativeNodeList;
            }
        }
    }

