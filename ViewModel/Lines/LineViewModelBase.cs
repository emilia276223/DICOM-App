using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using DICOMApp.DICOM;
using DICOMApp.Interfaces;

namespace DICOMApp.ViewModel
{
    

    /// <summary>
    /// Abstrakcyjna klasa bazowa modelująca linie
    /// </summary>
    public abstract partial class LineViewModelBase : ViewModelBase
    {

        /// <summary>
        /// Szerokość i wysokość obrazu, na którym linia jest rysowana
        /// </summary>
        [ObservableProperty] private int _canvasWidth, _canvasHeight;

        /// <summary>
        /// Długosc fizyczna linii
        /// </summary>
        [ObservableProperty] private double _physicalLength;

        /// <summary>
        /// Widoczność linii
        /// </summary>
        [ObservableProperty] private bool _isVisible;

        /// <summary>
        /// Kolor linii
        /// </summary>
        [ObservableProperty] private string _color;

        // dane do liczenia długości linii
        private IDicomScaler _scaler;
        private double _imageScale;

        /// <summary>
        /// Metoda ustawiająca skaler dla linii
        /// </summary>
        /// <param name="scaler">Skaler typu IDicomScaler, umożliwia obliczenie fizycznej długosci linii</param>
        /// <param name="img_scale">Skala obrazu, na którym linia jest rysowana</param>
        /// <param name="width">Szerość obrazu</param>
        /// <param name="height">Wysokość obrazu</param>
        public void UpdateScaler(IDicomScaler scaler, double img_scale, int width, int height)
        {
            _scaler = scaler;
            _imageScale = img_scale;
            CanvasWidth = width;
            CanvasHeight = height;
        }

        /// <summary>
        /// Metoda ustawiająca długosc fizyczna linii
        /// Na podstawie podanego wcześniej skalera i skali obrazu
        /// </summary>
        /// <param name="start">Początek linii</param>
        /// <param name="end">Końiec linii</param>
        internal void UpdateLength(Point start, Point end)
        {
            PhysicalLength = Math.Round(_scaler.GetActualLength(start * _imageScale, end * _imageScale), 2);
        }
    }
}
