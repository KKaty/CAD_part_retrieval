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
    using SolidWorks.Interop.sldworks;
    using SWIntegration;
    using SWIntegration.Data_Structure;


    /// <summary>
    /// The salvataggio.
    /// </summary>
    public partial class Utility
    {


        public static Graph MySaveData(List<Face2> mySelectedFace, ModelDoc2 myModel, SldWorks mySWApplication)
        {
            var nodeList = new List<Node>();
            foreach (Face2 face in mySelectedFace)
            {
                // Per ogni faccia vengono creati gli archi reali
                var myRealLinks = MySetRealLink(face, mySWApplication);
                var myVirtualLinks = MySetVirtualLink(face, mySelectedFace, myModel, mySWApplication);
                var myNode = MySetNode(face, myRealLinks, myVirtualLinks);
                nodeList.Add(myNode);
            }
            var myGraph = new Graph(nodeList);
            return myGraph;

        }


        private static List<Face2> MyAdjacenceFace(Face2 face)
        {
            var listAdjacentFace = new List<Face2>();
            // Aggiungo la faccia stessa e tutte le sue adiacenti,
            // Se non aggiungo anche se stessa poi controlla la presenza di edge virtuali tra sé e sé.
            listAdjacentFace.Add(face);

            var myEdgeOfFace = (Array)face.GetEdges();
            foreach (Edge edge in myEdgeOfFace)
            {
                Array myAdiacenceFaces = edge.GetTwoAdjacentFaces2();
                Face2 myAdiacenceFace = null;
                var firstFace = (Face2)myAdiacenceFaces.GetValue(0);
                var secondFace = (Face2)myAdiacenceFaces.GetValue(1);

                // Se la faccia non è quella che ho già la salvo come faccia adiacente, altrimenti salvo l'altra.
                // Se non c'è un'altra faccia adiacente allora ho un valore NULL e non salvo niente.

                if (firstFace == face)
                {
                    myAdiacenceFace = secondFace;
                }
                else if (secondFace == face)
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
        /// <param name="myFace">
        /// The my face.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static List<RealLink> MySetRealLink(Face2 myFace, SldWorks mySwApplication)
        {
            var myAdiacenceNodes = new List<RealLink>();

            var myEdgeOfFace = (Array)myFace.GetEdges();
            foreach (Edge edge in myEdgeOfFace)
            {
                Array myAdiacenceFaces = edge.GetTwoAdjacentFaces2();
                Face2 myAdiacenceFace = null;
                var firstFace = (Face2)myAdiacenceFaces.GetValue(0);
                var secondFace = (Face2)myAdiacenceFaces.GetValue(1);

                // Se la faccia non è quella che ho già la salvo come faccia adiacente, altrimenti salvo l'altra.
                // Se non c'è un'altra faccia adiacente allora ho un valore NULL e non salvo niente.
                if (firstFace == myFace)
                {
                    myAdiacenceFace = secondFace;
                }
                else if (secondFace == myFace)
                {
                    myAdiacenceFace = firstFace;
                }

                if (myAdiacenceFace != null)
                {
                    var angle = Utility.MyTypeOfConnectionOfRealLink(edge, myFace, myAdiacenceFace, mySwApplication);
                    mySwApplication.SendMsgToUser("Angolo impostato" + angle.ToString());
                    var lenght =Utility.MySetLenght(edge);
                    mySwApplication.SendMsgToUser("Lunghezza impostato" + lenght.ToString());
                    var myAdiacenceOfAdiacenceNode = new List<RealLink>();
                    var myVirtualOfVirtualNode = new List<VirtualLink>();
                    var myRealLink = new RealLink(
                        angle, lenght, MySetNode(myAdiacenceFace, myAdiacenceOfAdiacenceNode, myVirtualOfVirtualNode));
                    myAdiacenceNodes.Add(myRealLink);
                }

            }
            return myAdiacenceNodes;
        }

        /// <summary>
        /// The my set virtual link.
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
        private static List<VirtualLink> MySetVirtualLink(Face2 face, List<Face2> mySelectedFace, ModelDoc2 myModel, SldWorks mySWApplication)
       {
           var virtualLinks = new List<VirtualLink>();

           // Per ogni faccia creo una lista delle sue facce incidenti (quelle con cui si è un edge reale) per evitare di creare archi
           // virtuali tra facce adiacenti
           var listAdjacentFace = MyAdjacenceFace(face);

           // Se le due facce sono piane, allora ha senso verificare il tipo di connessione.
           foreach (Face2 face2 in mySelectedFace)
           {
               if (!listAdjacentFace.Contains(face2))
               {
                   var firstSurface = (Surface)face.GetSurface();
                   var secondSurface = (Surface)face2.GetSurface();

                   if (firstSurface.Identity() == 4001 & secondSurface.Identity() == 4001)
                   {
                       // scarto i tipi di connessione che non sono simmetrici, dati dai casi convessi non interessanti.
                       var myFirstTypeOfConnection = MyTypeOfConnectionOfVirtualLink(face, face2, mySWApplication);
                       var mySecondTypeOfConnection = MyTypeOfConnectionOfVirtualLink(face2, face, mySWApplication);

                       if (myFirstTypeOfConnection == mySecondTypeOfConnection)
                       {
                           // Verifico se il caso concavo/convesso è corretto, poi verifico quelli paralleli se sono pieni o vuoti
                          /* if (IsCorrectTypeOfConnection(face, face2, myFirstTypeOfConnection, myModel))
                           {
                               mySWApplication.SendMsgToUser("la connessione è corretta");
                               double dist = 0;
                               var adj = new List<RealLink>();
                               var vir = new List<VirtualLink>();
                               var virtualLink = new VirtualLink(myFirstTypeOfConnection, dist, MySetNode(face2, adj, vir));
                               virtualLinks.Add(virtualLink);
                           }
                           */

                            // se il tipo di connessione è parallela si verifica se è piena o vuota.
                           if (myFirstTypeOfConnection != ParalleloConcorde)
                           {
                               if (MyIsFull(face, face2, myFirstTypeOfConnection, myModel, mySWApplication) &
                                   MyIsFull(face2, face, myFirstTypeOfConnection, myModel, mySWApplication))
                               {
                                   if (myFirstTypeOfConnection == ParalleloDisconcorde)
                                   {
                                       double dist = MyDistanceParallelPlane(face, face2);
                                       mySWApplication.SendMsgToUser("sono parallelo pieno con distanza " + dist.ToString());
                                       var adj = new List<RealLink>();
                                       var vir = new List<VirtualLink>();
                                       var virtualLink = new VirtualLink(ParalleloPieno, dist, MySetNode(face2, adj, vir));
                                       virtualLinks.Add(virtualLink);
                                   }

                                   if (myFirstTypeOfConnection > 0 && myFirstTypeOfConnection <= 1)
                                   {
                                       double distMin;
                                       double distMax;
                                       double distMaxff2;
                                       double distMaxf2f;
                                       double distff2 = MyDistanceOfNonParallelPlane(face, face2, out distMaxff2);
                                       double distf2f = MyDistanceOfNonParallelPlane(face2, face, out distMaxf2f);
                                       if (distff2 > distf2f)
                                       {
                                           distMin = distf2f;
                                       }
                                       else
                                       {
                                           distMin =distff2;
                                       }

                                          if (distMaxff2 > distMaxf2f)
                                       {
                                           distMax = distMaxff2;
                                       }
                                       else
                                       {
                                           distMax = distMaxf2f;
                                       }
                                       mySWApplication.SendMsgToUser("sono convesso con distanza minima " + distMin.ToString()
                                                                     + " e distanza massima " + distMax.ToString());
                                       var adj = new List<RealLink>();
                                       var vir = new List<VirtualLink>();
                                       var virtualLink = new VirtualLink(myFirstTypeOfConnection, distMin, MySetNode(face2, adj, vir));
                                       virtualLinks.Add(virtualLink);
                                   }
                               }
                               else if (!MyIsFull(face, face2, myFirstTypeOfConnection, myModel, mySWApplication))
                               {
                                   if (myFirstTypeOfConnection == ParalleloDisconcorde)
                                   {
                                       double dist = MyDistanceParallelPlane(face, face2);
                                       var adj = new List<RealLink>();
                                       var vir = new List<VirtualLink>();
                                       var virtualLink = new VirtualLink(ParalleloVuoto, dist, MySetNode(face2, adj, vir));
                                       virtualLinks.Add(virtualLink);
                                       mySWApplication.SendMsgToUser("sono parallelo vuoto da verificare con distanza " + dist.ToString());
                                   }
                                   if (myFirstTypeOfConnection < 0 & myFirstTypeOfConnection >= -1)
                                   {
                                       double distMin;
                                       double distMax;
                                       double distMaxff2;
                                       double distMaxf2f;
                                       double distff2 = MyDistanceOfNonParallelPlane(face, face2, out distMaxff2);
                                       double distf2f = MyDistanceOfNonParallelPlane(face2, face, out distMaxf2f);
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
                                       mySWApplication.SendMsgToUser("sono concavo con distanza minima " + distMin.ToString()
                                                                    + " e distanza massima " + distMax.ToString());
                                       var adj = new List<RealLink>();
                                       var vir = new List<VirtualLink>();
                                       var virtualLink = new VirtualLink(myFirstTypeOfConnection, distMin, MySetNode(face2, adj, vir));
                                       virtualLinks.Add(virtualLink);
                                   }
                                   /*
                                   double dist = 0;
                                   var adj = new List<RealLink>();
                                   var vir = new List<VirtualLink>();
                                   var virtualLink = new VirtualLink(ParalleloPieno, dist, MySetNode(face2, adj, vir));
                                   virtualLinks.Add(virtualLink);
                                */  

                               }
                           }
                       }
                   }

               }
           }
           return virtualLinks;
       }


       // TERMINARE DI SCRIVERE LA FUNZIONE, SOPRATTUTTO PER I CASI CONVESSI!!!!
     /*  private static bool IsCorrectTypeOfConnection(Face2 face, Face2 face2, double typeOfConnection, ModelDoc2 myModel)
       {
           // se il tipo di connessione è concava, allora devo verificare che le facce si vedano.
           if (typeOfConnection >= -1 &&typeOfConnection < 0)
           {
               // Per ogni coppia di vertici devo controllare che non intersechi l'oggetto.
               if(MyIsFull(face, face2, typeOfConnection, myModel))
               {
                return true;
               }
           }

           // se il tipo di connessione è convessa?? Pensare ad una soluzione
           else if (typeOfConnection > 0 && typeOfConnection <= 1)
           {
               return true; // Per ora ritorna vero, ma è da modificare, controllare se l'intersezione cade in una faccia!
           }
           return false;
       }
        */

       public static Node MySetNode(Face2 myFace, List<RealLink> myAdjacence, List<VirtualLink> myVirtual)
        {
            Surface mySurface = myFace.GetSurface();
            if (mySurface == null)
            {
                throw new ArgumentNullException("myFace");
            }

            if (myFace == null)
            {
                throw new ArgumentNullException("myFace");
            }

            int idNode = myFace.GetFaceId();
            int numLoop = myFace.GetLoopCount();
            int numEdges = myFace.GetEdgeCount();
            Array boundParameters = myFace.GetUVBounds();
            bool faceSense = myFace.FaceInSurfaceSense();

            switch (mySurface.Identity())
            {
                case 4001:
                    Array myValuesPlane = mySurface.PlaneParams;
                    var normale = new double[3];
                    var rootPoint = new double[3];
                    Array.Copy(myValuesPlane, 0, normale, 0, 3);
                    Array.Copy(myValuesPlane, 3, rootPoint, 0, 3);
                    var nodoPiano = new PlaneNode(normale, rootPoint, idNode, numLoop, numEdges, boundParameters, faceSense, myAdjacence, myVirtual);
                    return nodoPiano;
                case 4002:
                    Array myValuesCylinder = mySurface.CylinderParams;
                    Array origineCilindro = new double[3];
                    Array asseCilindro = new double[3];
                    Array.Copy(myValuesCylinder, 0, origineCilindro, 0, 3);
                    Array.Copy(myValuesCylinder, 3, asseCilindro, 0, 3);
                    var raggioCilindro = (double)myValuesCylinder.GetValue(5);
                    var nodoCilindrico = new CylinderNode(origineCilindro, asseCilindro, raggioCilindro, idNode, numEdges, numLoop, boundParameters, faceSense, myAdjacence, myVirtual);
                    return nodoCilindrico;
                case 4003:
                    Array myValuesCone = mySurface.ConeParams;
                    Array origineCono = new double[3];
                    Array asseCono = new double[3];
                    Array.Copy(myValuesCone, 0, origineCono, 0, 3);
                    Array.Copy(myValuesCone, 3, asseCono, 0, 3);
                    var raggioCono = (double)myValuesCone.GetValue(5);
                    var angoloCono = (double)myValuesCone.GetValue(6);
                    var nodoConico = new ConeNode(origineCono, asseCono, raggioCono, angoloCono, idNode, numEdges, numLoop, boundParameters, faceSense, myAdjacence, myVirtual);
                    return nodoConico;
                case 4004:
                    Array myValueSphere = mySurface.SphereParams;
                    Array centroSfera = new double[3];
                    Array.Copy(myValueSphere, 0, centroSfera, 0, 3);
                    var raggioSfera = (double)myValueSphere.GetValue(5);
                    var nodoSperico = new SphereNode(centroSfera, raggioSfera, idNode, numLoop, numEdges, boundParameters, faceSense, myAdjacence, myVirtual);
                    return nodoSperico;
                case 4005:
                    Array myValueTorus = mySurface.TorusParams;
                    Array centroToro = new double[3];
                    Array asseToro = new double[3];
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
