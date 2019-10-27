using System;
using System.Collections.Generic;

namespace ASD
{
    /// <summary>
    /// Klasa drzewa prefiksowego z możliwością wyszukiwania słów w zadanej odległości edycyjnej
    /// </summary>
    public class Lab14_Trie : System.MarshalByRefObject
    {
        
        /// klasy TrieNode NIE WOLNO ZMIENIAĆ!
        private class TrieNode
        {
            public SortedDictionary<char, TrieNode> childs = new SortedDictionary<char, TrieNode>();
            public bool IsWord = false;
            public int WordCount = 0;
        }

        private TrieNode root;

        public Lab14_Trie()
        {
            root = new TrieNode();
        }

        /// <summary>
        /// Zwraca liczbę przechowywanych słów
        /// Ma działać w czasie stałym - O(1)
        /// </summary>
        public int Count { get { return root.WordCount; } }

        /// <summary>
        /// Zwraca liczbę przechowywanych słów o zadanym prefiksie
        /// Ma działać w czasie O(len(startWith))
        /// </summary>
        /// <param name="startWith">Prefiks słów do zliczenia</param>
        /// <returns>Liczba słów o zadanym prefiksie</returns>
        public int CountPrefix(string startWith)
        {
            TrieNode currentNode = root;
            int i = 0;
            for (; i < startWith.Length; i++)
            {
                if (currentNode == null) return 0;
                if (currentNode.childs.ContainsKey(startWith[i]))
                    currentNode = currentNode.childs[startWith[i]];
                else break;
            }
            if (i == startWith.Length) return currentNode.WordCount;
            return 0;
        }

        /// <summary>
        /// Dodaje słowo do słownika
        /// Ma działać w czasie O(len(newWord))
        /// </summary>
        /// <param name="newWord">Słowo do dodania</param>
        /// <returns>True jeśli słowo udało się dodać, false jeśli słowo już istniało</returns>
        public bool AddWord(string newWord)
        {
            if (Contains(newWord)) return false;
            TrieNode currentNode = root;
            root.WordCount++;
            int i = 0;
            while (i < newWord.Length && currentNode.childs.ContainsKey(newWord[i]))
            {
                currentNode = currentNode.childs[newWord[i]];
                currentNode.WordCount++;
                i++;
            }
            while (i < newWord.Length)
            {
                TrieNode newNode = new TrieNode();
                newNode.IsWord = false;
                newNode.WordCount = 1;
                currentNode.childs.Add(newWord[i], newNode);
                currentNode = newNode;
                i++;
            }
            currentNode.IsWord = true;
            return true;
        }

        /// <summary>
        /// Sprawdza czy podane słowo jest przechowywane w słowniku
        /// Ma działać w czasie O(len(word))
        /// </summary>
        /// <param name="word">Słowo do sprawdzenia</param>
        /// <returns>True jeśli słowo znajduje się w słowniku, wpp. false</returns>
        public bool Contains(string word)
        {
            TrieNode currentNode = root;
            int i = 0;
            for(; i < word.Length; i++)
            {
                if (currentNode.childs.ContainsKey(word[i]))
                    currentNode = currentNode.childs[word[i]];
                else break;
            }
            if (i == word.Length && currentNode.IsWord == true) return true;
            return false;
        }

        /// <summary>
        /// Usuwa podane słowo ze słownika
        /// Ma działać w czasie O(len(word))
        /// </summary>
        /// <param name="word">Słowo do usunięcia</param>
        /// <returns>True jeśli udało się słowo usunąć, false jeśli słowa nie było w słowniku</returns>
        public bool Remove(string word)
        {
            if (!Contains(word)) return false;
            root.WordCount--;
            TrieNode currentNode = root;
            TrieNode previousNode = root;
            int i = 0;
            int toDelete = 0;
            for (; i < word.Length; i++)
            {
                if (currentNode.IsWord == true || currentNode.WordCount != 1)
                { 
                    previousNode = currentNode;
                    toDelete = i;
                }
                currentNode = currentNode.childs[word[i]];
                currentNode.WordCount--;                
            }
            if (currentNode.WordCount == 0)
                previousNode.childs.Remove(word[toDelete]);
            else currentNode.IsWord = false;
            return true;
        }

