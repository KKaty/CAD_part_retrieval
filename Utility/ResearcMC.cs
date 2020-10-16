namespace SolidWorksAddinUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Accord.Math;
    using SolidWorks.Interop.sldworks;
    using SWIntegration.Data_Structure;

    // Metodo per la ricerca del massimo ciclo dato un grafo associato.
    /// <summary>
    /// The utility.
    /// </summary>
    public partial class Utility
    {
        // Parametri per il simulated annealing, valori presi dall'articolo di riferimento.
        /// <summary>
        /// The initial temperature.
        /// Costante che identifica la temperatura iniziale del processo di simulated annealing.
        /// </summary>
        private const double InitialTemperature = 1000;

        /// <summary>
        /// The final temperature.
        /// Costante che identifica la temperatura finale del processo di simulated annealing.
        /// </summary>
        private const double FinalTemperature = 0.1;

        /// <summary>
        /// The decreasing parameter.
        /// Costante che identifica la costante con la quale decrementare la temperatura ad ogni passaggio del processo di simulated annealing.
        /// </summary>
        private const double DecreasingParameter = 0.99;

        /// <summary>
        /// The my sum adjacent matrix.
        /// Dato un grafo associato ne costruisco la matrice di adiacenza (triangolare superiore)
        /// In realtà non occorre tutta la matrice di adiacwnza ma solamente \sum M_i,j, quindi basta
        /// sommare gli archi di ogni nodo e dividere per due (perché si vuole solo una parte della somma della matrice)
        /// </summary>
        /// <param name="associativeGraph">
        /// The associative graph.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int MySumAdjacentMatrix(AssociativeGraph associativeGraph)
        {
            var sumAdjiacentMatrix = 0;
            var nodeList = associativeGraph.listNode;

            foreach (AssociativeNode node in nodeList)
            {
                foreach (AssociativeNode destinationAssociativeNode in node.AssociativeLinks)
                {
                    
                    if (nodeList.Contains(destinationAssociativeNode))
                    {
                        sumAdjiacentMatrix++;
                    }
                }

            }

            return sumAdjiacentMatrix/2;
        }
        /// <summary>
        /// The my minimun function.
        /// </summary>
        /// <param name="subGraph">
        /// The sub graph.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int MyMinimunFunction(AssociativeGraph subGraph)
        {
                var min = 0;
                if (subGraph != null)
                {
                    var k = subGraph.listNode.Count();
                    var sumSubGraph = MySumAdjacentMatrix(subGraph);
                    min = k * (k - 1) / 2 - sumSubGraph;
                }
                return min;
          
        }
        // I vertici v_u sono selizionati dall'insieme V_s, mentre i vertici v_w sono selizionati dall'insieme V\V_S.
        private static AssociativeGraph  MySwap(AssociativeGraph originalGraph, AssociativeGraph subGraph, bool fullQuery, SldWorks mySwApplication)
        {
            int count = 0;
            
            var componentConness = new List<AssociativeNode>();
            componentConness.AddRange(originalGraph.listNode);
           
            var listVs = new List<AssociativeNode>();
            var listV_Vs = new List<AssociativeNode>();

            listVs.AddRange(subGraph.listNode);
            listVs.RemoveAll(x => x.AssociativeLinks.Count == 0);

            listV_Vs.AddRange(componentConness);
            listV_Vs.RemoveAll(x => listVs.Contains(x)); // Dalla lista dei vertici V rimuovo i vertici in V_s.

           // Random randomInt = new Random();
           // var intToRand = randomInt.Next();
            Random randomIndex = new Random();            

            /* Funzione di stampa per debug */
            /*
            Utility.PrintToFile("nodo estratto da compl", "swap.txt");
            if (listVertexW == null)
            {
                Utility.PrintToFile("ESTRAZIONE NULLA", "swap.txt");
            }
            else
            {
                foreach (AssociativeNode nodo in listVertexW)
                {
                    var aString = string.Format(
                                    "{0} --- {1}",
                        nodo.NodeFirstGraph.IdNode,
                        nodo.NodeSecondGraph.IdNode);
                    Utility.PrintToFile(aString, "swap.txt");
                }
            }
            
            
            Utility.PrintToFile("nodo estratto da sottografo", "swap.txt");
            if (listVertexW == null)
            {
                Utility.PrintToFile("ESTRAZIONE NULLA", "swap.txt");
            }
            else
            {
                foreach (AssociativeNode nodo in listVertexU)
                {
                    var aString = string.Format(
                                    "{0} --- {1}",
                        nodo.NodeFirstGraph.IdNode,
                        nodo.NodeSecondGraph.IdNode);
                   // Utility.PrintToFile(aString, "swap.txt");
                }
            }
            */
            /* fine funzione stampa per debug */

            int maxIteration;
                
            if(fullQuery == false)
            {
                maxIteration =  8*(originalGraph.listNode.Count);
            }
            else
            {
                maxIteration = 5;// originalGraph.listNode.Count / 5;
            }

            //mySwApplication.SendMsgToUser("maxIteration IMPOSTATO A " + maxIteration.ToString());
            var cliqueLenght = subGraph.listNode.Count;

            while (count < maxIteration)
            {
                count += 1;

                listVs.OrderBy(x => randomIndex.Next());
                int firstIndex = randomIndex.Next(0, listVs.Count);
                AssociativeNode vertexU = null;
                vertexU = listVs[firstIndex];

                List<AssociativeNode> removedNodes = listV_Vs.FindAll(x => x.NodeFirstGraph.Equals(vertexU.NodeFirstGraph) || x.NodeSecondGraph.Equals(vertexU.NodeSecondGraph));
                listV_Vs.RemoveAll(x => x.NodeFirstGraph.Equals(vertexU.NodeFirstGraph) || x.NodeSecondGraph.Equals(vertexU.NodeSecondGraph));

                /*
                List<AssociativeNode> removedNodes = new List<AssociativeNode>();
                foreach (AssociativeNode associativeNode in listVs)
                {
                    if (!associativeNode.Equals(vertexU))
                    {
                        listV_Vs.FindAll(x => x.NodeFirstGraph.Equals(associativeNode.NodeFirstGraph) || x.NodeSecondGraph.Equals(associativeNode.NodeSecondGraph));
                        listV_Vs.RemoveAll(x => x.NodeFirstGraph.Equals(associativeNode.NodeFirstGraph) || x.NodeSecondGraph.Equals(associativeNode.NodeSecondGraph));
                    }
                }
                */
                if (listV_Vs.Count == 0)
                {
                    //mySwApplication.SendMsgToUser("Non ci sono nodi da swappare");
                    return null;  // Se non ci sono elenti su cui eseguire lo swap, non si effettua lo swap.
                }
                listV_Vs.OrderBy(x => randomIndex.Next());
                int secondIndex = randomIndex.Next(0, listV_Vs.Count);
                AssociativeNode vertexW = listV_Vs[secondIndex];
                //Utility.PrintToFile("primo indice " + firstIndex, "swap.txt");
                //Utility.PrintToFile("secondo indice " + secondIndex, "swap.txt");

                if (vertexW != null)
                {
                     
                    if (vertexU != null)
                    {
                        // Nuovo swap, veloce ma al momento è sbagliato
                        
                        List<AssociativeNode> originalAdiacentVertexU = new List<AssociativeNode>();
                        List<AssociativeNode> originalAdiacentVertexW = new List<AssociativeNode>();
/*
                       // mySwApplication.SendMsgToUser("vertice prima eliminazione U" + vertexU.AssociativeLinks.Count().ToString() + " vertice prima eliminazione W" + vertexW.AssociativeLinks.Count().ToString());
                        originalAdiacentVertexU = vertexU.AssociativeLinks.FindAll(x => !listVs.Contains(x));
                        vertexU.AssociativeLinks.RemoveAll(x => !listVs.Contains(x));
                        
                        //var newPossibleSubGraph = new List<AssociativeNode>();
                        //newPossibleSubGraph.AddRange(listVs);
                        //newPossibleSubGraph.Remove(vertexU);
                        //newPossibleSubGraph.Add(vertexW);
                         
                        originalAdiacentVertexW = vertexW.AssociativeLinks.FindAll(x => !listVs.Contains(x) || x.Equals(vertexU));
                        vertexW.AssociativeLinks.RemoveAll(x => !listVs.Contains(x) || x.Equals(vertexU));

                        int degreeU = vertexU.AssociativeLinks.Count();
                        int degreeW = vertexW.AssociativeLinks.Count(); // Calcolo il grado dei vertici

                        vertexU.AssociativeLinks.AddRange(originalAdiacentVertexU);
                        vertexW.AssociativeLinks.AddRange(originalAdiacentVertexW);
                  */      
                       // mySwApplication.SendMsgToUser("vertice U" + degreeU.ToString() + " vertice W" + degreeW.ToString());
                       // mySwApplication.SendMsgToUser("vertice originale U" + vertexU.AssociativeLinks.Count().ToString() + " vertice originale W" + vertexW.AssociativeLinks.Count().ToString());
                    
                       //Swap funzionante ma lento!
                      
                        int degreeU = 0;
                        foreach (AssociativeNode destinationAssociativeNode in vertexU.AssociativeLinks)
                        {
                            if (listVs.Contains(destinationAssociativeNode) && vertexU != destinationAssociativeNode)
                            {
                                degreeU++;
                            }
                        }

                        int degreeW = 0;
                        var newPossibleSubGraph = new List<AssociativeNode>();
                        newPossibleSubGraph.AddRange(listVs);
                        newPossibleSubGraph.Remove(vertexU);
                        newPossibleSubGraph.Add(vertexW);
                        foreach (AssociativeNode destinationAssociativeNode in vertexW.AssociativeLinks)
                        {
                            if (newPossibleSubGraph.Contains(destinationAssociativeNode) && vertexW != destinationAssociativeNode)
                            {
                                degreeW++;
                            }
                        }
                        
                        
                        if (degreeW >= degreeU)
                        {/*
                            var aString = string.Format(
                                "Indice swappato: {0} -- {1} --- {2}-{3} ->{4}-{5}",
                                firstIndex.ToString(),
                                secondIndex.ToString(),
                                vertexU.NodeFirstGraph.IdNode,
                                vertexU.NodeSecondGraph.IdNode,
                                vertexW.NodeFirstGraph.IdNode,
                                vertexW.NodeSecondGraph.IdNode);
                            Utility.PrintToFile(aString, "swap.txt");
                           */
                            listVs.Remove(vertexU);
                            listVs.Add(vertexW);
                            break;

                        }
                    }
                    else { mySwApplication.SendMsgToUser("scambio nullo"); }
                }
                else { mySwApplication.SendMsgToUser("scambio nullo"); }
               listV_Vs.AddRange(removedNodes);
            }

            //mySwApplication.SendMsgToUser("nodi listVs " + listVs.Count.ToString());
            var newSubGraph = new AssociativeGraph(listVs);
           
            return newSubGraph;
        }

        private static AssociativeGraph MySimulatedAnnealing(AssociativeGraph originalGraph, AssociativeGraph subGraph, int cliqueLenght, bool fullQuery, SldWorks mySwApplication)
        {
           // mySwApplication.SendMsgToUser("Inizio simulated annealing con " + originalGraph.listNode.Count.ToString()+ " nodi");
            var subGraphReturn = subGraph;

            if (subGraphReturn == null)
                mySwApplication.SendMsgToUser("sottografo nullo in input");
            /*
            Utility.PrintToFile("Sottografo di nodi \n", "SimulateAnnealing.txt");
            foreach (AssociativeNode associativeNode in subGraphReturn.listNode)
            {
                string stampa = string.Format("{0} -- {1}", associativeNode.NodeFirstGraph.IdNode, associativeNode.NodeSecondGraph.IdNode);
                Utility.PrintToFile(stampa, "SimulateAnnealing.txt");
            }
             * */
            var temperature = InitialTemperature;
            var minimalFunction = MyMinimunFunction(subGraph);

            var stringa = "Valore funzione sottografo " + minimalFunction.ToString();
          //  Utility.PrintToFile(stringa, "swap.txt");
          //  Utility.PrintToFile(stringa, "SimulateAnnealing.txt");
            //mySwApplication.SendMsgToUser("Simulated Annealin prima del while \n valore funzione sottografo " + minimalFunction.ToString());
            var numberSwapCall = 0;
            while (temperature > FinalTemperature & minimalFunction > 0)
            {
               // mySwApplication.SendMsgToUser("nodi original graph per swap " + originalGraph.listNode.Count.ToString() + "subgraph " + subGraphReturn.listNode.Count.ToString());

                //mySwApplication.SendMsgToUser("Inizio swap");
                var sNew = MySwap(originalGraph, subGraphReturn, fullQuery, mySwApplication);
                if (sNew == null)
                {
                    //mySwApplication.SendMsgToUser("Swap nullo");
                    return subGraph;
                }

                numberSwapCall++;

                var numLinksSubGraph = 0;
          //      Utility.PrintToFile("Sottografo di nodi dopo lo swap \n", "SimulateAnnealing.txt");
                foreach (AssociativeNode associativeNode in sNew.listNode)
                {
                    foreach (AssociativeNode node in sNew.listNode)
                    {

                        if (associativeNode.AssociativeLinks.Contains(node) && node != associativeNode)
                            {
                                numLinksSubGraph++;
                            }
                        

                    }
                    //string stampa = string.Format("nodi sottografo swappato {0} -- {1} con archi --> {2}", associativeNode.NodeFirstGraph.IdNode, associativeNode.NodeSecondGraph.IdNode, numLinksSubGraph);
                    //Utility.PrintToFile(stampa, "SimulateAnnealing.txt");
                }
                
                var fNew = MyMinimunFunction(sNew);
              //  string provaStampa = "Valore funzione sottografo " + fNew.ToString();
               // Utility.PrintToFile(provaStampa, "swap.txt");
              //  Utility.PrintToFile(provaStampa, "SimulateAnnealing.txt");
                // mySwApplication.SendMsgToUser("archi del grafo swap \n" + stampa + 
                //   "\n funzione vecchia " + minimalFunction.ToString() + "funzione nuova " + fNew.ToString());
                var d = fNew - minimalFunction;
                
                if (d <= 0)
                {
                    subGraphReturn = sNew;
                    minimalFunction = fNew;
                    // Utility.PrintToFile("NUOVO SOTTOGRAFO ACCETTATO", "SimulateAnnealing.txt");
                    
                    //  break;
                }
                   
                else
                {


                    var prob = Math.Exp(-d / temperature);
                    var random = new Random();
                    var randomNumer = random.NextDouble();
                    randomNumer = random.NextDouble();
                    
                    if (prob - randomNumer > 0)
                    { 
                        subGraphReturn = sNew;
                        minimalFunction = fNew;
                        //Utility.PrintToFile("NUOVO SOTTOGRAFO ACCETTATO CON PROBABILITA'", "SimulateAnnealing.txt");
                       // mySwApplication.SendMsgToUser("Accetto la soluzione con probabilità " + prob.ToString() + " " + randomNumer.ToString() + 
                           // "\n decremento " + d.ToString() + " temperatura " + temperature.ToString());
                        //   break;
                    }
                    
                }
               
                temperature = DecreasingParameter * temperature;
            }
            
            if (subGraphReturn != null)
            {
               // Utility.PrintToFile("\n\nTemperatura finale " + temperature + "\n minimal function " + minimalFunction.ToString() + " lunghezza ciclo " + subGraphReturn.listNode.Count.ToString(), "SimulateAnnealing.txt");
            }
            else
            {
               // Utility.PrintToFile("simulated annealing nullo", "SimulateAnnealing.txt");
            }
            
            return subGraphReturn;
        }

        /// <summary>
        /// The my initialize sub graph.
        /// Funzione che crea un sottografo arbitrario S da un grafo G con k numero di nodi.
        /// </summary>
        /// <param name="originalGraph">
        /// The original graph.
        /// </param>
        /// <param name="numerOfVerticesOfSubGraph">
        /// The numer of vertices of sub graph.
        /// </param>
        /// <returns>
        /// The <see cref="AssociativeGraph"/>.
        /// </returns>
        private static AssociativeGraph MyInitializeSubGraph(AssociativeGraph originalGraph, int numerOfVerticesOfSubGraph, bool fullQuery, SldWorks mySwApplication)
        { 
            // Si riordinano i vertici del grafo originale in base al loro grado (numero di nodi incidenti),
            // successivamente vengono estratti i primi k nodi per formare il sottografo.
            var listNode = (List<AssociativeNode>)originalGraph.listNode;
            listNode.RemoveAll(x => x.AssociativeLinks.Count == 0);
            var listNodeOrdered = listNode.OrderBy(x => x.AssociativeLinks.Count).ToList();
            
            // Nuova inizializzazione sottografo in modo che non ci siano nodi ripetuti
           /*
            var listNodeSubGraph = new List<AssociativeNode>();
            int addedNodeNumber = 0;
           
            while (addedNodeNumber < numerOfVerticesOfSubGraph)
            {
               // mySwApplication.SendMsgToUser("Nodi per creare il sottografo" + listNodeOrdered.Count.ToString());
                if (listNodeOrdered.Count >= numerOfVerticesOfSubGraph - addedNodeNumber)
                {
                    AssociativeNode addedNode = listNodeOrdered.First();
                    listNodeSubGraph.Add(addedNode);
                    addedNodeNumber++;
                    listNodeOrdered.RemoveAll(x => x.NodeFirstGraph.Equals(addedNode.NodeFirstGraph) || x.NodeSecondGraph.Equals(addedNode.NodeSecondGraph));
                }
                else break;
                
            }
             
                  
            var initializeSubGraph = new AssociativeGraph(listNodeSubGraph);
            //if (listNodeSubGraph.Count == numerOfVerticesOfSubGraph)
            {

                return initializeSubGraph;
            }
            
            if (fullQuery)
            {
                if (listNodeSubGraph.Count == numerOfVerticesOfSubGraph)
                {

                    return initializeSubGraph;
                }
            }
            else
            {
                return initializeSubGraph;
            }
            */
       
            


            // Vecchia inizializzazione sottografo
            
            var listNodeSubGraph = new List<AssociativeNode>();
            if (listNodeOrdered.Count >= numerOfVerticesOfSubGraph)
            {
               

                listNodeSubGraph.Add(listNodeOrdered.First());
                listNodeSubGraph.AddRange(listNodeOrdered.Take(numerOfVerticesOfSubGraph)); // Prendo i primi k nodi.
                var numLinksSubGraph = 0;
                
                // Calcolo numero archi sel sottografo
                foreach (AssociativeNode node in listNodeSubGraph)
                {
                    foreach (AssociativeNode destinationAssociativeNode in node.AssociativeLinks)
                    {

                        if (listNodeSubGraph.Contains(destinationAssociativeNode))
                        {
                            numLinksSubGraph++;
                        }
                    }

                }

                var maxIteration = 0;
                while (numLinksSubGraph == 0 && maxIteration < 10)
                {
                    maxIteration++;
                    var lisNodeCompleteGraph = new List<AssociativeNode>();
                    lisNodeCompleteGraph.AddRange(listNode);
                    lisNodeCompleteGraph.RemoveAll(x => listNodeSubGraph.Contains(x));
                    var indexRandom = new Random();

                    var firstIndex = indexRandom.Next(listNodeSubGraph.Count());
                    var secondIndex = indexRandom.Next(lisNodeCompleteGraph.Count());

                    AssociativeNode firstVertex = listNodeSubGraph[firstIndex];
                    AssociativeNode secondVertex = lisNodeCompleteGraph[secondIndex];

                    listNodeSubGraph.Remove(firstVertex);
                    listNodeSubGraph.Add(secondVertex);

                    numLinksSubGraph = 0;
                    foreach (AssociativeNode node in listNodeSubGraph)
                    {
                        foreach (AssociativeNode destinationAssociativeNode in node.AssociativeLinks)
                        {

                            if (listNodeSubGraph.Contains(destinationAssociativeNode) && node != destinationAssociativeNode)
                            {
                                numLinksSubGraph++;
                            }
                        }

                    }
                }
                
                

                var prova = new List<AssociativeNode>();
                foreach (AssociativeNode nodo in listNodeSubGraph)
                {
                    prova.Add(nodo);
                }
                var initializeSubGraph = new AssociativeGraph((List<AssociativeNode>)prova);
                return initializeSubGraph;
          }
            

         //   mySwApplication.SendMsgToUser("Sottografo inizializzato ritornato " + listNodeSubGraph.Count.ToString());

            return null;
        }

        /// <summary>
        /// The my clique finder.
        /// </summary>
        /// <param name="originalGraph">
        /// The original graph.
        /// </param>
        /// <param name="lowerBound">
        /// The lower bound.
        /// </param>
        /// <param name="upperBound">
        /// The upper bound.
        /// </param>
        /// <param name="mySwApplication">
        /// The my sw application.
        /// </param>
        /// <returns>
        /// The <see cref="AssociativeGraph"/>.
        /// </returns>
        public static AssociativeGraph MyCliqueFinder(AssociativeGraph originalGraph, int lowerBound, int upperBound, bool fullQuery, SldWorks mySwApplication)
        {
            int cliqueLenght = lowerBound;
            AssociativeGraph oldClique = null;

            if (originalGraph.listNode.Count <= lowerBound)
            {
                return null;
            }

            int minFunction = 0;

            while (cliqueLenght <= upperBound && minFunction == 0)
            {
                var subGraph = MyInitializeSubGraph(originalGraph, cliqueLenght, fullQuery, mySwApplication);
                if (subGraph != null)
                {
                    
                    oldClique = MySimulatedAnnealing(originalGraph, subGraph, cliqueLenght, fullQuery, mySwApplication);
                    minFunction = MyMinimunFunction(oldClique);


                    //mySwApplication.SendMsgToUser("entro nel while");
                    cliqueLenght++;
                    //newClique = oldClique;

                }
                else { return null; }

            }

            //mySwApplication.SendMsgToUser("funzione " + minFunction.ToString() + "numero clique " + oldClique.listNode.Count.ToString());
            return oldClique;
        }

        public static double MyNumOfLinks(Graph myOriginalGraph, SldWorks mySWApplication)
        {

            var numOfLinks = 0;
            var nodeList = myOriginalGraph.Nodes;

            foreach (var node in nodeList)
            {
                foreach (var realLink in node.RealLinks)
                {

                    if (nodeList.Contains(realLink.DestinationNode))
                    {
                        numOfLinks++;
                    }
                }
                
                foreach (var virtualLink in node.VirtualLinks)
                {

                    if (nodeList.Contains(virtualLink.DestinationNode))
                    {
                        numOfLinks++;
                    }
                }
                
            }

            return numOfLinks/2;
        }
    }
}
