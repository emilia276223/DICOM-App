using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using Avalonia.Media.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using DICOMApp.Data;

namespace DICOMApp.ViewModel
{
    public partial class HeatmapViewModel : ViewModelBase
    {
        /// <summary>
        /// Obraz heatmap - Bitmapa
        /// </summary>
        [ObservableProperty]
        private Avalonia.Media.Imaging.Bitmap? _heatmap;

        /// <summary>
        /// Indeks heatmapy
        /// </summary>
        [ObservableProperty] private int _index;

        /// <summary>
        /// Obraz heatmapy jako ImageData
        /// </summary>
        private ImageData _imageData;

        /// <summary>
        /// Akcja wywoływana po wybraniu heatmapy
        /// </summary>
        private readonly Action<int> _onSelectionAction;


        private string _defaultBackgroundColor = "#5A5F67";
        private string _highlightedBackgroundColor = "#2D3A50";

        /// <summary>
        /// Kolor tła ikony heatmapy
        /// </summary>
        [ObservableProperty] private string _backgroundColor;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="index">Indeks heatmapy</param>
        /// <param name="imageData">ImageData obrazu heatmapy</param>
        /// <param name="onSelectionAction">Akcja, która bedzie wywoływana po wybraniu heatmapy</param>
        public HeatmapViewModel(int index, ImageData imageData, Action<int> onSelectionAction)
        {
            _imageData = imageData;
            _heatmap = _imageData.GetBitmap();
            Index = index + 1;
            _onSelectionAction = onSelectionAction;
            BackgroundColor = _defaultBackgroundColor;
        }

        /// <summary>
        /// Metoda ustawiająca aktualnie wyswietlaną heatmapę
        /// </summary>
        public void Show()
        {
            _onSelectionAction(Index - 1);
            BackgroundColor = _highlightedBackgroundColor;
        }

        /// <summary>
        /// Metoda ukrywająca aktualnie wyswietlaną heatmapę
        /// </summary>
        public void Hide()
        {
            BackgroundColor = _defaultBackgroundColor;
        }
    }
}
