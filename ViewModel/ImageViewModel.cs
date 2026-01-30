using Avalonia;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using DICOMApp.Data;
using DICOMApp.DICOM;
using System;
using System.Security.Cryptography;
using System.Text;


namespace DICOMApp.ViewModel
{
    /// <summary>
    /// Klasa modelująca widok obrazu
    /// </summary>
    public partial class ImageViewModel : ObservableObject
    {
        /// <summary>
        /// Szerokość wyświetlanego obrazu
        /// </summary>
        [ObservableProperty] private int imageWidth = 500;

        /// <summary>
        /// Wysokość wyświetlanego obrazu
        /// </summary>
        [ObservableProperty] private int imageHeight = 0;

        /// <summary>
        /// Skala wyświetlanego obrazu
        /// </summary>
        internal double _image_scale = 0;

        /// <summary>
        /// Wyświetlany obraz - Avalonia Bitmap
        /// </summary>
        [ObservableProperty] private Bitmap? _bitmap;

        /// <summary>
        /// ImageData reprezentujące wyświetlany obraz
        /// </summary>
        internal ImageData? _image;

        /// <summary>
        /// Konstruktor obiektu ImageViewModel
        /// </summary>
        public ImageViewModel(){}

        /// <summary>
        /// Metoda ustawiająca obraz z pliku DICOM
        /// i wyświetlająca go (tworząca Bitmapę do wyświetlenia)
        /// </summary>
        /// <param name="image">Obraz ImageData z pliku DICOM</param>
        /// <exception cref="Exception">Wyrzucane gdy plik nie jest w formacie DICOM</exception>
        internal void UpdateImage(ImageData image)
        {
            // zapisanie obrazu
            _image = image;

            // stworzenie z ImageData Bitmapy do wyświetlenia
            Bitmap = image.GetBitmap();

            // ustalenie wysokości wyświeltanego obrazu
            ImageHeight = (int)((Bitmap.Size.Height / Bitmap.Size.Width) * (float)ImageWidth);

            // ustalenie skali wyświetlanego obrazu względem ImageData
            _image_scale = (double)image.Width / (double)ImageWidth;
        }
    }
}
