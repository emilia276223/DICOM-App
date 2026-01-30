using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia;
using Avalonia.Controls;

namespace DICOMApp.ViewModel
{
    /// <summary>
    /// Klasa modelująca punkt rysowany jako X
    /// </summary>
    public partial class XPointViewModel : ViewModelBase
    {

        /// <summary>
        /// Punkty, które wyznaczają, końce linii rysujących X
        /// </summary>
        [ObservableProperty] private Point _start1, _end1, _start2, _end2;

        /// <summary>
        /// Punkt, w którym znajduje się X (środek)
        /// </summary>
        internal Point _place;

        /// <summary>
        /// Czy X jest widoczny
        /// </summary>
        [ObservableProperty] private bool _isVisible = true;

        /// <summary>
        /// Kolor punktu X
        /// </summary>
        [ObservableProperty] private string _color;

        /// <summary>
        /// Długość ramion X
        /// </summary>
        private int _size = 4;

        /// <summary>
        /// Konstruktor klasy
        /// </summary>
        /// <param name="place">Miejsce, w którym znajduje się X</param>
        /// <param name="color">Kolor</param>
        public XPointViewModel(Point place, string color)
        {
            UpdatePosition(place);
            _color = color;
        }

        /// <summary>
        /// Metoda zmieniająca punkt położenia X
        /// </summary>
        /// <param name="place">Punkt, w którym ma się znaleźć X</param>
        public void UpdatePosition(Point place)
        {
            _place = place;

            Start1 = new Point(place.X - _size, place.Y - _size);
            End1 = new Point(place.X + _size, place.Y + _size);

            Start2 = new Point(place.X - _size, place.Y + _size);
            End2 = new Point(place.X + _size, place.Y - _size);
        }
    }
}
