using System;
using System.Collections.Generic;

namespace ASD
{
    class Lab04Main
    {
        public class MaxTaxTestCase : TestCase
        {
            protected int expectedResult;
            protected int result;
            protected TaxAction[] plan;
            protected int[] money;
            protected int[] dist;
            protected int[] carrots;
            protected int maxCarrots;
            protected int startingCarrots;

            public MaxTaxTestCase(double timeLimit, string description, int[] money, int[] carrots, int[] dist, int maxCarrots,
                int startingCarrots, int expectedResult)
    : base(timeLimit, null, description)
            {
                this.money = money;
                this.dist = dist;
                this.carrots = carrots;
                this.startingCarrots = startingCarrots;
                this.maxCarrots = maxCarrots;
                this.expectedResult = expectedResult;
            }


            private (Result, string) VerifyPlan()
            {
                int n = plan.Length;
                int taxCollected = 0;
                int currentCarrots = startingCarrots;

                if (n != dist.Length)
                    return (Result.WrongResult, $"plan with incorrect length, returned {plan.Length}, expected {dist.Length}");

                for (int i = 0; i < n; ++i)
                {
                    if (plan[i] == TaxAction.Empty)
                    {
                        return (Result.WrongResult, $"empty action at position {i}, but you must reach the final point");
                    }


                    currentCarrots -= dist[i];

                    if (currentCarrots < 0)
                        return (Result.WrongResult, $"no energy to reach point {i}");

                    if (plan[i] == TaxAction.TakeMoney)
                        taxCollected += money[i];
                    else
                        currentCarrots += carrots[i];
                    if (currentCarrots > maxCarrots) currentCarrots = maxCarrots;
                }
                if (currentCarrots < startingCarrots)
                    return (Result.WrongResult, $"final amount of carrots is {currentCarrots} < {startingCarrots} (startingCarrots)");

                if (result != taxCollected)
                    return (Result.WrongResult, $"wrong plan, collected {taxCollected} but declared {result}");

                return (Result.Success, "OK");
            }

            protected override (Result resultCode, string message) VerifyTestCase(object settings)
            {
                if (result != expectedResult)
                    return (Result.WrongResult, $"wrong result, returned {result} and expected {expectedResult}");
                if (!(bool)settings)
                    return (Result.Success, "OK");


                if (result != -1 && plan == null)
                    return (Result.WrongResult, $"wrong plan, null returned");
                if (result == -1 && plan == null)
                    return (Result.Success, "OK");

                return VerifyPlan();

            }

            protected override void PerformTestCase(object prototypeObject)
            {
                result = ((TaxCollectorManager)prototypeObject).CollectMaxTax(dist, money, carrots, maxCarrots, startingCarrots, out plan);
            }
        }



        public class MaxTaxTester : TestModule
        {
            public override void PrepareTestSets()
            {
                List<(int, string, int[], int[], int[], int, int, int)> findOptTests = new List<(int, string, int[], int[], int[], int, int, int)>();
                List<(int, string, int[], int[], int[], int, int, int)> findSeqTests = new List<(int, string, int[], int[], int[], int, int, int)>();

                int time = 1;
                string description;
                int[] money;
                int[] carrots;
                int[] dist;
                int maxCarrots;
                int startingCarrots;
                int expectedResult;
                Random rnd;

                //1
                description = "prosty test trzyelementowy";
                money = new int[] { 1, 2, 3 };
                carrots = new int[] { 1, 2, 3 };
                dist = new int[] { 0, 1, 1 };
                startingCarrots = 1;
                maxCarrots = 5;
                expectedResult = 4;
                findOptTests.Add((time, description, money, carrots, dist, maxCarrots, startingCarrots, expectedResult));

                //2
                description = "cały czas trzeba brać marchewki";
                money = new int[] { 100, 100, 100, 100, 0 };
                carrots = new int[] { 10, 10, 10, 10, 10 };
                dist = new int[] { 0, 1, 1, 1, 37 };
                startingCarrots = 0;
                maxCarrots = 50;
                expectedResult = 0;
                findOptTests.Add((time, description, money, carrots, dist, maxCarrots, startingCarrots, expectedResult));

                //3
                description = "nie da się dojechać";
                money = new int[] { 100, 100, 100, 100, 0 };
                carrots = new int[] { 1, 1, 1, 1, 1 };
                dist = new int[] { 0, 1, 1, 1, 2 };
                startingCarrots = 0;
                maxCarrots = 1;
                expectedResult = -1;
                findOptTests.Add((time, description, money, carrots, dist, maxCarrots, startingCarrots, expectedResult));

                //4
                description = "musimy odrzucać marchewki";
                money = new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 0 };
                carrots = new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 4 };
                dist = new int[] { 0, 5, 1, 5, 1, 4, 1, 4, 2 };
                startingCarrots = 4;
                maxCarrots = 5;
                expectedResult = 200;
                findOptTests.Add((time, description, money, carrots, dist, maxCarrots, startingCarrots, expectedResult));

