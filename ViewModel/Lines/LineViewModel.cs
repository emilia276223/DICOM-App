using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using DICOMApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;

namespace DICOMApp.ViewModel.Lines
{
    /// <summary>
    /// Klasa modelująca linie z brzegami
    /// </summary>
    public partial class LineWithCapsViewModel : LineViewModelBase, ILineViewModel
    {
        /// <summary>
        /// Punkt początkowy i końcowy linii
        /// </summary>
        [ObservableProperty] private Point _startPoint, _endingPoint;

        /// <summary>
        /// Punkty wyznaczające początki i końce brzegów linii
        /// </summary>
        [ObservableProperty] private Point _capStart1, _capStart2, _capEnd1, _capEnd2;


        // czy linia jest rozpoczęta
        private bool _isStartSet = false;

        // długość wyświetlanych brzegów linii
        private int _capLength = 5;

        /// <summary>
        /// Konstruktor klasy
        /// </summary>
        /// <remarks>
        /// Na starcie linia jest niewidoczna i nie ma ustawionych brzegów
        /// Jest ustawiona na punkt (0,0)
        /// </remarks>
        /// <param name="color">Kolor, jaki będzie miała linia</param>
        public LineWithCapsViewModel(string color) 
        {
            // ustawienie początku i końca linii
            StartPoint = new Point(0,0);
            EndingPoint = new Point(0,0);

            // na starcie linia nie jest widoczna
            IsVisible = false;

            // zapisanie koloru linii
            Color = color;

            // na początku linia ma długość 0
            PhysicalLength = 0;
        }

        /// <summary>
        /// Metoda ustawiająca początek i koniec linii
        /// </summary>
        /// <param name="start">Punkt początkowy linii</param>
        /// <param name="end">Punkt końcowy linii</param>
        public void SetLine(Point start, Point end)
        {
            _isStartSet = false;
            IsVisible = true;
            StartPoint = start;
            EndingPoint = end;
            UpdateLength(StartPoint, EndingPoint);
            UpdateCaps();
        }

        /// <summary>
        /// Metoda ustawiająca końcówkę linii
        /// </summary>
        /// <param name="p">Punkt, w którym linia ma się kończyć</param>
        public void UpdateLine(Point p)
        {
            if (_isStartSet)
            {
                EndingPoint = p;
                IsVisible = true;
                UpdateLength(StartPoint, EndingPoint);
            }

            // ustawienie brzegów linii
            UpdateCaps();
        }

        /// <summary>
        /// Metoda ustawiająca początek linii
        /// </summary>
        /// <param name="p">Punkt, w którym linia ma się rozpoczynać</param>
        public void UpdateStart(Point p)
        {
            StartPoint = p;
            IsVisible = false;
            _isStartSet = true;
        }

        /// <summary>
        /// Metoda ustawiająca końcówkę linii
        /// </summary>
        /// <param name="p">Punkt, w którym linia ma się kończyć</param>
        public void UpdateEnd(Point p)
        {
            UpdateLine(p);
            _isStartSet = false;
        }

        /// <summary>
        /// Metoda ustawiająca brzegi linii tak, by były pod kątem prostym do linii
        /// </summary>
        private void UpdateCaps()
        {
            double dx = StartPoint.X - EndingPoint.X;
            double dy = StartPoint.Y - EndingPoint.Y;
            double len = Math.Sqrt(dx * dx + dy * dy);

            // jeśli linia jest zbyt krótka nie ma bzegów
            if (Math.Abs(dx) < 3 && Math.Abs(dy) < 3) {
                CapStart1 = StartPoint;
                CapStart2 = StartPoint;
                CapEnd1 = EndingPoint;
                CapEnd2 = EndingPoint;
                return;
            }

            // jeśli linia jest inna to brzegi ustawiamy pod kątem prostym do linii
            double sin = _capLength * dx / len;
            double cos = _capLength * dy / len; 
            CapStart1 = new Point(StartPoint.X - cos, StartPoint.Y + sin);
            CapStart2 = new Point(StartPoint.X + cos, StartPoint.Y - sin);
            CapEnd1 = new Point(EndingPoint.X - cos, EndingPoint.Y + sin);
            CapEnd2 = new Point(EndingPoint.X + cos, EndingPoint.Y - sin);
        }

        /// <summary>
        /// Ustawienie linii na widoczną
        /// </summary>
        public void Show()
        {
            IsVisible = true;
        }

        /// <summary>
        /// Ukrycie linii
        /// </summary>
        public void Hide()
        {
            IsVisible = false;
        }

        /// <summary>
        /// Metoda zwracająca koordynaty początku i końca linii
        /// </summary>
        /// <returns>Lista 4 wartości double przedstawiających koordynaty
        /// linii względem koordynatów okna - wartości od 0 do 1</returns>
        public List<double> GetChosenPoints()
        {
            return [
                (double)StartPoint.X / (double)CanvasWidth,
                (double)StartPoint.Y / (double)CanvasHeight,
                (double)EndingPoint.X / (double)CanvasWidth,
                (double)EndingPoint.Y / (double)CanvasHeight
            ];
        }
    }
}
