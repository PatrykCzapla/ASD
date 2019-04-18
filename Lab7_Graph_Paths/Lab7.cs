using System;
using System.Linq;
using ASD.Graphs;
using System.Collections.Generic;

namespace Lab7
{
    public class BestCitiesSolver : MarshalByRefObject
    {

        public (int c1, int c2, int? bypass, double time, Edge[] path)? FindBestCitiesPair(Graph times, double[] passThroughCityTimes, int[] nominatedCities, bool buildBypass)
        {
            Graph g = times.IsolatedVerticesGraph(true, times.VerticesCount);
            int c1 = -1;
            int c2 = -1;
            int city1 = -1;
            int city2 = -1;
            double best = double.MaxValue;
            for (int i = 0; i < times.VerticesCount; i++)
                foreach (Edge e in times.OutEdges(i))
                {
                    g.AddEdge(e.To, e.From, e.Weight + passThroughCityTimes[e.To]);
                    g.AddEdge(e.From, e.To, e.Weight + passThroughCityTimes[e.From]);
                }
            PathsInfo[][] distance = new PathsInfo[nominatedCities.Length][];
            for(int i = 0; i < nominatedCities.Length; i++)
            {
                foreach (Edge e in g.OutEdges(nominatedCities[i]))
                    g.ModifyEdgeWeight(e.From, e.To, -passThroughCityTimes[nominatedCities[i]]);
                g.DijkstraShortestPaths(nominatedCities[i], out distance[i]);
                foreach (Edge e in g.OutEdges(nominatedCities[i]))
                    g.ModifyEdgeWeight(e.From, e.To, passThroughCityTimes[nominatedCities[i]]);
                for (int j = 0; j < nominatedCities.Length; j++)
                {
                    if (i == j) continue;
                    if (distance[i][nominatedCities[j]].Dist.IsNaN()) continue;
                    if ((distance[i][nominatedCities[j]].Dist < best))
                    {
                        best = distance[i][nominatedCities[j]].Dist;
                        c1 = nominatedCities[i];
                        c2 = nominatedCities[j];
                        city1 = i;
                        city2 = j;
                    }
                }                
            }
            if (best == double.MaxValue) return null;
            Edge[] path = PathsInfo.ConstructPath(c1, c2, distance[city1]);
            if(buildBypass == false)
                return (c1, c2, null, best, path);
            PathsInfo firstPart = new PathsInfo();
            PathsInfo secondPart = new PathsInfo();
            int passedVertex = -1;
            for (int i = 0; i < nominatedCities.Length; i++)
            {
                for (int j = 0; j < nominatedCities.Length; j++)
                {
                    if (i == j) continue;
                    for (int z = 0; z < times.VerticesCount; z++)
                    {
                        if (z == nominatedCities[i] || z == nominatedCities[j]) continue;
                        if (distance[i][z].Dist.IsNaN() || distance[j][z].Dist.IsNaN()) continue;
                        if (distance[i][z].Dist + distance[j][z].Dist < best)
                        {
                            passedVertex = z;
                            firstPart = distance[i][z];
                            secondPart = distance[j][z];
                            best = distance[i][z].Dist + distance[j][z].Dist;
                            c1 = nominatedCities[i];
                            c2 = nominatedCities[j];
                            city1 = i;
                            city2 = j;
                        }
                    }
                }
            }
            if (passedVertex == -1)
                return (c1, c2, null, best, path);
            List<Edge> tmp = new List<Edge>();
            List<Edge> tmp2 = new List<Edge>();
            tmp.Add((Edge)firstPart.Last);
            int prev = ((Edge)firstPart.Last).From;
            while (prev != c1)
            {
                tmp.Add((Edge)distance[city1][prev].Last);
                prev = ((Edge)distance[city1][prev].Last).From;
            }
            tmp2.Add(new Edge(((Edge)secondPart.Last).To, ((Edge)secondPart.Last).From, ((Edge)secondPart.Last).Weight));
            prev = ((Edge)secondPart.Last).From;
            while (prev != c2)
            {
                tmp2.Add(new Edge(((Edge)distance[city2][prev].Last).To, ((Edge)distance[city2][prev].Last).From, ((Edge)distance[city2][prev].Last).Weight));
                prev = ((Edge)distance[city2][prev].Last).From;
            }
            tmp.Reverse();
            foreach (Edge e in tmp2)
                tmp.Add(e);
            return (c1, c2, passedVertex, best, tmp.ToArray());
        }

    }

}

