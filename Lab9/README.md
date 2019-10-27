# Laboratorium 9

## Opis

Na szkolnych półkoloniach każdego dnia organizowane są różne aktywności grupowe.
Przed rozpoczęciem ferii, nauczyciele muszą przygotować listę chętnych, aby wiedzieć
ile osób przyjdzie na dane zajęcia. Każdy z uczniów spisał już swoje preferencje,
umieszczając na ankiecie listę ulubionych zajęć. Nauczyciele chcą teraz tak przydzielić
osoby do danych grup zajęciowych, aby poziom satysfakcji uczestników był jak największy.

### Część 1.
Napisz metodę `FindBestDistribution`, która obliczy najlepsze rozdystrybuowanie dostępnych
miejsc na zajęciach między uczestników.

Uwaga: w tej części parametr isSportActivity jest zawsze równy null

### Część 2.
Zajęcia sportowe nie mogą odbyć się w niepełnym składzie (liczba uczestników musi być równa limitowi miejsc).

Jeśli parametr `isSportActivity` metody `FindBestDistribution` nie jest równy null to znaczy, że chcemy
zbadać czy mogą odbyć się wszystkie zajęcia sportowe.
Jako poziom satysfakcji zwróć 1 jeśli jest to możliwe albo 0 jeśli nie jest możliwe.

Uwaga: w tej części przydzielaj uczestników jedynie do zajęć sportowych, pozostałe zajęcia pomijaj.

## Założenia

1. Każde z zajęć ma swój maksymalny limit uczestników.
2. Każdy uczestnik ma przygotowany spis preferowanych przez siebie zajęć.
3. Każdy uczestnik może brać udział w maksymalnie jednych zajęciach.
4. Uczestnik będzie zadowolony tylko wtedy, gdy zostanie zapisany na jedno z preferowanych przez niego zajęć.
5. Jeżeli jakaś osoba nie może zostać zapisana na żadne z preferowanych zajęć, nie zapisujemy jej na siłę do innej grupy.

## Implementacja

Metoda `FindBestDistribution` przyjmuje na wejściu:
  * `limits`, listę k limitów osób na zajęciach (lista indeksowana zajęciami od 0 do k-1)
  * `preferences`, listę n zbiorów, zawierających indeksy preferowanych zajęć (lista zbiorów indeksowana osobami od 0 do n-1)
  * `isSportActivity`, listę k booli, mówiących o tym, które z zajęć są zajęciami sportowymi
a zwraca krotkę zawierającą:
  * `satisfactionLevel`, ogólny poziom satysfakcji uczestników (liczba zadowolonych osób).
  * `bestDistribution`, n-elementową listę będącą najlepszą dystrybucją zajęć między uczestników

* Lista `bestDistribution` na i-tej pozycji powinna zawierać indeks grupy zajęciowej przypisanej i-tej osobie.
* Brak przypisanej grupy powinien zostać oznaczony przez -1.

## Przykłady

### Zadanie 1
#### Przykład 1
Dane:
     limits = [2, 2]
preferences = [{0}, {1}, {0, 1}]

Przykładowy wynik:
satisfactionLevel = 3
 bestDistribution = [0, 1, 0] 
       lub 
 bestDistribution = [0, 1, 1] 

#### Przykład 2
Dane:
     limits = [2, 3]
preferences = [{0, 1}, {0}, {1}, {1}, {0, 1}, {0}]

Przykładowy wynik:
satisfactionLevel = 5
 bestDistribution = [1, 0, 1, -1, 1, 0]

### Zadanie 2
#### Przykład 1
Dane:
         limits = [2, 2]
    preferences = [{0}, {1}, {0, 1}]
isSportActivity = [true, false]

Przykładowy wynik:
satisfactionLevel = 1
 bestDistribution = [0, -1, 0]

#### Przykład 2
Dane:
         limits = [3, 2]
    preferences = [{0}, {1}, {0, 1}]
isSportActivity = [true, false]

Przykładowy wynik:
satisfactionLevel = 0
 bestDistribution = null

## Punktacja

Część 1. -- Zwracanie poprawnego poziomu satysfakcji oraz listy `bestDistribution`   (3 pkt.)
Część 2. -- Sprawdzenie, czy wszystkie zajęcia sportowe mogą się odbyć
            (i zwracanie listy `bestDistribution`, jeśli to możliwe)                 (1 pkt.)
