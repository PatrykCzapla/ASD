using System;
using System.Collections.Generic;

namespace GraphX
{
    [Serializable]
    public class Vertex
    {
        /// <summary>
        /// Współrzędna X
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Współrzędna Y
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Referencja do punktu o tych samych współrzędnych, ale leżącego w drugim wielokącie.
        /// Dotyczy jedynie punktów przecięcia, czyli takich, dla których IsIntersection == true.
        /// Dla pozostałych punktów wartość tego pola powinna być równa null.
        /// </summary>
        [NonSerialized]
        public LinkedListNode<Vertex> CorrespondingVertex;

        /// <summary>
        /// Liczba z przedziału [0, 1]. Względna pozycja na odcinku, ustawiana przez metodę GetIntersectionPoints().
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// Czy punkt wejściowy. Dotyczy tylko punktów przecięć. Dokładniejsze informacje w opisie etapu 2 
        /// </summary>
        public bool IsEntry { get; set; }

        /// <summary>
        /// Czy punkt przecięcia między dwoma wielokątami. Takie punkty są dodawane w etapie 1
        /// </summary>
        public bool IsIntersection { get; set; }

        /// <summary>
        /// Pole które może się przydać przy etapie 3. Oznacza, czy przetworzyliśmy już dany wierzchołek.
        /// </summary>
        public bool IsVisited { get; set; }

        public Vertex(double x, double y)
        {
            X = x;
            Y = y;
            this.IsEntry = false;
            this.IsIntersection = false;
            this.IsVisited = false;
            CorrespondingVertex = null;
            Distance = 0;
        }

        public Vertex(double x, double y, bool IsEntry)
        {
            X = x;
            Y = y;
            this.IsIntersection = true;
            this.IsEntry = IsEntry;
            this.IsVisited = false;
            CorrespondingVertex = null;
            Distance = 0;
        }
        public override bool Equals(object obj)
        {
            switch(obj)
            {
                case Vertex v: return this.X == v.X && this.Y == v.Y && this.IsEntry == v.IsEntry && this.IsIntersection == v.IsIntersection;
                default: return false;
            }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(Vertex p1, Vertex p2)
        {
            if (object.ReferenceEquals(p1, null) && object.ReferenceEquals(p2, null))
                return true;
            if (object.ReferenceEquals(p1, null) || object.ReferenceEquals(p2, null))
                return false;
            return p1.X == p2.X && p1.Y == p2.Y;
        }
        public static bool operator !=(Vertex p1, Vertex p2) { return !(p1 == p2); }
        public override string ToString()
        {
            if (IsIntersection)
            {
                if (IsEntry)
                    return string.Format("({0},{1}, entry)", X, Y);
                else
                    return string.Format("({0},{1}, exit)", X, Y);
            }
            else
                return string.Format("({0},{1})", X, Y);
        }
    }
}
