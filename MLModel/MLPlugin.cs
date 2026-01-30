using DICOMApp.Data;
using DICOMApp.Interfaces;
using DICOMApp.Utils;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DICOMApp.MLModel
{

    /// <summary>
    /// Klasa umożliwiająca korzystanie z modelu zapisanego w pliku ONNX
    /// </summary>
    public class MLPlugin : IMLPlugin
    {
        private string _modelPath;
        private InferenceSession _session;
        private string _input_name;
        private ONNXTensor _onnxTensor;


        /// <summary>
        /// Stworzenie instancji ML_Plugin
        /// </summary>
        public MLPlugin()
        {
            // prepare place to save model / get model from
            _createModelFile();
            _onnxTensor = new ONNXTensor();
            _loadModel();
        }

        /// <summary>
        /// Stworzenie pliku do zapisania modelu użuywanego przez program
        /// </summary>
        private void _createModelFile()
        {
            _modelPath = Path.Combine(
                AppDirectoryHandler.Instance.GetONNXModelDirectory(), 
                "model.onnx");

            // wgranie modelu domyślnego jeśli nie został wybrany inny
            if (!File.Exists(_modelPath))
            {
                File.Copy("Assets/unet_keypoints.onnx", _modelPath, true);
            }
        }

        /// <summary>
        /// Załadowanie modelu
        /// </summary>
        private void _loadModel()
        {
            // otworzenie sesji i wczytanie inputu
            _session = new InferenceSession(_modelPath);
            _input_name = _session.InputMetadata.First().Key;
        }


        /// <inheritdoc cref="IMLPlugin.ReloadModel(string)"/>
        public void ReloadModel(string path)
        {
            File.Copy(path, _modelPath, true);
            _loadModel();
        }

        private Tensor<float> GetHeatmapsPrediction(ImageData image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image), "Input image cannot be null.");
            }

            if (string.IsNullOrEmpty(_modelPath))
            {
                throw new InvalidOperationException("Model path is not set. Please set the model path before prediction.");
            }

            var (tensor, width, height) = _onnxTensor.ImageDataToTensor(image);

            List<NamedOnnxValue> inputs = [NamedOnnxValue.CreateFromTensor(_input_name, tensor)];

            using var results = _session.Run(inputs);

            var output = results.First().AsTensor<float>();

            return output;
        }

        private List<ImageData> ImageDataFromTensor(Tensor<float> output)
        {
            return _onnxTensor.TensorsToImageData(output);
        }
        

        /// <summary>
        /// Wyliczenie 4 punktów - maximów z przewidzianych heatmap
        /// </summary>
        /// <param name="output">Tensor zawierający dane heatmap</param>
        /// <param name="width">Szerokość każdej z heatmap</param>
        /// <param name="height">Wysokość każdej z heatmap</param>
        /// <returns>Przewidziane 4 punkty - każdy z nich w miejscu z wyliczonym największym ppb dla tego punktu</returns>
        private Avalonia.Point[] ExtractPointsFromHeatmaps(Tensor<float> output, int width, int height)
            {
                var points = new Avalonia.Point[4];

                // przejście przez wszystkie heatmapy
                for (int channel = 0; channel < 4; channel++)
                {
                    float maxVal = float.MinValue;
                    int bestX = 0;
                    int bestY = 0;

                    // znalezienie punktu o najwyższej wartości
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            float value = output[0, y, x, channel];

                            if (value > maxVal)
                            {
                                maxVal = value;
                                bestX = x;
                                bestY = y;
                            }
                        }
                    }

                    points[channel] = new Avalonia.Point(bestX, bestY);
                }
                return points;
            }

        /// <inheritdoc cref="IMLPlugin.GetPredictionFromImageData(ImageData)"/>
        public MLModelPrediction GetPredictionFromImageData(ImageData imageData)
        {
            var output = GetHeatmapsPrediction(imageData);
            var heatmaps = ImageDataFromTensor(output);
            var points = ExtractPointsFromHeatmaps(output, heatmaps[0].Width, heatmaps[0].Height);
            return new MLModelPrediction(
                points,
                heatmaps
                );
        }
    }
}
