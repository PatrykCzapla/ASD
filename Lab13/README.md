Dany jest skierowany acykliczny graf reprezentujący pewne procedury w
programie.  Krawędź xy w grafie oznacza, że wynik procedury x jest
konieczny do rozpoczęcia procedury y. Innymi słowy: procedurę y można
rozpocząć dopiero po zakończeniu x.
Wyliczenie każdego w zadań zajmuje t_i>0 jednostek czasu (może być inne
dla każdego zadania). Dostępna jest nieograniczona liczba procesorów,
więc wiele zadań może wykonywać się równocześnie. Jedynym warunkiem
jest to, żeby wszyscy poprzednicy zadania zakończyli się wcześniej.
Wykonanie całego programu to wykonanie wszystkich jego procedur
składowych. 

Wymagana złożoność (dla wszystkich etapów) do O(|V|+|E|).

Etapy:
1) Wyliczyć minimalny czas wymagany do wykonania programu. (2 punkty)
2) Podać czas startu (licząc 0 jako start programu) każdego zadania, w
   układzie który zapewnia optymalny czas i startuje każde z zadań
   możliwie **późno**. (1 punkt)
3) Podać dowolną ze ścieżek krytycznych w projekcie (opis poniżej) (1 punkt) 

Ścieżką krytyczną nazywamy taką ścieżkę od zadania bez poprzedników do
zadania bez następników (czyli maksymalną w grafie zadań), taką że
wydłużenie dowolnego zadania na tej ścieżce zawsze spowoduje
wydłużenie trwania całego programu. W grafie programu może być wiele
ścieżek krytycznych. Przykład:

  ---> B(3) --> C(4) 
 /
A(2) --> D(1) --> E(1) --> F(1) --> G(3)
 \                                  ↑  ↑
  ----------------->H(4)------------+  |
                                       |
                                       |
I(6)-----------------------------------+


W powyższym grafie są trzy ścieżki krytyczne:
I-G
A-H-G
A-B-C
