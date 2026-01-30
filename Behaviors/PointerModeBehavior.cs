using Avalonia;
using DICOMApp.Interfaces;

namespace DICOMApp.Behaviors
{
    /// <summary>
    /// Behavior obsługujący rozróżnianie trybów wskaźnika pomiędzy rysowaniem linii a lupą
    /// </summary>
    public partial class PointerModeBehavior
    {

        /// <summary>
        /// Specyfikuje dostępne tryby wskaźnika
        /// </summary>
        internal enum Options
        {
            Line,
            Magnifying
        }

        private Options _selectedOption;

        private ILineViewModel[] _lines;
        private IMagnifiedImageViewModel _magnifiedImageVM;
        private bool _isMagnifyingWindowMoving;
        private int _lineToDraw = 0;


        /// <summary>
        /// Tworzy behavior obsługujący rozróżnianie trybów wskaźnika pomiędzy rysowaniem linii a lupą
        /// </summary>
        /// <param name="magnifiedImageVM">Objekt okna powiększającego</param>
        /// <param name="lineVM1">Objekt pierwszej linii na oryginalnym obrazie</param>
        /// <param name="lineVM2">Objekt drugiej linii na oryginalnym obrazie</param>
        public PointerModeBehavior(IMagnifiedImageViewModel magnifiedImageVM, ILineViewModel lineVM1, ILineViewModel lineVM2) 
        {
            _lines = new ILineViewModel[] { lineVM1, lineVM2 };
            _magnifiedImageVM = magnifiedImageVM;
            _isMagnifyingWindowMoving = false;

            LineMode();
        }


        /// <summary>
        /// Obsługuje zdarzenie naciśnięcia wskaźnika
        /// </summary>
        /// <param name="p">Pozycja wskaźnika</param>
        public void OnPointerPressed(Point p)
        {
            switch (_selectedOption)
            {
                case Options.Line:
                    _lines[_lineToDraw].UpdateStart(p);
                    break;

                case Options.Magnifying:
                    _isMagnifyingWindowMoving = true;
                    break;
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie zwolnienia wskaźnika
        /// </summary>
        /// <param name="p">Pozycja wskaźnika</param>
        public void OnPointerReleased(Point p)
        {
            switch (_selectedOption)
            {
                case Options.Line:
                    _lines[_lineToDraw].UpdateEnd(p);
                    _lineToDraw = 1 - _lineToDraw; // zmiana rysowanej linii
                    break;
                case Options.Magnifying:
                    _isMagnifyingWindowMoving = false;
                    break;
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie ruchu wskaźnika
        /// </summary>
        /// <param name="p">Pozycja wskaźnika</param>
        public void OnPointerMoved(Point p)
        {
            switch (_selectedOption)
            {
                case Options.Line:
                    _lines[_lineToDraw].UpdateLine(p);
                    break;
                case Options.Magnifying:
                    if (_isMagnifyingWindowMoving)
                        _magnifiedImageVM.UpdateFrame(p);
                    break;
            }
        }

        /// <summary>
        /// Aktywuje tryb rysowania linii
        /// </summary>
        public void LineMode()
        {
            _selectedOption = Options.Line;
            _magnifiedImageVM.Hide();
            _lineToDraw = 0;
        }

        /// <summary>
        /// Aktywuje tryb lupy
        /// </summary>
        public void MagnifyingMode()
        {
            _selectedOption = Options.Magnifying;
            _magnifiedImageVM.Show();
        } 
    }
}