        /// <summary>
        /// Zwraca wszystkie słowa o podanym prefiksie. 
        /// Dla pustego prefiksu zwraca wszystkie słowa ze słownika.
        /// Wynik jest w porządku alfabetycznym.
        /// Ma działać w czasie O(liczba węzłów w drzewie)
        /// </summary>
        /// <param name="startWith">Prefiks</param>
        /// <returns>Wyliczenie zawierające wszystkie słowa ze słownika o podanym prefiksie</returns>
        public List<string> AllWords(string startWith = "")
        {
            TrieNode currentNode = root;
            List<string> result = new List<string>();
            int i = 0;
            for (; i < startWith.Length; i++)
            {
                if (currentNode == null) return result;
                if (currentNode.childs.ContainsKey(startWith[i]))
                    currentNode = currentNode.childs[startWith[i]];
                else break;
            }            
            if (i == startWith.Length)
            {
                if (currentNode.IsWord == true) result.Add(startWith);
                foreach (char child in currentNode.childs.Keys)
                    result.AddRange(AllWords(startWith + child));
            }                        
            return result;
        }

        /// <summary>
        /// Wyszukuje w słowniku wszystkie słowa w podanej odległości edycyjnej od zadanego słowa
        /// Wynik jest w porządku alfabetycznym ze względu na słowa (a nie na odległość).
        /// Ma działać optymalnie - tj. niedozwolone jest wyszukanie wszystkich słów i sprawdzenie ich odległości
        /// Należy przeszukując drzewo odpowiednio odrzucać niektóre z gałęzi.
        /// Złożoność pesymistyczna (gdy wszystkie słowa w słowniku mieszczą się w zadanej odległości)
        /// O(len(word) * (liczba węzłów w drzewie))
        /// </summary>
        /// <param name="word">Słowo</param>
        /// <param name="distance">Odległość edycyjna</param>
        /// <returns>Lista zawierająca pary (słowo, odległość) spełniające warunek odległości edycyjnej</returns>
        public List<(string, int)> Search(string word, int distance = 1)
        {
            //wersja właściwa
            List<(string, int)> result = new List<(string, int)>();
            List<(string, int)> res = new List<(string, int)>();
            int[,] tab = new int[word.Length + distance + 1, word.Length + 1];
            for (int i = 0; i <= word.Length; i++) tab[0, i] = i;
            for (int i = 1; i <= word.Length + distance; i++) tab[i, 0] = i;
            foreach(var child in root.childs)
                SearchRec(word, child.Key.ToString(), distance, child.Value, 1, tab, ref result, ref res);
            return result;
        }

        private void SearchRec(string word, string currentWord, int distance, TrieNode currentNode, int level, int[,] tab, ref List<(string, int)> result, ref List<(string, int)> res)
        {
            if (level > word.Length + distance) return;
            for (int i = 1; i <= word.Length; i++)
            {
                if (currentWord[level - 1] == word[i - 1]) tab[level, i] = Math.Min(Math.Min(tab[level - 1, i - 1], tab[level, i - 1] + 1), tab[level - 1, i] + 1);
                else tab[level, i] = Math.Min(Math.Min(tab[level - 1, i - 1] + 1, tab[level, i - 1] + 1), tab[level - 1, i] + 1);
            } 
            if (currentNode.IsWord == true && tab[level, word.Length] <= distance) result.Add((currentWord, tab[level, word.Length]));
            res.Add((currentWord, tab[level, word.Length]));
            foreach (var child in currentNode.childs)               
                SearchRec(word, currentWord + child.Key, distance, child.Value, level + 1, tab, ref result, ref res);         
        }
    }
}
////Search - wersja naiwna
//List<(string, int)> result = new List<(string, int)>();
//List<string> words = AllWords();
//foreach(string currentWord in words)
//{
//    int[,] tab = new int[word.Length + 1, currentWord.Length + 1];
//    for(int i = 0; i <= word.Length; i++) tab[i, 0] = i;
//    for(int i = 0; i <= currentWord.Length; i++) tab[0, i] = i;
//    for(int i = 1; i <= word.Length; i++)
//    {
//        for(int j = 1; j <= currentWord.Length; j++)
//        {
//            if (currentWord[j - 1] == word[i - 1]) tab[i, j] = Math.Min(Math.Min(tab[i - 1, j - 1], tab[i, j - 1] + 1), tab[i - 1, j] + 1);
//            else tab[i, j] = Math.Min(Math.Min(tab[i - 1, j - 1] + 1, tab[i, j - 1] + 1), tab[i - 1, j] + 1);
//        }
//    }
//    if(tab[word.Length, currentWord.Length] <= distance)
//        result.Add((currentWord, tab[word.Length, currentWord.Length]));
//}            
//return result;