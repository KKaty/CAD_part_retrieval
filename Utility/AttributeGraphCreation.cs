// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utility.cs" company="">
//   
// </copyright>
// <summary>
//   The salvataggio.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SolidWorksAddinUtility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using SolidWorks.Interop.sldworks;
    using SWIntegration;
    using SWIntegration.Data_Structure;

    using SolidWorks.Interop.swconst;

    /// <summary>
    /// The salvataggio.
    /// </summary>
    public partial class Utility
    {
        /// <summary>
        /// The my save data.
        /// </summary>
        /// <param name="mySelectedFace">
        /// The my selected face.
        /// </param>
        /// <param name="myEntityComparison"></param>
        /// <param name="myModel">
        /// The my model.
        /// </param>
        /// <param name="mySWApplication">
        /// The my sw application.
        /// </param>
        /// <returns>
        /// The <see cref="Graph"/>.
        /// </returns>
        public static Graph MySaveData(List<Entity> myEntityComparison, ModelDoc2 myModel, SldWorks mySWApplication)
        {
            
            var nodeList = new List<Node>();

            // Istanzio tutti i nodi senza gli archi, successivamente itero nuovamente sui nodi per istanziare gli archi
            // correttamente.
            var myFaceList = new List<Face2>();
            foreach (Entity entity in myEntityComparison)
            {
                var face = (Face2)entity.GetSafeEntity();  
                myFaceList.Add(face);
                var myRealLinks = new List<RealLink>();  
                var myVirtualLinks = new List<VirtualLink>();
                nodeList.Add(MySetNode(face, myRealLinks, myVirtualLinks));
            }

            // Aggiungo gli archi reali alla lista dei nodi.
            MySetRealLink(nodeList, myFaceList, mySWApplication);
            MySetVirtualLink(nodeList, myFaceList, myModel, mySWApplication);
  
            var myGraph = new Graph(nodeList);
            return myGraph;
        }

        /// <summary>
        /// The my adjacence face.
        /// </summary>
        /// <param name="face">
        /// The face.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static List<Face2> MyAdjacenceFace(Face2 face)
        {
            var listAdjacentFace = new List<Face2>();

            // Aggiungo la faccia stessa e tutte le sue adiacenti,
            // Se non aggiungo anche se stessa poi controlla la presenza di edge virtuali tra sé e sé.
            listAdjacentFace.Add(face);
            var myEdegOfFaceCount = face.GetEdgeCount();
            var myEdgeOfFace = (Array)face.GetEdges();
            foreach (Edge edge in myEdgeOfFace)
            {
                Array myAdiacenceFaces = edge.GetTwoAdjacentFaces2();
                Face2 myAdiacenceFace = null;
                var firstFace = (Face2)myAdiacenceFaces.GetValue(0);
                var secondFace = (Face2)myAdiacenceFaces.GetValue(1);

                // Se la faccia non è quella che ho già la salvo come faccia adiacente, altrimenti salvo l'altra.
                // Se non c'è un'altra faccia adiacente allora ho un valore NULL e non salvo niente.
                if (firstFace.GetFaceId() == face.GetFaceId())
                {
                    myAdiacenceFace = secondFace;
                }
                else if (secondFace.GetFaceId() == face.GetFaceId())
                {
                    myAdiacenceFace = firstFace;
                }

                listAdjacentFace.Add(myAdiacenceFace);
            }

            return listAdjacentFace;
        }

        /// <summary>
        /// The my set real link.
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="faceList"></param>
        /// <param name="mySwApplication">
        ///     The my Sw Application.
        /// </param>
        /// <param name="myFace">
        /// The my face.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
       
        private static void MySetRealLink(List<Node> nodeList, List<Face2> faceList, SldWorks mySwApplication)
        {
            foreach (Node node in nodeList)
            {
                if (node.GetType() != typeof(InvalidNode))
                {
                    int idNode = node.IdNode;
                    var myFace = faceList.Find(x => x.GetFaceId() == idNode);

                    var myEdgeOfFaceCount = myFace.GetEdgeCount();
                    var myEdgeOfFace = (Array)myFace.GetEdges();
                    foreach (Edge edge in myEdgeOfFace)
                    {
                        if(node.GetType() == typeof(CylinderNode))
                        {
                            var cylinderNode = (CylinderNode)node;
                            if (cylinderNode.Complete == false)
                            {
                                if (edge.GetEndVertex() == null || edge.GetStartVertex() == null)
                                {
                                    cylinderNode.Complete = true;
                                }
                            }
                        }
                        Array myAdiacenceFaces = edge.GetTwoAdjacentFaces2();
                        Face2 myAdjacenceFace = null;
                        var firstFace = (Face2)myAdiacenceFaces.GetValue(0);
                        var secondFace = (Face2)myAdiacenceFaces.GetValue(1);

                        // Se la faccia non è quella che ho già la salvo come faccia adiacente, altrimenti salvo l'altra.
                        // Se non c'è un'altra faccia adiacente allora ho un valore NULL e non salvo niente.
                        if (firstFace.GetFaceId() == myFace.GetFaceId())
                        {
                            myAdjacenceFace = secondFace;
                        }
                        else if (secondFace.GetFaceId() == myFace.GetFaceId())
                        {
                            myAdjacenceFace = firstFace;
                        }
                        
                        if (myAdjacenceFace != null)
                        {
                            Node myComparisonNode = nodeList.Find(x => x.IdNode == myAdjacenceFace.GetFaceId());
                            if (myComparisonNode != null && myComparisonNode.GetType() != typeof(InvalidNode))
                            {
                                var angle = Utility.MyTypeOfConnectionOfRealLink(edge, myFace, myAdjacenceFace, mySwApplication);
                                var lenght = 0; //Utility.MySetLenght(edge); Da utilizzare in futuro se si decide di avere la lunghezza dell'edge.
                                //var myAdjacenceNode = nodeList.Find(x => x.IdNode == myAdjacenceFace.GetFaceId());
                                if (angle != 5)
                                {
                                    var myRealLink = new RealLink(angle, lenght, myComparisonNode);
                                    node.RealLinks.Add(myRealLink);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //mySwApplication.SendMsgToUser("Nodo invalido");
                }
            }
        }   

        
        /// <summary>
        /// The my set virtual link.
        /// Al momento non è coerente con il documento della tesi, bisogna verificare quando l'intersezione tra due piani associati a due facce appartiene ad una delle due facce
        /// Il calcolo della distanza è da completare per facce piane non parallele e conseguentemente cambiare il modello di distanza nei nodi
        /// </summary>
        /// <param name="face">
        /// The face.
        /// </param>
        /// <param name="mySelectedFace">
        /// The my selected face.
        /// </param>
        /// <param name="myModel">
        /// The my model.
        /// </param>
        /// <param name="mySWApplication">
        /// The my sw application.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static void MySetVirtualLink(List<Node> myNodeList, List<Face2> myFaceList, ModelDoc2 myModel, SldWorks mySWApplication)
       {
            foreach (Node node in myNodeList)
            {
                int idNode = node.IdNode;
                var myFace = myFaceList.Find(x => x.GetFaceId() == idNode);


                // Per ogni faccia creo una lista delle sue facce incidenti (quelle con cui ha un edge reale) per evitare di creare archi
                // virtuali tra facce adiacenti


                var listAdjacentFace = MyAdjacenceFace(myFace);
               
                foreach (Face2 face in myFaceList)
                {
                    var myComparisonFace = face;
                    if (listAdjacentFace == null)
                    {
                        mySWApplication.SendMsgToUser("Non ci sono facce adiacenti!");
                        break;
                    }
                    
                    var myFaceConteins = listAdjacentFace.Find(x => x.GetFaceId() == myComparisonFace.GetFaceId());
                    if(myFaceConteins == null)
                    {
                        var virtualNode = myNodeList.Find(x => x.IdNode == myComparisonFace.GetFaceId());

                        var firstSurface = (Surface)myFace.GetSurface();
                        var secondSurface = (Surface)myComparisonFace.GetSurface();
                        var idFirstSurface = firstSurface.Identity();
                        var idSecondSurface = secondSurface.Identity();

                        // Se le due facce sono piane, allora verifico la connessione parallela e poi quella non parallela
                        if (idFirstSurface == 4001 && idSecondSurface == 4001)
                        {
                            var firstPlainNode = (PlaneNode)node;
                            var secondPlainNode = (PlaneNode)virtualNode;

                            // scarto i tipi di connessione che non sono simmetrici, dati dai casi convessi non interessanti.
                            var myFirstTypeOfConnection = MyTypeOfConnectionOfVirtualLink(
                                myFace, myComparisonFace, mySWApplication);
                            var mySecondTypeOfConnection = MyTypeOfConnectionOfVirtualLink(
                                myComparisonFace, myFace, mySWApplication);

                            const double Epsilon = 0.001;
                            if (Math.Abs(myFirstTypeOfConnection - mySecondTypeOfConnection) < Epsilon)
                            {
                                if (firstPlainNode.IsSamePlane(secondPlainNode))
                                {
                                    var virtualLink = new VirtualLink(
                                                Liscio, 0, virtualNode);
                                    node.VirtualLinks.Add(virtualLink);
                                }
                                // se il tipo di connessione è parallela si verifica se è piena o vuota.
                                else if (myFirstTypeOfConnection != ParalleloConcorde)
                                {

                                    if (MyIsFull(
                                        myFace, myComparisonFace, myFirstTypeOfConnection, myModel, mySWApplication)
                                        && MyIsFull(
                                            myComparisonFace, myFace, myFirstTypeOfConnection, myModel, mySWApplication))
                                    {
                                        // Caso parallelo discorde & pieno  ==> Aggiungo arco virtuale con valore +10
                                        if (myFirstTypeOfConnection == ParalleloDisconcorde)  // Controllo necessario, la connessione != ParalleloConcorde potrebbe essere anche convessa.
                                        {
                                            double dist = MyDistanceParallelPlane(myFace, myComparisonFace);
                                            var virtualLink = new VirtualLink(ParalleloPieno, dist, virtualNode);
                                            node.VirtualLinks.Add(virtualLink);
                                        }

                                        // Caso convesso
                                        if (myFirstTypeOfConnection > 0 && myFirstTypeOfConnection <= 1)
                                        {
                                            double distMin;
                                            double distMax;
                                            double distMaxff2;
                                            double distMaxf2f;
                                            double distff2 = MyDistanceOfNonParallelPlane(
                                                myFace, myComparisonFace, out distMaxff2);
                                            double distf2f = MyDistanceOfNonParallelPlane(
                                                myComparisonFace, myFace, out distMaxf2f);
                                            if (distff2 > distf2f)
                                            {
                                                distMin = distf2f;
                                            }
                                            else
                                            {
                                                distMin = distff2;
                                            }

                                            if (distMaxff2 > distMaxf2f)
                                            {
                                                distMax = distMaxff2;
                                            }
                                            else
                                            {
                                                distMax = distMaxf2f;
                                            }

                                            var virtualLink = new VirtualLink(
                                                myFirstTypeOfConnection, distMin, virtualNode);
                                            node.VirtualLinks.Add(virtualLink);
                                        }
                                    }
                                    else if (
                                        !MyIsFull(
                                            myFace,
                                            myComparisonFace,
                                            myFirstTypeOfConnection,
                                            myModel,
                                            mySWApplication))
                                    {
                                        if (myFirstTypeOfConnection == ParalleloDisconcorde)
                                        {
                                            if (MyIsPartiallyEmpty(
                                                 myFace,
                                                 myComparisonFace,
                                                 myFirstTypeOfConnection,
                                                 myModel,
                                                 mySWApplication))
                                            {
                                                double dist = MyDistanceParallelPlane(myFace, myComparisonFace);
                                                if (dist != 0)
                                                {
                                                    var virtualLink = new VirtualLink(ParalleloVuoto, dist, virtualNode);
                                                    node.VirtualLinks.Add(virtualLink);
                                                }
                                            }
                                        }
                                        if (myFirstTypeOfConnection < 0 & myFirstTypeOfConnection >= -1)
                                        {
                                            if (MyIsPartiallyEmpty(
                                                myFace,
                                                myComparisonFace,
                                                myFirstTypeOfConnection,
                                                myModel,
                                                mySWApplication))
                                            {
                                                double distMin;
                                                double distMax;
                                                double distMaxff2;
                                                double distMaxf2f;
                                                double distff2 = MyDistanceOfNonParallelPlane(
                                                    myFace, myComparisonFace, out distMaxff2);
                                                double distf2f = MyDistanceOfNonParallelPlane(
                                                    myComparisonFace, myFace, out distMaxf2f);
                                                if (distff2 > distf2f)
                                                {
                                                    distMin = distf2f;
                                                }
                                                else
                                                {
                                                    distMin = distff2;
                                                }

                                                if (distMaxff2 > distMaxf2f)
                                                {
                                                    distMax = distMaxff2;
                                                }
                                                else
                                                {
                                                    distMax = distMaxf2f;
                                                }

                                                var virtualLink = new VirtualLink(
                                                    myFirstTypeOfConnection, distMin, virtualNode);
                                                node.VirtualLinks.Add(virtualLink);
                                            }

                                        }

                                    }
                                }
                                else
                                {
                                    //if (firstPlainNode.IsSamePlane(secondPlainNode))
                                    {
                                        var virtualLink = new VirtualLink(
                                                    ParalleloConcorde, 0, virtualNode);
                                        node.VirtualLinks.Add(virtualLink);
                                    }
                                }
                            }
                        }
                            // Caso in cui le facce sono associate a cilindri.
                            // L'implementazione non è completa, al momento controlla solo se le facce sono cilindriche
                            // ed aggiunge un arco virtuale con valore dell'attributo tipo di connessione +10.
                            // Sarà da modificare secondo la descrizione fornita nel documento della tesi.
                        else if(idFirstSurface == 4002 && idSecondSurface == 4002)
                        {
                            var toll = 0.01;
                            var firstCylinderNode = (CylinderNode)node;
                            var secondCylinderNode = (CylinderNode)virtualNode;

                            var firstAxis = (Array)firstCylinderNode.Axis;
                            var secondAxis = (Array)secondCylinderNode.Axis;

                            if (Math.Abs(Math.Abs(Utility.MyInnerProduct(firstAxis, secondAxis)) - 1) < toll)
                            {
                                var dist = Utility.MyDistanceTwoPoint(firstCylinderNode.Origin, secondCylinderNode.Origin);

                                if (firstCylinderNode.IsSameCylinder(secondCylinderNode))
                                {
                                    var virtualLink = new VirtualLink(
                                                Liscio, 0, virtualNode);
                                    node.VirtualLinks.Add(virtualLink);
                                }
                                
                                else if ((firstCylinderNode.FaceSense == false && secondCylinderNode.FaceSense == false) || (firstCylinderNode.FaceSense == true && secondCylinderNode.FaceSense == true))
                                {
                                    if (MyIsPartiallyEmpty(
                                                myFace,
                                                myComparisonFace,
                                                -10,
                                                myModel,
                                                mySWApplication))
                                    {

                                        var pointFirstFace = MyGetPointFromFaceEntity(myFace);
                                        var pointSecondFace = MyGetPointFromFaceEntity(myComparisonFace);

                                        //mySWApplication.SendMsgToUser("prima faccia " + pointFirstFace.Count.ToString() + " seconda faccia " + pointSecondFace.Count.ToString());
                                        // Se i cilindri sono entrambi senza vertici
                                        if (pointFirstFace.Count == 12 || pointSecondFace.Count == 12)
                                        {
                                            double[] closestPointToIntersectionOnFirstSurface = firstSurface.GetClosestPointOn(pointSecondFace[0], pointSecondFace[1], pointSecondFace[2]);
                                            double[] closestPointToIntersectionOnSecondSurface = secondSurface.GetClosestPointOn(pointFirstFace[0], pointFirstFace[1], pointFirstFace[2]);

                                            var normalFirstFace = (double[])MyNormalInPoint(myFace, closestPointToIntersectionOnFirstSurface[0], closestPointToIntersectionOnFirstSurface[1], closestPointToIntersectionOnFirstSurface[2]);
                                            double[] dir =
                                                  (double[])MyNormalization(MyVectorDifferent(closestPointToIntersectionOnSecondSurface, closestPointToIntersectionOnFirstSurface));


                                            if (MyInnerProduct(dir, normalFirstFace) > 0)
                                            {
                                                //mySWApplication.SendMsgToUser("VUOTO");
                                                var virtualLink = new VirtualLink(10, dist, virtualNode);
                                                node.VirtualLinks.Add(virtualLink);
                                            }
                                            else if (MyInnerProduct(dir, normalFirstFace) < 0)
                                            {
                                                //mySWApplication.SendMsgToUser("PIENO");
                                                var virtualLink = new VirtualLink(-10, dist, virtualNode);
                                                node.VirtualLinks.Add(virtualLink);
                                            }
                                        }
                                        else
                                        {
                                            if ((firstCylinderNode.FaceSense == true && secondCylinderNode.FaceSense == true))
                                            {
                                                var virtualLink = new VirtualLink(-10, dist, virtualNode);
                                                node.VirtualLinks.Add(virtualLink);
                                            }
                                            else if ((firstCylinderNode.FaceSense == false && secondCylinderNode.FaceSense == false))
                                            {
                                                var virtualLink = new VirtualLink(10, dist, virtualNode);
                                                node.VirtualLinks.Add(virtualLink);
                                            }
                                        
                                        }
                                    }
                                        
                                    else
                                    {
                                        if ((firstCylinderNode.FaceSense == true && secondCylinderNode.FaceSense == true))
                                        {
                                            var virtualLink = new VirtualLink(10, dist, virtualNode);
                                            node.VirtualLinks.Add(virtualLink);
                                        }
                                    }
                                         
                                }
                                
                            }
                        }
                    }
                }
            }
       }

       private static bool MyIsPartiallyEmpty(Face2 firstFace, Face2 secondFace, double typeOfConnection, ModelDoc2 myModel, SldWorks mySwApplication)
       {
           var firstSurface = (Surface)firstFace.GetSurface();
           var secondSurface = (Surface)secondFace.GetSurface();

           List<double[]> puntiConDuplicati = new List<double[]>();
           List<double[]> puntiSenzaDuplicati = new List<double[]>();
          
           // Ricavo l'array dei body
           ModelDoc2 swDoc = ((ModelDoc2)(mySwApplication.ActiveDoc));
           var myPartDoc = (PartDoc)myModel;
           IBody2[] body = new IBody2[] {myPartDoc.GetProcessedBody() as IBody2 };

           var saveFirstFace = ((Entity)firstFace).GetSafeEntity();
           var saveSecondFace = ((Entity)secondFace).GetSafeEntity();

           List<double> listPointFirstFace = MyGetPointFromFaceEntity(firstFace);
           List<double> listPointSecondFace = MyGetPointFromFaceEntity(secondFace);

           int correctIntersection = 0;
           int correctIntersectionForPair = 0;
        
           for (int i = 0; i < listPointFirstFace.Count; i+=3)
           {
               var firstPoint = new double[] { listPointFirstFace[i], listPointFirstFace[i + 1], listPointFirstFace[i + 2]};
               
               for (int j = 0; j < listPointSecondFace.Count; j+=3)
               {
                   correctIntersectionForPair = 0;
                   var secondPoint = new double[] { listPointSecondFace[j], listPointSecondFace[j + 1], listPointSecondFace[j + 2]};
                   pointIntersection(firstPoint, firstFace, secondPoint, secondFace, body, myModel, mySwApplication, ref correctIntersection, ref correctIntersectionForPair);

                   if (correctIntersectionForPair == 0 && firstSurface.Identity() == 4001 && secondSurface.Identity() == 4001)
                   {
                       return true;
                   }
               }
           }


           if (correctIntersection == 0)
           {
               if (firstSurface.Identity() == 4002 && secondSurface.Identity() == 4002)
               {
                 //  mySwApplication.SendMsgToUser("Facce completamente visibili");
                   return true;
               }

           }
           else
           { 
               //mySwApplication.SendMsgToUser("Intersezioni totali " + correctIntersection.ToString());
           }
            

           return false;
       }

       public static List<double> MyGetPointFromFaceEntity(Face2 face)
        {
            //var face = (Face2)entity.GetSafeEntity();
            var listPointFace = new List<double>();
            
            var listEdgeFace = (Array)face.GetEdges();

            foreach (Edge edge in listEdgeFace)
            {
                if (edge != null)
                {
                    var edgeCurve = (Curve)edge.GetCurve();

                    var vertexS = (Vertex)edge.GetStartVertex();
                    double[] startPoint = new double[3];
                    if (vertexS != null)
                    {
                        startPoint = vertexS.GetPoint();
                    }
                    else
                    {
                        var curveParaData = (CurveParamData)edge.GetCurveParams3();

                        double[] vStartPoint = (double[])curveParaData.StartPoint;
                        for (int i = 0; i <= vStartPoint.GetUpperBound(0); i++)
                        {
                            startPoint[i] = vStartPoint[i];
                        }

                    }

                    var vertexE = (Vertex)edge.GetEndVertex(); 
                    double[] endPoint = new double[3];
                    if (vertexE != null)
                    {
                        endPoint = vertexE.GetPoint();
                    }
                    else
                    {
                        var curveParaData = (CurveParamData)edge.GetCurveParams3();

                        double[] vEndPoint = (double[])curveParaData.EndPoint;

                        int i = 0;

                        for (i = 0; i <= vEndPoint.GetUpperBound(0); i++)
                        {
                            endPoint[i] = vEndPoint[i];
                        }

                    }

                    for (int i = 0; i < startPoint.Length; i++)
                    {
                        listPointFace.Add((double)startPoint.GetValue(i));
                    }
                    for (int i = 0; i < startPoint.Length; i++)
                    {
                        listPointFace.Add((double)endPoint.GetValue(i));
                    }

                }
            }

            return listPointFace;
        }


       private static Array MyNormalInPoint(Face2 face, double x, double y, double z) 
       {
           var normal = new double[3];
           var mySurf = (Surface)face.GetSurface();

           double[] firstEvalutation = mySurf.EvaluateAtPoint(x, y, z); // --> EvaluateAtPoint restituisce la normale alla superficie in un punto! 

           
           if (face.FaceInSurfaceSense())
           {
               normal.SetValue((double)firstEvalutation.GetValue(0), 0);
               normal.SetValue((double)firstEvalutation.GetValue(1), 1);
               normal.SetValue((double)firstEvalutation.GetValue(2), 2);
           }
           else
           {
               normal.SetValue(-(double)firstEvalutation.GetValue(0), 0);
               normal.SetValue(-(double)firstEvalutation.GetValue(1), 1);
               normal.SetValue(-(double)firstEvalutation.GetValue(2), 2);
           }
           
           return normal;
       }

       private static void pointIntersection(double[] firstPoint, Face2 firstFace, double[] secondPoint, Face2 secondFace, IBody2[] body, ModelDoc2 myModel, SldWorks mySwApplication, ref int correctIntersection, ref int correctIntersectionForPair)
       {
           var firstSurface = (Surface)firstFace.GetSurface();
           var secondSurface = (Surface)secondFace.GetSurface();
           correctIntersection = 0;
           if (firstPoint != null && secondPoint != null)
           {

               var direction = (double[])MyNormalization(MyVectorDifferent(secondPoint, firstPoint));
               var intersection =
                   (int)
                   myModel.RayIntersections(
                       body,
                       (object)firstPoint,
                       (object)direction,
                       (int)(swRayPtsOpts_e.swRayPtsOptsENTRY_EXIT),
                       (double)0,
                       (double)0);

               // Se sono dei presenti dei punti di intersezione li salvo in un apposito vettore.

               if (intersection > 0)
               {
                   var points = (double[])myModel.GetRayIntersectionsPoints();
                   var totalEntity = (Array)myModel.GetRayIntersectionsTopology();
                   //mySwApplication.SendMsgToUser("punti totali " + points.Length.ToString() + "intersezioni " + intersection.ToString());
                   if (points != null)
                   {
                       for (int i = 0; i < points.Length; i += 9)
                       {
                           double[] pt = new double[] { points[i + 3], points[i + 4], points[i + 5] };
                           double[] directionIntersection =
                               (double[])MyNormalization(MyVectorDifferent(pt, firstPoint));

                           if (Math.Abs(MyInnerProduct(direction, directionIntersection) - 1) < 0.01
                               /*&& MyDistanceTwoPoint(pt, secondPoint) > 0.001 && MyDistanceTwoPoint(pt, firstPoint) > 0.001*/)
                           {
                               if (firstSurface.Identity() == 4001 && secondSurface.Identity() == 4001)
                               {
                                   if (MyDistanceTwoPoint(firstPoint, secondPoint) > MyDistanceTwoPoint(firstPoint, pt))
                                   {
                                       correctIntersection++;
                                       correctIntersectionForPair++;
                                   }
                               }
                               else if (firstSurface.Identity() == 4002 && secondSurface.Identity() == 4002)
                               {
                                   double[] closestPointToIntersectionOnFirstSurface = firstSurface.GetClosestPointOn(pt[0], pt[1], pt[2]);
                                   double[] closestPointToIntersectionOnSecondSurface = secondSurface.GetClosestPointOn(pt[0], pt[1], pt[2]);
                                   //myModel.Insert3DSketch();
                                   //myModel.CreatePoint2(pt[0], pt[1], pt[2]);
                                   if (MyDistanceTwoPoint(firstPoint, secondPoint) > MyDistanceTwoPoint(firstPoint, pt) &&
                                       Math.Abs(MyDistanceTwoPoint(closestPointToIntersectionOnFirstSurface, pt)) > 0.0001 && Math.Abs(MyDistanceTwoPoint(closestPointToIntersectionOnSecondSurface, pt)) > 0.0001)
                                   {
                                       correctIntersection++;
                                       correctIntersectionForPair++;
                                   }
                                   else
                                   {
                                       var normalFirstFace = (double[])MyNormalInPoint(firstFace, closestPointToIntersectionOnFirstSurface[0], closestPointToIntersectionOnFirstSurface[1], closestPointToIntersectionOnFirstSurface[2]);
                                       double[] dir =
                                             (double[])MyNormalization(MyVectorDifferent(closestPointToIntersectionOnSecondSurface, closestPointToIntersectionOnFirstSurface));
    
                                   }

                               }
                           }
                                
                       }
                   }
                   else { mySwApplication.SendMsgToUser("Punti intersezione nulli"); }
               }
           }
           else { mySwApplication.SendMsgToUser("Punti nulli"); }

       }
     
        /// <summary>
        /// The my set node.
        /// </summary>
        /// <param name="myFace">
        /// The my face.
        /// </param>
        /// <param name="myAdjacence">
        /// The my adjacence.
        /// </param>
        /// <param name="myVirtual">
        /// The my virtual.
        /// </param>
        /// <returns>
        /// The <see cref="Node"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static Node MySetNode(Face2 myFace, List<RealLink> myAdjacence, List<VirtualLink> myVirtual)
        {
            var mySurface = (Surface)myFace.GetSurface();
            if (mySurface == null)
            {
                throw new ArgumentNullException("myFace");
            }

            if (myFace == null)
            {
                throw new ArgumentNullException("myFace");
            }
                
            var idNode = myFace.GetHashCode();
            myFace.SetFaceId(idNode); 
           
            int numLoop = myFace.GetLoopCount();
            int numEdges = myFace.GetEdgeCount();
            double[] boundParameters = myFace.GetUVBounds();
            bool faceSense = myFace.FaceInSurfaceSense();

            switch (mySurface.Identity())
            {
                case 4001:
                    double[] myValuesPlane = mySurface.PlaneParams;
                    var normale = new double[3];
                    var rootPoint = new double[3];
                    Array.Copy(myValuesPlane, 0, normale, 0, 3);
                    Array.Copy(myValuesPlane, 3, rootPoint, 0, 3);
                    var nodoPiano = new PlaneNode(normale, rootPoint, idNode, numLoop, numEdges, boundParameters, faceSense, myAdjacence, myVirtual);
                    return nodoPiano;
                case 4002:
                    double[] myValuesCylinder = mySurface.CylinderParams;
                    double[] origineCilindro = new double[3];
                    double[] asseCilindro = new double[3];
                    Array.Copy(myValuesCylinder, 0, origineCilindro, 0, 3);
                    Array.Copy(myValuesCylinder, 3, asseCilindro, 0, 3);
                    var raggioCilindro = (double)myValuesCylinder.GetValue(6);
                    var complete = false;
                    var nodoCilindrico = new CylinderNode(origineCilindro, asseCilindro, raggioCilindro, complete, idNode, numEdges, numLoop, boundParameters, faceSense, myAdjacence, myVirtual);
                    return nodoCilindrico;
                case 4003:
                    double[] myValuesCone = mySurface.ConeParams;
                    double[] origineCono = new double[3];
                    double[] asseCono = new double[3];
                    Array.Copy(myValuesCone, 0, origineCono, 0, 3);
                    Array.Copy(myValuesCone, 3, asseCono, 0, 3);
                    var raggioCono = (double)myValuesCone.GetValue(6);
                    var angoloCono = (double)myValuesCone.GetValue(7);
                    var nodoConico = new ConeNode(origineCono, asseCono, raggioCono, angoloCono, idNode, numEdges, numLoop, boundParameters, faceSense, myAdjacence, myVirtual);
                    return nodoConico;
                case 4004:
                    double[] myValueSphere = mySurface.SphereParams;
                    double[] centroSfera = new double[3];
                    Array.Copy(myValueSphere, 0, centroSfera, 0, 3);
                    var raggioSfera = (double)myValueSphere.GetValue(3);
                    var nodoSperico = new SphereNode(centroSfera, raggioSfera, idNode, numLoop, numEdges, boundParameters, faceSense, myAdjacence, myVirtual);
                    return nodoSperico;
                case 4005:
                    double[] myValueTorus = mySurface.TorusParams;
                    double[] centroToro = new double[3];
                    double[] asseToro = new double[3];
                    Array.Copy(myValueTorus, 0, centroToro, 0, 3);
                    Array.Copy(myValueTorus, 3, asseToro, 0, 3);
                    var raggioMassimo = (double)myValueTorus.GetValue(6);
                    var raggioMinimo = (double)myValueTorus.GetValue(7);
                    var nodoToroidale = new TorusNode(asseToro, raggioMinimo, raggioMassimo, idNode, numLoop, numEdges, boundParameters, faceSense, myAdjacence, myVirtual);
                    return nodoToroidale;
                default:
                    var nodoInvalido = new InvalidNode(idNode, numLoop, numEdges, boundParameters, faceSense, myAdjacence, myVirtual);
                    return nodoInvalido;
            }
        }
    }
}
