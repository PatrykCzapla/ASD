Lab04: Podatki dla króla.

Poborca podatkowy wyrusza na swoim osiołku, aby zbierać daniny dla króla. Ma za zadanie odwiedzić
wszystkie wioski, znajdujące się przy głównej drodze - w ostatniej z nich jest skarbiec, gdzie
zostaną zdeponowane zebrane pieniądze.
Problemem jest to, że długa podróż jest wyczerpująca dla osiołka, który, pozbawiony stałego dopływu
pożywienia, może paść z wyczerpania. Oczywiście misja jest wtedy nieudana, gdyż pieniądze trzeba
dowieźć do ostatniej wioski.

Na szczęście prawo stanowi, że każda wioska może zapłacić daninę w jednej z dwóch postaci:
pieniędzy lub marchewek (pożywienia dla osiołka). Po przybyciu do wioski, poborca może zadecydować,
czy chce zabrać wszystkie pieniądze czy wszystkie marchewki. Nie ma możliwości pobrania daniny
w postaci mieszanej.

Zadaniem jest zaplanować plan poboru podatków, czyli to, w których wioskach należy pobierać
pieniądze, a w których marchewki w taki sposób, aby zmaksymalizować ilość zebranych pieniędzy.
Jest jeszcze jedno dodatkowe ograniczenie: poziom zapełnienia żołądka osiołka na końcu nie 
może być niższy niż startowy poziom zapełnienia.
Osiołek ma ograniczoną pojemność żołądka, oznaczaną jako maxCarrots. Jeśli pobrane
marchewki spowodują przekroczenie tego limitu, nadmiar marchewek jest odrzucany.

Dane wejściowe (wszystkie tablice przechowują liczby całkowite, nieujemne)
- startingCarrots - początkowy poziom zapełnienia żołądka osiołka, wyrażony w marchewkach.
Końcowy poziom zapełnienia żołądka nie może być niższy niż startowy.
- maxCarrots - maksymalny poziom zapełnienia żołądka
- dist[] - tablica, informująca o odległościach między siąsiednimi wioskami wzdłuż drogi. Wartość
dist[i] oznacza odległość między wioskami (i-1) a i. Pole dist[0] zawsze wynosi 0. Odległość jest
wyrażona w marchewkach.
- carrots[] - liczba marchewek, które można pobrać w kolejnych wioskach
- money[] - ilość pieniędzy, które można pobrać w kolejnych wioskach

Dane wyjściowe:
- zwracamy maksymalną ilość pieniędzy, którą można zdobyć, dojeżdżając do ostatniej wioski (łącznie
z tym, co zbierzemy w ostatniej wiosce). Jeśli niemożliwe jest dotarcie do ostatniej wioski, należy
zwrócić -1.
- jako parametr wyjściowy zwracamy tablicę plan, której kolejne pola opisują optymalne akcje w
poszczególnych wioskach: zabranie pieniędzy (TakeMoney) lub zabranie marchewek (TakeCarrots). Akcja
pusta nigdy nie powinna znaleźć się w rozwiązaniu. Jeśli niemożliwe jest dotarcie do ostatniej
wioski, należy zwrócić null.

Uwaga:
- Osiołek w każdym momencie ma pewien poziom zapełnienia żołądka. Przebycie odległości d zmniejsza
ten poziom o d. Jeśli poziom zapełnienia żołądka kiedykolwiek spadnie poniżej 0, osiołek padnie z
wyczerpania i misja zakończy się fiaskiem (poziom 0 jest dopuszczalny)
- Oczekiwana złożoność obliczeniowa i pamięciowa to O(n * maxCarrots), gdzie n jest liczbą
miejscowości.

Wskazówka:
- dla każdego (i,j) znajdź największą ilość pieniędzy, które można zebrać, opuszczając miasto
i przy stanie zapełnienia żołądka osiołka j.

Przykład:
dist	0 1 1
money	2 2 3
carrots	5 3 1

startingCarrots = 0
maxCarrots = 3

Rozważmy następujący ciąg akcji TC, TM, TM.
Początkowy poziom zapełnienia żołądka to 0, na starcie ilość pieniędzy to też 0.
W pierwszej wiosce pobieramy marchewki (TC). Jest ich 5, ale pojemność żołądka wynosi 3, czyli dwie
z nich zostaną odrzucone.
Pierwszą wioskę opuszczamy z poziomem zapełnienia 3 i bez pieniędzy.
Ponieważ odległość między pierwszą a drugą wioską wynosi 1, do drugiej wioski dojeżdżamy z poziomem
zapełnienia żołądka 2. Pobieramy tam pieniądze (TM), zatem drugą wioskę opuszczamy z poziomem
zapełnienia 2 i 2 workami pieniędzy. Dojeżdżamy do trzeciej wioski, nasz poziom zapełnienia żołądka
to 1. Pobieramy 3 worki pieniędzy (TC) i kończymy podróż z pięcioma workami.

Punktacja:
3 pkt - znalezienie optymalnej ilości pieniędzy
1 pkt - znalezienie optymalnego planu
