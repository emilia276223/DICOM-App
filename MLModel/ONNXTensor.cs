using DICOMApp.Data;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Collections.Generic;

namespace DICOMApp.MLModel
{
    /// <summary>
    /// Klasa obsługująca przetwarzanie danych dla modelu ONNX
    /// Z ImageData na Tensor oraz Tensor na ImageData
    /// </summary>
    public class ONNXTensor
    {
        /// <summary>
        /// Konstruktor klasy
        /// </summary>
        public ONNXTensor() { }

        /// <summary>
        /// Metoda zwracająca tensor odpowiedni dla modelu na ImageData
        /// </summary>
        /// <param name="imageData">Obraz, króry jest przetwarzany na tensor</param>
        /// <returns>
        /// Krotka zawierająca tensor oraz szerokość i wysokość obrazu (a zarazem tensora)
        /// </returns>
        internal (DenseTensor<float> tensor, int width, int height) ImageDataToTensor(ImageData imageData)
        {
            int width = imageData.Width;
            int height = imageData.Height;

            var tensor = new DenseTensor<float>(
                imageData.GetFloatPixels(),
                new[] { 1, height, width, 1 }
            );

            return (tensor, width, height);
        }

        /// <summary>
        /// Metoda zwracająca obrazy ImageData utworzone na podstawie tensora zwróconego przez model
        /// </summary>
        /// <param name="tensor">Tensor zawierający predykcję heatmap modelu</param>
        /// <returns>Listę obrazów heatmap (4 obrazy) w postaci ImageData</returns>
        internal List<ImageData> TensorsToImageData(Tensor<float> tensor)
        {
            var width = tensor.Dimensions[2];
            var height = tensor.Dimensions[1];

            List<ImageData> images = new List<ImageData>();
            for (int c = 0; c < tensor.Dimensions[3]; c++)
            {
                var pixels = new float[width * height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixels[y * width + x] = tensor[0, y, x, c] * 100;
                    }
                }

                images.Add(new ImageData(width, height, pixels));
            }
            return images;
        }

       
    }
}
