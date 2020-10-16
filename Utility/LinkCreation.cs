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

    using SWIntegration;

    /// <summary>
    /// The utility.
    /// </summary>
    public partial class Utility
    {
        /// <summary>
        /// The parallelo pieno, valore assunto da "tipo di connessione" quando è parallelo pieno.
        /// </summary>
        public const int ParalleloPieno = 10;

        /// <summary>
        /// The parallelo vuoto, valore assunto da "tipo di connessione" quando è parallelo vuoto. 
        /// </summary>
        public const int ParalleloVuoto = -10;

        /// <summary>
        /// The parallelo concorde, valore assunto in fase di costruzione da "tipo di connessione" quando è parallelo concorde
        /// quindi quando non è significativo studiare se è pieno o vuoto.
        /// </summary>
        public const int ParalleloConcorde = 100;

        /// <summary>
        /// The parallelo disconcorde, valore assunto da "tipo di connessione" quando è parallelo pieno,
        /// quindi quando è significativo studiare se è pieno o vuoto.
        /// </summary>
        public const int ParalleloDisconcorde = -100;


        /// <summary>
        /// The Liscio, valore assunto da "tipo di connessione" quando le due facce piane sono complanari oppure coassiali nel caso cilindriche.
        /// </summary>
        public const int Liscio = 0;

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
        /// <param name="mySwApplication">
        /// The my Sw Application.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private static double MyTypeOfConnectionOfRealLink(Edge angleEdge, Face2 firstFace, Face2 secondFace, SldWorks mySwApplication)
        {
           
            var firstSurface = (Surface)firstFace.GetSurface();
            var secondSurface = (Surface)secondFace.GetSurface();
            var firstNormal = new double[3];
            var secondNormal = new double[3];
            double[] firstEvalutation;
            double[] secondEvalutation;
            var myEdgeDirection = new double[3];
            var curve = (Curve)angleEdge.GetCurve();

            const double CurveStartPoint = 0.1; // Il punto di valutazione della curva è dato in forma paramentrica, t \in [0,1]
            const int NumerOfDerivate = 1; // Nemero di derivate richieste nella valutazione della curva

            var curvaEvalutation = (Array)curve.Evaluate2(CurveStartPoint, NumerOfDerivate);
            myEdgeDirection.SetValue((double)curvaEvalutation.GetValue(3), 0);
            myEdgeDirection.SetValue((double)curvaEvalutation.GetValue(4), 1);
            myEdgeDirection.SetValue((double)curvaEvalutation.GetValue(5), 2);
            myEdgeDirection = (double[])MyNormalization(myEdgeDirection);

            var x = (double)curvaEvalutation.GetValue(0);
            var y = (double)curvaEvalutation.GetValue(1);
            var z = (double)curvaEvalutation.GetValue(2);

            firstEvalutation = firstSurface.EvaluateAtPoint(x, y, z);
            secondEvalutation = secondSurface.EvaluateAtPoint(x, y, z);

            if (angleEdge.EdgeInFaceSense(firstFace))
            {
               
                myEdgeDirection = (double[])MyLambdaVectorProduct(myEdgeDirection, -1);
            }

            if (firstEvalutation != null)
            {
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
            }

            if (secondEvalutation != null)
            {
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
            }


            double angle = 0.0;
            if (firstEvalutation != null && secondEvalutation != null)
            {
                angle = Utility.MyInnerProduct(firstNormal, Utility.MyVectorProduct(myEdgeDirection, secondNormal));
            }
            else
            {
                //mySwApplication.SendMsgToUser("Normali nulle");
                return 5; //--> Valore per cui non si deve inserire l'arco perché è errato.
            }
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
        /// Al momento non uso questa funzione e verifico solo se le normali tra i piani sono discordi.
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
        /// <param name="mySWApplication">
        /// The my SW Application.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private static double MyTypeOfConnectionOfVirtualLink(Face2 firstFace, Face2 secondFace, SldWorks mySWApplication)
        {
            double typeOfConnection = 0;

            // Non controllo se le facce sono piane, ma lo faccio quando invoco la funzione.
            // Ricavo la normale alla faccia.
            double[] firstPoint;
            double[] secondPoint;

            var firstNormal = MyGetNormalForPlaneFace(firstFace, out firstPoint);
            var secondNormal = MyGetNormalForPlaneFace(secondFace, out secondPoint);
            
            // Verifico prima che i piani siano paralleli discordi, poi controllo le altre intersezioni.
            const double Toll = 0.01;

            // Se il prodotto vettoriale è circa 1, allora sono parallele con normale concorde
            if (Math.Abs(MyInnerProduct(firstNormal, secondNormal) - 1) < Toll)
            {
                typeOfConnection = ParalleloConcorde;
                return typeOfConnection;
            }

            // Se il prodotto vettoriale è circa -1, allora sono parallele con normale disconcorde
            else if (Math.Abs(MyInnerProduct(firstNormal, secondNormal) + 1) < Toll)
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
                var numberOfEdge = firstFace.GetEdgeCount();
                var index = 0;
                var edgeOfFirstFace = (Edge)edgesOfFirstFace.GetValue(index);
                var curve = (Curve)edgeOfFirstFace.GetCurve();
                
                if (!curve.IsLine())
                {
                    var prova = (Array)curve.Evaluate2(0, 1);
                    var prova2 = (Array)curve.Evaluate2(1, 1);
                }

                while (!curve.IsLine() & index < numberOfEdge - 1)
                {
                    index++;
                    edgeOfFirstFace = (Edge)edgesOfFirstFace.GetValue(index);
                    curve = (Curve)edgeOfFirstFace.GetCurve();
                }

                if (index < numberOfEdge - 1)
                {
                    // Ricavo uno spigolo della prima faccia per ottenere la retta r'
                    var startVertex = (Vertex)edgeOfFirstFace.GetStartVertex();
                    Array startPoint;
                    try
                    {
                        startPoint = (Array)startVertex.GetPoint();
                    }
                    catch (Exception)
                    {

                        throw new Exception("StartPoint");
                    }
                   
                    var endVertex = (Vertex)edgeOfFirstFace.GetEndVertex();
                    Array endPoint;
                    try
                    {
                        endPoint = (Array)endVertex.GetPoint();
                    }
                    catch (Exception)
                    {
                        
                        throw new Exception("EndPoint");
                    }
                   
                    Array edgeDirection;

                    if (edgeOfFirstFace.EdgeInFaceSense(firstFace))
                    {
                        edgeDirection = Utility.MyNormalization(Utility.MyVectorDifferent(startPoint, endPoint));
                    }
                    else
                    {
                        edgeDirection = Utility.MyNormalization(Utility.MyVectorDifferent(endPoint, startPoint));
                    }

                    /*
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

                    */
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
                }
                else
                {
                    typeOfConnection = -100;
                }

                return typeOfConnection;
            }
        }

        /// <summary>
        /// The my line equation.
        /// </summary>
        /// <param name="edgeDirection">
        /// The edge direction.
        /// </param>
        /// <param name="startPoint">
        /// The start point.
        /// </param>
        /// <param name="secondaEquazione">
        /// The seconda equazione.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
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

        /// <summary>
        /// The my point intersection two plane.
        /// </summary>
        /// <param name="primoPiano">
        /// The primo piano.
        /// </param>
        /// <param name="secondoPiano">
        /// The secondo piano.
        /// </param>
        /// <param name="primaEquazione">
        /// The prima equazione.
        /// </param>
        /// <param name="secondaEquazione">
        /// The seconda equazione.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        private static double[] MyPointIntersectionTwoPlane(
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

        /// <summary>
        /// The my point intersection line plane.
        /// </summary>
        /// <param name="primoPianoRetta">
        /// The primo piano retta.
        /// </param>
        /// <param name="secondoPianoRetta">
        /// The secondo piano retta.
        /// </param>
        /// <param name="equazionePiano">
        /// The equazione piano.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        private static double[] MyPointIntersectionLinePlane(
            double[] primoPianoRetta, double[] secondoPianoRetta, double[] equazionePiano)
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
                        (double)equazionePiano.GetValue(0), (double)equazionePiano.GetValue(1),
                        (double)equazionePiano.GetValue(2)
                    }
                };

            double[,] terminiNoti =
                {
                    { -(double)primoPianoRetta.GetValue(3) }, { -(double)secondoPianoRetta.GetValue(3) },
                    { -(double)equazionePiano.GetValue(3) }
                };

            // Risolvo il sistema per determinare il punto di intersezione tra le due rette
            var matIntersezione = matrice.Solve(terminiNoti, true);
            var puntoIntersezioneQ = matIntersezione.GetColumn(0);
            return puntoIntersezioneQ;
        }

        /// <summary>
        /// The my get plane equation.
        /// </summary>
        /// <param name="firstNormal">
        /// The first normal.
        /// </param>
        /// <param name="firstPoint">
        /// The first point.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
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
        /// The my is full.
        /// Funzione che verifica se esiste almeno un segmento tra i vertici delle due facce che non interseza il modello.
        /// Quindi torna falsa se il modello è concavo corretto.
        /// </summary>
        /// <param name="firstFace">
        /// The first face.
        /// </param>
        /// <param name="secondFace">
        /// The second face.
        /// </param>
        /// <param name="tipeOfConnection">
        /// The tipe of connection.
        /// </param>
        /// <param name="myModel">
        /// The my model.
        /// </param>
        /// <param name="mySWApplication">
        /// The my sw application.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool MyIsFull(Face2 firstFace, Face2 secondFace, double tipeOfConnection, ModelDoc2 myModel, SldWorks mySWApplication)
        {
            // Conto il numero di intersezioni 
            int numerOfIntersection = 0;
            
            // Estraggo i vertici delle delle due facce e conseguentemente i loro punti.
            var saveFirstFace = ((Entity)firstFace).GetSafeEntity();
            var swSafeFirstEntity = (Entity)saveFirstFace.GetSafeEntity();

            var saveSecondFace = ((Entity)secondFace).GetSafeEntity();
            var swSafeSecondEntity = (Entity)saveSecondFace.GetSafeEntity();

            List<Vertex> listVertexFirstFace = MyGetVertexFromFace(swSafeFirstEntity);
            List<Vertex> listVertexSecondFace = MyGetVertexFromFace(swSafeSecondEntity);

            var listVertexFirstFaceCount = listVertexFirstFace.Count;
            var listVertexSecondFaceCount = listVertexSecondFace.Count;
            
            foreach (Vertex firstVertex in listVertexFirstFace)
            {
                var firstPoint = (Array)firstVertex.GetPoint();
                foreach (Vertex secondVertex in listVertexSecondFace)
                {
                    var secondPoint = (Array)secondVertex.GetPoint();
                    if (firstPoint != null && secondPoint != null)
                    {
                        var direction = new double[3];
                        direction.SetValue((double)secondPoint.GetValue(0) - (double)firstPoint.GetValue(0), 0);
                            // direction x
                        direction.SetValue((double)secondPoint.GetValue(1) - (double)firstPoint.GetValue(1), 1);
                            // direction y
                        direction.SetValue((double)secondPoint.GetValue(2) - (double)firstPoint.GetValue(2), 2);
                            // direction z
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
            }
            
            if (numerOfIntersection == listVertexFirstFaceCount * listVertexSecondFaceCount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// The my get vertex from face.
        /// Funzione che estrae i vertici di una faccia solo se l'edge formato da quei vertici è una linea!
        /// </summary>
        /// <param name="face">
        /// The face.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<Vertex> MyGetVertexFromFace(Entity entity)
        {
            var face = (Face2)entity.GetSafeEntity(); 
            var listVertexFace = new List<Vertex>();
            var listEdgeFaceCount = face.GetEdgeCount(); // Anche se non è utilizzato è necessario per non far disconnettere l'oggetto, evitando il refresh.
            var listEdgeFace = (Array)face.GetEdges();

            foreach (Edge edge in listEdgeFace)
            {
                if (edge != null)
                {
                    var edgeCurve = (Curve)edge.GetCurve();
                    if (edgeCurve.IsLine())  // --> Non è da fare solo per edge piani, ma per tutti, con piccolo accorgimento
                        //Se lo start vertex o l'end vertex sono nulli, allora si chiama la funzione MyPointOfCircleEdge e si salva quel punto.
                    {
                        var vertexS = (Vertex)edge.GetStartVertex(); // Se è nullo si chiama MyPointOfCircleEdge con t=0
                        if (!listVertexFace.Contains(vertexS))
                        {
                            listVertexFace.Add(vertexS);
                        }

                        var vertexE = (Vertex)edge.GetEndVertex(); // Se è nullo si chiama MyPointOfCircleEdge con t=1
                        if (!listVertexFace.Contains(vertexE))
                        {
                            listVertexFace.Add(vertexE);
                        }
                    }
                }
            }

            return listVertexFace;
        }
    }
}
