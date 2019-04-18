
W kraju kwadratów wszystkie działki są kwadratowe. Marek odziedziczył tam po dziadku pole 
(oczywiście) kwadratowe o boku długości n. Chciałby teraz podzielić je na mniejsze działki. 
Prawo geodezyjne nakazuje, aby wszystkie działki powstałe w wyniku podziału miały następujące własności:
- były kwadratami o bokach będących liczbami naturalnymi
- miały boki równoległe do boków pierwotnej działki

Dodatkowo nałożone są ograniczenia na wielkość powstałych działek. 
Ich boki mogą przyjmować jedynie wartości spośród przygotowanej wcześniej przez wydział geodezji listy.

Pomóż Markowi stwierdzić czy istnieje podział, który spełnia te warunki 
oraz jeśli tak, to zaproponuj podział, w którym liczba wynikowych działek będzie najmniejsza.

Zadanie zrealizuj przy pomocy algorytmu z powrotami.

--- Część I (1 pkt) ---
Sprawdź, czy istnieje jakikolwiek podział spełniający warunki zadania.
Zwróć 0 jeśli podział nie istnieje lub dowolną liczę dodatnią jeśli istnieje. 
Wartość parametru solution nie ma znaczenia.

--- Część II (2 pkt) ---
Znajdź rozwiązanie, w którym liczba wynikowych działek będzie najmniejsza i zwróć tę liczbę. 
Rozwiązanie zapisz w dwuwymiarowej tablicy o rozmiarze takim jak pole, 
gdzie każdy element to unikalny dodatni identyfikator nowo powstałej działki.

Uwaga: Podział na 1 działkę o rozmiarze całego pola jest również możliwy 
(o ile rozmiar ten znajduje się na podanej liście dopuszczalnych rozmiarów).

Przykład podziału pola 7x7 na 9 działek o dopuszczalnych rozmiarach {1,2,3,4}:

1111555
1111555
1111555
1111688
2223388
2223399
2224799

--- Część III (1 pkt) ---
Program przechodzi testy wydajnościowe.

Uwagi:
1) Odważni mogą od razu zabrać się za część II (część I wyjdzie "przy okazji").
2) Limity w testach wydajnościowych są około 5 razy wyższe niż czasy działania
naszego rozwiązania (w testach domowych mogą być ostrzejsze!)
