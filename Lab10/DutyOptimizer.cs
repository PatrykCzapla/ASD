using ASD.Graphs;
using System;
using System.Collections.Generic;

namespace ASD
{
    public class DutyOptimizer : MarshalByRefObject
    {
        /// <summary>
        /// Metoda realizująca zadanie (sprzedaż lemoniady za granicą)
        /// </summary>
        /// <param name="possibleChangesBeforeBorder">Możliwe zamiany przed przekroczeniem granicy</param>
        /// <param name="possibleChangesAfterBorder">Możliwe zamiany po przekroczeniu granicy</param>
        /// <param name="substancesNumber">Liczba wszystkich substancji (identyfikatory substancji to 0 (lemoniada), 1, 2, ..., substancesNumber-1)</param>
        /// <param name="sellPrices">Tablica długości substancesNumber, i-ty element to cena jednostki i-tej substancji</param>
        /// <param name="lemonadeAmount">Liczba jednostek lemoniady do sprzedania za granicą</param>
        /// <param name="changesBeforeBorder">Wynikowe zamiany przed granicą</param>
        /// <param name="changesAfterBorder">Wynikowe zamiany za granicą</param>
        /// <returns>Całkowity zysk (przychód ze sprzedaży - wszystkie poniesione wydatki)</returns>
        public double CreateSimplePlan((int from, int to, int cost, int limit)[] possibleChangesBeforeBorder,
                                       (int from, int to, int cost, int limit)[] possibleChangesAfterBorder,
                                       int substancesNumber, double[] sellPrices, int lemonadeAmount,
                                       out List<(int from, int to, int amount)> changesBeforeBorder,
                                       out List<(int from, int to, int amount)> changesAfterBorder)
        {
            Graph limits = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 2 * substancesNumber + 2);
            Graph costs = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 2 * substancesNumber + 2);
            changesBeforeBorder = new List<(int from, int to, int amount)>();
            changesAfterBorder = new List<(int from, int to, int amount)>();
            //graf przepływu
            limits.AddEdge(0, 1, lemonadeAmount);
            limits.AddEdge(1, limits.VerticesCount -1, lemonadeAmount);
            limits.AddEdge(substancesNumber + 1, limits.VerticesCount - 1, lemonadeAmount);
            for (int i = 0; i < possibleChangesBeforeBorder.Length; i++)
                limits.AddEdge(possibleChangesBeforeBorder[i].from + 1, possibleChangesBeforeBorder[i].to + 1, possibleChangesBeforeBorder[i].limit);
            for(int i = 2; i <= substancesNumber; i++)
                limits.AddEdge(i, i + substancesNumber, double.PositiveInfinity);
            for (int i = 0; i < possibleChangesAfterBorder.Length; i++)
                limits.AddEdge(possibleChangesAfterBorder [i].from + 1 + substancesNumber, possibleChangesAfterBorder[i].to + 1 + substancesNumber, possibleChangesAfterBorder[i].limit);
            //graf kosztów
            costs.AddEdge(0, 1, 0);
            costs.AddEdge(1, limits.VerticesCount - 1, sellPrices[0] / 2);
            costs.AddEdge(substancesNumber + 1, limits.VerticesCount - 1, 0);
            for (int i = 0; i < possibleChangesBeforeBorder.Length; i++)
                costs.AddEdge(possibleChangesBeforeBorder[i].from + 1, possibleChangesBeforeBorder[i].to + 1, possibleChangesBeforeBorder[i].cost);
            for (int i = 2; i <= substancesNumber; i++)
                costs.AddEdge(i, i + substancesNumber, sellPrices[i-1] / 2);
            for (int i = 0; i < possibleChangesAfterBorder.Length; i++)
                costs.AddEdge(possibleChangesAfterBorder[i].from + 1 + substancesNumber, possibleChangesAfterBorder[i].to + 1 + substancesNumber, possibleChangesAfterBorder[i].cost);
            //obliczanie maksymalnego przepływu o minimalnym koszcie
            Graph flow = null;
            double amountSold = 0;
            double dutyCost = 0;
            (amountSold, dutyCost, flow) = limits.MinCostFlow(costs, 0, limits.VerticesCount - 1);
            double income = (int)amountSold * sellPrices[0] - dutyCost;
            //plan realizacji
            for (int i = 1; i <= substancesNumber; i++)
            {
                foreach (Edge e in flow.OutEdges(i))
                {
                    if (e.Weight == 0) continue;
                    if (e.To > substancesNumber) continue;
                    changesBeforeBorder.Add((e.From - 1, e.To - 1, (int)e.Weight));
                }                    
            }
            for (int i = substancesNumber + 1; i <= flow.VerticesCount - 2; i++)
            {
                foreach (Edge e in flow.OutEdges(i))
                {
                    if (e.Weight == 0) continue;
                    if (e.To == flow.VerticesCount - 1) continue;
                    changesAfterBorder.Add((e.From - substancesNumber - 1, e.To - substancesNumber - 1, (int)e.Weight));
                }                    
            }
            return income;
        }
    }
}