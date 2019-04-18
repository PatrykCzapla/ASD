
using System;
using System.Collections.Generic;

namespace ASD
{

public class CodesCounting : MarshalByRefObject
    {

    public int CountCodes(string text, string[] codes, out int[][] solutions )
        {
            int length = text.Length;
            int length_codes = codes.Length;
            int[] tab = new int[length];
            List<List<int>>[] seq = new List<List<int>>[length];
            for (int i = 0; i < length; i++)
                seq[i] = new List<List<int>>();
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length_codes; j++)
                {
                    int len = codes[j].Length;
                    if (len > i+1) continue;
                    if (len == i+1 && codes[j]== text.Substring(0, i + 1))
                    {
                        tab[i]++;
                        List<int> kod = new List<int>();
                        kod.Add(j);
                        seq[i].Add(kod);                    
                    }
                    else if(len <= i && text.Substring(i - len + 1, len)==codes[j] && tab[i-len]!=0)
                    {                        
                        tab[i] += tab[i - len];        
                        foreach(var elem in seq[i - len])
                        {
                            var kod = new List<int>(elem);
                            kod.Add(j);
                            seq[i].Add(kod);
                        }
                        
                    }
                }
            }
            int k = 0;
            solutions = new int[seq[length - 1].Count][];
            foreach(var elem in seq[length-1])
            {
                solutions[k++] = elem.ToArray();                
            }
            return tab[length-1];
        }

    }

}
