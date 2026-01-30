using Avalonia;
using DICOMApp.Interfaces;

namespace DICOMApp.Behaviors
{
    /// <summary>
    /// Interfejs okna powększającego z liniami powększonymi
    /// Stworzony na potrzeby oddzielenia trochę implementacji od LineBehavior
    /// </summary>
    public interface IMagnifiedImageViewModel
    {
        internal Point TranslateFromMagnifiedToOriginal(Point p);
        internal ILineViewModel Line1 { get; set; }
        internal ILineViewModel Line2 { get; set; }

        internal void Show();
        internal void Hide();
        internal void UpdateFrame(Point p);
    }



    /// <summary>
    /// Behavior synchronizujący linie pomiędzy oryginalnym obrazem a obrazem powiększonym
    /// </summary>
    public class LinesBehavior
    {
        private int _settingLine = 0;

        /// <summary>
        /// Obiekt linii pierwszej na oryginalnym obrazie
        /// </summary>
        private ILineViewModel _originalLine1;

        /// <summary>
        /// Obiekt linii drugiej na oryginalnym obrazie
        /// </summary>
        private ILineViewModel _originalLine2;

        /// <summary>
        /// Objekt okna powększającego z liniami powększonymi
        /// </summary>
        private IMagnifiedImageViewModel _magnifiedImageViewModel;


        /// <summary>
        /// Tworzy behavior synchronizujący linie pomiędzy oryginalnym obrazem a obrazem powiększonym
        /// </summary>
        /// <param name="originalLine1">Obiekt linii pierwszej na oryginalnym obrazie</param>
        /// <param name="originalLine2">Obiekt linii drugiej na oryginalnym obrazie</param>
        /// <param name="magnifiedImageViewModel">Objekt okna powiększającego z liniami powiększonymi</param>
        public LinesBehavior(ILineViewModel originalLine1,
                                ILineViewModel originalLine2,
                                IMagnifiedImageViewModel magnifiedImageViewModel) { 
            _originalLine1 = originalLine1;
            _originalLine2 = originalLine2;
            _magnifiedImageViewModel = magnifiedImageViewModel;
        }

        /// <summary>
        /// Aktualizuje koniec linii, która ma być aktualizowana
        /// </summary>
        /// <param name="p">Nowy punkt, który będzie końcem danej linii, podany w koordynatach okna powiększającego</param>
        internal void UpdateLine(Point p)
        {
            if (_settingLine == 0)
            {
                _magnifiedImageViewModel.Line1.UpdateLine(p);
                _originalLine1.UpdateLine(_magnifiedImageViewModel.TranslateFromMagnifiedToOriginal(p));
            }
            else
            {
                _magnifiedImageViewModel.Line2.UpdateLine(p);
                _originalLine2.UpdateLine(_magnifiedImageViewModel.TranslateFromMagnifiedToOriginal(p));
            }
        }


        /// <summary>
        /// Zmienia aktualnie ustawianą linię
        /// </summary>
        /// <param name="p">Nowy punkt startowy, podany w koordynatach okna powiększającego</param>
        internal void UpdateStart(Point p)
        {
            if (_settingLine == 0)
            {
                _magnifiedImageViewModel.Line1.UpdateStart(p);
                _originalLine1.UpdateStart(_magnifiedImageViewModel.TranslateFromMagnifiedToOriginal(p));
            }
            else
            {
                _magnifiedImageViewModel.Line2.UpdateStart(p);
                _originalLine2.UpdateStart(_magnifiedImageViewModel.TranslateFromMagnifiedToOriginal(p));

            }
        }

        /// <summary>
        /// Aktualizuje punkt końcowy aktualnie ustawianej linii
        /// </summary>
        /// <param name="p">Nowy punkt końcowy, podany w koordynatach okna powiększającego</param>
        internal void UpdateEnd(Point p) {
            if (_settingLine == 0) {
                _magnifiedImageViewModel.Line1.UpdateEnd(p);
                _originalLine1.UpdateEnd(_magnifiedImageViewModel.TranslateFromMagnifiedToOriginal(p));
                _settingLine = 1;
            }
            else
            {
                _magnifiedImageViewModel.Line2.UpdateEnd(p);
                _originalLine2.UpdateEnd(_magnifiedImageViewModel.TranslateFromMagnifiedToOriginal(p));
                _settingLine = 0;
            }
        }

        /// <summary>
        /// Aktualizuje skaler linii na oryginalnym obrazie
        /// </summary>
        /// <param name="scaler">IDicomScaler</param>
        /// <param name="imgScale">Skala w jakiej wyświetlany jest główny obraz</param>
        /// <param name="width">Szerokość głównego obrazu</param>
        /// <param name="height">Wysokość głównego obrazu</param>
        internal void UpdateScalerForOriginalLines(IDicomScaler scaler, double imgScale, int width, int height)
        {
            _originalLine1.UpdateScaler(scaler, imgScale, width, height);
            _originalLine2.UpdateScaler(scaler, imgScale, width, height);
        }

        /// <summary>
        /// Ustawia linie na głównym obrazie na przewidziane punkty
        /// </summary>
        /// <param name="points">Punkty przewidziane przez model AI</param>
        internal void SetLinesToPrediction(Point[] points)
        {
            // jeśli coś nie tak z predykcją to ignorujemy
            if (points.Length != 4) return;
            _originalLine1.SetLine(points[0], points[1]);
            _originalLine2.SetLine(points[2], points[3]);

            // teraz będziemy czerwoną ustawiać
            _settingLine = 0;
        }

    }
}
