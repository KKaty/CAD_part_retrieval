// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TipiDiArchi.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Utility type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SolidWorksAddinUtility
{
    using System;

    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Accord.Math;

    using SolidWorks.Interop.sldworks;

    using SolidWorks.Interop.swconst;

    using SWIntegration;

    using MySolidWorksAddIn;

    public partial class Utility
    {

        public const int ParalleloPieno = 10;
        public const int ParalleloVuoto = -10;
        public const int ParalleloConcorde = 100;
        public const int ParalleloDisconcorde = -100;
       
        /// <summary>
        /// The my set angle.
        /// </summary>
        /// <param name="angleEdge">
        /// The Angle Edge.
        /// </param>
        /// <param name="firstFace">
        /// The first face.
        /// </param>
        /// <param name="secondFace">
        /// The second face.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double MyTypeOfConnectionOfRealLink(Edge angleEdge, Face2 firstFace, Face2 secondFace, SldWorks mySwApplication)
        {
            Surface firstSurface = firstFace.GetSurface();
            Surface secondSurface = secondFace.GetSurface();
            Vertex startVertex = angleEdge.GetStartVertex();
            Vertex endVertex = angleEdge.GetEndVertex();
           // var startPoint = (Array)startVertex.GetPoint();
           // var endPoint = (Array)endVertex.GetPoint();
            var firstNormal = new double[3];
            var secondNormal = new double[3];
            double[] firstEvalutation;
            double[] secondEvalutation;
            var myEdgeDirection = Array.CreateInstance(typeof(double), 3);
            var curve = (Curve)angleEdge.GetCurve();

            double curveStartPoint = 0; // Il punto di valutazione della curva è dato in forma paramentrica, t \in [0,1]
            int numerOfDerivate = 1; // Nemero di derivate richieste nella valutazione della curva

            var curvaEvalutation = (Array)curve.Evaluate2(curveStartPoint, numerOfDerivate);
            myEdgeDirection.SetValue((double)curvaEvalutation.GetValue(3), 0);
            myEdgeDirection.SetValue((double)curvaEvalutation.GetValue(4), 1);
            myEdgeDirection.SetValue((double)curvaEvalutation.GetValue(5), 2);
            myEdgeDirection = MyNormalization(myEdgeDirection);
            
            if (angleEdge.EdgeInFaceSense(firstFace))
            {
                var x = (double)curvaEvalutation.GetValue(0);
                var y = (double)curvaEvalutation.GetValue(1);
                var z = (double)curvaEvalutation.GetValue(2);
                
                firstEvalutation = firstSurface.EvaluateAtPoint(x, y, z);
                secondEvalutation = secondSurface.EvaluateAtPoint(x, y, z);
                myEdgeDirection = MyLambdaVectorProduct(myEdgeDirection, -1);

            }
            else
            {
                var x = (double)curvaEvalutation.GetValue(0);
                var y = (double)curvaEvalutation.GetValue(1);
                var z = (double)curvaEvalutation.GetValue(2);

                firstEvalutation = firstSurface.EvaluateAtPoint(x, y, z);
                secondEvalutation = secondSurface.EvaluateAtPoint(x, y, z);
            }

            if (!firstFace.FaceInSurfaceSense())
            {
                firstNormal.SetValue((double)firstEvalutation.GetValue(0), 0);
                firstNormal.SetValue((double)firstEvalutation.GetValue(1), 1);
                firstNormal.SetValue((double)firstEvalutation.GetValue(2), 2);
            }
            else
            {
                firstNormal.SetValue(-(double)firstEvalutation.GetValue(0), 0);
                firstNormal.SetValue(-(double)firstEvalutation.GetValue(1), 1);
                firstNormal.SetValue(-(double)firstEvalutation.GetValue(2), 2);
            }

            if (!secondFace.FaceInSurfaceSense())
            {
                secondNormal.SetValue((double)secondEvalutation.GetValue(0), 0);
                secondNormal.SetValue((double)secondEvalutation.GetValue(1), 1);
                secondNormal.SetValue((double)secondEvalutation.GetValue(2), 2);
            }
            else
            {
                secondNormal.SetValue(-(double)secondEvalutation.GetValue(0), 0);
                secondNormal.SetValue(-(double)secondEvalutation.GetValue(1), 1);
                secondNormal.SetValue(-(double)secondEvalutation.GetValue(2), 2);
            }

            double angle = 0;
            angle = Utility.MyInnerProduct(firstNormal, Utility.MyVectorProduct(myEdgeDirection, secondNormal));
            return angle;
        }

        /// <summary>
        /// The my is parallel.
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
        ///  Al momento non uso questa funzione e verifico solo se le normali tra i piani sono discordi.
        private bool MyIsParallel(Node firstNode, Node secondNode)
        {
            const double TollParallel = 0.1;
            if ((firstNode is CylinderNode || firstNode is ConeNode || firstNode is TorusNode)
                && (secondNode is CylinderNode || secondNode is ConeNode || secondNode is TorusNode))
            {
                if (firstNode is CylinderNode && secondNode is CylinderNode)
                {
                    var firstSurface = (CylinderNode)firstNode;
                    var secondSurface = (CylinderNode)secondNode;

                    if (Math.Abs(Utility.MyInnerProduct(firstSurface.Axis, secondSurface.Axis)) - 1 < TollParallel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (firstNode is CylinderNode && secondNode is ConeNode)
                {
                    var firstSurface = (CylinderNode)firstNode;
                    var secondSurface = (ConeNode)secondNode;

                    if (Math.Abs(Utility.MyInnerProduct(firstSurface.Axis, secondSurface.Axis)) - 1 < TollParallel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (firstNode is CylinderNode && secondNode is TorusNode)
                {
                    var firstSurface = (CylinderNode)firstNode;
                    var secondSurface = (TorusNode)secondNode;

                    if (Math.Abs(Utility.MyInnerProduct(firstSurface.Axis, secondSurface.Axis)) - 1 < TollParallel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (firstNode is ConeNode && secondNode is ConeNode)
                {
                    var firstSurface = (ConeNode)firstNode;
                    var secondSurface = (ConeNode)secondNode;

                    if (Math.Abs(Utility.MyInnerProduct(firstSurface.Axis, secondSurface.Axis)) - 1 < TollParallel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (firstNode is ConeNode && secondNode is CylinderNode)
                {
                    var firstSurface = (ConeNode)firstNode;
                    var secondSurface = (CylinderNode)secondNode;

                    if (Math.Abs(Utility.MyInnerProduct(firstSurface.Axis, secondSurface.Axis)) - 1 < TollParallel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (firstNode is ConeNode && secondNode is TorusNode)
                {
                    var firstSurface = (ConeNode)firstNode;
                    var secondSurface = (TorusNode)secondNode;

                    if (Math.Abs(Utility.MyInnerProduct(firstSurface.Axis, secondSurface.Axis)) - 1 < TollParallel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (firstNode is TorusNode && secondNode is CylinderNode)
                {
                    var firstSurface = (TorusNode)firstNode;
                    var secondSurface = (CylinderNode)secondNode;

                    if (Math.Abs(Utility.MyInnerProduct(firstSurface.Axis, secondSurface.Axis)) - 1 < TollParallel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (firstNode is TorusNode && secondNode is ConeNode)
                {
                    var firstSurface = (TorusNode)firstNode;
                    var secondSurface = (ConeNode)secondNode;

                    if (Math.Abs(Utility.MyInnerProduct(firstSurface.Axis, secondSurface.Axis)) - 1 < TollParallel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (firstNode is TorusNode && secondNode is TorusNode)
                {
                    var firstSurface = (TorusNode)firstNode;
                    var secondSurface = (TorusNode)secondNode;

                    if (Math.Abs(Utility.MyInnerProduct(firstSurface.Axis, secondSurface.Axis)) - 1 < TollParallel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (firstNode is PlaneNode && secondNode is PlaneNode)
            {
                var firstSurface = (PlaneNode)firstNode;
                var secondSurface = (PlaneNode)secondNode;

                if (Math.Abs(Utility.MyInnerProduct(firstSurface.Normal, secondSurface.Normal)) - 1 < TollParallel)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (firstNode is PlaneNode && (secondNode is CylinderNode || secondNode is ConeNode || secondNode is TorusNode))
            {
                if (secondNode is CylinderNode)
                {
                    var firstSurface = (PlaneNode)firstNode;
                    var secondSurface = (CylinderNode)secondNode;

                    if (Math.Abs(Utility.MyInnerProduct(firstSurface.Normal, secondSurface.Axis)) < TollParallel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (secondNode is ConeNode)
                {
                    var firstSurface = (PlaneNode)firstNode;
                    var secondSurface = (ConeNode)secondNode;
                    if (Math.Abs(Utility.MyInnerProduct(firstSurface.Normal, secondSurface.Axis)) < TollParallel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (secondNode is TorusNode)
                {
                    var firstSurface = (PlaneNode)firstNode;
                    var secondSurface = (TorusNode)secondNode;
                    if (Math.Abs(Utility.MyInnerProduct(firstSurface.Normal, secondSurface.Axis)) < TollParallel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (secondNode is PlaneNode
                     && (firstNode is CylinderNode || firstNode is ConeNode || firstNode is TorusNode))
            {
                if (firstNode is CylinderNode)
                {
                    var firstSurface = (CylinderNode)firstNode;
                    var secondSurface = (PlaneNode)secondNode;

                    if (Math.Abs(Utility.MyInnerProduct(secondSurface.Normal, firstSurface.Axis)) < TollParallel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (firstNode is ConeNode)
                {
                    var firstSurface = (ConeNode)firstNode;
                    var secondSurface = (PlaneNode)secondNode;
                    if (Math.Abs(Utility.MyInnerProduct(secondSurface.Normal, firstSurface.Axis)) < TollParallel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (firstNode is TorusNode)
                {
                    var firstSurface = (TorusNode)firstNode;
                    var secondSurface = (PlaneNode)secondNode;
                    if (Math.Abs(Utility.MyInnerProduct(secondSurface.Normal, firstSurface.Axis)) < TollParallel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }
     
        /// <summary>
        /// The my type of connection.
        /// </summary>
        /// <param name="firstFace">
        /// The first face.
        /// </param>
        /// <param name="secondFace">
        /// The second face.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double MyTypeOfConnectionOfVirtualLink(Face2 firstFace, Face2 secondFace, SldWorks mySWApplication)
        {
            double typeOfConnection = 0;

            // Non controllo se le facce sono piane, ma lo faccio quando invoco la funzione.
            // Ricavo la normale alla faccia.

            double[] firstPoint;
            double[] secondPoint;
            var firstNormal = MyGetNormalForPlaneFace(firstFace, out firstPoint);
            var secondNormal = MyGetNormalForPlaneFace(secondFace, out secondPoint);

            // Verifico prima che i piani siano paralleli discordi, poi controllo le altre intersezioni.
            const double toll = 0.01;

            // Se il prodotto vettoriale è circa 1, allora sono parallele con normale concorde
            if(Math.Abs(MyInnerProduct(firstNormal, secondNormal) - 1) < toll)
            {
                typeOfConnection = ParalleloConcorde;
                return typeOfConnection;
            }

            // Se il prodotto vettoriale è circa -1, allora sono parallele con normale disconcorde
            else if (Math.Abs(MyInnerProduct(firstNormal, secondNormal) + 1) < toll)
            {
                typeOfConnection = ParalleloDisconcorde;
                return typeOfConnection;
            }

            // Se sono parallele discorde devo verificare se è pieno o vuoto, ma lo verifico nel salvataggio dei dati perché ho bisogno del modello.

             // Altrimenti non sono parallele e posso calcolare il tipo di connessione.
            else
            {
                // Equazione del piano del primo nodo
                var primoPiano = MyGetPlaneEquation(firstNormal, firstPoint);

                // Equazione del piano del secondo nodo
                var secondoPiano = MyGetPlaneEquation(secondNormal, secondPoint);

                // L'equzione cartesiana della retta r è ottenuta ponedo a sistema le equazioni dei due piani
                var rDirection = Utility.MyVectorProduct(primoPiano, secondoPiano);

                // Assegno il verso della retta in maniera arbitraria, poi verifico che sia corretto.
                var rVerse = rDirection;

                // Ricavo un edge della prima faccia non parallelo alla direzione della retta r
                // e tale che rDir x n1 sia concorde con il verso di percorrenza dell'edge. 
                var edgesOfFirstFace = (Array)firstFace.GetEdges();
                var edgeOfFirstFace = (Edge)edgesOfFirstFace.GetValue(0);

                // Ricavo uno spigolo della prima faccia per ottenere la retta r'
                var startVertex = (Vertex)edgeOfFirstFace.GetStartVertex();
                var startPoint = (Array)startVertex.GetPoint();
                var endVertex = (Vertex)edgeOfFirstFace.GetEndVertex();
                var endPoint = (Array)endVertex.GetPoint();
                Array edgeDirection;

                if (edgeOfFirstFace.EdgeInFaceSense(firstFace))
                {
                    edgeDirection = Utility.MyNormalization(Utility.MyVectorDifferent(startPoint, endPoint));
                }
                else
                {
                    edgeDirection = Utility.MyNormalization(Utility.MyVectorDifferent(endPoint, startPoint));
                }

                var index = 0;
                while (!(1 - Math.Abs(Utility.MyInnerProduct(edgeDirection, rVerse)) > 0.001))
                {
                    index++;
                    edgeOfFirstFace = (Edge)edgesOfFirstFace.GetValue(index);

                    // Ricavo uno spigolo della prima faccia per ottenere la retta r'
                    startVertex = (Vertex)edgeOfFirstFace.GetStartVertex();
                    startPoint = (Array)startVertex.GetPoint();
                    endVertex = (Vertex)edgeOfFirstFace.GetEndVertex();
                    endPoint = (Array)endVertex.GetPoint();

                    if (edgeOfFirstFace.EdgeInFaceSense(firstFace))
                    {
                        edgeDirection = Utility.MyNormalization(Utility.MyVectorDifferent(startPoint, endPoint));
                    }
                    else
                    {
                        edgeDirection = Utility.MyNormalization(Utility.MyVectorDifferent(endPoint, startPoint));
                    }
                }

                // Ho ottenuto un edge non parallelo, ora devo fissare il verso di percorrenza di r.

                double[] secondaEquazione;
                var primaEquazione = MyLineEquation(edgeDirection, startPoint, out secondaEquazione);

                // Combino le quattro equazioni trovate per scrivere il sitema Ax=b, che detrmina il punto di intersezione tra le due rette r e r'
                var puntoIntersezioneQ = MyPointIntersectionTwoPlane(primoPiano, secondoPiano, primaEquazione, secondaEquazione);

                // Si determina se il verso della retta r è corretto sapendo che il prodotto vettoriale tra due egde CONSECUTIVI
                // scalar la normale è maggiore di zero. Quindi occorre capire se si deve fare r' x r oppurre r x r'.
                double[] consecutiveEdge;

                if (edgeOfFirstFace.EdgeInFaceSense(firstFace))
                {
                    var memoria = startPoint;
                    startPoint = endPoint;
                    endPoint = memoria;
                }

                var distanceQS = Utility.MyDistanceTwoPoint(puntoIntersezioneQ, startPoint);
                var distanceQE = Utility.MyDistanceTwoPoint(puntoIntersezioneQ, endPoint);
                if (distanceQS < distanceQE)
                {
                    consecutiveEdge = (double[])Utility.MyVectorProduct(edgeDirection, rVerse);
                }
                else if (distanceQS > distanceQE)
                {
                    consecutiveEdge = (double[])Utility.MyVectorProduct(rVerse, edgeDirection);
                }
                else if (distanceQS == 0)
                {
                    consecutiveEdge = (double[])Utility.MyVectorProduct(rVerse, edgeDirection);
                }
                else
                {
                    consecutiveEdge = (double[])Utility.MyVectorProduct(edgeDirection, rVerse);
                }

                // Se il risultato è positivo, allora ho assegnato il verso corretto alla retta r, altrimenti lo devo invertire.
                if (Utility.MyInnerProduct(consecutiveEdge, firstNormal) < 0)
                {
                    rVerse = (double[])Utility.MyLambdaVectorProduct(rVerse, -1);
                }

                typeOfConnection = Utility.MyInnerProduct(firstNormal, Utility.MyVectorProduct(rVerse, secondNormal));
                return typeOfConnection;
            }
        }

        private static double[] MyLineEquation(Array edgeDirection, Array startPoint, out double[] secondaEquazione)
        {
            // Ricavo due vettori che servono per identificare l'equazione cartesiana della retta r' precedentemente descritta.
            // Per praticità riscrivo le componenti del vettore direzione della retta r' e il punto Sp per cui passa.
            var dirX = (double)edgeDirection.GetValue(0);
            var dirY = (double)edgeDirection.GetValue(1);
            var dirZ = (double)edgeDirection.GetValue(2);
            var puntoX = (double)startPoint.GetValue(0);
            var puntoY = (double)startPoint.GetValue(1);
            var puntoZ = (double)startPoint.GetValue(2);

            // Scrivo le due equazioni dei piani che identificano la retta r'
            var primaEquazione = new double[4] { dirY, -dirX, 0, dirX * puntoY - dirY * puntoX };
            secondaEquazione = new double[4] { dirZ, 0, -dirX, dirX * puntoZ - dirZ * puntoX };
            return primaEquazione;
        }

        public static double[] MyPointIntersectionTwoPlane(
            double[] primoPiano, double[] secondoPiano, double[] primaEquazione, double[] secondaEquazione)
        {
            double[,] matrice =
                {
                    {
                        (double)primoPiano.GetValue(0), (double)primoPiano.GetValue(1),
                        (double)primoPiano.GetValue(2)
                    },
                    {
                        (double)secondoPiano.GetValue(0), (double)secondoPiano.GetValue(1),
                        (double)secondoPiano.GetValue(2)
                    },
                    {
                        (double)primaEquazione.GetValue(0), (double)primaEquazione.GetValue(1),
                        (double)primaEquazione.GetValue(2)
                    },
                    {
                        (double)secondaEquazione.GetValue(0), (double)secondaEquazione.GetValue(1),
                        (double)secondaEquazione.GetValue(2)
                    }
                };

            double[,] terminiNoti =
                {
                    { -(double)primoPiano.GetValue(3) }, { -(double)secondoPiano.GetValue(3) },
                    { -(double)primaEquazione.GetValue(3) }, { -(double)secondaEquazione.GetValue(3) },
                };

            // Devo qudrare il sistema per poterlo risolverlo, così scrivo l'equazione normale A'Ax=A'b
            var matriceEqNormali = matrice.TransposeAndMultiply(matrice);
            var terminiNotiEqNormali = matrice.TransposeAndMultiply(terminiNoti);

            // Risolvo il sistema per determinare il punto di intersezione tra le due rette
            var matIntersezione = matriceEqNormali.Solve(terminiNotiEqNormali, true);
            var puntoIntersezioneQ = matIntersezione.GetColumn(0);
            return puntoIntersezioneQ;
        }

        public static double[] MyPointIntersectionLinePlane(
            double[] primoPianoRetta, double[] secondoPianoRetta, double[] EquazionePiano)
        {
            double[,] matrice =
                {
                    {
                        (double)primoPianoRetta.GetValue(0), (double)primoPianoRetta.GetValue(1),
                        (double)primoPianoRetta.GetValue(2)
                    },
                    {
                        (double)secondoPianoRetta.GetValue(0), (double)secondoPianoRetta.GetValue(1),
                        (double)secondoPianoRetta.GetValue(2)
                    },
                    {
                        (double)EquazionePiano.GetValue(0), (double)EquazionePiano.GetValue(1),
                        (double)EquazionePiano.GetValue(2)
                    }
                };

            double[,] terminiNoti =
                {
                    { -(double)primoPianoRetta.GetValue(3) }, { -(double)secondoPianoRetta.GetValue(3) },
                    { -(double)EquazionePiano.GetValue(3) }
                };

            // Risolvo il sistema per determinare il punto di intersezione tra le due rette
            var matIntersezione = matrice.Solve(terminiNoti, true);
            var puntoIntersezioneQ = matIntersezione.GetColumn(0);
            return puntoIntersezioneQ;
        }

        private static double[] MyGetPlaneEquation(double[] firstNormal, double[] firstPoint)
        {
            var primoPiano = new double[4];
            var k = (double)firstNormal.GetValue(0) * (double)firstPoint.GetValue(0)
                    + (double)firstNormal.GetValue(1) * (double)firstPoint.GetValue(1)
                    + (double)firstNormal.GetValue(2) * (double)firstPoint.GetValue(2);
            primoPiano.SetValue((double)firstNormal.GetValue(0), 0);
            primoPiano.SetValue((double)firstNormal.GetValue(1), 1);
            primoPiano.SetValue((double)firstNormal.GetValue(2), 2);
            primoPiano.SetValue(-k, 3);
            return primoPiano;
        }

        /// <summary>
        /// The my get normal for plane face.
        /// </summary>
        /// <param name="firstFace">
        /// The first face.
        /// </param>
        /// <param name="firstPoint">
        /// The first point.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here.")]
        private static double[] MyGetNormalForPlaneFace(Face2 firstFace, out double[] firstPoint)
        {
            var firstSurface = (Surface)firstFace.GetSurface();

            var firstNormal = new double[3];
            firstPoint = new double[3];
            var secondNormal = new double[3];
            var secondPoint = new double[3];
            Array firstValuesPlane = firstSurface.PlaneParams;
            if (firstSurface.IsPlane())
            {
                Array.Copy(firstValuesPlane, 0, firstNormal, 0, 3);
                Array.Copy(firstValuesPlane, 3, firstPoint, 0, 3);
            }

            // Pongo il verso della normale alla superficie concorde con quello della faccia.
            if (!firstFace.FaceInSurfaceSense())
            {
                firstNormal.SetValue((double)firstValuesPlane.GetValue(0), 0);
                firstNormal.SetValue((double)firstValuesPlane.GetValue(1), 1);
                firstNormal.SetValue((double)firstValuesPlane.GetValue(2), 2);
            }
            else
            {
                firstNormal.SetValue(-(double)firstValuesPlane.GetValue(0), 0);
                firstNormal.SetValue(-(double)firstValuesPlane.GetValue(1), 1);
                firstNormal.SetValue(-(double)firstValuesPlane.GetValue(2), 2);
            }
            return firstNormal;
        }

        // Funzione che verifica se esiste almeno un segmento tra i vertici delle due facce che non interseza il modello.
        // Quindi torna falsa se il modello è concavo corretto.
        public static bool  MyIsFull(Face2 firstFace, Face2 secondFace, double tipeOfConnection, ModelDoc2 myModel, SldWorks mySWApplication)
        {
            // Conto il numero di intersezioni 
            int numerOfIntersection = 0;
            // Estraggo i vertici delle delle due facce e conseguentemente i loro punti.
            List<Vertex> listVertexFirstFace = MyGetVertexFromFace(firstFace);
            List<Vertex> listVertexSecondFace = MyGetVertexFromFace(secondFace);

            foreach (Vertex firstVertex in listVertexFirstFace)
            {
                var firstPoint = (Array)firstVertex.GetPoint();

                foreach (Vertex secondVertex in listVertexSecondFace)
                {
                    var secondPoint = (Array) secondVertex.GetPoint();
                    /*
                    // Per ogni coppia di vertici v_i \in F_i e v_j \in F_j verifico se il segmento v_iv_j interseca l'oggetto.
                    var myRayParameters = new double[7];
                    myRayParameters.SetValue((double)firstPoint.GetValue(0), 0); // origin x
                    myRayParameters.SetValue((double)firstPoint.GetValue(1), 1);  // origin y
                    myRayParameters.SetValue((double)firstPoint.GetValue(2), 2); // origin z
                    myRayParameters.SetValue((double)secondPoint.GetValue(0) - (double)firstPoint.GetValue(0), 3); // direction x
                    myRayParameters.SetValue((double)secondPoint.GetValue(1) - (double)firstPoint.GetValue(1), 4); // direction y
                    myRayParameters.SetValue((double)secondPoint.GetValue(2) - (double)firstPoint.GetValue(2), 5); // direction z                 
                    int distance = 0;
                    myRayParameters.SetValue(distance, 6); // ray

                    bool intersection = myModel.SelectByRay(myRayParameters, (int)swSelectType_e.swSelFACES); 
                    if (intersection)
                    {
                        numerOfIntersection += 1;
                    }
                    */
                    var direction = new double[3];
                    direction.SetValue((double)secondPoint.GetValue(0) - (double)firstPoint.GetValue(0), 0); // direction x
                    direction.SetValue((double)secondPoint.GetValue(1) - (double)firstPoint.GetValue(1), 1); // direction y
                    direction.SetValue((double)secondPoint.GetValue(2) - (double)firstPoint.GetValue(2), 2); // direction z
                    double[] firstNormal;
                    double[] pointFirstFace;
                    firstNormal = MyGetNormalForPlaneFace(firstFace, out pointFirstFace);
                    var proofOfFull = MyInnerProduct(direction, firstNormal);
                    if (proofOfFull < 0 && proofOfFull > -1)
                    {
                        numerOfIntersection += 1;
                    }
                }
            }
            if (numerOfIntersection == listVertexFirstFace.Count * listVertexSecondFace.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Funzione che estrae i vertici di una faccia.
        public static List<Vertex> MyGetVertexFromFace(Face2 Face)
        {
            List<Vertex> listVertexFace = new List<Vertex>();

            var listEdgeFace = (Array)Face.GetEdges();

            foreach (Edge edge in listEdgeFace)
            {
                var vertexS = (Vertex)edge.GetStartVertex();
                if (!listVertexFace.Contains(vertexS))
                {
                    listVertexFace.Add(vertexS);
                }

                var vertexE = (Vertex)edge.GetEndVertex();
                if (!listVertexFace.Contains(vertexE))
                {
                    listVertexFace.Add(vertexE);
                }

            }
            return listVertexFace;
        }

    }
}
