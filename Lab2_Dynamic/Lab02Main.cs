
using System;

namespace ASD
{

public class CodingTestCase : TestCase
    {

    private string text;
    private string[] codes;
    private int expectedCodingsNumber;
    private int codingsNumber;
    private int[][] codings;

    public CodingTestCase(double timeLimit, Exception expectedException, string description, string text, string[] codes, int expectedCodesNumber)
        : base(timeLimit, expectedException, description)
        {
        this.text = text;
        this.codes = (string[])codes.Clone();
        this.expectedCodingsNumber = expectedCodesNumber;
        }

    protected override void PerformTestCase(object prototypeObject)
        {
        codingsNumber = (prototypeObject as CodesCounting).CountCodes(text,codes,out codings);
        }

    protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
        if ( codingsNumber!=expectedCodingsNumber )
            return (Result.WrongResult, $"Wrong number of solutions: {codingsNumber} (expected: {expectedCodingsNumber})");
        if ( !(bool)settings )
            return (Result.Success,"OK");
        if ( codings==null )
            return (Result.WrongResult, $"Solutions table is null");
        if ( codings.Length!=codingsNumber )
            return (Result.WrongResult, $"Declared number of solutions ({codingsNumber}) is different than length of solutions table ({codings.Length})");
        System.Text.StringBuilder sb;
        foreach ( int[] coding in codings )
            {
            sb = new System.Text.StringBuilder();
            foreach ( int i in coding )
                sb.Append(codes[i]);
            if ( text!=sb.ToString() )
                return (Result.WrongResult, $"Wrong coding sequence, coded text: {sb.ToString()} (expected: {text})");
            }
        return (Result.Success,"OK");
        }

    }

class SortingTestModule : TestModule
    {

    public override void PrepareTestSets()
        {

        const int testCaseNumber = 8;

        string[] text = new string[testCaseNumber];
        string[][] codes = new string[testCaseNumber][];
        int[] expected = new int[testCaseNumber];

        text[0] = "abca";
        codes[0] = new string[]{ "a", "abc", "ab", "ca" };
        expected[0] = 2;

        text[1] = "aaaaa";
        codes[1] = new string[]{ "a" };
        expected[1] = 1;

        text[2] = "a";
        codes[2] = new string[]{ "b" };
        expected[2] = 0;

        text[3] = "a";
        codes[3] = new string[]{ };
        expected[3] = 0;

        text[4] = "abca";
        codes[4] = new string[]{ "a", "b", "c", "ab", "ca", "abca" };
        expected[4] = 5;

        text[5] = "123123";
        codes[5] = new string[]{ "123", "1", "231", "23", "12", "312", "3", "123123", "2" };
        expected[5] = 21;

        text[6] = "121212";
        codes[6] = new string[]{ "12", "21", "1", "2" };
        expected[6] = 13;

        text[7] =                 "aaaaaaaaaaaaaaaaaa";  // 18 liter a
        codes[7] = new string[] { "aaaaaaaaaaaaaaaaaa",
                                  "aaaaaaaaaaaaaaaaa",
                                  "aaaaaaaaaaaaaaaa",
                                  "aaaaaaaaaaaaaaa",
                                  "aaaaaaaaaaaaaa",
                                  "aaaaaaaaaaaaa",
                                  "aaaaaaaaaaaa",
                                  "aaaaaaaaaaa",
                                  "aaaaaaaaaa",
                                  "aaaaaaaaa",
                                  "aaaaaaaa",
                                  "aaaaaaa",
                                  "aaaaaa",
                                  "aaaaa",
                                  "aaaa",
                                  "aaa",
                                  "aa",
                                  "a",
                                };
        expected[7] = 1<<17;

        double[] limits = new double[testCaseNumber] { 1, 1, 1, 1, 1, 1, 1, 15 };

        string[] descriptions = new string[testCaseNumber]
            {
            "prosty test 1",
            "tekst=\"aaaaa\", kod=\"a\"",
            "brak rozwiazania",
            "pusty zbior kodow",
            "prosty test 2",
            "test \"123\"",
            "test \"12\"",
            "duuuuuzo a",
            };

        TestSets["LabTestsWithoutSequences"] = new TestSet(new CodesCounting(), "Lab. tests - numbers of codings only",null,false);
        TestSets["LabTestsWithSequences"] = new TestSet(new CodesCounting(), "Lab. tests - with coding sequences",null,true);

        for ( int k=0 ; k<testCaseNumber ; ++k )
            {
            TestSets["LabTestsWithoutSequences"].TestCases.Add(new CodingTestCase(limits[k],null,descriptions[k], text[k], codes[k], expected[k]));
            TestSets["LabTestsWithSequences"].TestCases.Add(new CodingTestCase(limits[k],null,descriptions[k], text[k], codes[k], expected[k]));
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
