using System;
using System.Collections.Generic;
using ASD.Graphs;
using System.Linq;

namespace ASD
{
    public class Lab03 : MarshalByRefObject
    {
        // Część 1
        //  Sprawdzenie czy podany ciąg stopni jest grafowy
        //  0.5 pkt
        public bool IsGraphic(int[] sequence)
        {
            if (sequence == null) return false;
            if (sequence.All(x => x == 0)) return true;
            List<int> seq = new List<int>(sequence);
            while(seq.Count>0)
            {
                seq.Sort();
                seq.Reverse();
                if (seq.Sum() % 2 != 0 || seq[0] > seq.Count) return false;
                for(int i = 1;i<seq[0]+1;i++)
                {
                    seq[i]--;
                }
                seq.RemoveAt(0);
                if (seq.All(x => x == 0))return true;
                if (seq.Any(x => x < 0)) return false;
            }
            return false;
        }

        //Część 2
        // Konstruowanie grafu na podstawie podanego ciągu grafowego
        // 1.5 pkt
        public Graph ConstructGraph(int[] sequence)
        {
            if (sequence == null) return null;
            if (!IsGraphic(sequence)) return null;
            AdjacencyListsGraph<SimpleAdjacencyList> g = new AdjacencyListsGraph<SimpleAdjacencyList>(false, sequence.Length);
            List<(int, int)> lista = new List<(int, int)>();
            sequence = sequence.OrderByDescending(x => x).ToArray();
            for (int i = 0; i < sequence.Length; i++)
            {
                lista.Add((i, sequence[i]));
            }
            while(lista.Count() > 0)
            {
                if (lista.ElementAt(0).Item2 <= 0) break;
                int size = lista.ElementAt(0).Item2;
                for (int i = 1; i <= size; i++)
                {
                    if (lista.ElementAt(i).Item2 <= 0) continue;
                    lista[i] = (lista.ElementAt(i).Item1, lista.ElementAt(i).Item2 - 1);
                    lista[0] = (lista.ElementAt(0).Item1, lista.ElementAt(0).Item2 - 1);
                    g.AddEdge(lista.ElementAt(0).Item1, lista.ElementAt(i).Item1);
                }
                lista.RemoveAt(0);
                lista = lista.OrderByDescending(y => y.Item2).ToList();               
            }
            return g;
        }

        //Część 3
        // Wyznaczanie minimalnego drzewa (bądź lasu) rozpinającego algorytmem Kruskala
        // 2 pkt
        public Graph MinimumSpanningTree(Graph graph, out double min_weight)
        {
            if (graph.Directed == true) throw new ArgumentException();
            Graph g = graph.Clone();
            int counter = 0;
            EdgesMinPriorityQueue edges = new EdgesMinPriorityQueue();
            UnionFind uf = new UnionFind(g.VerticesCount);
            for(int i=0;i<g.VerticesCount;i++)
            {
                foreach(var e in g.OutEdges(i))
                {
                    if(e.To<e.From)
                        edges.Put(e);
                }
            }
            min_weight = 0;
            g = g.IsolatedVerticesGraph();
            while (!edges.Empty && counter != g.VerticesCount - 1)
            {
                Edge e = edges.Get();
                if(uf.Find(e.From)!=uf.Find(e.To))
                {
                    uf.Union(e.From, e.To);
                    g.AddEdge(e);
                    min_weight += e.Weight;
                    counter++;
                }
            }           
            return g;
        }
    }
}