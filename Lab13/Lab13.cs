using System;
using System.Linq;
using ASD.Graphs;
using System.Collections.Generic;

namespace Lab13
{
public class ProgramPlanning  : MarshalByRefObject
    {

        /// <summary>
        ///
        /// </summary>
        /// <param name="taskGraph">Graf opisujący zależności procedur</param>
        /// <param name="taskTimes">Tablica długości czasów procedur</param>
        /// <param name="startTimes">Parametr wyjśćiowy z najpóźniejszymi możliwymi startami procedur przy optymalnym czasie całości</param>
        /// <param name="startTimes">Parametr wyjśćiowy z dowolna wybraną ścieżką krytyczną</param>
        /// <returns>Najkrótszy czas w jakim można wykonać cały program</returns>
        public double CalculateTimesLatestPossible(Graph taskGraph, double[] taskTimes, out double[] startTimes, out int[] criticalPath)
        {
            Graph graph = taskGraph.Clone();
            graph.TopologicalSort(out int[] org2top, out int[] top2org);
            startTimes = new double[taskGraph.VerticesCount];
            double[] times = (double[])taskTimes.Clone();
            List<int> criticialPathList = new List<int>();

            //Etap 1
            for (int i = (graph.VerticesCount) - 1; i >= 0; i--)
            {
                double maxNeighbour = 0;
                foreach (Edge e in graph.OutEdges(top2org[i]))
                {
                    if (times[e.To] >= maxNeighbour)
                        maxNeighbour = times[e.To];
                }
                times[top2org[i]] = taskTimes[top2org[i]] + maxNeighbour;
            }
            double time = times.Max();

            //Etap 2
            for (int i = (graph.VerticesCount) - 1; i >= 0; i--)
            {
                if (graph.OutEdges(top2org[i]).Count() == 0)
                {
                    startTimes[top2org[i]] = time - taskTimes[top2org[i]];
                    continue;
                }
                double minNeighbour = double.MaxValue;
                foreach (Edge e in graph.OutEdges(top2org[i]))
                {
                    if (startTimes[e.To] < minNeighbour) minNeighbour = startTimes[e.To];
                }
                startTimes[top2org[i]] = minNeighbour - taskTimes[top2org[i]];
            }

            //Etap 3
            int currentVertex = -1;
            for (int i = 0; i < startTimes.Length; i++)
            {
                if (startTimes[i] == 0)
                {
                    currentVertex = i;
                    break;
                }
            }
            while(graph.OutEdges(currentVertex).Count() != 0)
            {
                double minTime = double.MaxValue;
                int nextVertex = -1;
                foreach(Edge e in graph.OutEdges(currentVertex))
                {
                    if(startTimes[e.To] < minTime)
                    {
                        minTime = startTimes[e.To];
                        nextVertex = e.To;
                    }
                }
                criticialPathList.Add(currentVertex);
                currentVertex = nextVertex;
            }
            criticialPathList.Add(currentVertex);
            criticalPath = criticialPathList.ToArray();

            return time;
        }
    }
}