                //5
                description = "nietrywialny przykład";
                money = new int[] { 10, 5, 15, 12, 6, 13, 5, 4, 10 };
                carrots = new int[] { 5, 2, 3, 6, 3, 2, 6, 3, 5 };
                dist = new int[] { 0, 2, 4, 2, 1, 10, 3, 2, 4 };
                startingCarrots = 10;
                maxCarrots = 100;
                expectedResult = 33;
                findOptTests.Add((time, description, money, carrots, dist, maxCarrots, startingCarrots, expectedResult));

                //6
                description = "nietrywialny przykład, na starcie mamy zero marchewek, małe max carrots";
                money = new int[] { 10, 5, 15, 12, 6, 13, 5, 4, 10 };
                carrots = new int[] { 5, 2, 3, 6, 3, 2, 6, 3, 1 };
                dist = new int[] { 0, 2, 4, 2, 1, 4, 3, 2, 3 };
                startingCarrots = 0;
                maxCarrots = 6;
                expectedResult = 20;
                findOptTests.Add((time, description, money, carrots, dist, maxCarrots, startingCarrots, expectedResult));

                //7
                description = "większy przykład z rozwiązaniem";
                money = new int[] { 5, 3, 6, 7, 8, 2, 1, 1, 6, 7, 3, 2, 4, 1, 9, 6, 2, 8, 9, 1, 6, 2, 3 };
                carrots = new int[] { 3, 2, 5, 1, 6, 4, 2, 6, 5, 3, 7, 1, 5, 3, 8, 7, 6, 1, 5, 3, 5, 5, 0 };
                dist = new int[] { 0, 6, 5, 3, 6, 2, 3, 1, 2, 3, 4, 1, 1, 1, 5, 3, 6, 2, 5, 1, 8, 2, 6 };
                startingCarrots = 10;
                maxCarrots = 20;
                expectedResult = 37;
                findOptTests.Add((time, description, money, carrots, dist, maxCarrots, startingCarrots, expectedResult));

                //8
                description = "większy przykład bez rozwiązania";
                money = new int[] { 5, 3, 6, 7, 8, 2, 1, 1, 6, 7, 3, 2, 4, 1, 9, 6, 2, 8, 9, 1, 6, 2, 3 };
                carrots = new int[] { 3, 2, 5, 1, 6, 4, 2, 6, 5, 3, 7, 1, 5, 3, 8, 7, 6, 1, 5, 3, 5, 5, 0 };
                dist = new int[] { 0, 6, 5, 3, 6, 2, 3, 1, 2, 3, 4, 1, 1, 1, 5, 3, 6, 2, 5, 1, 8, 2, 6 };
                startingCarrots = 11;
                maxCarrots = 11;
                expectedResult = -1;
                findOptTests.Add((time, description, money, carrots, dist, maxCarrots, startingCarrots, expectedResult));

                //9
                rnd = new Random(100);
                description = "duży test losowy";
                int n = 200;
                money = new int[n];
                carrots = new int[n];
                dist = new int[n];
                startingCarrots = 20;
                maxCarrots = 30;

                for (int i = 0; i < n; ++i)
                {
                    money[i] = 1 + rnd.Next(20);
                    dist[i] = 1 + rnd.Next(5);
                    carrots[i] = 1 + rnd.Next(10);
                }
                dist[0] = 0;
                expectedResult = 1680;
                findOptTests.Add((time, description, money, carrots, dist, maxCarrots, startingCarrots, expectedResult));

                //10
                rnd = new Random(200);
                description = "duży test losowy";
                n = 300;
                money = new int[n];
                carrots = new int[n];
                dist = new int[n];
                startingCarrots = 10;
                maxCarrots = 100;

                for (int i = 0; i < n; ++i)
                {
                    money[i] = 1 + rnd.Next(20);
                    dist[i] = 1 + rnd.Next(15);
                    carrots[i] = 1 + rnd.Next(20);
                }
                dist[0] = 0;
                expectedResult = 1688;
                findOptTests.Add((time, description, money, carrots, dist, maxCarrots, startingCarrots, expectedResult));


                TestSets["LabOnlyMax"] = new TestSet(new TaxCollectorManager(), "Cześć 1 : znalezienie ilości złota (3 pkt)", null, false);
                TestSets["LabFullSol"] = new TestSet(new TaxCollectorManager(), "Cześć 2 : znalezienie optymalnego planu (1 pkt)", null, true);


                foreach (var (t, desc, mon, car, dis, mCar, sCar, eRes) in findOptTests)
                {
                    TestSets["LabOnlyMax"].TestCases.Add(new MaxTaxTestCase(t, desc, mon, car, dis, mCar, sCar, eRes));
                    TestSets["LabFullSol"].TestCases.Add(new MaxTaxTestCase(t, desc, mon, car, dis, mCar, sCar, eRes));
                }


            }

            public override double ScoreResult()
            {
                return 1;
            }

        }

        static void Main(string[] args)
        {
            MaxTaxTester tests = new MaxTaxTester();

            tests.PrepareTestSets();
            foreach (var ts in tests.TestSets)
                ts.Value.PerformTests(false);
        }

    }
}
