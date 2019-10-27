using System;
using System.Linq;
using System.Collections.Generic;
using ASD;
using ASD.Graphs;

namespace Lab13
{
enum VerifyType
    {
    Basic, StartArray, CriticalPath, All
    }

public abstract class ProjectDurationTestBase : TestCase
    {
        protected readonly Graph g;
        protected readonly double[] durations;
        protected readonly double expected;
        private double length = -1;
        protected double[] startTimes=null;
        private int[] criticalPath=null;
        public ProjectDurationTestBase(Graph g, double[] durations, double expected,
                                       double timeLimit, string description) : base(timeLimit, null, description)
            {
            this.durations = durations;
            this.g = g;
            this.expected = expected;
            }

        protected override void PerformTestCase(object prototypeObject)
            {
            var sol = (ProgramPlanning)prototypeObject;
            length = sol.CalculateTimesLatestPossible(g, durations, out startTimes, out criticalPath);
            }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
            {
            var verifyType = (VerifyType)settings;
            return (length == expected) ?
                   CheckRest(verifyType) :
                   (Result.WrongResult, $"Wyliczona długość działania: {length}, oczekiwana: {expected}");
            }

        private static (Result, string) FirstFailure(IEnumerable<Func<(Result, string)>> checkers)
            {
            foreach(var checker in checkers)
                {
                var res = checker();
                if(res.Item1 != Result.Success) return res;
                }
            return (Result.Success, "OK");
            }

        private (Result, string) CheckCP()
            {
            if(criticalPath == null)
                return (Result.WrongResult, "Zwrócono null zamiast ścieżki krytycznej");
            if(criticalPath.Length == 0)
                return (Result.WrongResult, "Zwrócono ścieżkę krytyczna o długosci 0");
            if(criticalPath.Any(x => x<0 || x >= g.VerticesCount))
                return (Result.WrongResult, "Zwrócona ścieżka krytyczna zawiera wierzchołki spoza grafu");

            var sum = durations[criticalPath[0]];
            var ok1 = g.InDegree(criticalPath[0]) == 0 && g.OutDegree(criticalPath.Last()) == 0;
            var ok = ok1;
            for(int i=1; i<criticalPath.Length&&ok; i++)
                {
                sum+=durations[criticalPath[i]];
                ok = ok && !double.IsNaN(g.GetEdgeWeight(criticalPath[i-1], criticalPath[i]));
                }

            if(!ok1)
                return (Result.WrongResult, "Zwrócony ciąg wierzchołków nie zaczyna się na procedurze bez poprzedników lub nie kończy na procedurze bez następników");
            if(!ok)
                return (Result.WrongResult, "Zwrócony ciąg wierzchołków zawiera niesąsiednie wierzchołki");
            return sum != length ?
                (Result.WrongResult, $"Zwrócono ścieżkę, która nie jest krytyczna: {Stringify(criticalPath)}") :
                (Result.Success, "OK") ;
            }


        private (Result, string) CheckRest(VerifyType vt)
            {
            switch(vt)
                {
                case VerifyType.Basic:
//                    return (Result.Success, $"OK, {PerformanceTime:#0.00}");
                    return (Result.Success, "OK");
                case VerifyType.StartArray:
                    return CheckStartTimes();
                case VerifyType.CriticalPath:
                    return CheckCP();
                case VerifyType.All:
                    return FirstFailure(new Func<(Result, string)>[] {CheckCP, CheckStartTimes});
                default:
                    throw new Exception("Welcome to C#");
                }

            }

        protected abstract (Result resultCode, string message) CheckStartTimes();
        public static string Stringify<T>(IEnumerable<T> arr)
            {
            return arr.Aggregate("", (a, b) => a+", "+b);
            }
    }

class ProjectDurationTest : ProjectDurationTestBase
    {
        private readonly double[] startTimesExpected;

        public ProjectDurationTest(Graph g, double[] durations, double expected, double[] startTimesExpected,
                                   double timeLimit, string description) : base(g, durations, expected, timeLimit, description)
            {
            this.startTimesExpected = startTimesExpected;
            }

