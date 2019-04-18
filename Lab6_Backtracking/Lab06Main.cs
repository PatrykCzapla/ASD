using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    public class FindDisivionTestCase : TestCase
    {
        protected int n;
        protected int[] sizes;
        protected int expectedSquaresNr;

        protected int[,] result;
        protected int resultSquaresNr;

        public FindDisivionTestCase(double timeLimit, Exception expectedException, string description, int n, int[] sizes, int expectedSquaresNr)
            : base(timeLimit, expectedException, description)
        {
            this.n = n;
            this.expectedSquaresNr = expectedSquaresNr;
            this.sizes = sizes.Clone() as int[];
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            resultSquaresNr = ((Squares)prototypeObject).FindDisivion(n, sizes, out result);
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            if (expectedSquaresNr > 0 && resultSquaresNr <= 0)
                return (Result.WrongResult, "zwrócono brak rozwiązań, podczas gry rozwiązanie istnieje");

            if (expectedSquaresNr == 0 && resultSquaresNr > 0)
                return (Result.WrongResult, "zwrócono liczbę kwadratów większą od 0, podczas gry rozwiązanie nie istnieje");

            if (expectedSquaresNr == 0 && resultSquaresNr <= 0)
                return (Result.Success, $"OK - brak rozwiąznia, {PerformanceTime:#0.00}");

            if (!(bool)settings) //nie sprawdzamy poprawności tablicy
                return (Result.Success, $"OK - istnieje rozwiąznie, {PerformanceTime:#0.00}");

            string m = "istnienie rozwiązania OK, ";

            if (result == null || result.GetLength(0) != n || result.GetLength(1) != n)
                return (Result.WrongResult, m + "nieprawidłowy rozmiar tablicy z rozwiązaniem");

            Dictionary<int, int> squares = new Dictionary<int, int>(); //słownik (id_kwadratu, szerokość)

            bool isBlank = false;
            int notASquare = 0;
            int wrongSize = 0;

            //sprawdzamy czy we wszystkich wierszach to samo id kwadratu ma tę samą długość
            for (int y = 0; y < n && !isBlank; y++)
            {
                int size = 1; //aktualna długość ciągu o tym samym id kwadratu
                for (int x = 0; x < n; x++)
                {
                    if (result[x, y] <= 0) { isBlank = true; break; }

                    if (x == n - 1 || result[x, y] != result[x + 1, y]) //zmiana id kwadratu
                    {
                        if (squares.ContainsKey(result[x, y])) //jeśli już wcześniej był taki sam id kwadratu
                        {
                            if (squares[result[x, y]] != size) //to sprawdzamy czy była to taka sama długość ciągu
                                notASquare = result[x, y];
                        }
                        else //pierwsze wystąpienie danego id kwadratu, dodajemy długość boku do słownika
                        {
                            squares.Add(result[x, y], size);
                            if (!sizes.Contains(size)) //sprawdzamy czy znaleziona długość boku jest dopuszczalna
                                wrongSize = size;
                        }
                        size = 1;
                    }
                    else
                        size++;
                }
            }

            if (isBlank)
                return (Result.WrongResult, m + " nie wszystkie miejsca w tablicy wypełnione kwadratami");

            //sprawdzamy czy we wszystkich kolumnach to samo id kwadratu ma tę samą długość co wcześniej w wierszach (wtedy mamy pewność, że tworzy kwadrat)
            //nie musimy już sprawdzać istnienia klucza w słowniku, bo wszystkie elementy rozwiązania przeszliśmy wyżej podczas rozważania wierszy
            //podobnie jeśli było jakieś puste pole, to zostało już znalezione
            for (int x = 0; x < n; x++)
            {
                int size = 1;
                for (int y = 0; y < n; y++)
                {
                    if (y == n - 1 || result[x, y] != result[x, y + 1]) //zmiana kwadratu
                    {
                        if (squares[result[x, y]] != size)
                            notASquare = result[x, y];
                        size = 1;
                    }
                    else
                        size++;
                }
            }

            if (notASquare > 0)
                return (Result.WrongResult, m + " obiekt " + notASquare + " nie tworzy kwadratu");

            if (wrongSize > 0)
                return (Result.WrongResult, " w rozwiązaniu zwrócono kwadrat o boku " + wrongSize + " który nie jest dopuszczalny");

            if (squares.Count != expectedSquaresNr)
                return (Result.WrongResult, m + " nieoptymalne rozwiązanie. Użyto: " + squares.Count + " kwadratów, można: " + expectedSquaresNr);

            return (Result.Success, $"OK, {PerformanceTime:#0.00}");
        }

    }


    class Lab06TestModule : TestModule
    {

        public override void PrepareTestSets()
        {
            TestSets["SolutionExistenceTests"] = new TestSet(new Squares(), "Części 1 - testy sprawdzające istnienie rozwiązania", null, false);

            TestSets["SolutionExistenceTests"].TestCases.Add(new FindDisivionTestCase(1, null, "pole o boku 1", 1, new int[] { 1 }, 1));
            TestSets["SolutionExistenceTests"].TestCases.Add(new FindDisivionTestCase(1, null, "pole z jednostkowymi kwadratami", 10, new int[] { 1 }, 100));
            TestSets["SolutionExistenceTests"].TestCases.Add(new FindDisivionTestCase(1, null, "dostępny kwadrat większy niż pole", 2, new int[] { 5 }, 0));
            TestSets["SolutionExistenceTests"].TestCases.Add(new FindDisivionTestCase(1, null, "nieparzyste pole, dopuszczalne kwadraty parzyste", 9, new int[] { 2, 4, 6 }, 0));
            TestSets["SolutionExistenceTests"].TestCases.Add(new FindDisivionTestCase(1, null, "bok pola liczbą pierwszą (7)", 7, new int[] { 6, 5, 4, 1, 2, 3 }, 9));
            TestSets["SolutionExistenceTests"].TestCases.Add(new FindDisivionTestCase(1, null, "prosty test poprawnościowy", 6, new int[] { 3, 2 }, 4));
            TestSets["SolutionExistenceTests"].TestCases.Add(new FindDisivionTestCase(1, null, "prosty test poprawnościowy", 6, new int[] { 7, 9, 5, 13 }, 0));
            TestSets["SolutionExistenceTests"].TestCases.Add(new FindDisivionTestCase(1, null, "brak dopuszczalnych kwadratów", 5, new int[] { }, 0));
            TestSets["SolutionExistenceTests"].TestCases.Add(new FindDisivionTestCase(1, null, "pole bez dzielenia", 8, new int[] { 2, 4, 8 }, 1));
            TestSets["SolutionExistenceTests"].TestCases.Add(new FindDisivionTestCase(1, null, "pole o boku 5", 5, new int[] { 1, 2, 3 }, 8));
            TestSets["SolutionExistenceTests"].TestCases.Add(new FindDisivionTestCase(1, null, "pole o boku 6", 6, new int[] { 1, 5 }, 12));

            TestSets["SmallTests"] = new TestSet(new Squares(), "Części 2 - małe testy poprawnościowe", null, true);

            TestSets["SmallTests"].TestCases.Add(new FindDisivionTestCase(1, null, "pole o boku 1", 1, new int[] { 1 }, 1));
            TestSets["SmallTests"].TestCases.Add(new FindDisivionTestCase(1, null, "pole z jednostkowymi kwadratami", 10, new int[] { 1 }, 100));
            TestSets["SmallTests"].TestCases.Add(new FindDisivionTestCase(1, null, "dostępny kwadrat większy niż pole", 2, new int[] { 5 }, 0));
            TestSets["SmallTests"].TestCases.Add(new FindDisivionTestCase(1, null, "nieparzyste pole, dopuszczalne kwadraty parzyste", 9, new int[] { 2, 4, 6 }, 0));
            TestSets["SmallTests"].TestCases.Add(new FindDisivionTestCase(1, null, "bok pola liczbą pierwszą (7)", 7, new int[] { 6, 5, 4, 1, 2, 3 }, 9));
            TestSets["SmallTests"].TestCases.Add(new FindDisivionTestCase(1, null, "prosty test poprawnościowy", 6, new int[] { 3, 2 }, 4));
            TestSets["SmallTests"].TestCases.Add(new FindDisivionTestCase(1, null, "prosty test poprawnościowy", 6, new int[] { 7, 9, 5, 13 }, 0));
            TestSets["SmallTests"].TestCases.Add(new FindDisivionTestCase(1, null, "brak dopuszczalnych kwadratów", 5, new int[] { }, 0));
            TestSets["SmallTests"].TestCases.Add(new FindDisivionTestCase(1, null, "pole bez dzielenia", 8, new int[] { 2, 4, 8 }, 1));
            TestSets["SmallTests"].TestCases.Add(new FindDisivionTestCase(1, null, "pole o boku 5", 5, new int[] { 1, 2, 3 }, 8));
            TestSets["SmallTests"].TestCases.Add(new FindDisivionTestCase(1, null, "pole o boku 6", 6, new int[] { 1, 5 }, 12));


            TestSets["BigTests"] = new TestSet(new Squares(), "Część 3 - testy wydajnościowe", null, true);

            TestSets["BigTests"].TestCases.Add(new FindDisivionTestCase(2, null, "dużo kwadratów o boku 1", 50, new int[] { 1 }, 2500));
            TestSets["BigTests"].TestCases.Add(new FindDisivionTestCase(10, null, "mało kwadratów o długim boku", 2000, new int[] { 500 }, 16));
            TestSets["BigTests"].TestCases.Add(new FindDisivionTestCase(1, null, "mało kwadratów o długim boku", 101, new int[] { 5 }, 0));
            TestSets["BigTests"].TestCases.Add(new FindDisivionTestCase(1, null, "bok pola liczbą pierwszą (11)", 11, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 11));
            TestSets["BigTests"].TestCases.Add(new FindDisivionTestCase(1, null, "próba podziału pola o nieparzystym boku na tylko parzyste", 25, new int[] { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24 }, 0));
            TestSets["BigTests"].TestCases.Add(new FindDisivionTestCase(1, null, "dużo rozmiarów, wszystkie większe niż rozmiar pola", 10, Enumerable.Range(11, 10000).ToArray(), 0));
            TestSets["BigTests"].TestCases.Add(new FindDisivionTestCase(5, null, "duże pole, ale proste rozwiązanie: 4 kwadraty o boku 20", 40, Enumerable.Range(1, 20).ToArray(), 4));
            TestSets["BigTests"].TestCases.Add(new FindDisivionTestCase(20, null, "pole o boku 13", 13, Enumerable.Range(1, 12).ToArray(), 11));
            TestSets["BigTests"].TestCases.Add(new FindDisivionTestCase(20, null, "pole o boku 19", 19, Enumerable.Range(3, 14).ToArray(), 14));
            TestSets["BigTests"].TestCases.Add(new FindDisivionTestCase(25, null, "pole o boku 23", 23, new int[] { 3, 4, 7 }, 32));
            TestSets["BigTests"].TestCases.Add(new FindDisivionTestCase(30, null, "pole o boku 25", 25, new int[] { 4, 5, 6, 7 }, 25));
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
            Lab06TestModule tests = new Lab06TestModule();
            tests.PrepareTestSets();

            tests.TestSets["SolutionExistenceTests"].PerformTests(false);
            tests.TestSets["SmallTests"].PerformTests(false);
            tests.TestSets["BigTests"].PerformTests(true);
        }
    }
}
