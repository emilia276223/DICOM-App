using System.Collections.Generic;
using Avalonia;

namespace DICOMApp.Data
{
    /// <summary>
    /// Klasa przechowująca wynik predykcji modelu
    /// </summary>
    public class MLModelPrediction
    {
        /// <summary>
        /// Punkty przewidziane przez model
        /// </summary>
        public Point[] Points;
        
        /// <summary>
        /// Heatmapy obrazujące ppb na obrazie dla każdego z przewidzianych punktów
        /// </summary>
        public List<ImageData> Heatmaps;

        /// <summary>
        /// Konstruktor obiektów klasy
        /// </summary>
        /// <param name="points">4 punkty przewidziane przez model</param>
        /// <param name="heatmaps">4 heatmapy obrazujące ppb na obrazie dla każdego z przewidzianych punktów</param>
        public MLModelPrediction(Point[] points, List<ImageData> heatmaps)
        {
            Points = points;
            Heatmaps = heatmaps;
        }
    }
}
