using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace Lab9
{
    public class DistributionFinder : MarshalByRefObject
    {
        public (int satisfactionLevel, int[] bestDistribution) FindBestDistribution(int[] limits, int[][] preferences, bool[] isSportActivity)
        {
            Graph flowGraph = null;
            AdjacencyListsGraph<AVLAdjacencyList> graph = new AdjacencyListsGraph<AVLAdjacencyList>(true, (preferences.GetLength(0) + limits.Length + 2));
            bool canDoSports = true;
            int[] bestDistribution = new int[preferences.GetLength(0)];
            double flow = 0;
            for (int i = 0; i < bestDistribution.Length; i++)
                bestDistribution[i] = -1;    
            
            for (int i = 1; i <= preferences.GetLength(0); i++)
            {
                graph.AddEdge(0, i, 1);
                if (preferences[i - 1] == null) continue;
                foreach (int elem in preferences[i - 1])                                  
                    graph.AddEdge(i, preferences.GetLength(0) + elem + 1, 1);
            }
            if(isSportActivity != null)
            {
                for (int i = preferences.GetLength(0) + 1; i < graph.VerticesCount - 1; i++)
                {
                    if (isSportActivity[i-1-preferences.GetLength(0)] == false) continue;
                    graph.AddEdge(i, graph.VerticesCount - 1, limits[i - 1 - preferences.GetLength(0)]);
                }
            }
            else
            {
                for (int i = preferences.GetLength(0) + 1; i < graph.VerticesCount - 1; i++)
                    graph.AddEdge(i, graph.VerticesCount - 1, limits[i - 1 - preferences.GetLength(0)]);
            }
            (flow, flowGraph) = graph.FordFulkersonDinicMaxFlow(0, graph.VerticesCount - 1, MaxFlowGraphExtender.DFSBlockingFlow);
            if (isSportActivity != null)
            {
                for(int i = preferences.GetLength(0) + 1; i < flowGraph.VerticesCount - 1; i++)
                {
                    if (isSportActivity[i - 1 - preferences.GetLength(0)] == false) continue;
                    foreach(Edge e in flowGraph.OutEdges(i))
                    {
                        if(e.Weight != limits[i - 1 - preferences.GetLength(0)])
                            canDoSports = false;
                    }
                    if (canDoSports == false) break;
                }
            }
            if(isSportActivity != null && canDoSports == false)
                return (0, null);
            foreach (Edge e in flowGraph.OutEdges(0))
            {
                if (e.Weight == 0) continue;
                else
                {
                    foreach (Edge ed in flowGraph.OutEdges(e.To))
                    {
                        if (ed.Weight == 0) continue;
                        bestDistribution[ed.From - 1] = ed.To - preferences.GetLength(0) - 1;
                    }
                }
            }
            if (isSportActivity != null && canDoSports == true)
                    return (1, bestDistribution);
            else return ((int)flow, bestDistribution);
        }
    }
}
