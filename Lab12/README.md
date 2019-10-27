Przecinanie figur
20 maja 2019
Zadanie polega na przecieciu (wyznaczeniu czesci wspólnej) dwóch niekoniecznie wypukłych figur ze soba.
Zakładamy, ze figury nie maja samoprzeciec. Z grubsza algorytm bedzie polegał na znajdowaniu punktów
przeciec dwóch figur, a nastepnie uzywania ich do wyznaczenia przeciecia dwóch figur.
Informacje wstepne
Wielokat przechowujemy jako liste dwukierunkowa wierzchołków. Uzywamy wbudowanej klasy
LinkedList<Vertex>. Klasa ta nie pozwala niestety na zawijanie wierzchołków (ostatni element
wskazuje na pierwszy), dlatego aby samemu dokonac takiego zawijania nalezy uzyc konstrukcji c = c.Next
?? c.List.First;. Typ wierzchołka na liscie to LinkedListNode<Vertex>.
Przypadki szczególne
Algorytm niezbyt dobrze radzi sobie z nastepujacymi przypadkami szczególnymi, dlatego nie nalezy ich
rozwazac:
• wierzchołek jednego wielokata lezy na boku drugiego wielokata,
• boki dwóch wielokatów pokrywaja sie, czyli maja wiecej niz jeden punkt przeciecia.
Za to nastepujace przypadki szczególne powinny byc rozwazane:
• wielokat jest całkowicie zawarty w drugim,
• wielokaty nie maja czesci wspólnej (wtedy nalezy zwrócic pusta liste).
Co mozna
Mozna jedynie modyfikowac metody w pliku Lab12.cs. Mozna dopisywac metody pomocnicze w klasie
Clipper. Nie mozna modyfikowac plików Vertex.cs i Polygon.cs.
Etap 1 (1 pkt)
Napisac funkcje void MakeIntersectionPoints(Polygon source, Polygon clip). Funkcja ma za zadanie
przetestowac wszystkie pary boków figur takie ze pierwszy bok pochodzi z pierwszej figury, a drugi z drugiej pod
katem przeciec. Jesli taka para boków sie przecina, nalezy wyznaczyc punkt przeciecia. Nastepnie nalezy taki
punkt wstawic w odpowiednie miejsce obu wielokatów (wstawiane punkty przeciec powinny miec ustawione
pole IsIntersection na true). W szczególnosci nalezy pamietac, ze moze sie okazac, ze pomiedzy dana
pare oryginalnych wierzchołków figury wstawimy wiecej niz jeden punkt przeciecia. Ostatecznie musza one
znajdowac sie w odpowiedniej kolejnosci. Te nalezy zachowac dzieki przechowywaniu informacji, w jakiej czesci
odcinka znajduje sie punkt przeciecia (liczba z przedziału [0, 1], pole Distance). Co wiecej, punkt powinien
znac swój odpowiednik w drugiej figurze (przyda sie w dalszej czesci zadania, pole CorrespondingVertex).
Odpowiednik to punkt przeciecia o tych samych współrzednych, ale umieszczony w drugiej figurze.
Przydatne pola w klasie Vertex:
• IsIntersection - czy dany punkt jest punktem przeciecia,
1
• Distance - w jakiej czesci odcinka znajduje sie punkt,
• CorrespondingVertex - odpowiednik w drugim wielokacie.
Dana jest juz funkcja GetIntersectionPoints(), która znajduje punkt przeciecia oraz wylicza odpowiednia
wartosc pola Distance.
Uwaga: funkcja nie powinna modyfikowac argumentów wejsciowych. Powinna zwracac nowe
wielokaty, bedace kopiami oryginalnych wielokatów z wstawionymi wierzchołkami.
Uwaga2: Do znajdowania przeciec pomiedzy bokami uzyc algorytmu naiwnego (kazdy z
kazdym) o złozonosci O(nm), gdzie n i m sa liczbami boków obu wielokatów.
Etap 2 (1 pkt)
Napisac funkcje void MarkEntryExitPoints(Polygon source, Polygon clip).
Wyobrazmy sobie, ze zaczynamy od pewnego wierzchołka pierwszej figury oraz ze ten wierzchołek jest poza
druga figura. Idziemy wzdłuz boku figury. W koncu natrafimy na punkt przeciecia. Wtedy oznaczamy go jako
punkt wejsciowy (pole IsEntry ustawiamy na true), poniewaz wchodzimy w druga figure. Gdy napotkamy
kolejny punkt przeciecia, bedzie to punkt wyjsciowy (pole IsEntry ustawiamy na false). I tak na przemian.
Pamietajmy, ze algorytm ulegnie modyfikacji, gdy zaczynamy w wierzchołku który nie jest na zewnatrz.
W tym etapie nalezy odpowiednio oznaczyc wierzchołki przeciecia w obu figurach. Moze sie zdarzyc tak, ze
wierzchołek o tych samych współrzednych bedzie w jednej figurze wejsciowym, a w drugiej wyjsciowym.
To, czy dany wierzchołek jest wejsciowy, czy nie, zalezy takze od tego, w jakiej kolejnosci przechowujemy
wierzchołki na liscie (czy zgodnie z ruchem wskazówek zegara, czy przeciwnie). My oczywiscie idziemy
naprzód listy, czyli od pierwszego wierzchołka do ostatniego.
Dana jest juz funkcja IsInside(), która stwierdza, czy dany wierzchołek znajduje sie wewnatrz danej figury.
Uwaga: funkcja nie powinna modyfikowac argumentów wejsciowych. Powinna zwracac nowe
wielokaty, bedace kopiami oryginalnych wielokatów z wstawionymi wierzchołkami z ustawionymi
odpowiednimi polami.
Uwaga2: zakładamy, ze funkcja z tego etapu na samym poczatku wywołuje funkcje z etapu 1.
Wywołanie jest juz napisane w dostarczonym kodzie.
2
Przykład
(0,0)
(0,2) (2,2)
(2,0)
(1,3)
(1,1)
(1,2)
(2,1)
(3,3)
(3,1)
Kwadrat 1
Kwadrat 2
Przesledzmy przykład. Strzałki na kwadratach oznaczaja, w która strone ida punkty na liscie. Zacznijmy w
punkcie (0, 0). Idac do przodu przejdziemy do punktu (0, 2), a nastepnie (1, 2). Punkt (1, 2) jest punktem
przeciecia i przechodzac przez niego wchodzimy w kwadrat 2. Dlatego jest to punkt wchodzacy, czyli jego
pole IsEntry jest równe true. Z podobnych powodów punkt (2, 1) jest punktem wychodzacym z punktu
widzenia kwadratu 1 (bo idac po kolei przez liste punktów opuszczamy kwadrat 2).
Inaczej sytuacja ma sie dla kwadratu 2. Zacznijmy od punktu (1, 1). Idac do przodu, docieramy do punktu
(1, 2), ale tym razem przechodzac przez niego wychodzimy z kwadratu 1, dlatego tez jest to punkt wychodzacy
(IsEntry równe false). Z analogicznych powodów punkt (2, 1) jest punktem wchodzacym.
Etap 3 (2 pkt)
Napisac funkcje List<Polygon> ReturnClippedPolygons(Polygon source, Polygon clip). W tym
etapie rozpoczynamy od dowolnego wierzchołka przeciecia jednej z figur, a nastepnie poruszamy sie wzdłuz
boków, az nie domkniemy powstałej figury. Domknieta figure wrzucamy na zwracana liste. Jezeli wierzchołek
przeciecia jest wejsciowy, nalezy poruszac sie do przodu. W przeciwnym wypadku do tyłu.
Jezeli podczas poruszania sie napotkamy kolejny wierzchołek przeciecia, nalezy przeniesc sie na druga figure
i wzdłuz jej boków kontynuowac trase. Ten nowy wierzchołek przeciecia (jego wersja w drugiej figurze)
wyznacza, czy od teraz poruszamy sie do przodu czy do tyłu.
Algorytm konczy sie, gdy przetworzymy wszystkie wierzchołki przeciecia.
Uwaga: zakładamy, ze funkcja z tego etapu na samym poczatku wywołuje funkcje z etapu 2.
Wywołanie jest juz napisane w dostarczonym kodzie.
Przykład
Spójrzmy jeszcze raz na przykładowe kwadraty. Zaczynamy od dowolnego punktu przeciecia: np. od (1, 2)
na kwadracie 1. W która strone (do przodu albo do tyłu) nalezy sie udac po kwadracie 1, aby isc po czesci
wspólnej? Poniewaz punkt (1, 2) jest punktem wchodzacym z punktu widzenia kwadratu 1, to udajemy sie
do przodu (wgłab drugiej figury). Gdy dotrzemy do kolejnego punktu przeciecia, (2, 1), wiemy, ze dalsza
droga po kwadracie 1 nie ma sensu, gdyz wyjdziemy poza czesc wspólna. Dlatego przechodzimy na kwadrat
3
2 (uzywajac pola CorrespondingVertex). Czy teraz nalezy poruszac sie do przodu czy do tyłu? Poniewaz
(2, 1) jest punktem wchodzacym dla kwadratu 2, nalezy isc naprzód, az do zamkniecia figury.
4
