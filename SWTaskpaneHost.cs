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
    using Microsoft.Office;
    using Microsoft.Office.Interop.Excel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using SolidWorks.Interop.sldworks;
    using SolidWorks.Interop.swconst;
    using SolidWorksAddinUtility;
    using SWIntegration;
    using SWIntegration.Data_Structure;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Runtime.Serialization;

    using System.Xml;
    using System.Xml.Linq;
    


    /// <summary>
    /// The sw taskpane host.
    /// </summary>
    [ComVisible(true)]
    [ProgId(SwtaskpaneProgid)]
   
    public partial class SWTaskpaneHost : UserControl
    {
        #region Private and Public Members
        /// <summary>
        /// The swtaskpan e_ progid.
        /// </summary>
        public const string SwtaskpaneProgid = "AngelSix.SWTaskPane_MenusAndToolbars";

        /// <summary>
        /// The my sw application.
        /// </summary>
        private SldWorks mySWApplication;

        /// <summary>
        /// The my original model.
        /// </summary>
        private ModelDoc2 myOriginalModel;
                  

        /// <summary>
        /// The path directory file comparison.
        /// Al momento inserire il percorso della cartella a mano,
        /// poi si sistema facendo scegliere la cartella all'utente!!!
        /// </summary>
       //  private string pathOfDirectoryOfFileComparison = @"C:\Users\SoullessPG\Desktop\SWConfronti - Copia";

        private string pathOfDirectoryOfFileComparison = @"C:\Users\Katia\Desktop\SWConfronti - Copia";

        /// <summary>
        /// The my original graph.
        /// Grafo dell'oggetto di partenza da confrontare con gli oggetti in memoria.
        /// Lo si crea globale così non si ricrea il grafo ogni volta che si modifica la ricerca.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        private Graph myOriginalGraph = null;

        /// <summary>
        /// The search type.
        /// Variabile che indica il tipo di ricerca richiesta dall'utente.
        /// </summary>
        private int searchType = 0;

        // Variabile booleana per indicare se la ricerca eseguita è sul modello completo o parziale
        private bool fullQuery;

        private bool serialization = false;
        #endregion

        #region .ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SWTaskpaneHost"/> class.
        /// </summary>
        public SWTaskpaneHost()
        {
            this.InitializeComponent();
           // Utility.DeleteDebugFiles();
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
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public void BOttieniDatiSelezioneClick(object sender, EventArgs e)
        {
            var swSafeEntityList = new List<Entity>();
            this.fullQuery = false;
            myOriginalModel = (ModelDoc2)this.mySWApplication.ActiveDoc;
            
            SelectionMgr mySelManager = (SelectionMgr)myOriginalModel.SelectionManager;
            var selectionCount = mySelManager.GetSelectedObjectCount2(-1);
                     
            for(int i=0; i<selectionCount; i++)
            {
                var myFace = (Face2)mySelManager.GetSelectedObject6(i+1,-1);
                var swEntity = ((Entity)myFace).GetSafeEntity();
                var swSafeEntity = (Entity)swEntity.GetSafeEntity();
                swSafeEntityList.Add(swSafeEntity);
            }

            /* Dopo aver caricato correttamente le facce viene creato il grafo di attributi*/
            myOriginalGraph = Utility.MySaveData(swSafeEntityList, myOriginalModel, mySWApplication);
            
            //string path = "C:\\Users\\Katia\\Desktop\\temp";
            /*var ser = new XmlSerializer(typeof(Graph), new[] { typeof(List<Node>), typeof(Node), typeof(PlaneNode), typeof(CylinderNode), typeof(ConeNode), typeof(SphereNode), typeof(TorusNode), typeof(InvalidNode), typeof(RealLink), typeof(VirtualLink) });
            TextWriter writer = new StreamWriter(path);

            ser.Serialize(writer, myOriginalGraph);
            writer.Close();
            */

            
            //Utility.WriteToBinaryFile(path, myOriginalGraph, false);

            if (myOriginalGraph.Nodes.Count >= 3)
            {
                mySWApplication.SendMsgToUser(
                    Path.GetFileName(myOriginalModel.GetPathName()) + "caricato correttamente.");
            }
            else
            {
                mySWApplication.SendMsgToUser("Per effettuarte le ricerche selezionare almeno 3 facce.");
            }

            /* Per vari test si stampano alcuni valori del grafo creato*/
            /* SARA' DA ELIMINARE UNA VOLTA TERMINATO */
            var numeroArchi = Utility.MyNumOfLinks(myOriginalGraph, mySWApplication);
            Resu.Items.Add("Numero di nodi " + myOriginalGraph.Nodes.Count().ToString() + " numero archi " + numeroArchi.ToString());
            foreach (Node node in myOriginalGraph.Nodes)
            {
                Resu.Items.Add("Tipo nodo " + node.GetType().ToString());

                if (node.GetType() == typeof(CylinderNode))
                {
                    var cylinderNode = (CylinderNode)node;
                    Resu.Items.Add("Pieno " + cylinderNode.Complete.ToString());
                    Resu.Items.Add("FaceSence " + cylinderNode.FaceSense.ToString());
                }
             
                /*
                ListaRisultati.Items.Add("Tipo nodo " + node.GetType().ToString() + "FaceSense" + node.FaceSense.ToString());
                ListaRisultati.Items.Add("Nodo con " + node.RealLinks.Count.ToString() + " archi reali");
                foreach (RealLink link in node.RealLinks)
                {
                    ListaRisultati.Items.Add("connessione " + link.Angle.ToString());
                }
                 */
                Resu.Items.Add("Nodo con " + node.VirtualLinks.Count.ToString() + " archi virtuali");
                foreach (VirtualLink link in node.VirtualLinks)
                {
                    if (node.GetType() == typeof(CylinderNode))
                    {
                        var cylinderNode = (CylinderNode)node;
                        Resu.Items.Add("Pieno " + cylinderNode.Complete.ToString());
                        if (link.DestinationNode.GetType() == typeof(CylinderNode))
                        {
                            var otherCylinderNode = (CylinderNode)link.DestinationNode;
                            /*
                            ListaRisultati.Items.Add("Primo asse " + cylinderNode.Axis.GetValue(0) + " " + cylinderNode.Axis.GetValue(1) + " " + cylinderNode.Axis.GetValue(2));
                            ListaRisultati.Items.Add("Primo punto " + cylinderNode.Origin.GetValue(0) + " " + cylinderNode.Origin.GetValue(1) + " " + cylinderNode.Origin.GetValue(2));
                            ListaRisultati.Items.Add("Secondo asse " + otherCylinderNode.Axis.GetValue(0) + " " + otherCylinderNode.Axis.GetValue(1) + " " + otherCylinderNode.Axis.GetValue(2));
                            ListaRisultati.Items.Add("Secondo punto " + otherCylinderNode.Origin.GetValue(0) + " " + otherCylinderNode.Origin.GetValue(1) + " " + otherCylinderNode.Origin.GetValue(2));
                            */
                            Resu.Items.Add("Coassiale " + cylinderNode.IsSameCylinder(otherCylinderNode).ToString());
                        }
                    }
                    Resu.Items.Add("connessione " + link.Connection.ToString());
                //    ListaRisultati.Items.Add("distanza " + link.Lenght.ToString());
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
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private void BSimilarityClick(object sender, EventArgs e)
        {
            // searchType = 0;
            Resu.Items.Clear();

            this.SimilarComparisonMethod();
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private void SimilarComparisonMethod()
        {

            //List<string> pathOfFileComparison = new List<string>();
            IEnumerable<string> pathOfFileComparison;
            pathOfFileComparison = Directory.EnumerateFiles(this.pathOfDirectoryOfFileComparison);
            //mySWApplication.SendMsgToUser("Ci sono " + pathOfFileComparison.Count().ToString() + " file da comparare");

            Microsoft.Office.Interop.Excel.Application excelapp = new Microsoft.Office.Interop.Excel.Application();
            excelapp.Visible = true;

            _Workbook workbook = (_Workbook)(excelapp.Workbooks.Add(Type.Missing));
            _Worksheet worksheet = (_Worksheet)workbook.ActiveSheet;

            var indexCell = 2;
            foreach (var fileComparisonName in pathOfFileComparison)
            {
                var fileName = Path.GetFileName(fileComparisonName);
                if (!fileName.StartsWith("~"))
                {
                 
                    int error = 0;
                    int warnings = 0;
                    var myModelComparison = (ModelDoc2)mySWApplication.OpenDoc6(fileComparisonName, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_LoadModel, "", ref error, ref warnings);
                    myModelComparison = (ModelDoc2)this.mySWApplication.ActivateDoc2(fileComparisonName, true, (int)swActivateDocError_e.swGenericActivateError);
                    myModelComparison = (ModelDoc2)this.mySWApplication.ActiveDoc;

                    // Estraggo le facce dell'oggetto
                    var myEntityComparison = this.MyGetFaceOfFileComparison(fileComparisonName);

                    // Creo o carico il grafo con attributi dell'oggetto
                    Graph myComparisonGraph = (Graph)Utility.MySaveData(myEntityComparison, myModelComparison, this.mySWApplication);

                    string path = "C:\\Users\\Katia\\Desktop\\PIASTRABUCATA.bin";
                    
                    /*
                   // if (!serialization)
                    {
                        myComparisonGraph = (Graph)Utility.MySaveData(myEntityComparison, myModelComparison, this.mySWApplication);
                        Utility.WriteToBinaryFile(path, myComparisonGraph);
                        //serialization = true;
                        mySWApplication.SendMsgToUser("Ho creato il grafo");
                    }
                     
                //    else
                    {
                        myComparisonGraph = (Graph)Utility.ReadToBinaryFile(path);
                        mySWApplication.SendMsgToUser("Ho caricato il grafo");
                    }
                     
                    */
                    // Creo il grafo associato tra l'oggetto corrente e quello selezionato dall'utente
                    var watch = Stopwatch.StartNew();
                    var associativeGraph = Utility.MyCreateAssociativeGraph(this.myOriginalGraph, myComparisonGraph, this.mySWApplication);
                    watch.Stop();
                    var elapsedMs = watch.Elapsed.Milliseconds;

                    //ListaRisultati.Items.Add("Nodi grafo associato " + associativeGraph.listNode.Count.ToString() + " tempo " + elapsedMs.ToString());
                    //Utility.PrintToFile(fileName, "graph.txt");
                  /*  
                      // Per debag: stampo il grafo associato
                    Utility.PrintToFile(fileName, "AssociativeGraphStarted.txt");
                      foreach (AssociativeNode node in associativeGraph.listNode)
                      {
                         // this.ListaRisultati.Items.Add("archi associati " + node.AssociativeLinks.Count.ToString());

                          var aString = string.Format(
                                     "id grafo: {0} --- {1} \n numero archi associati --> {2}",
                                     node.NodeFirstGraph.IdNode,
                                     node.NodeSecondGraph.IdNode,
                                     node.AssociativeLinks.Count);
                          Utility.PrintToFile(aString, "AssociativeGraphStarted.txt");
                          foreach (var nodoAssociato in node.AssociativeLinks)
                          {
                              var stampa = string.Format("{0} --- {1}",
                                  nodoAssociato.NodeFirstGraph.IdNode, nodoAssociato.NodeSecondGraph.IdNode);
                            //  Utility.PrintToFile(stampa, "graph.txt");
                          }
                      }
                    */ 

                    /*
                       ListaRisultati.Items.Add("Numero nodi associati" + associativeGraph.listNode.Count.ToString());
                       foreach (AssociativeNode associativeNode in associativeGraph.listNode)
                       {
                           ListaRisultati.Items.Add("numero link associati" + associativeNode.AssociativeLinks.Count.ToString());
                       }
                      */

                    // Imposto i valori del lower e upper bound in base al tipo di grafo.
                    //var lowerBound = Math.Min(this.myOriginalGraph.Nodes.Count, myComparisonGraph.Nodes.Count);
                    int lowerBound;
                    if (this.fullQuery)
                    {
                        lowerBound = 3;// Math.Max(Math.Min(this.myOriginalGraph.Nodes.Count, myComparisonGraph.Nodes.Count) / 2, 3);
                    }
                    else
                    {
                        lowerBound = Math.Max(this.myOriginalGraph.Nodes.Count, 3);
                    }

                  //  mySWApplication.SendMsgToUser("Lowerbound impostato a " + lowerBound.ToString());
                    var upperBound = this.myOriginalGraph.Nodes.Count;
                    var associativeNodeRetrieval = new List<int>();
                    /*
                    Utility.PrintToFile(fileName, "SimulateAnnealing.txt");
                    Utility.PrintToFile(fileName, "clique.txt");
                    Utility.PrintToFile(fileName, "StampaArchi.txt");
                    Utility.PrintToFile(fileName, "colorFace.txt");
                    */
                    
                    var myMaxClique = Utility.MyCliqueFinder(associativeGraph, lowerBound, upperBound, this.fullQuery, this.mySWApplication);
                    
                    var newOriginalNodes = new List<Node>();
                    var newComparisonNodes = new List<Node>();
                    newOriginalNodes.AddRange(myOriginalGraph.Nodes);
                    newComparisonNodes.AddRange(myComparisonGraph.Nodes);
                    double misura = 0.0;
                    var nodeSubGraphRetrieved = new List<Node>();
                    var nodeSubGraphMatched = new List<Node>();
                    var idNodesRetrieved = new List<int>();
                    int minF = 0;
                    var associativeNodeRetrived = new List<AssociativeNode>();

                    // Copio il grafo associativo iniziale
                    var associativeGraphStarted = new AssociativeGraph(associativeGraph.listNode);
                    var stopCriterion = 0;
                    while (myMaxClique != null && myMaxClique.listNode.Count >= lowerBound /*&& newComparisonGraph.Nodes.Count >= lowerBound && nodeSubGraphRetrieved.Count <= newOriginalNodes.Count()*/)
                    //if(myMaxClique != null)
                    {
                        stopCriterion++;
                        minF += Utility.MyMinimunFunction(myMaxClique);
                        //mySWApplication.SendMsgToUser("trovato ciclo da " + myMaxClique.listNode.Count.ToString() + " con funzione " + minF.ToString());
                        //Utility.MyColorFace(myEntityComparison, myMaxClique, this.mySWApplication, myModelComparison);
                        // Elimino tutti i nodi visitati con il ciclo

                        foreach (AssociativeNode associativeNodeInClique in myMaxClique.listNode)
                        {

                            int firstIndex = 0;
                            var firstNode = myOriginalGraph.Nodes[firstIndex];
                            int secondIndex = 0;
                            var secondNode = myComparisonGraph.Nodes[secondIndex];
                            
                            while (associativeNodeInClique.NodeFirstGraph.IdNode != firstNode.IdNode)
                            {
                                firstIndex++;
                                firstNode = myOriginalGraph.Nodes[firstIndex];
                            }
                            newOriginalNodes.Remove(firstNode);
                            

                            while (associativeNodeInClique.NodeSecondGraph.IdNode != secondNode.IdNode)
                            {
                                secondIndex++;
                                secondNode = myComparisonGraph.Nodes[secondIndex];
                            }
                            newComparisonNodes.Remove(secondNode);
                            
                            // Salvo il sottografo ritrovato dal modello nel repository
                            if (!nodeSubGraphMatched.Contains(firstNode) && !nodeSubGraphRetrieved.Contains(secondNode))
                            {
                                nodeSubGraphMatched.Add(firstNode);
                                nodeSubGraphRetrieved.Add(secondNode);
                                var associativeNodeInAssGStarted = associativeGraphStarted.listNode.Find(x => x.Equals(associativeNodeInClique));
                                associativeNodeRetrived.Add(associativeNodeInAssGStarted);
                            }
                        }

                        // Elimino nodi restituiti dalla clique
                        var newOriginalGraph = new Graph(newOriginalNodes);
                        var newComparisonGraph = new Graph(newComparisonNodes);

                        //mySWApplication.SendMsgToUser("grafo input " + newOriginalGraph.Nodes.Count.ToString() + /*"\n grafo output " + newComparisonGraph.Nodes.Count.ToString()*/ " grafo originale " + myOriginalGraph.Nodes.Count.ToString()); 
                        if (newOriginalGraph != null && newComparisonGraph != null)
                        {
                       
                            associativeGraph = Utility.MyCreateAssociativeGraph(newOriginalGraph, newComparisonGraph, mySWApplication);
                            // Ricalcolo il massimo ciclo
                            if (associativeGraph != null /*&& newOriginalGraph.Nodes.Count >= lowerBound && newComparisonGraph.Nodes.Count >= lowerBound*/)
                            {

                                myMaxClique = Utility.MyCliqueFinder(associativeGraph, lowerBound, upperBound, this.fullQuery, this.mySWApplication);
                            }
                            else { myMaxClique = null; }
                        }
                        else { myMaxClique = null;}
                    }


                    AssociativeGraph associativeGraphRetrived = new AssociativeGraph(associativeNodeRetrived);
                    /*
                     // PER DEBUG
                    Utility.PrintToFile(fileName, "AssociativeGraphRetrived.txt");
                    foreach (AssociativeNode node in associativeGraphRetrived.listNode)
                    {
                        // this.ListaRisultati.Items.Add("archi associati " + node.AssociativeLinks.Count.ToString());

                        var aString = string.Format(
                                   "id grafo: {0} --- {1} \n numero archi associati --> {2}",
                                   node.NodeFirstGraph.IdNode,
                                   node.NodeSecondGraph.IdNode,
                                   node.AssociativeLinks.Count);
                        Utility.PrintToFile(aString, "AssociativeGraphRetrived.txt");
                        
                        foreach (var nodoAssociato in node.AssociativeLinks)
                        {
                            var stampa = string.Format("{0} --- {1}",
                                nodoAssociato.NodeFirstGraph.IdNode, nodoAssociato.NodeSecondGraph.IdNode);
                            //  Utility.PrintToFile(stampa, "graph.txt");
                        }
                    }
                */
                    int totalFunction = 0;
                    Graph subGraphRetrieved = new Graph(nodeSubGraphRetrieved);
                    double numLinkOriginal = 0;
                    double numLinkComparison = 0;
                    if (associativeGraphRetrived.listNode.Count == 0)
                    {
                        misura = 0;
                    }
                    else
                    {
                        Utility.MyColorFace(myEntityComparison, associativeGraphRetrived, this.mySWApplication, myModelComparison);
                        totalFunction = Utility.MyMinimunFunction(associativeGraphRetrived);

                        numLinkOriginal = Utility.MyNumOfLinks(myOriginalGraph, mySWApplication);
                        numLinkComparison = Utility.MyNumOfLinks(subGraphRetrieved, mySWApplication);
                        
                        var numTotalLinkComparison = Utility.MyNumOfLinks(myComparisonGraph, mySWApplication);
                        //  mySWApplication.SendMsgToUser("archi com " + numLinkComparison.ToString() + " nodi com " + subGraphRetrieved.Nodes.Count.ToString()
                        //   + " misura " + minF.ToString() + "original " + myOriginalGraph.Nodes.Count.ToString() + " link " + numLinkOriginal.ToString());


                        if (this.fullQuery == true)
                        {
                         //   misura = (double)(((double)(1 / (double)(1 + totalFunction)) * (double)(1 - (Math.Abs(myOriginalGraph.Nodes.Count - subGraphRetrieved.Nodes.Count) / (double)myOriginalGraph.Nodes.Count))));
                            misura = (2 * (numLinkComparison + subGraphRetrieved.Nodes.Count /*- totalFunction*/))
                            / (numLinkOriginal + myOriginalGraph.Nodes.Count + numTotalLinkComparison + myComparisonGraph.Nodes.Count); //misura totale                    
                        }
                        else
                        {
                            misura = (double)(((double)(1 / (double)(1 + totalFunction)) * (double)(1 - (Math.Abs(myOriginalGraph.Nodes.Count - subGraphRetrieved.Nodes.Count) / (double)myOriginalGraph.Nodes.Count))));
                        }

                    }
                    Resu.Items.Add(fileName + " Measure " + misura.ToString() + " con funzione globale " + totalFunction.ToString() + " nodi grafo associato " + associativeNodeRetrived.Count.ToString());
                    
                    
                    worksheet.Cells[1, 1] = "Nome";
                    worksheet.Cells[1, 2] = "Misura";
                    worksheet.Cells[1, 3] = "Funzione totale";
                    worksheet.Cells[1, 4] = "Funzione somma";
                    worksheet.Cells[1, 5] = "Nodi ritrovati";
                    worksheet.Cells[1, 6] = "Nodi originali matchati";
                    //worksheet.Cells[1, 7] = "Nodi associati";

                    worksheet.Cells[indexCell, 1] = fileName;
                    worksheet.Cells[indexCell, 2] = misura.ToString();
                    worksheet.Cells[indexCell, 3] = totalFunction.ToString();
                    worksheet.Cells[indexCell, 4] = minF.ToString();
                    worksheet.Cells[indexCell, 5] = subGraphRetrieved.Nodes.Count.ToString();
                    worksheet.Cells[indexCell, 6] = nodeSubGraphMatched.Count.ToString();
                    //worksheet.Cells[indexCell, 7] = associativeGraphRetrived.listNode.Count.ToString(); 
                    excelapp.UserControl = true;
                    indexCell++;

                    mySWApplication.CloseDoc(fileComparisonName);
                }
            }    
        }

        /// <summary>
        /// The b proporzioni_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BProporzioniClick(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// The b incastro_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BIncastroClick(object sender, EventArgs e)
        {
            Resu.Items.Clear();
            // La ricerca del complementare funziona come quella di similarità solo modificando
            // il tipo di connessione del grafo in input


            foreach (Node node in myOriginalGraph.Nodes)
            {
                if (node.GetType() == typeof(CylinderNode))
                {
                    var cylinderNode = (CylinderNode)node;
                    cylinderNode.FaceSense = !cylinderNode.FaceSense;
                }
                foreach (RealLink realLink in node.RealLinks)
                {
                    realLink.Angle = -realLink.Angle;
                }
                foreach (VirtualLink virtualLink in node.VirtualLinks)
                {
                    virtualLink.Connection = -virtualLink.Connection;
                }
            }

            this.SimilarComparisonMethod();

            // Una volta eseguita la ricerca, riporto le connessioni originarie del solido caricato.

            foreach (Node node in myOriginalGraph.Nodes)
            {
                if (node.GetType() == typeof(CylinderNode))
                {
                    var cylinderNode = (CylinderNode)node;
                    cylinderNode.FaceSense = !cylinderNode.FaceSense;
                }
                foreach (RealLink realLink in node.RealLinks)
                {
                    realLink.Angle = -realLink.Angle;
                }
                foreach (VirtualLink virtualLink in node.VirtualLinks)
                {
                    virtualLink.Connection = -virtualLink.Connection;
                }
            }
        }

        /// <summary>
        /// The b esatto_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BEsattoClick(object sender, EventArgs e)
        {
            searchType = 3;
            mySWApplication.SendMsgToUser("Ricerca ancora da implementare");

        }

        /// <summary>
        /// The lista risultati selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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
        private List<Entity> MyGetFaceOfFileComparison(string fileComparisonName)
        {
            //var myImportData = new ImportIgesData();
            //int error = 0;
            //var myModelComparison = (ModelDoc2)mySWApplication.LoadFile4(fileComparisonName, "R", myImportData, error);

            int error = 0;
            int warnings = 0;
            //var myModelComparison = (ModelDoc2)mySWApplication.OpenDoc6(fileComparisonName, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref error, ref warnings);
            //myModelComparison = (ModelDoc2)mySWApplication.ActivateDoc2(fileComparisonName, true, (int)swActivateDocError_e.swGenericActivateError);
            var myModelComparison = (ModelDoc2)mySWApplication.ActiveDoc;

            var myPartDoc = (PartDoc)myModelComparison;

            Array myBodyVar = myPartDoc.GetBodies2((int)swBodyType_e.swAllBodies, true);
            var swSafeEntityList = new List<Entity>();
            int myBodyCount = 0;
            for (myBodyCount = myBodyVar.GetLowerBound(0); myBodyCount <= myBodyVar.GetUpperBound(0); myBodyCount++)
            {
                var myBodyComparison = (Body2)myBodyVar.GetValue(myBodyCount);
                /*
                var swBody = (Body2)myBodyComparison.GetProcessedBody();
                Array swFaces = swBody.GetFaces();
                foreach (var face in swFaces)
                {
                    var swEntity = ((Entity)face).GetSafeEntity();
                    var swSafeEntity = (Entity)swEntity.GetSafeEntity();
                    swSafeEntityList.Add(swSafeEntity);
                }
                */
                
                var swFace = (Face2)myBodyComparison.GetFirstFace();
                while (swFace != null)
                {
                    var swEntity = ((Entity)swFace).GetSafeEntity();
                    var swSafeEntity = (Entity)swEntity.GetSafeEntity();
                    swSafeEntityList.Add(swSafeEntity);
                    swFace = swFace.GetNextFace();
                }
                
            }
                 

            return swSafeEntityList;
            
            /*
            var myPartDocComparison = (PartDoc)myModelComparison;
            Body2 myBodyComparison;
            Array myBodyVar = myPartDocComparison.GetBodies2((int)swBodyType_e.swAllBodies, true);
            var myFacesComparison = new List<Face2>();
            int myBodyCount = 0;
            for (myBodyCount = myBodyVar.GetLowerBound(0); myBodyCount <= myBodyVar.GetUpperBound(0); myBodyCount++)
            {
                myBodyComparison = (Body2)myBodyVar.GetValue(myBodyCount);
                Body2 myProcBody = myBodyComparison.GetProcessedBody();
                Array myFaceOfBody = myProcBody.GetFaces();

                foreach (Face2 myFace in myFaceOfBody)
                {
                    if (!myFacesComparison.Contains(myFace) & myFace != null)
                    {
                        myFacesComparison.Add(myFace);
                    }
                }
            }
            
            return myFacesComparison;
             * */
        }

        #endregion

        /// <summary>
        /// The cancella_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CancellaClick(object sender, EventArgs e)
        {
            Resu.Items.Clear();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           // var lowerBound = 3;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
           // lowerBound = (int)this.numericUpDown1.Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.fullQuery = true;
            myOriginalModel = (ModelDoc2)this.mySWApplication.ActiveDoc;

            // Estraggo le facce dell'oggetto
            var swSafeEntityList = new List<Entity>();
            var myOriginalPartDoc = (PartDoc)myOriginalModel;
            Body2 myOriginalBody;
            Array myBodyVar = myOriginalPartDoc.GetBodies2((int)swBodyType_e.swAllBodies, true);
            int myBodyCount = 0;
            var myFaceList = new List<Face2>();
            for (myBodyCount = myBodyVar.GetLowerBound(0); myBodyCount <= myBodyVar.GetUpperBound(0); myBodyCount++)
            {
                myOriginalBody = (Body2)myBodyVar.GetValue(myBodyCount);

                // ATTENZIONE: DEVE ESSERE COERENTE IL GETPROCESSEDBODY.
                // Deve comparire sia per caricare il modello singolo che quelli nei repository, se si aggiunge allora le facce considerate sono solo quelle massimali,
                // ma porta a degli errori nei conti della misura per la selazione parziale.
                // La cosa migliore sarebbe poterlo usare, ma solleva un'eccezione - da risolve!!                               
                
                /*
                // Modo in cui funziona, non cancellare.
                var swBody = (Body2)myOriginalPartDoc.GetProcessedBody();
                Array swFaces = swBody.GetFaces();
                foreach (var face in swFaces)
                {
                    var swEntity = ((Entity)face).GetSafeEntity();
                    var swSafeEntity = (Entity)swEntity.GetSafeEntity();
                    swSafeEntityList.Add(swSafeEntity);
                }
                */
                
                var swFace = (Face2)myOriginalBody.GetFirstFace();
                while(swFace != null)
                {
                    myFaceList.Add(swFace);
                    var swEntity = ((Entity)swFace).GetSafeEntity();
                    var swSafeEntity = (Entity)swEntity.GetSafeEntity();
                    swSafeEntityList.Add(swSafeEntity);
                    swFace = swFace.GetNextFace();
                }
               
            }

            var watch = Stopwatch.StartNew();
            myOriginalGraph = Utility.MySaveData(swSafeEntityList, myOriginalModel, mySWApplication);
            watch.Stop();
            var elapsedMs = watch.Elapsed.Milliseconds;
             

            if (myOriginalGraph.Nodes.Count >= 3)
            {
                mySWApplication.SendMsgToUser(
                    Path.GetFileName(myOriginalModel.GetPathName()) + "caricato correttamente in " + elapsedMs.ToString() + " millsecondi.");
            }
            else
            {
                mySWApplication.SendMsgToUser("Per effettuarte le ricerche selezionare almeno 3 facce.");
            }
            /* Per vari test si stampano alcuni valori del grafo creato*/
            /* SARA' DA ELIMINARE UNA VOLTA TERMINATO */
            Resu.Items.Add("Ho premuto il bottone corretto");
            Resu.Items.Add("Numero di nodi " + myOriginalGraph.Nodes.Count().ToString());
            foreach (Node node in myOriginalGraph.Nodes)
            {
                int idNode = node.IdNode;
                var myFace = myFaceList.Find(x => x.GetFaceId() == idNode);
                var mySurface = (Surface)myFace.GetSurface();
                if (mySurface.Identity() == 4002)
                {
                    var entity = ((Entity)myFace).GetSafeEntity();
                    List<Vertex> listVertexFirstFace = Utility.MyGetVertexFromFace(entity);
                    Resu.Items.Add("Nodo cilindrico con  " + listVertexFirstFace.Count.ToString() + " vertici");
                }

                Resu.Items.Add("Nodo con " + node.RealLinks.Count.ToString() + " archi reali");
                foreach (RealLink link in node.RealLinks)
                {
                    Resu.Items.Add("connessione " + link.Angle.ToString());
                }
                Resu.Items.Add("Nodo con " + node.VirtualLinks.Count.ToString() + " archi virtuali");
                foreach (VirtualLink link in node.VirtualLinks)
                {
                    Resu.Items.Add("connessione " + link.Connection.ToString());
                }

            }

        }

        private void FolderSelection_Click(object sender, EventArgs e)
        {
            DialogResult result = this.folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                pathOfDirectoryOfFileComparison = this.folderBrowserDialog1.SelectedPath;
            }


        }

    }
}