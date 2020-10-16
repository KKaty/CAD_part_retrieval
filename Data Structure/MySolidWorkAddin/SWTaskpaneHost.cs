// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SWTaskpaneHost.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the SWTaskpaneHost type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace AngelSix
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using SolidWorks.Interop.sldworks;
    using SolidWorks.Interop.swconst;
    using SWIntegration;
    using SWIntegration.Data_Structure;
    using MySolidWorksAddIn;
     
    /// <summary>
    /// The sw taskpane host.
    /// </summary>
    [ComVisible(true)]
    [ProgId(SwtaskpaneProgid)]
    public partial class SWTaskpaneHost : UserControl
    {
        #region Private Members
        /// <summary>
        /// The swtaskpan e_ progid.
        /// </summary>
        public const string SwtaskpaneProgid = "AngelSix.SWTaskPane_MenusAndToolbars";

        /// <summary>
        /// The my sw application.
        /// </summary>
        private SldWorks mySWApplication;
       
        /// <summary>
        /// The my graph.
        /// </summary>
        //private Graph myGraph;

        /// <summary>
        /// The path directory file comparison.
        /// Al momento inserire il percorso della cartella a mano,
        /// poi si sistema facendo scegliere la cartella all'utente!!!
        /// </summary>
        private string pathOfDirectoryOfFileComparison = @"C:\Users\Katia\Desktop\SWConfronti";

        private int searchType = 0;
        #endregion

        #region .ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SWTaskpaneHost"/> class.
        /// </summary>
        public SWTaskpaneHost()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the m cookie.
        /// </summary>
        public int MCookie { get; set; }

        #endregion

        #region Init

        /// <summary>
        /// The connect.
        /// </summary>
        /// <param name="swApp">
        /// The sw app.
        /// </param>
        /// <param name="cookie">
        /// The cookie.
        /// </param>
       
        public void Connect(SldWorks swApp, int cookie)
        {
            if (swApp == null)
            {
                throw new ArgumentNullException("swApp");
            }
            // Store the SolidWorks instance passed in
            this.mySWApplication = swApp;
            this.MCookie = cookie;
        }
        #endregion

        #region Eventi Bottoni

        /// <summary>
        /// The b ottieni dati selezione click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BOttieniDatiSelezioneClick(object sender, EventArgs e)
        {

            ModelDoc2 myModel = (ModelDoc2)this.mySWApplication.ActiveDoc;
            PartDoc myPartDoc = (PartDoc)myModel;
            Body2 MyBody;
            Array myBodyVar = myPartDoc.GetBodies2((int)swBodyType_e.swAllBodies, true);
            List<Face2> mySelectedFace = new List<Face2>();
            int myBodyCount = 0;
            for (myBodyCount = myBodyVar.GetLowerBound(0); myBodyCount <= myBodyVar.GetUpperBound(0); myBodyCount++)
            {
                MyBody = (Body2)myBodyVar.GetValue(myBodyCount);
                Body2 myProcBody = MyBody.GetProcessedBodyWithSelFace();
                Array myFaceOfBody = myProcBody.GetSelectedFaces();
                foreach (Face2 myFace in myFaceOfBody)
                {
                    mySelectedFace.Add(myFace);
                    var surf = (Surface) myFace.GetSurface();
                    ListaRisultati.Items.Add(surf.Identity().ToString());
                }
            }
        }

        /// <summary>
        /// The b similarity click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BSimilarityClick(object sender, EventArgs e)
        {
            searchType = 0;
            ListaRisultati.Items.Clear();

            ModelDoc2 myModel = (ModelDoc2)this.mySWApplication.ActiveDoc;
            PartDoc myPartDoc = (PartDoc)myModel;
            Body2 MyBody;
            Array myBodyVar = myPartDoc.GetBodies2((int)swBodyType_e.swAllBodies, true);
            List<Face2> mySelectedFace = new List<Face2>();
            int myBodyCount = 0;
            for (myBodyCount = myBodyVar.GetLowerBound(0); myBodyCount <= myBodyVar.GetUpperBound(0); myBodyCount++)
            {
                MyBody = (Body2)myBodyVar.GetValue(myBodyCount);
                Body2 myProcBody = MyBody.GetProcessedBodyWithSelFace();
                Array myFaceOfBody = myProcBody.GetSelectedFaces();
                foreach (Face2 myFace in myFaceOfBody)
                {
                    mySelectedFace.Add(myFace);
                    var surf = (Surface)myFace.GetSurface();
                    ListaRisultati.Items.Add(surf.Identity().ToString());
                }
            }

            var myGraph = (Graph) MySavaData(mySelectedFace);

            string[] pathOfFileComparison = Directory.GetFiles(pathOfDirectoryOfFileComparison);
            foreach (string fileComparisonName in pathOfFileComparison)
            {
                List<Face2> myFaceComparison = MyGetFaceOfFileComparison(fileComparisonName);
                var myComparisonGraph = (Graph)this.MySavaData(myFaceComparison);

                foreach (Node nodo in myGraph.Nodes)
                {
                    if (nodo is CylinderNode)
                    {
                        ListaRisultati.Items.Add("CILINDRO");
                    }
                    else if (nodo is PlaneNode)
                    {
                        ListaRisultati.Items.Add("PIANO");
                    }
                    else
                    {
                        ListaRisultati.Items.Add("ALTRO");
                    }
                }
                foreach (Node nodo in myComparisonGraph.Nodes)
                {
                    if (nodo is CylinderNode)
                    {
                        ListaRisultati.Items.Add("cilindro");
                    }
                    else if (nodo is PlaneNode)
                    {
                        ListaRisultati.Items.Add("piano");
                    }
                    else
                    {
                        ListaRisultati.Items.Add("altro");
                    }
                }

                if (myGraph.MyComparisonToTwoGraphs(searchType, myComparisonGraph))
                {
                    ListaRisultati.Items.Add(fileComparisonName);
                }
                /*foreach (Face2 myFace in myFaceComparison)
                { 
                    
                }
                */
            }
        }

        private void bProporzioni_Click(object sender, EventArgs e)
        {
            searchType = 1;
            ListaRisultati.Items.Clear();
            string[] pathOfFileComparison = Directory.GetFiles(pathOfDirectoryOfFileComparison);
            foreach (string fileComparisonName in pathOfFileComparison)
            {
                List<Face2> myFaceComparison = MyGetFaceOfFileComparison(fileComparisonName);
                var myComparisonGraph = (Graph)this.MySavaData(myFaceComparison);
                /*if (myGraph.MyComparisonToTwoGraphs(searchType, myComparisonGraph))
                {
                    ListaRisultati.Items.Add(fileComparisonName);
                }*/
            }
        }


        private void BIncastro_Click(object sender, EventArgs e)
        {
            searchType = 2;
            mySWApplication.SendMsgToUser("Ricerca ancora da implementare");

        }

        private void bEsatto_Click(object sender, EventArgs e)
        {
            searchType = 3;
            mySWApplication.SendMsgToUser("Ricerca ancora da implementare");

        }
        private void ListaRisultatiSelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// The cartella modelli help request.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private void CartellaModelliHelpRequest(object sender, EventArgs e)
        {
            if (CartellaModelli.ShowDialog() == DialogResult.OK)
            {
                pathOfDirectoryOfFileComparison = CartellaModelli.SelectedPath;
            }
        }
        /// <summary>
        /// The sw taskpane host load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SWTaskpaneHostLoad(object sender, EventArgs e)
        {
        }
        #endregion

        #region Selezione delle facce degli oggetti da confrontare

        /// <summary>
        /// The my get face of file comparison.
        /// </summary>
        /// <param name="fileComparisonName">
        /// The file comparison name.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<Face2> MyGetFaceOfFileComparison(string fileComparisonName)
        {
               // ImportIgesData myImportData = null;
               // int error = 0;
                //var myModelComparison = (ModelDoc2)mySWApplication.LoadFile4(fileComparisonName, "R", myImportData, error);

                int error = 0;
                int warnings = 0;
                var myModelComparison = (ModelDoc2)mySWApplication.OpenDoc6(fileComparisonName, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref error, ref warnings);
                myModelComparison = (ModelDoc2)mySWApplication.ActivateDoc(fileComparisonName);
                myModelComparison = mySWApplication.ActiveDoc;
                var myPartDocComparison = (PartDoc)myModelComparison;
                Body2 myBodyComparison;
                Array myBodyVar = myPartDocComparison.GetBodies2((int)swBodyType_e.swAllBodies, true);
                List<Face2> myFacesComparison = new List<Face2>();
                int myBodyCount = 0;
                    for (myBodyCount = myBodyVar.GetLowerBound(0); myBodyCount <= myBodyVar.GetUpperBound(0); myBodyCount++)
                    {
                        myBodyComparison = (Body2)myBodyVar.GetValue(myBodyCount);
                        Body2 myProcBody = myBodyComparison.GetProcessedBody();
                        Array myFaceOfBody = myProcBody.GetFaces();

                        foreach (Face2 myFace in myFaceOfBody)
                        {
                            myFacesComparison.Add(myFace);
                        }
                    }
                    return myFacesComparison;
        }

        #endregion

        #region Salvataggio Dati

        /// <summary>
        /// The my sava data.
        /// </summary>
        /// <param name="mySelectedFace">
        /// The my selected face.
        /// </param>
        /// <returns>
        /// The <see cref="Graph"/>.
        /// </returns>
        private Graph MySavaData(List<Face2> mySelectedFace)
        {
                var nodeList = new List<Node>();
                foreach (Face2 face in mySelectedFace)
                {
                    nodeList.Add(MySetNode(face));
                    var adiacenti = MySetNode(face).Adjacents;
                }
                var myGraph = new Graph(nodeList);
                return myGraph;
        }

        /// <summary>
        /// The my set node.
        /// </summary>
        /// <param name="myFace">
        /// The my face.
        /// </param>
        /// <returns>
        /// The <see cref="Node"/>.
        /// </returns>
     
        private Node MySetNode(Face2 myFace)
        {
            if (myFace == null)
            {
                throw new ArgumentNullException("myFace");
            }

            var myAdiacenceNodes = new List<AdiacenceNode>();

            var myEdgeOfFace = (Array)myFace.GetEdges();

            foreach (Edge edge in myEdgeOfFace)
            {
                Array myAdiacenceFaces = edge.GetTwoAdjacentFaces2();
                Face2 myAdiacenceFace = null;
                var firstFace = (Face2)myAdiacenceFaces.GetValue(0);
                var secondFace = (Face2)myAdiacenceFaces.GetValue(1);

                // Se la faccia non è quella che ho già la salvo come faccia adiacente, altrimenti salvo l'altra.
                // Se non c'è un'altra faccia adiacente allora ho un valore NULL e non salvo niente.
                if (firstFace != null && secondFace != null)
                {
                    myAdiacenceFace = firstFace != myFace ? firstFace : secondFace;
                }
                var angle = MySetAngle(edge, myFace, myAdiacenceFace);
                var lenght = MySetLenght(edge);
                var myAdiacenceOfAdiacenceNode = new List<AdiacenceNode>();
                var myAdiacenceNode = new AdiacenceNode(angle, lenght, IdentifyNode(myAdiacenceFace, myAdiacenceOfAdiacenceNode));
                myAdiacenceNodes.Add(myAdiacenceNode);
            }

            var identifyNode = IdentifyNode(myFace, myAdiacenceNodes);
            return identifyNode;
        }

        /// <summary>
        /// The identify node.
        /// </summary>
        /// <param name="myFace">
        /// The my face.
        /// </param>
        /// <param name="myAdjacence">
        /// The my Adjacence.
        /// </param>
        /// <returns>
        /// The <see cref="Node"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>

        private Node IdentifyNode(Face2 myFace, List<AdiacenceNode> myAdjacence)
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
                    var nodoPiano = new PlaneNode(normale, rootPoint, numLoop, numEdges, boundParameters, faceSense, myAdjacence);
                    return nodoPiano;
                case 4002:
                    Array myValuesCylinder = mySurface.CylinderParams;
                    Array origineCilindro = new double[3];
                    Array asseCilindro = new double[3];
                    Array.Copy(myValuesCylinder, 0, origineCilindro, 0, 3);
                    Array.Copy(myValuesCylinder, 3, asseCilindro, 0, 3);
                    var raggioCilindro = (double)myValuesCylinder.GetValue(5);
                    var nodoCilindrico = new CylinderNode(origineCilindro, asseCilindro, raggioCilindro, numEdges, numLoop, boundParameters, faceSense, myAdjacence);
                    return nodoCilindrico;
                case 4003:
                    Array myValuesCone = mySurface.ConeParams;
                    Array origineCono = new double[3];
                    Array asseCono = new double[3];
                    Array.Copy(myValuesCone, 0, origineCono, 0, 3);
                    Array.Copy(myValuesCone, 3, asseCono, 0, 3);
                    var raggioCono = (double)myValuesCone.GetValue(5);
                    var nodoConico = new ConeNode(origineCono, asseCono, raggioCono, numEdges, numLoop, boundParameters, faceSense, myAdjacence);
                    return nodoConico;
                case 4004:
                    Array myValueSphere = mySurface.SphereParams;
                    Array centroSfera = new double[3];
                    Array.Copy(myValueSphere, 0, centroSfera, 0, 3);
                    var raggioSfera = (double)myValueSphere.GetValue(5);
                    var nodoSperico = new SphereNode(centroSfera, raggioSfera, numLoop, numEdges, boundParameters, faceSense, myAdjacence);
                    return nodoSperico;
                case 4005:
                    Array myValueTorus = mySurface.TorusParams;
                    Array centroToro = new double[3];
                    Array asseToro = new double[3];
                    Array.Copy(myValueTorus, 0, centroToro, 0, 3);
                    Array.Copy(myValueTorus, 3, asseToro, 0, 3);
                    var raggioMassimo = (double)myValueTorus.GetValue(6);
                    var raggioMinimo = (double)myValueTorus.GetValue(7);
                    var nodoToroidale = new TorusNode(asseToro, raggioMinimo, raggioMassimo, numLoop, numEdges, boundParameters, faceSense, myAdjacence);
                    return nodoToroidale;
                default:
                    var nodoInvalido = new InvalidNode(numLoop, numEdges, boundParameters, faceSense, myAdjacence);
                    return nodoInvalido;
            }
        }
 #endregion



        #region Funzioni di Calcolo

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
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private double MySetAngle(Edge angleEdge, Face2 firstFace, Face2 secondFace)
        {
            Surface firstSurface = firstFace.GetSurface();
            Surface secondSurface = secondFace.GetSurface();
            Vertex startVertex = angleEdge.GetStartVertex();
            Vertex endVertex = angleEdge.GetEndVertex();
            var startPoint = (Array)startVertex.GetPoint();
            var endPoint = (Array)endVertex.GetPoint();
            var firstNormal = new double[3];
            var secondNormal = new double[3];
            double[] firstEvalutation;
            double[] secondEvalutation;

            if (angleEdge.EdgeInFaceSense(firstFace))
            {
                var x = (double)endPoint.GetValue(0);
                var y = (double)endPoint.GetValue(1);
                var z = (double)endPoint.GetValue(2);
                firstEvalutation = firstSurface.EvaluateAtPoint(x, y, z);
                secondEvalutation = secondSurface.EvaluateAtPoint(x, y, z);
            }
            else
            {
                var x = (double)startPoint.GetValue(0);
                var y = (double)startPoint.GetValue(1);
                var z = (double)startPoint.GetValue(2);
                firstEvalutation = firstSurface.EvaluateAtPoint(x, y, z);
                secondEvalutation = secondSurface.EvaluateAtPoint(x, y, z);
            }

            if (firstFace.FaceInSurfaceSense())
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
                
                if (!angleEdge.EdgeInFaceSense(firstFace))
                {
                    var myEdgeDirection = MyNormalization(MyVectorDifferent(endPoint, startPoint));    
                    angle = MyInnerProduct(firstNormal, MyVectorProduct(secondNormal, myEdgeDirection));
                }
                else
                {
                    var myEdgeDirection = MyNormalization(MyVectorDifferent(startPoint, endPoint));
                    angle = MyInnerProduct(firstNormal, MyVectorProduct(secondNormal, myEdgeDirection));
                }
               
                return angle;
        }

        /// <summary>
        /// The my set lenght.
        /// </summary>
        /// <param name="edge">
        /// The edge.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private static double MySetLenght(Edge edge) 
        {
            Vertex startVertex = edge.GetStartVertex();
            Vertex endVertex = edge.GetEndVertex();
            var startPoint = (Array)startVertex.GetPoint();
            var endPoint = (Array)endVertex.GetPoint();

            var x1 = (double)startPoint.GetValue(0);
            var x2 = (double)startPoint.GetValue(1);
            var x3 = (double)startPoint.GetValue(2);

            var y1 = (double)endPoint.GetValue(0);
            var y2 = (double)endPoint.GetValue(1);
            var y3 = (double)endPoint.GetValue(2);

            var lenght = Math.Sqrt(Math.Pow(x1 - y1, 2) + Math.Pow(x2 - y2, 2) + Math.Pow(x3 - y3, 2));
            return lenght;
        }

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
       
        private double MyInnerProduct(Array first, Array second)
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
       
        private Array MyVectorProduct(Array first, Array second)
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
      
        private static Array MyVectorDifferent(Array first, Array second)
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
        private Array MyNormalization(Array first)
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

        #endregion

        private void Cancella_Click(object sender, EventArgs e)
        {
            ListaRisultati.Items.Clear();
        }

    }
}