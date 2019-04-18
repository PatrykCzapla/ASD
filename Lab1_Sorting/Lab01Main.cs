
using System;

namespace ASD
{

public enum SortingAlgorithm { Undefined, QuickSort, ShellSort, HeapSort, MergeSort }

public class SortingTestCase : TestCase
    {

    private int[] tab;
    private int[] sortedTab;
    private SortingAlgorithm algorithm;

    public SortingTestCase(double timeLimit, Exception expectedException, string description, int[] source, int[] expectedResult, SortingAlgorithm algorithm)
        : base(timeLimit, expectedException, description)
        {
        this.tab = (int[])source.Clone();
        this.sortedTab = (int[])expectedResult.Clone();
        this.algorithm = algorithm;
        }

    protected override void PerformTestCase(object prototypeObject)
        {
        SortingMethods sortingMethodTester = (SortingMethods)prototypeObject;
        switch ( algorithm )
            {
            case SortingAlgorithm.QuickSort:
                tab = sortingMethodTester.QuickSort(tab);
                break;
            case SortingAlgorithm.ShellSort:
                tab = sortingMethodTester.ShellSort(tab);
                break;
            case SortingAlgorithm.HeapSort:
                tab = sortingMethodTester.HeapSort(tab);
                break;
            case SortingAlgorithm.MergeSort:
                tab = sortingMethodTester.MergeSort(tab);
                break;
            }
        }

    protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
        return System.Linq.Enumerable.SequenceEqual(tab,sortedTab) ?
               (Result.Success,"OK") : (Result.WrongResult, "Elements haven't been sorted properly");
        }

    }

class SortingTestModule : TestModule
    {

    public override void PrepareTestSets()
        {

        const int testCaseNumber = 9;
        Random rnd;
        int[][] tabs = new int[testCaseNumber][];
        int[][] sortedTabs = new int[testCaseNumber][];

        tabs[0] = new int[2] { 3, -2 };
        tabs[1] = new int[10] { 5, 8, -3, 10, 2, 5, 7, -6, 5, 1 };
        tabs[2] = new int[1] { -1 };
        tabs[3] = new int[0];

        tabs[4] = new int[500];
        for ( int i=0 ; i<tabs[4].Length ; ++i )
            tabs[4][i] = i-250;

        rnd = new Random(123);
        tabs[5] = new int[800];
        for ( int i=0 ; i<tabs[5].Length ; ++i )
            tabs[5][i] = rnd.Next(-500,500);

        rnd = new Random(125);
        tabs[6] = new int[2000];
        for ( int i=0 ; i<tabs[6].Length ; ++i )
            tabs[6][i] = rnd.Next();

        rnd = new Random(127);
        tabs[7] = new int[1000];
        for ( int i=0 ; i<tabs[7].Length ; ++i )
            tabs[7][i] = rnd.Next(-10,10);

        rnd = new Random(129);
        tabs[8] = new int[1600];
        for ( int i=0 ; i<tabs[8].Length ; ++i )
            tabs[8][i] = rnd.Next(-1000,1000);

        for ( int k=0 ; k<testCaseNumber ; ++k )
            {
            sortedTabs[k] = (int[])tabs[k].Clone();
            Array.Sort(sortedTabs[k]);
            }

        double[] limits = new double[testCaseNumber] { 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        string[] descriptions = new string[testCaseNumber]
            {
            "bardzo prosty test",
            "prosty test",
            "tablica jednoelementowa",
            "tablica pusta",
            "tablica posortowana",
            "dane losowe 1",
            "dane losowe 2",
            "dane losowe 3",
            "dane losowe 4",
            };

        TestSets["LabQuickSortTests"]    = new TestSet(new SortingMethods(), "Lab. - QuickSort");
        TestSets["LabShellSortTests"]    = new TestSet(new SortingMethods(), "Lab. - ShellSort");
        TestSets["LabHeapSortSortTests"] = new TestSet(new SortingMethods(), "Lab. - HeapSort");
        TestSets["LabMergeSortTests"]    = new TestSet(new SortingMethods(), "Lab. - MergeSort");

        for ( int k=0 ; k<testCaseNumber ; ++k )
            {
            TestSets["LabQuickSortTests"].TestCases.Add(new SortingTestCase(limits[k],null,descriptions[k], tabs[k], sortedTabs[k], SortingAlgorithm.QuickSort));
            TestSets["LabShellSortTests"].TestCases.Add(new SortingTestCase(limits[k],null,descriptions[k], tabs[k], sortedTabs[k], SortingAlgorithm.ShellSort));
            TestSets["LabHeapSortSortTests"].TestCases.Add(new SortingTestCase(limits[k],null,descriptions[k], tabs[k], sortedTabs[k], SortingAlgorithm.HeapSort));
            TestSets["LabMergeSortTests"].TestCases.Add(new SortingTestCase(limits[k],null,descriptions[k], tabs[k], sortedTabs[k], SortingAlgorithm.MergeSort));
            }

        }

    public override double ScoreResult()
        {
        return 1;
        }

    }

class Lab01
    {

    static void Main(string[] args)
        {
        SortingTestModule sortingTests = new SortingTestModule();
        sortingTests.PrepareTestSets();
        foreach (var ts in sortingTests.TestSets)
            ts.Value.PerformTests(false);
        }

    }

}
