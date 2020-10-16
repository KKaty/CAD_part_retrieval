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
        private int color = 255;
 


        public static void MyColorFace(List<Entity> myEntityComparison, AssociativeGraph myCliqueGraph, SldWorks mySwApplication, ModelDoc2 myModel)
        {
            double colorRgb1 = 1;
            double colorRgb2 = 0;
            double colorRgb3 = 0;


            var myFaceComparison = new List<Face2>();

            foreach (Entity entity in myEntityComparison)
            {
                var myFace = (Face2)entity.GetSafeEntity();
                myFaceComparison.Add(myFace);
            }
            

                foreach (AssociativeNode associativeNode in myCliqueGraph.listNode)
                {
                    var node = (Node)associativeNode.NodeSecondGraph;
                    var idNode = node.IdNode;
                    
                    var faceRetrieved = (Face2) myFaceComparison.Find(x => x.GetFaceId() == idNode);
                    var faceMaterial = (Array)faceRetrieved.MaterialPropertyValues;

                        if (faceMaterial == null)
                        {
                            faceMaterial = (Array)myModel.MaterialPropertyValues;
                        }
          
                        faceMaterial.SetValue(colorRgb1, 0);
                        faceMaterial.SetValue(colorRgb2, 1);
                        faceMaterial.SetValue(colorRgb3, 2);

                        faceRetrieved.MaterialPropertyValues = faceMaterial;
                        //Utility.PrintToFile(idNode.ToString(), "colorFace.txt");

                    
                }
            

            myModel.ClearSelection2(true);
        }
    }
}
