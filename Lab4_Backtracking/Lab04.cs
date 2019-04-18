using System;
using System.Linq;
namespace ASD
{
    public enum TaxAction
    {
        Empty,
        TakeMoney,
        TakeCarrots
    }

    public class TaxCollectorManager : MarshalByRefObject
    {
        public int CollectMaxTax(int[] dist, int[] money, int[] carrots, int maxCarrots, int startingCarrots, out TaxAction[] collectingPlan)
        {
            collectingPlan = new TaxAction[dist.Length];
            int max = -1;
            int[,] tab = new int[dist.Length + 1, maxCarrots + 1];
            int mar = startingCarrots;
            for (int i = 0; i <= dist.Length; i++)
                for (int j = 0; j <= maxCarrots; j++)
                    tab[i, j] = -1;
            tab[0, startingCarrots] = 0;
            TaxAction[,] tx = new TaxAction[dist.Length + 1, maxCarrots + 1];
            for (int i = 0; i <= dist.Length; i++)
                for (int j = 0; j <= maxCarrots; j++)
                    tx[i, j] = TaxAction.Empty;
            int[,] cr = new int[dist.Length + 1, maxCarrots + 1];
            for (int i = 0; i <= dist.Length; i++)
                for (int j = 0; j <= maxCarrots; j++)
                    cr[i, j] = -1;
            cr[0, startingCarrots] = startingCarrots;
            for (int i = 0; i < dist.Length; i++)
            {
                if (i == dist.Length - 1)
                {
                    for (int j = 0; j < startingCarrots; j++)
                    {
                        if(tab[i, j] != -1)
                        {
                            mar = j + carrots[i];
                            if (mar > maxCarrots) mar = maxCarrots;
                            if(mar >= startingCarrots)
                            {
                                if (tab[i, mar] < tab[i, j])
                                {
                                    tab[i + 1, mar] = tab[i, j];
                                    tx[i + 1, mar] = TaxAction.TakeCarrots;
                                    cr[i + 1, mar] = j;
                                }                                    
                                else
                                {
                                    tab[i + 1, mar] = tab[i, mar];
                                    tx[i + 1, mar] = tx[i, mar];
                                    cr[i + 1, mar] = mar;
                                }                                    
                            }
                        }
                    }
                    for (int j = startingCarrots; j <= maxCarrots; j++)
                        if(tab[i, j] != -1)
                            if(tab[i + 1, j] < tab[i, j] + money[i])
                            {
                                tab[i + 1, j] = tab[i, j] + money[i];
                                tx[i + 1, j] = TaxAction.TakeMoney;
                                cr[i + 1, j] = j;
                            }
                }
                else
                {
                    for (int j = 0; j <= maxCarrots; j++)
                    {
                        if (tab[i, j] != -1)
                        {
                            mar = j + carrots[i];
                            if (mar > maxCarrots) mar = maxCarrots;
                            if (mar - dist[i + 1] < 0) continue;
                            if (tab[i + 1, mar - dist[i + 1]] < tab[i, j])
                            {
                                tab[i + 1, mar - dist[i + 1]] = tab[i, j];
                                tx[i + 1, mar - dist[i + 1]] = TaxAction.TakeCarrots;
                                cr[i + 1, mar - dist[i + 1]] = j;
                            }                                
                            if (j - dist[i + 1] < 0) continue;
                            if (tab[i + 1, j - dist[i + 1]] < tab[i, j] + money[i])
                            {
                                tab[i + 1, j - dist[i + 1]] = tab[i, j] + money[i];
                                tx[i + 1, j - dist[i + 1]] = TaxAction.TakeMoney;
                                cr[i + 1, j - dist[i + 1]] = j;
                            }                                
                        }
                    }
                }
            }
            mar = startingCarrots;
            for (int i = startingCarrots; i <= maxCarrots; i++)
            {
                if (tab[dist.Length, i] > max)
                {
                    max = tab[dist.Length, i];
                    mar = i;
                }
            }
            if (max == -1)
            {
                collectingPlan = null;
                return max;
            }
            for(int i = dist.Length; i > 0; i--)
            {
                collectingPlan[i - 1] = tx[i, mar];
                mar = cr[i, mar];
            }
            return max;
        }
    }
}