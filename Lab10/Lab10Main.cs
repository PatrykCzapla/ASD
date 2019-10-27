using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class CreatePlanTestCase : TestCase
    {
        protected (int from, int to, int cost, int limit)[] possibleChangesBeforeBorder;
        protected (int from, int to, int cost, int limit)[] possibleChangesAfterBorder;
        protected int substancesNumber;
        protected double[] sellPrices;
        protected int lemonadeAmount;
        protected double expectedResult;

        protected double result;
        protected List<(int from, int to, int amount)> changesBeforeBorder;
        protected List<(int from, int to, int amount)> changesAfterBorder;

        void CheckFlow(double result, out string message)
        {
            message = "";
            double cost = 0.0;

            int[] flowDifferencesBeforeBorder = new int[substancesNumber];

            if (changesBeforeBorder.Any(x => x.from < 0 || x.from >= substancesNumber || x.to < 0 || x.to >= substancesNumber))
            {
                message = "Nieprawidłowy identyfikator substancji";
                return;
            }
            if (changesBeforeBorder.Any(x => x.amount <= 0))
            {
                message = "Niedodatnia liczba zamian substancji";
                return;
            }

            for (int i = 0; i < substancesNumber; i++)
            {
                int inFlow = changesBeforeBorder.Where(x => x.to == i).Sum(x => x.amount);
                int outFlow = changesBeforeBorder.Where(x => x.from == i).Sum(x => x.amount);
                flowDifferencesBeforeBorder[i] = inFlow - outFlow;
            }

            for (int from = 0; from < substancesNumber; from++)
                for (int to = 0; to < substancesNumber; to++)
                {
                    int flow = changesBeforeBorder.Where(x => x.from == from && x.to == to).Sum(x => x.amount);
                    if (flow > 0)
                    {
                        if (!possibleChangesBeforeBorder.Any(x => x.from == from && x.to == to))
                        {
                            message = "Brak możliwości zamiany substancji " + from + " w " + to;
                            return;
                        }
                        else if (possibleChangesBeforeBorder.First(x => x.from == from && x.to == to).limit < flow)
                        {
                            message = "Przekroczony limit zamian substancji " + from + " w " + to;
                            return;
                        }
                        else
                            cost += possibleChangesBeforeBorder.First(x => x.from == from && x.to == to).cost * flow;
                    }
                }

            if (-flowDifferencesBeforeBorder[0] > lemonadeAmount)
            {
                message = "Nie ma tyle lemoniady do zamiany";
                return;
            }

            cost += 0.5 * sellPrices[0] * (lemonadeAmount + flowDifferencesBeforeBorder[0]);

            for (int i = 1; i < substancesNumber; i++)
            {
                if (flowDifferencesBeforeBorder[i] < 0)
                {
                    message = "Większa liczba zmianianych substancji niż dostępnych";
                    return;
                }
                if (flowDifferencesBeforeBorder[i] > 0)
                    cost += 0.5 * (sellPrices[i] * flowDifferencesBeforeBorder[i]);
            }


            int[] flowDifferencesAfterBorder = new int[substancesNumber];
            for (int i = 0; i < substancesNumber; i++)
            {
                int inFlow = changesAfterBorder.Where(x => x.to == i).Sum(x => x.amount);
                int outFlow = changesAfterBorder.Where(x => x.from == i).Sum(x => x.amount);
                flowDifferencesAfterBorder[i] = inFlow - outFlow;
            }

            for (int i = 0; i < substancesNumber; i++)
                if (flowDifferencesAfterBorder[i] != -flowDifferencesBeforeBorder[i])
                {
                    message = "W wyniku operacji zostają napoje inne niż lemoniada";
                    return;
                }

            for (int from = 0; from < substancesNumber; from++)
                for (int to = 0; to < substancesNumber; to++)
                {
                    int flow = changesAfterBorder.Where(x => x.from == from && x.to == to).Sum(x => x.amount);
                    if (flow > 0)
                    {
                        if (!possibleChangesAfterBorder.Any(x => x.from == from && x.to == to))
                        {
                            message = "Brak możliwości zamiany substancji " + from + " w " + to;
                            return;
                        }
                        else if (possibleChangesAfterBorder.First(x => x.from == from && x.to == to).limit < flow)
                        {
                            message = "Przekroczony limit zamian substancji " + from + " w " + to;
                            return;
                        }
                        else
                            cost += possibleChangesAfterBorder.First(x => x.from == from && x.to == to).cost * flow;
                    }
                }

            if (result != sellPrices[0] * lemonadeAmount - cost)
            {
                message = "Dochód obliczony zgodnie z operacjami jest inny niż zwrócony";
                return;
            }
        }

        public CreatePlanTestCase(double timeLimit, Exception expectedException, string description,
            (int from, int to, int cost, int limit)[] possibleChangesBeforeBorder, (int from, int to, int cost, int limit)[] possibleChangesAfterBorder,
            int substancesNumber, double[] sellPrices, int lemonadeAmount, double expectedResult)
            : base(timeLimit, expectedException, description)
        {
            this.possibleChangesBeforeBorder = possibleChangesBeforeBorder.Clone() as (int from, int to, int cost, int limit)[];
            this.possibleChangesAfterBorder = possibleChangesAfterBorder.Clone() as (int from, int to, int cost, int limit)[];
            this.substancesNumber = substancesNumber;
            this.sellPrices = sellPrices.Clone() as double[];
            this.lemonadeAmount = lemonadeAmount;
            this.expectedResult = expectedResult;
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            result = ((DutyOptimizer)prototypeObject).CreateSimplePlan(possibleChangesBeforeBorder, possibleChangesAfterBorder, substancesNumber, sellPrices, lemonadeAmount, out changesBeforeBorder, out changesAfterBorder);
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            if (Math.Abs(expectedResult - result) > 0.0001)
                return (Result.WrongResult, "zwrócono: " + result + " prawidłowy wynik: " + expectedResult);

            if (!(bool)settings)
                return (Result.Success, $"OK, {PerformanceTime:#0.00}");

            if (changesBeforeBorder == null || changesAfterBorder == null)
                return (Result.WrongResult, "Wynik OK, brak planu realizacji");

            string message;
            CheckFlow(result, out message);

            if (message != "")
                return (Result.WrongResult, "Wynik OK, " + message);

            return (Result.Success, $"OK, {PerformanceTime:#0.00}");

        }
    }


    class Lab10TestModule : TestModule
    {
        Random rand = new Random(7);

        (int from, int to, int cost, int limit)[] CreateRandomConnections(int n, bool reversed)
        {
            List<(int from, int to, int cost, int limit)> result = new List<(int from, int to, int cost, int limit)>();

            for (int i = 0; i < n - 1; i++)
            {
                int from = i;
                int left = (n - i) - 1;
                while (left > 0)
                {
                    int to = rand.Next(i + 1, n);
                    if (!result.Any(x => x.from == from && x.to == to))
                    {
                        if (!reversed)
                            result.Add((from, to, rand.Next(10), rand.Next(100)));
                        else
                        {
                            var c = (to, from, rand.Next(10), rand.Next(100));
                            if (!result.Any(x => x.from == to && x.to == from))
                                result.Add(c);
                        }
                        left--;
                    }

                }

            }

            return result.ToArray();
        }

        public override void PrepareTestSets()
        {
            PrepareLabTestSets();
        }

        private void PrepareLabTestSets()
        {
            TestSets["LabPlanTests1"] = new TestSet(new DutyOptimizer(), "Testy z lab - czesc 1", null, false);
            TestSets["LabPlanTests2"] = new TestSet(new DutyOptimizer(), "Testy z lab - czesc 2", null, true);

            //mimo że pozostałe substancje są tanie, to nie opłaca się nic zamieniać, bo koszt zamian zbyt wysoki
            TestSets["LabPlanTests1"].TestCases.Add(new CreatePlanTestCase(1, null, "nieopłacalne zamiany",
                new (int from, int to, int cost, int limit)[] { (0, 1, 50, 10), (0, 2, 50, 10), (0, 3, 50, 10), (0, 4, 50, 10) },
                new (int from, int to, int cost, int limit)[] { (1, 0, 50, 10), (2, 0, 50, 10), (3, 0, 50, 10), (4, 0, 50, 10) }, 5, new double[] { 100, 1, 1, 1, 1 }, 10, 500));

            //zerowy koszt wszystkich zamian, więc najlepiej przejść przez granicę z najtańszą substancją
            TestSets["LabPlanTests1"].TestCases.Add(new CreatePlanTestCase(1, null, "nieopłacalne zamiany",
                new (int from, int to, int cost, int limit)[] { (0, 1, 0, 10), (1, 2, 0, 10), (2, 3, 0, 10), (3, 4, 0, 10) },
                new (int from, int to, int cost, int limit)[] { (3, 1, 0, 10), (1, 4, 0, 10), (4, 2, 0, 10), (2, 0, 0, 10) }, 5, new double[] { 100, 40, 20, 10, 30 }, 10, 950));

            //zerowy koszt wszystkich zamian, więc najlepiej przejść przez granicę z najtańszą substancją, ale ograniczenia zamian na ścieżce do niej nie pozwalają zamienić wszystkich butelek
            TestSets["LabPlanTests1"].TestCases.Add(new CreatePlanTestCase(1, null, "zerowy koszt zamian",
                new (int from, int to, int cost, int limit)[] { (0, 1, 0, 10), (1, 2, 0, 5), (2, 3, 0, 5), (3, 4, 0, 10) },
                new (int from, int to, int cost, int limit)[] { (3, 1, 0, 10), (1, 4, 0, 10), (4, 2, 0, 5), (2, 0, 0, 10) }, 5, new double[] { 100, 40, 20, 10, 30 }, 10, 850));

            //zerowy koszt wszystkich zamian, przez granicę przewozimy każdej substancji po 2 sztuki
            TestSets["LabPlanTests1"].TestCases.Add(new CreatePlanTestCase(1, null, "zerowy koszt zamian",
                new (int from, int to, int cost, int limit)[] { (0, 1, 0, 8), (1, 2, 0, 6), (2, 3, 0, 4), (3, 4, 0, 2) },
                new (int from, int to, int cost, int limit)[] { (4, 3, 0, 2), (3, 2, 0, 4), (2, 1, 0, 6), (1, 0, 0, 8) }, 5, new double[] { 10, 8, 6, 4, 2 }, 10, 70));

            //brak możliwych jakichkolwiek zamian substancji
            TestSets["LabPlanTests1"].TestCases.Add(new CreatePlanTestCase(1, null, "brak możliwych zamian",
                new (int from, int to, int cost, int limit)[] { },
                new (int from, int to, int cost, int limit)[] { }, 10, new double[] { 1000, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 100, 50000));

            //brak możliwych jakichkolwiek zamian za granicą
            TestSets["LabPlanTests1"].TestCases.Add(new CreatePlanTestCase(1, null, "brak zamian za granicą",
                new (int from, int to, int cost, int limit)[] { (0, 1, 0, 100), (0, 2, 0, 100), (1, 2, 0, 100) },
                new (int from, int to, int cost, int limit)[] { }, 3, new double[] { 1000, 1, 1 }, 100, 50000));

            //substancja z zerową wartością
            TestSets["LabPlanTests1"].TestCases.Add(new CreatePlanTestCase(1, null, "substancja z zerową wartością",
                new (int from, int to, int cost, int limit)[] { (0, 1, 10, 100), (1, 2, 10, 100), (1, 3, 10, 100), (2, 3, 10, 100) },
                new (int from, int to, int cost, int limit)[] { (1, 0, 10, 100), (2, 1, 10, 100), (3, 1, 10, 100), (3, 2, 10, 100) }, 4, new double[] { 100, 50, 50, 0 }, 100, 6000));

            //dłuższe ścieżki zamian bardziej opłacalne niż krótsze
            TestSets["LabPlanTests1"].TestCases.Add(new CreatePlanTestCase(1, null, "dłuższe ścieżki zamian bardziej opłacalne niż krótsze",
                new (int from, int to, int cost, int limit)[] { (0, 1, 100, 1000), (0, 2, 1, 1000), (2, 3, 1, 1000), (3, 4, 1, 1000), (4, 1, 1, 1000) },
                new (int from, int to, int cost, int limit)[] { (1, 0, 100, 1000), (1, 3, 1, 1000), (3, 2, 1, 1000), (2, 0, 1, 1000) }, 5, new double[] { 100, 1, 80, 80, 80 }, 10, 925));

            //wykorzystanie różnych ścieżek do zamiany w tą samą substancję
            TestSets["LabPlanTests1"].TestCases.Add(new CreatePlanTestCase(1, null, "wykorzystanie wielu ścieżek zamiany",
                new (int from, int to, int cost, int limit)[] { (0, 1, 1, 33), (0, 2, 1, 33), (0, 3, 1, 33), (3, 4, 1, 33), (2, 4, 1, 33), (1, 4, 1, 33), (0, 4, 100, 100) },
                new (int from, int to, int cost, int limit)[] { (4, 0, 0, 100) }, 5, new double[] { 10, 20, 20, 20, 5 }, 100, 549.5));

            //przypadek szczególny: brak lemoniady
            TestSets["LabPlanTests1"].TestCases.Add(new CreatePlanTestCase(1, null, "brak lemoniady",
                new (int from, int to, int cost, int limit)[] { (0, 1, 1, 33), (0, 2, 1, 33), (0, 3, 1, 33), (3, 4, 1, 33), (2, 4, 1, 33), (1, 4, 1, 33), (0, 4, 100, 100) },
                new (int from, int to, int cost, int limit)[] { (4, 0, 0, 100) }, 5, new double[] { 10, 20, 20, 20, 5 }, 0, 0));

            //losowy mały test
            TestSets["LabPlanTests1"].TestCases.Add(new CreatePlanTestCase(1, null, "losowy mały test",
                CreateRandomConnections(10, false),
                CreateRandomConnections(10, true), 10, Enumerable.Range(0, 10).Select(x => x == 0 ? 100 : (double)rand.Next(100)).ToArray(), 100, 8705));

            //losowy średni test
            TestSets["LabPlanTests1"].TestCases.Add(new CreatePlanTestCase(15, null, "losowy średni test",
                CreateRandomConnections(25, false),
                CreateRandomConnections(25, true), 25, Enumerable.Range(0, 25).Select(x => x == 0 ? 100 : (double)rand.Next(100)).ToArray(), 1000, 70558.5));

            //losowy duży test
            TestSets["LabPlanTests1"].TestCases.Add(new CreatePlanTestCase(30, null, "losowy duży test",
                CreateRandomConnections(50, false),
                CreateRandomConnections(50, true), 50, Enumerable.Range(0, 50).Select(x => x == 0 ? 100 : (double)rand.Next(100)).ToArray(), 10000, 548784));

            TestSets["LabPlanTests2"].TestCases.AddRange(TestSets["LabPlanTests1"].TestCases);
        }

        public override double ScoreResult()
        {
            return 1;
        }

    }

    public class Lab11Main
    {
        public static void Main()
        {
            var tests = new Lab10TestModule();
            tests.PrepareTestSets();
            tests.TestSets["LabPlanTests1"].PerformTests(false);
            tests.TestSets["LabPlanTests2"].PerformTests(false);
        }
    }
}