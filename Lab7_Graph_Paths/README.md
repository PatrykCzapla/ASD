
Przedsiębiorstwo chce zbudować nową siedzibę, gdyż stara ma otwierane
okna i jest już niemodna.  Żeby móc łatwiej pozyskać nowych
pracowników zarząd zdecydował, żeby zamiast jednej siedziby zbudować
dwie siedziby w różnych miastach. Z drugiej strony jednak chodzi o to,
żeby czas przejazdu między obiema siedzibami był możliwie mały.

Dany jest graf nieskierowany reprezentujący drogi między n
miastami. Wagi krawędzi to czasy przejazdu. Dodatkowo dana jest
tablica czasów przejazdu przez każde z miast. 
Jeśli jedziemy z miasta A do miasta C przez miasto B, to sumaryczny
czas przejazdu to czas A--B + czas przejazdu przez B + czas B--C. 

Zarząd wytypował k<=n miast na lokalizację siedzib. Napisać program,
który spośród tych k miast wybierze 2 o najkrótszym czasie przejazdu
między nimi. Jeśli w proponowanym grafie nie istnieje para
proponowanych miast, miedzy którą jest ścieżka, zwrócić null.

Można założyć, że graf wejściowy jest:
 - nieskierowany,
 - wszystkie wagi są nieujemne.

Uwaga: oba etapy implementujemy jedną metodą. W pierwszym etapie
parametr buildBypass będzie ustawiany na false. W drugim na true. 

Uwaga: zwrócona ścieżka (ostatni element wyniku) to tablica kolejnych
krawędzi prowadzących od jednej siedziby do drugiej, ale wagi krawędzi w tej
tablicy NIE MAJĄ ZNACZENIA - liczy się tylko to jakie wierzchołki łączą 

Etap 1: 2p (1p za wersję bez podania ścieżki
        czyli błąd "Zwrócony null zamiast ścieżki")
Podać najlepszą parę miast i czas przejazdu między nimi lub
informację, że się nie da. W tym etapie element bypass zwracanej
krotki zawsze ustawiać na null. 

Etap 2: 2p (1p za wersję bez podania ścieżki
        czyli błąd "Zwrócony null zamiast ścieżki")
Przedsiębiorstwo ma środki aby dodatkowo sfinansować budowę
obwodnicy jednego miasta. Obwodnica zmienia czas przejazdu przez
jedno wybrane miasto na 0. 
Znaleźć: najlepszą lokalizację dla obwodnicy, czyli taką, która da
jak najmniejszy wynik końcowy oraz parę miast do budowy
siedziby i czas przejazdu (po zbudowaniu obwodnicy) między
nimi. Jeśli budowa obwodnicy nie poprawi wyniku, zwrócić miasta jak w
etapie 1 i null.

Wymagana złożoność obliczeniowa to (e oznacza liczbę krawędzi)
dla etapu I   -  k*e*log(n)
dla etapu II  -  max(k*e*log(n),k*k*n)

Wskazówka:
1) Zadanie można rozwiązać stosując jeden z algorytmów poszukiwania
   najkrótszych ścieżek w grafie. Trzeba jednak zbudować odpowiedni
   graf, który będzie zawierał wszystkie dane z zadania.
2) wymagana złożoność również jest wskazówką
