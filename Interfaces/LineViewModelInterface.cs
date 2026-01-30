using System.Collections.Generic;
using Avalonia;

namespace DICOMApp.Interfaces
{
    /// <summary>
    /// Interfejs modelujący linie
    /// </summary>
    public interface ILineViewModel
    {
        /// <summary>
        /// Czy linia jest widoczna
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Długosc fizyczna linii
        /// </summary>
        public double PhysicalLength { get; }

        /// <summary>
        /// Kolor linii
        /// </summary>
        public string Color { get; }

        /// <summary>
        /// Metoda ustawiająca początek i koniec linii
        /// </summary>
        /// <param name="start">Punkt ustawiany jako początek linii</param>
        /// <param name="end">Punkt ustawiany jako koniec linii</param>
        public void SetLine(Point start, Point end);

        /// <summary>
        /// Metoda ustawiająca początek linii
        /// </summary>
        /// <param name="p">Punkt ustawiany jako początek linii</param>
        public void UpdateStart(Point p);

        /// <summary>
        /// Metoda ustawiająca końcówkę linii
        /// </summary>
        /// <param name="p">Punkty ustawiany jako koniec linii</param>
        public void UpdateEnd(Point p);

        /// <summary>
        /// Metoda ustawiająca linię na podany koniec
        /// </summary>
        /// <param name="p">Punkt ustawiany jako koniec linii</param>
        public void UpdateLine(Point p);

        /// <summary>
        /// Metoda ustawiająca skaler dla linii 
        /// i wymiary obrazu, na którym linia jest rysowana
        /// </summary>
        /// <param name="scaler">Skaler typu IDicomScaler, który umożliwia obliczenie fizycznej długosci linii</param>
        /// <param name="img_scale">Skala obrazu, na którym linia jest rysowana</param>
        /// <param name="width">Szerość obrazu</param>
        /// <param name="height">Wysokość obrazu</param>
        public void UpdateScaler(IDicomScaler scaler, double img_scale, int width, int height);

        /// <summary>
        /// Pokazanie linii
        /// </summary>
        public void Show();

        /// <summary>
        /// Ukrycie linii
        /// </summary>
        public void Hide();

        /// <summary>
        /// Metoda zwracająca punkty zaznaczonych linii
        /// Jako wartości od 0 do 1 reprezentujące koordynaty początku i końca linii
        /// względem koordynatów okna
        /// </summary>
        /// <returns></returns>
        public List<double> GetChosenPoints();


    }
}
