using Avalonia;
using DICOMApp.Data;
using System;
using System.Collections.Generic;
using DICOMApp.Interfaces;

namespace DICOMApp.MLModel
{
    /// <summary>
    /// Klasa mockująca model ML. Zwraca losowe punkty i heatmapy
    /// </summary>
    public class MLModelMock : IMLPlugin
    {
        private Random _rand = new Random();

        private List<ImageData> GetHeatmapsFromImageData(ImageData imageData)
        {
            return new List<ImageData>
            {
                imageData.Copy(),
                imageData.Copy(),
                imageData.Copy(),
                imageData.Copy()
            };
        }
        private Point[] PredictFromImageData(ImageData imageData)
        {
            var min = 0.3;
            var max = 0.7;
            var values = new double[8];
            for (var i = 0; i < 8; i += 2)
            {
                values[i] = (min + _rand.NextDouble() * (max - min)) * imageData.Width;
                values[i + 1] = (min + _rand.NextDouble() * (max - min)) * imageData.Height;
            }

            return new Point[]
            {
                new Point(values[0], values[1]),
                new Point(values[2], values[3]),
                new Point(values[4], values[5]),
                new Point(values[6], values[7])
            };
        }

        /// <inheritdoc cref="IMLPlugin.GetPredictionFromImageData(ImageData)"/>
        /// <remarks>
        /// Heatmapy to kopie obrazu
        /// Przewidziane punkty są losowe (ale w miarę ze środka obrazu)
        /// </remarks>
        public MLModelPrediction GetPredictionFromImageData(ImageData imageData)
        {
            return new MLModelPrediction(
                PredictFromImageData(imageData),
                GetHeatmapsFromImageData(imageData)
                );
        }

        /// <inheritdoc cref="IMLPlugin.ReloadModel(string)"/>
        /// <remarks>
        /// w tym przypadku nic nie robi
        /// </remarks>
        public void ReloadModel(string path)
        {
            return;
        }
    }
}
