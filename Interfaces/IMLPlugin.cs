using DICOMApp.Data;

namespace DICOMApp.Interfaces
{

    /// <summary>
    /// Interfejs dla klas umożliwiających predykcję na podstawie modelu AI
    /// </summary>
    public interface IMLPlugin
    {
        /// <summary>
        /// Wyliczenie predukcji na podstawie obrazu
        /// </summary>
        /// <param name="imageData">Obraz jako ImageData</param>
        /// <returns>MLModelPrediction zawierający wynik predykcji</returns>
        public MLModelPrediction GetPredictionFromImageData(ImageData imageData);

        /// <summary>
        /// Ponowne załadowanie modelu z podanego pliku
        /// </summary>
        /// <param name="path">Ścieżka do pliku z modelem</param>
        void ReloadModel(string path);
    }
}
