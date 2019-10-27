using System;
using System.Collections.Generic;
using GraphX;
using System.Linq;

namespace CSG
{
    public class Clipper : MarshalByRefObject
    {
        /// <summary>
        /// Metoda znajdowania przecięcia odcinków s1s2 i c1c2. Użyj jej w etapie 1.
        /// Metoda wypełnia pole Distance w zwracanych wierzchołkach, czyli względną pozycję wierzchołka na odcinku.
        /// </summary>
        /// <param name="s1">Początek pierwszego odcinka</param>
        /// <param name="s2">Koniec pierwszego odcinka</param>
        /// <param name="c1">Początek drugiego odcinka</param>
        /// <param name="c2">Koniec drugiego odcinka</param>
        /// <returns>Zwraca dwa wierzchołki, które mają te same współrzędne, 
        /// ale różnią się względną pozycją na swoim odcinku (pierwszy zwracany wierzchołek leży na s1s2,
        /// a drugi na c1c2). Gdy odcinki się nie przecinają, zwracany jest null.</returns>
        public (Vertex, Vertex)? GetIntersectionPoints(Vertex s1, Vertex s2, Vertex c1, Vertex c2)
        {
            double d = (c2.Y - c1.Y) * (s2.X - s1.X) - (c2.X - c1.X) * (s2.Y - s1.Y);
            if (d == 0.0)
                return null;
            double toSource = ((c2.X - c1.X) * (s1.Y - c1.Y) - (c2.Y - c1.Y) * (s1.X - c1.X)) / d;
            double toClip = ((s2.X - s1.X) * (s1.Y - c1.Y) - (s2.Y - s1.Y) * (s1.X - c1.X)) / d;

            Vertex s = new Vertex(s1.X + toSource * (s2.X - s1.X), s1.Y + toSource * (s2.Y - s1.Y))
            {
                Distance = toSource,
                IsIntersection = true
            };
            Vertex c = new Vertex(s1.X + toSource * (s2.X - s1.X), s1.Y + toSource * (s2.Y - s1.Y))
            {
                Distance = toClip,
                IsIntersection = true
            };

            if ((0 < toSource && toSource < 1) && (0 < toClip && toClip < 1))
            {
                return (s, c);
            }
            else
                return null;
        }

        /// <summary>
        /// Metoda sprawdzająca, czy dany wierzchołek v znajduje się w danym wielokącie p. Użyj w etapach 2.
        /// </summary>
        /// <param name="v">Wierzchołek</param>
        /// <param name="p">Wielokąt</param>
        /// <returns>Prawda, jeśli wierzchołek znajduje się wewnątrz wielokąta, fałsz w przeciwnym wypadku</returns>
        public bool IsInside(LinkedListNode<Vertex> v, Polygon p)
        {
            // funkcja strzela nieskończoną prostą w lewo (ujemne X-y) od punktu v
            // jeśli na swojej drodze napotka nieparzystą liczbę boków, to znaczy, że jest w środku wielokąta p
            // w praktyce odbywa się to tak, że sprawdzamy, czy dany bok przecina Y-ową współrzędną punktu,
            // jednocześnie (używając interpolacji liniowej) czy to miejsce w odcinku, na który pada rzut punktu v na oś OY
            // jest po lewej (po ludzku: czy odcinek jest na lewo od punktu). 
            // Jeśli tak, to jest to bok przecinający się z nieskończoną prostą idącą w lewo. Inne boki nas nie interesują

            bool oddNodes = false;
            double x = v.Value.X;
            double y = v.Value.Y;

            // znów nie da się sprytnie foreachem, bo trzeba mieć dostęp także do następnego
            for (LinkedListNode<Vertex> LLvertex = p.Vertices.First; LLvertex != null; LLvertex = LLvertex.Next)
            {
                Vertex vertex;
                Vertex next;

                //if ma zapewnić, że sprawdzimy także bok łączący ostatni wierzchołek z pierwszym 
                if (LLvertex.Next == null)
                {
                    vertex = LLvertex.Value;
                    next = p.Vertices.First.Value;
                }
                else
                {
                    vertex = LLvertex.Value;
                    next = LLvertex.Next.Value;
                }

                // czy odcinek przecina Y-ową współrzędną punktu?
                // czy choć jedna współrzędna X-owa odcinka jest po lewej?
                if ((vertex.Y < y && next.Y >= y ||
                       next.Y < y && vertex.Y >= y) &&
                    (vertex.X <= x || next.X <= x))
                {
                    // jeśli tak, to czy odcinek jest na lewo od punktu?
                    oddNodes ^= vertex.X + (y - vertex.Y) / (next.Y - vertex.Y) * (next.X - vertex.X) < x;
                }
            }

            return oddNodes;
        }

