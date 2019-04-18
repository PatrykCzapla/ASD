using System;

namespace ASD
{

public class SortingMethods: MarshalByRefObject
    {
        public int[] QuickSort(int[] tab)
        {
            if(tab.Length==0)return tab;
            QuickSortRec(tab, 0, tab.Length-1);
            return tab;
        }

        public void QuickSortRec(int[] tab,int l,int r)
        {

            int i = l, j = r;
            int v = tab[(l + r) / 2];
            while (i < j)
            {
                while (tab[i] < v) i++;
                while (tab[j] > v) j--;
                if (i <= j)
                {
                    int tmp = tab[i];
                    tab[i++] = tab[j];
                    tab[j--] = tmp;
                }
            }
            if (l < j) QuickSortRec(tab,l, j);
            if (i < r) QuickSortRec(tab,i, r);
        }


        public int[] ShellSort(int[] tab)
        {
            int n = tab.Length;
            int h = 2;
            while (h - 1 < (n) / 2) h *= 2;
            h--;
            while (h >= 1)
            {
                for (int j = h; j < n; j++)
                {
                    int v = tab[j];
                    int i = j - h;
                    while (i >= 0 && tab[i] > v)
                    {
                        tab[i + h] = tab[i];
                        i -= h;
                    }
                    tab[i + h] = v;
                }
                h = (h + 1) / 2 - 1;
            }
            return tab;
        }

        public int[] HeapSort(int[] tab)
        {
            if (tab.Length == 0||tab.Length==1 || tab == null) return tab;
            int hl = tab.Length;
            CreateHeap(tab, hl);
            while (hl > 0)
            {
                int tmp = tab[0];
                tab[0] = tab[hl-1];
                tab[hl-1] = tmp;
                hl--;
                DownHeap(tab, hl, 0);
            }
            return tab;
        }
       
        public void CreateHeap(int[] tab,int hl)
        {
            int n = hl;
            for (int i = (n / 2); i >= 0; i--) DownHeap(tab, hl, i);
        }

        public void DownHeap(int[] tab,int hl, int i)
        {
            int v = tab[i];
            int k = 2 * i;
            while (k < hl)
            {
                if (k + 1 < hl)
                {
                    if (tab[k] < tab[k + 1]) k = k + 1;
                }
                if (v < tab[k])
                {
                    tab[i] = tab[k];
                    i = k;
                    k = 2 * i;
                }
                else break;
            }
            tab[i] = v;
        }
        
        public int[] MergeSort(int[] tab)
        {
            if (tab.Length == 0) return tab;
            MergeSortRec(tab, 0, tab.Length - 1);
            return tab;
        }

        public void MergeSortRec(int[] tab, int l, int r)
        {
            if (l == r) return;
            int m = (l + r) / 2;
            MergeSortRec(tab,l, m);
            MergeSortRec(tab, m + 1, r);
            Merge(tab, l, m, m + 1, r);
        }

        public void Merge(int[] tab, int l, int m, int m1, int r)
        {
            int l1 = l, l2 = m1;
            int count = r - l + 1;
            int[] tmptab = new int[count];
            int i = 0;
            for (; i < count && l1 <= m && l2 <= r; i++)
            {
                if (tab[l1] <= tab[l2])
                {
                    tmptab[i] = tab[l1];
                    l1++;
                }
                else
                {
                    tmptab[i] = tab[l2];
                    l2++;
                }
            }
            if (l1 > m)
                for (; i < count; i++, l2++)
                    tmptab[i] = tab[l2];
            else if(l2 > r)
                for (; i < count; i++, l1++)
                    tmptab[i] = tab[l1];
            for(i = 0;i < count;i++,l++)
            {
                tab[l] = tmptab[i];
            }
        }
    }
}
