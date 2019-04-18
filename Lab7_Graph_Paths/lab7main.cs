using System;
using System.Linq;
using System.Collections.Generic;
using ASD;
using ASD.Graphs;

namespace Lab7
{
    public class BestCityPairTestCase : TestCase
    {
        private static readonly double EPS = 1e-3;
        private readonly Graph g;
        private readonly Graph gOrig;
        private readonly double[] passThroughCityTimes;
        private readonly int[] nominatedCities;
        private readonly double[] passThroughCityTimesOrig;
        private readonly int[] nominatedCitiesOrig;

        protected readonly double? bestTime;
        protected (int, int, int?, double, Edge[])? solution = null;
        protected virtual bool BuildBypass { get => false; }

        public BestCityPairTestCase(Graph g, double[] passThorughCityTimes, int[] nominatedCities, double? bestTime,
                                    double timeLimit, string description) : base(timeLimit, null, description)
        {
            this.g = g;
            this.gOrig = g.Clone();
            this.passThroughCityTimes = passThorughCityTimes;
            this.passThroughCityTimesOrig = passThorughCityTimes.Select(x => x).ToArray();
            this.nominatedCities = nominatedCities;
            this.nominatedCitiesOrig = nominatedCities.Select(x => x).ToArray();
            this.bestTime = bestTime;
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            var bcs = (BestCitiesSolver)prototypeObject;
            solution = bcs.FindBestCitiesPair(g, passThroughCityTimes, nominatedCities, BuildBypass);
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            if (CheckModifiedData())
            {
                return (Result.WrongResult, "Zmodyfikowano argumenty wejściowe!!");
            }
            else
                if (solution.HasValue && bestTime.HasValue)
                return CheckSolution(solution.Value);
            else
                    if (!solution.HasValue && !bestTime.HasValue)
            {
                return (Result.Success, $"OK (brak rozwiąznia), {PerformanceTime:#0.00}");
            }
            else
                return (Result.WrongResult, solution.HasValue ? "Zwrócono rozwiązanie, mimo że nie istnieje" : "Nie zwrócono rozwiązania, mimo że instnieje");
        }

        private bool CheckModifiedData()
        {
            return !gOrig.IsEqual(g) || passThroughCityTimes.Zip(passThroughCityTimesOrig, (x, y) => (x, y)).Any(a => a.Item1 != a.Item2)
                                     || nominatedCitiesOrig.Zip(nominatedCities, (x, y) => (x, y)).Any(a => a.Item1 != a.Item2);
        }

        private (Result resultCode, string message) CheckSolution((int, int, int?, double, Edge[]) value)
        {
            var (c1, c2, bypass, time, path) = value;
            if (!nominatedCities.Contains(c1) || !nominatedCities.Contains(c2))
            {
                return (Result.WrongResult, $"Zwrócone miasta {c1}, {c2} nie należą do tablicy miast proponowanych.");
            }
            else
            {
                return Math.Abs(time - bestTime.Value) > EPS ? (Result.WrongResult, $"Zwrócony nieoptymalny wynik. Proponowany czas: {time}, najlepszy: {bestTime}") :
                                                             CheckPath(path, time, c1, c2, bypass);
            }
        }

        private (Result resultCode, string message) CheckPath(Edge[] path, double declaredLength, int declaredStart, int declaredEnd, int? bypass)
        {
            if (path == null)
                return (Result.WrongResult, "Zwrócony null zamiast ścieżki");
            if (path.Length == 0)
                return (Result.WrongResult, "Zwrócona ścieżka o długości 0");
            if ((path.First().From != declaredStart || declaredEnd != path.Last().To) &&
                 (path.First().From != declaredEnd || declaredStart != path.Last().To))
                return (Result.WrongResult, "Zwrócona ścieżka nie łączy zwróconych miast");
            if (path.Any(e => double.IsNaN(g.GetEdgeWeight(e.From, e.To))))
                return (Result.WrongResult, "W ścieżce jest krawędź, która nie należy do grafu");
            double l = 0;
            bool good = true;
            l += g.GetEdgeWeight(path.First().From, path.First().To);
            for (int i = 1; i < path.Length && good; i++)
            {
                l += g.GetEdgeWeight(path[i].From, path[i].To);
                if (!bypass.HasValue || bypass.Value != path[i].From)
                    l += passThroughCityTimes[path[i].From];
                good = good && path[i - 1].To == path[i].From;
            }
            return good ? (Math.Abs(l - declaredLength) < EPS ? (Result.Success, $"OK, {PerformanceTime:#0.00}") :
                                                            (Result.WrongResult, $"Zwrócono czas {declaredLength}, a ścieżka sumuje się do {l}")) :
                          (Result.WrongResult, "Sąsiednie krawędzie w zwróconej ścieżce nie są incydentne");
        }

    }

