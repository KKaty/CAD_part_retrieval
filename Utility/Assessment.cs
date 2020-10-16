
namespace SolidWorksAddinUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Accord.Math;

    using SWIntegration;

    using SolidWorks.Interop.sldworks;
    using SolidWorks.Interop.swconst;

    using SWIntegration.Data_Structure;

    public partial class Utility
    {
        public static double MyNumOfLinks(Graph mySubGraph, SldWorks mySwApplication)
        {
           // var sumRealLinks = 0;
           // var sumVirtualLinks = 0;
            var sumTotalLinks = 0;
            var nodeList = mySubGraph.Nodes;

            foreach (Node node in nodeList)
            {
                foreach (Node destinationNode in nodeList)
                {
                    if (node != destinationNode)
                    {
                        foreach (var link in node.RealLinks)
                        {
                            if (link.DestinationNode.IdNode == destinationNode.IdNode)
                            {
                                // sumRealLinks++;
                                sumTotalLinks++;
                            }
                        }

                        foreach (var link in node.VirtualLinks)
                        {
                            if (link.DestinationNode.IdNode == destinationNode.IdNode)
                            {
                                // sumVirtualLinks++;
                                sumTotalLinks++;
                            }
                        }
                    }
                }
            
            }
            
           // mySwApplication.SendMsgToUser("Archi reali " + (sumRealLinks/2).ToString());
           // mySwApplication.SendMsgToUser("Archi virtuali " + (sumVirtualLinks/2).ToString());
            //double numOfLinks = (double)sumRealLinks / 4 + (double)sumVirtualLinks / 2;

            return sumTotalLinks/2;
        }

        public static double MySimilarityAssessment(List<int> idRetrievedFaces, Graph myComparisonGraph, Graph myOriginalGraph, int minFuncion, SldWorks mySwApplication)
        { 
            var myFaceComparison = new List<Face2>();
            var myNodeSubGraph = new List<Node>();
            
            foreach (var node in myComparisonGraph.Nodes)
            {
                var idNode = node.IdNode;
                foreach (var idRetrievedNode in idRetrievedFaces)
                {
                    if (idRetrievedNode == idNode)
                    {
                        myNodeSubGraph.Add(node);
                    }
                }
            }

            var mySubGraph = new Graph(myNodeSubGraph);
            var nodeNonIsolated = 0;
            var numOfOriginalLinks = MyNumOfLinks(myOriginalGraph, mySwApplication);
            var numOfComparisonLinks = MyNumOfLinks(mySubGraph, mySwApplication);
            var numRetrievedFaces = idRetrievedFaces.Count();
           // mySwApplication.SendMsgToUser("num Links " + numOfLinks.ToString() + "facce input " + numInputNodes.ToString());
            double measure = 0;
            measure = (numOfComparisonLinks + nodeNonIsolated) / (numOfOriginalLinks - myOriginalGraph.Nodes.Count);
            return measure;
        }
    }
}
