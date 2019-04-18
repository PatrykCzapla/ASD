using System;
using ASD.Graphs;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASD
{

    public class FindCycleTestCase : TestCase
    {
        protected Graph g;
        protected Graph input;
        protected int[] result;
        protected bool expectNull;

        public FindCycleTestCase(double timeLimit, Exception expectedException, string description, Graph g, bool expectNull)
            : base(timeLimit, expectedException, description)
        {
            this.expectNull = expectNull;
            this.g = g;
            input = g.Clone();
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            result = ((RoutePlanner)prototypeObject).FindCycle(g);
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            if (!g.IsEqual(input))
                return (Result.WrongResult, "graf wejściowy został zmodyfikowany");

            if (!expectNull && result == null)
                return (Result.WrongResult, "zwrócono null, podczas gdy istnieje rozwiązanie");

            if (expectNull && result == null)
                return (Result.Success, "OK");

            for (int i = 0; i < result.Length; ++i)
            {
                if (result[i] < 0 || result[i] >= g.VerticesCount)
                    return (Result.WrongResult, $"nie ma w grafie wierzchołka o numerze {result[i]}");
                int prev = (i == 0) ? result.Length - 1 : i - 1;

                if (g.GetEdgeWeight(result[prev], result[i]).IsNaN())
                    return (Result.WrongResult, $"krawędź ({result[prev]},{result[i]}) jest w rozwiązaniu, a nie ma jej w grafie");
            }

            StringBuilder sb = new StringBuilder();

            var ru = result.Distinct();
            if (ru.Count() != result.Length)
            {
                sb.Append("w zwróconym rozwiązaniu powtarzają się wierzchołki: (");
                foreach (var v in result)
                {
                    sb.Append($"{v}, ");
                }
                sb.Append(")");
                return (Result.WrongResult, sb.ToString());
            }
            return (Result.Success, "OK");
        }

    }

    public abstract class RoutesTestCase : TestCase
    {
        protected Graph g;
        protected Graph input;
        protected int[][] result;
        protected bool expectNull;

        public RoutesTestCase(double timeLimit, Exception expectedException, string description, Graph g, bool expectNull)
            : base(timeLimit, expectedException, description)
        {
            this.expectNull = expectNull;
            this.g = g;
            input = g.Clone();
        }

        protected (Result resultCode, string message) IsEdgePartition(int[][] routes)
        {
            Graph gc = g.Clone();
            foreach (var route in routes)
            {
                if ( route==null )
                    return (Result.WrongResult, "null route retuned");
                for (int i = 0; i < route.Length; ++i)
                {
                    if (route[i] < 0 || route[i] >= g.VerticesCount)
                        return (Result.WrongResult, $"nie ma w grafie wierzchołka o numerze {route[i]}");
                    int prev = (i == 0) ? route.Length - 1 : i - 1;
                    if (g.GetEdgeWeight(route[prev], route[i]).IsNaN())
                        return (Result.WrongResult, $"krawędź ({route[prev]},{route[i]}) jest w rozwiązaniu, a nie ma jej w grafie");
                    if (gc.GetEdgeWeight(route[prev], route[i]).IsNaN())
                        return (Result.WrongResult, $"krawędź ({route[prev]},{route[i]}) użyta wielokrotnie");
                    gc.DelEdge(route[prev], route[i]);
                }
            }
            if (gc.EdgesCount > 0)
                return (Result.WrongResult, $"nie wszystkie krawędzie są wykorzystane");

            return (Result.Success, "OK");
        }

    }

    public class ShortRoutesTestCase : RoutesTestCase
    {
        public ShortRoutesTestCase(double timeLimit, Exception expectedException, string description, Graph g, bool expectNull) :
            base(timeLimit, expectedException, description, g, expectNull)
        { }

        private (Result resultCode, string message) AreSimple(int[][] routes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var r in routes)
            {
                var ru = r.Distinct();
                if (ru.Count() != r.Length)
                {
                    sb.Append("na trasie powtarzają się wierzchołki: (");
                    foreach (var v in r)
                    {
                        sb.Append($"{v}, ");
                    }
                    sb.Append(")");
                    return (Result.WrongResult, sb.ToString());
                }
            }
            return (Result.Success, "OK");
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            result = ((RoutePlanner)prototypeObject).FindShortRoutes(g);
        }


        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            if (!g.IsEqual(input))
                return (Result.WrongResult, "graf wejściowy został zmodyfikowany");

            if (!expectNull && result == null)
                return (Result.WrongResult, "zwrócono null, podczas gdy istnieje rozwiązanie");

            if (expectNull && result == null)
                return (Result.Success, "OK");

            var r = IsEdgePartition(result);
            if (r.resultCode != Result.Success)
                return r;

            return AreSimple(result);
        }
    }

    public class LongRoutesTestCase : RoutesTestCase
    {
        int optNum;

        public LongRoutesTestCase(double timeLimit, Exception expectedException, string description, Graph g, bool expectNull, int optNum) :
            base(timeLimit, expectedException, description, g, expectNull)
        {
            this.optNum = optNum;
        }


        protected override void PerformTestCase(object prototypeObject)
        {
            result = ((RoutePlanner)prototypeObject).FindLongRoutes(g);
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            if (!g.IsEqual(input))
                return (Result.WrongResult, "graf wejściowy został zmodyfikowany");

            if (!expectNull && result == null)
                return (Result.WrongResult, "zwrócono null, podczas gdy istnieje rozwiązanie");

            if (expectNull && result == null)
                return (Result.Success, "OK");

            var r = IsEdgePartition(result);
            if (r.resultCode != Result.Success)
                return r;

            if (result.Length != optNum)
                return (Result.WrongResult, $"zwrócono {result.Length} tras, a można {optNum}");

            return (Result.Success, "OK");
        }
    }

    class Lab05TestModule : TestModule
    {

        private void PrepareCycleTestSets()
        {
            List<(Graph, bool, string)> cycleTests = new List<(Graph, bool, string)>();
            Graph g;
            bool expectNull;
            string desc;

            //1
            desc = "skierowany cykl";
            g = new AdjacencyMatrixGraph(true, 3);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(2, 0);
            expectNull = false;
            cycleTests.Add((g, expectNull, desc));


            //2
            desc = "dag";
            g = new AdjacencyMatrixGraph(true, 4);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(2, 3);
            g.AddEdge(0, 2);
            g.AddEdge(0, 3);
            g.AddEdge(1, 3);
            expectNull  = true;
            cycleTests.Add((g, expectNull, desc));

            //3
            desc = "cykl + dużo izolowanych wierzchołków";
            g = new AdjacencyListsGraph<AVLAdjacencyList>(true, 14);
            g.AddEdge(10, 11);
            g.AddEdge(11, 12);
            g.AddEdge(12, 10);
            expectNull = false;
            cycleTests.Add((g, expectNull, desc));

            //4
            desc = "dwie nietrywialne składowe, jedna ma cykl a druga nie";
            g = new AdjacencyListsGraph<AVLAdjacencyList>(true, 6);
            g.AddEdge(0, 1);
            g.AddEdge(0, 2);
            g.AddEdge(1, 2);

            g.AddEdge(3, 4);
            g.AddEdge(4, 5);
            g.AddEdge(5, 3);
            expectNull = false;
            cycleTests.Add((g, expectNull, desc));


            //5
            desc = "jest jeden cykl, dwuelementowy";
            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 4);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(2, 3);
            g.AddEdge(0, 2);
            g.AddEdge(0, 3);
            g.AddEdge(1, 3);
            g.AddEdge(3, 1);
            expectNull = false;
            cycleTests.Add((g, expectNull, desc));


            TestSets["CycleTests"] = new TestSet(new RoutePlanner(), "Część 1 - znajdowanie cyklu");

            foreach (var (gr, ce, des) in cycleTests)
            {
                TestSets["CycleTests"].TestCases.Add(new FindCycleTestCase(1, null, des, gr, ce));                
            }

        }

        private void PrepareRouteTestSets()
        {
            
            List<(Graph, bool, int, string)> labTests = new List<(Graph, bool, int, string)>();

            Graph g;
            bool expectNull;           
            int optNum;
            string desc;

            //1
            desc = "skierowany cykl";
            g = new AdjacencyMatrixGraph(true, 3);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);    
            g.AddEdge(2, 0);
            expectNull = false;
            optNum = 1;            
            labTests.Add((g, expectNull, optNum, desc));


            //2
            desc = "dag";
            g = new AdjacencyMatrixGraph(true, 4);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(2, 3);
            g.AddEdge(0, 2);
            g.AddEdge(0, 3);
            g.AddEdge(1, 3);
            expectNull = true;
            optNum = 0;
            labTests.Add((g, expectNull, optNum, desc));

            //3
            desc = "cykl z krawędzią wiszącą";
            g = new AdjacencyMatrixGraph(true, 4);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(2, 0);
            g.AddEdge(1, 3);
            expectNull = true;
            optNum = 0;
            labTests.Add((g, expectNull, optNum, desc));

            //4
            desc = "ósemka";
            g = new AdjacencyListsGraph<AVLAdjacencyList>(true, 5);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(2, 0);
            g.AddEdge(1, 3);
            g.AddEdge(3, 4);
            g.AddEdge(4, 1);
            expectNull = false;
            optNum = 1;
            labTests.Add((g, expectNull, optNum, desc));

            //5
            desc = "ósemka + wierzchołki izolowane";
            g = new AdjacencyListsGraph<AVLAdjacencyList>(true, 10);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(2, 0);
            g.AddEdge(1, 3);
            g.AddEdge(3, 4);
            g.AddEdge(4, 1);
            expectNull = false;
            optNum = 1;
            labTests.Add((g, expectNull, optNum, desc));

            //6
            desc = "dwa rozłączne cykle";
            g = new AdjacencyListsGraph<AVLAdjacencyList>(true, 10);
            for (int i = 0; i < 4; ++i)
            {
                g.AddEdge(i, i + 1);
                g.AddEdge(5 + i, 5 + i + 1);
            }
            g.AddEdge(4, 0);
            g.AddEdge(9, 5);
            expectNull = false;
            optNum = 2;
            labTests.Add((g, expectNull, optNum, desc));

            //7
            desc = "bardziej skomplikowany graf z jedną trasą";
            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 7);
            g.AddEdge(0, 2);
            g.AddEdge(2, 3);
            g.AddEdge(3, 5);
            g.AddEdge(5, 6);
            g.AddEdge(6, 1);
            g.AddEdge(1, 2);
            g.AddEdge(1, 3);
            g.AddEdge(3, 4);
            g.AddEdge(4, 1);
            g.AddEdge(2, 6);
            g.AddEdge(6, 0);
            expectNull = false;
            optNum = 1;
            labTests.Add((g, expectNull, optNum, desc));

            //8
            desc = "bardziej skomplikowany graf bez rozwiązania";
            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 7);
            g.AddEdge(0, 2);
            g.AddEdge(2, 3);
            g.AddEdge(3, 5);
            g.AddEdge(5, 6);
            g.AddEdge(6, 1);
            g.AddEdge(1, 2);
            g.AddEdge(1, 3);
            g.AddEdge(3, 4);
            g.AddEdge(4, 1);
            g.AddEdge(2, 6);
            g.AddEdge(6, 0);
            g.AddEdge(6, 2);
            expectNull = true;
            optNum = 0;
            labTests.Add((g, expectNull, optNum, desc));

            //9
            desc = "graf pełny, krawędzie tam i z powrotem";
            g = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 7);
            for (int i = 0; i < 7; ++i)
                for (int j = i + 1; j < 7; ++j)
                {
                    g.AddEdge(i, j);
                    g.AddEdge(j, i);
                }
            expectNull = false;
            optNum = 1;
            labTests.Add((g, expectNull, optNum, desc));


            //10
            desc = "graf z kilku nietrywialnych składowych";
            g = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 20);

            g.AddEdge(2, 3);
            g.AddEdge(3, 4);
            g.AddEdge(4, 1);
            g.AddEdge(1, 2);
            g.AddEdge(0, 4);
            g.AddEdge(4, 0);

            g.AddEdge(5, 6);
            g.AddEdge(6, 7);
            g.AddEdge(7, 5);
            g.AddEdge(5, 9);
            g.AddEdge(9, 8);
            g.AddEdge(8, 5);

            g.AddEdge(10, 11);
            g.AddEdge(11, 12);
            g.AddEdge(12, 13);
            g.AddEdge(13, 10);
            g.AddEdge(10, 12);
            g.AddEdge(12, 14);
            g.AddEdge(14, 10);

            g.AddEdge(15, 16);
            g.AddEdge(16, 17);
            g.AddEdge(17, 18);
            g.AddEdge(18, 15);
            g.AddEdge(19, 15);
            g.AddEdge(15, 18);
            g.AddEdge(18, 17);
            g.AddEdge(17, 16);
            g.AddEdge(16, 19);

            expectNull = false;
            optNum = 4;
            labTests.Add((g, expectNull, optNum, desc));


            TestSets["LabShortRoutes"] = new TestSet(new RoutePlanner(), "Część 2 - dużo krótkich tras");
            TestSets["LabLongRoutes"] = new TestSet(new RoutePlanner(), "Część 3 - mało długich tras");

            foreach (var (gr, en, on, des) in labTests)
            {
                TestSets["LabShortRoutes"].TestCases.Add(new ShortRoutesTestCase(1, null, des, gr, en));
                TestSets["LabLongRoutes"].TestCases.Add(new LongRoutesTestCase(1, null, des, gr, en, on));
            }

        }

        public override void PrepareTestSets()
        {
            PrepareCycleTestSets();
            PrepareRouteTestSets();
        }

        public override double ScoreResult()
        {
            return 1;
        }

    }

    class Lab05Main
    {

        public static void Main()
        {
            Lab05TestModule tests = new Lab05TestModule();
            tests.PrepareTestSets();
            foreach (var ts in tests.TestSets)
                ts.Value.PerformTests(false);
        }
    }
}
