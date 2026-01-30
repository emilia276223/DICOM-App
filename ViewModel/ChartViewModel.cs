using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DICOMApp.ViewModel
{
    /// <summary>
    /// Klasa modelująca wykres, przechowuje dane wykresu
    /// Implementuje interfejs IChartViewModel
    /// </summary>
    public partial class ChartViewModel : ViewModelBase
    {
        /// <summary>
        /// Dane wykresu
        /// </summary>
        [ObservableProperty] ISeries[] _series;

        /// <summary>
        /// Wygląd osi wykresu
        /// </summary>
        [ObservableProperty] private Axis[] _xAxes;

        /// <summary>
        /// Czy wykres jest widoczny
        /// </summary>
        [ObservableProperty] private bool _isVisible;

        /// <summary>
        /// Konstruktor obiektu
        /// </summary>
        public ChartViewModel()
        {
            SetupXAxis();
            SetupExampleChart();
            _isVisible = false;
        }

        /// <summary>
        /// Metoda zwracająca datę z napisu
        /// </summary>
        /// <remarks>
        /// Metoda działa dla napisów postaci:
        ///     * yyyy.mm.dd
        ///     * yyyymmdd
        /// </remarks>
        /// <param name="date">napis przedstawiający date</param>
        /// <returns>Obiekt DateTime przedstawiający datę</returns>
        private DateTime GetDateFromString(string date)
        {
            try
            {
                return DateTime.Parse(date);
            }
            catch { };

            int year, month, day;
            var res = new string[3];
            if (date.Contains("."))
            {
                var res2 = date.Split(".");
                Debug.WriteLine(res2.ToString());
                Debug.WriteLine(res2.Length);
                if (res2.Length != 3)
                {
                    return new DateTime(2000, 1, 1);
                }
                res = res2;
            }
            else
            {
                if(date.Length != 8)
                {
                    return new DateTime(2000, 1, 1);
                }
                res[0] = date.Substring(0, 4);
                res[1] = date.Substring(4, 2);
                res[2] = date.Substring(6, 2);
            }

            try
            {
                // zrobienie liczb z napisów
                year = int.Parse(res[0]);
                month = int.Parse(res[1]);
                day = int.Parse(res[2]);
                return new DateTime(year, month, day);
            }
            catch
            {
                return new DateTime(2000, 1, 1);
            }
        }

        /// <summary>
        /// Metoda ustawiająca przykładowy wykres
        /// </summary>
        private void SetupExampleChart()
        {
            var data = new List<(string, double)>
            {
                ("20000101", 3),
                ("20000103", 6),
                ("20000105", 4)
            };
            var points = new DateTimePoint[data.Count];
            // przekształcenie danych
            var i = 0;
            foreach (var (date, val) in data)
            {
                points[i] = new DateTimePoint()
                {
                    Value = val,
                    DateTime = GetDateFromString(date)
                };
                i++;
            }

            Series = GetLineSeries(points);
        }

        /// <summary>
        /// Metoda ustawiająca oś X wykresu
        /// </summary>
        private void SetupXAxis()
        {
            XAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => new DateTime((long)value).ToString("dd.MM.yyyy"),
                    LabelsRotation = 15,
                    UnitWidth = TimeSpan.FromDays(1).Ticks // szerokość jednostki to 1 dzień
                }
            };
        }

        /// <summary>
        /// Metoda ustawiająca linię wykresu (dane)
        /// </summary>
        /// <param name="points">Punkty, które mają zostać zaznaczone na wykresie</param>
        /// <returns>ISeries[] - obiekt, który można umieścić na wykresie</returns>
        private ISeries[] GetLineSeries(DateTimePoint[] points)
        {
            return new ISeries[]
            {
                new LineSeries<DateTimePoint>
                {
                    Values = points,
                    Name = "Szerokość nerwu wzrokowego",
                    Fill = null, // brak wypełnienia pod linią
                    LineSmoothness = 0, // nie chcemy łuków
                    GeometrySize = 5 // wielkość punktów
                }
            };
        }

        /// <summary>
        /// Aktualizacja wykresu na podstawie podanych danych
        /// </summary>
        /// <param name="data">Lista wierszy danych (data, wartość) - data jako napis, wartość jako liczba double</param>
        public void UpdateChart(List<(string, double)> data)
        {
            if(data.Count < 1)
            {
                IsVisible = false;
                return;
            }

            // jak będzie to robimy widoczny
            IsVisible = true;

            // sortujemy dane, zeby sie ladnie wyswietlaly
            data.Sort();

            var points = new DateTimePoint[data.Count];
            // przekształcenie danych
            var i = 0;
            foreach(var (date, val) in data)
            {
                points[i] = new DateTimePoint()
                {
                    Value = val,
                    DateTime = GetDateFromString(date)
                };
                i++;
            }

            Series = GetLineSeries(points);

            // Konfiguracja osi X, aby poprawnie wyświetlała daty
            
        }

    }
}
