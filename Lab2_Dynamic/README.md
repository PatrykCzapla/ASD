
Dany jest pewien zbiór napisów (można przejąć, że są to ciągi kodujące).
Dany jest również jeden dodatkowy napis (nazwijmy go: tekst).

Zadanie.

1) Obliczyć na ile sposobów podany tekst może być zakodowany przy pomocy
   zadanego zbioru ciągów kodujących
   Wymagana złożoność czasowa nie gorsza niż N*K
   (gdzie N oznacza długość tekstu, a K liczbę ciągów kodujących)
2) Podać wszystkie sposoby takiego kodowania
   Wymagana złożoność czasowa: max(N*K,sumaryczna_liczba_użytych_kodów)

Uwaga: Przy wyznaczaniu złożoności przyjmujemy, że porównanie ciągu kodującego
z fragmentem tekstu (a także ewentualnie inne potrzebne operacje na napisach)
mają koszt stały (niezależny od długości napisów).

Przykład 1.
Ciągi kodujące: 'a' , 'abc' , 'ab' , 'ca'
Tekst: 'abca'
Odpowiedź:
możliwe są 2 sposoby zakodowania tekstu
('abc','a')    // zapisane jako tablica int[2]{1,0}
('ab','ca')    // zapisane jako tablica int[2]{2,3}

Przykład 2.
Ciągi kodujące: 'a' , 'b' , 'c' , 'ab' , 'ca' , 'abca'
Tekst: 'abca'
Odpowiedź:
możliwych jest 5 sposobów zakodowania tekstu
('a','b','c','a')  // tablica int[4]{0,1,2,0}
('a','b','ca')     // tablica int[3]{0,1,4}
('ab','c','a')     // tablica int[3]{3,2,0}
('ab','ca')        // tablica int[2]{3,4}
('abca')           // tablica int[1]{5}

Innymi słowy należy policzyć na ile sposobów tekst można przedstawić
jako konkatenację ciągów kodujących.

Uwagi:
1) Zakładamy, że każdy z ciągów kodujących jest inny, jednak może się zdarzyć,
   że ciąg kodujący można zakodować przy pomocy pozostałych ciągów kodujących.

Wskazówki
1) Do rozwiązania pierwszej części zadania wystarczy 1-wymiarowa tablica pomocnicza
   (tab[j] oznacza na ile sposobów można zakodować początkowy fragment 
   tekstu długości j+1)
2) Organizacja pętli wynika z żądanej złożoności

Punktacja
- wyznaczenie liczby kodowań                  -  2.0 pkt
- wyznaczenie wszystkich sposobów kodowania   -  2.0 pkt
Podana punktacja obowiązuje dla rozwiązań spełniających wymagania dotyczące złożoności
