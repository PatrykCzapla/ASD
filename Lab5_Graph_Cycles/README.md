
PKS Jatne chce uruchomić nowe kursy autobusów. Zarząd przygotował plan połączeń w postaci grafu
skierowanego (wierzchołki oznaczają miejscowości, a krawędzie oznaczają bezpośrednie połączenia,
jakie należy zrealizować).

Plan tras musi spełniać trzy warunki:
1. Kurs autobusu powinien zaczynać się i kończyć w tym samym miejscu. 
2. Autobusy realizują tylko połączenia uwzględnione w planie, tj. używają tylko krawędzi z naszego grafu.
3. Ponadto ze względów ekonomicznych każde bezpośrednie (skierowane) połączenie powinno
być zrealizowane dokładnie raz (czyli każdą sjierowaną karawędż grafu należy "wykorzystać" dokładnie raz).

Firma rozważa dwa warianty biznesplanu.

1. Dostajemy kredyt na zakup dużej liczby autobusów i uruchamiamy wiele krótkich kursów.
Wtedy trasa każdego autobusu jest (skierowanym) cyklem, w którym nie powtarzają się wierzchołki.
Zadanie polega na takim zaplanowaniu tras, żeby każda była cyklem (bez powtarzania wierzchołków), przy 
spełnieniu warunków 1,2,3, lub stwierdzeniu, że jest to niemożliwe.
Jako krok pośredni, zacznijmy od metody, która znajduje jakikolwiek cykl w grafie skierowanym lub stwierdza,
że takiego cyklu nie ma.
Uwaga: sformułowanie "uruchamiamy wiele krótkich kursów" to tylko element historyjki
       - algorytmy wyznaczania najkrótszych ścieżek do niczego się w tym zadaniu nie przydadzą!

2. Nie dostajemy kredytu, więc mamy mało autobusów i uruchamiamy mało długich kursów.
W tym wariancie dopuszczamy powtarzanie się wierzchołków na trasie jednego autobusu, czyli każda trasa
jest drogą zamkniętą.
Zadanie polega na znalezieniu najmniejszej możliwej liczby tras, z których każda jest drogą zamkniętą i spełnione
są warunki 1,2,3 (lub stwierdzeniu, że takie zaplanowanie tras jest niemożliwe).


Punktacja:
- metoda znajdująca jakikolwiek cykl skierowany lub stwierdzająca, że go nie ma  -  1 pkt
- znalezienie dopuszczalnego planu połączeń w wariancie 1.                       -  1 pkt
- znalezienie optymalnego planu połączeń w wariancie 2.                          -  2 pkt


Uwagi:
- jaki problem grafowy chcemy właściwie rozwiązać?
- w rozwiązaniu wariantu 2 wykorzystaj rozwiązanie wariantu 1.
- żadna z metod nie może modyfikować grafu wejściowego!

Wartość zwracana:
- W obu wariantach należy zwrócić tablicę tablic, z których każda reprezentuje kurs jednego autobusu.
  Wariant pierwszy:
    Jeśli autobus ma wystartować w mieście 1, potem pojechać do miasta 2, potem do miasta 3, a potem wrócić do 1,
    to taki kurs jest reprezentowany przez tablicę [1,2,3] (nie wpisujemy ponownie 1).
  Wariant drugi:
    Jeśli autobus ma wystartować w mieście 1, potem pojechać do miasta 2, potem do miasta 3,
    potem wrócić do miasta 1, nastepnie pojechać dalej do miasta 4, potem do miasta 5 i zakończyć kurs w mieście 1,
    to taki kurs jest reprezentowany przez tablicę [1,2,3,1,4,5] (nie wpisujemy końcowego 1, ale pośrednie wpisujemy).
- Jeśli nie ma rozwiązania, należy zwrócić null
- można założyć, że w planie jest co najmniej jedno połączenie do zrealizowania, tj. graf ma co najmniej
  jedną krawędź