        /// <summary>
        /// Metoda znajdowania punktów, gdzie przecinają się dwa wielokąty ze sobą. 
        /// Argumenty nie są modyfikowane, zmodyfikowane wersje są zwracane jako wynik.
        /// </summary>
        /// <param name="source">Pierwszy z przecinanych wielokątów</param>
        /// <param name="clip">Drugi z przecinanych wielokątów</param>
        /// <returns>Zwraca zmodyfikowane kopie wielokątów wejściowych</returns>
        public (Polygon, Polygon) MakeIntersectionPoints(Polygon source, Polygon clip)
        {
            Polygon sourceCopy = new Polygon(source);
            Polygon clipCopy = new Polygon(clip);
            LinkedListNode<Vertex> s1 = sourceCopy.Vertices.First;
            LinkedListNode<Vertex> s2 = s1.Next;
            do
            {
                LinkedListNode<Vertex> c1 = clipCopy.Vertices.First;
                LinkedListNode<Vertex> c2 = c1.Next;
                do
                {
                    (Vertex s, Vertex c)? inter = GetIntersectionPoints(s1.Value, s2.Value, c1.Value, c2.Value);
                    if (inter.HasValue)
                    {                        
                        if (((s1.Next ?? sourceCopy.Vertices.First)).Value.IsIntersection && (s1.Next ?? sourceCopy.Vertices.First).Value.Distance < inter.Value.s.Distance)
                        {
                            LinkedListNode<Vertex> tmp = s1.Next ?? sourceCopy.Vertices.First;
                            while ((tmp.Next ?? sourceCopy.Vertices.First).Value.IsIntersection && (tmp.Next ?? sourceCopy.Vertices.First).Value.Distance < inter.Value.s.Distance)
                            {
                                tmp = tmp.Next ?? sourceCopy.Vertices.First;
                            }
                            sourceCopy.Vertices.AddAfter(tmp, inter.Value.s);
                        }
                        else
                        {
                            sourceCopy.Vertices.AddAfter(s1, inter.Value.s);
                        }
                        if ((c1.Next ?? clipCopy.Vertices.First).Value.IsIntersection && (c1.Next ?? clipCopy.Vertices.First).Value.Distance < inter.Value.c.Distance)
                        {
                            LinkedListNode<Vertex> tmp = c1.Next ?? clipCopy.Vertices.First;
                            while ((tmp.Next ?? clipCopy.Vertices.First).Value.IsIntersection && (tmp.Next ?? clipCopy.Vertices.First).Value.Distance < inter.Value.c.Distance)
                            {
                                tmp = tmp.Next ?? clipCopy.Vertices.First;
                            }
                            clipCopy.Vertices.AddAfter(tmp, inter.Value.c);
                        }
                        else
                        {
                            clipCopy.Vertices.AddAfter(c1, inter.Value.c);
                        }
                        inter.Value.s.CorrespondingVertex = clipCopy.Vertices.Find(inter.Value.c);
                        inter.Value.c.CorrespondingVertex = sourceCopy.Vertices.Find(inter.Value.s);
                    }
                    c1 = c2;
                    c2 = c2.Next ?? clipCopy.Vertices.First;
                } while (c1 != clipCopy.Vertices.First);
                s1 = s2;
                s2 = s2.Next ?? sourceCopy.Vertices.First;
            } while (s1 != sourceCopy.Vertices.First);



            return (sourceCopy, clipCopy);
        }

        /// <summary>
        /// Metoda oznaczająca wierzchołki jako wejściowe lub wyjściowe.
        /// Argumenty nie są modyfikowane, zmodyfikowane wersje są zwracane jako wynik.
        /// </summary>
        /// <param name="source">Pierwszy z przecinanych wielokątów</param>
        /// <param name="clip">Drugi z przecinanych wielokątów</param>
        /// <returns>Zwraca zmodyfikowane kopie wielokątów wejściowych</returns>
        public (Polygon, Polygon) MarkEntryExitPoints(Polygon source, Polygon clip)
        {
            (Polygon sourceCopy, Polygon clipCopy) = MakeIntersectionPoints(source, clip);
            bool flag = false;
            LinkedListNode<Vertex> s1 = sourceCopy.Vertices.First;
            if (IsInside(s1, clipCopy)) flag = true;
            do
            {
                if(s1.Value.IsIntersection)
                {
                    s1.Value.IsEntry = !flag;
                    flag = !flag;
                }
                s1 = s1.Next ?? sourceCopy.Vertices.First;
            } while (s1 != sourceCopy.Vertices.First);
            s1 = clipCopy.Vertices.First;
            if (IsInside(s1, sourceCopy)) flag = true;
            else flag = false;
            do
            {
                if (s1.Value.IsIntersection)
                {
                    s1.Value.IsEntry = !flag;
                    flag = !flag;
                }
                s1 = s1.Next ?? clipCopy.Vertices.First;
            } while (s1 != clipCopy.Vertices.First);
            return (sourceCopy, clipCopy);
        }

