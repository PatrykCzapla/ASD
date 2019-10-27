using ASD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab9
{

    public class Lab09TestCase : TestCase
    {
        private readonly int[] limits;
        private readonly int[][] preferences;
        private readonly bool[] isSportActivity;
        private readonly int expectedSatisfactionLevel;

        private int satisfactionLevel;
        private int[] bestDistribution;

        public Lab09TestCase(double timeLimit, string description, int[] limits, int[][] preferences, bool[] isSportAct, int expectedSatisfactionLevel)
            : base(timeLimit, null, description)
        {
            this.limits = limits.Clone() as int[];
            this.preferences = preferences.Clone() as int[][];
            this.isSportActivity = isSportAct?.Clone() as bool[];
            this.expectedSatisfactionLevel = expectedSatisfactionLevel;
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            (satisfactionLevel, bestDistribution) = ((DistributionFinder)prototypeObject).FindBestDistribution(limits, preferences, isSportActivity);
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            if (satisfactionLevel < expectedSatisfactionLevel)
                return (Result.WrongResult, $"X  Za niski poziom satysfakcji: {satisfactionLevel} (oczekiwany: {expectedSatisfactionLevel})");
            if (satisfactionLevel > expectedSatisfactionLevel)
                return (Result.WrongResult, $"X  Za wysoki poziom satysfakcji: {satisfactionLevel} (oczekiwany: {expectedSatisfactionLevel})");
            if ( isSportActivity!=null && expectedSatisfactionLevel==0 )
                return bestDistribution==null ?
                       (Result.Success, $"OK ({PerformanceTime:#0.00})") :  // jedyny przypadek gdy bestDistribution==null jest OK
                       (Result.WrongResult, "X  Zwrócona lista dystrybucji powinna być nullem") ;
            if (bestDistribution == null)
                return (Result.WrongResult, "X  Zwrócona lista dystrybucji jest nullem");
            if (bestDistribution.Length != preferences.Length)
                return (Result.WrongResult, $"X  Zwrócona lista dystrybucji ma niepoprawną długość: {bestDistribution.Length} (oczekiwana: {preferences.Length})");
            if (bestDistribution.Any(x => x >= limits.Length || x < -1))
                return (Result.WrongResult, "X  Zwrócona lista dystrybucji zawiera indeks nieistniejących zajęć");

            for (int i = 0; i < limits.Length; i++)
                {
                var usedSlots = bestDistribution.Count(x => x == i);
                if (usedSlots > limits[i])
                    return (Result.WrongResult, "X  Zbyt wielu uczestników przypisanych do tej samej aktywności");
                }

            for (int i = 0; i < preferences.Length; i++)
                if (bestDistribution[i] != -1 && !preferences[i].Contains(bestDistribution[i]))
                    return (Result.WrongResult, "X  Uczeń został przypisany do niepreferowanej grupy zajęciowej");

            if ( isSportActivity!=null )
                {
                for (int i = 0; i < isSportActivity.Length; i++)
                    if ( isSportActivity[i] )
                        {
                        if ( bestDistribution.Count(x => x == i)!=limits[i] )
                            return (Result.WrongResult, $"X  Zajęcia sportowe #{i} nie mają pełnych składów");
                        }
                    else
                        {
                        if ( bestDistribution.Count(x => x == i)>0 )
                            return (Result.WrongResult, $"X  uczestnik przydzielony do zajęć które nie są sportowe");
                        }
                }
            else 
                {
                var enlistedStudentsNum = bestDistribution.Where(x => x >= 0).Count();
                if (enlistedStudentsNum != expectedSatisfactionLevel)
                    return (Result.WrongResult, $"X  Niepoprawna odpowiedź, zapisano {enlistedStudentsNum} uczniów (oczekiwany poziom satysfakcji: {expectedSatisfactionLevel})");
                }

            return (Result.Success, $"OK ({PerformanceTime:#0.00})");
        }
    }

    public class DistributionTestModule : TestModule
    {
        public override void PrepareTestSets()
        {
            PrepareLabTestSets();
        }

        public override double ScoreResult()
        {
            return 1;
        }

        private void PrepareLabTestSets()
        {
            string description;
            int[] limits;
            int[][] preferences;
            int expectedSatisfaction;
            TestSets["Satisfaction"] = new TestSet(new DistributionFinder(), "Część 1. -- Poziom satysfakcji (3 p.)\n");
            TestSets["Sports"] = new TestSet(new DistributionFinder(), "Część 2. -- Zajęcia sportowe (1 p.)\n");

            description = "Przykład 1";
            limits = new int[] { 2, 2 };
            preferences = new int[][]
            {
                new int[] {0},
                new int[] {1},
                new int[] {0, 1},
            };
            expectedSatisfaction = 3;
            TestSets["Satisfaction"].TestCases.Add(new Lab09TestCase(2, description, limits, preferences, null, expectedSatisfaction));

            description = "Przykład 2";
            limits = new int[] { 2, 3 };
            preferences = new int[][]
            {
                new int[] {0, 1},
                new int[] {0},
                new int[] {1},
                new int[] {1},
                new int[] {0, 1},
                new int[] {0},
            };
            expectedSatisfaction = 5;
            TestSets["Satisfaction"].TestCases.Add(new Lab09TestCase(1, description, limits, preferences, null, expectedSatisfaction));

            description = "Trzech indywidualistów";
            limits = new int[] { 1, 1, 1 };
            preferences = new int[][]
            {
                new int[] {0, 2},
                new int[] {1},
                new int[] {0, 1},
            };
            expectedSatisfaction = 3;
            TestSets["Satisfaction"].TestCases.Add(new Lab09TestCase(1, description, limits, preferences, null, expectedSatisfaction));

            description = "2. Do tanga trzeba dwojga";
            limits = new int[] { 2, 2, 2, 2 };
            preferences = new int[8][];
            for (int i = 0; i < 8; i++)
                preferences[i] = new int[] { i % 4 };
            expectedSatisfaction = 8;
            TestSets["Satisfaction"].TestCases.Add(new Lab09TestCase(1, description, limits, preferences, null, expectedSatisfaction));

            description = "Za dużo chętnych";
            limits = new int[] { 10 };
            preferences = new int[20][];
            for (int i = 0; i < 20; i++)
                preferences[i] = new int[] { 0 };
            expectedSatisfaction = 10;
            TestSets["Satisfaction"].TestCases.Add(new Lab09TestCase(1, description, limits, preferences, null, expectedSatisfaction));

            description = "Noga, siatka, kosz i ręczna";
            limits = new int[] { 22, 12, 10, 14 };
            preferences = new int[58][];
            for (int i = 0; i < 25; i++)
                preferences[i] = new[] { 0 };
            for (int i = 25; i <= 30; i++)
                preferences[i] = new[] { 0, 1, 2 };
            for (int i = 31; i < 32; i++)
                preferences[i] = new[] { 1, 2 };
            preferences[32] = preferences[33] = new int[] { };
            for (int i = 34; i < 45; i++)
                preferences[i] = new[] { 1, 2 };
            for (int i = 45; i < 58; i++)
                preferences[i] = new[] { 1, 3 };
            expectedSatisfaction = 53;
            TestSets["Satisfaction"].TestCases.Add(new Lab09TestCase(1, description, limits, preferences, null, expectedSatisfaction));

            description = "Ktoś ma pecha";
            limits = new int[] { 15, 15, 10, 8, 12, 20, 8, 10 };
            preferences = new int[62][];
            var random = new Random(42);

            for (int i = 0; i < 62; i++)
            {
                var tmp = new List<int>();
                for (int k = 0; k < limits.Length; k++)
                    if (random.NextDouble() < 0.4)
                        tmp.Add(k);
                preferences[i] = tmp.ToArray();
            }
            expectedSatisfaction = 61;
            TestSets["Satisfaction"].TestCases.Add(new Lab09TestCase(1, description, limits, preferences, null, expectedSatisfaction));

            description = "Losowe dane #1";
            (limits, preferences) = GenerateRandomTest(5, 5, 15, 5);
            TestSets["Satisfaction"].TestCases.Add(new Lab09TestCase(1, description, limits, preferences, null, expectedSatisfactionLevel: 11));

            description = "Losowe dane #2";
            (limits, preferences) = GenerateRandomTest(10, 50, 100, 5432);
            TestSets["Satisfaction"].TestCases.Add(new Lab09TestCase(1, description, limits, preferences, null, expectedSatisfactionLevel: 99));

            description = "Losowe dane #3";
            (limits, preferences) = GenerateRandomTest(20, 50, 1000, 417);
            TestSets["Satisfaction"].TestCases.Add(new Lab09TestCase(2, description, limits, preferences, null, expectedSatisfactionLevel: 502));

            description = "Iluzja wolnego wyboru";
            limits = new int[] { 0, 4 };
            preferences = new int[10][];
            for (int i = 0; i < 10; i++)
                preferences[i] = new int[] { 0, 1 };
            expectedSatisfaction = 4;
            TestSets["Satisfaction"].TestCases.Add(new Lab09TestCase(1, description, limits, preferences, null, expectedSatisfaction));

            description = "Malkontent";
            limits = new int[] { 1, 1, 1, 1, 1 };
            preferences = new int[][] { new int[] { } };
            expectedSatisfaction = 0;
            TestSets["Satisfaction"].TestCases.Add(new Lab09TestCase(1, description, limits, preferences, null, expectedSatisfaction));

            description = "Losowe dane #4";
            (limits, preferences) = GenerateRandomTest(5, 5, 15, 6);
            TestSets["Satisfaction"].TestCases.Add(new Lab09TestCase(1, description, limits, preferences, null, expectedSatisfactionLevel: 14));

            description = "Losowe dane #5";
            (limits, preferences) = GenerateRandomTest(80, 10, 500, 2);
            TestSets["Satisfaction"].TestCases.Add(new Lab09TestCase(2, description, limits, preferences, null, expectedSatisfactionLevel: 386));

            PrepareLabSportsTestSets();
        }

        private void PrepareLabSportsTestSets()
        {
            var defaultTimeLimit = 1;

            {
                var description = "Prosty przykład (pozytywny)";
                var limits = new int[] { 2, 2 };
                var preferences = new int[][]
                {
                    new int[] {0},
                    new int[] {1},
                    new int[] {0, 1},
                };
                var isSportsActivity = new bool[] { true, false };

                TestSets["Sports"].TestCases.Add(new Lab09TestCase(defaultTimeLimit, description, limits, preferences, isSportsActivity, 1));
            }

            {
                var description = "Prosty przykład (negatywny)";
                var limits = new int[] { 3, 2 };
                var preferences = new int[][]
                {
                    new int[] {0},
                    new int[] {1},
                    new int[] {0, 1},
                };
                var isSportsActivity = new bool[] { true, false };

                TestSets["Sports"].TestCases.Add(new Lab09TestCase(defaultTimeLimit, description, limits, preferences, isSportsActivity, 0));
            }

            {
                var description = "Parzyści";
                var limits = new int[] { 1, 2, 3, 4, 5, 6 };
                var preferences = new int[][]
                {
                    new int[] {0, 1, 2},
                    new int[] {1, 4, 5},
                    new int[] {2, 3, 4},
                    new int[] {0, 1, 2},
                    new int[] {1, 4, 5},
                    new int[] {2, 3, 4},
                    new int[] {1, 4, 5},
                    new int[] {1, 4, 5},
                    new int[] {1, 4, 5},
                    new int[] {2, 3, 4},
                    new int[] {2, 3, 4},
                    new int[] {1, 4, 5},
                };
                var isSportsActivity = new bool[] { false, true, false, true, false, true };

                TestSets["Sports"].TestCases.Add(new Lab09TestCase(defaultTimeLimit, description, limits, preferences, isSportsActivity, 1));
            }

            {
                var description = "Każdemu wedle uznania";
                var limits = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                var preferences = new int[][]
                {
                    new int[] {0},
                    new int[] {1},
                    new int[] {2},
                    new int[] {3},
                    new int[] {4},
                    new int[] {5},
                    new int[] {6},
                    new int[] {7},
                    new int[] {8},
                    new int[] {9},
                };
                var isSportsActivity = new bool[10];
                for (int i = 0; i < 10; i++) isSportsActivity[i] = true;

                TestSets["Sports"].TestCases.Add(new Lab09TestCase(defaultTimeLimit, description, limits, preferences, isSportsActivity, 1));
            }

            {
                var description = "Zdublowany";
                var limits = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                var preferences = new int[][]
                {
                    new int[] {0},
                    new int[] {1},
                    new int[] {2},
                    new int[] {4},
                    new int[] {5},
                    new int[] {6},
                    new int[] {7},
                    new int[] {4},
                    new int[] {8},
                    new int[] {9},
                };
                var isSportsActivity = new bool[10];
                for (int i = 0; i < 10; i++) isSportsActivity[i] = true;

                TestSets["Sports"].TestCases.Add(new Lab09TestCase(defaultTimeLimit, description, limits, preferences, isSportsActivity, 0));
            }

            {
                var description = "Inne niż wszystkie";
                var limits = new int[] { 1, 1, 1, 1, 1, 1, 2, 1, 1, 1 };
                var preferences = new int[][]
                {
                    new int[] {0},
                    new int[] {1},
                    new int[] {2},
                    new int[] {3},
                    new int[] {4},
                    new int[] {5},
                    new int[] {7},
                    new int[] {8},
                    new int[] {9},
                };
                var isSportsActivity = new bool[10];
                for (int i = 0; i < 10; i++) isSportsActivity[i] = true;
                isSportsActivity[6] = false;

                TestSets["Sports"].TestCases.Add(new Lab09TestCase(defaultTimeLimit, description, limits, preferences, isSportsActivity, 1));
            }
        }

        private (int[], int[][]) GenerateRandomTest(int activitiesNum, int maxLimit, int peopleNum, int seed)
        {
            var rng = new Random(seed);
            var limits = new List<int>();
            for (int i = 0; i < activitiesNum; i++)
                limits.Add(rng.Next(1, maxLimit));
            var preferences = new int[peopleNum][];
            for (int i = 0; i < peopleNum; i++)
            {
                var list = new List<int>();
                for (int actIdx = 0; actIdx < activitiesNum; actIdx++)
                    if (rng.NextDouble() > 0.75)
                        list.Add(actIdx);
                if (list.Count == 0)
                    list.Add(rng.Next(activitiesNum));
                preferences[i] = list.ToArray();
            }
            return (limits.ToArray(), preferences);
        }

        //private (int[], int[][]) GenerateRandomTest2(int activitiesNum, int maxLimit, int peopleNum, int seed)
        //{
        //    var rng = new Random(seed);
        //    var limits = new List<int>();
        //    for (int i = 0; i < activitiesNum; i++)
        //        limits.Add(rng.Next(1, maxLimit));
        //    var preferences = new int[peopleNum][];
        //    for (int i = 0; i < peopleNum; i++)
        //    {
        //        var set = new HashSet<int>();
        //        var actNum = rng.Next(activitiesNum / 2);
        //        var currentNum = 0;
        //        while (currentNum < actNum)
        //        {
        //            var activity = rng.Next(activitiesNum);
        //            if (!set.Contains(activity))
        //                set.Add(activity);
        //        }
        //        preferences[i] = set.ToArray();
        //    }
        //    return (limits.ToArray(), preferences);
        //}
    }

    class Lab09Main
    {
        static void Main(string[] args)
        {
            var testRunner = new DistributionTestModule();
            testRunner.PrepareTestSets();
            foreach (var testSet in testRunner.TestSets)
                testSet.Value.PerformTests(false);
            Console.ReadLine();

        }
    }

}
