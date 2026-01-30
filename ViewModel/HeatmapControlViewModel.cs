using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DICOMApp.Data;

namespace DICOMApp.ViewModel
{
    /// <summary>
    /// Klasa modelująca kontroler heatmap
    /// </summary>
    public partial class HeatmapControlViewModel : ViewModelBase
    {
        /// <summary>
        /// Lista przechowująca obiekty heatmap
        /// </summary>
        [ObservableProperty] private ObservableCollection<HeatmapViewModel> _heatmapVMs;

        /// <summary>
        /// Czy aktualnie wyswietlana jest heatmapa
        /// </summary>
        [ObservableProperty] private bool _isHeatmapVisible;

        /// <summary>
        /// Aktualnie wyswietlana heatmapa
        /// </summary>
        [ObservableProperty]
        private Avalonia.Media.Imaging.Bitmap? _currentHeatmap;

        /// <summary>
        /// Indeks aktualnie wyswietlanej heatmapy
        /// </summary>
        private int _currentIndex = -1;


        /// <summary>
        /// Konstruktor
        /// </summary>
        public HeatmapControlViewModel()
        {
            _heatmapVMs = new ObservableCollection<HeatmapViewModel>();
            _isHeatmapVisible = false;
        }

        /// <summary>
        /// Metoda ustawiająca aktualnie wyswietlaną heatmapę
        /// </summary>
        /// <param name="i">Indeks heatmapy do wyswietlenia</param>
        /// <exception cref="ArgumentException">Wyrzucany, jeśli <paramref name="i"/> jest inny niż możliwy indeks heatmapy</exception>
        internal void ShowHeatmap(int i)
        {
            if (HeatmapVMs.Count < i || i < 0)
            {
                throw new ArgumentException("Heatmap not set");
            }

            // ukrycie poprzedniej heatmapy
            if (_currentIndex != -1)
                HeatmapVMs[_currentIndex].Hide();

            // ustawienie nowej heatmapy
            _currentIndex = i;
            CurrentHeatmap = HeatmapVMs[i].Heatmap;
            IsHeatmapVisible = true;
        }

        /// <summary>
        /// Metoda ukrywająca aktualnie wyswietlaną heatmapę
        /// </summary>
        internal void HideHeatmap()
        {
            IsHeatmapVisible = false;
            if (_currentIndex != -1)
            {
                HeatmapVMs[_currentIndex].Hide();
                _currentIndex = -1;
            }
        }

        /// <summary>
        /// Metoda ustawiająca nowe heatmapy
        /// </summary>
        /// <param name="hm">Lista heatmap w postaci ImageData</param>
        internal void UpdateHeatmaps(List<ImageData> hm)
        {
            HideHeatmap();
            HeatmapVMs.Clear();
            for (int i = 0; i < hm.Count; i++)
            {
                HeatmapVMs.Add(new HeatmapViewModel(
                    i,
                    hm[i],
                    (int x) => { ShowHeatmap(x); }
                    ));
            }
        }
    }
}
