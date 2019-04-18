using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class Squares : MarshalByRefObject
    {
        /// <param name="n">Długość boku działki, którą dzielimy</param>
        /// <param name="sizes">Dopuszczalne długości wynikowych działek</param>
        /// <param name="solution">Tablica n*n z znalezionym podziałem, każdy element to unikalny dodatni identyfikator kwadratu</param>
        /// <returns>Liczba kwadratów na jakie została podzielona działka lub 0 jeśli poprawny podział nie istnieje </returns>
        public int FindDisivion(int n, int[] sizes, out int[,] solution)
        {
            solution = new int[n, n];
            if (n == 0) return 0;
            sizes = sizes.OrderByDescending(x => x).ToArray();
            int BestSolution = int.MaxValue;
            int[,] IsDivided = new int[n, n];
            FindDivisionRec(n, sizes, IsDivided, ref solution, ref BestSolution, 0);
            return (BestSolution == int.MaxValue) ? 0 : BestSolution;
        }
        public void FindDivisionRec(int n, int[] sizes, int[,] IsDivided, ref int[,] solution, ref int BestSolution, int TmpSolution)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    int CurrentSolution = TmpSolution;
                    if (IsDivided[i, j] != 0) continue;

                    for (int z = 0; z < sizes.Length; z++)
                    {
                        bool IsOk = true;
                        if ((i + sizes[z] > n) || (j + sizes[z] > n)) continue;
                        for (int k = i; k < i + sizes[z]; k++)
                        {
                            for (int w = j; w < j + sizes[z]; w++)
                            {
                                if (IsDivided[k, w] != 0)
                                {
                                    IsOk = false;
                                    break;
                                }
                            }
                            if (IsOk == false) break;
                        }
                        if (IsOk == false) continue;
                        for (int k = i; k < i + sizes[z]; k++)
                            for (int w = j; w < j + sizes[z]; w++)
                                IsDivided[k, w] = CurrentSolution + 1;
                        CurrentSolution++;
                        if (BestSolution < CurrentSolution)
                        {
                            for (int k = i; k < i + sizes[z]; k++)
                                for (int w = j; w < j + sizes[z]; w++)
                                    IsDivided[k, w] = 0;
                            CurrentSolution--;
                            continue;
                        }
                        IsOk = true;
                        for (int k = 0; k < n; k++)
                        {
                            {
                                for (int w = 0; w < n; w++)
                                    if (IsDivided[k, w] == 0)
                                    {
                                        IsOk = false;
                                        break;
                                    }
                                if (IsOk == false) break;
                            }
                        }
                        if (IsOk == true)
                        {
                            if (CurrentSolution < BestSolution)
                            {
                                solution = (int[,])IsDivided.Clone();
                                BestSolution = CurrentSolution;
                            }
                            for (int k = i; k < i + sizes[z]; k++)
                                for (int w = j; w < j + sizes[z]; w++)
                                    IsDivided[k, w] = 0;
                            CurrentSolution--;
                            continue;
                        }
                        FindDivisionRec(n, sizes, IsDivided, ref solution, ref BestSolution, CurrentSolution);
                        for (int k = i; k < i + sizes[z]; k++)
                            for (int w = j; w < j + sizes[z]; w++)
                                IsDivided[k, w] = 0;
                        CurrentSolution--;
                    }
                    return;
                }
            }
        }

        ///Wersja teoretyczie (zaskakująco) szybsza (dla przykładów labolatoryjnych), ale mniej czytelna

        //public int FindDisivion(int n, int[] sizes, out int[,] solution)
        //{
        //    solution = new int[n, n];
        //    if (n == 0) return 0;
        //    sizes = sizes.OrderByDescending(x => x).ToArray();
        //    int no = 1;
        //    int best = int.MaxValue;
        //    int[,] sol = new int[n, n];
        //    bool[,] isDivided = new bool[n, n];
        //    FindDivisionRec(n, sizes, isDivided, ref solution, ref no, ref best, 0, sol);
        //    if (best == int.MaxValue) return 0;
        //    return best;
        //}

        //public int FindDivisionRec(int n, int[] sizes, bool[,] isDivided, ref int[,] solution, ref int no, ref int best, int ile, int[,] sol)
        //{
        //    for (int i = 0; i < n; i++)
        //    {
        //        for (int j = 0; j < n; j++)
        //        {
        //            int tmp = ile;
        //            if (isDivided[i, j] == true) continue;
        //            for (int z = 0; z < sizes.Length; z++)
        //            {
        //                bool isOk = true;
        //                if ((i + sizes[z] > n) || (j + sizes[z] > n))
        //                {
        //                    continue;
        //                }
        //                for (int k = i; k < i + sizes[z]; k++)
        //                {
        //                    for (int w = j; w < j + sizes[z]; w++)
        //                    {
        //                        if (isDivided[k, w] == true || sol[k, w] != 0)
        //                        {
        //                            isOk = false;
        //                            break;
        //                        }
        //                    }
        //                    if (isOk == false) break;
        //                }
        //                if (isOk == false)
        //                    continue;
        //                for (int k = i; k < i + sizes[z]; k++)
        //                {
        //                    for (int w = j; w < j + sizes[z]; w++)
        //                    {
        //                        isDivided[k, w] = true;
        //                        sol[k, w] = no;
        //                    }
        //                }
        //                no++;
        //                tmp++;
        //                if (tmp >= best)
        //                {
        //                    for (int k = i; k < i + sizes[z]; k++)
        //                    {
        //                        for (int w = j; w < j + sizes[z]; w++)
        //                        {
        //                            isDivided[k, w] = false;
        //                            sol[k, w] = 0;
        //                        }
        //                    }
        //                    tmp--;
        //                    continue;
        //                }
        //                isOk = true;
        //                for (int t = 0; t < n; t++)
        //                {
        //                    for (int a = 0; a < n; a++)
        //                    {
        //                        if (isDivided[t, a] == false) isOk = false;
        //                        if (isOk == false) break;
        //                    }
        //                    if (isOk == false) break;
        //                }
        //                if (isOk == true)
        //                {
        //                    if (tmp < best)
        //                    {
        //                        solution = (int[,])sol.Clone();
        //                        no++;
        //                        best = tmp;
        //                    }
        //                    for (int k = i; k < i + sizes[z]; k++)
        //                    {
        //                        for (int w = j; w < j + sizes[z]; w++)
        //                        {
        //                            isDivided[k, w] = false;
        //                            sol[k, w] = 0;
        //                        }
        //                    }
        //                    tmp--;
        //                    continue;
        //                }
        //                FindDivisionRec(n, sizes, isDivided, ref solution, ref no, ref best, tmp, sol);
        //                for (int k = i; k < i + sizes[z]; k++)
        //                {
        //                    for (int w = j; w < j + sizes[z]; w++)
        //                    {
        //                        isDivided[k, w] = false;
        //                        sol[k, w] = 0;
        //                    }
        //                }
        //                tmp--;
        //            }
        //            return 0;
        //        }
        //    }
        //    return 0;
        //}
    }
}