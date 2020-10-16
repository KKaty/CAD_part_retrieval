// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssociativeGraphCreation.cs" company="">
//   
// </copyright>
// <summary>
//   The utility.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SolidWorksAddinUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SolidWorks.Interop.sldworks;

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
            /// <param name="mySwApplication">
            /// The my Sw Application.
            /// </param>
            /// <returns>
            /// The <see cref="AssociativeGraph"/>.
            /// </returns>
            public static AssociativeGraph MyCreateAssociativeGraph(Graph originalGraph, Graph comparisonGraph, SldWorks mySwApplication)
            {
                // Dai due grafi creo i nodi dell'associative graph
                var associativeNodeList = MySetAssociativeNodeList(originalGraph, comparisonGraph, mySwApplication);

                // Creo gli archi dell'associative graph
                var newAssociativeNodeList = MySetAssociativeLink(associativeNodeList, mySwApplication);
               
                var associativeGraph = new AssociativeGraph(newAssociativeNodeList);
 
                return associativeGraph;
            }

            /// <summary>
            /// The my set associative link.
            /// </summary>
            /// <param name="associativeNodeList">
            /// The associative node list.
            /// </param>
            /// <param name="mySwApplication">
            /// The my Sw Application.
            /// </param>
            /// <returns>
            /// The <see cref="List"/>.
            /// </returns>
            private static List<AssociativeNode> MySetAssociativeLink(List<AssociativeNode> associativeNodeList, SldWorks mySwApplication)
            {
                foreach (AssociativeNode firstAssociativeNode in associativeNodeList)
                {

                    var firstOriginalNode = firstAssociativeNode.NodeFirstGraph;
                    var secondOriginalNode = firstAssociativeNode.NodeSecondGraph;
                    foreach (AssociativeNode secondAssociativeNode in associativeNodeList)
                    {
                        var firstDestinationNode = secondAssociativeNode.NodeFirstGraph;
                        var secondDestinationNode = secondAssociativeNode.NodeSecondGraph;

                        if (firstOriginalNode.IdNode != firstDestinationNode.IdNode && secondOriginalNode.IdNode != secondDestinationNode.IdNode)
                        {
                            object linkFirstGraph = MyGetTypeOfLink(firstOriginalNode, firstDestinationNode, mySwApplication);
                            object linkSecondGraph = MyGetTypeOfLink(secondOriginalNode, secondDestinationNode, mySwApplication);
                            if (linkFirstGraph != null && linkSecondGraph != null)
                            {
                                if (firstOriginalNode.GetType() == typeof(PlaneNode)
                                    && secondOriginalNode.GetType() == typeof(PlaneNode))
                                {
                                    if (MyAddAssociativeLink(linkFirstGraph, linkSecondGraph, mySwApplication))
                                    {
                                        firstAssociativeNode.AssociativeLinks.Add(secondAssociativeNode);
                                    }
                                }
                                else if (firstOriginalNode.GetType() == typeof(CylinderNode)
                                         && secondOriginalNode.GetType() == typeof(CylinderNode))
                                {
                                    var node1 = (CylinderNode) firstOriginalNode;
                                    var node2 = (CylinderNode) secondOriginalNode;
                                 // Controllo per la SIZE!
                                    //if (Math.Abs(node1.Radius - node2.Radius) < 0.01)
                                    {
                                        if (MyAddAssociativeLink(linkFirstGraph, linkSecondGraph, mySwApplication))
                                        {
                                            firstAssociativeNode.AssociativeLinks.Add(secondAssociativeNode);
                                        }
                                    }
                                }
                            }
                                
                            else if (linkFirstGraph == null && linkSecondGraph == null)
                            {
                                //mySwApplication.SendMsgToUser("inserisco relazione NON C'E'");
                                firstAssociativeNode.AssociativeLinks.Add(secondAssociativeNode);
                            }
                                 
                        }
                    }
                }
                return associativeNodeList;
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
            /// <param name="mySwApplication">
            /// The my Sw Application.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            private static bool MyAddAssociativeLink(object linkFirstGraph, object linkSecondGraph, SldWorks mySwApplication)
            {
                // Controllo se i link sono entrambi reali.
                var tollAngolo = 0.01;
                var tollLunghezza = 0.01;
                if (linkFirstGraph.GetType() == typeof(RealLink) && linkSecondGraph.GetType() == typeof(RealLink))
                {
                    var linkRealFirstGraph = (RealLink)linkFirstGraph;
                    var linkRealSecondGraph = (RealLink)linkSecondGraph;

                    // Verifico che abbiamo lo stesso tipo di connessione
                    if (Math.Abs(linkRealFirstGraph.Angle - linkRealSecondGraph.Angle) < tollAngolo)
                    {
                        //mySwApplication.SendMsgToUser("INSERISCO ARCO REALE-REALE");
                        return true;
                    }
                }
                else if (linkFirstGraph.GetType() == typeof(VirtualLink)
                    && linkSecondGraph.GetType() == typeof(VirtualLink))
                {
                    var linkVirtualFirstGraph = (VirtualLink)linkFirstGraph;
                    var linkVirtualSecondGraph = (VirtualLink)linkSecondGraph;
                    
                    // Verifico che abbiamo lo stesso tipo di connessione e la stessa distanza FINIRE IMPLEMENTARE LA DISTANZA
                    if (Math.Abs(linkVirtualFirstGraph.Connection - linkVirtualSecondGraph.Connection) < tollAngolo)
                    {
                         return true;

                        // Controllo per il "size constraints"
                        /*
                        if (Math.Abs(linkVirtualFirstGraph.Lenght - linkVirtualSecondGraph.Lenght) < tollLunghezza)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                        */

                    }
                }
                /*
                                // Se un link è reale e uno virtuale allora verifico che abbiano lo stesso tipo di connessione.
                                    else if (linkFirstGraph.GetType() == typeof(RealLink)
                                             && linkSecondGraph.GetType() == typeof(VirtualLink))
                                    {
                                        var linkRealFirstGraph = (RealLink)linkFirstGraph;
                                        var linkVirtualSecondGraph = (VirtualLink)linkSecondGraph;
                                        //mySwApplication.SendMsgToUser("primo arco " + linkRealFirstGraph.Angle.ToString() + " secondo arco " + linkVirtualSecondGraph.Connection.ToString());
                                            // Verifico che abbiamo lo stesso tipo di connessione e la stessa distanza FINIRE IMPLEMENTARE LA DISTANZA
                                            if (Math.Abs(linkRealFirstGraph.Angle - linkVirtualSecondGraph.Connection) < tollAngolo)
                                            {
                                                return true;
                                            }
                                            else
                                            {
                                                return false;
                                            }
                                    }
                
                                    else if (linkFirstGraph.GetType() == typeof(VirtualLink) && linkSecondGraph.GetType() == typeof(RealLink))
                                    {
                                        var linkRealSecondGraph = (RealLink)linkSecondGraph;
                                        var linkVirtualFirstGraph = (VirtualLink)linkFirstGraph;

                                            // Verifico che abbiamo lo stesso tipo di connessione e la stessa distanza FINIRE IMPLEMENTARE LA DISTANZA
                                            if (Math.Abs(linkRealSecondGraph.Angle - linkVirtualFirstGraph.Connection) < tollAngolo)
                                            {
                                                return true;
                                            }
                                            else
                                            {
                                                return false;
                                            }
                                    }
                                */


                return false;
            }

            /// <summary>
            /// The my get type of link.
            /// </summary>
            /// <param name="originalNode">
            /// The first original node.
            /// </param>
            /// <param name="destinationNode">
            /// The first destination node.
            /// </param>
            /// <returns>
            /// The <see cref="T"/>.
            /// </returns>
            private static object MyGetTypeOfLink(Node originalNode, Node destinationNode, SldWorks mySwApplication)
            {
                foreach (RealLink realLink in originalNode.RealLinks)
                {
                 //  mySwApplication.SendMsgToUser("coppia reale " + realLink.DestinationNode.IdNode.ToString() + " " + destinationNode.IdNode.ToString());
                     if (realLink.DestinationNode.IdNode == destinationNode.IdNode)
                    {
                        //Utility.PrintToFile(originalNode.IdNode.ToString() + " " + destinationNode.IdNode.ToString() + " REALE ", "StampaArchi.txt");
                        return realLink;
                    }
           
                }
                
                foreach (VirtualLink virtualLink in originalNode.VirtualLinks)
                {
                    if (virtualLink.DestinationNode.IdNode == destinationNode.IdNode)
                    {
                        //Utility.PrintToFile(originalNode.IdNode.ToString() + " " + destinationNode.IdNode.ToString() + " Virtuale ", "StampaArchi.txt");
                        return virtualLink;
                    }
                }
               
                return null;

                // Se non ho la connessione ritorno null.
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
            /// <param name="mySwApplication">
            /// The my Sw Application.
            /// </param>
            /// <returns>
            /// The <see cref="List"/>.
            /// </returns>
            private static List<AssociativeNode> MySetAssociativeNodeList(Graph originalGraph, Graph comparisonGraph, SldWorks mySwApplication)
        {
            List<AssociativeNode> list = new List<AssociativeNode>();
            foreach (Node originalNode in originalGraph.Nodes)
            {
                foreach (Node comparisonNode in comparisonGraph.Nodes)
                {
                    if (originalNode.MyIsSameTypeOfNode(comparisonNode) && comparisonNode.MyIsSameTypeOfNode(originalNode))
                    {
                        if (originalNode is PlaneNode)
                        {
                            List<AssociativeNode> associativeLinks = new List<AssociativeNode>();
                            var associativeNode = new AssociativeNode(originalNode, comparisonNode, associativeLinks);
                            list.Add(associativeNode);
                        }
                            
                        else if(originalNode is CylinderNode)
                        {
                            var node1 = (CylinderNode)originalNode;
                            var node2 = (CylinderNode)comparisonNode;
                            if (node1.FaceSense == node2.FaceSense && node1.Complete == node2.Complete)
                            {
                                List<AssociativeNode> associativeLinks = new List<AssociativeNode>();
                                var associativeNode = new AssociativeNode(originalNode, comparisonNode, associativeLinks);
                                list.Add(associativeNode);
                            }
                        }
                        
                    }
                }
            }
            return list;
        }
    }
    }