        protected override (Result resultCode, string message) CheckStartTimes()
            {
            if (startTimes == null)
                return (Result.WrongResult, "tablica startów jest nullem");
            if(startTimes.Length != startTimesExpected.Length)
                return (Result.WrongResult, $"błędna długość tablicy startów, powinna być {startTimesExpected.Length}, jest {startTimes.Length}");
            return startTimes.Zip(startTimesExpected, (a,b) => a == b).Contains(false) ?
                (Result.WrongResult, $"złe czasy startów. Oczekiwane: {Stringify(startTimesExpected)}, wyliczone: {Stringify(startTimes)}") :
                (Result.Success, "OK") ;
            }

    }

public class ProjectDurationTests : TestModule
    {
        protected TestSet part1 = new TestSet(new ProgramPlanning(), "Część 1: wyliczenie długości projektu (2 punkty)", null, VerifyType.Basic);
        protected TestSet part2 = new TestSet(new ProgramPlanning(), "Część 2: wyliczenie startów zadań (1 punkt)", null, VerifyType.StartArray);
        protected TestSet part3 = new TestSet(new ProgramPlanning(), "Cześć 3: wyliczenie śceiżki krytycznej (1 punkt)", null, VerifyType.CriticalPath);

        public void PrepareTestSetsClass()
            {
            void AddAll(ProjectDurationTest t)
                {
                part1.TestCases.Add(t);
                part2.TestCases.Add(t);
                part3.TestCases.Add(t);
                }

                {
                var desc = "Prosty test";
                var isolated = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 3) { new Edge(0,1), new Edge(0,2) };
                var tt = new ProjectDurationTest(isolated, new double[] {1,2,3}, 4, new double[] {0,2,1}, 1, desc);
                AddAll(tt);
                }

                {
                var desc = "Wszystkie zadania równolegle";
                var isolated = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 10);
                var tt = new ProjectDurationTest(isolated, new double[] {2,4,3, 7,4,6, 3,2,7, 4}, 7, new double[] {5,3,4,0,3,1,4,5,0,3}, 1, desc);
                AddAll(tt);
                }

