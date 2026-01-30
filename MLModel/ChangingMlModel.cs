using DICOMApp.Data;
using DICOMApp.Interfaces;

namespace DICOMApp.MLModel
{
    /// <summary>
    /// Klasa umożliwiająca zmiane modelu w czasie działania programu
    /// </summary>
    internal class ChangingMlModel : IMLPlugin
    {
        /// <summary>
        /// Instancja modelu
        /// </summary>
        private IMLPlugin _instance = new MLModelMock();

        /// <inheritdoc cref="IMLPlugin.GetPredictionFromImageData(ImageData)"/>
        /// <remarks>Korzysta z modelu, który jest aktualnie zainstalowany</remarks>
        public MLModelPrediction GetPredictionFromImageData(ImageData imageData)
        {
            return _instance.GetPredictionFromImageData(imageData);
        }

        /// <inheritdoc cref="IMLPlugin.ReloadModel(string)"/>
        /// <remarks>
        /// Ustawia nowy model:
        ///     * jeśli ścieżka do pliku jest pusta nic nie zostaje zmienione
        ///     * wpp jeśli do tej pory był model Mock zostaje zmieniony na Plugin z załadowanym modelem z pliku
        ///     * jeśli do tej pory był model Plugin to jest on załadowany ponownie
        /// </remarks>
        public void ReloadModel(string path)
        {
            if (path != null)
            {
                if (_instance is MLPlugin)
                {
                    _instance.ReloadModel(path);
                }
                else {
                    _instance = new MLPlugin();
                    _instance.ReloadModel(path);
                }
            }
        }
    }
}
