// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utility.cs" company="">
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
    using SolidWorks.Interop.sldworks;

    /// <summary>
    /// The calcolo distanze.
    /// </summary>
    public partial class Utility
    {
        /// <summary>
        /// The my set lenght.
        /// </summary>
        /// <param name="edge">
        /// The edge.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double MySetLenght(Edge edge)
        {
            var curve = (Curve)edge.GetCurve();
            
            double startPoint = 0;
            double endPoint = 1;

            var lenght = curve.GetLength(startPoint, endPoint);
            return lenght;
        }

        /// <summary>
        /// The my point distance.
        /// </summary>
        /// <param name="firstPoint">
        /// The first point.
        /// </param>
        /// <param name="secondPoint">
        /// The second point.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double MyDistanceTwoPoint(Array firstPoint, Array secondPoint)
        {
            var x1 = (double)firstPoint.GetValue(0);
            var y1 = (double)firstPoint.GetValue(1);
            var z1 = (double)firstPoint.GetValue(2);

            var x2 = (double)secondPoint.GetValue(0);
            var y2 = (double)secondPoint.GetValue(1);
            var z2 = (double)secondPoint.GetValue(2);

            var distance = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2) + Math.Pow(z1 - z2, 2));

            return distance;
        }

        public static double MyDistanceParallelPlane(Face2 firstFace, Face2 secondFace)
        {
            double distance;
            double[] firstPoint;
            double[] secondPoint;

            var firstNormal = MyGetNormalForPlaneFace(firstFace, out firstPoint);
            var secondNormal = MyGetNormalForPlaneFace(secondFace, out secondPoint);
            var primoPiano = MyGetPlaneEquation(firstNormal, firstPoint);
            distance =
                Math.Abs(
                      (double)primoPiano.GetValue(0) * (double)secondPoint.GetValue(0)
                    + (double)primoPiano.GetValue(1) * (double)secondPoint.GetValue(1)
                    + (double)primoPiano.GetValue(2) * (double)secondPoint.GetValue(2) + (double)primoPiano.GetValue(3))
                / Math.Sqrt(Math.Pow((double)firstNormal.GetValue(0), 2) + Math.Pow((double)firstNormal.GetValue(1), 2) + Math.Pow((double)firstNormal.GetValue(2), 2));

            return distance;
        }

        public static double MyDistanceOfNonParallelPlane(Face2 firstFace, Face2 secondFace, out double distanceMax)
        {
            // Proietto ogni vertice della prima faccia sulla seconda e calcolo la distanza minima e la massima.
            double distanceMin = 100;
            distanceMax = 0;
            List<Vertex> listVertexFirstFace = MyGetVertexFromFace(firstFace);
            double[] firstPoint;
            double[] secondPoint;
            var firstNormal = MyGetNormalForPlaneFace(firstFace, out firstPoint);
            var secondNormal = MyGetNormalForPlaneFace(secondFace, out secondPoint);
            var secondPlaneEquation = MyGetPlaneEquation(secondNormal, secondPoint);

            // Calcolo la proiezione dei vertici della prima faccia
            foreach (Vertex vertex in listVertexFirstFace)
            {
                // Dal vertice ottengo il punto e lo proietto, considerando la retta passante per il punto e direzione firstNormal
                double[] point = vertex.GetPoint();

                double[] secondEquationLine;
                var firstEquationLine = MyLineEquation(firstNormal, point, out secondEquationLine);
                var pointProjection = MyPointIntersectionLinePlane(
                    firstEquationLine, secondEquationLine, secondPlaneEquation);

                var distanceTest = MyDistanceTwoPoint(point, pointProjection);

                if (distanceTest > distanceMax)
                {
                    distanceMax = distanceTest;
                }
                if (distanceTest < distanceMin)
                {
                    distanceMin = distanceTest;
                }
            }

            return distanceMin;
        }
    }
}
