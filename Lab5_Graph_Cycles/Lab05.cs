using System;
using System.Collections.Generic;
using ASD.Graphs;
using System.Collections;
using System.Linq;

namespace ASD
{
    public class RoutePlanner : MarshalByRefObject
    {

        // mozna dodawac metody pomocnicze

        /// <summary>
        /// Znajduje dowolny cykl skierowany w grafie lub stwierdza, że go nie ma.
        /// </summary>
        /// <param name="g">Graf wejściowy</param>
        /// <returns>Kolejne wierzchołki z cyklu lub null, jeśli cyklu nie ma.</returns>
        public int[] FindCycle(Graph g)
        {
            if (g.EdgesCount == 0 || g.Directed == false) return null;
            List<int> lista = new List<int>();
            int start = -1;

            Predicate<int> pre = delegate (int n)
            {
                lista.Add(n);
                return true;
            };

            Predicate<int> post = delegate (int n)
            {
                lista.Remove(n);
                return true;
            };

            Predicate<Edge> edge = delegate (Edge e)
            {
                if (lista.Contains(e.To))
                {
                    start = e.To;
                    return false;
                }
                return true;
            };            

            g.GeneralSearchAll<EdgesStack>(pre, post, edge, out int cc);
            if (start == -1) return null;
            lista.RemoveRange(0, lista.IndexOf(start));
            return lista.ToArray();
        }

        /// <summary>
        /// Rozwiązanie wariantu 1.
        /// </summary>
        /// <param name="g">Graf połączeń, które trzeba zrealizować</param>
        /// <returns>Lista tras autobusów lub null, jeśli zadanie nie ma rozwiązania</returns>
        public int[][] FindShortRoutes(Graph g)
        {
            for(int i = 0; i < g.VerticesCount; i++)
            {
                if ((g.OutDegree(i) + g.InDegree(i)) % 2 != 0) return null;
            }
            Graph graph = g.Clone();
            List<int[]> lista = new List<int[]>();
            int[] cycle = FindCycle(graph);
            
            while (cycle != null)
            {
                lista.Add(cycle);
                for (int i = 0; i < cycle.Length - 1; i++)
                {
                    graph.DelEdge(cycle[i], cycle[i + 1]);
                }
                graph.DelEdge(cycle[cycle.Length - 1], cycle[0]);
                cycle = FindCycle(graph);
            }

            if (graph.EdgesCount != 0) return null;
            return lista.ToArray();
        }

        /// <summary>
        /// Rozwiązanie wariantu 2.
        /// </summary>
        /// <param name="g">Graf połączeń, które trzeba zrealizować</param>
        /// <returns>Lista tras autobusów lub null, jeśli zadanie nie ma rozwiązania</returns>
        /// </summary>
        public int[][] FindLongRoutes(Graph g)
        {
            int[][] shortCycles = FindShortRoutes(g);
            if (shortCycles == null) return null;
            bool IsChanged = true;
            List<List<int>> cycles = new List<List<int>>();          
            foreach (int[] elem in shortCycles)
            {
                cycles.Add(elem.ToList());
            }

            while(IsChanged)
            {
                IsChanged = false;
                for (int i = 0; i < cycles.Count; i++)
                {
                    for (int j = i + 1; j < cycles.Count; j++)
                    {
                        for(int z = 0; z < cycles[j].Count; z++)
                        {
                            if(cycles[i].Contains(cycles[j][z]))
                            {
                                List<int> tmp = new List<int>();
                                tmp.Add(cycles[j][z]);
                                int index = cycles[i].IndexOf(cycles[j][z]);
                                for (int k = index + 1; k < cycles[i].Count; k++)
                                    tmp.Add(cycles[i][k]);
                                for (int k = 0; k < index; k++)
                                    tmp.Add(cycles[i][k]);
                                tmp.Add(cycles[j][z]);
                                for (int k = z + 1; k < cycles[j].Count; k++)
                                    tmp.Add(cycles[j][k]);
                                for (int k = 0; k < z; k++)
                                    tmp.Add(cycles[j][k]);                 
                                cycles.Remove(cycles[j]);
                                cycles.Remove(cycles[i]);
                                cycles.Add(tmp);
                                IsChanged = true;
                                break;
                            }
                            if (IsChanged) break;
                        }
                        if (IsChanged) break;
                    }
                    if (IsChanged) break;
                }
            }

            shortCycles = new int[cycles.Count][];
            for (int i = 0; i < cycles.Count; i++)
                shortCycles[i] = cycles[i].ToArray();
            return shortCycles;
        }
    }

}