# Kampania wyborcza biednego kandydata

Zbliżają się wybory prezydenckie. Pan Marchewa, jeden z kandydatów
ze znanej partii Altruistycznego Stowarzyszenia Demokratów,
celem zwiększenia poparcia wyborców zdecydował się jeździć po kraju
i zorganizować spotkania z wyborcami w poszczególnych miastach.
Panu Marchewie udało się zdobyć odrobinę partyjnych pieniędzy, ale nie na tyle
dużo, żeby zorganizować spotkania z wyborcami ze wszystkich miast w państwie.
Pan Marchewa chce spotkać się z jak największą liczbą mieszkańców
zostawiając w kieszeni jak najwięcej pieniędzy.
Musi on zdecydować które miasta odwiedzi, rozpoczynając od stolicy,
w której aktualnie się znajduje.

Założenia:
1. Każda podróż między dwoma miastami kosztuje.
2. Całkowity koszt podróży nie może przekroczyć budżetu.
3. Każde miasto można odwiedzić maksymalnie raz (może pojawić sie na liście wynikowej
   maksymalnie raz).
4. Każde miasto może mieć różną liczbę mieszkańców.
5. Po odwiedzeniu ostatniego miasta na swojej ścieżce pan Marchewa wraca do stolicy
   (to połączenie musi istnieć oraz musi wystarczyć na nie budżetu).
6. Jeżeli istnieje kilka ścieżek skutkujących identyczną liczbą odwiedzonych mieszkańców,
   należy zwrócić najtańszą z nich. Jeżeli jest kilka ścieżek o minimalnym koszcie
   można zwrócić dowolną.


## Część 1 (2 punkty)

### Zadanie
Znaleźć ciąg miast, dla których koszt podróży zmieści się w budżecie,
a suma mieszkańców odwiedzonych miast będzie maksymalna.
Jeśli dla kilku tras suma mieszkańców miast jest jest taka sama
wybieramy tą o niższym koszcie.
Pierwszym miastem musi być stolica (do stolicy musimy też wrócić,
ale nie umieszczamy jej po raz drugi w odpowiedzi).

### Przykład

Klika K3 (wierzchołki 0, 1, 2)
Koszty podróży między miastami: stałe, równe 1
Ilości mieszkańców: 100, 200, 300
Dostępny budżet: 5
Stolica to miasto o numerze 0

Wynik:
(0, true), (1, true), (2, true)

Czyli pan Marchewa zacznie w stolicy (0) i zorganizuje tam spotkanie,
po czym pojedzie do miasta 1 za 1 pieniędzy, zorganizuje tam spotkanie,
a później pojedzie do miasta 2 za 1 pieniędzy i tam również zorganizuje
spotkanie. Na końcu wróci do stolicy za 1 pieniędzy.

W sumie spotka się z 600 mieszkańcami. Cena tej ścieżki wynosi 3.

### Punktacja

Testy poprawnościowe: 1.5 punktu
Testy wydajnościowe: 0.5 punktu


## Część 2 (2 punkty)

W związku z nową ustawą o Surowym Okresie Przedwyborczym,
miasta wymagają od kandydatów opłaty za zorganizowanie spotkania
z wyborcami. Każde miasto ustala swoją opłatę.

Pan Marchewa modyfikuje zatem swoją strategię i może wybrać w których
odwiedzanych przez siebie miastach zorganizuje spotkania wyborcze,
a w których nie.

W stolicy, z której startuje, może również wybrać czy organizuje spotkanie,
czy nie.

### Zadanie
Znaleźć ciąg miast, które powinien odwiedzić pan Marchewa oraz zdecydować
w których z nich powinien on zorganizować spotkania tak, aby spotkać
się z jak największą liczbą mieszkańców, a jednocześnie zmieścić w budżecie.
Jeśli dla kilku tras suma mieszkańców miast w których zorganizowano spotkania
jest jest taka sama wybieramy tą o niższym koszcie (mieszkańców miast, przez które
przejeżdzamy, ale nie zorganizowano w nich spotkania nie uwzględniamy
w sumowaniu spotkanych mieszkańców).
Pierwszym miastem musi być stolica (do stolicy musimy też wrócić - nie umieszczamy
jej po raz drugi w odpowiedzi), ale niekoniecznie musi w niej być spotkanie.

### Przykład

Klika K3 (wierzchołki 0, 1, 2)
Koszty podróży między miastami: stałe, równe 1
Ilości mieszkańców: 100, 200, 300
Koszty organizacji spotkań: 1, 1, 1
Dostępny budżet: 5
Stolica to miasto o numerze 0

Wynik:
(0, false), (1, true), (2, true)

Czyli pan Marchewa zacznie w stolicy (0), ale nie zorganizuje tam spotkania,
po czym pojedzie do miasta 1 za 1 pieniędzy, zorganizuje tam spotkanie
za 1 pieniędzy, a później pojedzie do miasta 2 za 1 pieniędzy i tam
również zorganizuje spotkanie za 1 pieniędzy. Na końcu wróci do stolicy
za 1 pieniędzy.

W sumie spotka się z 500 mieszkańcami. Cena tej ścieżki wynosi 5
(koszt transportu między miastami jest równy w sumie 3, a koszt
organizacji spotkań w miastach 1 i 2 wynosi w sumie 2).

### Punktacja

Testy poprawnościowe: 1.5 punktu
Testy wydajnościowe: 0.5 punktu



## Wejście
* cities - mapa państwa przekazana w postaci nieskierowanego grafu, którego
  wierzchołkami są miasta, a krawędziami drogi między miastami. Waga krawędzi
  równa jest kosztowi podróży między miastami, które łączy.
  Wagi krawędzi zawsze są nieujemne.
* citiesPopulation - tablica zawierająca liczby mieszkańców w każdym z miast.
  citiesPopulation[i] jest liczbą mieszkańców w mieście i.
  Wartości zawsze są nieujemne.
* meetingCosts - tablica zawierająca koszt zorganizowania spotkania z mieszkańcami
  w każdym z miast.
  meetingCosts[i] jest kosztem zorganizowania spotkania w mieście i.
  Wartości zawsze są nieujemne.
  Dla części 1 koszt zorganizowania spotkań jest równy 0.
* budget - dostępny budżet, zawsze wartość nieujemna.
* capitalCity - numer wierzchołka odpowiadającego stolicy - miastu startowemu

Nie można modyfikować danych wejściowych.


## Wyjście
* maksymalna liczba mieszkańców, z którymi uda się spotkać panu Marchewie
* out (int, bool)[] path - tablica opisująca kolejne miasta odwiedzane przez
  pana Marchewę. Pierwszy element krotki to numer miasta, a drugi decyduje
  czy w danym mieście ma być zorganizowane spotkanie.
  Pierwszym miastem zawsze musi być stolica (w której można, ale nie trzeba
  organizować spotkania).
  Jeżeli pan Marchewa nie będzie wyjeżdżał ze stolicy, to ta lista powinna
  zawierać jedynie wpis dla stolicy. Nie wystąpią wtedy żadne koszty związane
  z podróżą.
  W części 1 drugi element krotki musi być zawsze równy `true`