    public class BestPairWithBypassTestCase : BestCityPairTestCase
    {
        private readonly bool needsBypass;
        protected override bool BuildBypass { get => true; }

        public BestPairWithBypassTestCase(Graph g, double[] passThorughCityTimes, int[] nominatedCities, double? bestTime,
                                          bool needsBypass, double timeLimit, string description) :
        base(g, passThorughCityTimes, nominatedCities, bestTime, timeLimit, description)
        {
            this.needsBypass = needsBypass;
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            if (solution.HasValue)
                if (needsBypass == solution.Value.Item3.HasValue) return base.VerifyTestCase(settings);
                else return needsBypass ? (Result.WrongResult, "Nie zaproponowano obwodnicy, mimo że poprawi wynik") : (Result.WrongResult, "Zaproponwano obwodnicę, mimo że nie jest potrzebna");
            else
                if (bestTime.HasValue)
                return (Result.WrongResult, "Nie zwrócono rozwiązania, mimo że instnieje");
            else
                return (Result.Success, $"OK (brak rozwiąznia), {PerformanceTime:#0.00}");
        }
    }

    class BestPairTests : TestModule
    {
        private TestSet bestPair = new TestSet(new BestCitiesSolver(), "Część 1: wyszukanie najlepszej pary miast");
        private TestSet bestPairWithBypass = new TestSet(new BestCitiesSolver(), "Część 2: wyszukanie najlepszej pary przy możliwosci budowy jednej obwodnicy");

        protected static readonly double TIME_MULTIPLIER = 1;

        public override void PrepareTestSets()
        {
            TestSets["BestPair"] = bestPair;
            TestSets["BestPairWithBypass"] = bestPairWithBypass;

            //Test1
            {
                var description = "Ścieżka, miasta do znalezienia na końcach";
                var graph = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 7);
                for (int i = 1; i < 7; i++)
                {
                    var ii = i * 10 % 7;
                    var iii = (i - 1) * 10 % 7;
                    graph.AddEdge(iii, ii, (i * 10 + 6) % 7);
                }
                var times = new double[7];
                for (int i = 0; i < 7; i++)
                {
                    times[i] = i + 2;
                }

                var nominatedCities = new int[2] { 0, 4 };
                var bestTime = 27 + 15;

                bestPair.TestCases.Add(new BestCityPairTestCase(graph, times, nominatedCities, bestTime, 1 * TIME_MULTIPLIER, description));
                bestPairWithBypass.TestCases.Add(new BestPairWithBypassTestCase(graph, times, nominatedCities, bestTime - 8, true, 1 * TIME_MULTIPLIER, description));
            }

            //Test 2
            {
                var description = "Siatka 4×4, gdzie kośzty przejazdów przez miasta wymuszająca obejście dookoła";
                var grid = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 16);
                int toIdx(int x, int y) => x * 4 + y;
                for (int i = 1; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        grid.AddEdge(toIdx(i - 1, j), toIdx(i, j), 1);
                        grid.AddEdge(toIdx(j, i - 1), toIdx(j, i), 1);
                    }
                }
                grid.ModifyEdgeWeight(0, 1, 100000);

                double[] cityTimes = new double[]
                    {
                    1000, 1000, 1, 1,
                    1, 100, 100, 1,
                    1, 100, 100, 1,
                    1, 1, 1, 1
                    };

                int[] nominated = new int[] { 0, 1 };
                var bestTime = 21;
                var bestTimeBypass = 4;

