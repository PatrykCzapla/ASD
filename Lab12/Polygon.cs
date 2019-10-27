using System;
using System.Collections.Generic;
using System.Text;

namespace GraphX
{
    [Serializable]
    public class Polygon
    {
        /// <summary>
        /// Lista dwukierunkowa wierzchołków w wielokącie
        /// </summary>
        public LinkedList<Vertex> Vertices { get; set; }

        /// <summary>
        /// Pole które może się przydać przy etapie 3. Wskazuje na wierzchołek, od którego powinniśmy zacząć tworzyć kolejny wynikowy wielokąt.
        /// </summary>
        [NonSerialized]
        public LinkedListNode<Vertex> LastUnprocessed;// { get; set; }

        public Polygon()
        {
            Vertices = new LinkedList<Vertex>();
            LastUnprocessed = null;
        }

        public Polygon(Vertex[] vertices)
        {
            LastUnprocessed = null;
            Vertices = new LinkedList<Vertex>();
            for (int i = 0; i < vertices.Length; i++)
            {
                Vertices.AddLast(vertices[i]);
            }
        }

        public Polygon(Polygon p)
        {
            LastUnprocessed = null;
            Vertices = new LinkedList<Vertex>();
            foreach (Vertex v in p.Vertices)
            {
                Vertices.AddLast(new Vertex(v.X, v.Y));
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Vertex v in Vertices)
            {
                sb.AppendFormat("{0}, ", v.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Metoda używana przez testy. Nie przyda Ci się, nie ruszaj jej.
        /// </summary>
        public bool IsTheSame(Polygon p, bool isEntryChecked, out string message)
        {
            bool[] foundInMine = new bool[Vertices.Count];
            bool[] foundInTheirs = new bool[p.Vertices.Count];
            int foundInMineTrueCount = 0;
            int foundInTheirsTrueCount = 0;

            StringBuilder messageBuilder = new StringBuilder();

            int i = 0;
            foreach (Vertex myVertex in Vertices)
            {
                int j = 0;
                foreach (Vertex theirVertex in p.Vertices)
                {
                    if (!foundInMine[i] && !foundInTheirs[j])
                    {
                        if (Math.Abs(myVertex.X - theirVertex.X) < 1e-5 && Math.Abs(myVertex.Y - theirVertex.Y) < 1e-5)
                        {
                            if (!isEntryChecked ||
                                (!myVertex.IsIntersection && !theirVertex.IsIntersection) ||
                                (myVertex.IsIntersection == theirVertex.IsIntersection && myVertex.IsEntry == theirVertex.IsEntry))
                            {
                                foundInMine[i] = true;
                                foundInTheirs[j] = true;
                                foundInMineTrueCount++;
                                foundInTheirsTrueCount++;
                            }
                        }
                    }
                    j++;
                }
                i++;
            }

            if (foundInTheirsTrueCount != p.Vertices.Count)
            {
                messageBuilder.Append("Not expected vertices in your solution: ");
                i = 0;
                foreach (Vertex theirVertex in p.Vertices)
                {
                    if (!foundInTheirs[i])
                    {
                        messageBuilder.Append(theirVertex);
                        messageBuilder.Append(", ");
                    }
                    i++;
                }
                messageBuilder.Append(" ");
            }

            if (foundInMineTrueCount != Vertices.Count)
            {
                messageBuilder.Append("Expected vertices not in your solution: ");
                i = 0;
                foreach (Vertex myVertex in Vertices)
                {
                    if (!foundInMine[i])
                    {
                        messageBuilder.Append(myVertex);
                        messageBuilder.Append(", ");
                    }
                    i++;
                }
            }

            if (foundInMineTrueCount != Vertices.Count || foundInTheirsTrueCount != p.Vertices.Count)
            {
                message = messageBuilder.ToString();
                return false;
            }

            var s1 = Vertices.First;
            var c1 = p.Vertices.First;
            bool isGoodOrder = true;
            for (i = 0; i < Vertices.Count; i++)
            {
                if(i==0)
                { 
                    for (int j = 0; j < p.Vertices.Count; j++)
                    {
                        if(s1.Value == c1.Value)
                        {
                            c1 = c1.Next ?? c1.List.First;
                            break;
                        }
                        c1 = c1.Next ?? c1.List.First;
                    }
                }
                else
                {
                    if (s1.Value != c1.Value)
                    {
                        isGoodOrder = false;
                        break;
                    }
                    c1 = c1.Next ?? c1.List.First;
                }
                s1 = s1.Next ?? s1.List.First;
            }

            if(!isGoodOrder)
            {
                s1 = Vertices.First;
                c1 = p.Vertices.First;
                isGoodOrder = true;
                for (i = 0; i < Vertices.Count; i++)
                {
                    if (i == 0)
                    {
                        for (int j = 0; j < p.Vertices.Count; j++)
                        {
                            if (s1.Value == c1.Value)
                            {
                                c1 = c1.Previous ?? c1.List.Last;
                                break;
                            }
                            c1 = c1.Previous ?? c1.List.Last;
                        }
                    }
                    else
                    {
                        if (s1.Value != c1.Value)
                        {
                            messageBuilder.Append("Vertices are good, but their order is not proper.");
                            isGoodOrder = false;
                            break;
                        }
                        c1 = c1.Previous ?? c1.List.Last;
                    }
                    s1 = s1.Next ?? s1.List.First;
                }
            }

            if(isGoodOrder)
            {
                message = "";
                return true;
            }

            message = messageBuilder.ToString();
            return false;
        }
    }
}
