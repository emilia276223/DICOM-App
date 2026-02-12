🇵🇱 Polski | [🇬🇧 English](README/README.en.md)

# Aplikacja do przeglądania i edycji obrazów DICOM z możliwością podłączenia pluginów AI

Aplikacja powstała w ramach pracy inżynierskiej na Uniwersytecie Wrocławskim. Została stworzona w celu ułatwienia i automatyzacji pomiarów szerokości nerwu wzrokowego.

## Instalacja aplikacji

### Wymagania systemowe

Aplikacja działa zarówno na systemie Windows jak i Linux.

Aplikacja wymaga zainstalowanej platformy `.NET 9.0`. Pakiet `.NET 9.0` można pobrać ze strony `dotnet.microsoft.com/en-us/download/dotnet/9.0`. Po zainstalowaniu `.NET` będzie można uruchomić aplikację.

### Instalacja poprzez pobranie pakietu

Pakiet aplikacji jest dostępny do pobrania na stronie `github.com/emilia276223/DICOM-App/releases/tag/v1.0.0`. Należy pobrać plik `publish.zip` a następnie go rozpakować. Plik `DICOMApp.exe` będzie znajdował się wewnątrz katalogu publish.
Jeśli platforma `.NET 9.0` nie jest zainstalowana, podczas pierwszej próby uruchomienia aplikacji użytkownik zostanie przekierowany do strony umożliwiającej jej pobranie.

### Instalacja i uruchomienie na podstawie kodu źródłowego aplikacji

Kod źródłowy aplikacji jest dostępny na stronie `github.com/emilia276223/DICOM-App`. Można pobrać .ZIP kodu źródłowego z tej strony lub pobrać repozytorium poleceniem:
```bash
git clone https://github.com/emilia276223/DICOM-App
cd DICOM-App
```
A następnie uruchomić aplikację poleceniem:
```
dotnet run
```
Aplikacja wymaga zainstalowanej platformy `.NET 9.0`, którą można pobrać ze strony `dotnet.microsoft.com/en-us/download/dotnet/9.0`.


## Funkcjonalności aplikacji

1. Otwieranie plików DICOM
2. Zaznaczanie na nich punktów
3. Zapisywanie do bazy danych wykonanych zaznaczeń
4. Włączenie powiększenia obrazu i na nim zaznaczanie punktów
5. Eksport zanonimizowanych danych z bazy danych na temat zaznaczeń
6. Zobaczenie przebiegu zmian wykonanych pomiarów dla wybranego pacjenta
7. Podłączenie modelu AI, który będzie automatycznie wykonywał zaznaczenia

[Podręcznik użytkownika](README/README.user_guide_pl.md)

## Dokumentacja
Dokumentacja jest dostępna na stronie:
https://emilia276223.github.io/DICOM-App/