        /// <summary>
        /// Metoda zwracająca wynik operacji logicznej na dwóch wielokątach.
        /// </summary>
        /// <param name="source">Pierwszy z przecinanych wielokątów</param>
        /// <param name="clip">Drugi z przecinanych wielokątów</param>
        /// <returns>Lista wynikowych wielokątów</returns>
        public List<Polygon> ReturnClippedPolygons(Polygon source, Polygon clip)
        {
            (Polygon sourceCopy, Polygon clipCopy) = MarkEntryExitPoints(source, clip);
            List<Polygon> result = new List<Polygon>();
            LinkedList<Vertex> intersections = new LinkedList<Vertex>();
            bool isMovingForward = true;
            foreach (Vertex v in sourceCopy.Vertices)
                if (v.IsIntersection == true)
                {
                    if (intersections.Count == 0) intersections.AddFirst(new LinkedListNode<Vertex>(v));
                    else intersections.AddAfter(intersections.Last, new LinkedListNode<Vertex>(v));
                }
            if(intersections.Count == 0)
            {
                if (IsInside(clipCopy.Vertices.First, sourceCopy))
                {
                    result.Add(clipCopy);
                    return result;
                }
                else if (IsInside(sourceCopy.Vertices.First, clip))
                {
                    result.Add(source);
                    return result;
                }
            }
            while(intersections.Count > 0)
            {
                LinkedList<Vertex> partialResult = new LinkedList<Vertex>();
                LinkedListNode<Vertex> s1 = sourceCopy.Vertices.Find(intersections.First());
                while(true)
                {
                    if (s1.Value.IsVisited == true) break;
                    s1.Value.IsVisited = true;
                    partialResult.AddLast(new Vertex(s1.Value.X, s1.Value.Y));
                    if (s1.Value.IsIntersection == true)
                    {
                        if(intersections.Contains(s1.Value)) intersections.Remove(s1.Value);
                        else intersections.Remove(s1.Value.CorrespondingVertex.Value);
                        s1 = s1.Value.CorrespondingVertex;
                        isMovingForward = s1.Value.IsEntry ? true : false;
                    }
                    if(isMovingForward == true)
                        s1 = s1.Next ?? s1.List.First;
                    else
                        s1 = s1.Previous ?? s1.List.Last;                   
                }
                result.Add(new Polygon(partialResult.ToArray()));
            }
            return result;
        }
    }
}
//ReturnClippedPolygons - wersja z HashSet
//
//public List<Polygon> ReturnClippedPolygons(Polygon source, Polygon clip)
//{
//    (Polygon sourceCopy, Polygon clipCopy) = MarkEntryExitPoints(source, clip);
//    List<Polygon> result = new List<Polygon>();
//    HashSet<Vertex> intersections = new HashSet<Vertex>();
//    bool isMovingForward = true;
//    foreach (Vertex v in sourceCopy.Vertices)
//        if (v.IsIntersection == true) intersections.Add(v);
//    if (intersections.Count == 0)
//    {
//        if (IsInside(clipCopy.Vertices.First, sourceCopy))
//        {
//            result.Add(clipCopy);
//            return result;
//        }
//        else if (IsInside(sourceCopy.Vertices.First, clip))
//        {
//            result.Add(source);
//            return result;
//        }
//    }
//    while (intersections.Count > 0)
//    {
//        LinkedList<Vertex> partialResult = new LinkedList<Vertex>();
//        LinkedListNode<Vertex> s1 = sourceCopy.Vertices.Find(intersections.First());
//        while (true)
//        {
//            if (s1.Value.IsVisited == true) break;
//            s1.Value.IsVisited = true;
//            partialResult.AddLast(new Vertex(s1.Value.X, s1.Value.Y));
//            if (s1.Value.IsIntersection == true)
//            {
//                intersections.RemoveWhere(x => x.X == s1.Value.X && x.Y == s1.Value.Y);
//                s1 = s1.Value.CorrespondingVertex;
//                isMovingForward = s1.Value.IsEntry ? true : false;
//            }
//            if (isMovingForward == true)
//                s1 = s1.Next ?? s1.List.First;
//            else
//                s1 = s1.Previous ?? s1.List.Last;

//        }
//        result.Add(new Polygon(partialResult.ToArray()));
//    }
//    return result;
//}