# Specyfikacja wejścia i wyjścia modelu AI

Aplikacja wspiera integrację modeli sztucznej inteligencji zapisanych w formacie `Open Neural Network Exchange` - `.ONNX`. Aby zapewnić poprawną współpracę z systemem, załadowany model musi spełniać specyfikację techniczną opisaną poniżej.

## Dane wejściowe

Model na wejście powinien przyjmować pojedynczy obraz USG w skali szarości, zapisany w postaci tensora o wymiarach  $(1, 680, 560, 1)$. Dane w tensorze są typu float32 a wartości w zakresie [0-1].

## Dane wyjściowe

Na wyjściu model powinien zwracać cztery heatmapy opisujące położenie czterech punktów, które są zaznaczane w celu wyliczenia szerokości nerwu wzrokowego oka. Heatmapy powinny być zwracane jako tensor o kształcie $(1, 680, 560, 4)$ zawierający wartości typu float32 w zakresie [0-1].  Wartości powinny reprezentować prawdopodobieństwo, że dany piksel jest miejscem, w którym znajduje się dany punkt. Każda heatmapa powinna opisywać inny punkt, odpowiednio:

1. Punkt wyznaczający krawędź siatkówki w osi nerwu
2. Punkt 3 mm poniżej punktu 1. w osi nerwu
3. Lewa krawędź pochewki nerwu wzrokowego na wysokości punktu 2.
4. Prawa krawędź pochewki nerwu wzrokowego na wysokości punktu 2.

Odpowiada to punktom zaznaczanym przez osobę prowadzącą badanie w celu wyliczania szerokości nerwu wzrokowego oka.