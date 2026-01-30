using Avalonia;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using DICOMApp.Behaviors;
using DICOMApp.Data;
using DICOMApp.Interfaces;
using System;


namespace DICOMApp.ViewModel
{
    /// <summary>
    /// Klasa modelująca widok powiekszenia obrazu
    /// </summary>
    public partial class MagnifiedImageViewModel : ViewModelBase, IMagnifiedImageViewModel
    {
        /// <summary>
        /// Obraz oryginalny jako ImageData
        /// </summary>
        private ImageData _originalImage;

        /// <summary>
        /// Skala oryginalnego obrazu
        /// </summary>
        private double _image_scale;

        /// <summary>
        /// Rozmiar wyświetlanego fragmentu obrazu w pikselach
        /// </summary>
        private int _windowPixelSize = 150;

        /// <summary>
        /// Przesuniecie wycinka obrazu względem oryginalnego obrazu (w pikselach)
        /// </summary>
        private int _startX = 0;
        private int _startY = 0;


        /// <summary>
        /// Czy okno powiekszenia jest widoczne
        /// </summary>
        [ObservableProperty] private bool _isVisible = false;

        /// <summary>
        /// Bitmapa aktualnie pokazywanego wycinka obrazu
        /// </summary>
        [ObservableProperty] private Bitmap _currentImage;

        /// <summary>
        /// Rozmiar aktualnie pokazywanego wycinka obrazu (jak duży jest wyświetlany fragment obrazu)
        /// </summary>
        [ObservableProperty] private int _windowShownSize = 400;

        /// <summary>
        /// Linie wycinka obrazu
        /// </summary>
        [ObservableProperty] private ILineViewModel _line1 = new XPointLineViewModel("red");
        [ObservableProperty] private ILineViewModel _line2 = new XPointLineViewModel("blue");
        //[ObservableProperty] private ILineViewModel _line1 = new LineViewModel("green");
        //[ObservableProperty] private ILineViewModel _line2 = new LineViewModel("pink");


        /// <summary>
        /// Konstruktor
        /// </summary>
        public MagnifiedImageViewModel()
        {
            _line1.Hide();
            _line2.Hide();
        }

        /// <summary>
        /// Metoda zwracająca pozycję środka wycinka obrazu
        /// </summary>
        /// <returns></returns>
        public (double, double) GetCenterOfMagnifiedImage()
        {
            return (
                ((double)_startX + (double)_windowPixelSize / 2) / _originalImage.Width,
                ((double)_startY + (double)_windowPixelSize / 2) / _originalImage.Height);
        }

        /// <summary>
        /// Metoda aktualizowania obrazu, którego wycinek jest przedstawiany
        /// </summary>
        /// <param name="image">ImageData obrazu</param>
        /// <param name="image_scale">Skala obrazu</param>
        /// <param name="scaler">IDicomScaler umożliwiający wyliczenie długosci linii</param>
        internal void UpdateImage(ImageData image, double image_scale, IDicomScaler scaler)
        {
            // zapisanie kopii ImageData
            _originalImage = image.Copy();
            _image_scale = image_scale;

            // update skalowania linii
            UpdateLines(scaler);

            // aktualizacja okna powiększającego
            UpdateFrame(new Point(_startX, _startY));
        }

        /// <summary>
        /// Metoda aktualizowania skali linii
        /// </summary>
        /// <param name="scaler">IDicomScaler umożliwiający wyliczenie długosci linii</param>
        internal void UpdateLines(IDicomScaler scaler)
        {
            // zmiana skalowania linii - do wyliczania ich długości potrzebna jest skala
            var scale = ((double)_windowPixelSize / (double)WindowShownSize);
            Line1.UpdateScaler(scaler, scale, WindowShownSize, WindowShownSize);
            Line2.UpdateScaler(scaler, scale, WindowShownSize, WindowShownSize);
        }

        /// <summary>
        /// Metoda aktualizowania okna powększającego na podstawie pozycji myszki
        /// W efekcie obraz powększający jest wyciętym kwadratem obrazu oryginalnego
        /// Ze środkiem tam, gdzie znajduje sie punkt myszki na obrazie oryginalnym
        /// </summary>
        /// <param name="p">Punkt, w którym jest myszka, względem oryginalnego obrazu</param>
        public void UpdateFrame(Point p)
        {
            // jeśli okno jest ukryte nic nie trzeba robić
            if (!IsVisible)
            {
                return;
            }

            // translacja położenia myszki na pixel
            var x = p.X * _image_scale;
            var y = p.Y * _image_scale;

            // obraz powiększony jest wyciętym kwadratem obrazu oryginalnego o boku długości _windowPixelSize
            // jeśli punkt jest bardzo blisko brzegu wycięcie jest ustawiane do brzegu obrazu (żeby nie wyjść za)
            _startX = (int)Math.Clamp(x - (_windowPixelSize / 2), 0, _originalImage.Width - _windowPixelSize);
            _startY = (int)Math.Clamp(y - (_windowPixelSize / 2), 0, _originalImage.Height - _windowPixelSize);

            // tworzenie nowego ImageData dla wycinka obrazu
            int[] pixels = new int[_windowPixelSize * _windowPixelSize];

            for (int i = 0; i < _windowPixelSize; i++)
            {
                for (int j = 0; j < _windowPixelSize; j++)
                {
                    pixels[i + j * _windowPixelSize] = _originalImage.Pixels[(_startX + i) + (_startY + j) * _originalImage.Width];
                }
            }

            var currentFrame = new ImageData(_windowPixelSize, _windowPixelSize, pixels);

            // stworzenie Bitmapy, która będzie wyświetlana
            CurrentImage = currentFrame.GetBitmap();
        }

        /// <summary>
        /// Metoda przetwarzająca położenie na obrazie powiększającym na położenie na obrazie oryginalnym
        /// </summary>
        /// <param name="p">Punkt względem obrazu powiększonego</param>
        /// <returns>Punkt względem oryginalnego obrazu</returns>
        public Point TranslateFromMagnifiedToOriginal(Point p)
        {
            // zmiana z położenia na wyświetleniu na położenie pixeli na oryginalnym obrazie
            double scaleFactor = (double)_windowPixelSize / (double)WindowShownSize;
            int originalX = _startX + (int)(p.X * scaleFactor);
            int originalY = _startY + (int)(p.Y * scaleFactor);

            // zmiana na położenie wyświetlenia na oryginalnym obrazie
            return new Point(originalX / _image_scale, originalY / _image_scale);
        }

        /// <summary>
        /// Pokazanie okna powększającego
        /// </summary>
        public void Show()
        {
            IsVisible = true;
            if (CurrentImage == null) UpdateFrame(new Point(0, 0));
            Line1.Show();
            Line2.Show();
        }

        /// <summary>
        /// Ukrycie okna powększającego
        /// </summary>
        public void Hide()
        {
            IsVisible = false;
            Line1.Hide();
            Line2.Hide();
        }
    }
}