                {
                var desc = "Zadania sekwencyjnie";
                var path = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 10);
                for(int i=0; i<9; i++)
                    {
                    path.Add(new Edge(i, i+1));
                    }
                AddAll(new ProjectDurationTest(path, new double[] {2,4,3, 7,4,6, 3,2,7, 4}, 42,
                                               new double[] {0,2,6,9,16,20,26,29,31,38}, 1,  desc));
                }
                {
                var desc = "Zadania sekwencyjnie, losowa kolejnośc wierzchołków w ścieżce";
                var path = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 10);
                path.AddEdge(0, 5);
                path.AddEdge(5,8);
                path.AddEdge(8,1);
                path.AddEdge(1,2);
                path.AddEdge(2,9);
                path.AddEdge(9, 4);
                path.AddEdge(4,7);
                path.AddEdge(7,3);
                path.AddEdge(3, 6);
                AddAll(new ProjectDurationTest(path, new double[] {2,4,3, 7,4,6, 3,2,7, 4}, 42,
                                               new double[] {0,15,19,32,26,2,39,30,8,22},
                                               1, desc));
                }
                {
                var desc = "Dwie zupełnie niezależnie ścieżki";
                var paths = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 10);
                paths.AddEdge(0,4);
                paths.AddEdge(4,2);
                paths.AddEdge(2,1);
                paths.AddEdge(1,3);
                paths.AddEdge(6,5);
                paths.AddEdge(5,9);
                paths.AddEdge(9,7);
                paths.AddEdge(7,8);
                AddAll(new ProjectDurationTest(paths, new double[] {2,4,3,7,4, 6,8,4,9,2}, 29,
                                               new double[] {9,18,15,22,11,8,0,16,20,14}, 1, desc));

                }
                {
                var desc = "Pełne drzewo binarne";
                var tree = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 15);
                for(int i=1; i<15; i++)
                    {
                    tree.AddEdge(i, ((i+1)/2)-1);
                    }
                AddAll(new ProjectDurationTest(tree, new double[] {4,2,1,5, 3,2,8,3, 1,3,5,3, 2,7,8}, 21,
                                               new double[] {17,15,16,10,12,14,8,7,9,9,7,11, 12, 1,0},
                                               1, desc));
                }
                {
                var desc = "Pełne drzewo binarne odwrócone";
                var tree = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 15);
                for(int i=1; i<15; i++)
                    {
                    tree.AddEdge(((i+1)/2)-1, i);
                    }
                AddAll(new ProjectDurationTest(tree, new double[] {4,2,1,5, 3,2,8,3, 1,3,5,3, 2,7,8}, 21,
                                               new double[] {0,11,4,13,13,16,5,18,20,18,16,18,19,14,13},
                                               1, desc));
                }
                {
                var desc = "Siatka dwóch poprzedników każdego zadania";
                var grid = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 20);
                for(int row=1; row<4; row++)
                    {
                    for(int col=0; col<5; col++)
                        {
                        if(col > 0)
                            grid.AddEdge((row-1)*5+col-1, row*5+col);
                        grid.AddEdge((row-1)*5+col, row*5+col);
                        }
                    }
                AddAll(new ProjectDurationTest(grid, new double[] {2,1,2,1,2, 2,3,2,3,2, 1,1,1,1,10, 2,2,2,2,2},
                                               17, new double[] {9, 10, 0, 1, 1, 12, 11, 12, 2, 3, 14, 14, 14, 14, 5, 15, 15, 15, 15, 15},
                                               1, desc));
                }
                {
                var desc = "Trzy ścieżki o wspólnym poczatku i końcu";
                var threepath = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 14);

                // 4,8,10
                threepath.AddEdge(4, 8);
                threepath.AddEdge(8, 10);
                // 4,13,5,2,10
                threepath.AddEdge(4, 13);
                threepath.AddEdge(13, 5);
                threepath.AddEdge(5, 2);
                threepath.AddEdge(2, 10);

                // 4,7,1,12,6,9,11,0,3,10
                threepath.AddEdge(4,7);
                threepath.AddEdge(7,1);
                threepath.AddEdge(1,12);
                threepath.AddEdge(12,6);
                threepath.AddEdge(6,9);
                threepath.AddEdge(9,11);
                threepath.AddEdge(11,0);
                threepath.AddEdge(0,3);
                threepath.AddEdge(3,10);

                AddAll(new ProjectDurationTest(threepath, new double[] {1,1,4,2,2,3,1,1,12,2,2,2,2,5}, 16,
                                               new double[] {11, 3, 10, 12, 0, 7, 6, 2, 2, 7, 14, 9, 4, 2}, 1, desc));
                }

                {
                var desc = "Trzy ścieżki o wspólnym poczatku i końcu z odnogami na końcu";
                var threepath = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 16);

                // 4,8,10
                threepath.AddEdge(15,8);
                threepath.AddEdge(4, 8);
                threepath.AddEdge(8, 10);
                threepath.AddEdge(8,14);
                // 4,13,5,2,10
                threepath.AddEdge(15,13);
                threepath.AddEdge(4, 13);
                threepath.AddEdge(13, 5);
                threepath.AddEdge(5, 2);
                threepath.AddEdge(2, 10);
                threepath.AddEdge(2,14);

                // 4,7,1,12,6,9,11,0,3,10
                threepath.AddEdge(15,7);
                threepath.AddEdge(4,7);
                threepath.AddEdge(7,1);
                threepath.AddEdge(1,12);
                threepath.AddEdge(12,6);
                threepath.AddEdge(6,9);
                threepath.AddEdge(9,11);
                threepath.AddEdge(11,0);
                threepath.AddEdge(0,3);
                threepath.AddEdge(3,10);
                threepath.AddEdge(3,14);

                AddAll(new ProjectDurationTest(threepath, new double[] {1,1,4,2,2,3,1,1,12,2,2,2,2,5,2,2}, 16,
                                               new double[] {11, 3, 10, 12, 0, 7, 6, 2, 2, 7, 14, 9, 4, 2,14, 0}, 1, desc));
                }

            TestSets["LabOnlyDuration"] = part1;
            TestSets["LabStartTimes"] = part2;
            TestSets["LabTaskToOpt"] = part3;
            }


        public override void PrepareTestSets()
            {
            PrepareTestSetsClass();
            }
    }

class MainClass
    {
        public static void Main(string[] args)
            {
            ProjectDurationTests tests = new ProjectDurationTests();
            tests.PrepareTestSets();
            foreach (var ts in tests.TestSets)
                ts.Value.PerformTests(false);
            }
    }
}
