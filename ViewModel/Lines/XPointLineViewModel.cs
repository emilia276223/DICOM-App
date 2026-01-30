using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using DICOMApp.Interfaces;

namespace DICOMApp.ViewModel
{
    public partial class XPointLineViewModel : LineViewModelBase, ILineViewModel
    {
        /// <summary>
        /// Punkt początkowy i końcowy linii
        /// </summary>
        [ObservableProperty] private XPointViewModel _start, _end;

        /// <summary>
        /// Konstruktor klasy
        /// </summary>
        /// <param name="color">Kolor linii</param>
        public XPointLineViewModel(string color)
        {
            Color = color;
            _start = new XPointViewModel(new Point(0, 0), color);
            _end = new XPointViewModel(new Point(0, 0), color);
            Show();
        }

        /// <summary>
        /// Metoda ustawiająca początek i koniec linii
        /// </summary>
        /// <param name="start">Punkt początkowy linii</param>
        /// <param name="end">Punkt końcowy linii</param>
        public void SetLine(Point start, Point end)
        {
            Start.UpdatePosition(start);
            End.UpdatePosition(end);
            UpdateLength(Start._place, End._place);
        }

        private bool _isLineBeingDrawn = false;

        /// <summary>
        /// Metoda ustawiająca początek linii
        /// </summary>
        /// <param name="p">Punkt, w którym linia ma się rozpoczynać</param>
        public void UpdateStart(Point p)
        {
            Start.UpdatePosition(p);
            _isLineBeingDrawn = true;
        }

        /// <summary>
        /// Metoda ustawiająca końcówkę linii
        /// </summary>
        /// <param name="p">Punkt, w którym linia ma się kończyć</param>
        public void UpdateEnd(Point p)
        {
            End.UpdatePosition(p);
            _isLineBeingDrawn = false;
        }

        /// <summary>
        /// Metoda zmieniająca aktualny koniec linii
        /// </summary>
        /// <param name="p">Punkt, w którym linia ma się kończyć</param>
        public void UpdateLine(Point p)
        {
            if (!_isLineBeingDrawn) return;
            End.UpdatePosition(p);
            UpdateLength(Start._place, End._place);
        }

        /// <summary>
        /// Metoda pokazująca linię
        /// </summary>
        public void Show()
        {
            IsVisible = true;
            Start.IsVisible = true;
            End.IsVisible = true;
        }

        /// <summary>
        /// Metoda ukrywająca linię
        /// </summary>
        public void Hide()
        {
            IsVisible = false;
            Start.IsVisible = false;
            End.IsVisible = false;
        }

        /// <summary>
        /// Metoda zwracająca odpowiednie wartości reprezentujące koordynaty początku i końca linii
        /// </summary>
        /// <returns>Listę 4 wartości double przedstawiających koordynaty linii</returns>
        public List<double> GetChosenPoints()
        {
            return [
                (double)Start._place.X / (double)CanvasWidth,
                (double)Start._place.Y / (double)CanvasHeight,
                (double)End._place.X / (double)CanvasWidth,
                (double)End._place.Y / (double)CanvasHeight
            ];
        }
    }
}