                bestPair.TestCases.Add(new BestCityPairTestCase(grid, cityTimes, nominated, bestTime, 1 * TIME_MULTIPLIER, description));
                bestPairWithBypass.TestCases.Add(new BestPairWithBypassTestCase(grid, cityTimes, nominated, bestTimeBypass, true, 1 * TIME_MULTIPLIER, description));
            }

            //Test 3
            {
                var description = "Niespójny graf";
                var twoPaths = new AdjacencyListsGraph<AVLAdjacencyList>(false, 10);
                for (int i = 0; i < 4; i++)
                {
                    twoPaths.AddEdge(i, i + 1);
                    twoPaths.AddEdge(5 + i, 5 + i + 1);
                }
                int[] nominated = new int[] { 0, 9 };
                double[] cityTimes = new double[10];

                bestPair.TestCases.Add(new BestCityPairTestCase(twoPaths, cityTimes, nominated, null, 1 * TIME_MULTIPLIER, description));
                bestPairWithBypass.TestCases.Add(new BestPairWithBypassTestCase(twoPaths, cityTimes, nominated, null, false, 1 * TIME_MULTIPLIER, description));
            }

            //Test 4
            {
                var description = "Niespójny graf, ale są dwa miasta w tej samej spójnej składowej";
                var twoPaths = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 10);
                for (int i = 0; i < 4; i++)
                {
                    twoPaths.AddEdge(i, i + 1);
                    twoPaths.AddEdge(5 + i, 5 + i + 1);
                }
                int[] nominated = new int[] { 0, 3, 9 };
                double[] cityTimes = new double[10];

                bestPair.TestCases.Add(new BestCityPairTestCase(twoPaths, cityTimes, nominated, 3, 1 * TIME_MULTIPLIER, description));
                bestPairWithBypass.TestCases.Add(new BestPairWithBypassTestCase(twoPaths, cityTimes, nominated, 3, false, 1 * TIME_MULTIPLIER, description));
            }

            //Test 5
            {
                var description = "Dwa wierzchołki połączone krawędzią";
                var two = new AdjacencyMatrixGraph(false, 2);
                two.AddEdge(0, 1, 5);
                var times = new double[] { 1000, 1001 };
                var bestTime = 5;
                var nominated = new int[] { 0, 1 };
                bestPair.TestCases.Add(new BestCityPairTestCase(two, times, nominated, bestTime, 1 * TIME_MULTIPLIER, description));
                bestPairWithBypass.TestCases.Add(new BestPairWithBypassTestCase(two, times, nominated, bestTime, false, 1 * TIME_MULTIPLIER, description));
            }

            //Test 6
            {
                var description = "Ścieżka z równymi wagami, miasto na środku i dwa po bokach, różne czasy przejazdu przez miasta";
                var path = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 9);
                for (int i = 0; i < 8; i++)
                {
                    path.AddEdge(i, i + 1, 1);
                }
                var times = new double[] { 2, 3, 4, 2, 1, 1, 1, 8, 2 };
                var bestTime = 4 + 9;
                var bestTimeBypass = 4 + 2;
                var nominated = new int[] { 0, 4, 8 };
                bestPair.TestCases.Add(new BestCityPairTestCase(path, times, nominated, bestTime, 1 * TIME_MULTIPLIER, description));
                bestPairWithBypass.TestCases.Add(new BestPairWithBypassTestCase(path, times, nominated, bestTimeBypass, true, 1 * TIME_MULTIPLIER, description));
            }

            //Test 7
            {
                var gridW = 12;
                var gridH = 12;
                var desc = $"Siatka {gridW}×{gridH}, miasta w narożnikach, czas przejazdu przez każde miasto oprócz przekątnej 100000";
                var grid = new AdjacencyListsGraph<HashTableAdjacencyList>(false, gridW * gridH);
                int toCoord(int x, int y) => x * gridH + y;
                for (int x = 0; x < gridW; x++)
                {
                    for (int y = 1; y < gridH; y++)
                    {
                        grid.AddEdge(toCoord(x, y), toCoord(x, y - 1), 3);
                    }
                }
                for (int x = 1; x < gridW; x++)
                {
                    for (int y = 0; y < gridH; y++)
                    {
                        grid.AddEdge(toCoord(x, y), toCoord(x - 1, y), 3);
                    }
                }
                double[] times = new double[gridW * gridH];
                for (int i = 0; i < times.Length; i++)
                {
                    times[i] = 100000;
                }
                var nominated = new int[] { toCoord(0, 0), toCoord(0, gridH - 1), toCoord(gridW - 1, 0), toCoord(gridW - 1, gridH - 1) };

                for (int x = 1; x < gridW; x++)
                {
                    times[toCoord(x, x)] = 1;
                    times[toCoord(x - 1, x)] = 1;
                }
                var bestTime = (gridW - 1) * 3 + (gridH - 1) * 3 + gridW * 2 - 3;

                bestPair.TestCases.Add(new BestCityPairTestCase(grid, times, nominated, bestTime, 1 * TIME_MULTIPLIER, desc));
                bestPairWithBypass.TestCases.Add(new BestPairWithBypassTestCase(grid, times, nominated, bestTime - 1, true, 1 * TIME_MULTIPLIER, desc));
            }

            // Test 8
            {
                var desc = $"Izolowane wierzchołki";
                var rowCount = 5;
                var rowLength = 7;
                var hexes = new AdjacencyListsGraph<SimpleAdjacencyList>(false, rowCount * rowLength);
                var times = new double[]
                    {
                    2, 2, 2, 2, 2, 2, 2,
                    50,50,50,50,50,50,50,
                    1, 3, 1, 1, 1, 4, 1,
                    1, 1, 1, 1, 1, 1, 1,
                    5, 5, 5, 5, 5, 5, 5,
                    };


                var nominated = new int[] { 3, 21, 20 };
                bestPair.TestCases.Add(new BestCityPairTestCase(hexes, times, nominated, null, 1 * TIME_MULTIPLIER, desc));
                bestPairWithBypass.TestCases.Add(new BestPairWithBypassTestCase(hexes, times, nominated, null, false, 1 * TIME_MULTIPLIER, desc));
            }

            // Test 9
            {
                var desc = $"Siatka szcześciokątów, w drugim wierszu bardzo duże koszty przejazdu";
                var rowCount = 5;
                var rowLength = 7;
                var hexes = new AdjacencyListsGraph<SimpleAdjacencyList>(false, rowCount * rowLength);
                for (int r = 0; r < rowCount; r++)
                {
                    for (int i = 0; i < rowLength; i++)
                    {
                        if (r > 0)
                        {
                            hexes.AddEdge((r - 1) * rowLength + i, r * rowLength + i, 1);
                            if (r % 2 == 0 && i > 0) hexes.AddEdge((r - 1) * rowLength + i - 1, r * rowLength + i, 1);
                            if (r % 2 == 1 && i < rowLength - 1) hexes.AddEdge((r - 1) * rowLength + i + 1, r * rowLength + i, 1);
                        }
                        if (i > 0) hexes.AddEdge(r * rowLength + i, r * rowLength + i - 1, 5);
                    }
                }
                var times = new double[]
                    {
                    2, 2, 2, 2, 2, 2, 2,
                    50,50,50,50,50,50,50,
                    1, 3, 1, 1, 1, 4, 1,
                    1, 1, 1, 1, 1, 1, 1,
                    5, 5, 5, 5, 5, 5, 5,
                    };

                var bestTime = 9 + 8 + 4 + 4;
                var nominated = new int[] { 3, 21, 20 };
                bestPair.TestCases.Add(new BestCityPairTestCase(hexes, times, nominated, bestTime, 1 * TIME_MULTIPLIER, desc));
                bestPairWithBypass.TestCases.Add(new BestPairWithBypassTestCase(hexes, times, nominated, 10, true, 1 * TIME_MULTIPLIER, desc));
            }

            //Test 10
            {
                var desc = "Graf pełny";
                var full = new AdjacencyMatrixGraph(false, 50);
                for (int i = 0; i < 50; i++)
                {
                    for (int j = i + 1; j < 50; j++)
                    {
                        full.AddEdge(i, j, i * j);
                    }
                }
                var times = new double[50];
                for (int i = 0; i < 50; i++)
                {
                    times[50 - i - 1] = 1;
                }
                var nominated = new int[] { 4, 7, 12, 16 };
                var bestTime = 1;
                bestPair.TestCases.Add(new BestCityPairTestCase(full, times, nominated, bestTime, 1 * TIME_MULTIPLIER, desc));
                bestPairWithBypass.TestCases.Add(new BestPairWithBypassTestCase(full, times, nominated, 0, true, 1 * TIME_MULTIPLIER, desc));
            }

            //Test 11
            {
                var gridW = 25;
                var gridH = 25;
                var desc = $"Siatka {gridW}×{gridH}, miasta w narożnikach, czas przejazdu przez każde miasto oprócz przekątnej 100000";
                var grid = new AdjacencyListsGraph<HashTableAdjacencyList>(false, gridW * gridH);
                int toCoord(int x, int y) => x * gridH + y;
                for (int x = 0; x < gridW; x++)
                {
                    for (int y = 1; y < gridH; y++)
                    {
                        grid.AddEdge(toCoord(x, y), toCoord(x, y - 1), 3);
                    }
                }
                for (int x = 1; x < gridW; x++)
                {
                    for (int y = 0; y < gridH; y++)
                    {
                        grid.AddEdge(toCoord(x, y), toCoord(x - 1, y), 3);
                    }
                }
                double[] times = new double[gridW * gridH];
                for (int i = 0; i < times.Length; i++)
                {
                    times[i] = 100000;
                }
                var nominated = new int[] { toCoord(0, 0), toCoord(0, gridH - 1), toCoord(gridW - 1, 0), toCoord(gridW - 1, gridH - 1) };

                for (int x = 1; x < gridW; x++)
                {
                    times[toCoord(x, x)] = 1;
                    times[toCoord(x - 1, x)] = 1;
                }
                var bestTime = (gridW - 1) * 3 + (gridH - 1) * 3 + gridW * 2 - 3;

                bestPair.TestCases.Add(new BestCityPairTestCase(grid, times, nominated, bestTime, 2 * TIME_MULTIPLIER, desc));
                bestPairWithBypass.TestCases.Add(new BestPairWithBypassTestCase(grid, times, nominated, bestTime - 1, true, 2 * TIME_MULTIPLIER, desc));
            }

            //Test 12
            {
                var desc = "Graf pełny, z zerowymi czasami do jednego z wierzchołków i dużym kosztem przejścia przez niego";
                var full = new AdjacencyMatrixGraph(false, 1000);
                for (int i = 0; i < 1000; i++)
                {
                    for (int j = i + 1; j < 1000; j++)
                    {
                        full.AddEdge(i, j, i * j);
                    }
                }
                var times = new double[1000];
                for (int i = 0; i < 1000; i++)
                {
                    times[1000 - i - 1] = 2 * i;
                }
                var nominated = new int[] { 4, 7, 12, 16 };
                var bestTime = 4 * 7;

                bestPair.TestCases.Add(new BestCityPairTestCase(full, times, nominated, bestTime, 40 * TIME_MULTIPLIER, desc));
                bestPairWithBypass.TestCases.Add(new BestPairWithBypassTestCase(full, times, nominated, 0, true, 40 * TIME_MULTIPLIER, desc));
            }

            //Test 13 (tylko obwodnice)
            {
                var desc = $"Cykl, Nie opłaca się budować obwodnicy, bo nic nie poprawi";
                var cycle = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 6);
                cycle.AddEdge(0, 1, 11);
                for (int i = 1; i < 6; i++)
                {
                    cycle.AddEdge(i, (i + 1) % 6, 1);
                }
                var distances = new double[] { 2, 2, 2, 2, 2, 2 };
                var nominated = new int[] { 0, 1 };
                var bestTime = 11;
                bestPairWithBypass.TestCases.Add(new BestPairWithBypassTestCase(cycle, distances, nominated, bestTime, false, 1 * TIME_MULTIPLIER, desc));
            }

        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            BestPairTests tests = new BestPairTests();
            tests.PrepareTestSets();
            tests.TestSets["BestPair"].PerformTests(false);
            tests.TestSets["BestPairWithBypass"].PerformTests(false);
        }
    }
}
