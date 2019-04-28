using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace ASD
{
    public class Lab08 : MarshalByRefObject
    {
        /// <summary>
        /// Znajduje cykl rozpoczynający się w stolicy, który dla wybranych miast,
        /// przez które przechodzi ma największą sumę liczby ludności w tych wybranych
        /// miastach oraz minimalny koszt.
        /// </summary>
        /// <param name="cities">
        /// Graf miast i połączeń między nimi.
        /// Waga krawędzi jest kosztem przejechania między dwoma miastami.
        /// Koszty transportu między miastami są nieujemne.
        /// </param>
        /// <param name="citiesPopulation">Liczba ludności miast</param>
        /// <param name="meetingCosts">
        /// Koszt spotkania w każdym z miast.
        /// Dla części pierwszej koszt spotkania dla każdego miasta wynosi 0.
        /// Dla części drugiej koszty są nieujemne.
        /// </param>
        /// <param name="budget">Budżet do wykorzystania przez kandydata.</param>
        /// <param name="capitalCity">Numer miasta będącego stolicą, z której startuje kandydat.</param>
        /// <param name="path">
        /// Tablica dwuelementowych krotek opisująca ciąg miast, które powinen odwiedzić kandydat.
        /// Pierwszy element krotki to numer miasta do odwiedzenia, a drugi element decyduje czy
        /// w danym mieście będzie organizowane spotkanie wyborcze.
        /// 
        /// Pierwszym miastem na tej liście zawsze będzie stolica (w której można, ale nie trzeba
        /// organizować spotkania).
        /// 
        /// Zakładamy, że po odwiedzeniu ostatniego miasta na liście kandydat wraca do stolicy
        /// (na co musi mu starczyć budżetu i połączenie między tymi miastami musi istnieć).
        /// 
        /// Jeżeli kandydat nie wyjeżdża ze stolicy (stolica jest jedynym miastem, które odwiedzi),
        /// to lista `path` powinna zawierać jedynie jeden element: stolicę (wraz z informacją
        /// czy będzie tam spotkanie czy nie). Nie są wtedy ponoszone żadne koszty podróży.
        /// 
        /// W pierwszym etapie drugi element krotki powinien być zawsze równy `true`.
        /// </param>
        /// <returns>
        /// Liczba mieszkańców, z którymi spotka się kandydat.
        /// </returns>

        public Graph cities;
        public int[] citiesPopulation;
        public double[] meetingCosts;
        public int capitalCity;
        public bool[] visited;
        public int best;
        
        public int ComputeElectionCampaignPath(Graph cities, int[] citiesPopulation, double[] meetingCosts, double budget, int capitalCity, out (int, bool)[] path)
        {
            path = null;
            visited = new bool[cities.VerticesCount];
            this.cities = cities;
            this.citiesPopulation = citiesPopulation;
            this.meetingCosts = meetingCosts;
            this.capitalCity = capitalCity;
            this.best = 0;
            double bestCost = double.MaxValue;
            List<(int, bool)> tmp = new List<(int, bool)> { (capitalCity, false) };
            ComputeElectionCampaignPathRec(budget, tmp, 0, ref path, ref bestCost, 0);
            if (budget - meetingCosts[capitalCity] >= 0)
            {
                if (best == 0) best = citiesPopulation[capitalCity];
                visited = new bool[cities.VerticesCount];
                tmp = new List<(int, bool)> { (capitalCity, true) };
                ComputeElectionCampaignPathRec((budget - meetingCosts[capitalCity]), tmp, citiesPopulation[capitalCity], ref path, ref bestCost, meetingCosts[capitalCity]);
            }
            if (best == citiesPopulation[capitalCity] && budget - meetingCosts[capitalCity] >= 0)
                path = new[] {(capitalCity,true)};
            else if (best == 0)
                path = new[] { (capitalCity, false) };
            return best; 
        }

        public void ComputeElectionCampaignPathRec(double currentBudget, List<(int,bool)> tmpPath, int current, ref (int,bool)[] path, ref double bestCost, double currentCost)
        {
            foreach (Edge e in cities.OutEdges(tmpPath.Last().Item1))
            {
                if (visited[e.To]) continue;               
                if (e.To == capitalCity)
                {
                    if (current < best || currentBudget - e.Weight < 0) continue;
                    else
                    {
                        if (currentCost + e.Weight >= bestCost && current == best) continue;
                        best = current;
                        bestCost = currentCost + e.Weight;
                        path = tmpPath.ToArray();
                        continue;
                    }
                }
                if (currentBudget - e.Weight <= 0) continue;
                visited[e.To] = true;
                currentBudget -= e.Weight;
                currentCost += e.Weight;
                if (meetingCosts[e.To] == 0)
                {
                    tmpPath.Add((e.To, true));                   
                    current += citiesPopulation[e.To];
                    ComputeElectionCampaignPathRec(currentBudget, tmpPath, current, ref path, ref bestCost, currentCost);
                }
                else
                {
                    tmpPath.Add((e.To, false));
                    ComputeElectionCampaignPathRec(currentBudget, tmpPath, current, ref path, ref bestCost, currentCost);
                    if (currentBudget - meetingCosts[e.To] <= 0)
                    {
                        tmpPath.RemoveAt(tmpPath.Count - 1);
                        visited[e.To] = false;
                        currentBudget += e.Weight;
                        currentCost -= e.Weight;
                        continue;
                    }
                    tmpPath.RemoveAt(tmpPath.Count - 1);
                    tmpPath.Add((e.To, true));
                    currentBudget -= meetingCosts[e.To];
                    currentCost += meetingCosts[e.To];
                    current += citiesPopulation[e.To];
                    ComputeElectionCampaignPathRec(currentBudget, tmpPath, current, ref path, ref bestCost, currentCost);                    
                }
                tmpPath.RemoveAt(tmpPath.Count - 1);
                visited[e.To] = false;
                currentBudget += e.Weight;
                currentCost -= meetingCosts[e.To];
                currentBudget += meetingCosts[e.To];
                current -= citiesPopulation[e.To];
                currentCost -= e.Weight;
            }
        }
    }
}
