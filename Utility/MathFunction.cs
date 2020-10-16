// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MathFunction.cs" company="">
//   
// </copyright>
// <summary>
//   The funzioni per calcoli.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace SolidWorksAddinUtility
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using SolidWorks.Interop.sldworks;

    /// <summary>
    /// The funzioni per calcoli.
    /// </summary>
    public partial class Utility
    {
        /// <summary>
        /// The my inner product.
        /// </summary>
        /// <param name="first">
        /// The first.
        /// </param>
        /// <param name="second">
        /// The second.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
        public static double MyInnerProduct(Array first, Array second)
        {
            var x1 = (double)first.GetValue(0);
            var x2 = (double)first.GetValue(1);
            var x3 = (double)first.GetValue(2);

            var y1 = (double)second.GetValue(0);
            var y2 = (double)second.GetValue(1);
            var y3 = (double)second.GetValue(2);

            var innerProduct = x1 * y1 + x2 * y2 + x3 * y3;
            return innerProduct;
        }

        /// <summary>
        /// The my vector product.
        /// </summary>
        /// <param name="first">
        /// The first.
        /// </param>
        /// <param name="second">
        /// The second.
        /// </param>
        /// <returns>
        /// The <see cref="Array"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
        public static Array MyVectorProduct(Array first, Array second)
        {
            var x1 = (double)first.GetValue(0);
            var x2 = (double)first.GetValue(1);
            var x3 = (double)first.GetValue(2);

            var y1 = (double)second.GetValue(0);
            var y2 = (double)second.GetValue(1);
            var y3 = (double)second.GetValue(2);

            Array vectorProduct = new double[3];
            vectorProduct.SetValue(x2 * y3 - x3 * y2, 0);
            vectorProduct.SetValue(x3 * y1 - x1 * y3, 1);
            vectorProduct.SetValue(x1 * y2 - x2 * y1, 2);
            return vectorProduct;
        }

        /// <summary>
        /// The my vector different.
        /// </summary>
        /// <param name="first">
        /// The first.
        /// </param>
        /// <param name="second">
        /// The second.
        /// </param>
        /// <returns>
        /// The <see cref="Array"/>.
        /// </returns>
        public static Array MyVectorDifferent(Array first, Array second)
        {
            var x1 = (double)first.GetValue(0);
            var x2 = (double)first.GetValue(1);
            var x3 = (double)first.GetValue(2);

            var y1 = (double)second.GetValue(0);
            var y2 = (double)second.GetValue(1);
            var y3 = (double)second.GetValue(2);

            Array vectorDifferent = new double[3];
            vectorDifferent.SetValue(x1 - y1, 0);
            vectorDifferent.SetValue(x2 - y2, 1);
            vectorDifferent.SetValue(x3 - y3, 2);
            return vectorDifferent;
        }

        /// <summary>
        /// The my normalization.
        /// </summary>
        /// <param name="first">
        /// The first.
        /// </param>
        /// <returns>
        /// The <see cref="Array"/>.
        /// </returns>
        public static Array MyNormalization(Array first)
        {
            var x1 = (double)first.GetValue(0);
            var x2 = (double)first.GetValue(1);
            var x3 = (double)first.GetValue(2);

            var norma = Math.Sqrt(Math.Pow(x1, 2) + Math.Pow(x2, 2) + Math.Pow(x3, 2));

            first.SetValue(x1 / norma, 0);
            first.SetValue(x2 / norma, 1);
            first.SetValue(x3 / norma, 2);

            return first;
        }

        /// <summary>
        /// The my lambda vector product.
        /// </summary>
        /// <param name="first">
        /// The first.
        /// </param>
        /// <param name="lambda">
        /// The lambda.
        /// </param>
        /// <returns>
        /// The <see cref="Array"/>.
        /// </returns>
        public static Array MyLambdaVectorProduct(Array first, double lambda)
        {
            var x1 = (double)first.GetValue(0);
            var x2 = (double)first.GetValue(1);
            var x3 = (double)first.GetValue(2);

            first.SetValue(x1 * lambda, 0);
            first.SetValue(x2 * lambda, 1);
            first.SetValue(x3 * lambda, 2);

            return first;
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
        private static double[] MyGetNormalForPlaneFace(Face2 firstFace, out double[] firstPoint)
        {
            var firstSurface = (Surface)firstFace.GetSurface();
            var firstNormal = new double[3];
            firstPoint = new double[3];
            var secondNormal = new double[3];
            var secondPoint = new double[3];
            Array firstValuesPlane = null;

            if (firstSurface.IsPlane())
                {
                    firstValuesPlane = firstSurface.PlaneParams;
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
    }
}
